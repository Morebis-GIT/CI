using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using xggameplan.AuditEvents;
using xggameplan.Database;

namespace xggameplan.core.Export
{
    public class TenantDatabaseProcessingSettings : DatabaseMassProcessingSettings
    {

        private void Init(string dataFolder = null)
        {
            DataFolder = dataFolder;
            SetDocumentTypesToProcess(_seedDataTypes);
        }

        public TenantDatabaseProcessingSettings(string dataFolder = null) => Init(dataFolder);

        private readonly List<Type> _seedDataTypes = new List<Type>()
                {
                    typeof(AutoBookDefaultParameters),
                    typeof(AutoBookInstanceConfiguration),
                    typeof(AutoBookSettings),
                    typeof(ClearanceCode),
                    typeof(EmailAuditEventSettings),
                    typeof(EfficiencySettings),
                    typeof(FunctionalArea),
                    typeof(IndexType),
                    typeof(Language),
                    typeof(Metadata),
                    typeof(MSTeamsAuditEventSettings),
                    typeof(OutputFile),
                    typeof(ProgrammeClassification),
                    typeof(SmoothConfiguration),
                    typeof(SmoothFailureMessage),
                    typeof(TenantSettings),
                    typeof(KPIComparisonConfig),
                    typeof(ProgrammeCategoryHierarchy)
                };
    }
}
