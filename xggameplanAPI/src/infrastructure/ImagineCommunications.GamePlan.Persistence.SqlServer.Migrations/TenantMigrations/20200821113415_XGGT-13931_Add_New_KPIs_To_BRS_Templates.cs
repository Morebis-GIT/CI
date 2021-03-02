using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13931_Add_New_KPIs_To_BRS_Templates : Migration
    {
        private const int ExcludedPriorityId = 1;
        private readonly string[] _newKpiNames = {
            "conversionEfficiencyADS",
            "conversionEfficiencyMN1634",
            "conversionEfficiencyCHD",
            "conversionEfficiencyHWCH",
            "conversionEfficiencyADABC1",
            "availableRatingsADS",
            "availableRatingsMN1634",
            "availableRatingsCHD",
            "availableRatingsHWCH",
            "availableRatingsADABC1",
            "differenceValue",
            "differenceValuePercentage",
            "differenceValueWithPayback",
            "differenceValuePercentagePayback",
            "ratingCampaignsRatedSpots",
            "reservedRatingsADS",
            "spotCampaignsRatedSpots",
            "totalNominalValue"
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var insertionData = new List<string>(_newKpiNames.Length);
            foreach (var kpiName in _newKpiNames)
            {
                insertionData.Add($"(@template_id, '{kpiName}', {ExcludedPriorityId})");
            }

            migrationBuilder.Sql($@"
                DECLARE @template_id INT

                DECLARE templates_cursor CURSOR FOR
                SELECT Id FROM BRSConfigurationTemplates

                OPEN templates_cursor
                FETCH NEXT FROM templates_cursor INTO @template_id

                WHILE @@FETCH_STATUS = 0
                BEGIN
	                INSERT INTO BRSConfigurationForKPIs VALUES
	                {string.Join(",\n", insertionData)}

	                UPDATE BRSConfigurationTemplates
	                SET LastModified = GETUTCDATE()
	                WHERE Id = @template_id

                    FETCH NEXT FROM templates_cursor INTO @template_id
                END

                CLOSE templates_cursor
                DEALLOCATE templates_cursor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BRSConfigurationForKPIs",
                keyColumn: "KPIName",
                keyValues: _newKpiNames);

            migrationBuilder.Sql(@"
                UPDATE BRSConfigurationTemplates
	            SET LastModified = GETUTCDATE()");
        }
    }
}
