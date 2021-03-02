using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18532_Universe_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_Universes_SalesArea",
                table: "Universes");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Universes",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE Universes
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM Universes s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "Universes",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Universes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Universes_SalesAreaId",
                table: "Universes",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_Universes_SalesAreaId",
                table: "Universes");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Universes ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE Universes
                  SET SalesArea = t.[Name]
                  FROM Universes s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Universes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Universes_SalesArea",
                table: "Universes",
                column: "SalesArea");
        }
    }
}
