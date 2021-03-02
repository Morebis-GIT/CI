namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig
{
    public class ExcelMemberOptionsBuilder
        : BaseExcelMemberOptionsBuilder<ExcelMemberOptionsBuilder, ExcelMemberOptions>
            , IBaseExcelMemberOptionsBuilder<ExcelMemberOptionsBuilder, ExcelMemberOptions>
    {
        public ExcelMemberOptionsBuilder(string memberName) : this(new ExcelMemberOptions(memberName))
        {

        }

        public ExcelMemberOptionsBuilder(ExcelMemberOptions options) : base(options)
        {
        }
    }
}
