using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantDbMigrations
{
    public partial class XGGT17303rebaseaurorafromdevelopbranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_SalesArea",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_Date_SalesArea",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_SalesArea",
                table: "ScheduleBreaks");

            migrationBuilder.DropIndex(
                name: "IX_Programmes_SalesArea",
                table: "Programmes");

            migrationBuilder.DropIndex(
                name: "IX_ISRSettings_SalesArea",
                table: "ISRSettings");

            migrationBuilder.DropIndex(
                name: "IX_IndexTypes_SalesArea",
                table: "IndexTypes");

            migrationBuilder.DropIndex(
                name: "IX_ClashDifferences_SalesArea",
                table: "ClashDifferences");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_SalesArea",
                table: "Breaks");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "SalesAreaPriorities");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "LengthFactors");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ISRSettings");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "InventoryLocks");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "IndexTypes");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ClashDifferences");

            migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Breaks");

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Schedules",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PassesEncounteringFailure",
                table: "ScenarioCampaignFailures",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "SalesAreaPriorities",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Programmes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "LengthFactors",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "ISRSettings",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "InventoryCode",
                table: "InventoryLocks",
                type: "NCHAR(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "CHAR(10)");

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "InventoryLocks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "IndexTypes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "ClashDifferences",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Breaks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "AutoDistributed",
                table: "AutoBookSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SalesAreaId",
                table: "Schedules",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Date_SalesAreaId",
                table: "Schedules",
                columns: new[] { "Date", "SalesAreaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_SalesAreaId",
                table: "ScheduleBreaks",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreaPriorities_SalesAreaId",
                table: "SalesAreaPriorities",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_SalesAreaId",
                table: "Programmes",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_LengthFactors_SalesAreaId",
                table: "LengthFactors",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ISRSettings_SalesAreaId",
                table: "ISRSettings",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocks_SalesAreaId",
                table: "InventoryLocks",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_SalesAreaId",
                table: "IndexTypes",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClashDifferences_SalesAreaId",
                table: "ClashDifferences",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_SalesAreaId",
                table: "Breaks",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesAreaId", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });

            migrationBuilder.AddForeignKey(
                name: "FK_Breaks_SalesAreas_SalesAreaId",
                table: "Breaks",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClashDifferences_SalesAreas_SalesAreaId",
                table: "ClashDifferences",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IndexTypes_SalesAreas_SalesAreaId",
                table: "IndexTypes",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLocks_SalesAreas_SalesAreaId",
                table: "InventoryLocks",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ISRSettings_SalesAreas_SalesAreaId",
                table: "ISRSettings",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LengthFactors_SalesAreas_SalesAreaId",
                table: "LengthFactors",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programmes_SalesAreas_SalesAreaId",
                table: "Programmes",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesAreaPriorities_SalesAreas_SalesAreaId",
                table: "SalesAreaPriorities",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleBreaks_SalesAreas_SalesAreaId",
                table: "ScheduleBreaks",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_SalesAreas_SalesAreaId",
                table: "Schedules",
                column: "SalesAreaId",
                principalTable: "SalesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breaks_SalesAreas_SalesAreaId",
                table: "Breaks");

            migrationBuilder.DropForeignKey(
                name: "FK_ClashDifferences_SalesAreas_SalesAreaId",
                table: "ClashDifferences");

            migrationBuilder.DropForeignKey(
                name: "FK_IndexTypes_SalesAreas_SalesAreaId",
                table: "IndexTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocks_SalesAreas_SalesAreaId",
                table: "InventoryLocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ISRSettings_SalesAreas_SalesAreaId",
                table: "ISRSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_LengthFactors_SalesAreas_SalesAreaId",
                table: "LengthFactors");

            migrationBuilder.DropForeignKey(
                name: "FK_Programmes_SalesAreas_SalesAreaId",
                table: "Programmes");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesAreaPriorities_SalesAreas_SalesAreaId",
                table: "SalesAreaPriorities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleBreaks_SalesAreas_SalesAreaId",
                table: "ScheduleBreaks");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_SalesAreas_SalesAreaId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_SalesAreaId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_Date_SalesAreaId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_SalesAreaId",
                table: "ScheduleBreaks");

            migrationBuilder.DropIndex(
                name: "IX_SalesAreaPriorities_SalesAreaId",
                table: "SalesAreaPriorities");

            migrationBuilder.DropIndex(
                name: "IX_Programmes_SalesAreaId",
                table: "Programmes");

            migrationBuilder.DropIndex(
                name: "IX_LengthFactors_SalesAreaId",
                table: "LengthFactors");

            migrationBuilder.DropIndex(
                name: "IX_ISRSettings_SalesAreaId",
                table: "ISRSettings");

            migrationBuilder.DropIndex(
                name: "IX_InventoryLocks_SalesAreaId",
                table: "InventoryLocks");

            migrationBuilder.DropIndex(
                name: "IX_IndexTypes_SalesAreaId",
                table: "IndexTypes");

            migrationBuilder.DropIndex(
                name: "IX_ClashDifferences_SalesAreaId",
                table: "ClashDifferences");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_SalesAreaId",
                table: "Breaks");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "PassesEncounteringFailure",
                table: "ScenarioCampaignFailures");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "SalesAreaPriorities");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "LengthFactors");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "ISRSettings");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "InventoryLocks");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "IndexTypes");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "ClashDifferences");

            migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "AutoDistributed",
                table: "AutoBookSettings");

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "Schedules",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "ScheduleBreaks",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "SalesAreaPriorities",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "Programmes",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "LengthFactors",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "ISRSettings",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InventoryCode",
                table: "InventoryLocks",
                type: "CHAR(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCHAR(10)");

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "InventoryLocks",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "IndexTypes",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "ClashDifferences",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesArea",
                table: "Breaks",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SalesArea",
                table: "Schedules",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Date_SalesArea",
                table: "Schedules",
                columns: new[] { "Date", "SalesArea" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_SalesArea",
                table: "ScheduleBreaks",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_SalesArea",
                table: "Programmes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ISRSettings_SalesArea",
                table: "ISRSettings",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_SalesArea",
                table: "IndexTypes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ClashDifferences_SalesArea",
                table: "ClashDifferences",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_SalesArea",
                table: "Breaks",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesArea", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });
        }
    }
}
