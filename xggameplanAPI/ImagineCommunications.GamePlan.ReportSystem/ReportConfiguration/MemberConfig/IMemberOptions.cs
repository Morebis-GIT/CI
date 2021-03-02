using System.Linq.Expressions;
using ImagineCommunications.GamePlan.ReportSystem.Formaters;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig
{
    public interface IMemberOptions
    {
        string MemberName { get; }
        IFormatter Formatter { get; set; }
        Expression FormatterExpression { get; set; }
        string Header { get; set; }
        bool IsHeader { get; set; }
        bool Ignore { get; set; }
        int Order { get; set; }
    }
}
