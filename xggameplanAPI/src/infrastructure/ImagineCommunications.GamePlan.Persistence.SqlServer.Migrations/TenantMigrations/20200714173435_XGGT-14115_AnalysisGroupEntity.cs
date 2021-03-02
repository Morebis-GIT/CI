using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14115_AnalysisGroupEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FilterAdvertiserExternalIds = table.Column<string>(nullable: true),
                    FilterAgencyExternalIds = table.Column<string>(nullable: true),
                    FilterAgencyGroupCodes = table.Column<string>(nullable: true),
                    FilterBusinessTypes = table.Column<string>(nullable: true),
                    FilterCampaignExternalIds = table.Column<string>(nullable: true),
                    FilterClashExternalRefs = table.Column<string>(nullable: true),
                    FilterProductExternalIds = table.Column<string>(nullable: true),
                    FilterReportingCategories = table.Column<string>(nullable: true),
                    FilterSalesExecExternalIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisGroups_IsDeleted",
                table: "AnalysisGroups",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisGroups");
        }
    }
}
