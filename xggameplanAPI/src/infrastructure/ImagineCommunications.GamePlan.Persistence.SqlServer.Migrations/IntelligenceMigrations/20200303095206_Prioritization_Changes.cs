using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class Prioritization_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagePriorities");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "GroupTransactionInfos");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "GroupTransactionInfos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MessageEntityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageEntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    MessageEntityTypeId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTypes_MessageEntityTypes_MessageEntityTypeId",
                        column: x => x.MessageEntityTypeId,
                        principalTable: "MessageEntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_MessageEntityTypeId",
                table: "MessageTypes",
                column: "MessageEntityTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageTypes");

            migrationBuilder.DropTable(
                name: "MessageEntityTypes");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "GroupTransactionInfos");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "GroupTransactionInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MessagePriorities",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagePriorities", x => x.Id);
                });
        }
    }
}
