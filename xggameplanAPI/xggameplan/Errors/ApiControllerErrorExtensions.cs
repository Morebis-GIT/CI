using System;
using System.Web.Http;

using JetBrains.Annotations;

namespace xggameplan.Errors
{
    /// <summary>
    /// Contains extension methods for ApiController which can return custom error responses.
    /// </summary>
    public static class ApiControllerErrorExtensions
    {
        public static ApiControllerErrorResponseCreator Error([NotNull] this ApiController apiController)
        {
            if (apiController == null)
            {
                throw new ArgumentNullException(nameof(apiController));
            }

            return new ApiControllerErrorResponseCreator(apiController);
        }
    }
}
