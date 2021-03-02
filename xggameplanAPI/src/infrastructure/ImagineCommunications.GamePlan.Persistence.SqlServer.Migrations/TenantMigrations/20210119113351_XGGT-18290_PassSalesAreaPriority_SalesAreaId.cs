using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18290_PassSalesAreaPriority_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<Guid>(
                    name: "SalesAreaId",
                    table: "PassSalesAreaPriorities",
                    nullable: true);

            _ = migrationBuilder.Sql(
                    @"UPDATE PassSalesAreaPriorities
                    SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                    FROM PassSalesAreaPriorities s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea = t.[Name]");

            _ = migrationBuilder.Sql(
                    @"DELETE FROM PassSalesAreaPriorities
                    WHERE SalesAreaId is null");

            _ = migrationBuilder.AlterColumn<Guid>(
                    name: "SalesAreaId",
                    table: "PassSalesAreaPriorities",
                    nullable: false,
                    oldNullable: true,
                    oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                    name: "SalesArea",
                    table: "PassSalesAreaPriorities");

            _ = migrationBuilder.CreateIndex(
                    name: "IX_PassSalesAreaPriorities_SalesAreaId",
                    table: "PassSalesAreaPriorities",
                    column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                    name: "IX_PassSalesAreaPriorities_SalesAreaId",
                    table: "PassSalesAreaPriorities");

            _ = migrationBuilder.Sql(
                    @"ALTER TABLE PassSalesAreaPriorities
                    ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                    @"UPDATE PassSalesAreaPriorities
                    SET SalesArea = t.[Name]
                    FROM PassSalesAreaPriorities s JOIN
                    (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                    @"ALTER TABLE PassSalesAreaPriorities
                    ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                    name: "SalesAreaId",
                    table: "PassSalesAreaPriorities");
        }
    }
}
