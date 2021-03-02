using Autofac;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using xggameplan.AuditEvents;
using xggameplan.utils.seeddata.Seeding;
using AutopilotSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotSettings;
using BookingPositionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPosition;
using BookingPositionGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup;
using FacilityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Facility;
using FlexibilityLevelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FlexibilityLevel;
using LibrarySalesAreaPassPriorityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.LibrarySalesAreaPassPriority;
using RuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Rule;
using RuleTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RuleType;
using TenantEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.Tenant;
using TenantProductFeatureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeature;
using UserEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.User;

namespace xggameplan.utils.seeddata.SqlServer
{
    public class SqlServerImportersModule : Module
    {
        private readonly DatabaseType _databaseType;

        public SqlServerImportersModule(DatabaseType databaseType)
        {
            _databaseType = databaseType;
        }

        protected override void Load(ContainerBuilder builder)
        {
            switch (_databaseType)
            {
                case DatabaseType.Master:
                    _ = builder.RegisterType<JsonFileImporter<AccessToken>>()
                        .Keyed<IJsonFileImporter>(typeof(AccessToken));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<Tenant, TenantEntity>>()
                       .Keyed<IJsonFileImporter>(typeof(Tenant));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<TenantProductFeature, TenantProductFeatureEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(TenantProductFeature));
                    _ = builder.RegisterType<JsonFileImporter<UpdateDetails>>()
                        .Keyed<IJsonFileImporter>(typeof(UpdateDetails));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<User, UserEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(User));
                    break;
                case DatabaseType.Tenant:
                    _ = builder.RegisterType<JsonFileImporter<AutoBookDefaultParameters>>()
                        .Keyed<IJsonFileImporter>(typeof(AutoBookDefaultParameters));
                    _ = builder.RegisterType<JsonFileImporter<AutoBookInstanceConfiguration>>()
                        .Keyed<IJsonFileImporter>(typeof(AutoBookInstanceConfiguration));
                    _ = builder.RegisterType<JsonFileImporter<AutoBookSettings>>()
                        .Keyed<IJsonFileImporter>(typeof(AutoBookSettings));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<BookingPosition, BookingPositionEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(BookingPosition))
                        .OnActivating(x => x.Instance.TableName = $"{nameof(BookingPosition)}s");
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<BookingPositionGroup, BookingPositionGroupEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(BookingPositionGroup));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<Facility, FacilityEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(Facility));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<FlexibilityLevel, FlexibilityLevelEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(FlexibilityLevel));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<RuleType, RuleTypeEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(RuleType));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<Rule, RuleEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(Rule));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<AutopilotSettings, AutopilotSettingsEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(AutopilotSettings));
                    _ = builder.RegisterType<JsonFileImporter<AutopilotRule>>()
                        .Keyed<IJsonFileImporter>(typeof(AutopilotRule));
                    _ = builder.RegisterType<JsonFileImporter<AWSInstanceConfiguration>>()
                        .Keyed<IJsonFileImporter>(typeof(AWSInstanceConfiguration));
                    _ = builder.RegisterType<JsonFileImporter<ClearanceCode>>()
                        .Keyed<IJsonFileImporter>(typeof(ClearanceCode));
                    _ = builder.RegisterType<JsonFileImporter<EfficiencySettings>>()
                        .Keyed<IJsonFileImporter>(typeof(EfficiencySettings));
                    _ = builder.RegisterType<JsonFileImporter<EmailAuditEventSettings>>()
                        .Keyed<IJsonFileImporter>(typeof(EmailAuditEventSettings));
                    _ = builder.RegisterType<JsonFileImporter<FunctionalArea>>()
                        .Keyed<IJsonFileImporter>(typeof(FunctionalArea));
                    _ = builder.RegisterType<JsonFileImporter<IndexType>>()
                        .Keyed<IJsonFileImporter>(typeof(IndexType));
                    _ = builder.RegisterType<JsonFileImporter<KPIComparisonConfig>>()
                        .Keyed<IJsonFileImporter>(typeof(KPIComparisonConfig));
                    _ = builder.RegisterType<JsonFileImporter<Language>>()
                        .Keyed<IJsonFileImporter>(typeof(Language));
                    _ = builder.RegisterType<JsonFileSqlServerIdentityImporter<LibrarySalesAreaPassPriority, LibrarySalesAreaPassPriorityEntity>>()
                        .Keyed<IJsonFileImporter>(typeof(LibrarySalesAreaPassPriority));
                    _ = builder.RegisterType<JsonFileImporter<Metadata>>()
                        .Keyed<IJsonFileImporter>(typeof(Metadata));
                    _ = builder.RegisterType<JsonFileImporter<MSTeamsAuditEventSettings>>()
                        .Keyed<IJsonFileImporter>(typeof(MSTeamsAuditEventSettings));
                    _ = builder.RegisterType<JsonFileImporter<OutputFile>>()
                        .Keyed<IJsonFileImporter>(typeof(OutputFile));
                    _ = builder.RegisterType<JsonFileImporter<Pass>>()
                        .Keyed<IJsonFileImporter>(typeof(Pass));
                    _ = builder.RegisterType<JsonFileImporter<Scenario>>()
                        .Keyed<IJsonFileImporter>(typeof(Scenario));
                    _ = builder.RegisterType<JsonFileImporter<ProgrammeClassification>>()
                        .Keyed<IJsonFileImporter>(typeof(ProgrammeClassification));
                    _ = builder.RegisterType<JsonFileImporter<SmoothConfiguration>>()
                        .Keyed<IJsonFileImporter>(typeof(SmoothConfiguration));
                    _ = builder.RegisterType<JsonFileImporter<SmoothFailureMessage>>()
                        .Keyed<IJsonFileImporter>(typeof(SmoothFailureMessage));
                    _ = builder.RegisterType<JsonFileImporter<TenantSettings>>()
                        .Keyed<IJsonFileImporter>(typeof(TenantSettings));
                    _ = builder.RegisterType<JsonFileImporter<ProgrammeCategoryHierarchy>>()
                        .Keyed<IJsonFileImporter>(typeof(ProgrammeCategoryHierarchy));
                    _ = builder.RegisterType<JsonFileImporter<KPIPriority>>()
                        .Keyed<IJsonFileImporter>(typeof(KPIPriority));
                    _ = builder.RegisterType<JsonFileImporter<BRSConfigurationTemplate>>()
                        .Keyed<IJsonFileImporter>(typeof(BRSConfigurationTemplate));
                    break;
            }
        }
    }
}
