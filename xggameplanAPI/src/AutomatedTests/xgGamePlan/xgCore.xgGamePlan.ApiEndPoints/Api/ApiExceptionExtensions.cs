using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Refit;

namespace xgCore.xgGamePlan.ApiEndPoints.Api
{
    public static class ApiExceptionExtensions
    {
        public static ResponseErrorInfo ToResponseErrorInfo(this ApiException apiException)
        {
            ResponseErrorInfo errorInfo = null;
            try
            {
                errorInfo = apiException.GetContentAsAsync<ResponseErrorInfo>().Result;
            }
            catch (JsonException)
            {
            }
            catch (AggregateException e) when (e.InnerException is JsonException)
            {
            }

            return errorInfo;
        }
    }
}
