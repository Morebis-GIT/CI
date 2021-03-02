using Autofac;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using xggameplan.Areas.System.Auth;
using xggameplan.Autopilot;
using xggameplan.Contexts;
using xggameplan.core.Validations;
using xggameplan.core.Validators;
using xggameplan.Reports;
using xggameplan.Reports.Common;
using xggameplan.Reports.DataAdapters;
using xggameplan.Reports.ExcelReports.Campaigns;

namespace xggameplan.Modules
{
    public class WebAutofacModule : Module
    {
        /// <summary>
        /// Registers all services located in the Web project to the dependency resolver.
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            //System
            builder.Register(x =>
            {
                var productSettingsRepository = x.Resolve<IProductSettingsRepository>();
                return new SystemContext(productSettingsRepository.Get(1));
            });
            builder.RegisterType<AuthorizationManager>().As<IAuthorizationManager>().InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationManager>().As<IAuthenticationManager>().InstancePerLifetimeScope();
            

            //Validators
            builder.RegisterModule<ValidationAutofacModule>();
            builder.RegisterType<ClashValidator>().As<IClashValidator>();
            builder.RegisterType<ClashExceptionValidations>().As<IClashExceptionValidations>();

            //Excel Reports
            builder.RegisterType<ExcelReportGenerator>().As<IExcelReportGenerator>();
            builder.RegisterType<RunExcelReportDataAdapter>().As<IRunExcelReportDataAdapter>();
            builder.RegisterType<ReportColumnFormatter>().As<IReportColumnFormatter>();

            //Recommendations Excel Reports
            builder.RegisterType<CampaignExcelReportGenerator>().As<ICampaignExcelReportGenerator>();
            //Autopilot
            builder.RegisterType<AutopilotManager>().As<IAutopilotManager>().InstancePerLifetimeScope();
        }

    }
}
