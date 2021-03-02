using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13649_ProductChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advertisers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true),
                    ExternalIdentifier = table.Column<string>(maxLength: 64, nullable: false),
                    TokenizedName = table.Column<string>(nullable: true, computedColumnSql: "CONCAT_WS(' ', ExternalIdentifier, Name, ShortName)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true),
                    ExternalIdentifier = table.Column<string>(maxLength: 64, nullable: false),
                    TokenizedName = table.Column<string>(nullable: true, computedColumnSql: "CONCAT_WS(' ', ExternalIdentifier, Name, ShortName)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgencyGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShortName = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    ExternalIdentifier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAdvertisers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<Guid>(nullable: false),
                    AdvertiserId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdvertisers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisers_Advertisers_AdvertiserId",
                        column: x => x.AdvertiserId,
                        principalTable: "Advertisers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisers_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAgencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<Guid>(nullable: false),
                    AgencyId = table.Column<int>(nullable: false),
                    AgencyGroupId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAgencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAgencies_AgencyGroups_AgencyGroupId",
                        column: x => x.AgencyGroupId,
                        principalTable: "AgencyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAgencies_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAgencies_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPersons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<Guid>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPersons_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPersons_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisers_ExternalIdentifier",
                table: "Advertisers",
                column: "ExternalIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_ExternalIdentifier",
                table: "Agencies",
                column: "ExternalIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyGroups_ShortName_Code",
                table: "AgencyGroups",
                columns: new[] { "ShortName", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ExternalIdentifier",
                table: "Persons",
                column: "ExternalIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisers_AdvertiserId",
                table: "ProductAdvertisers",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisers_ProductId",
                table: "ProductAdvertisers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisers_ProductId_AdvertiserId_StartDate_EndDate",
                table: "ProductAdvertisers",
                columns: new[] { "ProductId", "AdvertiserId", "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_AgencyGroupId",
                table: "ProductAgencies",
                column: "AgencyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_AgencyId",
                table: "ProductAgencies",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_ProductId",
                table: "ProductAgencies",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_ProductId_AgencyId_AgencyGroupId_StartDate_EndDate",
                table: "ProductAgencies",
                columns: new[] { "ProductId", "AgencyId", "AgencyGroupId", "StartDate", "EndDate" },
                unique: true,
                filter: "[AgencyGroupId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPersons_PersonId",
                table: "ProductPersons",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPersons_ProductId",
                table: "ProductPersons",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPersons_ProductId_PersonId_StartDate_EndDate",
                table: "ProductPersons",
                columns: new[] { "ProductId", "PersonId", "StartDate", "EndDate" },
                unique: true);

            //migrate product data to the new structure
            migrationBuilder.Sql(
                @"insert into Advertisers
                 (ExternalIdentifier, [Name], ShortName)
                 select t.AdvertiserIdentifier as ExternalIdentifier, (select top 1 ap.AdvertiserName from Products ap where ap.AdvertiserIdentifier = t.AdvertiserIdentifier) as [Name], null as ShortName
                 from (select distinct AdvertiserIdentifier from Products where AdvertiserIdentifier is not null) as t");
            migrationBuilder.Sql(
                @"insert into Agencies
                 (ExternalIdentifier, [Name], ShortName)
                 select t.AgencyIdentifier as ExternalIdentifier, (select top 1 ap.AgencyName from Products ap where ap.AgencyIdentifier = t.AgencyIdentifier) as [Name], null as ShortName
                 from (select distinct AgencyIdentifier from Products where AgencyIdentifier is not null) as t");
            migrationBuilder.Sql(
                @"insert into ProductAdvertisers
                 (ProductId, AdvertiserId, StartDate, EndDate)
                 select p.[Uid] as ProductId, a.Id as AdvertiserId, p.AdvertiserLinkStartDate as StartDate, p.AdvertiserLinkEndDate as EndDate
                 from Products p inner join Advertisers a on a.ExternalIdentifier = p.AdvertiserIdentifier");
            migrationBuilder.Sql(
                @"insert into ProductAgencies
                 (ProductId, AgencyId, StartDate, EndDate, AgencyGroupId)
                 select p.[Uid] as ProductId, a.Id as AgencyId, p.AgencyStartDate as StartDate, p.AgencyLinkEndDate as EndDate, null as AgencyGroupId
                 from Products p inner join Agencies a on a.ExternalIdentifier = p.AgencyIdentifier");

            migrationBuilder.Sql("DROP FULLTEXT INDEX ON [Products]", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG [Products]", true);

            migrationBuilder.DropIndex(
                name: "IX_Products_AdvertiserIdentifier",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AdvertiserName",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AgencyName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TokenizedAdvertiser",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TokenizedCampaign",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AdvertiserIdentifier",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AdvertiserLinkEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AdvertiserLinkStartDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AdvertiserName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AgencyIdentifier",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AgencyLinkEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AgencyStartDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AgencyName",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ReportingCategory",
                table: "Products",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [Products]", true);
            migrationBuilder.Sql(
                "CREATE FULLTEXT INDEX ON [dbo].[Advertisers] KEY INDEX [PK_Advertisers] ON ([Products]) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Advertisers] ADD ([TokenizedName])", true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Advertisers] ENABLE", true);
            migrationBuilder.Sql(
                "CREATE FULLTEXT INDEX ON [dbo].[Agencies] KEY INDEX [PK_Agencies] ON ([Products]) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Agencies] ADD ([TokenizedName])", true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Agencies] ENABLE", true);
            migrationBuilder.Sql(
                "CREATE FULLTEXT INDEX ON [Products] KEY INDEX [PK_Products] ON ([Products]) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Products] ADD ([TokenizedName])", true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Products] ENABLE", true);

            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[CampaignWithProductRelations]
                AS
                SELECT cmp.Id AS CampaignId, p.Uid AS ProductId, a.Id AS AdvertiserId, ag.Id AS AgencyId, agp.Id AS AgencyGroupId, pr.Id AS PersonId
                FROM  dbo.Campaigns AS cmp LEFT OUTER JOIN
                      dbo.Products AS p ON cmp.Product = p.Externalidentifier LEFT OUTER JOIN
                          (SELECT pa.ProductId, pa.AdvertiserId, cl.Id AS CampaignId
                           FROM      dbo.ProductAdvertisers AS pa INNER JOIN
                                             dbo.Products AS pl ON pl.Uid = pa.ProductId INNER JOIN
                                             dbo.Campaigns AS cl ON cl.Product = pl.Externalidentifier AND pa.StartDate <= cl.StartDateTime AND pa.EndDate > cl.StartDateTime) AS ta ON p.Uid = ta.ProductId AND cmp.Id = ta.CampaignId LEFT OUTER JOIN
                      dbo.Advertisers AS a ON a.Id = ta.AdvertiserId LEFT OUTER JOIN
                          (SELECT pa.ProductId, pa.AgencyId, pa.AgencyGroupId, cl.Id AS CampaignId
                           FROM      dbo.ProductAgencies AS pa INNER JOIN
                                             dbo.Products AS pl ON pl.Uid = pa.ProductId INNER JOIN
                                             dbo.Campaigns AS cl ON cl.Product = pl.Externalidentifier AND pa.StartDate <= cl.StartDateTime AND pa.EndDate > cl.StartDateTime) AS tag ON p.Uid = tag.ProductId AND cmp.Id = tag.CampaignId LEFT OUTER JOIN
                      dbo.Agencies AS ag ON ag.Id = tag.AgencyId LEFT OUTER JOIN
                      dbo.AgencyGroups AS agp ON agp.Id = tag.AgencyGroupId LEFT OUTER JOIN
                          (SELECT pp.ProductId, pp.PersonId, cl.Id AS CampaignId
                           FROM      dbo.ProductPersons AS pp INNER JOIN
                                             dbo.Products AS pl ON pl.Uid = pp.ProductId INNER JOIN
                                             dbo.Campaigns AS cl ON cl.Product = pl.Externalidentifier AND pp.StartDate <= cl.StartDateTime AND pp.EndDate > cl.StartDateTime) AS tap ON p.Uid = tap.ProductId AND cmp.Id = tap.CampaignId LEFT OUTER JOIN
                      dbo.Persons AS pr ON pr.Id = tap.PersonId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON [Advertisers]", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON [Agencies]", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON [Products]", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG [Products]", true);
            migrationBuilder.Sql("DROP VIEW [dbo].[CampaignWithProductRelations]");

            migrationBuilder.DropColumn(
                name: "ReportingCategory",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "AdvertiserIdentifier",
                table: "Products",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdvertiserLinkEndDate",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdvertiserLinkStartDate",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdvertiserName",
                table: "Products",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgencyIdentifier",
                table: "Products",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgencyLinkEndDate",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgencyStartDate",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgencyName",
                table: "Products",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenizedAdvertiser",
                table: "Products",
                nullable: true,
                computedColumnSql: "CONCAT_WS(' ', AdvertiserIdentifier, AdvertiserName)");

            migrationBuilder.AddColumn<string>(
                name: "TokenizedCampaign",
                table: "Products",
                nullable: true,
                computedColumnSql: "CONCAT_WS(' ', AdvertiserName, AgencyName, Name)");

            //update product data
            migrationBuilder.Sql(
                @"update Products
                 set AdvertiserIdentifier = a.ExternalIdentifier,
	                 AdvertiserName = a.[Name],
	                 AdvertiserLinkStartDate = pa.StartDate,
	                 AdvertiserLinkEndDate = pa.EndDate
                 from Products p
                 inner join ProductAdvertisers pa on pa.ProductId = p.[Uid]
                 inner join Advertisers a on a.Id = pa.AdvertiserId");

            migrationBuilder.Sql(
                @"update Products
                 set AgencyIdentifier = a.ExternalIdentifier,
	                 AgencyName = a.[Name],
	                 AgencyStartDate = pa.StartDate,
	                 AgencyLinkEndDate = pa.EndDate
                 from Products p
                 inner join ProductAgencies pa on pa.ProductId = p.[Uid]
                 inner join Agencies a on a.Id = pa.AgencyId");

            migrationBuilder.Sql(
                @"update Products
                 set AdvertiserLinkStartDate = '0001-01-01 00:00:00',
	                 AdvertiserLinkEndDate = '9999-12-31 23:59:59'
                 where AdvertiserLinkStartDate is null and AdvertiserLinkEndDate is null");

            migrationBuilder.Sql(
                @"update Products
                 set AgencyStartDate = '0001-01-01 00:00:00',
	                 AgencyLinkEndDate = '9999-12-31 23:59:59'
                 where AgencyStartDate is null and AgencyLinkEndDate is null");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AdvertiserLinkEndDate",
                table: "Products",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AdvertiserLinkStartDate",
                table: "Products",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AgencyLinkEndDate",
                table: "Products",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AgencyStartDate",
                table: "Products",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_AdvertiserIdentifier",
                table: "Products",
                column: "AdvertiserIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AdvertiserName",
                table: "Products",
                column: "AdvertiserName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AgencyName",
                table: "Products",
                column: "AgencyName");

            migrationBuilder.DropTable(
                name: "ProductAdvertisers");

            migrationBuilder.DropTable(
                name: "ProductAgencies");

            migrationBuilder.DropTable(
                name: "ProductPersons");

            migrationBuilder.DropTable(
                name: "Advertisers");

            migrationBuilder.DropTable(
                name: "AgencyGroups");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [Products]", true);
            migrationBuilder.Sql(
                "CREATE FULLTEXT INDEX ON [Products] KEY INDEX [PK_Products] ON ([Products]) WITH (CHANGE_TRACKING AUTO)",
                true);
            migrationBuilder.Sql(
                "ALTER FULLTEXT INDEX ON [Products] ADD ([TokenizedAdvertiser], [TokenizedCampaign], [TokenizedName])",
                true);
            migrationBuilder.Sql("ALTER FULLTEXT INDEX ON [Products] ENABLE", true);
        }
    }
}
