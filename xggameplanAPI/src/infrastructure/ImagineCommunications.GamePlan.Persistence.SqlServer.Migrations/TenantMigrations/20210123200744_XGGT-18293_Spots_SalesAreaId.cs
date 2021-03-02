using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18293_Spots_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
               name: "IX_Spots_SalesArea",
               table: "Spots");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "Spots",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE Spots
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM Spots s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.AlterColumn<Guid>(
                name: "SalesAreaId",
                table: "Spots",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "Spots");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Spots_SalesAreaId",
                table: "Spots",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_Spots_SalesAreaId",
                table: "Spots");

            _ = migrationBuilder.Sql(
                "ALTER TABLE Spots ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE Spots
                  SET SalesArea = t.[Name]
                  FROM Spots s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "Spots");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Spots_SalesArea",
                table: "Spots",
                column: "SalesArea");
        }
    }
}
