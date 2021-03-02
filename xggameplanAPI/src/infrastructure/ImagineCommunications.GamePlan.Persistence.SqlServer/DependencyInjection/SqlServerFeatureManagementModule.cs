using System;
using Autofac;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.core.Validators.ProductAdvertiser;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    public class SqlServerFeatureManagementModule : Autofac.Module
    {
        private readonly IFeatureManager _featureManager;

        public SqlServerFeatureManagementModule(IFeatureManager featureManager)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var landmarkProductRelatedCollectionsFeatureFlag =
                _featureManager.IsEnabled(ProductFeature.LandmarkProductRelatedCollections);

            if (!landmarkProductRelatedCollectionsFeatureFlag)
            {
                return;
            }

            _ = builder.RegisterType<ProductAdvertiserValidator>().As<IProductAdvertiserValidator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<ClashExceptionSerializer>().As<IClashExceptionSerializer>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignSerializer>().As<ICampaignSerializer>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignFlattener>().As<ICampaignFlattener>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<RecommendationAggregator>().As<IRecommendationAggregator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignResultReportCreator>().As<IScenarioCampaignResultReportCreator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<RecommendationsResultReportCreator>().As<IRecommendationsResultReportCreator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<LandmarkSpotModelCreator>().As<ISpotModelCreator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterRepository<LandmarkCampaignRepository, ICampaignRepository>();
            _ = builder.RegisterRepository<LandmarkRestrictionRepository, IRestrictionRepository>();
            _ = builder.RegisterRepository<LandmarkProductRepository, IProductRepository>();

            _ = builder.RegisterType<LandmarkAnalysisGroupCampaignQuery>()
                .As<xggameplan.core.Services.IAnalysisGroupCampaignQuery>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<LandmarkProgTxDetailSerializer>().As<IProgTxDetailSerializer>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<BreakSerializer>().As<IBreakSerializer>()
                .InstancePerLifetimeScope();
        }
    }
}
