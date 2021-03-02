using System;
using System.Linq;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Attributes;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.SchedulerTasks
{
    public static class SchedulerTasksExtensions
    {
        public static JobScopeType GetJobScope(this Type jobType)
        {
            var attributes = jobType
                .GetCustomAttributes(typeof(ScopeJobAttribute), true)
                .OfType<ScopeJobAttribute>()
                .ToArray();

            return attributes.Length != 1 ? JobScopeType.Application : attributes[0].Scope;
        }

        public static bool IsJobFeaturesEnabled(this IFeatureManager featureManager, Type jobType)
        {
            var attributes = jobType
                .GetCustomAttributes(typeof(FeatureFilterAttribute), true)
                .OfType<FeatureFilterAttribute>()
                .ToArray();

            if (attributes.Length != 1)
            {
                return true;
            }

            return attributes[0].RequirementType == RequirementType.All
                ? attributes[0].Features.All(featureManager.IsEnabled)
                : attributes[0].Features.Any(featureManager.IsEnabled);
        }
    }
}
