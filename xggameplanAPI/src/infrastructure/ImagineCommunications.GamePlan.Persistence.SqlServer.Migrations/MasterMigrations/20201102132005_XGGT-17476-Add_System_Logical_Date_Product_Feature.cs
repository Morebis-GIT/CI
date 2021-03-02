using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT17476Add_System_Logical_Date_Product_Feature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                    IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{ProductFeature.UseSystemLogicalDate}')
                        INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                        SELECT [Id], '{ProductFeature.UseSystemLogicalDate}', 0, 1 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{ProductFeature.UseSystemLogicalDate}'");
        }
    }
}
