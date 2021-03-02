using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT12670_Adding_Sky_Specific_Feature_Flags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures])
                BEGIN
                    INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                        SELECT [Id], 'NineValidationMinSpot', 1, 0 FROM [dbo].[Tenants];
                    INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                        SELECT [Id], 'NineValidationRatingPredictions', 1, 0 FROM [dbo].[Tenants];
                    INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                        SELECT [Id], 'IncludeChannelGroupFileForOptimiser', 1, 0 FROM [dbo].[Tenants]
                END"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'NineValidationMinSpot'");
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'NineValidationRatingPredictions'");
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'IncludeChannelGroupFileForOptimiser'");
        }
    }
}
