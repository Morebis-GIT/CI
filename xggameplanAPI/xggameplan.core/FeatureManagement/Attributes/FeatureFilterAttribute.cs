using System;
using System.Collections.Generic;

namespace xggameplan.core.FeatureManagement.Attributes
{
    /// <summary>
    /// An attribute that can be placed on component to require all or any of a set of features to be enabled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FeatureFilterAttribute : Attribute
    {
        /// <summary>
        /// Collection of the feature types that the feature attribute will activate for.
        /// </summary>
        public IEnumerable<string> Features { get; }

        /// <summary>
        /// Controls whether any or all features in <see cref="Features"/> should be enabled to pass.
        /// </summary>
        public RequirementType RequirementType { get; }

        /// <summary>
        /// Creates an attribute that can be used to control feature specific components.
        /// </summary>
        /// <param name="requirementType">Specifies whether all or any of the provided features should be enabled in order to pass.</param>
        /// <param name="features">A set of feature types representing the features that the attribute will represent.</param>
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
        /// Creates an attribute that can be used to control feature specific components.
        /// </summary>
        /// <param name="features">A set of feature types representing the features that the attribute will represent.</param>
        public FeatureFilterAttribute(params string[] features)
            : this(RequirementType.All, features)
        {
        }
    }
}
