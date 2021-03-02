using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16136_BRS_And_ComparisonConfigs_KPIs_Correction : Migration
    {
        public const string AvailableRatingsByDemo = "availableRatings";

        private const int ExcludedPriorityId = 1;
        private readonly string[] _kpisToRemove = {
            "reservedRatingsADS",
            "ratingCampaignsRatedSpots",
            "spotCampaignsRatedSpots",
            "totalSpotsBooked"
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BRSConfigurationForKPIs",
                keyColumn: "KPIName",
                keyValues: _kpisToRemove);

            migrationBuilder.Sql(@"
                UPDATE BRSConfigurationTemplates
	            SET LastModified = GETUTCDATE()");

            migrationBuilder.Sql($@"
                UPDATE KPIComparisonConfigs
                SET HigherIsBest = 1
                WHERE KPIName LIKE '{AvailableRatingsByDemo}%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var insertionData = new List<string>(_kpisToRemove.Length);
            foreach (var kpiName in _kpisToRemove)
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

            migrationBuilder.Sql($@"
                UPDATE KPIComparisonConfigs
                SET HigherIsBest = 0
                WHERE KPIName LIKE '{AvailableRatingsByDemo}%'");
        }
    }
}
