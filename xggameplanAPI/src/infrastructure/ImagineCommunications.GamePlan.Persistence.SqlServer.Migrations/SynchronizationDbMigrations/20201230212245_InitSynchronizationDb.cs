using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.SynchronizationDbMigrations
{
    public partial class InitSynchronizationDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SynchronizationObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(type: "Varbinary(16)", nullable: true),
                    OwnerCount = table.Column<int>(nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SynchronizationObjectOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SynchronizationObjectId = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<string>(maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    CapturedDate = table.Column<DateTime>(nullable: false),
                    ReleasedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationObjectOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynchronizationObjectOwners_SynchronizationObjects_Synchroni~",
                        column: x => x.SynchronizationObjectId,
                        principalTable: "SynchronizationObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationObjectOwners_SynchronizationObjectId",
                table: "SynchronizationObjectOwners",
                column: "SynchronizationObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SynchronizationObjectOwners");

            migrationBuilder.DropTable(
                name: "SynchronizationObjects");
        }
    }
}
