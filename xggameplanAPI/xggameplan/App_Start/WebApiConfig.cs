using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Validation;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Serilog;
using Serilog.Extensions.Logging;
using xggameplan.AuditEvents;
using xggameplan.Configuration;
using xggameplan.core.Logging;
using xggameplan.Exceptions;
using xggameplan.Filters;
using xggameplan.Helpers;
using xggameplan.Logging;
using xggameplan.Services.Compression;
using xggameplan.Validations;

namespace xggameplan
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration httpConfig)
        {
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.GetData("DataDirectory").ToString())
                .AddJsonFile("config.json", reloadOnChange: true, optional: false)
                .Build();

            httpConfig.MapHttpAttributeRoutes();
            ConfigureFormatters(httpConfig);
            ConfigureLogging(httpConfig, appConfig);

            httpConfig.Services.Replace(typeof(IExceptionHandler),
                new DefaultExceptionHandler(appConfig.GetValue("debug:sendDetailedErrors", false)));

            httpConfig.Services.Replace(typeof(IBodyModelValidator), new BodyModelValidator());

            var allowedOrigins = appConfig["cors:allowedOrigins"];
            if (!String.IsNullOrWhiteSpace(allowedOrigins))
            {
                httpConfig.EnableCors(new EnableCorsAttribute(allowedOrigins, "*", "*"));
            }

            // Configure to use custom parameter binding to support multiple Deletes
            httpConfig.ParameterBindingRules.Add(descriptor => descriptor.ActionDescriptor.SupportedHttpMethods.Contains(HttpMethod.Delete) &&
                (descriptor.ParameterType == typeof(string[]) || descriptor.ParameterType == typeof(IEnumerable<string>))
                    ? new CommaDelimitedToIEnumerableStringParameterBinder(descriptor)
                    : null);

            ApplicationModulesLoader.Configure(httpConfig, appConfig);

            SwaggerConfig.SetupSwagger(httpConfig, appConfig);
            httpConfig.Filters.Add(new AuthenticationRequestFilter());

            if (appConfig["Environment:Id"] == "Dev") { return; }

            // Add compression handler
            httpConfig.MessageHandlers.Add(new CompressedHandler());
        }

        private static void ConfigureFormatters(HttpConfiguration httpConfig)
        {
            httpConfig.Formatters.Remove(httpConfig.Formatters.XmlFormatter);
            var jsonSerializerSettings = httpConfig.Formatters.JsonFormatter.SerializerSettings;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        private static void ConfigureLogging(HttpConfiguration httpConfig, IConfiguration appConfiguration)
        {
            var logFolder = HostingEnvironment.MapPath("/Logs");
            var apiLoggerFactory = new SerilogLoggerFactory(new LoggerConfiguration()
                            .Enrich.With<UtcTimeLogEventEnricher>()
                            .Enrich.WithHttpRequestType()
                            .Enrich.WithHttpRequestClientHostIP()
                            .Enrich.WithHttpRequestClientHostName()
                            .Enrich.WithHttpRequestRawUrl()
                            .Enrich.WithWebApiControllerName()
                            .Enrich.WithWebApiActionName()
                            .WriteTo.Async(a =>
                                a.File(Path.Combine(logFolder, $"app-errors.log"),
                                    rollingInterval: RollingInterval.Day,
                                    outputTemplate: "{UtcTime}|{HttpRequestType}|{HttpRequestRawUrl}|{WebApiController}|{WebApiAction}|{HttpRequestClientHostIP}|{HttpRequestClientHostName}|{Message:lj}{NewLine}{Exception}",
                                    shared: true))
                            .CreateLogger(), true);
            var logger = apiLoggerFactory.CreateLogger("app-errors");

            if (appConfiguration.GetValue("Logging:LogRequests", false))
            {
                httpConfig.MessageHandlers.Add(new RequestLoggerCSV(logFolder,
                        logRequests: true,
                        logResponses: true,
                        logContent: appConfiguration.GetValue("Logging:LogContent", false)));
            }

            httpConfig.Services.Replace(typeof(IExceptionLogger),
             new DefaultExceptionLogger(GetExceptionLoggerAuditEventRepository(logFolder), logger));
        }

        /// <summary>
        /// Returns exception logger audit event repository
        /// </summary>
        /// <param name="csvLogsFolder"></param>
        /// <returns></returns>
        private static IAuditEventRepository GetExceptionLoggerAuditEventRepository(string csvLogsFolder)
        {
            var csvAuditEventRepositoryFactory =
                new CSVConfiguration(
                            auditEventTypeRepository: new AuditEventTypeRepository(),
                            auditEventValueTypeRepository: new AuditEventValueTypeRepository(),
                            csvAuditEventSettingsRepository: null,
                            folder: csvLogsFolder);

            return new MasterAuditEventRepository(new List<IAuditEventRepository>() { csvAuditEventRepositoryFactory.GetAuditEventRepository() });
        }
    }
}
