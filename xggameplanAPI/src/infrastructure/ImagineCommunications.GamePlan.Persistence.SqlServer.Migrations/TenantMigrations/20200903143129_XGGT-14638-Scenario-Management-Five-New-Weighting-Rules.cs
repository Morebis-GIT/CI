using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14638ScenarioManagementFiveNewWeightingRules : Migration
    {
        private const int WeightingsRuleTypeId = 2;

        private readonly List<(int ruleId, string name)> _newRules = new List<(int ruleId, string name)>
        {
            (10, "Strike Weight/Daypart"),
            (11, "Strike Weight/Length"),
            (12, "Strike Weight/Daypart/Length"),
            (13, "Campaign Price"),
            (14, "Programme")
        };

        private readonly string _requiredDataDetectionTemplate = @$"
            IF EXISTS(
                SELECT 1
                FROM RuleTypes
                WHERE Id = {WeightingsRuleTypeId})
            BEGIN
                {{0}}
            END
        ";

        private readonly string _ruleInsertTemplate = @$"
            IF NOT EXISTS (
                SELECT 1
                FROM Rules
                WHERE Type = 'weightings'
                AND Description = '{{0}}')
            BEGIN
                INSERT INTO Rules ([RuleId],[RuleTypeId],[InternalType],[Description],[Value],[Type])
                VALUES ({{1}}, {WeightingsRuleTypeId}, 'Weightings', '{{0}}', '0', 'weightings')
            END
        ";

        private const string PassEnumerationTemplate = @"
            DECLARE @PassId INT;

            DECLARE PASSES_CURSOR CURSOR
                LOCAL STATIC READ_ONLY FORWARD_ONLY
            FOR
            SELECT Id FROM Passes
            WHERE IsLibraried = 1
            UNION
            SELECT SPR.PassId
            FROM Scenarios S
            INNER JOIN ScenarioPassReferences SPR
                ON SPR.ScenarioId = S.Id
            WHERE S.IsLibraried = 1

            OPEN PASSES_CURSOR
            FETCH NEXT FROM PASSES_CURSOR INTO @PassId
            WHILE @@FETCH_STATUS = 0
            BEGIN
                {0}
                FETCH NEXT FROM PASSES_CURSOR INTO @PassId
            END
            CLOSE PASSES_CURSOR
            DEALLOCATE PASSES_CURSOR
        ";

        private const string PassRuleInsertTemplate = @"
            IF NOT EXISTS(
                SELECT 1
                FROM PassRules
                WHERE PassId = @PassId
                AND Type = 'weightings'
                AND Description = '{0}')
            BEGIN
                INSERT INTO PassRules ([PassId],[RuleId],[InternalType],[Description],[Value],[Type],[Discriminator])
                VALUES(@PassId, {1}, 'Weightings', '{0}', '0', 'weightings', 4)
            END
        ";

        private const string GenericRulesDeletionTemplate = @"
            DELETE FROM {0}
            WHERE Type = 'weightings'
            AND Description IN ({1})
        ";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            InsertRules(migrationBuilder);
            InsertPassRules(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var ruleNameList = string.Join(", ", _newRules.Select(rule => $"'{rule.name}'"));

            var deleteRulesQuery = string.Format(GenericRulesDeletionTemplate, "Rules", ruleNameList);
            migrationBuilder.Sql(deleteRulesQuery);

            var deletePassRulesQuery = string.Format(GenericRulesDeletionTemplate, "PassRules", ruleNameList);
            migrationBuilder.Sql(deletePassRulesQuery);
        }

        private void InsertRules(MigrationBuilder migrationBuilder)
        {
            var sb = new StringBuilder();

            foreach (var (ruleId, ruleName) in _newRules)
            {
                sb.AppendFormat(_ruleInsertTemplate, ruleName, ruleId);
            }

            var query = string.Format(_requiredDataDetectionTemplate, sb.ToString());

            migrationBuilder.Sql(query);
        }

        private void InsertPassRules(MigrationBuilder migrationBuilder)
        {
            var sb = new StringBuilder();

            foreach (var (ruleId, ruleName) in _newRules)
            {
                sb.AppendFormat(PassRuleInsertTemplate, ruleName, ruleId);
            }

            var passEnumerationQuery = string.Format(PassEnumerationTemplate, sb.ToString());
            var query = string.Format(_requiredDataDetectionTemplate, passEnumerationQuery);

            migrationBuilder.Sql(query);
        }
    }
}
