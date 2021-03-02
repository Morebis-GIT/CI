using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterDbMigrations
{
    public partial class InitSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(maxLength: 128, nullable: false),
                    UserId = table.Column<int>(nullable: false, defaultValue: 0),
                    ValidUntilValue = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<string>(maxLength: 128, nullable: false),
                    TenantId = table.Column<int>(nullable: false, defaultValue: 0),
                    Status = table.Column<int>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    TimeCompleted = table.Column<DateTime>(nullable: false),
                    TimeLastActive = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantProductFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Enabled = table.Column<bool>(nullable: false, defaultValue: false),
                    IsShared = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProductFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    DefaultTheme = table.Column<string>(maxLength: 128, nullable: true),
                    TenantDb_Provider = table.Column<int>(nullable: false),
                    TenantDb_ConnectionString = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UpdateDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    TenantId = table.Column<int>(nullable: false, defaultValue: 0),
                    TimeApplied = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Surname = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    Password = table.Column<string>(maxLength: 256, nullable: true),
                    ThemeName = table.Column<string>(maxLength: 128, nullable: false),
                    Location = table.Column<string>(maxLength: 256, nullable: true),
                    Role = table.Column<string>(maxLength: 128, nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DefaultTimeZone = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSettingFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    ProductSettingsId = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false, defaultValue: false),
                    Settings = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSettingFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSettingFeatures_ProductSettings_ProductSettingsId",
                        column: x => x.ProductSettingsId,
                        principalTable: "ProductSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskInstanceParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskInstanceId = table.Column<Guid>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskInstanceParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskInstanceParameters_TaskInstances_TaskInstanceId",
                        column: x => x.TaskInstanceId,
                        principalTable: "TaskInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantProductFeatureReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantProductFeatureChildId = table.Column<int>(nullable: false),
                    TenantProductFeatureParentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProductFeatureReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantProductFeatureReferences_TenantProductFeatures_TenantP~",
                        column: x => x.TenantProductFeatureChildId,
                        principalTable: "TenantProductFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Previews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(maxLength: 256, nullable: true),
                    FileName = table.Column<string>(maxLength: 128, nullable: false),
                    FileExtension = table.Column<string>(maxLength: 64, nullable: true),
                    ContentType = table.Column<string>(maxLength: 64, nullable: false),
                    ContentLength = table.Column<long>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Content = table.Column<byte[]>(type: "LongBlob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Previews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Previews_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Previews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Token",
                table: "AccessTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_Previews_TenantId",
                table: "Previews",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Previews_UserId",
                table: "Previews",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSettingFeatures_ProductSettingsId",
                table: "ProductSettingFeatures",
                column: "ProductSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstanceParameters_TaskInstanceId",
                table: "TaskInstanceParameters",
                column: "TaskInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstances_TaskId",
                table: "TaskInstances",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantProductFeatureReferences_TenantProductFeatureChildId_T~",
                table: "TenantProductFeatureReferences",
                columns: new[] { "TenantProductFeatureChildId", "TenantProductFeatureParentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantProductFeatures_Name_TenantId",
                table: "TenantProductFeatures",
                columns: new[] { "Name", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_Name",
                table: "UserSettings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropTable(
                name: "Previews");

            migrationBuilder.DropTable(
                name: "ProductSettingFeatures");

            migrationBuilder.DropTable(
                name: "TaskInstanceParameters");

            migrationBuilder.DropTable(
                name: "TenantProductFeatureReferences");

            migrationBuilder.DropTable(
                name: "UpdateDetails");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "ProductSettings");

            migrationBuilder.DropTable(
                name: "TaskInstances");

            migrationBuilder.DropTable(
                name: "TenantProductFeatures");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
