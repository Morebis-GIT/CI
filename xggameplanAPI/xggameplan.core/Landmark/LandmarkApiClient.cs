using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using xggameplan.AuditEvents;
using xggameplan.core.Extensions;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;

namespace xggameplan.core.Landmark
{
    /// <inheritdoc />
    public class LandmarkApiClient : ILandmarkApiClient
    {
        private const int RetentionInDays = 10;
        private const string ServiceRestEndpointAddress = "LandmarkSystemQSSQueues.svc/rest/jobs/";

        /// <summary>
        /// HTTP client with <see cref="IAsyncPolicy{TResult}"/> resilience policies configured.
        /// </summary>
        /// <remarks>
        /// Uses request's <seealso cref="Polly.Context"/> to manage instance specific URI / Content changes during switching between
        /// <see cref="LandmarkApiConfig.Primary"/> and <see cref="LandmarkApiConfig.Secondary"/> landmark instances.
        /// </remarks>
        private readonly HttpClient _policyHttpClient;

        private readonly LandmarkInstanceConfig _instanceConfiguration;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly Uri _serviceUri;

        public LandmarkApiClient(HttpClient policyHttpClient, LandmarkApiConfig apiConfig, IAuditEventRepository auditEventRepository)
        {
            _policyHttpClient = policyHttpClient;
            _instanceConfiguration = apiConfig.DefaultInstance;
            _auditEventRepository = auditEventRepository;

            _serviceUri = new Uri(_instanceConfiguration.BaseUri, ServiceRestEndpointAddress);
        }

