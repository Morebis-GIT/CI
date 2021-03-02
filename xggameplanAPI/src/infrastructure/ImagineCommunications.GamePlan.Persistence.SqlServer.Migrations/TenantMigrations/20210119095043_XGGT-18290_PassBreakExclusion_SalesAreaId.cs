using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18290_PassBreakExclusion_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<Guid>(
                    name: "SalesAreaId",
                    table: "PassBreakExclusions",
                    nullable: true);

            _ = migrationBuilder.Sql(
                    @"UPDATE PassBreakExclusions
                    SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                    FROM PassBreakExclusions s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea = t.[Name]");

            _ = migrationBuilder.Sql(
                    @"DELETE FROM PassBreakExclusions
                    WHERE SalesAreaId is null");

            _ = migrationBuilder.AlterColumn<Guid>(
                    name: "SalesAreaId",
                    table: "PassBreakExclusions",
                    nullable: false,
                    oldNullable: true,
                    oldClrType: typeof(Guid));

            _ = migrationBuilder.DropColumn(
                    name: "SalesArea",
                    table: "PassBreakExclusions");

            _ = migrationBuilder.CreateIndex(
                    name: "IX_PassBreakExclusions_SalesAreaId",
                    table: "PassBreakExclusions",
                    column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                    name: "IX_PassBreakExclusions_SalesAreaId",
                    table: "PassBreakExclusions");

            _ = migrationBuilder.Sql(
                    @"ALTER TABLE PassBreakExclusions
                    ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                    @"UPDATE PassBreakExclusions
                    SET SalesArea = t.[Name]
                    FROM PassBreakExclusions s JOIN
                    (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(
                    @"ALTER TABLE PassBreakExclusions
                    ALTER COLUMN SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL");

            _ = migrationBuilder.DropColumn(
                    name: "SalesAreaId",
                    table: "PassBreakExclusions");
        }
    }
}
