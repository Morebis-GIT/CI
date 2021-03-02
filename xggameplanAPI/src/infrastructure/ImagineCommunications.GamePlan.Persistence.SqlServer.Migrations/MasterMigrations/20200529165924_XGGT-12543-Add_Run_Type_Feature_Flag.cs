using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT12543Add_Run_Type_Feature_Flag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                        IF EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures])
                        INSERT INTO [dbo].[TenantProductFeatures] 
                        ([TenantId], [Name], [Enabled], [IsShared])
	                        SELECT [Id], 'RunType', 1, 1 
                            FROM [dbo].[Tenants] 
	                        WHERE [ID] NOT IN 
                                    (SELECT [TenantId] 
                                     FROM [TenantProductFeatures] 
                                     WHERE [Name] = 'RunType')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'RunType'");
        }
    }
}
