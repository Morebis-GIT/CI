using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace xggameplan.Configuration
{
    /// <summary>
    /// Provides methods for configuring Swagger - the tool for automatic API documentation and help page generator.
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Configures swagger on the provided HttpConfiguration.
        /// </summary>
        /// <param name="httpConfiguration">Http configuration which will be configured</param>
        /// <param name="applicationConfiguration">Configuration specifying which modules should be loaded</param>
        public static void SetupSwagger(HttpConfiguration httpConfiguration, IConfiguration applicationConfiguration)
        {
            if (!applicationConfiguration.GetValue<bool>("swagger:enabled"))
            {
                return;
            }


            var xmlCommentsPath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "xggameplan.xml");

            httpConfiguration.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "xG Gameplan API");
                    c.IncludeXmlComments(xmlCommentsPath);
                    c.MapType<Instant>(() => new Schema { type = "string", example = "2016-05-13T11:28:48.4779237Z" });
                    c.MapType<Duration>(() => new Schema { type = "string", example = "1:28:48.4779237" });
                    c.MapType<LocalTime>(() => new Schema { type = "string", example = "06:00:00" });
                    c.MapType<LocalDate>(() => new Schema { type = "string", example = "2015-05-26" });

                    // CMF Added. This is a workaround for an error stating that there are duplicate paths for POST for 'api/Subscriptions/{id}' but
                    // there is no mention of Subscriptions anywhere.
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    // Set document filter to hide endpoints
                    c.DocumentFilter<Services.SwaggerDocumentFilter>();

                    // Register token so that the Javascript can include it in the header
                    c.ApiKey("Token")
                        .Description("Filling bearer token here")
                        .Name("Authorization")
                        .In("header");

                    // this is needed to tell Swashbuckle to correctly recognize root of the application when running under IIS (https://github.com/domaindrivendev/Swashbuckle/issues/226)
                    // otherwise Swashbuckle tries to find swagger docs at <domain>/swagger/docs instead of <domain>/<virtualapp>/swagger/docs
                    c.RootUrl(req => new Uri(req.RequestUri, req.GetRequestContext().VirtualPathRoot).ToString());

                    // group actions by their RoutePrefix. That means that every ApiController needs to have a RoutePrefix attribute
                    // by default, actions are grouped by Controller name and that is not what we want to show to users of the API                    
                    c.GroupActionsBy(apiDesc =>
                        apiDesc.GetControllerAndActionAttributes<RoutePrefixAttribute>().Any()
                        ? apiDesc.GetControllerAndActionAttributes<RoutePrefixAttribute>().Single().Prefix
                        : "Internal");
                }).EnableSwaggerUi(x => x.InjectJavaScript(typeof(SwaggerConfig).Assembly, "xggameplan.Configuration.SwaggerAPIKey.js"));
        }
    }
}
