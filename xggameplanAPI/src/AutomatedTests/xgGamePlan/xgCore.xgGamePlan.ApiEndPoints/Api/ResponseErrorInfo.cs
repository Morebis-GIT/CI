namespace xgCore.xgGamePlan.ApiEndPoints.Api
{
    public class ResponseErrorInfo
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ExceptionType { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string StackTrace { get; set; }
    }
}
