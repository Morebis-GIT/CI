using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12646_Max_Ratings_rules_renaming : Migration
    {
        private const int MaxRatingsForSpotCampaignsRuleId = 21;
        private const int MaxRatingsForRatingCampaignsRuleId = 27;
        private const string MaxRatingsForSpotCampaignsDescriptionNew = "Max Rating Points for Spot Campaigns";
        private const string MaxRatingsForRatingCampaignsDescriptionNew = "Max Rating Points for Rating Campaigns";
        private const string MaxRatingsForSpotCampaignsDescriptionOld = "Max Ratings for Spot Campaigns";
        private const string MaxRatingsForRatingCampaignsDescriptionOld = "Max Ratings for Rating Campaigns";

        private readonly string StartedRunsPassRulesQuery = $@"
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
                where RuleId in ({MaxRatingsForSpotCampaignsRuleId}, {MaxRatingsForRatingCampaignsRuleId})";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(StartedRunsPassRulesQuery + $@"

                UPDATE Rules
                SET Description = '{MaxRatingsForSpotCampaignsDescriptionNew}'
                WHERE RuleId = {MaxRatingsForSpotCampaignsRuleId}

                UPDATE Rules
                SET Description = '{MaxRatingsForRatingCampaignsDescriptionNew}'
                WHERE RuleId = {MaxRatingsForRatingCampaignsRuleId}

                UPDATE PassRules
                SET Description = '{MaxRatingsForSpotCampaignsDescriptionNew}'
                WHERE Id NOT IN (SELECT PassRuleId FROM @StartedRuns_PassRules)
                    AND RuleId = {MaxRatingsForSpotCampaignsRuleId}

                UPDATE PassRules
                SET Description = '{MaxRatingsForRatingCampaignsDescriptionNew}'
                WHERE Id NOT IN (SELECT PassRuleId FROM @StartedRuns_PassRules)
                    AND RuleId = {MaxRatingsForRatingCampaignsRuleId}");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(StartedRunsPassRulesQuery + $@"
                UPDATE Rules
                SET Description = '{MaxRatingsForSpotCampaignsDescriptionOld}'
                WHERE RuleId = {MaxRatingsForSpotCampaignsRuleId}

                UPDATE Rules
                SET Description = '{MaxRatingsForRatingCampaignsDescriptionOld}'
                WHERE RuleId = {MaxRatingsForRatingCampaignsRuleId}

                UPDATE PassRules
                SET Description = '{MaxRatingsForSpotCampaignsDescriptionOld}'
                WHERE Id NOT IN (SELECT PassRuleId FROM @StartedRuns_PassRules)
                    AND RuleId = {MaxRatingsForSpotCampaignsRuleId}

                UPDATE PassRules
                SET Description = '{MaxRatingsForRatingCampaignsDescriptionOld}'
                WHERE Id NOT IN (SELECT PassRuleId FROM @StartedRuns_PassRules)
                    AND RuleId = {MaxRatingsForRatingCampaignsRuleId}");
        }
    }
}
