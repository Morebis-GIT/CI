using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

using JetBrains.Annotations;

namespace xggameplan.Errors
{
    public class ApiControllerErrorResponseCreator
    {
        private readonly ApiController _apiController;

        public ApiControllerErrorResponseCreator([NotNull] ApiController apiController)
        {
            if (apiController == null)
            {
                throw new ArgumentNullException(nameof(apiController));
            }
            _apiController = apiController;
        }

        /// <summary>
        /// Returns 404 response which indicates that the requested resource was not found as opposed to the situation when route doesn't match any controller (which is 404 as well).
        /// </summary>
        public IHttpActionResult ResourceNotFound(string customErrorMessage = null)
        {
            var response = _apiController.Request.CreateResourceNotFoundError(customErrorMessage);

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Returns 400 response with indication that passed parameters were invalid.
        /// </summary>
        public IHttpActionResult InvalidParameters(string customErrorMessage = null)
        {
            string errorMessage = customErrorMessage ?? "Required parameters missing or invalid.";

            var errors = _apiController.ModelState.Values.SelectMany(v => v.Errors).ToList();

            if (errors.Any())
            {
                errorMessage = errorMessage + " " + string.Join(". ", errors.Select(e => e.ErrorMessage));
            }

            var response = _apiController.Request.CreateInvalidParametersError(errorMessage);

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Returns 400 response with indication that passed parameters were invalid.
        /// </summary>
        public IHttpActionResult InvalidParameters(List<string> customErrorMessage)
        {
            var errors = _apiController.ModelState.Values.SelectMany(v => v.Errors).ToList().Select(e => e.ErrorMessage);

            customErrorMessage.AddRange(errors);

            var response = _apiController.Request.CreateResponse(HttpStatusCode.BadRequest, new { errorCode = ApiError.InvalidParameters.Code, errorMessage = customErrorMessage });
            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Returns 500 response with explanation of the error.
        /// </summary>
        public IHttpActionResult UnknownError(string customErrorMessage)
        {
            var response = _apiController.Request.CreateUnknownError(customErrorMessage);

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Returns 400 response with explanation of the error.
        /// </summary>
        public IHttpActionResult BadRequest(ApiError error)
        {
            var response = _apiController.Request
                .CreateBadRequest(error);

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Returns 400 response with explanation of the error.
        /// </summary>
        public IHttpActionResult BadRequest(string message)
        {
            var error = ApiError.BadRequest(message);
            var response = _apiController.Request
                .CreateBadRequest(error);

            return new ResponseMessageResult(response);
        }
    }
}
