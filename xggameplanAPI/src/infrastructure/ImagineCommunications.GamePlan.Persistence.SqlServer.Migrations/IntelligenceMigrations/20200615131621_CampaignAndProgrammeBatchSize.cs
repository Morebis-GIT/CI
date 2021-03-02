using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class CampaignAndProgrammeBatchSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update MessageTypes set BatchSize = 50 where id = 'IBulkCampaignCreatedOrUpdated';" +
                                 "update MessageTypes set BatchSize = 200 where id = 'IBulkProgrammeCreated';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update MessageTypes set BatchSize = NULL where id = 'IBulkCampaignCreatedOrUpdated';" +
                                 "update MessageTypes set BatchSize = NULL where id = 'IBulkProgrammeCreated';");
        }
    }
}
