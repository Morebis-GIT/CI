using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT17536_Add_ScenarioCampaignResults_FeatureFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@$"
                IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{nameof(ProductFeature.ScenarioCampaignResultsProcessing)}')
                INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                SELECT [Id], '{nameof(ProductFeature.ScenarioCampaignResultsProcessing)}', 1, 0 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{nameof(ProductFeature.ScenarioCampaignResultsProcessing)}'");
        }
    }
}
