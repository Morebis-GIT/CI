using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13239_Change_Multipart_length : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingPosition",
                table: "CampaignSalesAreaTargetMultipartLengths",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sequencing",
                table: "CampaignSalesAreaTargetMultipartLengths",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                DECLARE @MultipartLengthId_Sequencing TABLE
                (
                    MultipartLengthId int,
                    Sequencing int
                )

                INSERT INTO @MultipartLengthId_Sequencing
                SELECT
                    Id,
                    ROW_NUMBER() OVER(PARTITION BY [CampaignSalesAreaTargetMultipartId] ORDER BY Id ASC) 
                FROM [CampaignSalesAreaTargetMultipartLengths]
                ORDER BY [CampaignSalesAreaTargetMultipartId]

                UPDATE [CampaignSalesAreaTargetMultipartLengths]
                SET Sequencing = (
                    SELECT Sequencing FROM @MultipartLengthId_Sequencing ms
                    WHERE ms.MultipartLengthId = Id
                )
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingPosition",
                table: "CampaignSalesAreaTargetMultipartLengths");

            migrationBuilder.DropColumn(
                name: "Sequencing",
                table: "CampaignSalesAreaTargetMultipartLengths");
        }
    }
}
