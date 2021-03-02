using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18293_StandardDayParts_And_Groups_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region StandardDayParts
            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "StandardDayParts",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE StandardDayParts
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM StandardDayParts s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "StandardDayParts",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "StandardDayParts");

            _ = migrationBuilder.CreateIndex(
                name: "IX_StandardDayParts_SalesAreaId",
                table: "StandardDayParts",
                column: "SalesAreaId");
            #endregion StandardDayParts

            #region StandardDayPartGroups
            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "StandardDayPartGroups",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE StandardDayPartGroups
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM StandardDayPartGroups s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "StandardDayPartGroups",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "StandardDayPartGroups");

            _ = migrationBuilder.CreateIndex(
                name: "IX_StandardDayPartGroups_SalesAreaId",
                table: "StandardDayPartGroups",
                column: "SalesAreaId");
            #endregion StandardDayPartGroups
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region StandardDayParts
            _ = migrationBuilder.DropIndex(
                name: "IX_StandardDayParts_SalesAreaId",
                table: "StandardDayParts");

            _ = migrationBuilder.Sql(
                "ALTER TABLE StandardDayParts ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE StandardDayParts
                  SET SalesArea = t.[Name]
                  FROM StandardDayParts s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE StandardDayParts ALTER COLUMN SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "StandardDayParts");
            #endregion StandardDayParts

            #region StandardDayPartGroups
            _ = migrationBuilder.DropIndex(
                name: "IX_StandardDayPartGroups_SalesAreaId",
                table: "StandardDayPartGroups");

            _ = migrationBuilder.Sql(
                "ALTER TABLE StandardDayPartGroups ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE StandardDayPartGroups
                  SET SalesArea = t.[Name]
                  FROM StandardDayPartGroups s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE StandardDayPartGroups ALTER COLUMN SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "StandardDayPartGroups");
            #endregion StandardDayPartGroups
        }
    }
}
