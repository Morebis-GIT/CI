using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18290_PassRatingPoint_SalesAreaRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "PassRatingPoint_SalesAreaRefs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PassRatingPointId = table.Column<int>(nullable: false),
                    SalesAreaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_PassRatingPoint_SalesAreaRefs", x => x.Id);
                    _ = table.ForeignKey(
                             name: "FK_PassRatingPoint_SalesAreaRefs_PassRatingPoints_PassRatingPointId",
                             column: x => x.PassRatingPointId,
                             principalTable: "PassRatingPoints",
                             principalColumn: "Id",
                             onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                ALTER COLUMN SalesAreaId uniqueidentifier NULL");

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                DROP CONSTRAINT FK_PassRatingPoint_SalesAreaRefs_PassRatingPoints_PassRatingPointId
                GO");

            _ = migrationBuilder.Sql(@"
                DECLARE @delimiter nvarchar(1) = N'‖';
                INSERT INTO PassRatingPoint_SalesAreaRefs
                        ([PassRatingPointId]
                        ,[SalesArea])
                SELECT Id, ltrim(rtrim(value))
                FROM PassRatingPoints
                CROSS APPLY STRING_SPLIT(SalesAreas, @delimiter)");

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                WITH CHECK ADD CONSTRAINT FK_PassRatingPoint_SalesAreaRefs_PassRatingPoints_PassRatingPointId
                FOREIGN KEY(PassRatingPointId) REFERENCES PassRatingPoints(Id)");

            _ = migrationBuilder.Sql(
                @"UPDATE PassRatingPoint_SalesAreaRefs
                SET SalesAreaId = ISNULL(t.Id, '00000000-0000-0000-0000-000000000000')
                FROM PassRatingPoint_SalesAreaRefs s JOIN (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesArea = t.[Name]");

            _ = migrationBuilder.Sql(
                @"DELETE FROM PassRatingPoint_SalesAreaRefs
                WHERE SalesAreaId is null");

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                ALTER COLUMN SalesAreaId uniqueidentifier NOT NULL");

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                DROP COLUMN SalesArea");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreas",
                table: "PassRatingPoints");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PassRatingPoint_SalesAreaRefs_PassRatingPointId",
                table: "PassRatingPoint_SalesAreaRefs",
                column: "PassRatingPointId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PassRatingPoint_SalesAreaRefs_SalesAreaId",
                table: "PassRatingPoint_SalesAreaRefs",
                column: "SalesAreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<string>(
              name: "SalesAreas",
              table: "PassRatingPoints",
              nullable: true);

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                ADD SalesArea NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL");

            _ = migrationBuilder.Sql(@"
                UPDATE PassRatingPoint_SalesAreaRefs
                SET SalesArea = t.[Name]
                FROM PassRatingPoint_SalesAreaRefs s JOIN
                (SELECT sa.Id, sa.[Name] FROM SalesAreas sa) AS t ON s.SalesAreaId = t.Id");

            _ = migrationBuilder.Sql(@"
                UPDATE PassRatingPoints
                SET SalesAreas = (SELECT STRING_AGG(SalesArea, N'‖')
                FROM PassRatingPoint_SalesAreaRefs sar
                WHERE PassRatingPointId = PassRatingPoints.Id)");

            _ = migrationBuilder.Sql(@"
                ALTER TABLE PassRatingPoint_SalesAreaRefs
                DROP COLUMN SalesArea");

            _ = migrationBuilder.DropTable(
                 name: "PassRatingPoint_SalesAreaRefs");
        }
    }
}
