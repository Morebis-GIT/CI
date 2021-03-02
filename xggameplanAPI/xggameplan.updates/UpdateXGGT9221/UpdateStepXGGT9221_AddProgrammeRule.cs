using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT9221_AddProgrammeRule : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT9221_AddProgrammeRule(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("D232E5E4-A8DF-480C-AE27-850E392FA089");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var programmeRuleId = (int) ToleranceRuleId.Programme;
                    var programmeRuleTypeId = (int)RuleCategory.Tolerances;
                    var rules = session.GetAll<Rule>();

                    if (!rules.Any(r => r.RuleId == programmeRuleId))
                    {
                        session.Store(
                            new Rule
                            {
                                RuleTypeId = programmeRuleTypeId,
                                RuleId = programmeRuleId,
                                Description = "Programme",
                                InternalType = "Campaign",
                                Type = "tolerances"
                            });

                        session.SaveChanges();
                    }

                    var isAllowedAutopilot = session.GetAll<RuleType>()
                        .Any(rt => rt.AllowedForAutopilot && rt.Id == programmeRuleTypeId);

                    var hasProgrammeAutopilotRule = session.GetAll<AutopilotRule>()
                        .Any(rt => rt.RuleId == programmeRuleId);

                    if (isAllowedAutopilot && !hasProgrammeAutopilotRule)
                    {
                        var newAutopilotRules = new List<AutopilotRule>();
                        newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, programmeRuleId, programmeRuleTypeId, 5, 5, 10, 10));
                        newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, programmeRuleId, programmeRuleTypeId, 10, 10, 20, 20));
                        newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, programmeRuleId, programmeRuleTypeId, 15, 15, 30, 30));
                        newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, programmeRuleId, programmeRuleTypeId, 20, 20, 40, 40));

                        foreach (var autopilotRule in newAutopilotRules)
                        {
                            session.Store(autopilotRule);
                        }

                        session.SaveChanges();
                    }
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-9221 : Update script for Programme Tolerances rule does not define the rule in Rules repository, only in all passes.";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
