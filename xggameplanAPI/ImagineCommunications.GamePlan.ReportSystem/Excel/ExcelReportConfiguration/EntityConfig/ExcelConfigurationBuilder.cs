using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig
{
    public class ExcelConfigurationBuilder<TEntity>
        : ConfigurationBuilder<TEntity, ExcelMemberOptionsBuilder
            , ExcelConfigurationBuilder<TEntity>, ExcelReportConfiguration, ExcelMemberOptions>
        where TEntity : class
    {
        protected virtual IExcelConfigurationOptions Options { get; set; }

        public ExcelConfigurationBuilder(): this(new ExcelConfigurationOptions())
        {

        }

        public ExcelConfigurationBuilder(IExcelConfigurationOptions options) : base(options)
        {
            Options = options;
        }

        public ExcelConfigurationBuilder<TEntity> HeaderStyle(string predefinedStyleName)
        {
            Options.HeaderStyleName = predefinedStyleName;
            Options.IsSetHeaderStyle = true;
            return this;
        }

        public ExcelConfigurationBuilder<TEntity> HeaderStyle(ExcelStyle style)
        {
            Options.HeaderStyle = style;
            Options.IsSetHeaderStyle = true;
            return this;
        }

        public ExcelConfigurationBuilder<TEntity> DefaultStyle(string predefinedStyleName)
        {
            Options.DefaultStyleName = predefinedStyleName;
            Options.IsSetDefaultStyle = true;
            return this;
        }

        public ExcelConfigurationBuilder<TEntity> AlternateBackgroundColors(Color[] alternateBackgroundColors)
        {
            Options.AlternateBackgroundColors = alternateBackgroundColors
                .Select(x => new KeyValuePair<string, Color>(x.ToArgb().ToString("X"), x)).ToList();
            Options.HasAlternateBackgroundColor = true;
            return this;
        }

        public ExcelConfigurationBuilder<TEntity> DefaultStyle(ExcelStyle style)
        {
            Options.DefaultStyle = style;
            Options.IsSetDefaultStyle = true;
            return this;
        }

        public override ExcelReportConfiguration BuildConfiguration()
        {
            return BuildConfiguration<ExcelConfigurationOptions>();
        }
    }
}
