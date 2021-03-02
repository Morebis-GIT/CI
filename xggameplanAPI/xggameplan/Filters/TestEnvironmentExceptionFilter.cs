using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using xggameplan.Errors;

namespace xggameplan.Filters
{
    public class TestEnvironmentExceptionFilter : IAutofacExceptionFilter
    {
        public Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var exception = actionExecutedContext.Exception;
            var error = new HttpError
            {
                {"errorCode", ApiError.Unknown.Code},
                {"errorMessage", exception.Message},
                {"exceptionType", exception.GetType().Name},
                {
                    "controllerName",
                    actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName
                },
                {"actionName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName},
                {"stackTrace", exception.StackTrace}
            };

            actionExecutedContext.Response =
                actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);

            return Task.CompletedTask;
        }
    }
}
