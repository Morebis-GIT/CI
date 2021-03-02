using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceDbMigrations
{
    public partial class InitIntelligenceDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupTransactionInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventCount = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ReceivedDate = table.Column<DateTime>(nullable: false),
                    ExecutedDate = table.Column<DateTime>(nullable: true),
                    CompletedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTransactionInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageEntityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageEntityTypes", x => x.Id);
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
                    ExecutedDate = table.Column<DateTime>(nullable: true),
                    TotalBatchCount = table.Column<int>(nullable: true),
                    ProcessedBatchCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageInfos_GroupTransactionInfos_GroupTransactionId",
                        column: x => x.GroupTransactionId,
                        principalTable: "GroupTransactionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    BatchSize = table.Column<int>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "MessagePayloads",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Payload = table.Column<byte[]>(type: "LongBlob", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_MessageInfos_GroupTransactionId",
                table: "MessageInfos",
                column: "GroupTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_MessageEntityTypeId",
                table: "MessageTypes",
                column: "MessageEntityTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagePayloads");

            migrationBuilder.DropTable(
                name: "MessageTypes");

            migrationBuilder.DropTable(
                name: "MessageInfos");

            migrationBuilder.DropTable(
                name: "MessageEntityTypes");

            migrationBuilder.DropTable(
                name: "GroupTransactionInfos");
        }
    }
}
