using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.Filters
{
    /// <summary>
    /// An attribute that can be placed on MVC actions to require all or any of a set of features to be enabled
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FeatureFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Collection of the feature types that the feature attribute will activate for
        /// </summary>
        public IEnumerable<string> Features { get; }

        /// <summary>
        /// Controls whether any or all features in <see cref="Features"/> should be enabled to pass
        /// </summary>
        public RequirementType RequirementType { get; }

        /// <summary>
        /// Creates an attribute that can be used to process feature specific actions
        /// </summary>
        /// <param name="requirementType">Specifies whether all or any of the provided features should be enabled in order to pass.</param>
        /// <param name="features">A set of feature types representing the features that the attribute will represent</param>
        public FeatureFilterAttribute(RequirementType requirementType, params string[] features)
        {
            if (features == null || features.Length == 0)
            {
                throw new ArgumentNullException(nameof(features));
            }

            Features = features;
            RequirementType = requirementType;
        }

        /// <summary>
        /// Creates an attribute that can be used to process feature specific actions
        /// </summary>
        /// <param name="features">A set of feature types representing the features that the attribute will represent</param>
        public FeatureFilterAttribute(params string[] features)
            : this(RequirementType.All, features)
        {
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var actionEnabled = false;

            if (actionContext.Request?.GetDependencyScope()?.GetService(typeof(IFeatureManager)) is IFeatureManager featureManager)
            {
                actionEnabled = RequirementType == RequirementType.All
                    ? Features.All(f => featureManager.IsEnabled(f))
                    : Features.Any(f => featureManager.IsEnabled(f));
            }

            if (!actionEnabled)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }
        }
    }
}
