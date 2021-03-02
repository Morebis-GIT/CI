using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13931_Add_KPIComparisonConfigs_For_New_Kpis : Migration
    {
        private const float DefaultDiscernibleDifference = 1.0f;

        private readonly KPIComparisonConfig[] _kpiComparisonConfigs = new[]
        {
            new KPIComparisonConfig
            {
                KPIName = "conversionEfficiencyADS",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "conversionEfficiencyMN1634",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "conversionEfficiencyCHD",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "conversionEfficiencyHWCH",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "conversionEfficiencyADABC1",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "availableRatingsADS",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = false,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "availableRatingsMN1634",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = false,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "availableRatingsCHD",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = false,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "availableRatingsHWCH",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = false,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "availableRatingsADABC1",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = false,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "reservedRatingsADS",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = false,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "ratingCampaignsRatedSpots",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = false
            },
            new KPIComparisonConfig
            {
                KPIName = "spotCampaignsRatedSpots",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = false
            },
            new KPIComparisonConfig
            {
                KPIName = "baseDemographRatings",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "totalRatingCampaignSpots",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = false
            },
            new KPIComparisonConfig
            {
                KPIName = "totalSpotCampaignSpots",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = false
            },
            new KPIComparisonConfig
            {
                KPIName = "totalNominalValue",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "differenceValue",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "differenceValueWithPayback",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "differenceValuePercentage",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            },
            new KPIComparisonConfig
            {
                KPIName = "differenceValuePercentagePayback",
                DiscernibleDifference = DefaultDiscernibleDifference,
                HigherIsBest = true,
                Ranked = true
            }
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var insertionData = new List<string>(_kpiComparisonConfigs.Length);
            foreach (var config in _kpiComparisonConfigs)
            {
                insertionData.Add(string.Format("('{0}', {1}, {2}, {3})",
                    config.KPIName,
                    config.DiscernibleDifference,
                    config.HigherIsBest ? 1 : 0,
                    config.Ranked ? 1 : 0));
            }

            migrationBuilder.Sql($@"
                INSERT INTO KPIComparisonConfigs VALUES
                {string.Join(",\n", insertionData)}");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KPIComparisonConfigs",
                keyColumn: "KPIName",
                keyValues: _kpiComparisonConfigs.Select(x => x.KPIName).ToArray());
        }
    }
}
