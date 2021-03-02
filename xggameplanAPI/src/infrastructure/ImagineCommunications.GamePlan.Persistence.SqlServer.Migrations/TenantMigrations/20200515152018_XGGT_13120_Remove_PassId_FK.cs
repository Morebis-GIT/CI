using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT_13120_Remove_PassId_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[ScenarioPassReferences]
                DROP CONSTRAINT IF EXISTS [FK_ScenarioPassReferences_Passes_PassId]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[ScenarioPassReferences]
                ADD CONSTRAINT [FK_ScenarioPassReferences_Passes_PassId]
                FOREIGN KEY (PassId) REFERENCES Passes(Id) ON DELETE CASCADE");
        }
    }
}
