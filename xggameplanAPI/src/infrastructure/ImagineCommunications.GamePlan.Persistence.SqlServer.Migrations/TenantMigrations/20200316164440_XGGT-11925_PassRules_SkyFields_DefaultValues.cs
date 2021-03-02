using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11925_PassRules_SkyFields_DefaultValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [dbo].[PassRules] SET BookTargetArea = 0 WHERE BookTargetArea IS NULL AND Discriminator = {(int)PassRuleType.Tolerance};");

            migrationBuilder.Sql($"UPDATE [dbo].[PassRules] SET CampaignType = {(int)CampaignDeliveryType.Rating} WHERE CampaignType IS NULL AND Discriminator IN ({(int)PassRuleType.Rule}, {(int)PassRuleType.Tolerance});");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [dbo].[PassRules] SET BookTargetArea = NULL WHERE BookTargetArea = 0 AND Discriminator = {(int)PassRuleType.Tolerance};");

            migrationBuilder.Sql($"UPDATE [dbo].[PassRules] SET CampaignType = NULL WHERE CampaignType = {(int)CampaignDeliveryType.Rating} AND Discriminator IN ({(int)PassRuleType.Rule}, {(int)PassRuleType.Tolerance});");
        }
    }
}
