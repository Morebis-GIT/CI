using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18534_Restriction_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_RestrictionsSalesAreas_SalesArea",
                table: "RestrictionsSalesAreas");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "RestrictionsSalesAreas",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            _ = migrationBuilder.Sql(
                @"UPDATE RestrictionsSalesAreas
                  SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                  FROM RestrictionsSalesAreas s LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "RestrictionsSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_RestrictionsSalesAreas_SalesAreaId",
                table: "RestrictionsSalesAreas",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_RestrictionsSalesAreas_SalesAreaId",
                table: "RestrictionsSalesAreas");

            _ = migrationBuilder.Sql(
                "ALTER TABLE RestrictionsSalesAreas ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE RestrictionsSalesAreas
                  SET SalesArea = t.[Name]
                  FROM RestrictionsSalesAreas s LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "RestrictionsSalesAreas");

            _ = migrationBuilder.CreateIndex(
                name: "IX_RestrictionsSalesAreas_SalesArea",
                table: "RestrictionsSalesAreas",
                column: "SalesArea");
        }
    }
}
