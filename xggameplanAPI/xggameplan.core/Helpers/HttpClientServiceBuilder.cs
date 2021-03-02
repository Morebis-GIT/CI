using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Polly;
using xggameplan.core.Interfaces;

namespace xggameplan.core.Helpers
{
    /// <inheritdoc />
    internal sealed class HttpClientServiceBuilder : IHttpClientServiceBuilder
    {
        private readonly Action<HttpClient> _postBuildAction;
        private readonly List<DelegatingHandler> _innerHandlers = new List<DelegatingHandler>(5);

        private Action<HttpClient> _configureClient;
        private HttpMessageHandler _primaryHandler;
        private bool _servicePointConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientServiceBuilder"/> class.
        /// </summary>
        public HttpClientServiceBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientServiceBuilder"/> class.
        /// </summary>
        /// <param name="postBuildAction">Delegate to be executed after <see cref="HttpClient"/> instance is prepared.</param>
        /// <exception cref="ArgumentNullException">postBuildAction</exception>
        public HttpClientServiceBuilder(Action<HttpClient> postBuildAction) =>
            _postBuildAction = postBuildAction ?? throw new ArgumentNullException(nameof(postBuildAction));

        /// <inheritdoc />
        public IHttpClientServiceBuilder ConfigureHttpClient(Action<HttpClient> configureClient)
        {
            _configureClient = configureClient ?? throw new ArgumentNullException(nameof(configureClient));
            return this;
        }

        /// <inheritdoc />
        public IHttpClientServiceBuilder ConfigurePrimaryHttpMessageHandler(Func<HttpMessageHandler> configureHandler)
        {
            if (configureHandler is null)
            {
                throw new ArgumentNullException(nameof(configureHandler));
            }

            _primaryHandler = configureHandler();
            return this;
        }

        /// <inheritdoc />
        public IHttpClientServiceBuilder AddHttpMessageHandler(Func<DelegatingHandler> configureHandler)
        {
            if (configureHandler is null)
            {
                throw new ArgumentNullException(nameof(configureHandler));
            }

            _innerHandlers.Add(configureHandler());
            return this;
        }

        /// <inheritdoc />
        public IHttpClientServiceBuilder AddPolicyHandler(IAsyncPolicy<HttpResponseMessage> policy)
        {
            if (policy is null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            return AddHttpMessageHandler(() => new PolicyHttpMessageHandler(policy));
        }

        /// <inheritdoc />
        public IHttpClientServiceBuilder AddPolicyHandler(Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> policySelector)
        {
            if (policySelector is null)
            {
                throw new ArgumentNullException(nameof(policySelector));
            }

            return AddHttpMessageHandler(() => new PolicyHttpMessageHandler(policySelector));
        }

        /// <inheritdoc />
        public IHttpClientServiceBuilder ConfigureConnectionLeaseTimeout(Uri address, TimeSpan interval)
        {
            SetConnectionLeaseTimeout(address, interval);
            _servicePointConfigured = true;

            return this;
        }

        /// <inheritdoc />
        public HttpClient BuildClient()
        {
            var primaryHandler = _primaryHandler ?? new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            var client = HttpClientFactory.Create(primaryHandler, _innerHandlers.ToArray());

            _configureClient?.Invoke(client);

            if (!_servicePointConfigured)
            {
                if (client.BaseAddress is null)
                {
                    throw new Exception($"ServicePoint for {nameof(HttpClient)} should be configured to avoid outdated DNS issue");
                }

                _ = ConfigureConnectionLeaseTimeout(client.BaseAddress, TimeSpan.FromSeconds(120));
            }

            _postBuildAction?.Invoke(client);

            return client;
        }

        public static void SetConnectionLeaseTimeout(Uri address, TimeSpan interval)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var sp = ServicePointManager.FindServicePoint(address);
            sp.ConnectionLeaseTimeout = (int)interval.Duration().TotalMilliseconds;
        }
    }
}
