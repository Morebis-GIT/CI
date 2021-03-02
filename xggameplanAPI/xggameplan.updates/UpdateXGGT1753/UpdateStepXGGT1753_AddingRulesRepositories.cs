using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT1753_AddingRulesRepositories : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT1753_AddingRulesRepositories(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("8A971ADC-3CC1-4AF8-AF5D-79B3D7F88B01");

        public string Name => "XGGT-1753";

        public int Sequence => 1;

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var rules4Delete = session.GetAll<Rule>();
                    var ruleTypes4Delete = session.GetAll<RuleType>();
                    var flexLevels4Delete = session.GetAll<FlexibilityLevel>();
                    var needSaveChanges = false;

                    if (rules4Delete != null && rules4Delete.Any())
                    {
                        needSaveChanges = true;
                        rules4Delete.ForEach(item => session.Delete(item));
                    }

                    if (ruleTypes4Delete != null && ruleTypes4Delete.Any())
                    {
                        needSaveChanges = true;
                        ruleTypes4Delete.ForEach(item => session.Delete(item));
                    }

                    if (flexLevels4Delete != null && flexLevels4Delete.Any())
                    {
                        needSaveChanges = true;
                        flexLevels4Delete.ForEach(item => session.Delete(item));
                    }

                    if (needSaveChanges)
                    {
                        session.SaveChanges();
                    }

                    // Get default Pass to extract rules
                    var defaultScenarioId = session.Load<TenantSettings>().FirstOrDefault().DefaultScenarioId;
                    var defaultPassReference = session.Load<Scenario>(defaultScenarioId).Passes.OrderBy(p => p.Id).First();
                    var defaultPass = session.Load<Pass>(defaultPassReference.Id);

                    // Add flexibility levels
                    session.Store(new FlexibilityLevel {Name = "Low", Id = 1});
                    session.Store(new FlexibilityLevel {Name = "Medium", Id = 2});
                    session.Store(new FlexibilityLevel {Name = "High", Id = 3});
                    session.Store(new FlexibilityLevel {Name = "Extreme", Id = 4});

                    // Add rule types
                    session.Store(NewRuleType((int)RuleCategory.General, "General", true, false));
                    session.Store(NewRuleType((int)RuleCategory.Weighting, "Weighting", false, false));
                    session.Store(NewRuleType((int)RuleCategory.Tolerances, "Tolerances", true, false));
                    session.Store(NewRuleType((int)RuleCategory.Rules, "Rules", true, false));
                    session.Store(NewRuleType((int)RuleCategory.SlottingLimits, "Slotting Limits", true, true));

                    // Add General rules
                    defaultPass.General.ForEach(rule =>
                    {
                        session.Store(
                            new Rule
                            {
                                RuleId = rule.RuleId,
                                RuleTypeId = (int)RuleCategory.General,
                                InternalType = rule.InternalType,
                                Description = rule.Description,
                                Type = rule.Type
                            });
                    });

                    // Add Weighting rules
                    defaultPass.Weightings.ForEach(rule =>
                    {
                        session.Store(
                            new Rule
                            {
                                RuleId = rule.RuleId,
                                RuleTypeId = (int)RuleCategory.Weighting,
                                InternalType = rule.InternalType,
                                Description = rule.Description,
                                Type = rule.Type
                            });
                    });

                    // Add Tolerance rules
                    defaultPass.Tolerances.ForEach(rule =>
                    {
                        session.Store(
                            new Rule
                            {
                                RuleId = rule.RuleId,
                                RuleTypeId = (int)RuleCategory.Tolerances,
                                InternalType = rule.InternalType,
                                Description = rule.Description,
                                Type = rule.Type
                            });
                    });

                    // Add PassRule rules
                    defaultPass.Rules.ForEach(rule =>
                    {
                        session.Store(
                            new Rule
                            {
                                RuleId = rule.RuleId,
                                RuleTypeId = (int)RuleCategory.Rules,
                                InternalType = rule.InternalType,
                                Description = rule.Description,
                                Type = rule.Type
                            });
                    });

                    // Add default Slotting Limits rule
                    session.Store(
                        new Rule
                        {
                            RuleId = 1,
                            RuleTypeId = (int)RuleCategory.SlottingLimits,
                            InternalType = "SlottingLimits",
                            Description = "Default Settings",
                            Type = "slotting limits"
                        });

                    session.SaveChanges();
                }
            }
        }

        private static RuleType NewRuleType(int id, string name, bool allowedForAutopilot, bool isCustom)
        {
            return new RuleType
            {
                Id = id,
                Name = name,
                AllowedForAutopilot = allowedForAutopilot,
                IsCustom = isCustom
            };
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
