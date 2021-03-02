using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18183_ClashDifference_IndexType_InventoryLock_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //-- IndexTypes
            _ = migrationBuilder.DropIndex(
                name: "IX_IndexTypes_SalesArea",
                table: "IndexTypes");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "IndexTypes",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE IndexTypes
                  SET SalesAreaId = t.Id
                  FROM IndexTypes s LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "IndexTypes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_SalesAreaId",
                table: "IndexTypes",
                column: "SalesAreaId");

            //-- ClashDifferences
            _ = migrationBuilder.DropIndex(
                name: "IX_ClashDifferences_SalesArea",
                table: "ClashDifferences");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "ClashDifferences",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE ClashDifferences
                  SET SalesAreaId = t.Id
                  FROM ClashDifferences s LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ClashDifferences");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClashDifferences_SalesAreaId",
                table: "ClashDifferences",
                column: "SalesAreaId");

            //-- InventoryLocks
            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "InventoryLocks",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE InventoryLocks
                  SET SalesAreaId = t.Id
                  FROM InventoryLocks s LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea COLLATE Latin1_General_100_CS_AI = t.[Name]");

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "InventoryLocks");

            _ = migrationBuilder.CreateIndex(
                name: "IX_InventoryLocks_SalesAreaId",
                table: "InventoryLocks",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //-- IndexTypes
            _ = migrationBuilder.DropIndex(
                name: "IX_IndexTypes_SalesAreaId",
                table: "IndexTypes");

            _ = migrationBuilder.Sql(
                "ALTER TABLE IndexTypes ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE IndexTypes
                  SET SalesArea = t.[Name]
                  FROM IndexTypes s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "IndexTypes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_SalesArea",
                table: "IndexTypes",
                column: "SalesArea");

            //-- ClashDifferences
            _ = migrationBuilder.DropIndex(
                name: "IX_ClashDifferences_SalesAreaId",
                table: "ClashDifferences");

            _ = migrationBuilder.Sql(
                "ALTER TABLE ClashDifferences ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE ClashDifferences
                  SET SalesArea = t.[Name]
                  FROM ClashDifferences s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "ClashDifferences");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClashDifferences_SalesArea",
                table: "ClashDifferences",
                column: "SalesArea");

            //-- InventoryLocks
            _ = migrationBuilder.DropIndex(
                name: "IX_InventoryLocks_SalesAreaId",
                table: "InventoryLocks");

            _ = migrationBuilder.Sql(
                "ALTER TABLE InventoryLocks ADD SalesArea NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(
                @"UPDATE InventoryLocks
                  SET SalesArea = t.[Name]
                  FROM InventoryLocks s LEFT JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "InventoryLocks");
        }
    }
}
