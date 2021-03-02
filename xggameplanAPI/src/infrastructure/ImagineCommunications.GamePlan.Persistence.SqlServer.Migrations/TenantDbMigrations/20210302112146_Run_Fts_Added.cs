using Microsoft.EntityFrameworkCore.Migrations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantDbMigrations
{
    public partial class Run_Fts_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddFulltextIndex<Run>(TargetModel, Run.SearchField);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
