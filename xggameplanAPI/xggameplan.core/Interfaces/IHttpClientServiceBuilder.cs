using System;
using System.Net;
using System.Net.Http;
using Polly;

namespace xggameplan.core.Interfaces
{
    /// <summary>
    /// Represents logic for creating and configuring <see cref="HttpClient"/> instance.
    /// </summary>
    public interface IHttpClientServiceBuilder
    {
        /// <summary>
        /// Adds a delegate that will be used to configure a named <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the client.</returns>
        IHttpClientServiceBuilder ConfigureHttpClient(Action<HttpClient> configureClient);

        /// <summary>
        /// Adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a <see cref="HttpClient"/>
        /// </summary>
        /// <param name="configureHandler">A delegate that is used to create an <see cref="HttpMessageHandler"/>.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the client.</returns>
        IHttpClientServiceBuilder ConfigurePrimaryHttpMessageHandler(Func<HttpMessageHandler> configureHandler);

        /// <summary>
        /// Adds a delegate that will be used to create an additional message handler for a named <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="configureHandler">A delegate that is used to create a <see cref="DelegatingHandler"/>.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the client.</returns>
        IHttpClientServiceBuilder AddHttpMessageHandler(Func<DelegatingHandler> configureHandler);

        /// <summary>
        /// Adds a <see cref="PolicyHttpMessageHandler"/> which will surround request execution with
        /// the provided <see cref="IAsyncPolicy{HttpResponseMessage}"/>.
        /// </summary>
        /// <param name="policy">The <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the client.</returns>
        IHttpClientServiceBuilder AddPolicyHandler(IAsyncPolicy<HttpResponseMessage> policy);

        /// <summary>
        /// Adds a <see cref="PolicyHttpMessageHandler"/> which will surround request execution with a policy returned
        /// by the <paramref name="policySelector"/>.
        /// </summary>
        /// <param name="policySelector">Selects an <see cref="IAsyncPolicy{HttpResponseMessage}"/> to apply to the current request.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the client.</returns>
        IHttpClientServiceBuilder AddPolicyHandler(Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> policySelector);

        /// <summary>
        /// Configures HTTP connection options for the specified <paramref name="address"/>.
        /// </summary>
        /// <param name="address">Resource base address.</param>
        /// <param name="timeout">Value that is used to configure an <see cref="ServicePoint.ConnectionLeaseTimeout"/>.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the client.</returns>
        IHttpClientServiceBuilder ConfigureConnectionLeaseTimeout(Uri address, TimeSpan timeout);

        /// <summary>
        /// Builds <see cref="HttpClient"/> instance.
        /// </summary>
        /// <returns><see cref="HttpClient"/> instance.</returns>
        HttpClient BuildClient();
    }
}
