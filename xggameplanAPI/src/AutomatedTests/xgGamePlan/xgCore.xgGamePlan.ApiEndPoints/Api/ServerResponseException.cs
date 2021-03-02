using System;
using System.Text;
using Refit;

namespace xgCore.xgGamePlan.ApiEndPoints.Api
{
    public class ServerResponseException : Exception
    {
        private readonly string _message;

        private string FormatExceptionMessage(ApiException apiException)
        {
            var response = apiException.ToResponseErrorInfo();

            if (response == null)
            {
                return "See inner exception for more information.";
            }

            var builder = new StringBuilder();
            builder.AppendLine(response.ErrorMessage ?? "API Server Error");
            if (response.StackTrace != null)
            {
                builder.AppendLine(response.StackTrace);
                builder.AppendLine();
            }

            if (response.ExceptionType != null)
            {
                builder.AppendLine($"{nameof(response.ExceptionType)}: {response.ExceptionType}");
            }
            if (response.ErrorCode != null)
            {
                builder.AppendLine($"{nameof(response.ErrorCode)}: {response.ErrorCode}");
            }
            if (response.ControllerName != null)
            {
                builder.AppendLine($"{nameof(response.ControllerName)}: {response.ControllerName}");
            }
            if (response.ActionName != null)
            {
                builder.AppendLine($"{nameof(response.ActionName)}: {response.ActionName}");
            }

            return builder.ToString();
        }

        public override string Message => _message ?? base.Message;

        public ServerResponseException(ApiException apiException) : base(apiException.Message, apiException)
        {
            _message = FormatExceptionMessage(apiException);
        }

        public ServerResponseException()
        {
        }

        public ServerResponseException(string message) : base(message)
        {
        }

        public ServerResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
