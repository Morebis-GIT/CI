using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18289CampaignSalesAreas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PredictionSchedules

            _ = migrationBuilder.DropIndex(
                name: "IX_PredictionSchedules_SalesArea",
                table: "PredictionSchedules");

            _ = migrationBuilder.DropIndex(
                name: "IX_PredictionSchedules_SalesArea_ScheduleDay",
                table: "PredictionSchedules");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "PredictionSchedules",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE PredictionSchedules
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM PredictionSchedules p JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON p.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "PredictionSchedules",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "PredictionSchedules");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesAreaId",
                table: "PredictionSchedules",
                column: "SalesAreaId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesAreaId_ScheduleDay",
                table: "PredictionSchedules",
                columns: new[] { "SalesAreaId", "ScheduleDay" });

            // CampaignTimeRestrictionsSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_Name",
                table: "CampaignTimeRestrictionsSalesAreas");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignTimeRestrictionsSalesAreas",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignTimeRestrictionsSalesAreas
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM CampaignTimeRestrictionsSalesAreas ct JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON ct.[Name] = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignTimeRestrictionsSalesAreas",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "Name",
                table: "CampaignTimeRestrictionsSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_SalesAreaId",
                table: "CampaignTimeRestrictionsSalesAreas",
                column: "SalesAreaId");

            // CampaignSalesAreaTargets

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignSalesAreaTargets_SalesArea",
                table: "CampaignSalesAreaTargets");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignSalesAreaTargets",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignSalesAreaTargets
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM CampaignSalesAreaTargets cs JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cs.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignSalesAreaTargets",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "CampaignSalesAreaTargets");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargets_SalesAreaId",
                table: "CampaignSalesAreaTargets",
                column: "SalesAreaId");

            // CampaignSalesAreaTargetGroupSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_Name",
                table: "CampaignSalesAreaTargetGroupSalesAreas");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignSalesAreaTargetGroupSalesAreas
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM CampaignSalesAreaTargetGroupSalesAreas cs JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cs.[Name] = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "Name",
                table: "CampaignSalesAreaTargetGroupSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_SalesAreaId",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                column: "SalesAreaId");

            // CampaignProgrammeRestrictionsSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_Name",
                table: "CampaignProgrammeRestrictionsSalesAreas");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignProgrammeRestrictionsSalesAreas
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM CampaignProgrammeRestrictionsSalesAreas cp JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cp.[Name] = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "Name",
                table: "CampaignProgrammeRestrictionsSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_SalesAreaId",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                column: "SalesAreaId");

            // CampaignBreakRequirements

            _ = migrationBuilder.Sql(
                @"ALTER TABLE CampaignBreakRequirements
                  ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignBreakRequirements",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignBreakRequirements
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM CampaignBreakRequirements cb LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cb.SalesArea = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignBreakRequirements",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "CampaignBreakRequirements");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakRequirements_SalesAreaId",
                table: "CampaignBreakRequirements",
                column: "SalesAreaId");

            // CampaignBookingPositionGroupSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_Name",
                table: "CampaignBookingPositionGroupSalesAreas");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignBookingPositionGroupSalesAreas",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignBookingPositionGroupSalesAreas
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM CampaignBookingPositionGroupSalesAreas cb JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cb.[Name] = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "CampaignBookingPositionGroupSalesAreas",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "Name",
                table: "CampaignBookingPositionGroupSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_SalesAreaId",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // PredictionSchedules

            _ = migrationBuilder.DropIndex(
                name: "IX_PredictionSchedules_SalesAreaId",
                table: "PredictionSchedules");

            _ = migrationBuilder.DropIndex(
                name: "IX_PredictionSchedules_SalesAreaId_ScheduleDay",
                table: "PredictionSchedules");

            _ = migrationBuilder.Sql(
                "ALTER TABLE PredictionSchedules ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE PredictionSchedules
                  SET SalesArea = t.[Name]
                  FROM PredictionSchedules p JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON p.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE PredictionSchedules ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "PredictionSchedules");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesArea",
                table: "PredictionSchedules",
                column: "SalesArea");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesArea_ScheduleDay",
                table: "PredictionSchedules",
                columns: new[] { "SalesArea", "ScheduleDay" });

            // CampaignTimeRestrictionsSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_SalesAreaId",
                table: "CampaignTimeRestrictionsSalesAreas");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignTimeRestrictionsSalesAreas ADD [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignTimeRestrictionsSalesAreas
                  SET [Name] = t.[Name]
                  FROM CampaignTimeRestrictionsSalesAreas ct JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON ct.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignTimeRestrictionsSalesAreas ALTER COLUMN [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "CampaignTimeRestrictionsSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_Name",
                table: "CampaignTimeRestrictionsSalesAreas",
                column: "Name");

            // CampaignSalesAreaTargets

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignSalesAreaTargets_SalesAreaId",
                table: "CampaignSalesAreaTargets");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignSalesAreaTargets ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignSalesAreaTargets
                  SET SalesArea = t.[Name]
                  FROM CampaignSalesAreaTargets cs JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cs.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignSalesAreaTargets ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "CampaignSalesAreaTargets");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargets_SalesArea",
                table: "CampaignSalesAreaTargets",
                column: "SalesArea");

            // CampaignSalesAreaTargetGroupSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_SalesAreaId",
                table: "CampaignSalesAreaTargetGroupSalesAreas");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignSalesAreaTargetGroupSalesAreas ADD [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignSalesAreaTargetGroupSalesAreas
                  SET [Name] = t.[Name]
                  FROM CampaignSalesAreaTargetGroupSalesAreas cs JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cs.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignSalesAreaTargetGroupSalesAreas ALTER COLUMN [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "CampaignSalesAreaTargetGroupSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_Name",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                column: "Name");

            // CampaignProgrammeRestrictionsSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_SalesAreaId",
                table: "CampaignProgrammeRestrictionsSalesAreas");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignProgrammeRestrictionsSalesAreas ADD [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignProgrammeRestrictionsSalesAreas
                  SET [Name] = t.[Name]
                  FROM CampaignProgrammeRestrictionsSalesAreas cp JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cp.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignProgrammeRestrictionsSalesAreas ALTER COLUMN [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "CampaignProgrammeRestrictionsSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_Name",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                column: "Name");

            // CampaignBreakRequirements

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignBreakRequirements_SalesAreaId",
                table: "CampaignBreakRequirements");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignBreakRequirements ADD SalesArea NVARCHAR(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignBreakRequirements
                  SET SalesArea = ISNULL(t.[Name], '')
                  FROM CampaignBreakRequirements cb LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cb.SalesAreaId = t.Id");

            //_ = migrationBuilder.Sql(
            //    "ALTER TABLE CampaignBreakRequirements ALTER COLUMN SalesArea NVARCHAR(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "CampaignBreakRequirements");

            // CampaignBookingPositionGroupSalesAreas

            _ = migrationBuilder.DropIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_SalesAreaId",
                table: "CampaignBookingPositionGroupSalesAreas");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignBookingPositionGroupSalesAreas ADD [Name] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE CampaignBookingPositionGroupSalesAreas
                  SET [Name] = t.[Name]
                  FROM CampaignBookingPositionGroupSalesAreas cb JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON cb.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE CampaignBookingPositionGroupSalesAreas ALTER COLUMN [Name] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "CampaignBookingPositionGroupSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_Name",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "Name");
        }
    }
}
