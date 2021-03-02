using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.MemberConfig
{
    public class OneTableExcelMemberOptionsBuilder
        : BaseExcelMemberOptionsBuilder<OneTableExcelMemberOptionsBuilder, OneTableExcelMemberOptions>
            , IOneTableExcelMemberOptionsBuilder
    {
        public OneTableExcelMemberOptionsBuilder(string memberName)
            : this(new OneTableExcelMemberOptions(memberName))
        {

        }

        public OneTableExcelMemberOptionsBuilder(OneTableExcelMemberOptions options) : base(options)
        {
        }


        public IOneTableExcelMemberOptionsBuilder Width(double width)
        {
            Options.Width = width;
            return this;
        }

        public IOneTableExcelMemberOptionsBuilder AutoFitColumn(bool isAutoFitColumn)
        {
            Options.IsAutoFitColumn = isAutoFitColumn;
            return this;
        }

        public IOneTableExcelMemberOptionsBuilder Format(string format)
        {
            Options.Format = format;
            return this;
        }
    }
}
