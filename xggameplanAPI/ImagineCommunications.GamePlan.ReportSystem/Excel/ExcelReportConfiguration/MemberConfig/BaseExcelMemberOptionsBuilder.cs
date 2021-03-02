using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig
{
    public abstract class BaseExcelMemberOptionsBuilder<TMemberOptionsBuilder, TMemberOptions>
        : MemberOptionsBuilder<TMemberOptionsBuilder, TMemberOptions>
        where TMemberOptionsBuilder : BaseExcelMemberOptionsBuilder<TMemberOptionsBuilder, TMemberOptions>
        where TMemberOptions : class, IExcelMemberOptions
    {
        protected BaseExcelMemberOptionsBuilder(TMemberOptions options) : base(options)
        {
        }

        public virtual TMemberOptionsBuilder ColSpan(int value)
        {
            Options.ColSpan = value;

            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder Merge()
        {
            return Merge(true);
        }

        public virtual TMemberOptionsBuilder Merge(bool merge)
        {
            Options.Merge = merge;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder Style(string predefinedStyleName)
        {
            Options.StyleName = predefinedStyleName;
            Options.IsStyleSet = true;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder Style(ExcelStyle style)
        {
            Options.Style = style;
            Options.IsStyleSet = true;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder HeaderStyle(string predefinedStyleName)
        {
            Options.HeaderStyleName = predefinedStyleName;
            Options.IsHeaderStyleSet = true;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder HeaderStyle(ExcelStyle style)
        {
            Options.HeaderStyle = style;
            Options.IsHeaderStyleSet = true;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder Background(Color color)
        {
            Options.BackgroundColor = color;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder HAlign(ExcelHorizontalAlignment alignment)
        {
            Options.HAlign = alignment;
            return (TMemberOptionsBuilder)this;
        }

        public virtual TMemberOptionsBuilder VAlign(ExcelVerticalAlignment alignment)
        {
            Options.VAlign = alignment;
            return (TMemberOptionsBuilder)this;
        }
    }
}
