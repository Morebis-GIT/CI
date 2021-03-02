using System;
using System.Net.Http;
using Polly;

namespace xggameplan.core.Landmark.Abstractions
{
    public interface ILandmarkHttpPoliciesHolder
    {
        IAsyncPolicy<HttpResponseMessage> HttpFallbackPolicy { get; }
        IAsyncPolicy<HttpResponseMessage> HttpCircuitBreakerPolicy { get; }
        IAsyncPolicy<HttpResponseMessage> HttpTimeoutPolicy { get; }
        Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> GetRetryPolicyAction { get; }
        bool LandmarkIsNotAvailable { get; set; }
    }
}
