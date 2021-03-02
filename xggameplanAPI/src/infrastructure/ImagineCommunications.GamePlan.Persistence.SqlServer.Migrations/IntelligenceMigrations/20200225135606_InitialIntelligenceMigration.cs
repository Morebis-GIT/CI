using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class InitialIntelligenceMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupTransactionInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventCount = table.Column<int>(nullable: false),
                    RetryCount = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ReceivedDate = table.Column<DateTime>(nullable: false),
                    ExecutedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTransactionInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GroupTransactionId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RetryCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ReceivedDate = table.Column<DateTime>(nullable: false),
                    ExecutedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageInfos", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "MessagePayloads",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Payload = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagePayloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessagePayloads_MessageInfos_Id",
                        column: x => x.Id,
                        principalTable: "MessageInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupTransactionInfos");

            migrationBuilder.DropTable(
                name: "MessagePayloads");

            migrationBuilder.DropTable(
                name: "MessagePriorities");

            migrationBuilder.DropTable(
                name: "MessageInfos");
        }
    }
}
