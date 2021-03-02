using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18146_ProgrammeAndBreak_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_Schedules_SalesArea",
                table: "Schedules");

            _ = migrationBuilder.DropIndex(
                name: "IX_Schedules_Date_SalesArea",
                table: "Schedules");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Schedules",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE Schedules
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM Schedules s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "Schedules",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Schedules");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Schedules_SalesAreaId",
                table: "Schedules",
                column: "SalesAreaId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Schedules_Date_SalesAreaId",
                table: "Schedules",
                columns: new[] { "Date", "SalesAreaId" },
                unique: true);

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_SalesArea",
                table: "ScheduleBreaks");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "ScheduleBreaks",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE ScheduleBreaks
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM ScheduleBreaks sb JOIN (
                  SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON sb.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "ScheduleBreaks",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ScheduleBreaks");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_SalesAreaId",
                table: "ScheduleBreaks",
                column: "SalesAreaId");

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks");

            _ = migrationBuilder.DropIndex(
                name: "IX_Breaks_SalesArea",
                table: "Breaks");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Breaks",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE Breaks
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM Breaks b JOIN (
                  SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON b.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "Breaks",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Breaks");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Breaks_SalesAreaId",
                table: "Breaks",
                column: "SalesAreaId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesAreaId", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });

            _ = migrationBuilder.DropIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes");

            _ = migrationBuilder.DropIndex(
                name: "IX_Programmes_SalesArea",
                table: "Programmes");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Programmes",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE Programmes
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM Programmes p JOIN (
                  SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON p.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "Programmes",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Programmes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes",
                column: "ProgrammeDictionaryId")
                .Annotation("SqlServer:Include", new[] { "LiveBroadcast" });

            _ = migrationBuilder.CreateIndex(
                name: "IX_Programmes_SalesAreaId",
                table: "Programmes",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_Schedules_SalesAreaId",
                table: "Schedules");

            _ = migrationBuilder.DropIndex(
                name: "IX_Schedules_Date_SalesAreaId",
                table: "Schedules");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Schedules ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE Schedules
                  SET SalesArea = t.[Name]
                  FROM Schedules s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Schedules ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Schedules");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Schedules_SalesArea",
                table: "Schedules",
                column: "SalesArea");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Schedules_Date_SalesArea",
                table: "Schedules",
                columns: new[] { "Date", "SalesArea" },
                unique: true);

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_SalesAreaId",
                table: "ScheduleBreaks");

            _ = migrationBuilder.Sql(
                "ALTER TABLE ScheduleBreaks ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE ScheduleBreaks
                  SET SalesArea = t.[Name]
                  FROM ScheduleBreaks sb JOIN (
                  SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON sb.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "ScheduleBreaks");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_SalesArea",
                table: "ScheduleBreaks",
                column: "SalesArea");

            _ = migrationBuilder.DropIndex(
                name: "IX_Breaks_SalesAreaId",
                table: "Breaks");

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Breaks ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE Breaks
                  SET SalesArea = t.[Name]
                  FROM Breaks b JOIN (
                  SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON b.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Breaks");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Breaks_SalesArea",
                table: "Breaks",
                column: "SalesArea");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesArea", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });

            _ = migrationBuilder.DropIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes");

            _ = migrationBuilder.DropIndex(
                name: "IX_Programmes_SalesAreaId",
                table: "Programmes");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Programmes ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE Programmes
                  SET SalesArea = t.[Name]
                  FROM Programmes p JOIN (
                  SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON p.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Programmes ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Programmes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes",
                column: "ProgrammeDictionaryId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Programmes_SalesArea",
                table: "Programmes",
                column: "SalesArea");
        }
    }
}
