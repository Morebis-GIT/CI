using System;
using System.Collections.Generic;
using Autofac;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Maintenance.DatabaseDetail;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Serilog;
using xggameplan.AuditEvents;
using xggameplan.common.Helpers;
using xggameplan.core.Extensions;
using xggameplan.utils.seeddata.Extensions;
using xggameplan.utils.seeddata.Migration.RavenToSql;
using xggameplan.utils.seeddata.SqlServer.Migration.Interfaces;

namespace xggameplan.utils.seeddata.Migration
{
    public class Migrator
    {
        #region SupportedMigrationDomainModelTypes
        private static readonly Type[] SupportedMigrationDomainModelTypes =
        {
            typeof(AccessToken),
            typeof(TenantProductFeature),
            typeof(ProductSettings),
            typeof(TaskInstance),
            typeof(User),
            typeof(Tenant),

            typeof(AutoBookDefaultParameters),
            typeof(AutoBookInstanceConfiguration),
            typeof(AutoBook),
            typeof(AutoBookSettings),
            typeof(AWSInstanceConfiguration),
            typeof(BookingPosition),
            typeof(BookingPositionGroup),
            typeof(Break),
            typeof(Campaign),
            typeof(Channel),
            typeof(ClashException),
            typeof(Clash),
            typeof(ClearanceCode),
            typeof(DatabaseDetails),
            typeof(Demographic),
            typeof(EfficiencySettings),
            typeof(EmailAuditEventSettings),
            typeof(Failures),
            typeof(Facility),
            typeof(FunctionalArea),
            typeof(IndexType),
            typeof(ISRSettings),
            typeof(KPIComparisonConfig),
            typeof(Language),
            typeof(LibrarySalesAreaPassPriority),
            typeof(Metadata),
            typeof(MSTeamsAuditEventSettings),
            typeof(ResultFileMigrationDocumentHandler.ResultFile),
            typeof(OutputFile),
            typeof(Pass),
            typeof(Product),
            typeof(ProductSettings),
            typeof(ProgrammeClassification),
            typeof(ProgrammeDictionary),
            typeof(Programme),
            typeof(RatingsPredictionSchedule),
            typeof(Recommendation),
            typeof(Restriction),
            typeof(RSSettings),
            typeof(Run),
            typeof(SalesArea),
            typeof(Scenario),
            typeof(ScenarioResult),
            typeof(ScenarioCampaignResult),
            typeof(Schedule),
            typeof(SmoothConfiguration),
            typeof(SmoothFailureMessage),
            typeof(SmoothFailure),
            typeof(Sponsorship),
            typeof(SpotPlacement),
            typeof(Spot),
            typeof(TaskInstance),
            typeof(TenantSettings),
            typeof(Universe),
            typeof(UpdateDetails),
            typeof(ISRGlobalSettings),
            typeof(RSGlobalSettings),
            typeof(RuleType),
            typeof(FlexibilityLevel),
            typeof(Rule),
            typeof(AutopilotRule),
            typeof(AutopilotSettings),
            typeof(KPIPriority),
            typeof(BRSConfigurationTemplate)
        };
        #endregion

        private readonly MigrationOptions _options;

        protected IEnumerable<Type> GetMigrationTypes() => SupportedMigrationDomainModelTypes;

        public Migrator(MigrationOptions options)
        {
            _options = options;
        }

        public void Run()
        {
            using var container = CreateMigrationContainer();
            var logger = container.Resolve<ILogger>();
            try
            {
                foreach (var action in container.Resolve<IEnumerable<IMigrationPrepareAction>>())
                {
                    action.Execute();
                }

                foreach (var migrationType in GetMigrationTypes())
                {
                    var handler =
                        container.ResolveOptional(
                                typeof(IMigrationDocumentHandler<>).MakeGenericType(migrationType)) as
                            IMigrationDocumentHandler;
                    if (handler?.Validate() ?? false)
                    {
                        logger.Information($"Migration of {migrationType.Name} has started.");
                        var result = StopwatchHelper.StopwatchFunc(() => handler.Execute(), out var watch);
                        logger.Information($"{result} {migrationType.Name} documents have been migrated successfully.");
                        logger.Debug($"Total time of {migrationType.Name} migration is {watch.Elapsed}." + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Migration has ended with an error.");
                throw;
            }
        }

        private IContainer CreateMigrationContainer()
        {
            var builder = new ContainerBuilder();

            _ = builder.AddAutoMapper(
                typeof(AutofacBuilderExtensions).Assembly,
                typeof(AccessTokenProfile).Assembly);

            _ = builder.RegisterModule(new MigrationModule(_options));
            builder.RegisterSerilog("serilogsettings.json");

            return builder.Build();
        }
    }
}
