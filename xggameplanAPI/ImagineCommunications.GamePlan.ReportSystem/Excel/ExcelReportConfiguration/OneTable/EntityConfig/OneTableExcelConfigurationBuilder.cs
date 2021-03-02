using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig
{
    public class OneTableExcelConfigurationBuilder<TEntity>
        : ConfigurationBuilder<TEntity, OneTableExcelMemberOptionsBuilder, OneTableExcelConfigurationBuilder<TEntity>,
            OneTableExcelReportConfiguration, OneTableExcelMemberOptions>
        where TEntity : class
    {
        protected new virtual IExcelConfigurationOptions Options { get; set; }

        public OneTableExcelConfigurationBuilder(IExcelConfigurationOptions options) : base(options)
        {
            Options = options;
        }

        public OneTableExcelConfigurationBuilder() : this(new ExcelConfigurationOptions())
        {
        }

        public OneTableExcelConfigurationBuilder<TEntity> HeaderStyle(string predefinedStyleName)
        {
            Options.HeaderStyleName = predefinedStyleName;
            Options.IsSetHeaderStyle = true;
            return this;
        }

        public OneTableExcelConfigurationBuilder<TEntity> HeaderStyle(ExcelStyle style)
        {
            Options.HeaderStyle = style;
            Options.IsSetHeaderStyle = true;
            return this;
        }

        public OneTableExcelConfigurationBuilder<TEntity> DefaultStyle(string predefinedStyleName)
        {
            Options.DefaultStyleName = predefinedStyleName;
            Options.IsSetDefaultStyle = true;
            return this;
        }

        public OneTableExcelConfigurationBuilder<TEntity> AlternateBackgroundColors(Color[] alternateBackgroundColors)
        {
            Options.AlternateBackgroundColors = alternateBackgroundColors
                .Select(x => new KeyValuePair<string, Color>(x.ToArgb().ToString("X"), x)).ToList();
            Options.HasAlternateBackgroundColor = true;
            return this;
        }

        public OneTableExcelConfigurationBuilder<TEntity> DefaultStyle(ExcelStyle style)
        {
            Options.DefaultStyle = style;
            Options.IsSetDefaultStyle = true;
            return this;
        }

        public override OneTableExcelReportConfiguration BuildConfiguration()
        {
            return BuildConfiguration<ExcelConfigurationOptions>();
        }
    }
}
