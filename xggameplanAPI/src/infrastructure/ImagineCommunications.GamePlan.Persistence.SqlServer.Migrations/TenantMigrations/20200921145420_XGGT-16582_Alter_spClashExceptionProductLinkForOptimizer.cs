using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16582_Alter_spClashExceptionProductLinkForOptimizer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                IF OBJECT_ID('[dbo].[spClashExceptionProductLinkForOptimizer]') IS NULL
                    EXEC('CREATE PROCEDURE [dbo].[spClashExceptionProductLinkForOptimizer] AS SET NOCOUNT ON;');");

            _ = migrationBuilder.Sql(@"
                ALTER PROCEDURE [dbo].[spClashExceptionProductLinkForOptimizer]
	                @EndDate datetime2 = null
                AS
                BEGIN
	                SET NOCOUNT ON;
                    WITH product_cte (ProductExternalRef)
                    AS
                    (SELECT Externalidentifier FROM dbo.Products),
                    advertiser_cte (AdvertiserExternalRef, ProductExternalRef, StartDate, EndDate)
                    AS
                    (SELECT a.ExternalIdentifier, p.Externalidentifier AS Expr1, pa.StartDate, pa.EndDate
                    FROM dbo.ProductAdvertisers AS pa
                    INNER JOIN dbo.Products AS p ON p.Uid = pa.ProductId
                    INNER JOIN dbo.Advertisers AS a ON a.Id = pa.AdvertiserId)
                    SELECT DISTINCT t.Id, t.FromValue, t.ToValue
                    FROM (SELECT ce.Id,
                            CASE ce.FromType
                                WHEN 0 THEN ce.FromValue
                                WHEN 1 THEN product_from.ProductExternalRef
                                WHEN 2 THEN adv_from.ProductExternalRef END AS FromValue,
                            CASE ce.ToType
                                WHEN 0 THEN ce.ToValue
                                WHEN 1 THEN product_to.ProductExternalRef
                                WHEN 2 THEN adv_to.ProductExternalRef END AS ToValue
                            FROM dbo.ClashExceptions AS ce
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                IF OBJECT_ID('[dbo].[spClashExceptionProductLinkForOptimizer]') IS NULL
                    EXEC('CREATE PROCEDURE [dbo].[spClashExceptionProductLinkForOptimizer] AS SET NOCOUNT ON;');");

            _ = migrationBuilder.Sql(@"
                ALTER PROCEDURE [dbo].[spClashExceptionProductLinkForOptimizer]
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
        }
    }
}
