using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;

namespace xggameplan.Updates
{
    class UpdateXGGT8737_CreateRule : CodeUpdateStepBase, IUpdateStep
    {
        private IEnumerable<string> _tenantConnectionStrings;
        private string _updatesFolder;
        private string _rollBackFolder;

        public UpdateXGGT8737_CreateRule(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id
        {
            get { return new Guid("833CE351-265E-4656-8B1C-76D040CE6E0E"); }
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
        /// add "Sponsorship Exclusivity" to Rule Document
        /// </summary>
        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var ruleRepository = new RavenRuleRepository(session);

                        var rules = ruleRepository.GetAll();
                        if (!rules.Any(r => r.RuleId == 6 && r.RuleTypeId == 1))
                        {
                            var rule = new Rule()
                            {
                                RuleId = 6,
                                RuleTypeId = 1,
                                InternalType = "Defaults",
                                Description = "Sponsorship Exclusivity",
                                Type = "general"
                            };
                            ruleRepository.Add(rule);
                            ruleRepository.SaveChanges();
                        }
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
