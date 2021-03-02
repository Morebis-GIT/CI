using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT15328_Add_SAPPTargetAreaName_FeatureFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@$"
                    IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{ProductFeature.SAPPTargetAreaName}')
                        INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                        SELECT [Id], '{ProductFeature.SAPPTargetAreaName}', 0, 1 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{ProductFeature.SAPPTargetAreaName}'");
        }
    }
}
