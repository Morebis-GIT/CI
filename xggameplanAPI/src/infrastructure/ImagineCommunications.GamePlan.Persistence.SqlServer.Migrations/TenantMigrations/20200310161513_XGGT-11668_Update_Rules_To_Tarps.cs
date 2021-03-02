using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11668_Update_Rules_To_Tarps : Migration
    {
        private const int MaxRatingsForSpotCampaignsRuleId = 21;
        private const int MaxRatingsForRatingCampaignsRuleId = 27;
        private const int True = 1;
        private const int False = 0;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                declare @StartedRuns_PassRules Table
                (
	                RunId uniqueidentifier,
	                PassRuleId int
                )

                insert into @StartedRuns_PassRules
                select Runs.Id, PassRules.Id
                from Runs
                join RunScenarios 
                    ON RunScenarios.RunId = Runs.Id 
                    AND Runs.RunStatus != {(int)RunStatus.NotStarted}
                join ScenarioPassReferences 
                    ON ScenarioPassReferences.ScenarioId = RunScenarios.ScenarioId
                join PassRules 
                    ON PassRules.PassId = ScenarioPassReferences.PassId

                update PassRules
                set Ignore = '{True}'
                where Id NOT IN (select PassRuleId from @StartedRuns_PassRules)
	                AND RuleId IN ({MaxRatingsForRatingCampaignsRuleId}, {MaxRatingsForSpotCampaignsRuleId})
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                declare @StartedRuns_PassRules Table
                (
	                RunId uniqueidentifier,
	                PassRuleId int
                )

                insert into @StartedRuns_PassRules
                select Runs.Id, PassRules.Id
                from Runs
                join RunScenarios 
                    ON RunScenarios.RunId = Runs.Id 
                    AND Runs.RunStatus != {(int)RunStatus.NotStarted}
                join ScenarioPassReferences 
                    ON ScenarioPassReferences.ScenarioId = RunScenarios.ScenarioId
                join PassRules 
                    ON PassRules.PassId = ScenarioPassReferences.PassId

                update PassRules
                set Ignore = '{False}'
                where Id NOT IN (select PassRuleId from @StartedRuns_PassRules)
	                AND RuleId IN ({MaxRatingsForRatingCampaignsRuleId}, {MaxRatingsForSpotCampaignsRuleId})
            ");
        }
    }
}
