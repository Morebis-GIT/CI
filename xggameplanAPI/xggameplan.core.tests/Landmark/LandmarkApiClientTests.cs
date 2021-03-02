using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using xggameplan.AuditEvents;
using xggameplan.core.Landmark;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.Model;

namespace xggameplan.core.tests.Landmark
{
    [TestFixture]
    public class LandmarkApiClientTests : IDisposable
    {
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpResponseMessage _mockResponse;
        private Fixture _fixture;
        private ILandmarkApiClient _landmarkApiClient;
        private Mock<IAuditEventRepository> _mockAuditEventRepository;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockAuditEventRepository = new Mock<IAuditEventRepository>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _landmarkApiClient = new LandmarkApiClient(_httpClient, _fixture.Create<LandmarkApiConfig>(), _mockAuditEventRepository.Object);

            _mockResponse = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent(JsonConvert.SerializeObject(_fixture.Create<Guid>()))
            };

            _ = _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(_mockResponse);
        }

        [Test]
        [Description("Given valid AutoBookRequest when triggering run then result model must not be null")]
        public async Task TriggerRunWhenCalledWithValidModelThenShouldNotReturnNull()
        {
            var result = await _landmarkApiClient.TriggerRunAsync(_fixture.Create<LandmarkBookingRequest>()).ConfigureAwait(false);
            _ = result.Should().NotBeNull();
        }

        [Test]
        [Description("Given valid AutoBookRequest when triggering run then result model must contain valid processing id")]
        public async Task TriggerRunWhenCalledWithValidModelThenShouldSetProcessingId()
        {
            var processingId = Guid.NewGuid();
            _mockResponse.Content = new StringContent(JsonConvert.SerializeObject(processingId));

            var result = await _landmarkApiClient.TriggerRunAsync(_fixture.Create<LandmarkBookingRequest>()).ConfigureAwait(false);
            _ = result.ProcessingId.Should().Be(processingId);
        }

        [Test]
        [Description("Given AutoBookRequest when triggering run and unsuccessful status code returned then exception must be thrown")]
        public void TriggerRunWhenUnsuccessfulStatusCodeReturnsThenShouldThrowException()
        {
            _mockResponse.StatusCode = HttpStatusCode.InternalServerError;

            _ = _landmarkApiClient.Awaiting(async x => await x.TriggerRunAsync(_fixture.Create<LandmarkBookingRequest>()).ConfigureAwait(false)).Should().Throw<Exception>();
        }

        [Test]
        [Description("Given valid processingId when getting run status then result model must not be null")]
        public async Task GetRunStatusWhenCalledWithValidModelThenShouldNotReturnNull()
        {
            var result = await _landmarkApiClient.GetRunStatusAsync(_fixture.Create<Guid>()).ConfigureAwait(false);
            _ = result.Should().NotBeNull();
        }

        [Test]
        [Description("When landmark probe is called then must execute successfully")]
        public void ProbeLandmarkAsyncWhenCalledThenShouldNotThrowException()
        {
            _landmarkApiClient.Awaiting(async x => await x.ProbeLandmarkAsync().ConfigureAwait(false)).Should().NotThrow();
        }

        [TearDown]
        public void Cleanup()
        {
            Dispose();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClient = null;
            _mockResponse?.Dispose();
            _mockResponse = null;
        }
    }
}
