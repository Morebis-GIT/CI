using System;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Microsoft.Extensions.Logging;
using xggameplan.AuditEvents;

namespace xggameplan.Logging
{
    /// <summary>
    /// Logs exceptions via error log and audit event
    /// </summary>
    public class DefaultExceptionLogger : ExceptionLogger
    {
        private IAuditEventRepository _auditEventRepository;
        private readonly ILogger _logger;

        public DefaultExceptionLogger(IAuditEventRepository auditEventRepository, ILogger logger)
        {
            _auditEventRepository = auditEventRepository;
            _logger = logger;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            if (context.Exception == null)
            {
                return;
            }

            _logger.LogError(context.Exception, "application exception");

            try
            {
                _auditEventRepository.Insert(
                        AuditEventFactory.CreateAuditEventForException(
                                tenantId: 0,
                                userId: 0,
                                message: GetAuditInfo(context),
                                exception: context.Exception));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "auditevent insert exception"); // hope unreachable code
            };

            base.Log(context);
        }

        private static string GetAuditInfo(ExceptionLoggerContext context)
        {
            const string template = "Method ={method}, URL ={url}, ContentType ={contentType}, ClientIP ={clientId}";
            if (context.Request != null)
            {
                var method = context.Request.Method.Method;
                var url = context.Request.RequestUri.ToString();
                var contentType = context.Request.Content?.Headers?.ContentType?.MediaType ?? "";
                var clientId = GetClientIP(context.Request);
                return $"Method={method}, URL={url}, ContentType={contentType}, ClientIP={clientId}";
            }
            return $"Method=-, URL=-, ContentType=-, ClientIP=-";
        }

        private static string GetClientIP(System.Net.Http.HttpRequestMessage request)
        {
            try
            {
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    HttpContextWrapper httpContextWrapper = (HttpContextWrapper)request.Properties["MS_HttpContext"];
                    return httpContextWrapper.Request.UserHostAddress;
                }
                else if (HttpContext.Current?.Request != null)
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
            }
            catch { };
            return "";
        }
    }
}
