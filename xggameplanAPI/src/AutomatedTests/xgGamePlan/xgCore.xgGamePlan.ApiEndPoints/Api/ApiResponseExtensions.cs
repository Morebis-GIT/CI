using System.Net;
using Refit;

namespace xgCore.xgGamePlan.ApiEndPoints.Api
{
    public static class ApiResponseExtensions
    {
        public static void Validate<T>(this ApiResponse<T> apiResponse, HttpStatusCode expectedStatusCode = 0)
        {
            if (apiResponse == null)
            {
                return;
            }

            if (expectedStatusCode != apiResponse.StatusCode)
            {
                if (expectedStatusCode == 0 && apiResponse.IsSuccessStatusCode)
                {
                    return;
                }
            }

            throw new ServerResponseException(
                $"Expected status code '{expectedStatusCode}' but received '{apiResponse.StatusCode}' for request: {apiResponse.RequestMessage.RequestUri}.",
                apiResponse.Error);
        }
    }
}
