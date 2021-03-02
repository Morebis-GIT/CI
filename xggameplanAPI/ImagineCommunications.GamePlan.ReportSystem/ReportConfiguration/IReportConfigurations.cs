using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration
{
    public interface IReportConfigurations<out TConfOptions, TMemberOptions>
        : IReportConfigurations where TMemberOptions: IMemberOptions
    {
        TConfOptions Options { get; }
        MemberConfigurationDictionary<TMemberOptions> MemberConfigurations { get; }
    }

    public interface IReportConfigurations
    {

    }
}
