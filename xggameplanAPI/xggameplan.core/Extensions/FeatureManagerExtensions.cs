using System;
using System.Linq;
using System.Reflection;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Attributes;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.core.Extensions
{
    /// <summary>
    /// Extends <see cref="IFeatureManager"/> functionality.
    /// </summary>
    public static class FeatureManagerExtensions
    {
        /// <summary>
        /// Returns true if the specified <see cref="ProductFeature"/> value is enabled, otherwise false.
        /// </summary>
        public static bool IsEnabled(this IFeatureManager featureManager, ProductFeature feature)
        {
            if (featureManager is null)
            {
                throw new ArgumentNullException(nameof(featureManager));
            }

            return featureManager.IsEnabled(feature.ToString());
        }

        /// <summary>
        /// Determines whether the specified type is enabled using <see cref="FeatureFilterAttribute"/> attribute
        /// </summary>
        /// <param name="featureManager">Feature manager.</param>
        /// <param name="serviceType">Type of the service to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified service is enabled; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">featureManager</exception>
        public static bool IsServiceEnabled(this IFeatureManager featureManager, Type serviceType)
        {
            if (featureManager is null)
            {
                throw new ArgumentNullException(nameof(featureManager));
            }

            var featureAttribute = serviceType.GetCustomAttribute<FeatureFilterAttribute>(true);
            if (featureAttribute is null)
            {
                return true;
            }

            return featureAttribute.RequirementType == RequirementType.All
                ? featureAttribute.Features.All(featureManager.IsEnabled)
                : featureAttribute.Features.Any(featureManager.IsEnabled);
        }
    }
}
