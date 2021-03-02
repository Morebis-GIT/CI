using System.Net;
using System.Runtime.Serialization;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults
{
    [DataContract]
    public class ApiErrorResult
    {
        public ApiErrorResult()
        {

        }

        public ApiErrorResult(HttpStatusCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode.ToString("D");
            ErrorMessage = errorMessage;
        }

        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