        /// <inheritdoc />
        public async Task<LandmarkBookingResponseModel> TriggerRunAsync(LandmarkBookingRequest bookingRequest, ScheduledRunSettingsModel scheduledRunModel = null)
        {
            if (bookingRequest is null)
            {
                throw new ArgumentNullException(nameof(bookingRequest));
            }

            Uri UriBuildAction(LandmarkInstanceConfig config)
            {
                var timezone = config.Timezone;
                if (scheduledRunModel != null && !string.IsNullOrWhiteSpace(timezone) && TimeZoneInfo.GetSystemTimeZones().Any(x => x.StandardName == timezone))
                {
                    var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                    scheduledRunModel.DateTime = TimeZoneInfo.ConvertTimeFromUtc(scheduledRunModel.DateTime, timezoneInfo);
                }

                return MakeRunBookingRequestUri(new Uri(config.BaseUri, ServiceRestEndpointAddress), scheduledRunModel);
            }

            HttpContent ContentBuildAction(LandmarkInstanceConfig config)
            {
                bookingRequest.OrganizationCode ??= config.OrganizationCode;
                bookingRequest.PositionCode ??= config.PositionCode;

                // content must be sent as plain json string
                return new ObjectContent<string>(JsonConvert.SerializeObject(bookingRequest), new JsonMediaTypeFormatter());
            }

            var jsonString = ContentBuildAction(_instanceConfiguration);
            using var request = new HttpRequestMessage(HttpMethod.Post, UriBuildAction(_instanceConfiguration))
            {
                Content = jsonString
            };

            request.SetCustomUriBuildAction(UriBuildAction);
            request.SetCustomContentBuildAction(ContentBuildAction);

            using var response = await _policyHttpClient.SendAsync(request).ConfigureAwait(false);

            RaiseInfo($"Payload to Landmark: {jsonString}");

            var result = new LandmarkBookingResponseModel();

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var responseContent = response.Content != null ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : default;
                var processingId = responseContent != null ? JsonConvert.DeserializeObject<Guid>(responseContent) : default;
                if (processingId == Guid.Empty)
                {
                    throw new InvalidOperationException("Landmark run processing id was not found in response to the booking request");
                }

                result.ProcessingId = processingId;
            }
            else
            {
                var message = response.StatusCode == HttpStatusCode.InternalServerError
                    ? "Error occurred while processing Landmark booking request"
                    : "Unexpected Landmark run booking response status code returned";
                var exception = new InvalidOperationException($"{message}. StatusCode: {response.StatusCode}");
                exception.Data.Add("ResponseContent", await ReadErrorResponseContent(response).ConfigureAwait(false));
                throw exception;
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> CancelRunAsync(Guid processingId)
        {
            var cancelRunUri = new Uri(_serviceUri, $"{processingId}/cancel");

            using (var response = await _policyHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, cancelRunUri)).ConfigureAwait(false))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK: return true;
                    case HttpStatusCode.InternalServerError: return false;
                    default:
                        throw new InvalidOperationException($"LMK cancel run returned invalid status: {response.StatusCode}");
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> ProbeLandmarkAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, _instanceConfiguration.ProbeUri.Value))
            {
                request.SetCustomUriBuildAction(config => config.ProbeUri.Value);
                using (var response = await _policyHttpClient.SendAsync(request).ConfigureAwait(false))
                {
                    return response.IsSuccessStatusCode;
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<LandmarkJobInfo>> GetAllJobsAsync()
        {
            try
            {
                RaiseInfo("Getting all jobs");

                using var response = await _policyHttpClient.GetAsync(new Uri(_serviceUri, "ALL")).ConfigureAwait(false);
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var data = JArray.Parse(responseContent);
                return data.ToObject<List<LandmarkJobInfo>>();
            }
            catch (Exception e)
            {
                RaiseInfo(e.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<LandmarkJobStatusModel> GetRunStatusAsync(Guid processingId)
        {
            var statusUpdateUri = new Uri(_serviceUri, $"{processingId}/status");
            var result = new LandmarkJobStatusModel();
            using var response = await _policyHttpClient.GetAsync(statusUpdateUri).ConfigureAwait(false);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    result.JobStatus = ExternalScenarioStatus.Accepted;
                    break;

                case HttpStatusCode.NotFound:
                    result.JobStatus = ExternalScenarioStatus.NotFound;
                    result.ErrorMessage = "Invalid processing ID supplied or Job has been purged from the reporting table";
                    break;

                case HttpStatusCode.Conflict:
                    result.JobStatus = ExternalScenarioStatus.Conflict;
                    result.ErrorMessage = await ReadErrorResponseContent(response).ConfigureAwait(false);
                    break;

                case HttpStatusCode.OK:
                    result.JobStatus = ExternalScenarioStatus.Completed;
                    if (response.Content != null)
                    {
                        try
                        {
                            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            var untokenizedData = JToken.Parse(data).Value<string>();

                            result.OutputFiles = JToken.Parse(untokenizedData)
                                .ToObject<List<LandmarkOutputFileModel>>();
                        }
                        catch (JsonException e)
                        {
                            throw new InvalidOperationException("Error occurred while deserializing Landmark run result", e);
                        }
                    }
                    break;

                case HttpStatusCode.InternalServerError:
                    result.JobStatus = ExternalScenarioStatus.Error;
                    result.ErrorMessage = $"Error occurred while processing run on Landmark, ProcessingId {processingId}";
                    break;

                default:
                    var message = $"Unexpected status code returned to the Landmark run status update request ProcessingId {processingId}";
                    var exception = new InvalidOperationException($"{message}. StatusCode: {response.StatusCode}");
                    exception.Data.Add("ResponseContent", await ReadErrorResponseContent(response).ConfigureAwait(false));
                    throw exception;
            }

            return result;
        }

        private static async Task<string> ReadErrorResponseContent(HttpResponseMessage response)
        {
            if (response.Content is null)
            {
                return string.Empty;
            }

            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            stream.ReadTimeout = Timeout.Infinite;
            using var errorStreamReader = new StreamReader(stream);
            return errorStreamReader.ReadToEnd();
        }

        private void RaiseInfo(string message) => _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, message));

        private static Uri MakeRunBookingRequestUri(Uri uri, ScheduledRunSettingsModel settings)
        {
            var parameters = new List<string>
            {
                "AutomatedBooking"
            };

            if (!(settings is null))
            {
                parameters.Add(Uri.EscapeDataString(settings.QueueName));
                parameters.Add(settings.Priority.ToString());
                parameters.Add(settings.DateTime.ToString("yyyy-MM-ddTHHmmss"));
                parameters.Add(RetentionInDays.ToString());

                if (!string.IsNullOrWhiteSpace(settings.Comment))
                {
                    parameters.Add(Uri.EscapeDataString(settings.Comment));
                }
            }

            return new Uri(uri, string.Join("/", parameters));
        }
    }
}
