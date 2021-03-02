using System;
using System.Net;
using System.Net.Http;

using JetBrains.Annotations;

namespace xggameplan.Errors
{
    public static class ErrorResponseRequestExtensions
    {
        /// <summary>
        /// Creates 404 response which indicates that the requested resource was not found as opposed to the situation when route doesn't match any controller (which is 404 as well).
        /// </summary>
        public static HttpResponseMessage CreateResourceNotFoundError([NotNull] this HttpRequestMessage request, string customErrorMessage = null)
            => request.CreateResponse(HttpStatusCode.NotFound, ApiError.ResourceNotFound.Code, customErrorMessage ?? ApiError.ResourceNotFound.Message);

        /// <summary>
        /// Creates 400 response with explanation of the error.
        /// </summary>
        public static HttpResponseMessage CreateBadRequest([NotNull] this HttpRequestMessage request, ApiError error)
            => request.CreateResponse(HttpStatusCode.BadRequest, error.Code, error.Message);

        /// <summary>
        /// Creates 400 response with indication that passed parameters were invalid.
        /// </summary>
        public static HttpResponseMessage CreateInvalidParametersError([NotNull] this HttpRequestMessage request, string customErrorMessage = null)
            => request.CreateResponse(HttpStatusCode.BadRequest, ApiError.InvalidParameters.Code, customErrorMessage ?? ApiError.InvalidParameters.Message);

        /// <summary>
        /// Creates 500 response with explanation of the error.
        /// </summary>
        public static HttpResponseMessage CreateUnknownError([NotNull] this HttpRequestMessage request, string customErrorMessage = null)
            => request.CreateResponse(HttpStatusCode.InternalServerError, ApiError.Unknown.Code, customErrorMessage ?? ApiError.Unknown.Message);

        /// <summary>
        /// Creates 401 response with explanation of the error.
        /// </summary>
        public static HttpResponseMessage CreateUnauthrorizedError([NotNull] this HttpRequestMessage request, string customErrorMessage = null)
            => request.CreateResponse(HttpStatusCode.Unauthorized, ApiError.Unauthorized.Code, customErrorMessage ?? ApiError.Unauthorized.Message);

        /// <summary>
        /// Creates 405 response which indicates that the requested resource was not allowed.
        /// </summary>
        public static HttpResponseMessage CreateMethodNotAllowedError([NotNull] this HttpRequestMessage request, string customErrorMessage = null)
            => request.CreateResponse(HttpStatusCode.MethodNotAllowed, ApiError.MethodNotAllowed.Code, customErrorMessage ?? ApiError.MethodNotAllowed.Message);

        private static HttpResponseMessage CreateResponse([NotNull] this HttpRequestMessage request, HttpStatusCode statusCode, string errorCode, string errorMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.CreateResponse(statusCode, new ErrorResponseModel(errorCode, errorMessage));
        }

        /// <summary>
        /// Represents error message containing an explanation what happened.
        /// </summary>
        private class ErrorResponseModel
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public ErrorResponseModel(string errorCode, string errorMessage)
            {
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }

            /// <summary>
            /// Error code describing why request failed.
            /// </summary>
            public string ErrorCode { get; set; }

            /// <summary>
            /// Human readable reason of the error.
            /// </summary>
            public string ErrorMessage { get; set; }
        }
    }

    /// <summary>
    /// Api error
    /// </summary>
    public sealed class ApiError
    {
        private ApiError(
            string code,
            string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Api error code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Api error message
        /// </summary>
        public string Message { get; }

        /// <summary></summary>
        public static ApiError BadRequest(string message) => new ApiError(
                "BAD_REQUEST",
                message);

        /// <summary></summary>
        public static readonly ApiError ResourceNotFound = new ApiError(
            "RESOURCE_NOT_FOUND",
            "Requested resource was not found.");

        /// <summary></summary>
        public static readonly ApiError InvalidParameters = new ApiError(
            "INVALID_PARAMETERS",
            "Required parameters missing or invalid.");

        /// <summary></summary>
        public static readonly ApiError Unauthorized = new ApiError(
            "UNAUTHORIZED",
            "User not authorized to perform requested operation.");

        /// <summary></summary>
        public static readonly ApiError Unknown = new ApiError(
            "UNKNOWN_ERROR",
            "Unknown error occurred.");

        /// <summary></summary>
        public static readonly ApiError CanNotDeleteSlotWithInterstitials = new ApiError(
            "CAN_NOT_DELETE_SLOT_WITH_INTERSTITIALS",
            "Slot can not be deleted when there is interstitials associated with it.");

        /// <summary>
        /// Includes 405 response code and message.
        /// </summary>
        public static readonly ApiError MethodNotAllowed = new ApiError(
            "METHOD_NOT_ALLOWED",
            "Requested resource was not allowed.");
    }
}
