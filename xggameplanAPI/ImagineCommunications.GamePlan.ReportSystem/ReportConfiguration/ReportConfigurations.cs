using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration
{
    public abstract class ReportConfigurations<TConfOptions, TMemberOptions> : IReportConfigurations<TConfOptions, TMemberOptions>
        where TMemberOptions : IMemberOptions
        where TConfOptions : IConfigurationOptions
    {
        public virtual TConfOptions Options { get; }
        public virtual MemberConfigurationDictionary<TMemberOptions> MemberConfigurations { get; }

        protected ReportConfigurations(TConfOptions options, MemberConfigurationDictionary<TMemberOptions> memberConfigurations)
        {
            Options = options;
            MemberConfigurations = memberConfigurations;
        }
    }
}
