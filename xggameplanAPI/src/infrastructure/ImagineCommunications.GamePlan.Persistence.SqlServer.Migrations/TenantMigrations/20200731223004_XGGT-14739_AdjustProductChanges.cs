using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14739_AdjustProductChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var item in Enum.GetValues(typeof(ClashExceptionType)))
            {
                migrationBuilder.Sql($"UPDATE ClashExceptions SET FromType = '{(int)item}' WHERE FromType = '{item}'");
                migrationBuilder.Sql($"UPDATE ClashExceptions SET ToType = '{(int)item}' WHERE ToType = '{item}'");
            }

            migrationBuilder.AlterColumn<int>(
                name: "FromType",
                table: "ClashExceptions",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ToType",
                table: "ClashExceptions",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[ClashExceptionDescriptions] AS
                    WITH product_cte (ProductName, ProductExternalRef)
                    AS
                    (SELECT Name, Externalidentifier FROM dbo.Products),
                    clash_cte (ClashName, ClashExternalRef)
                    AS
                    (SELECT Description, Externalref FROM dbo.Clashes),
                    advertiser_cte (AdvertiserName, AdvertiserExternalRef, StartDate, EndDate)
                    AS
                    (SELECT a.Name, a.ExternalIdentifier, pa.StartDate, pa.EndDate
                    FROM dbo.ProductAdvertisers AS pa
                    INNER JOIN dbo.Advertisers AS a ON a.Id = pa.AdvertiserId)
                    SELECT Id,
                      CASE ce.FromType
                        WHEN 0 THEN (SELECT ClashName FROM clash_cte WHERE ClashExternalRef = ce.FromValue)
                        WHEN 1 THEN (SELECT ProductName FROM product_cte WHERE ProductExternalRef = ce.FromValue)
                        WHEN 2 THEN
                            (SELECT TOP 1 COALESCE (AdvertiserName, AdvertiserExternalRef) FROM advertiser_cte
                            WHERE AdvertiserExternalRef = ce.FromValue
                              AND (StartDate < COALESCE(DATEADD(day, 1, ce.EndDate), CONVERT(datetime2, '9999-12-31')) AND EndDate > ce.StartDate))
                      END AS FromValueDescription,
                      CASE ce.ToType
                        WHEN 0 THEN (SELECT ClashName FROM clash_cte WHERE ClashExternalRef = ce.ToValue)
                        WHEN 1 THEN (SELECT ProductName FROM product_cte WHERE ProductExternalRef = ce.ToValue)
                        WHEN 2 THEN
                            (SELECT TOP 1 COALESCE (AdvertiserName, AdvertiserExternalRef) FROM advertiser_cte
                            WHERE AdvertiserExternalRef = ce.ToValue
			                  AND (StartDate < COALESCE(DATEADD(day, 1, ce.EndDate), CONVERT(datetime2, '9999-12-31')) AND EndDate > ce.StartDate))
                      END AS ToValueDescription
                    FROM dbo.ClashExceptions AS ce");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE [dbo].[spClashExceptionProductLinkForOptimizer]
	                @EndDate datetime2 = null
                AS
                BEGIN
	                SET NOCOUNT ON;
                    WITH product_cte (ProductExternalRef)
                    AS
                    (SELECT Externalidentifier FROM dbo.Products),
                    clash_cte (ClashExternalRef, ParentClashExternalRef)
                    AS
                    (SELECT Externalref, ParentExternalidentifier FROM dbo.Clashes),
                    advertiser_cte (AdvertiserExternalRef, ProductExternalRef, StartDate, EndDate)
                    AS
                    (SELECT a.ExternalIdentifier, p.Externalidentifier AS Expr1, pa.StartDate, pa.EndDate
                    FROM dbo.ProductAdvertisers AS pa
                    INNER JOIN dbo.Products AS p ON p.Uid = pa.ProductId
                    INNER JOIN dbo.Advertisers AS a ON a.Id = pa.AdvertiserId)
                    SELECT DISTINCT t.Id, t.FromValue, t.ToValue
                    FROM (SELECT ce.Id,
                            CASE ce.FromType
                                WHEN 0 THEN clash_from.ClashExternalRef
                                WHEN 1 THEN product_from.ProductExternalRef
                                WHEN 2 THEN adv_from.ProductExternalRef END AS FromValue,
                            CASE ce.ToType
                                WHEN 0 THEN clash_to.ClashExternalRef
                                WHEN 1 THEN product_to.ProductExternalRef
                                WHEN 2 THEN adv_to.ProductExternalRef END AS ToValue
                            FROM dbo.ClashExceptions AS ce
                            LEFT OUTER JOIN clash_cte AS clash_from ON ce.FromType = 0 AND (clash_from.ClashExternalRef = ce.FromValue OR clash_from.ParentClashExternalRef = ce.FromValue)
                            LEFT OUTER JOIN clash_cte AS clash_to ON ce.ToType = 0 AND (clash_to.ClashExternalRef = ce.ToValue OR clash_to.ParentClashExternalRef = ce.ToValue)
                            LEFT OUTER JOIN product_cte AS product_from ON ce.FromType = 1 AND product_from.ProductExternalRef = ce.FromValue
                            LEFT OUTER JOIN product_cte AS product_to ON ce.ToType = 1 AND product_to.ProductExternalRef = ce.ToValue
                            LEFT OUTER JOIN advertiser_cte AS adv_from ON
				                ce.FromType = 2 AND adv_from.AdvertiserExternalRef = ce.FromValue AND
				                adv_from.StartDate < COALESCE (DATEADD(day, 1, ce.EndDate), @EndDate, CONVERT(datetime2, '9999-12-31')) AND adv_from.EndDate > ce.StartDate
                            LEFT OUTER JOIN advertiser_cte AS adv_to ON
				                ce.ToType = 2 AND adv_to.AdvertiserExternalRef = ce.ToValue AND
				                adv_to.StartDate < COALESCE (DATEADD(day, 1, ce.EndDate), @EndDate, CONVERT(datetime2, '9999-12-31')) AND adv_to.EndDate > ce.StartDate) as t
	                WHERE t.FromValue IS NOT NULL and t.ToValue IS NOT NULL
                END");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE [dbo].[spGetRecommendationAggregate]
	                @ScenarioId uniqueidentifier
                AS
                BEGIN
	                SET NOCOUNT ON;
	                SELECT t.ExternalCampaignNumber, t.SpotRating, c.CampaignGroup, c.[Name] as CampaignName, c.ActualRatings, c.TargetRatings, c.EndDateTime, c.IsPercentage, ad.[Name] as AdvertiserName
	                FROM (
		                SELECT r.ExternalCampaignNumber,
		                SUM(CASE r.[Action] WHEN 'b' THEN [r].[SpotRating] WHEN 'c' THEN -r.SpotRating END) AS SpotRating
		                FROM Recommendations AS r
		                WHERE r.ScenarioId = @ScenarioId AND (r.[Action] = N'B' OR r.[Action] = N'C')
		                GROUP BY r.ExternalCampaignNumber) as t
	                INNER JOIN Campaigns c ON c.ExternalId = t.ExternalCampaignNumber
	                INNER JOIN CampaignWithProductRelations pr ON pr.CampaignId = c.Id
	                LEFT JOIN Advertisers ad ON ad.Id = pr.AdvertiserId

                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spGetRecommendationAggregate]");
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spClashExceptionProductLinkForOptimizer]");
            migrationBuilder.Sql("DROP VIEW [dbo].[ClashExceptionDescriptions]");

            migrationBuilder.AlterColumn<string>(
                name: "ToType",
                table: "ClashExceptions",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "FromType",
                table: "ClashExceptions",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int));

            foreach (var item in Enum.GetValues(typeof(ClashExceptionType)))
            {
                migrationBuilder.Sql($"UPDATE ClashExceptions SET FromType = '{item}' WHERE FromType = '{(int)item}'");
                migrationBuilder.Sql($"UPDATE ClashExceptions SET ToType = '{item}' WHERE ToType = '{(int)item}'");
            }
        }
    }
}
