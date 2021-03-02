using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT1778_AddPassNewGeneralRule : CodeUpdateStepBase, IUpdateStep
    {
        private IEnumerable<string> _tenantConnectionStrings;
        private string _updatesFolder;
        private string _rollBackFolder;

        public UpdateStepXGGT1778_AddPassNewGeneralRule(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("7BEF67CF-F3B5-411E-AE03-180252F88269");

        public int Sequence => 1;

        public string Name => "XGGT-1778";

        /// <summary>
        /// Added field IsLibraried for scenarios and passes
        /// </summary>
        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var passes = session.GetAll<Pass>();
                        // Add new General rule to the Pass with default value 0
                        passes
                            .ForEach(pass =>
                            {
                                if (!pass.General.Any(rule => rule.RuleId == (int)RuleID.UseCampaignMaxSpotRatings))
                                {
                                    pass.General.Add(new General
                                    {
                                        RuleId = (int)RuleID.UseCampaignMaxSpotRatings,
                                        Description = "Use Max Spot Ratings Set By Campaigns",
                                        InternalType = "Defaults",
                                        Type = "general",
                                        Value = "0"
                                    });
                                }
                            });

                        session.SaveChanges();
                    }
                }
            }
        }

        public bool SupportsRollback => false;

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
