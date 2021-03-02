using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18291_RSSetting_SalesAreaDemographics_TotalRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region RSSettings
            _ = migrationBuilder.DropIndex(
                name: "IX_RSSettings_SalesArea",
                table: "RSSettings");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "RSSettings",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE RSSettings
                SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                FROM RSSettings s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "RSSettings",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "RSSettings");

            _ = migrationBuilder.CreateIndex(
                name: "IX_RSSettings_SalesAreaId",
                table: "RSSettings",
                column: "SalesAreaId");

            #endregion RSSettings

            #region SalesAreaDemographics
            _ = migrationBuilder.DropIndex(
                name: "IX_SalesAreaDemographics_SalesArea",
                table: "SalesAreaDemographics");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "SalesAreaDemographics",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE SalesAreaDemographics
                SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                FROM SalesAreaDemographics s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "SalesAreaDemographics",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.Sql("ALTER TABLE SalesAreaDemographics DROP CONSTRAINT FK_SalesAreaDemographics_SalesAreas_SalesArea");

            _ = migrationBuilder.DropColumn(
                 name: "SalesArea",
                 table: "SalesAreaDemographics");

            _ = migrationBuilder.CreateIndex(
                name: "IX_SalesAreaDemographics_SalesAreaId",
                table: "SalesAreaDemographics",
                column: "SalesAreaId");

            #endregion SalesAreaDemographics

            #region TotalRatings
            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "TotalRatings",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE TotalRatings
                SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                FROM TotalRatings s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "TotalRatings",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "TotalRatings");

            _ = migrationBuilder.CreateIndex(
                name: "IX_TotalRatings_SalesAreaId",
                table: "TotalRatings",
                column: "SalesAreaId");

            #endregion TotalRatings
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region RSSettings
            _ = migrationBuilder.DropIndex(
                name: "IX_RSSettings_SalesAreaId",
                table: "RSSettings");

            _ = migrationBuilder.Sql(
                "ALTER TABLE RSSettings ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE RSSettings
                SET SalesArea = t.[Name]
                FROM RSSettings s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "RSSettings");

            _ = migrationBuilder.CreateIndex(
                name: "IX_RSSettings_SalesArea",
                table: "RSSettings",
                column: "SalesArea");

            #endregion RSSettings

            #region SalesAreaDemographics
            _ = migrationBuilder.DropIndex(
                name: "IX_SalesAreaDemographics_SalesAreaId",
                table: "SalesAreaDemographics");

            _ = migrationBuilder.Sql(
                "ALTER TABLE SalesAreaDemographics ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE SalesAreaDemographics
                SET SalesArea = t.[Name]
                FROM SalesAreaDemographics s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE SalesAreaDemographics ALTER COLUMN SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.Sql(
                @"ALTER TABLE SalesAreaDemographics
                ADD CONSTRAINT FK_SalesAreaDemographics_SalesAreas_SalesArea FOREIGN KEY(SalesArea) references SalesAreas(Name)");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "SalesAreaDemographics");

            _ = migrationBuilder.CreateIndex(
                name: "IX_SalesAreaDemographics_SalesArea",
                table: "SalesAreaDemographics",
                column: "SalesArea");

            #endregion SalesAreaDemographics

            #region TotalRatings
            _ = migrationBuilder.DropIndex(
                name: "IX_TotalRatings_SalesAreaId",
                table: "TotalRatings");

            _ = migrationBuilder.Sql(
                "ALTER TABLE TotalRatings ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE TotalRatings
                SET SalesArea = t.[Name]
                FROM TotalRatings s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE TotalRatings ALTER COLUMN SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "TotalRatings");

            #endregion TotalRatings
        }
    }
}
