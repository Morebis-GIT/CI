using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT16623_AddToleranceSpotsProgrammeRule : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        private const int ProgrammeRuleId = 38;
        private const string ProgrammeRuleName = "Programme";

        public UpdateStepXGGT16623_AddToleranceSpotsProgrammeRule(IEnumerable<string> tenantConnectionStrings) =>
            _tenantConnectionStrings = tenantConnectionStrings;

        public Guid Id => new Guid("bbc15ebe-f68d-461d-b75e-4684d1b95c45");

        public int Sequence => 1;

        public string Name => "XGGT-16623 Add Spot based Programme Rule to Pass Tolerances";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var connectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(connectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    InsertPassRule(session);
                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        private void InsertPassRule(IDocumentSession session)
        {
            IScenarioRepository scenarioRepository = new RavenScenarioRepository(session);
            IPassRepository passRepository = new RavenPassRepository(session);

            var librariedScenarios = scenarioRepository.GetAll()
                .Where(scenario => scenario.IsLibraried == true)
                .ToList();

            var librariedScenariosPassIds = librariedScenarios
                .SelectMany(scenario => scenario.Passes)
                .Select(passRef => passRef.Id)
                .ToHashSet();

            var librariedPasses = passRepository.GetAll()
                .Where(pass => pass.IsLibraried == true
                            || librariedScenariosPassIds.Contains(pass.Id))
                .ToList();

            foreach (var pass in librariedPasses)
            {
                if (!pass.Tolerances.Any(t => t.Description == ProgrammeRuleName && t.RuleId == ProgrammeRuleId))
                {
                    pass.Tolerances.Add(new Tolerance
                    {
                        RuleId = ProgrammeRuleId,
                        InternalType = "Campaign",
                        Description = ProgrammeRuleName,
                        Value = "0",
                        Type = "tolerances",
                        CampaignType = CampaignDeliveryType.Spot
                    });

                    session.Store(pass);
                }
            }
        }
    }
}
