using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18290_RunSalesAreaPriority_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                    name: "IX_RunSalesAreaPriorities_SalesArea",
                    table: "RunSalesAreaPriorities");

            _ = migrationBuilder.AddColumn<Guid>(
                    name: "SalesAreaId",
                    table: "RunSalesAreaPriorities",
                    nullable: true);

            _ = migrationBuilder.Sql(
                    @"UPDATE RunSalesAreaPriorities
                    SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                    FROM RunSalesAreaPriorities s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea = t.[Name]");

            _ = migrationBuilder.Sql(
                    @"DELETE FROM RunSalesAreaPriorities
                    WHERE SalesAreaId is null");

            _ = migrationBuilder.AlterColumn<Guid>(
                    name: "SalesAreaId",
                    table: "RunSalesAreaPriorities",
                    nullable: false,
                    oldNullable: true,
                    oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                    name: "SalesArea",
                    table: "RunSalesAreaPriorities");

            _ = migrationBuilder.CreateIndex(
                    name: "IX_RunSalesAreaPriorities_SalesAreaId",
                    table: "RunSalesAreaPriorities",
                    column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                    name: "IX_RunSalesAreaPriorities_SalesAreaId",
                    table: "RunSalesAreaPriorities");

            _ = migrationBuilder.Sql(
                    @"ALTER TABLE RunSalesAreaPriorities
                    ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                    @"UPDATE RunSalesAreaPriorities
                    SET SalesArea = t.[Name]
                    FROM RunSalesAreaPriorities s JOIN
                    (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                    @"ALTER TABLE RunSalesAreaPriorities
                    ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                    name: "SalesAreaId",
                    table: "RunSalesAreaPriorities");

            _ = migrationBuilder.CreateIndex(
                    name: "IX_RunSalesAreaPriorities_SalesArea",
                    table: "RunSalesAreaPriorities",
                    column: "SalesArea");
        }
    }
}
