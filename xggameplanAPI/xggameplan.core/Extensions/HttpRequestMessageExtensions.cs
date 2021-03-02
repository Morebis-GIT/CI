using System;
using System.Net.Http;
using Polly;
using xggameplan.core.Landmark;

namespace xggameplan.core.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Sets custom URI build delegate which will be used during transition to the <see cref="LandmarkApiConfig.Secondary"/> landmark instance.
        /// </summary>
        /// <param name="request">Current request.</param>
        /// <param name="uriBuildAction">Request URI build delegate.</param>
        public static void SetCustomUriBuildAction(this HttpRequestMessage request, Func<LandmarkInstanceConfig, Uri> uriBuildAction) =>
            SetPollyContext(request, LandmarkHttpPolicyOptions.CustomRequestUriActionName, uriBuildAction);

        /// <summary>
        /// Sets custom Content build delegate which will be used during transition to the <see cref="LandmarkApiConfig.Secondary"/> landmark instance.
        /// </summary>
        /// <param name="request">Current request.</param>
        /// <param name="contentBuildAction">Request content build delegate.</param>
        public static void SetCustomContentBuildAction(this HttpRequestMessage request, Func<LandmarkInstanceConfig, HttpContent> contentBuildAction) =>
            SetPollyContext(request, LandmarkHttpPolicyOptions.CustomRequestBodyActionName, contentBuildAction);

        private static void SetPollyContext<T>(HttpRequestMessage request, string key, T value)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var context = request.GetPolicyExecutionContext();
            if (context is null)
            {
                context = new Context();
                request.SetPolicyExecutionContext(context);
            }

            context[key] = value;
        }
    }
}
