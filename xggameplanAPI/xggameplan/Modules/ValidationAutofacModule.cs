using System.Collections.Generic;
using Autofac;
using FluentValidation;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.Validations;
using xggameplan.Validations.AnalysisGroups;
using xggameplan.Validations.AutopilotSettings;
using xggameplan.Validations.BRS;
using xggameplan.Validations.DeliveryCappingGroup;
using xggameplan.Validations.Landmark;

namespace xggameplan.Modules
{
    public class ValidationAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Add custom validators here
            builder.RegisterType<CreateLibrarySalesAreaPassPriorityModelValidator>()
                   .As<IModelDataValidator<CreateLibrarySalesAreaPassPriorityModel>>().InstancePerDependency();
            builder.RegisterType<UpdateLibrarySalesAreaPassPriorityModelValidator>()
                   .As<IModelDataValidator<UpdateLibrarySalesAreaPassPriorityModel>>().InstancePerDependency();
            builder.RegisterType<CreateProductValidator>()
                .As<IModelDataValidator<CreateProduct>>().InstancePerDependency();         
            builder.RegisterType<AutopilotSettingsModelValidator>()
                .As<IModelDataValidator<UpdateAutopilotSettingsModel>>().InstancePerDependency();
            builder.RegisterType<AutopilotEngageModelValidator>()
                .As<IModelDataValidator<AutopilotEngageModel>>().InstancePerDependency();
            builder.RegisterType<CreateSponsorshipsModelValidator>()
                .As<IModelDataValidator<IEnumerable<CreateSponsorshipModel>>>().InstancePerDependency();
            builder.RegisterType<UpdateSponsorshipModelValidator>()
                .As<IModelDataValidator<UpdateSponsorshipModel>>().InstancePerDependency();
            builder.RegisterType<LandmarkRunTriggerModelValidator>()
                .As<IModelDataValidator<LandmarkRunTriggerModel>>().InstancePerDependency();
            builder.RegisterType<ScheduledRunSettingsModelValidator>()
                .As<IModelDataValidator<ScheduledRunSettingsModel>>().InstancePerDependency();
            builder.RegisterType<CampaignPriorityRoundsModelValidator>()
                .As<IModelDataValidator<CampaignPriorityRoundsModel>>().InstancePerDependency();
            builder.RegisterType<CreateOrUpdateBRSConfigurationTemplateValidator>()
               .As<IModelDataValidator<CreateOrUpdateBRSConfigurationTemplateModel>>().InstancePerDependency();
            builder.Register<DeliveryCappingGroupValidator>(DeliveryCappingGroupValidationAutofacRegistration.GetValidator)
                .As<IModelDataValidator<DeliveryCappingGroupModel>>().InstancePerDependency();
            builder.RegisterType<CreateAnalysisGroupModelValidator>()
                .As<IModelDataValidator<CreateAnalysisGroupModel>>().InstancePerDependency();

            //This will Add all fluentvalidation implementations
            foreach (var r in AssemblyScanner.FindValidatorsInAssembly(typeof(ValidationAutofacModule).Assembly))
            {
                builder.RegisterType(r.ValidatorType).As(r.InterfaceType);
            }
        }
    }
}
