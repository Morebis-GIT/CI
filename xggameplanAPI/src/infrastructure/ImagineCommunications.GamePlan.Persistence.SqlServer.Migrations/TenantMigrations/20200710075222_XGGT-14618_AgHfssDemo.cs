using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14618_AgHfssDemo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgHfssDemos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    IndexType = table.Column<int>(nullable: false),
                    BaseDemoNo = table.Column<int>(nullable: false),
                    IndexDemoNo = table.Column<int>(nullable: false),
                    BreakScheduledDate = table.Column<string>(maxLength: 32, nullable: true),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgHfssDemos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgHfssDemos_AutoBookDefaultParameters_AutoBookDefaultParametersId",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgHfssDemos_AutoBookDefaultParametersId",
                table: "AgHfssDemos",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.Sql(@"
                INSERT INTO AgHfssDemos
                SELECT  AgHfssDemo_SalesAreaNo,
                        AgHfssDemo_IndexType,
                        AgHfssDemo_BaseDemoNo,
                        AgHfssDemo_IndexDemoNo,
                        AgHfssDemo_BreakScheduledDate,
                        Id
                FROM AutoBookDefaultParameters
            ");

            migrationBuilder.DropColumn(
                name: "AgHfssDemo_BaseDemoNo",
                table: "AutoBookDefaultParameters");

            migrationBuilder.DropColumn(
                name: "AgHfssDemo_BreakScheduledDate",
                table: "AutoBookDefaultParameters");

            migrationBuilder.DropColumn(
                name: "AgHfssDemo_IndexDemoNo",
                table: "AutoBookDefaultParameters");

            migrationBuilder.DropColumn(
                name: "AgHfssDemo_IndexType",
                table: "AutoBookDefaultParameters");

            migrationBuilder.DropColumn(
                name: "AgHfssDemo_SalesAreaNo",
                table: "AutoBookDefaultParameters");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgHfssDemo_BaseDemoNo",
                table: "AutoBookDefaultParameters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AgHfssDemo_BreakScheduledDate",
                table: "AutoBookDefaultParameters",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgHfssDemo_IndexDemoNo",
                table: "AutoBookDefaultParameters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AgHfssDemo_IndexType",
                table: "AutoBookDefaultParameters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AgHfssDemo_SalesAreaNo",
                table: "AutoBookDefaultParameters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                    UPDATE s SET 
                           s.AgHfssDemo_SalesAreaNo = c.SalesAreaNo,  
                           s.AgHfssDemo_IndexType = c.IndexType,  
                           s.AgHfssDemo_BaseDemoNo = c.BaseDemoNo,  
                           s.AgHfssDemo_IndexDemoNo = c.IndexDemoNo,  
                           s.AgHfssDemo_BreakScheduledDate = c.BreakScheduledDate
                      FROM AutoBookDefaultParameters s 
                INNER JOIN dbo.AgHfssDemos c ON c.AutoBookDefaultParametersId = s.Id
            ");

            migrationBuilder.DropTable(
                name: "AgHfssDemos");

        }
    }
}
