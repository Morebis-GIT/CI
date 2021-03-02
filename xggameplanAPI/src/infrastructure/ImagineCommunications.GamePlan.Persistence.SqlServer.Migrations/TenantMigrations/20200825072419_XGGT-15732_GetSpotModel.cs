using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT15732_GetSpotModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[SpotWithCampaignAndProductRelations]
                AS
                SELECT sp.Uid AS SpotUid, cmp.ExternalId AS CampaignExternalId, p.Uid AS ProductId, pa.Id AS ProductAdvertiserId, pag.Id AS ProductAgencyId, pp.Id AS ProductPersonId
                FROM  dbo.Spots AS sp LEFT OUTER JOIN
                      dbo.Campaigns AS cmp ON cmp.ExternalId = sp.ExternalCampaignNumber LEFT OUTER JOIN
                      dbo.Products AS p ON sp.Product = p.Externalidentifier LEFT OUTER JOIN
                      dbo.ProductAdvertisers AS pa ON p.Uid = pa.ProductId AND pa.StartDate <= ISNULL(cmp.StartDateTime, sp.StartDateTime) AND pa.EndDate > ISNULL(cmp.StartDateTime, sp.StartDateTime) LEFT OUTER JOIN
                      dbo.Advertisers AS a ON a.Id = pa.AdvertiserId LEFT OUTER JOIN
                      dbo.ProductAgencies AS pag ON p.Uid = pag.ProductId AND pag.StartDate <= ISNULL(cmp.StartDateTime, sp.StartDateTime) AND pag.EndDate > ISNULL(cmp.StartDateTime, sp.StartDateTime) LEFT OUTER JOIN
                      dbo.Agencies AS ag ON ag.Id = pag.AgencyId LEFT OUTER JOIN
                      dbo.AgencyGroups AS agp ON agp.Id = pag.AgencyGroupId LEFT OUTER JOIN
                      dbo.ProductPersons AS pp ON p.Uid = pp.ProductId AND pp.StartDate <= ISNULL(cmp.StartDateTime, sp.StartDateTime) AND pp.EndDate > ISNULL(cmp.StartDateTime, sp.StartDateTime) LEFT OUTER JOIN
                      dbo.Persons AS pr ON pr.Id = pp.PersonId
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW [dbo].[SpotWithCampaignAndProductRelations]");
        }
    }
}
