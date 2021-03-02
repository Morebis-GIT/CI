using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18188_ISRSettings_LengthFactor_LSAPP_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_ISRSettings_SalesArea",
                table: "ISRSettings");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "ISRSettings",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE ISRSettings
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM ISRSettings s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "ISRSettings",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ISRSettings");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ISRSettings_SalesAreaId",
                table: "ISRSettings",
                column: "SalesAreaId");

            //--

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "SalesAreaPriorities",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE SalesAreaPriorities
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM SalesAreaPriorities s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "SalesAreaPriorities",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "SalesAreaPriorities");

            _ = migrationBuilder.CreateIndex(
                name: "IX_SalesAreaPriorities_SalesAreaId",
                table: "SalesAreaPriorities",
                column: "SalesAreaId");

            //--

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "LengthFactors",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE LengthFactors
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM LengthFactors s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "LengthFactors",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "LengthFactors");

            _ = migrationBuilder.CreateIndex(
                name: "IX_LengthFactors_SalesAreaId",
                table: "LengthFactors",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_ISRSettings_SalesAreaId",
                table: "ISRSettings");

            _ = migrationBuilder.Sql(
                "ALTER TABLE ISRSettings ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE ISRSettings
                  SET SalesArea = t.[Name]
                  FROM ISRSettings s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "ISRSettings");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ISRSettings_SalesArea",
                table: "ISRSettings",
                column: "SalesArea");

            //--

            _ = migrationBuilder.DropIndex(
                name: "IX_SalesAreaPriorities_SalesAreaId",
                table: "SalesAreaPriorities");

            _ = migrationBuilder.Sql(
                "ALTER TABLE SalesAreaPriorities ADD SalesArea NVARCHAR(256) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE SalesAreaPriorities
                  SET SalesArea = t.[Name]
                  FROM SalesAreaPriorities s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "SalesAreaPriorities");

            //--

            _ = migrationBuilder.DropIndex(
                name: "IX_LengthFactors_SalesAreaId",
                table: "LengthFactors");

            _ = migrationBuilder.Sql(
                "ALTER TABLE LengthFactors ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE LengthFactors
                  SET SalesArea = t.[Name]
                  FROM LengthFactors s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                "ALTER TABLE LengthFactors ALTER COLUMN SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "LengthFactors");
        }
    }
}
