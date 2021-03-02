using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Client;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT15514
{
    internal class UpdateStepXGGT15514_FixFitToSpotLengthRuleTypo : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private const string Typo = "FIT TO SPOT LENGT"; // uppercased for easy comparison
        private const string CorrectSpelling = "Fit to Spot Length";

        public UpdateStepXGGT15514_FixFitToSpotLengthRuleTypo(IEnumerable<string> tenantConnectionStrings) =>
            _tenantConnectionStrings = tenantConnectionStrings;

        public Guid Id => new Guid("C70E6297-0E66-4446-87BE-E2196E758232");

        public int Sequence => 1;

        public string Name => "XGGT-15514";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var connectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(connectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    CorrectRules(session);
                    CorrectPassRules(session);
                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        private void CorrectRules(IDocumentSession session)
        {
            var rules = session.Query<Rule>().ToList();

            var rulesContainingTypo = rules.Where(r => ContainsTypo(r.Description));

            foreach (var rule in rulesContainingTypo)
            {
                rule.Description = CorrectSpelling;
                session.Store(rule);
            }
        }

        private void CorrectPassRules(IDocumentSession session)
        {
            var passes = session.Query<Pass>().ToList();

            var passesContainingRulesWithTypo = passes
                .Where(p => p.Weightings.Any(w => ContainsTypo(w.Description)));

            foreach (var pass in passesContainingRulesWithTypo)
            {
                var passUpdated = false;

                foreach (var weighting in pass.Weightings)
                {
                    if (ContainsTypo(weighting.Description))
                    {
                        weighting.Description = CorrectSpelling;
                        passUpdated = true;
                    }
                }

                if (passUpdated)
                {
                    session.Store(pass);
                }
            }
        }

        private static bool ContainsTypo(string text) => text.Equals(Typo, StringComparison.OrdinalIgnoreCase);
    }
}
