using System.Linq.Expressions;
using ImagineCommunications.GamePlan.ReportSystem.Formaters;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig
{
    public class MemberOptions: IMemberOptions
    {
        public virtual string MemberName { get; }
        public virtual IFormatter Formatter { get; set; }
        public virtual string Header { get; set; }
        public virtual bool IsHeader { get; set; }
        public virtual bool Ignore { get; set; }
        public virtual Expression FormatterExpression { get; set; }
        public virtual int Order { get; set; }

        public MemberOptions()
        {
        }

        public MemberOptions(string memberName)
        {
            MemberName = memberName;
        }
    }
}
