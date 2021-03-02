using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12505Add_New_Campaign_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Payback",
                table: "CampaignTargetStrikeWeights",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AutomatedBooked",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RevenueBooked",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Spots",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetXP",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopTail",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CampaignPaybacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPaybacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignPaybacks_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPaybacks_CampaignId",
                table: "CampaignPaybacks",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignPaybacks");

            migrationBuilder.DropColumn(
                name: "Payback",
                table: "CampaignTargetStrikeWeights");

            migrationBuilder.DropColumn(
                name: "AutomatedBooked",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "RevenueBooked",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Spots",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "TargetXP",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "TopTail",
                table: "Campaigns");
        }
    }
}
