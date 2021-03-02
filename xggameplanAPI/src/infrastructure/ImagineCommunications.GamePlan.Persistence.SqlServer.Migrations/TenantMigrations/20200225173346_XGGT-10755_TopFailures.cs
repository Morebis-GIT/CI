using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10755_TopFailures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SELECT * INTO #Failures_temp FROM [dbo].[Failures]");

            migrationBuilder.Sql("DELETE FROM [dbo].[Failures]");

            migrationBuilder.DropIndex(
                name: "IX_Failures_ScenarioId",
                table: "Failures");

            migrationBuilder.DropColumn(
                name: "Campaign",
                table: "Failures");

            migrationBuilder.DropColumn(
                name: "CampaignName",
                table: "Failures");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Failures");

            migrationBuilder.DropColumn(
                name: "Failures",
                table: "Failures");

            migrationBuilder.DropColumn(
                name: "SalesAreaName",
                table: "Failures");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Failures");

            migrationBuilder.CreateTable(
                name: "FailureItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FailureId = table.Column<int>(nullable: false),
                    Campaign = table.Column<long>(nullable: false),
                    CampaignName = table.Column<string>(maxLength: 256, nullable: true),
                    ExternalId = table.Column<string>(maxLength: 256, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Failures = table.Column<long>(nullable: false),
                    SalesAreaName = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailureItems_Failures_FailureId",
                        column: x => x.FailureId,
                        principalTable: "Failures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Failures_ScenarioId",
                table: "Failures",
                column: "ScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FailureItems_FailureId",
                table: "FailureItems",
                column: "FailureId");

            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[Failures] ([ScenarioId])
                SELECT DISTINCT [ScenarioId] FROM #Failures_temp"
            );

            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[FailureItems] ([FailureId],[Campaign],[CampaignName], [ExternalId],[Type], [Failures], [SalesAreaName])
                    SELECT f.[Id], ft.[Campaign], ft.[CampaignName], ft.[ExternalId], ft.[Type], ft.[Failures], ft.[SalesAreaName] FROM #Failures_temp ft
                    INNER JOIN [dbo].[Failures] f ON f.[ScenarioId] = ft.[ScenarioId]
                ");

            migrationBuilder.Sql("DROP TABLE #Failures_temp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"SELECT * INTO #Failures_temp FROM 
                    (
                        SELECT f.[ScenarioId], fi.[Campaign], fi.[CampaignName], fi.[ExternalId], fi.[Type], fi.[Failures], fi.[SalesAreaName] 
                        FROM [dbo].[Failures] f 
                        INNER JOIN [dbo].[FailureItems] fi ON f.[Id] = fi.[FailureId]
                    ) t
                ");

            migrationBuilder.Sql("DELETE FROM [dbo].[Failures]");

            migrationBuilder.DropIndex(
                name: "IX_Failures_ScenarioId",
                table: "Failures");

            migrationBuilder.AddColumn<long>(
                name: "Campaign",
                table: "Failures",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CampaignName",
                table: "Failures",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Failures",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Failures",
                table: "Failures",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SalesAreaName",
                table: "Failures",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Failures",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Failures_ScenarioId",
                table: "Failures",
                column: "ScenarioId");
            
            migrationBuilder.DropTable(
                name: "FailureItems");

            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[Failures] ([ScenarioId], [Campaign], [CampaignName], [ExternalId], [Type], [Failures], [SalesAreaName])
                    SELECT ft.[ScenarioId], ft.[Campaign], ft.[CampaignName], ft.[ExternalId], ft.[Type], ft.[Failures], ft.[SalesAreaName] 
                    FROM #Failures_temp ft"
                );

            migrationBuilder.Sql("DROP TABLE #Failures_temp");
        }
    }
}
