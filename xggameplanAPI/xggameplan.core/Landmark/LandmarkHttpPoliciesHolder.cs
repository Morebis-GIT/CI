using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using xggameplan.core.Exceptions;
using xggameplan.core.Landmark.Abstractions;

namespace xggameplan.core.Landmark
{
    public sealed class LandmarkHttpPoliciesHolder : ILandmarkHttpPoliciesHolder
    {
        private static readonly Func<HttpResponseMessage, bool> _handledHttpStatusCodes = response =>
            response.StatusCode == HttpStatusCode.RequestTimeout ||
            response.StatusCode == HttpStatusCode.BadGateway ||
            response.StatusCode == HttpStatusCode.ServiceUnavailable ||
            response.StatusCode == HttpStatusCode.GatewayTimeout ||
            response.StatusCode == (HttpStatusCode)429; // Too Many Requests

        private long _landmarkNotAvailable;

        public LandmarkHttpPoliciesHolder(LandmarkApiConfig apiConfig, LandmarkHttpPolicyOptions httpPolicyOptions)
        {
            HttpFallbackPolicy = Policy.Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .Or<BrokenCircuitException>()
                .OrResult(_handledHttpStatusCodes)
                .FallbackAsync(fallbackValue: null, onFallbackAsync: OnFallbackAsync);

            HttpCircuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .OrResult(_handledHttpStatusCodes)
                .CircuitBreakerAsync(1, httpPolicyOptions.CircuitBreakDuration,
                    onBreak: (result, duration, context) => context["BlockedTill"] = DateTime.UtcNow.Add(duration),
                    onReset: context => context.Remove("BlockedTill"));

            GetRetryPolicyAction = request => RetryPolicyBuilder(request, apiConfig, httpPolicyOptions.MaxAttemptsPerInstance);

            HttpTimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(httpPolicyOptions.TimeoutPerRequest);
        }

        public IAsyncPolicy<HttpResponseMessage> HttpFallbackPolicy { get; }
        public IAsyncPolicy<HttpResponseMessage> HttpCircuitBreakerPolicy { get; }
        public IAsyncPolicy<HttpResponseMessage> HttpTimeoutPolicy { get; }
        public Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> GetRetryPolicyAction { get; }

        /// <summary>
        /// Gets or sets a value indicating whether landmark is available, in a thread safe manner.
        /// </summary>
        /// <value>
        ///   <c>true</c> if landmark is not available; otherwise, <c>false</c>.
        /// </value>
        public bool LandmarkIsNotAvailable
        {
            get => Interlocked.Read(ref _landmarkNotAvailable) == 1;
            set => _ = value
                ? Interlocked.CompareExchange(ref _landmarkNotAvailable, 1, 0)
                : Interlocked.CompareExchange(ref _landmarkNotAvailable, 0, 1);
        }

        /// <summary>
        /// Configures LMK request headers.
        /// </summary>
        /// <param name="headersCollection">Request headers collection.</param>
        /// <param name="instanceConfig">LMK instance configuration.</param>
        public static void ConfigureLmkHeaders(HttpRequestHeaders headersCollection, LandmarkInstanceConfig instanceConfig)
        {
            _ = headersCollection.Remove("LMK-Environment");
            headersCollection.Authorization = new AuthenticationHeaderValue("Basic", instanceConfig.LmkApiKey);
            headersCollection.Add("LMK-Environment", instanceConfig.LmkEnvironment);
        }

        /// <summary>
        /// Builds LMK request retry policy.
        /// </summary>
        /// <param name="request">Request instance.</param>
        /// <param name="apiConfig">LMK API configuration.</param>
        /// <param name="maxRetriesPerInstance">Maximum retries per LMK instance.</param>
        /// <returns></returns>
        private static IAsyncPolicy<HttpResponseMessage> RetryPolicyBuilder(HttpRequestMessage request, LandmarkApiConfig apiConfig, int maxAttemptsPerInstance) => Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutRejectedException>()
            .OrResult(_handledHttpStatusCodes)
            .WaitAndRetryAsync(retryCount: (maxAttemptsPerInstance * 2) - 1,
                sleepDurationProvider: (sleepRetryCount, response, context) =>
                {
                    var retryAfter = response?.Result?.Headers?.RetryAfter;
                    if (retryAfter is null)
                    {
                        return TimeSpan.Zero;
                    }

                    var interval = retryAfter.Date.HasValue
                        ? retryAfter.Date.Value - DateTime.UtcNow
                        : retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);

                    context["RetryAfterHeaderSleepDuration"] = interval;
                    return interval;
                },
                onRetryAsync: (response, timespan, retryCount, context) =>
                {
                    context["RetryCount"] = retryCount;
                    if (retryCount != maxAttemptsPerInstance || !apiConfig.Secondary.IsValid)
                    {
                        return Task.CompletedTask;
                    }

                    ConfigureLmkHeaders(request.Headers, apiConfig.Secondary);

                    // update RequestUri
                    if (context.TryGetValue(LandmarkHttpPolicyOptions.CustomRequestUriActionName, out var uriActionObject) &&
                        uriActionObject is Func<LandmarkInstanceConfig, Uri> uriAction)
                    {
                        request.RequestUri = uriAction(apiConfig.Secondary);
                    }
                    else
                    {
                        var builder = new UriBuilder(request.RequestUri)
                        {
                            Host = apiConfig.Secondary.BaseUri.Host,
                            Port = apiConfig.Secondary.BaseUri.Port
                        };

                        request.RequestUri = builder.Uri;
                    }

                    // optionally update body if it contains host-specific information
                    if (request.Content != null && context.TryGetValue(LandmarkHttpPolicyOptions.CustomRequestBodyActionName, out var contentActionObject) &&
                        contentActionObject is Func<LandmarkInstanceConfig, HttpContent> contentAction)
                    {
                        request.Content.Dispose();
                        request.Content = contentAction(apiConfig.Secondary);
                    }

                    return Task.CompletedTask;
                });

        /// <summary>
        /// Creates final <see cref="LandmarkNotAvailableException"/> exception if request wasn't handled.
        /// </summary>
        /// <param name="result">Policy result.</param>
        /// <param name="context">Policy context.</param>
        /// <returns></returns>
        private Task OnFallbackAsync(DelegateResult<HttpResponseMessage> result, Context context)
        {
            LandmarkIsNotAvailable = true;

            var finalException = result.Exception is null
                ? new LandmarkNotAvailableException()
                : new LandmarkNotAvailableException(result.Exception);

            if (result.Result != null)
            {
                finalException.Data.Add("StatusCode", result.Result.StatusCode);
                finalException.Data.Add("ReasonPhrase", result.Result.ReasonPhrase);
            }

            if (context.TryGetValue("RetryAfterHeaderSleepDuration", out var rawRetryAfter) && rawRetryAfter is TimeSpan retryAfter)
            {
                finalException.Data.Add("RetryAfterHeaderSleepDuration", retryAfter);
            }

            if (context.TryGetValue("RetryCount", out var rawCount) && rawCount is int count)
            {
                finalException.Data.Add("RetryCount", count);
            }

            if (context.TryGetValue("BlockedTill", out var rawBlockTime) && rawBlockTime is DateTime blockTime)
            {
                finalException.Data.Add("BlockedTill", blockTime);
            }

            throw finalException;
        }
    }
}
