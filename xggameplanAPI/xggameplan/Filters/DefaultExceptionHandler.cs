using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using xggameplan.Errors;

namespace xggameplan.Exceptions
{
    public class DefaultExceptionHandler : ExceptionHandler
    {
        private const string ControllerActivatorClassName = "DefaultHttpControllerActivator";
        private readonly bool _sendDetailedError;

        public DefaultExceptionHandler(bool sendDetailedError)
        {
            _sendDetailedError = sendDetailedError;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            Exception exception = context.Exception;
            var errorMessage = _sendDetailedError
                    ? $"Message: {exception.Message} {Environment.NewLine} StackTrace: {exception.StackTrace}"
                    : exception.Message;

            /*context.Result = new ErrorMessageResult(context.Request, context.Request.CreateUnknownError(errorMessage));*/

            var httpResponseMessage = (exception.TargetSite.DeclaringType.Name == ControllerActivatorClassName)
                // often happens because some of controller dependencies (f.e. current user) can not be resolved.
                // for now just simulating 401 as it was in original nextgen implementation
                ? context.Request.CreateUnauthrorizedError(errorMessage)
                : context.Request.CreateUnknownError(errorMessage);
            context.Result = new ErrorMessageResult(context.Request, httpResponseMessage);
        }

        public override bool ShouldHandle(ExceptionHandlerContext context) => true;

        internal class ErrorMessageResult : IHttpActionResult
        {
            private HttpRequestMessage _request;
            private HttpResponseMessage _httpResponseMessage;

            public ErrorMessageResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
            {
                _request = request;
                _httpResponseMessage = httpResponseMessage;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_httpResponseMessage);
            }
        }
    }
}
