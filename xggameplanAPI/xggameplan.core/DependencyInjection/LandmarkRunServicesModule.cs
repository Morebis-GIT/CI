using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Microsoft.Extensions.Configuration;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Helpers;
using xggameplan.core.Landmark;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.core.Landmark.PayloadProviders;

namespace xggameplan.core.DependencyInjection
{
    /// <summary>
    /// Registers components for managing landmark runs.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class LandmarkRunServicesModule : Module
    {
        private readonly IConfiguration _applicationConfiguration;
        private readonly IFeatureManager _featureManager;

        public LandmarkRunServicesModule(IConfiguration applicationConfiguration, IFeatureManager featureManager)
        {
            _applicationConfiguration = applicationConfiguration;
            _featureManager = featureManager;
        }

        protected override void Load(ContainerBuilder  builder)
        {
            var (landmarkApiConfig, httpPolicyOptions) = RegisterLmkConfig(builder);

            _ = builder.RegisterType<LandmarkRunService>()
                .As<ILandmarkRunService>()
                .InstancePerLifetimeScope();

            if (landmarkApiConfig is null || _applicationConfiguration.GetValue<bool?>("LandmarkApi:GenerateLocalInputFiles") == true)
            {
                _ = builder.RegisterType<LocalLandmarkAutoBookPayloadProvider>()
                    .As<ILandmarkAutoBookPayloadProvider>()
                    .InstancePerLifetimeScope();
            }
            else
            {
                _ = builder.RegisterType<CloudLandmarkAutoBookPayloadProvider>()
                    .As<ILandmarkAutoBookPayloadProvider>()
                    .InstancePerLifetimeScope();
            }

            if (landmarkApiConfig is null)
            {
                return;
            }

            var policyHolder = new LandmarkHttpPoliciesHolder(landmarkApiConfig, httpPolicyOptions);
            _ = builder.RegisterInstance(policyHolder)
                .As<ILandmarkHttpPoliciesHolder>()
                .SingleInstance();

            _ = builder.AddHttpClient<LandmarkApiClient, ILandmarkApiClient>()
                .ConfigureHttpClient(client =>
                {
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
                    LandmarkHttpPoliciesHolder.ConfigureLmkHeaders(client.DefaultRequestHeaders, landmarkApiConfig.DefaultInstance);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                })
                .ConfigureConnectionLeaseTimeout(landmarkApiConfig.Primary.BaseUri, httpPolicyOptions.TimeoutPerRequest)
                .AddPolicyHandler(policyHolder.HttpFallbackPolicy)
                .AddPolicyHandler(policyHolder.HttpCircuitBreakerPolicy)
                .AddPolicyHandler(policyHolder.GetRetryPolicyAction)
                .AddPolicyHandler(policyHolder.HttpTimeoutPolicy)
                .BuildClient();

            if (landmarkApiConfig.Secondary?.IsValid == true)
            {
                HttpClientServiceBuilder.SetConnectionLeaseTimeout(landmarkApiConfig.Secondary.BaseUri, httpPolicyOptions.TimeoutPerRequest);
            }
        }

        /// <summary>
        /// Registers the API configuration.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <returns></returns>
        /// <exception cref="Exception">landmark booking feature is enabled but configuration was not resolved</exception>
        private (LandmarkApiConfig apiConfig, LandmarkHttpPolicyOptions policyOptions) RegisterLmkConfig(ContainerBuilder builder)
        {
            LandmarkApiConfig landmarkApiConfig = new LandmarkApiConfig();
            LandmarkHttpPolicyOptions httpPolicyOptions;
            var configSection = _applicationConfiguration.GetSection("LandmarkApi");

            try
            {
                landmarkApiConfig.Secondary = configSection.GetSection("Secondary").Get<LandmarkInstanceConfig>();
            }
            catch (Exception)
            {
                // secondary instance is optional
                landmarkApiConfig.Secondary = new LandmarkInstanceConfig();
            }

            try
            {
                landmarkApiConfig.Primary = configSection.GetSection("Primary").Get<LandmarkInstanceConfig>();
                httpPolicyOptions = configSection.GetSection("HttpPolicyOptions").Get<LandmarkHttpPolicyOptions>();

                landmarkApiConfig.ApplyDefaultsToSecondaryInstance();
                _ = builder.RegisterInstance(landmarkApiConfig).SingleInstance();
            }
            catch (Exception)
            {
                landmarkApiConfig = null;
                httpPolicyOptions = null;
            }

            if ((landmarkApiConfig is null || landmarkApiConfig.Primary?.IsValid != true) && _featureManager.IsEnabled(nameof(ProductFeature.LandmarkBooking)))
            {
                throw new Exception("landmark booking feature is enabled but primary instance configuration or http policy options were not resolved");
            }

            return (landmarkApiConfig, httpPolicyOptions);
        }
    }
}
