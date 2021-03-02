using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT1888_AddingDefaultAutopilotSettings : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT1888_AddingDefaultAutopilotSettings(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("FB890F83-ADA1-422F-9B62-F9D1B5A064F6");

        public int Sequence => 1;

        public string Name => "XGGT-1888";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    // Default autopilot settings
                    session.Store(new AutopilotSettings {Id = 1, DefaultFlexibilityLevelId = 1, ScenariosToGenerate = 8});

                    var newAutopilotRules = new List<AutopilotRule>();

                    #region General Rules

                    // Minimum Efficiency
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 1, (int)RuleCategory.General, 5, -5, 10, -10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 1, (int)RuleCategory.General, 10, -10, 20, -20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 1, (int)RuleCategory.General, 15, -15, 30, -30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 1, (int)RuleCategory.General, 20, -20, 40, -40));
                    // Maximum Rank
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 2, (int)RuleCategory.General, -5, 5, -10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 2, (int)RuleCategory.General, -10, 10, -20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 2, (int)RuleCategory.General, -15, 15, -30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 2, (int)RuleCategory.General, -20, 20, -40, 40));
                    // Demograph Banding Tolerance
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 3, (int)RuleCategory.General, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 3, (int)RuleCategory.General, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 3, (int)RuleCategory.General, -5, 5, -10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 3, (int)RuleCategory.General, -10, 10, -15, 15));

                    #endregion

                    #region Tolerances Rules

                    // Campaign
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 1, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 1, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 1, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 1, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Booking Position
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 8, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 8, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 8, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 8, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Centre/End
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 9, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 9, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 9, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 9, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Sales Area
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 2, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 2, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 2, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 2, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Spot Length
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 3, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 3, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 3, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 3, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Daypart
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 4, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 4, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 4, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 4, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Strike Weight
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 5, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 5, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 5, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 5, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Strike Wgt/Daypart
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 6, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 6, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 6, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 6, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Peak Daypart
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 7, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 7, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 7, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 7, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Daypart/Length
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 12, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 12, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 12, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 12, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Strike Weight/Length
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 13, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 13, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 13, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 13, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Strike Weight/Daypart/Length
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 14, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 14, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 14, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 14, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Campaign (Budget)
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 10, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 10, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 10, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 10, (int)RuleCategory.Tolerances, 20, 20, 40, 40));
                    // Sales Area (Budget)
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 11, (int)RuleCategory.Tolerances, 5, 5, 10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 11, (int)RuleCategory.Tolerances, 10, 10, 20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 11, (int)RuleCategory.Tolerances, 15, 15, 30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 11, (int)RuleCategory.Tolerances, 20, 20, 40, 40));

                    #endregion

                    #region Rules

                    // Max Spots Per Day
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 1, (int)RuleCategory.Rules, -5, 5, -10, 10));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 1, (int)RuleCategory.Rules, -10, 10, -20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 1, (int)RuleCategory.Rules, -15, 15, -30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 1, (int)RuleCategory.Rules, -20, 20, -40, 40));
                    // Max Spots Per Hour
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 2, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 2, (int)RuleCategory.Rules, 0, 0, -20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 2, (int)RuleCategory.Rules, -20, 20, -40, 40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 2, (int)RuleCategory.Rules, -40, 40, -80, 80));
                    // Max Spots Per 2 Hours
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 3, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 3, (int)RuleCategory.Rules, 0, 0, -20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 3, (int)RuleCategory.Rules, -20, 20, -40, 40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 3, (int)RuleCategory.Rules, -40, 40, -80, 80));
                    // Min Breaks Between Spots
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 4, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 4, (int)RuleCategory.Rules, 0, 0, 20, -20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 4, (int)RuleCategory.Rules, 20, -20, 40, -40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 4, (int)RuleCategory.Rules, 40, -40, 80, -80));
                    // Min Hours Between Spots
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 5, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 5, (int)RuleCategory.Rules, 0, 0, 20, -20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 5, (int)RuleCategory.Rules, 20, -20, 40, -40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 5, (int)RuleCategory.Rules, 40, -40, 80, -80));
                    // Max Spots Per Programme/Day
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 6, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 6, (int)RuleCategory.Rules, 0, 0, 20, -20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 6, (int)RuleCategory.Rules, 20, -20, 40, -40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 6, (int)RuleCategory.Rules, 40, -40, 80, -80));
                    // Max Spots Per Programme/Week
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 7, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 7, (int)RuleCategory.Rules, 0, 0, -15, 15));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 7, (int)RuleCategory.Rules, -15, 15, -30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 7, (int)RuleCategory.Rules, -30, 30, -60, 60));
                    // Max Spots per Prog/100 rtgs
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 17, (int)RuleCategory.Rules, -10, 10, -20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 17, (int)RuleCategory.Rules, -20, 20, -30, 30));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 17, (int)RuleCategory.Rules, -30, 30, -50, 50));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 17, (int)RuleCategory.Rules, -50, 50, -80, 80));
                    // Min Weeks Between Programmes
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 20, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 20, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 20, (int)RuleCategory.Rules, 0, 0, 50, -50));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 20, (int)RuleCategory.Rules, 50, -50, 100, -100));
                    // Minimum Break Availability
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 23, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 23, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 23, (int)RuleCategory.Rules, 50, 0, 100, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 23, (int)RuleCategory.Rules, 50, 0, 100, 0));
                    // Max Spots Per Prog/Time
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 24, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 24, (int)RuleCategory.Rules, 0, 0, -20, 20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 24, (int)RuleCategory.Rules, -20, 20, -40, 40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 24, (int)RuleCategory.Rules, -40, 40, -80, 80));
                    // Min Days Between Prog/Time
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 25, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 25, (int)RuleCategory.Rules, 0, 0, 20, -20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 25, (int)RuleCategory.Rules, 20, -20, 40, -40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 25, (int)RuleCategory.Rules, 40, -40, 80, -80));
                    // Min Weeks Between Prog/Time
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 26, (int)RuleCategory.Rules, 0, 0, 0, 0));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 26, (int)RuleCategory.Rules, 0, 0, 20, -20));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 26, (int)RuleCategory.Rules, 20, -20, 40, -40));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 26, (int)RuleCategory.Rules, 40, -40, 80, -80));

                    #endregion

                    // Slotting Limits default rule
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Low, 1, (int)RuleCategory.SlottingLimits));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Medium, 1, (int)RuleCategory.SlottingLimits));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.High, 1, (int)RuleCategory.SlottingLimits));
                    newAutopilotRules.Add(AutopilotRule.Create((int)AutopilotFlexibilityLevel.Extreme, 1, (int)RuleCategory.SlottingLimits));

                    foreach (var autopilotRule in newAutopilotRules)
                    {
                        session.Store(autopilotRule);
                    }

                    session.SaveChanges();
                }
            }
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
