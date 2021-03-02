using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT4255_AddPassNewGeneralRules : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT4255_AddPassNewGeneralRules(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("B79BB5A1-6E06-4F78-8B56-1041946EE258");

        public string Name => "XGGT-4255";

        public int Sequence => 1;

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var passes = session.GetAll<Pass>();

                    // Add new General rules to the Pass with default values
                    foreach (var pass in passes)
                    {
                        if (pass.General.All(rule => rule.RuleId != (int)RuleID.EvenDistributionZeroRatingSpots))
                        {
                            pass.General.Add(new General
                            {
                                RuleId = (int)RuleID.EvenDistributionZeroRatingSpots,
                                Description = "Even Distribution of Zero Rating Spots",
                                InternalType = "Defaults",
                                Type = "general",
                                Value = "0"
                            });
                        }
                        if (pass.General.All(rule => rule.RuleId != (int)RuleID.ZeroRatedBreaks))
                        {
                            pass.General.Add(new General
                            {
                                RuleId = (int)RuleID.ZeroRatedBreaks,
                                Description = "Zero Rated Breaks",
                                InternalType = "Defaults",
                                Type = "general",
                                Value = "0"
                            });
                        }
                    }
                    session.SaveChanges();
                }
            }
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
