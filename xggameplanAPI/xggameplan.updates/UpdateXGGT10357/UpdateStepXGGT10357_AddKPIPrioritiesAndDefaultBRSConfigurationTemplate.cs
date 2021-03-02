using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Abstractions.Data;
using xggameplan.core.Helpers;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10357
{
    internal class UpdateStepXGGT10357_AddKPIPrioritiesAndDefaultBRSConfigurationTemplate : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT10357_AddKPIPrioritiesAndDefaultBRSConfigurationTemplate(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("A1ED9E4F-24B9-4B3E-B952-DF515BDA746A");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var kpiPriorities = new List<KPIPriority>
                    {
                        new KPIPriority { Id = 1, Name = "Exclude", WeightingFactor = 0 },
                        new KPIPriority { Id = 2, Name = "Extremely Low", WeightingFactor = 0.3 },
                        new KPIPriority { Id = 3, Name = "Low", WeightingFactor = 0.7 },
                        new KPIPriority { Id = 4, Name = "Medium", WeightingFactor = 1 },
                        new KPIPriority { Id = 5, Name = "High", WeightingFactor = 1.3 },
                        new KPIPriority { Id = 6, Name = "Extremely High", WeightingFactor = 1.7 },
                    };
                    using (var bulkInsert = session.Advanced.DocumentStore.BulkInsert(null, new BulkInsertOptions() { OverwriteExisting = true }))
                    {
                        kpiPriorities.ToList().ForEach(item => bulkInsert.Store(item));
                    }

                    var template = new BRSConfigurationTemplate
                    {
                        Name ="Default template",
                        IsDefault = true,
                        LastModified = DateTime.UtcNow,
                        KPIConfigurations = BRSHelper.KPIs.Select(kpi => new BRSConfigurationForKPI
                        {
                            KPIName = kpi,
                            PriorityId = 4
                        }).ToList()
                    };

                    session.Store(template);
                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-10357: Add KPI Priorities and default BRS configuration template";

        public bool SupportsRollback => false;
    }
}
