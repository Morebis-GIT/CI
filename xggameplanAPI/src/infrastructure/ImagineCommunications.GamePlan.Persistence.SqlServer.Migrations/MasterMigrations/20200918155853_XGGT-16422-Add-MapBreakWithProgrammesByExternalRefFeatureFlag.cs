﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT16422AddMapBreakWithProgrammesByExternalRefFeatureFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures])\n" +
                "INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])\n" +
                "SELECT [Id], 'MapBreakWithProgrammesByExternalRef', 1, 0 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'MapBreakWithProgrammesByExternalRef'");
        }
    }
}
