using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10499_Enhance_existing_Campaign_pre_post_run_data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignFinalRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "CampaignStartRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "CampaignTargetRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "DaypartFinalRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "DaypartStartRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "DaypartTargetRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "DowPattern",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "SalesAreaGroupFinalRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "SalesAreaGroupStartRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightTargetRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightStartRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightFinalRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "SpotLengthTargetRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "SpotLengthStartRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "SpotLengthFinalRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "SalesAreaGroupTargetRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightStartDate",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightEndDate",
                table: "ScenarioCampaignResults");

            migrationBuilder.AddColumn<long>(
                name: "TargetRatings",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RSCancelledSpots",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RSCancelledRatings",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PreRunRatings",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OptimiserRatings",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ISRCancelledSpots",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ISRCancelledRatings",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StrikeWeightStartDate",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StrikeWeightEndDate",
                table: "ScenarioCampaignResults",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SalesAreaName",
                table: "ScenarioCampaignResults",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DaypartName",
                table: "ScenarioCampaignResults",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaypartName",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "TargetRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "RSCancelledSpots",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "RSCancelledRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "PreRunRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "OptimiserRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "ISRCancelledSpots",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "ISRCancelledRatings",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightStartDate",
                table: "ScenarioCampaignResults");

            migrationBuilder.DropColumn(
                name: "StrikeWeightEndDate",
                table: "ScenarioCampaignResults");

            migrationBuilder.AddColumn<double>(
                name: "StrikeWeightTargetRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StrikeWeightStartRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StrikeWeightFinalRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpotLengthTargetRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpotLengthStartRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpotLengthFinalRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SalesAreaGroupTargetRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CampaignFinalRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CampaignStartRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CampaignTargetRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DaypartFinalRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DaypartStartRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DaypartTargetRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "DowPattern",
                table: "ScenarioCampaignResults",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SalesAreaGroupFinalRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SalesAreaGroupStartRatings",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "StrikeWeightStartDate",
                table: "ScenarioCampaignResults",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "StrikeWeightEndDate",
                table: "ScenarioCampaignResults",
                nullable: true,
                defaultValue: 0);
        }
    }
}
