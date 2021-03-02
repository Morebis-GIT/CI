using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16602_SpotBasedAutopilotRules : Migration
    {
        private const string TempRulesTableQuery = @"
                CREATE TABLE #TempRulesMapping
                (
                    RuleTypeId int,
                    OldRuleId int,
                    NewRuleId int,
                    NewCampaignType int
                );

                INSERT INTO #TempRulesMapping
                    (RuleTypeId, OldRuleId, NewRuleId, NewCampaignType)
                VALUES
                    (3, 1, 19, 1),
                    (3, 2, 20, 1),
                    (3, 3, 21, 1),
                    (3, 4, 22, 1),
                    (3, 5, 23, 1),
                    (3, 6, 24, 1),
                    (3, 7, 25, 1),
                    (3, 8, 26, 1),
                    (3, 9, 27, 1),
                    (3, 10, 28, 1),
                    (3, 11, 29, 1),
                    (3, 12, 30, 1),
                    (3, 13, 31, 1),
                    (3, 14, 32, 1),
                    (3, 37, 38, 1),
                    (4, 1, 28, 0),
                    (4, 2, 29, 0),
                    (4, 3, 30, 0),
                    (4, 4, 31, 0),
                    (4, 5, 32, 0),
                    (4, 6, 33, 0),
                    (4, 7, 34, 0),
                    (4, 17, 44, 0),
                    (4, 20, 47, 0),
                    (4, 23, 50, 0),
                    (4, 24, 51, 0),
                    (4, 25, 52, 0),
                    (4, 26, 53, 0);
        ";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($@"
                {TempRulesTableQuery}

                -- Set default CampaignType for existing rules
                UPDATE dbo.[Rules]
                SET CampaignType = CASE
                                       WHEN RuleTypeId = 3 THEN 0
                                       WHEN RuleTypeId = 4 THEN 1
                                       ELSE NULL
                                   END;

                -- Generate/Clone missing spot based Rules
                INSERT INTO dbo.[Rules] ([RuleId], [RuleTypeId], [InternalType], [Description], [Type], [CampaignType])
                SELECT
                    map.[NewRuleId], r.[RuleTypeId], r.[InternalType], r.[Description], r.[Type], map.[NewCampaignType]
                FROM
                    dbo.[Rules] r
                INNER JOIN
                    #TempRulesMapping map ON (map.[RuleTypeId] = r.[RuleTypeId] AND map.[OldRuleId] = r.[RuleId])
                WHERE
                    r.[RuleTypeId] IN (3, 4);

                -- Generate/Clone missing spot based AutopilotRules
                INSERT INTO dbo.[AutopilotRules] ([RuleId], [RuleTypeId], [FlexibilityLevelId], [Enabled], [LoosenBit], [LoosenLot], [TightenBit], [TightenLot])
                SELECT
                    map.[NewRuleId], ar.[RuleTypeId], ar.[FlexibilityLevelId], ar.[Enabled], ar.[LoosenBit], ar.[LoosenLot], ar.[TightenBit], ar.[TightenLot]
                FROM
                    dbo.[AutopilotRules] ar
                INNER JOIN
                    #TempRulesMapping map ON (map.[RuleTypeId] = ar.[RuleTypeId] AND map.[OldRuleId] = ar.[RuleId])
                WHERE
                    ar.[RuleTypeId] IN (3, 4);

                DROP TABLE #TempRulesMapping;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($@"
                {TempRulesTableQuery}

                UPDATE dbo.[Rules] SET CampaignType = NULL;

                DELETE r FROM dbo.[Rules] r INNER JOIN #TempRulesMapping map ON (r.[RuleTypeId] = map.[RuleTypeId] AND r.[RuleId] = map.[NewRuleId]);

                DELETE ar FROM dbo.[AutopilotRules] ar INNER JOIN #TempRulesMapping map ON (ar.[RuleTypeId] = map.[RuleTypeId] AND ar.[RuleId] = map.[NewRuleId]);

                DROP TABLE #TempRulesMapping;
            ");
        }
    }
}
