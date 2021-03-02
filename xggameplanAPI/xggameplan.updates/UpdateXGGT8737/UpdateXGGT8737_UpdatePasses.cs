using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;

namespace xggameplan.Updates
{
    class UpdateXGGT8737_UpdatePasses : CodeUpdateStepBase, IUpdateStep
    {
        private IEnumerable<string> _tenantConnectionStrings;
        private string _updatesFolder;
        private string _rollBackFolder;

        public UpdateXGGT8737_UpdatePasses(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id
        {
            get { return new Guid("FA9A10CD-CFBA-4B92-96A6-CF06D796D514"); }
        }

        public int Sequence
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "XGGT-8737"; }
        }

        /// <summary>
        /// Adding "Sponsorship Exclusivity" to passes
        /// </summary>
        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var passRepository = new RavenPassRepository(session);

                        var passes = passRepository.GetAll();
                        foreach (var pass in passes)
                        {
                            var updated = false;
                            if (pass.General != null && pass.General.Count > 0)
                            {
                                var sponsorshipExclusivityItem = pass.General.Find(item => item.RuleId == 6);
                                if (sponsorshipExclusivityItem == null)
                                {
                                    sponsorshipExclusivityItem = new General()
                                    {
                                        RuleId = 6,
                                        InternalType = "Defaults",
                                        Description = "Sponsorship Exclusivity",
                                        Value = "1",
                                        Type = "general"
                                    };
                                    pass.General.Add(sponsorshipExclusivityItem);
                                    updated = true;
                                }
                                else if (!(sponsorshipExclusivityItem.Value == "0" || sponsorshipExclusivityItem.Value == "1"))
                                {
                                    sponsorshipExclusivityItem.Description = "Sponsorship Exclusivity";
                                    sponsorshipExclusivityItem.Value = "1";
                                    updated = true;
                                }
                                if (updated)
                                {
                                    passRepository.Update(pass);
                                }
                            }
                        }
                        passRepository.SaveChanges();
                    }
                }
            }
        }

        public bool SupportsRollback
        {
            get { return false; }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
