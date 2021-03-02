using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18292_SmoothDiagnosticConf_SponsorshipItem_SpotBookingRule_SalesAreaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // SponsorshipItemsSalesAreas
            _ = migrationBuilder.CreateTable(
                name: "SponsorshipItemsSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SponsorshipItemId = table.Column<int>(nullable: false),
                    SalesAreaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_SponsorshipItemsSalesAreas", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_SponsorshipItemsSalesAreas_SponsorshipItems_SponsorshipItemId",
                        column: x => x.SponsorshipItemId,
                        principalTable: "SponsorshipItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_SponsorshipItemsSalesAreas_SponsorshipItemId",
                table: "SponsorshipItemsSalesAreas",
                column: "SponsorshipItemId");

            _ = migrationBuilder.Sql(@"INSERT INTO SponsorshipItemsSalesAreas(SponsorshipItemId, SalesAreaId)
                SELECT si.Id as SponsorshipItemId, sa.Id as SalesAreaId 
                FROM SponsorshipItems AS si CROSS APPLY STRING_SPLIT(si.SalesAreas, N'‖')
                LEFT JOIN [SalesAreas] AS sa ON [Name] = value");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreas",
                table: "SponsorshipItems");


            // SmoothDiagnosticConfigurations
            _ = migrationBuilder.CreateTable(
                name: "SmoothDiagnosticConfigurationSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SmoothDiagnosticConfigurationId = table.Column<int>(nullable: false),
                    SalesAreaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_SmoothDiagnosticConfigurationSalesAreas", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_SmoothDiagnosticConfigurationSalesAreas_SmoothDiagnosticConfigurations_SmoothDiagnosticConfigurationId",
                        column: x => x.SmoothDiagnosticConfigurationId,
                        principalTable: "SmoothDiagnosticConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_SmoothDiagnosticConfigurationSalesAreas_SmoothDiagnosticConfigurationId",
                table: "SmoothDiagnosticConfigurationSalesAreas",
                column: "SmoothDiagnosticConfigurationId");

            _ = migrationBuilder.Sql(@"INSERT INTO SmoothDiagnosticConfigurationSalesAreas(SmoothDiagnosticConfigurationId, SalesAreaId)
                SELECT si.Id AS SmoothDiagnosticConfigurationId, sa.Id AS SalesAreaId 
                FROM SmoothDiagnosticConfigurations AS si CROSS APPLY STRING_SPLIT(si.SpotSalesAreas, N'‖')
                LEFT JOIN [SalesAreas] AS sa ON [Name] = value;");

            _ = migrationBuilder.DropColumn(
                name: "SpotSalesAreas",
                table: "SmoothDiagnosticConfigurations");


            // SpotBookingRuleSalesAreas
            _ = migrationBuilder.AddColumn<Guid>(
                name: "SalesAreaId",
                table: "SpotBookingRuleSalesAreas",
                defaultValue: Guid.Empty
             );

            _ = migrationBuilder.Sql(@"UPDATE SpotBookingRuleSalesAreas 
                       SET SalesAreaId = sa.id
                       FROM SpotBookingRuleSalesAreas as sr
                       Inner Join SalesAreas sa 
                       on sa.Name COLLATE Latin1_General_100_CS_AI = sr.Name");

               _ = migrationBuilder.DropColumn(
                   name: "Name",
                   table: "SpotBookingRuleSalesAreas");

                _ = migrationBuilder.AlterColumn<Guid>(
                    name: "SalesAreaId",
                    table: "SpotBookingRuleSalesAreas"
                );

            // Indexes
            _ = migrationBuilder.CreateIndex(
                name: "IX_SpotBookingRuleSalesAreas_SalesAreaId",
                table: "SpotBookingRuleSalesAreas",
                column: "SalesAreaId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_SponsorshipItemsSalesAreas_SalesAreaId",
                table: "SponsorshipItemsSalesAreas",
                column: "SalesAreaId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_SmoothDiagnosticConfigurationSalesAreas_SalesAreaId",
                table: "SmoothDiagnosticConfigurationSalesAreas",
                column: "SalesAreaId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // SmoothDiagnosticConfigurations
            _ = migrationBuilder.AddColumn<string>(
                name: "SpotSalesAreas",
                table: "SmoothDiagnosticConfigurations",
                nullable: false,
                defaultValue: "");

            _ = migrationBuilder.Sql(@"
            SELECT si.SmoothDiagnosticConfigurationId,
                   STRING_AGG (sa.Name, N'‖')
              WITHIN GROUP (ORDER BY SalesAreaId ASC) AS csv
                INTO #t_values
                FROM SmoothDiagnosticConfigurationSalesAreas AS si
                INNER JOIN SalesAreas sa
                ON sa.Id  = si.SalesAreaId
                GROUP BY SmoothDiagnosticConfigurationId;

            UPDATE SmoothDiagnosticConfigurations 
                SET SpotSalesAreas = #t_values.csv
                FROM SmoothDiagnosticConfigurations AS si
                INNER JOIN #t_values ON #t_values.SmoothDiagnosticConfigurationId = si.Id;

            DROP TABLE #t_values;");

            _ = migrationBuilder.DropTable(
                name: "SmoothDiagnosticConfigurationSalesAreas");

            // SponsorshipItemsSalesAreas
            _ = migrationBuilder.AddColumn<string>(
                name: "SalesAreas",
                table: "SponsorshipItems",
                nullable: true);

            _ = migrationBuilder.Sql(@"
                SELECT si.SponsorshipItemId, STRING_AGG (sa.Name, N'‖')
                  WITHIN GROUP (ORDER BY SalesAreaId ASC) AS csv
                    INTO #t_values
                    FROM SponsorshipItemsSalesAreas AS si
                    INNER JOIN SalesAreas sa
                    ON sa.Id  = si.SalesAreaId
                    GROUP BY SponsorshipItemId

                UPDATE SponsorshipItems 
                    SET SalesAreas = #t_values.csv
                    FROM SponsorshipItems AS si
                    INNER JOIN #t_values ON #t_values.SponsorshipItemId = si.Id;

                DROP TABLE #t_values;");

            _ = migrationBuilder.DropTable(
                name: "SponsorshipItemsSalesAreas");

            //SpotBookingRuleSalesAreas

            _ = migrationBuilder.AddColumn<string>(
               name: "Name",
               table: "SpotBookingRuleSalesAreas",
               maxLength: 512,
               nullable: false,
               defaultValue: "");

            _ = migrationBuilder.Sql(@"
                        UPDATE SpotBookingRuleSalesAreas 
                        SET Name = sa.Name
                        FROM SpotBookingRuleSalesAreas as sr
                        INNER JOIN SalesAreas sa 
                        ON sa.Id = sr.SalesAreaId");

            _ = migrationBuilder.DropIndex(
                      name: "IX_SpotBookingRuleSalesAreas_SalesAreaId",
                      table: "SpotBookingRuleSalesAreas");

            _ = migrationBuilder.DropColumn(
                name: "SalesAreaId",
                table: "SpotBookingRuleSalesAreas");


        }
    }
}
