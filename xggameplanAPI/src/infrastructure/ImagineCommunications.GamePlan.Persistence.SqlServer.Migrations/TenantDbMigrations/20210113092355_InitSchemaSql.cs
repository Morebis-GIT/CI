using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantDbMigrations
{
    public partial class InitSchemaSql : Migration
    {
        private const int MaxRatingsForSpotCampaignsRuleId = 21;
        private const int MaxRatingsForRatingCampaignsRuleId = 27;
        private const int True = 1;
        private const int False = 0;
        private const int TarpsFaultTypeId = 82;
        private const string EngLanguageabbreviation = "ENG";
        private const string araLanguageabbreviation = "aRa";
        private const string TarpsDescription = "Min TaRPs not met";
        private const string SlottingControlsFunctionalareaId = "3152EDD5-CD20-40aC-B315-C8D5D9FE890B";
        private const string FileId = "XG_CaMP_FaIL_REPT.out";
        private const string Description = "Scenario Campaign Failures";
        private const string autoBookFileName = "XG_CaMP_FaIL_REPT.out";
        private const string MaxRatingsForSpotCampaignsDescriptionNew = "Max Rating Points for Spot Campaigns";
        private const string MaxRatingsForRatingCampaignsDescriptionNew = "Max Rating Points for Rating Campaigns";
        private const string MaxRatingsForSpotCampaignsDescriptionOld = "Max Ratings for Spot Campaigns";
        private const string MaxRatingsForRatingCampaignsDescriptionOld = "Max Ratings for Rating Campaigns";
        public const string AvailableRatingsByDemo = "availableRatings";

        private const int ExcludedPriorityId = 1;
        private readonly string[] _kpisToRemove = {
            "reservedRatingsADS",
            "ratingCampaignsRatedSpots",
            "spotCampaignsRatedSpots",
            "totalSpotsBooked"
        };
        private readonly string StartedRunsPassRulesQuery = $@"
                CREATE TEMPORARY table IF NOT EXISTS StartedRuns_PassRules
                (
	                RunId varchar(36),
	                PassRuleId int
                ); ";


        private readonly string StartedRunsPassRulesQuery2 = $@"insert into StartedRuns_PassRules
                select Runs.Id, PassRules.Id
                from Runs
                join RunScenarios 
                    ON RunScenarios.RunId = Runs.Id 
                    aND Runs.RunStatus != {(int)RunStatus.NotStarted}
                join ScenarioPassReferences 
                    ON ScenarioPassReferences.ScenarioId = RunScenarios.ScenarioId
                join PassRules 
                    ON PassRules.PassId = ScenarioPassReferences.PassId
                where RuleId in ({MaxRatingsForSpotCampaignsRuleId}, {MaxRatingsForRatingCampaignsRuleId})";




        private void AlterColumns(MigrationBuilder migrationBuilder)
        {
            var fieldInfo = this.TargetModel.FindEntityType("SalesAreas")?.FindProperty("Name")?.FieldInfo;
            var types = this.TargetModel.GetEntityTypes();



            migrationBuilder.DropForeignKey(
                    name: "FK_SalesAreaDemographics_SalesAreas_SalesArea",
                    table: "SalesAreaDemographics");

            migrationBuilder.SetCasesensitiveColumn(
                    columnName: "Name",
                    table: "SalesAreas",
                    maxLenght: 255,
                    nullable: false)
                .SetCasesensitiveColumn(
                    columnName: "ShortName",
                    table: "SalesAreas",
                    maxLenght: 255,
                    nullable: true)
                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "SalesAreaDemographics",
                    maxLenght: 255,
                    nullable: false)
                .AddForeignKey(
                    name: "FK_SalesAreaDemographics_SalesAreas_SalesArea",
                    table: "SalesAreaDemographics",
                    column: "SalesArea",
                    principalTable: "SalesAreas",
                    principalColumn: "Name");

            migrationBuilder.SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "Breaks",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "CampaignSalesAreaTargets",
                    maxLenght: 64,
                    nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "ClashDifferences",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesAreaName",
                   table: "FailureItems",
                   maxLenght: 512,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "IndexTypes",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "InventoryLocks",
                   maxLenght: 512,
                   nullable: false)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "ISRSettings",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "PassBreakExclusions",
                   maxLenght: 64,
                   nullable: false)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "PassSalesAreaPriorities",
                   maxLenght: 64,
                   nullable: false)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "PredictionSchedules",
                   maxLenght: 64,
                   nullable: false)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "Programmes",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "Recommendations",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "RestrictionsSalesAreas",
                   maxLenght: 64,
                   nullable: false)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "RSSettings",
                   maxLenght: 64,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "RunSalesAreaPriorities",
                   maxLenght: 64,
                   nullable: false)

                .SetCasesensitiveColumn(
                   columnName: "SalesArea",
                   table: "SalesAreaPriorities",
                   maxLenght: 256,
                   nullable: true)

                .SetCasesensitiveColumn(
                   columnName: "SalesAreaName",
                   table: "ScenarioCampaignResults",
                   maxLenght: 512,
                   nullable: true)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "ScheduleBreaks",
                    maxLenght: 64,
                    nullable: true)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "Schedules",
                    maxLenght: 64,
                    nullable: false)

                .SetLongTextColumn(
                    columnName: "SpotSalesAreas",
                    table: "SmoothDiagnosticConfigurations",
                    nullable: false)

                .SetLongTextColumn(
                    columnName: "SalesArea",
                    table: "SmoothFailures",
                    nullable: true)

                 .SetLongTextColumn(
                    columnName: "SalesAreas",
                    table: "SponsorshipItems",
                    nullable: true)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "Spots",
                    maxLenght: 64,
                    nullable: true)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "StandardDayPartGroups",
                    maxLenght: 512,
                    nullable: false)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "StandardDayParts",
                    maxLenght: 512,
                    nullable: false)

                .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "TotalRatings",
                    maxLenght: 512,
                    nullable: false)

                 .SetCasesensitiveColumn(
                    columnName: "SalesArea",
                    table: "Universes",
                    maxLenght: 64,
                    nullable: true)

                 .SetLongTextColumn(
                    columnName: "SalesAreas",
                    table: "AgCampaignProgrammes",
                    nullable: true)

                 .SetCasesensitiveColumn(
                    columnName: "Name",
                    table: "CampaignBookingPositionGroupSalesAreas",
                    maxLenght: 255,
                    nullable: false)

                .SetCasesensitiveColumn(
                    columnName: "Name",
                    table: "CampaignProgrammeRestrictionsSalesAreas",
                    maxLenght: 64,
                    nullable: false)

                 .SetCasesensitiveColumn(
                    columnName: "Name",
                    table: "CampaignSalesAreaTargetGroupSalesAreas",
                    maxLenght: 64,
                    nullable: false)

                  .SetCasesensitiveColumn(
                    columnName: "Name",
                    table: "CampaignTimeRestrictionsSalesAreas",
                    maxLenght: 64,
                    nullable: false);

        }

        private void UpTriggers(MigrationBuilder migrationBuilder)
        {

            /* Code is commented out for keep all potential FTS related fields in one place 
             migrationBuilder
                    .CreateTokenizedNameCalculation(
                       table: "Agencies",
                       expression: "CONCAT(NEW.ExternalIdentifier, ' ', NEW.Name, ' ', NEW.ShortName)")
                      .CreateTokenizedNameCalculation(
                       table: "Advertisers",
                       expression: "CONCAT(NEW.ExternalIdentifier, ' ', NEW.Name, ' ', NEW.ShortName)")
                   .CreateTokenizedNameCalculation(
                       table: "Clashes",
                       expression: "CONCAT(NEW.Externalref, ' ', NEW.Description)")
                   .CreateTokenizedNameCalculation(
                       table: "Passes",
                       expression: "CONCAT(NEW.Id, ' ', NEW.Name)")
                   .CreateTokenizedNameCalculation(
                       table: "Products",
                       expression: "CONCAT(NEW.Externalidentifier, ' ', NEW.Name)")

                   .CreateTokenizedNameCalculation(
                       table: "Scenarios",
                       expression: "CONCAT(NEW.Id, ' ', NEW.Name)");*/

            migrationBuilder.Sql(
                $"CREATE TRIGGER LibrarySalesAreaPassPrioritiesAutoIncreament " +
                $"BEFORE INSERT ON LibrarySalesAreaPassPriorities " +
                $"FOR EACH ROW " +
                    $"SET NEW.id = (SELECT ifnull(MAX(id), 0) + 1 FROM LibrarySalesAreaPassPriorities);");

            migrationBuilder.Sql(
                $"CREATE TRIGGER RunsInsertTriggers " +
                $"BEFORE INSERT ON Runs " +
                $"FOR EACH ROW BEGIN " +
                    $"SET NEW.CreatedOrExecuteDateTime = ifnull(NEW.ExecuteStartedDateTime, NEW.CreatedDateTime); " +
                    $"SET NEW.TokenizedName = CONCAT(NEW.Id, ' ', NEW.Description); END;");

            migrationBuilder.Sql(
               $"CREATE TRIGGER RunsUpdateTriggers " +
               $"BEFORE UPDATE ON Runs " +
               $"FOR EACH ROW BEGIN " +
                   $"SET NEW.CreatedOrExecuteDateTime = ifnull(NEW.ExecuteStartedDateTime, NEW.CreatedDateTime); " +
                   $"SET NEW.TokenizedName = CONCAT(NEW.Id, ' ', NEW.Description); END;");
        }

        private void UpViews(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW v_productAdvertisers_and_Products AS
                    SELECT 
	                    a.ExternalIdentifier, 
	                    p.Externalidentifier AS Expr1, 
	                    pa.StartDate, pa.EndDate
                    FROM ProductAdvertisers AS pa
                    INNER JOIN Products AS p 
	                    ON p.Uid = pa.ProductId
                    INNER JOIN Advertisers AS a 
	                    ON a.Id = pa.AdvertiserId;");


            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW CampaignWithProductRelations as
                    SELECT 
						cmp.Id AS CampaignId, 
						p.Uid AS ProductId, 
						a.Id AS AdvertiserId, 
						ag.Id AS AgencyId,   
						agp.Id AS AgencyGroupId,
						pr.Id AS PersonId
	                    FROM  Campaigns AS cmp 
                    LEFT OUTER JOIN Products AS p 
	                    ON cmp.Product = p.Externalidentifier 
                    LEFT OUTER JOIN ProductAdvertisers pad
						ON  p.Uid = pad.ProductId
						AND pad.StartDate <= cmp.StartDateTime 
     	                AND pad.EndDate > cmp.StartDateTime
                    LEFT OUTER JOIN Advertisers AS a 
	                    ON a.Id = pad.AdvertiserId 
					LEFT OUTER JOIN ProductAgencies pa
						ON pa.ProductId = p.Uid
						 AND pa.StartDate <= cmp.StartDateTime 
	 	                AND pa.EndDate > cmp.StartDateTime
                    LEFT OUTER JOIN Agencies AS ag 
	                    ON ag.Id = pa.AgencyId 
                    LEFT OUTER JOIN AgencyGroups AS agp 
	                    ON agp.Id = pa.AgencyGroupId 
                    LEFT OUTER JOIN	ProductPersons AS pp  
                        ON p.Uid = pp.ProductId 
                        	AND pp.StartDate <= cmp.StartDateTime
	                   		AND pp.EndDate > cmp.StartDateTime
                    LEFT OUTER JOIN Persons AS pr 
	                    ON pr.Id = pp.PersonId;");


            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW  SpotWithCampaignAndProductRelations as	
	            SELECT sp.Uid
		            AS SpotUid, 
		            cmp.ExternalId AS CampaignExternalId, 
		            p.Uid AS ProductId, 
		            pa.Id AS ProductAdvertiserId, 
		            pag.Id AS ProductAgencyId, 
		            pp.Id AS ProductPersonId
                FROM  Spots AS sp 
                LEFT OUTER JOIN Campaigns AS cmp 
    	            ON cmp.ExternalId = sp.ExternalCampaignNumber 
                LEFT OUTER JOIN Products AS p 
    	            ON sp.Product = p.Externalidentifier 
                LEFT OUTER JOIN ProductAdvertisers AS pa 
    	            ON p.Uid = pa.ProductId 
    	            AND pa.StartDate <= IFNULL(cmp.StartDateTime, sp.StartDateTime) 
    	            AND pa.EndDate > IFNULL(cmp.StartDateTime, sp.StartDateTime) 
                LEFT OUTER JOIN Advertisers AS a 
    	            ON a.Id = pa.AdvertiserId 
                LEFT OUTER JOIN ProductAgencies AS pag 
    	            ON p.Uid = pag.ProductId 
    	            AND pag.StartDate <= IFNULL(cmp.StartDateTime, sp.StartDateTime) 
    	            AND pag.EndDate > IFNULL(cmp.StartDateTime, sp.StartDateTime) 
                LEFT OUTER JOIN Agencies AS ag 
    	            ON ag.Id = pag.AgencyId 
   	            LEFT OUTER JOIN AgencyGroups AS agp 
   		            ON agp.Id = pag.AgencyGroupId 
   	            LEFT OUTER JOIN ProductPersons AS pp 
   		            ON p.Uid = pp.ProductId 
   		            AND pp.StartDate <= IFNULL(cmp.StartDateTime, sp.StartDateTime) 
   		            AND pp.EndDate > IFNULL(cmp.StartDateTime, sp.StartDateTime) 
   	            LEFT OUTER JOIN Persons AS pr 
   		            ON pr.Id = pp.PersonId;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW ClashExceptionDescriptions AS
                        SELECT Id,
                          CASE ce.FromType
                            WHEN 0 THEN  c.Description
                            WHEN 1 THEN  p.Name 
                            WHEN 2 THEN
                                (SELECT COALESCE (a.Name, a.ExternalIdentifier) FROM Advertisers a
                                inner join ProductAdvertisers pa
                                ON a.Id = pa.AdvertiserId
                                WHERE ExternalIdentifier = ce.FromValue
                                  AND (StartDate < COALESCE(DATE_ADD( ce.EndDate, interval 1 day),  cast('9999-12-31' as datetime)) 
                                  AND EndDate > ce.StartDate)limit 1)
                          END AS FromValueDescription ,
                         CASE ce.ToType
                            WHEN 0 THEN c1.Description 
                            WHEN 1 THEN  p1.Name 
                            WHEN 2 THEN
                                (SELECT  COALESCE (Name, ExternalIdentifier)  FROM Advertisers a
                                inner join ProductAdvertisers pa
                                ON a.Id = pa.AdvertiserId
                                WHERE ExternalIdentifier = ce.ToValue
			                      AND (StartDate < COALESCE(DATE_ADD(ce.EndDate,interval 1 Day),  cast('9999-12-31' as datetime)) 
			                      AND EndDate > ce.StartDate)limit 1)
                          END AS ToValueDescription
                        FROM ClashExceptions AS ce
                        LEFT JOIN Clashes c on c.externalref = ce.fromvalue
                        LEFT JOIN Products p on ce.fromvalue = p.Externalidentifier
						LEFT JOIN Clashes c1 on c1.externalref = ce.tovalue
                        LEFT JOIN Products p1 on ce.tovalue = p1.Externalidentifier;");

            migrationBuilder.Sql(@"CREATE OR REPLACE View v_clash_for_root as 
                    	SELECT
                            c.Externalref, c.ParentExternalidentifier, c.Externalref AS RootExternalRef
                        FROM
                            Clashes c
                        WHERE
                            c.ParentExternalidentifier IS NULL OR TRIM(c.ParentExternalidentifier) = ''");


            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS clashroots;");

            migrationBuilder.Sql(@"CREATE FUNCTION clashroots (v_exref varchar(50))
                                    RETURNS varchar(50)
                                    BEGIN
 	                                    declare val varchar(50);
 	                                    set val = '';
	                                    SELECT case when lv='' then v_exref else lv end as parents into val
	                                    FROM (
	                                    SELECT @pv:=(
	                                    SELECT GROUP_CONCAT(ParentExternalidentifier SEPARATOR ',')
	                                    FROM Clashes WHERE Externalref IN (@pv)
	                                    ) AS lv FROM Clashes
	                                    JOIN
	                                    (SELECT @pv:=v_exref)tmp
	                                    WHERE Externalref In (@pv) order by lv asc limit 1
	                                    ) a;
	                                    return val;
                                    END");

            migrationBuilder.Sql(@"CREATE OR REPLACE view ClashRoots as
                     SELECT c.Externalref AS Externalref,clashroots(c.Externalref) AS RootExternalRef 
                        FROM Clashes c;");

            migrationBuilder.Sql(@" CREATE OR REPLACE VIEW ProgrammesExternalRefs
	                    AS SELECT DISTINCT Name, ExternalReference
	                    FROM ProgrammeDictionaries;");

        }
        private void UpFunctions(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TABLE __Sequence (
                                Name  VARCHAR(50) NOT NULL,
                                Current_Value INT NOT NULL,
                                Increment       INT NOT NULL DEFAULT 1,
                                PRIMARY KEY(Name)
                                ) ENGINE = InnoDB; ");

            migrationBuilder.Sql(@"INSERT INTO __Sequence
                                values('CampaignNoIdentity',10000,1),
                                ('SalesAreaNoIdentity',10000,1),
                                ('RunNoIdentity',10000,1),
                                ('ScenarioNoIdentity',10000,1),
                                ('PassIdIdentity',10000,1)
                                ");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS currvalue;");

            migrationBuilder.Sql(@"CREATE FUNCTION currvalue(idname varchar(50))
                                  RETURNS int 
                                  LANGUAGE SQL -- This element is optional and will be omitted from subsequent examples
                                BEGIN
                                  declare cnt int;
                                  Set cnt=0;
                                  select current_value into cnt from __Sequence where name = idname ;
                                  Return cnt;
                                END;
                                ");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS setvalue;");

            migrationBuilder.Sql(@"CREATE FUNCTION setvalue(idname varchar(50))
                                 RETURNS varchar(50) 
                                 BEGIN
                                  declare cnt int;
                                  Set cnt=0;
  	                                UPDATE __Sequence SET current_value = current_value + increment
  	                                WHERE name = idname;
    
                                   Return idname;
                                END;
                                ");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS nextvalue;");
            migrationBuilder.Sql(@"CREATE FUNCTION nextvalue(idname varchar(50))
                                  RETURNS int 
  
                                BEGIN
                                  declare cnt int;
                                  Set cnt=0;
  	                                UPDATE __Sequence SET current_value = current_value + increment
  	                                WHERE name = idname;
                                   select current_value into cnt from __Sequence where name = idname ;
                                   Return cnt;
                                END;
                                ");
        }

        private void UpData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
              table: "BRSConfigurationForKPIs",
              keyColumn: "KPIName",
              keyValues: _kpisToRemove);

            migrationBuilder.Sql(@"CREATE Temporary table if not exists t_values(kpi varchar(50));");

            migrationBuilder.Sql(@"INSERT INTO t_values values
                ('conversionEfficiencyADS'),
               ('conversionEfficiencyMN1634'),
               ('conversionEfficiencyCHD'),
               ('conversionEfficiencyHWCH'),
               ('conversionEfficiencyADABC1'),
               ('availableRatingsADS'),
               ('availableRatingsMN1634'),
               ('availableRatingsCHD'),
               ('availableRatingsHWCH'),
               ('availableRatingsADABC1'),
               ('differenceValue'),
               ('differenceValuePercentage'),
               ('differenceValueWithPayback'),
               ('differenceValuePercentagePayback'),
               ('ratingCampaignsRatedSpots'),
               ('reservedRatingsADS'),
               ('spotCampaignsRatedSpots'),
               ('totalNominalValue')");

            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateID,KPIname,PriorityId)
                   Select b.id,t.kpi,1
                   from t_values t
                   join BRSConfigurationTemplates b");

            migrationBuilder.Sql(@"UPDATE BRSConfigurationTemplates
                                SET LastModified = NOW()");



            migrationBuilder.Sql(@"
                UPDATE BRSConfigurationTemplates
	            SET LastModified = UTC_TIMESTAMP()");

            migrationBuilder.Sql($@"
                UPDATE KPIComparisonConfigs
                SET HigherIsBest = 1
                WHERE KPIName LIKE '{AvailableRatingsByDemo}%'");

            migrationBuilder.Sql(@" CREATE Temporary TABLE IF Not Exists t_TempRulesMapping
                (
                    RuleTypeId int,
                    OldRuleId int,
                    NewRuleId int,
                    NewCampaignType int
                );");

            migrationBuilder.Sql(@" INSERT INTO t_TempRulesMapping
                    (RuleTypeId, OldRuleId, NewRuleId, NewCampaignType)
                VALUES
                    (3, 1, 19, 1),
                    (3, 2, 20, 1),
                    (3, 3, 21, 1),
                    (3, 4, 22, 1),
                    (3, 5, 23, 1),
                    (3, 6, 24, 1),
                    (3, 7, 25, 1),
                    (3, 8, 26, 1),
                    (3, 9, 27, 1),
                    (3, 10, 28, 1),
                    (3, 11, 29, 1),
                    (3, 12, 30, 1),
                    (3, 13, 31, 1),
                    (3, 14, 32, 1),
                    (3, 37, 38, 1),
                    (4, 1, 28, 0),
                    (4, 2, 29, 0),
                    (4, 3, 30, 0),
                    (4, 4, 31, 0),
                    (4, 5, 32, 0),
                    (4, 6, 33, 0),
                    (4, 7, 34, 0),
                    (4, 17, 44, 0),
                    (4, 20, 47, 0),
                    (4, 23, 50, 0),
                    (4, 24, 51, 0),
                    (4, 25, 52, 0),
                    (4, 26, 53, 0);");

            migrationBuilder.Sql(@"   UPDATE Rules
                SET CampaignType = CASE
                                       WHEN RuleTypeId = 3 THEN 0
                                       WHEN RuleTypeId = 4 THEN 1
                                       ELSE NULL
                                   END;");

            migrationBuilder.Sql(@"INSERT INTO Rules (RuleId, RuleTypeId, InternalType, Description, Type, CampaignType)
                SELECT
                    map.NewRuleId, r.RuleTypeId, r.InternalType, r.Description, r.Type, map.NewCampaignType
                FROM
                    Rules r
                INNER JOIN
                    t_TempRulesMapping map ON (map.RuleTypeId = r.RuleTypeId AND map.OldRuleId = r.RuleId)
                WHERE
                    r.RuleTypeId IN (3, 4);");

            migrationBuilder.Sql(@" INSERT INTO AutopilotRules (RuleId, RuleTypeId, FlexibilityLevelId, Enabled, LoosenBit, LoosenLot, TightenBit, TightenLot)
                SELECT
                    map.NewRuleId, ar.RuleTypeId, ar.FlexibilityLevelId, ar.Enabled, ar.LoosenBit, ar.LoosenLot, ar.TightenBit, ar.TightenLot
                FROM
                    AutopilotRules ar
                INNER JOIN
                    t_TempRulesMapping map ON (map.RuleTypeId = ar.RuleTypeId AND map.OldRuleId = ar.RuleId)
                WHERE
                    ar.RuleTypeId IN (3, 4);");


            migrationBuilder.Sql(" DROP TABLE t_TempRulesMapping;");


            migrationBuilder.Sql(@"INSERT INTO Failures(ScenarioID)
                 VALUES('B180E716-E2BC-424C-9E0F-158D212C5C6D'),
                 ('350A48C4-8145-4BE8-8058-188E04260A58'),
                 ('186905CD-FA6A-4C47-A8D1-292EE134E70E'),
                 ('08740FB8-47AB-48A7-B2F0-2F27DEAF1555'),
                 ('A2440DE8-B035-4081-9673-346FB09FCD2C'),
                 ('691A0DBC-AA6B-451D-9ACB-4D19FC649D7C');");


            migrationBuilder.Sql(@"CREATE TEMPORARY table IF NOT EXISTS Failures_temp aS(SELECT* FROM Failures);");

            migrationBuilder.Sql(@"DELETE FROM Failures;");


            migrationBuilder.Sql(@"INSERT INTO Failures(ScenarioId)
                     SELECT DISTINCT ScenarioId FROM Failures_temp;");

            migrationBuilder.Sql(@"DROP table IF EXISTS Failures_temp;");


            migrationBuilder.Sql(@"INSERT Runs(Id, CustomId, Description, CreatedDateTime, StartDate, StartTime, EndDate, EndTime,
                 LastModifiedDateTime, ExecuteStartedDateTime, IsLocked, `Real`, Smooth, SmoothDateStart, SmoothDateEnd,
                 ISR, ISRDateStart, ISRDateEnd, Optimisation, OptimisationDateStart, OptimisationDateEnd, RightSizer,
                 RightSizerDateStart, RightSizerDateEnd, SpreadProgramming, Objectives, EfficiencyPeriod, NumberOfWeeks,

                 FailureTypes, Manual, RunStatus, BookTargetArea, IgnoreZeroPercentageSplit, BRSConfigurationTemplateId,
                 ExcludeBankHolidays, ExcludeSchoolHolidays, IgnorePremiumCategoryBreaks, SkipLockedBreaks, RunTypeId, positioninprogramme, CreatedOrExecuteDateTime)
                 VALUES(N'b0e759d1-e7ca-11ea-92e2-0611c003d7e6', 21, N'1', N'2020-08-03 12:55:53.5013528', N'2020-08-04 00:00:00.0000000',
                 216000000000, N'2020-08-10 00:00:00.0000000', 215990000000, N'2020-08-03 12:55:53.5013528', N'2020-08-03 12:56:12.5640533',
                  0, 1, 0, N'2020-08-04T00:00:00.0000000', N'2020-08-04T00:00:00.0000000', 1, N'2020-08-04T00:00:00.0000000',
                  N'2020-08-10 00:00:00.0000000', 1, N'2020-08-04 00:00:00.0000000', N'2020-08-10 00:00:00.0000000', 1,
                  N'2020-08-04 00:00:00.0000000', N'2020-08-10 00:00:00.0000000', 1, N'', 0, 1,
                 N'25‖52‖5‖42‖66‖51‖41‖6‖78‖40‖79‖8‖39‖43‖65‖4‖71‖69‖58‖1‖47‖60‖57‖44‖61‖62‖2‖54‖82‖63‖3‖73‖53‖64‖50‖80‖9‖68‖16‖45‖20‖23‖28‖19‖30‖55‖56‖24‖72‖67‖26‖70‖27‖22‖29‖31‖49‖37‖11‖36‖10‖35‖12‖34‖13‖33‖14‖48‖32‖21‖15‖38‖59',
                  1, 3, 1, 1, 0, 1, 1, 1, 1, 0, 1, utc_date());");


            migrationBuilder.Sql(@"INSERT Runs(Id, CustomId, Description, CreatedDateTime, StartDate, StartTime, EndDate, EndTime, LastModifiedDateTime,
                 ExecuteStartedDateTime, IsLocked, `Real`, `Smooth`, `SmoothDateStart`, `SmoothDateEnd`, ISR, ISRDateStart, ISRDateEnd, Optimisation,
                 OptimisationDateStart, OptimisationDateEnd, RightSizer, RightSizerDateStart, RightSizerDateEnd, SpreadProgramming, Objectives,
                 EfficiencyPeriod, NumberOfWeeks, FailureTypes, Manual, RunStatus, BookTargetArea, IgnoreZeroPercentageSplit,
                 BRSConfigurationTemplateId, ExcludeBankHolidays, ExcludeSchoolHolidays, IgnorePremiumCategoryBreaks, SkipLockedBreaks,
                 RunTypeId, PositionInProgramme, CreatedOrExecuteDateTime)
                 VALUES(N'ecc15bbe-f454-455f-b2ad-738e1bab4c0d', 9, N'XGGT-14904', N'2020-07-22 19:31:50.5969358', N'2020-07-24 00:00:00.000'
                 , 216000000000, N'2020-07-31 00:00:00.0000000', 215990000000, N'2020-07-22 19:31:50.5969358', N'2020-07-22 19:32:06.9719881'
                 , 0, 1, 0, N'2020-07-23 00:00:00.0000000', N'2020-07-23 00:00:00.0000000', 1, N'2020-07-24 00:00:00.0000000',
                 N'2020-07-31 00:00:00.0000000', 1, N'2020-07-24 00:00:00.0000000', N'2020-07-31 00:00:00.0000000', 1,
                 N'2020-07-24 00:00:00.0000000', N'2020-07-31 00:00:00.0000000', 0, N'', 0, 1,
                 N'25‖52‖5‖42‖66‖51‖41‖6‖78‖40‖79‖8‖39‖43‖65‖4‖71‖69‖58‖1‖47‖60‖57‖44‖61‖62‖2‖54‖82‖63‖3‖73‖53‖64‖50‖80‖9‖68‖16‖45‖20‖23‖28‖19‖30‖55‖56‖24‖72‖67‖26‖70‖27‖22‖29‖31‖49‖37‖11‖36‖10‖35‖12‖34‖13‖33‖14‖48‖32‖21‖15‖38‖59',
                 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, utc_date());");

            migrationBuilder.Sql(@"INSERT Runs(Id, CustomId, Description, CreatedDateTime, StartDate, StartTime, EndDate, EndTime,
                 LastModifiedDateTime, ExecuteStartedDateTime, IsLocked, `Real`, Smooth, SmoothDateStart, SmoothDateEnd,
                 ISR, ISRDateStart, ISRDateEnd, Optimisation, OptimisationDateStart, OptimisationDateEnd, RightSizer,
                 RightSizerDateStart, RightSizerDateEnd, SpreadProgramming, Objectives, EfficiencyPeriod, NumberOfWeeks,
                 FailureTypes, Manual, RunStatus, BookTargetArea, IgnoreZeroPercentageSplit, BRSConfigurationTemplateId,
                 ExcludeBankHolidays, ExcludeSchoolHolidays, IgnorePremiumCategoryBreaks, SkipLockedBreaks, RunTypeId, positioninprogramme, CreatedOrExecuteDateTime)
                 VALUES(N'c656f8d9-3679-463b-a8d9-8dece32e6553', 21, N'1', N'2020-08-03 12:55:53.5013528', N'2020-08-04 00:00:00.0000000',
                 216000000000, N'2020-08-10 00:00:00.0000000', 215990000000, N'2020-08-03 12:55:53.5013528', N'2020-08-03 12:56:12.5640533',
                  0, 1, 0, N'2020-08-04T00:00:00.0000000', N'2020-08-04T00:00:00.0000000', 1, N'2020-08-04T00:00:00.0000000',
                  N'2020-08-10 00:00:00.0000000', 1, N'2020-08-04 00:00:00.0000000', N'2020-08-10 00:00:00.0000000', 1,
                  N'2020-08-04 00:00:00.0000000', N'2020-08-10 00:00:00.0000000', 1, N'', 0, 1,
                 N'25‖52‖5‖42‖66‖51‖41‖6‖78‖40‖79‖8‖39‖43‖65‖4‖71‖69‖58‖1‖47‖60‖57‖44‖61‖62‖2‖54‖82‖63‖3‖73‖53‖64‖50‖80‖9‖68‖16‖45‖20‖23‖28‖19‖30‖55‖56‖24‖72‖67‖26‖70‖27‖22‖29‖31‖49‖37‖11‖36‖10‖35‖12‖34‖13‖33‖14‖48‖32‖21‖15‖38‖59',
                  1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, utc_date());");


            migrationBuilder.Sql(@"INSERT INTO Scenarios
              (Id, TokenizedName, Name, CustomId, DateCreated, DateModified, DateUserModified, IsAutopilot, IsLibraried)
                 VALUES('928b44b7-e7c5-11ea-92e2-0611c003d7e6', 'tn', 'Test1',1 ,utc_date() ,utc_date() ,utc_date() ,1 ,1 );");



            migrationBuilder.Sql(@"INSERT INTO RunScenarios
                     (RunId, ScenarioId, StartedDateTime, CompletedDateTime, Progress, Status, DateCreated, DateModified, `Order`)
                 VALUES('b0e759d1-e7ca-11ea-92e2-0611c003d7e6', '928b44b7-e7c5-11ea-92e2-0611c003d7e6', utc_date(), utc_date(), 'progress', 2, utc_date(), utc_date, 1);");


            migrationBuilder.Sql(@"INSERT INTO RunScenarios
              ( RunId, ScenarioId, StartedDateTime, CompletedDateTime, Progress,  Status, DateCreated, DateModified, `Order`)
                 VALUES( 'c656f8d9-3679-463b-a8d9-8dece32e6553', '928b44b7-e7c5-11ea-92e2-0611c003d7e6',utc_date() ,utc_date() , 'progress', 2 ,utc_date() ,utc_date ,1 );");


            migrationBuilder.Sql(@"INSERT INTO Passes
                  (id,Name, TokenizedName, DateCreated, DateModified, IsLibraried)
                 VALUES(1,'New Pass', '1 New Pass', utc_date(), utc_date, 1);");


            migrationBuilder.Sql(@"INSERT INTO Passes
              ( id,Name, TokenizedName, DateCreated, DateModified, IsLibraried)
                 VALUES(2, 'Untitled Pass', '2 Untitled Pass',utc_date() ,utc_date ,1 );");


            migrationBuilder.Sql(@"INSERT INTO Passes
                 (id,Name, TokenizedName, DateCreated, DateModified, IsLibraried)
                 VALUES(3,'Default Pass', '2 Default Pass', utc_date(), utc_date, 0);");

            migrationBuilder.Sql(@"INSERT INTO ScenarioPassReferences
              ( PassId, ScenarioId, `Order`)
                 VALUES(1 , '928b44b7-e7c5-11ea-92e2-0611c003d7e6',1 );");


            migrationBuilder.Sql(@"INSERT INTO PassRules
                 (PassId, RuleId, InternalType, Description, Value, Type, Discriminator, `Ignore`, PeakValue, CampaignType, Under, `Over`, ForceOverUnder, BookTargetArea)
                 VALUES(1, 1, 'Defaults', 'Minimum Efficiency', '0', 'general', 1, 0, '', 0, 0, 10, 0, 0);");


            migrationBuilder.Sql(@"CREATE TEMPORARY table IF NOT EXISTS StartedRuns_PassRules
                  (
                      RunId varchar(36),
                      PassRuleId int
                  );");


            migrationBuilder.Sql($@"INSERT into StartedRuns_PassRules(RunId, PassRuleId)
              select Runs.Id, PassRules.Id
              from Runs
              join RunScenarios
                  ON RunScenarios.RunId = Runs.Id
                      AND Runs.RunStatus != { (int)RunStatus.NotStarted}
                          join ScenarioPassReferences
                  ON ScenarioPassReferences.ScenarioId = RunScenarios.ScenarioId
              join PassRules
                  ON PassRules.PassId = ScenarioPassReferences.PassId;");



            migrationBuilder.Sql($@"update `PassRules`
                  set `Ignore` = '{True}'
                  where `Id` NOT IN (select `PassRuleId` from StartedRuns_PassRules)
                   aND `RuleId` IN ({MaxRatingsForRatingCampaignsRuleId}, {MaxRatingsForSpotCampaignsRuleId})");



            migrationBuilder.Sql($"UPDATE PassRules SET BookTargetarea = 0 WHERE BookTargetarea IS NULL AND Discriminator = {(int)PassRuleType.Tolerance};");

            migrationBuilder.Sql($"UPDATE PassRules SET CampaignType = {(int)CampaignDeliveryType.Rating} WHERE CampaignType IS NULL AND Discriminator IN ({(int)PassRuleType.Rule}, {(int)PassRuleType.Tolerance});");


            migrationBuilder.Sql($@"
                         INSERT INTO FaultTypes VALUES (1),
                         (2), (3), (4), (5),(6),(8),(9),(10),(11),(12),(13),(14),(15),(16),(19),(20),
                         (21),(22), (23),(24), (25),(26),(27),(28),(29),(30),(31),(32),(33),(34),(35),
                         (36),(37),(38),(39),(40),(41),(42),(43),(44),(45),(47),(48),(49),(50),(51),
                         (52),(53),(54),(55),(56),(57),(58),(59),(60),(61),(62),(63),(64),(65),(66),
                         (67),(68),(69),(70),(71),(72),(73),(78),(79),(80),(81),(82);");

            migrationBuilder.Sql(@"INSERT INTO FaultTypeDescriptions (Id, LanguageAbbreviation, Description, FaultTypeId, ShortName) VALUES
              (1, N'ENG', N'Programme Requirement (Achieved/Oversupply)', 78, N'Prgm Req (Ach/Ovrsp)'),
              (2, N'ARA', N'Not Enough Availability in Break', 10, N'Not Enough Avail in Brk'),
              (3, N'ENG', N'Clash detected', 14, N'Clash det'),
              (4, N'ARA', N'Clash detected', 14, N'Clash det'),
              (5, N'ENG', N'Restrictions', 15, N'Restrictions'),
              (6, N'ARA', N'Restrictions', 15, N'Restrictions'),
              (7, N'ENG', N'Clash with self detected', 16, N'Clash w/self det'),
              (8, N'ARA', N'Clash with self detected', 16, N'Clash w/self det'),
              (9, N'ENG', N'Sponsorship Clash detected', 45, N'Spnsp Clash det'),
              (10, N'ENG', N'Not Enough Availability in Break', 10, N'Not Enough Avail in Brk'),
              (11, N'ARA', N'Sponsorship Clash detected', 45, N'Spnsp Clash det'),
              (12, N'ARA', N'Existing Spot', 55, N'Existing Spot'),
              (13, N'ENG', N'No Available Spot Types', 56, N'No avlbl Spot Tps'),
              (14, N'ARA', N'No Available Spot Types', 56, N'No avlbl Spot Tps'),
              (15, N'ENG', N'No Available Schedule Payments', 67, N'No avlbl Sched Pmts'),
              (16, N'ARA', N'No Available Schedule Payments', 67, N'No avlbl Sched Pmts'),
              (17, N'ENG', N'Floor Rate', 68, N'Floor Rate'),
              (18, N'ARA', N'Floor Rate', 68, N'Floor Rate'),
              (19, N'ENG', N'Break Demographic Restrictions', 70, N'Brk Demo Restr'),
              (20, N'ENG', N'Existing Spot', 55, N'Existing Spot'),
              (21, N'ARA', N'Break Demographic Restrictions', 70, N'Brk Demo Restr'),
              (22, N'ARA', N'Sponsorship Restriction applied for Competitor Spot based on Competing <Advertiser>', 80, N'Sponsorship Restr…<Advertiser>'),
              (23, N'ARA', N'Sponsorship Restriction applied for Competitor Spot based on Competing <Clash>', 79, N'Sponsorship Restr…<Clash>'),
              (24, N'ARA', N'Campaign Ratecard Restriction', 59, N'Camp Rtcd Restr'),
              (25, N'ENG', N'Bonus Booking Requirement', 60, N'Bonus Book Req'),
              (26, N'ARA', N'Bonus Booking Requirement', 60, N'Bonus Book Req'),
              (27, N'ENG', N'Strike Weight/Daypart Length Requirement (Achieved/Oversupply)', 61, N'Strk Wght/Dprt Lght Req (Ach/Ovrsp)'),
              (28, N'ARA', N'Strike Weight/Daypart Length Requirement (Achieved/Oversupply)', 61, N'Strk Wght/Dprt Lght Req (Ach/Ovrsp)'),
              (29, N'ENG', N'Daypart Length Requirement (Achieved/Oversupply)', 62, N'Dprt Len Req (Ach/Ovrsp)'),
              (30, N'ARA', N'Daypart Length Requirement (Achieved/Oversupply)', 62, N'Dprt Len Req (Ach/Ovrsp)'),
              (31, N'ENG', N'Strike Weight Length Requirement (Achieved/Oversupply)', 63, N'Strk Wght Len Req (Ach/Ovrsp)'),
              (32, N'ENG', N'Sponsorship Restriction applied for Competitor Spot based on Competing <Advertiser>', 80, N'Sponsorship Restr…<Advertiser>'),
              (33, N'ARA', N'Strike Weight Length Requirement (Achieved/Oversupply)', 63, N'Strk Wght Len Req (Ach/Ovrsp)'),
              (34, N'ARA', N'Strike Weight/Daypart Length Requirement (Target Achieved/Oversupply)', 64, N'Strk Wght/Dprt. Len Req (Tgt Ach/Ovrsp)'),
              (35, N'ENG', N'Daypart Length Requirement (Target Achieved/Oversupply)', 65, N'Dprt Len Req (Tgt. Ach/Ovrsp)'),
              (36, N'ARA', N'Daypart Length Requirement (Target Achieved/Oversupply)', 65, N'Dprt Len Req (Tgt. Ach/Ovrsp)'),
              (37, N'ENG', N'Strike Weight Length Requirement (Target Achieved/Oversupply)', 66, N'Strk Wght Len Req (Tgt Ach/Ovrsp)'),
              (38, N'ARA', N'Strike Weight Length Requirement (Target Achieved/Oversupply)', 66, N'Strk Wght Len Req (Tgt Ach/Ovrsp)'),
              (39, N'ENG', N'Macro Booking', 69, N'Macro Book'),
              (40, N'ARA', N'Programme Requirement (Achieved/Oversupply)', 78, N'Prgm Req (Ach/Ovrsp)'),
              (41, N'ENG', N'Sponsorship Restriction applied for Competitor Spot based on Competing <Clash>', 79, N'Sponsorship Restr…<Clash>'),
              (42, N'ENG', N'Strike Weight/Daypart Length Requirement (Target Achieved/Oversupply)', 64, N'Strk Wght/Dprt. Len Req (Tgt Ach/Ovrsp)'),
              (43, N'ENG', N'Spot Length Booking Rules', 72, N'Spot Len Book Rules'),
              (44, N'ARA', N'Spot Length Booking Rules', 72, N'Spot Len Book Rules'),
              (45, N'ENG', N'Max Zero Ratings', 19, N'Max 0 Rtgs'),
              (46, N'ENG', N'Min Days Between Programme/Time', 40, N'Min Days Btw Prgm/Time'),
              (47, N'ARA', N'Min Days Between Programme/Time', 40, N'Min Days Btw Prgm/Time'),
              (48, N'ENG', N'Min Weeks Between Programme/Time', 41, N'Min Wks Btw Prgm/Time'),
              (49, N'ARA', N'Min Weeks Between Programme/Time', 41, N'Min Wks Btw Prgm/Time'),
              (50, N'ENG', N'Min Hours Between Spots', 42, N'Min Hrs Btw Spots'),
              (51, N'ARA', N'Min Hours Between Spots', 42, N'Min Hrs Btw Spots'),
              (52, N'ENG', N'Miniumum Break Availability', 43, N'Min Brk Avail'),
              (53, N'ARA', N'Miniumum Break Availability', 43, N'Min Brk Avail'),
              (54, N'ARA', N'Max Spots Per Programme/Time', 39, N'Max Spots/Prgm/Time'),
              (55, N'ENG', N'Zero Rated Breaks', 71, N'Zero Rate Brks'),
              (56, N'ENG', N'Max Ratings for Rating Campaigns', 73, N'Max Rtgs for Rtg Camp'),
              (57, N'ARA', N'Max Ratings for Rating Campaigns', 73, N'Max Rtgs for Rtg Camp'),
              (58, N'ENG', N'Min Rating Points not met', 82, N'Min TARP`s not met'),
              (59, N'ARA', N'Min Rating Points not met', 82, N'Min TARP`s not met'),
              (60, N'ENG', N'Split Bookings not allowed', 44, N'Split Book Not Alwd'),
              (61, N'ARA', N'Split Bookings not allowed', 44, N'Split Book Not Alwd'),
              (62, N'ENG', N'Regional Break booking', 47, N'Rgnl Brk book'),
              (63, N'ARA', N'Regional Break booking', 47, N'Rgnl Brk book'),
              (64, N'ARA', N'Zero Rated Breaks', 71, N'Zero Rate Brks'),
              (65, N'ENG', N'Max Spots Per Programme/Time', 39, N'Max Spots/Prgm/Time'),
              (66, N'ARA', N'Max Ratings for Spot Campaigns', 38, N'Max Rtg for Spot Camp'),
              (67, N'ENG', N'Max Ratings for Spot Campaigns', 38, N'Max Rtg for Spot Camp'),
              (68, N'ARA', N'Max Zero Ratings', 19, N'Max 0 Rtgs'),
              (69, N'ENG', N'Efficiency/Rank Limit', 20, N'Effcy/Rank Lim'),
              (70, N'ARA', N'Efficiency/Rank Limit', 20, N'Effcy/Rank Lim'),
              (71, N'ENG', N'Max Spots Per Day', 30, N'Max Spots/Day'),
              (72, N'ARA', N'Max Spots Per Day', 30, N'Max Spots/Day'),
              (73, N'ENG', N'Max Spots Per Hour', 31, N'Max Spots/Hr'),
              (74, N'ARA', N'Max Spots Per Hour', 31, N'Max Spots/Hr'),
              (75, N'ENG', N'Max Spots Per 2 Hours', 32, N'Max Spots/2 Hrs'),
              (76, N'ARA', N'Max Spots Per 2 Hours', 32, N'Max Spots/2 Hrs'),
              (77, N'ENG', N'Min Breaks Between Spots', 33, N'Min Brk Btw Spots'),
              (78, N'ARA', N'Min Breaks Between Spots', 33, N'Min Brk Btw Spots'),
              (79, N'ENG', N'Max Spots Per Programme/Day', 34, N'Max Spots/Prgm/Day'),
              (80, N'ARA', N'Max Spots Per Programme/Day', 34, N'Max Spots/Prgm/Day'),
              (81, N'ENG', N'Max Spots Per Programme/Week', 35, N'Max Spots/Prgm/Wk'),
              (82, N'ARA', N'Max Spots Per Programme/Week', 35, N'Max Spots/Prgm/Wk'),
              (83, N'ENG', N'Max Spots per Programme/100 ratings', 36, N'Max Spots/Prgm/100 rtg'),
              (84, N'ARA', N'Max Spots per Programme/100 ratings', 36, N'Max Spots/Prgm/100 rtg'),
              (85, N'ENG', N'Min Weeks Between Programmes', 37, N'Min Wks Btw Prgm'),
              (86, N'ARA', N'Min Weeks Between Programmes', 37, N'Min Wks Btw Prgm'),
              (87, N'ENG', N'Campaign Ratecard Restriction', 59, N'Camp Rtcd Restr'),
              (88, N'ARA', N'Sales Area Revenue Requirement (Achieved/Oversupply)', 58, N'SA Rev Req (Ach/Oversp)'),
              (89, N'ARA', N'Macro Booking', 69, N'Macro Book'),
              (90, N'ARA', N'Centre/End Requirement (Achieved/Oversupply)', 23, N'Cntr/End Req (Ach/Ovrsp)'),
              (91, N'ENG', N'Outside Campaign Daypart', 9, N'Out Camp Dprt'),
              (92, N'ARA', N'Outside Campaign Daypart', 9, N'Out Camp Dprt'),
              (93, N'ENG', N'Centre/End Requirement', 11, N'Cntr/End Req'),
              (94, N'ARA', N'Centre/End Requirement', 11, N'Cntr/End Req'),
              (95, N'ARA', N'Exclude Campaign Packages', 48, N'Excl Camp Pkgs'),
              (96, N'ENG', N'Exclude Campaign Packages', 48, N'Excl Camp Pkgs'),
              (97, N'ENG', N'Time not in Requirement', 12, N'Time not in Req'),
              (98, N'ARA', N'Time not in Requirement', 12, N'Time not in Req'),
              (99, N'ENG', N'Break Type', 13, N'Brk Type'),
              (100, N'ARA', N'Break Type', 13, N'Brk Type'),
              (101, N'ARA', N'Booking Position Requirement (Achieved/Oversupply)', 29, N'Book Pos Req (Ach/Ovrsp)'),
              (102, N'ENG', N'Campaign Requirement (Achieved/Oversupply)', 21, N'Camp Req (Ach/Ovrsp)'),
              (103, N'ENG', N'Sales Area Requirement (Target Achieved/Oversupply)', 49, N'SA Req (Tgt Ach/Ovrsp)'),
              (104, N'ENG', N'Booking Position Requirement (Achieved/Oversupply)', 29, N'Book Pos Req (Ach/Ovrsp)'),
              (105, N'ENG', N'Sales Area Requirement (Achieved/Oversupply)', 22, N'SA Req (Ach/Ovrsp)'),
              (106, N'ARA', N'Spot Length Requirement (Achieved/Oversupply)', 28, N'Spot Len Req (Ach/Ovrsp)'),
              (107, N'ENG', N'Spot Length Requirement (Achieved/Oversupply)', 28, N'Spot Len Req (Ach/Ovrsp)'),
              (108, N'ARA', N'Sales Area Requirement (Achieved/Oversupply)', 22, N'SA Req (Ach/Ovrsp)'),
              (109, N'ENG', N'Centre/End Requirement (Achieved/Oversupply)', 23, N'Cntr/End Req (Ach/Ovrsp)'),
              (110, N'ARA', N'Daypart Requirement (Achieved/Oversupply)', 27, N'Dprt Req (Ach/Ovrsp)'),
              (111, N'ENG', N'Daypart Requirement (Achieved/Oversupply)', 27, N'Dprt Req (Ach/Ovrsp)'),
              (112, N'ENG', N'Sales Area Revenue Requirement (Achieved/Oversupply)', 58, N'SA Rev Req (Ach/Oversp)'),
              (113, N'ARA', N'Strike Weight/Daypart Requirement (Achieved/Oversupply)', 26, N'Strk Wght/Dprt Req (Ach/Ovrsp)'),
              (114, N'ENG', N'Strike Weight/Daypart Requirement (Achieved/Oversupply)', 26, N'Strk Wght/Dprt Req (Ach/Ovrsp)'),
              (115, N'ENG', N'Strike Weight Requirement (Achieved/Oversupply)', 24, N'Strk Wght Req (Ach/Ovrsp)'),
              (116, N'ARA', N'Strike Weight Requirement (Achieved/Oversupply)', 24, N'Strk Wght Req (Ach/Ovrsp)'),
              (117, N'ARA', N'Campaign Requirement (Achieved/Oversupply)', 21, N'Camp Req (Ach/Ovrsp)'),
              (118, N'ARA', N'Strike Weight Length Requirement (Achieved/Oversupply)', 25, N'Strk Wght Len Req (Ach/Ovrsp)'),
              (119, N'ARA', N'Zero requirement for Length', 8, N'0 req for Lgth'),
              (120, N'ENG', N'Zero requirement for Length', 8, N'0 req for Lgth'),
              (121, N'ARA', N'Campaign Revenue Requirement (Achieved/Oversupply)', 57, N'Camp Rev Req (Ach/Ovrsp)'),
              (122, N'ENG', N'Outside Campaign Strike Weights', 1, N'Out Camp Strk Wght'),
              (123, N'ENG', N'Campaign Revenue Requirement (Achieved/Oversupply)', 57, N'Camp Rev Req (Ach/Ovrsp)'),
              (124, N'ARA', N'Outside Campaign Strike Weights', 1, N'Out Camp Strk Wght'),
              (125, N'ARA', N'Spot Length Requirement (Target Achieved/Oversupply)', 54, N'Spot Len Req (Tgt Ach/Ovrsp)'),
              (126, N'ENG', N'Spot Length Requirement (Target Achieved/Oversupply)', 54, N'Spot Len Req (Tgt Ach/Ovrsp)'),
              (127, N'ENG', N'Programme not in Requirement', 2, N'Prgm not in Req'),
              (128, N'ARA', N'Daypart Requirement (Target Achieved/Oversupply)', 53, N'Dprt Req (Tgt Ach/Ovrsp)'),
              (129, N'ARA', N'Programme not in Requirement', 2, N'Prgm not in Req'),
              (130, N'ARA', N'Sales Area Requirement (Target Achieved/Oversupply)', 49, N'SA Req (Tgt Ach/Ovrsp)'),
              (131, N'ENG', N'Programme Catery not in Requirement', 3, N'Prgm Cat not in Req'),
              (132, N'ARA', N'Programme Catery not in Requirement', 3, N'Prgm Cat not in Req'),
              (133, N'ENG', N'Daypart Requirement (Target Achieved/Oversupply)', 53, N'Dprt Req (Tgt Ach/Ovrsp)'),
              (134, N'ENG', N'Strike Weight/Daypart Requirement (Target Achieved/Oversupply)', 52, N'Strk Wght/Dprt Req (Ach/Ovrsp)'),
              (135, N'ARA', N'Position Requirement not available', 6, N'Pos Req not avlbl'),
              (136, N'ENG', N'Strike Weight Requirement (Target Achieved/Oversupply)', 50, N'Strk Wght Req (Tgt Ach/Ovrsp)'),
              (137, N'ARA', N'Strike Weight Requirement (Target Achieved/Oversupply)', 50, N'Strk Wght Req (Tgt Ach/Ovrsp)'),
              (138, N'ENG', N'Position Requirement not available', 6, N'Pos Req not avlbl'),
              (139, N'ARA', N'Strike Weight/Daypart Requirement (Target Achieved/Oversupply)', 52, N'Strk Wght/Dprt Req (Ach/Ovrsp)'),
              (140, N'ARA', N'Spot Length Invalid for Period', 5, N'Spot Lgth Inval for Prd'),
              (141, N'ENG', N'Strike Weight Length Requirement (Achieved/Oversupply)', 25, N'Strk Wght Len Req (Ach/Ovrsp)'),
              (142, N'ENG', N'Spot Length Invalid for Period', 5, N'Spot Lgth Inval for Prd'),
              (143, N'ARA', N'Strike Weight Length Requirement (Target Achieved/Oversupply)', 51, N'Strk Wght Len Req (Tgt Ach/Ovrsp)'),
              (144, N'ARA', N'Programme Required Inclusion Mismatch', 4, N'Prgm Req Incl Mismatch'),
              (145, N'ENG', N'Programme Required Inclusion Mismatch', 4, N'Prgm Req Incl Mismatch'),
              (146, N'ENG', N'Strike Weight Length Requirement (Target Achieved/Oversupply)', 51, N'Strk Wght Len Req (Tgt Ach/Ovrsp)');");

            migrationBuilder.Sql($@"
                      insert into FaultTypeDescriptions(
                          {nameof(FaultTypeDescription.FaultTypeId)},
                          {nameof(FaultTypeDescription.LanguageAbbreviation)},
                          {nameof(FaultTypeDescription.Description)})
                      values	({TarpsFaultTypeId}, '{EngLanguageabbreviation}', '{TarpsDescription}'),
                        ({TarpsFaultTypeId}, '{araLanguageabbreviation}', '{TarpsDescription}');");

            migrationBuilder.Sql($@"
                          INSERT INTO FunctionalAreas(ID)VALUES('{SlottingControlsFunctionalareaId}');");

            migrationBuilder.Sql($@"	   
                          insert into FunctionalAreaFaultTypes(
                              {nameof(FunctionalAreaFaultType.FunctionalAreaId)},
                              {nameof(FunctionalAreaFaultType.FaultTypeId)},
                              {nameof(FunctionalAreaFaultType.IsSelected)})
                          values	('{SlottingControlsFunctionalareaId}', {TarpsFaultTypeId}, 1)");

            migrationBuilder.Sql(@"DELETE FROM KPIPriorities;");

            migrationBuilder.Sql(@"INSERT INTO KPIPriorities (Id, Name, WeightingFactor) VaLUES (1, 'Exclude',0);");
            migrationBuilder.Sql(@"INSERT INTO KPIPriorities (Id, Name, WeightingFactor) VaLUES (2, 'Extremely Low',0.3);");
            migrationBuilder.Sql(@"INSERT INTO KPIPriorities (Id, Name, WeightingFactor) VaLUES (3, 'Low',0.7);");
            migrationBuilder.Sql(@"INSERT INTO KPIPriorities (Id, Name, WeightingFactor) VaLUES (4, 'Medium',1);");
            migrationBuilder.Sql(@"INSERT INTO KPIPriorities (Id, Name, WeightingFactor) VaLUES (5, 'High',1.3);");
            migrationBuilder.Sql(@"INSERT INTO KPIPriorities (Id, Name, WeightingFactor) VaLUES (6, 'Extremely High',1.7);");

            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationTemplates(Id, Name, LastModified, IsDefault) VALUES(1, 'Default template', now(), 1);");

            migrationBuilder.Sql(@"DELETE FROM BRSConfigurationForKPIs;");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'percent95to105', 4);");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'percentbelow75', 4);");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'averageEfficiency', 4);");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'totalSpotsBooked', 4);");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'remainaudience', 4);");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'remainingavailability', 4);");
            migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'standardaverageCompletion', 4);");
            //migrationBuilder.Sql(@"INSERT INTO BRSConfigurationForKPIs(BRSConfigurationTemplateId, KPIname, PriorityId) VaLUES(1, 'weightedaverageCompletion', 4);");


            /*migrationBuilder.Sql(@"
                   UPDATE ProgrammeDictionaries pd
                   inner join  Programmes p         
                   on pd.ExternalReference = p.ExternalReference        
                   set pd.Name = p.Name
                   WHERE pd.Name is NULL");*/


            migrationBuilder.Sql($@"DELETE FROM OutputFiles;");

            migrationBuilder.Sql($@"insert into OutputFiles(
                      {nameof(OutputFile.FileId)},
                      {nameof(OutputFile.Description)},
                      {nameof(OutputFile.AutoBookFileName)})
                  values ('{FileId}', '{Description}', '{autoBookFileName}'); ");


            migrationBuilder.Sql(StartedRunsPassRulesQuery);
            migrationBuilder.Sql(StartedRunsPassRulesQuery2);


            migrationBuilder.Sql(@"INSERT INTO Rules
                  (RuleId, RuleTypeId, InternalType, Description, Type,CampaignType)
                  VALUES(1, 1, 'Defaults', 'Minimum Efficiency','general',1);");


            migrationBuilder.Sql($@"UPDATE Rules
                  SET Description = '{MaxRatingsForSpotCampaignsDescriptionNew}'
                  WHERE RuleId = {MaxRatingsForSpotCampaignsRuleId};");

            migrationBuilder.Sql($@"UPDATE Rules
                  SET Description = '{MaxRatingsForRatingCampaignsDescriptionNew}'
                  WHERE RuleId = {MaxRatingsForRatingCampaignsRuleId};");

            migrationBuilder.Sql($@"UPDATE PassRules
                  SET Description = '{MaxRatingsForSpotCampaignsDescriptionNew}'
                  WHERE NOT EXISTS (SELECT passruleid FROM StartedRuns_PassRules WHERE PassRuleId = Id)
                      aND RuleId = {MaxRatingsForSpotCampaignsRuleId};");


            migrationBuilder.Sql($@"UPDATE PassRules
                  SET Description = '{MaxRatingsForRatingCampaignsDescriptionNew}'
                  WHERE NOT EXISTS (SELECT PassRuleId FROM StartedRuns_PassRules WHERE PassRuleId = Id)
                      AND RuleId = {MaxRatingsForRatingCampaignsRuleId};");

            migrationBuilder.Sql($@"insert into KPIComparisonConfigs(
                      { nameof(KPIComparisonConfig.KPIName)},
                      { nameof(KPIComparisonConfig.DiscernibleDifference)},
                      { nameof(KPIComparisonConfig.HigherIsBest)},
                      { nameof(KPIComparisonConfig.Ranked)})
                  values('totalValueDelivered', '{1.0f}', 1, 1),
                          ('totalZeroRatedSpots', '{1.0f}', 0, 1),
                          ('averageSpotsDeliveredPerDay', '{1.0f}', 1, 1);");


            migrationBuilder.Sql(@"
                  CREATE TEMPORARY table IF NOT EXISTS MultipartLengthId_Sequencing 
                  (
                      MultipartLengthId int,
                      Sequencing int
                  );");


            migrationBuilder.Sql("UPDATE Campaigns SET DeliveryCurrency = IF(DeliveryType = 0, 6, 7)");


            migrationBuilder.Sql($@"
                  UPDATE Scenarios
                  SET {nameof(Scenario.DateUserModified)} = {nameof(Scenario.DateModified)}
                  WHERE {nameof(Scenario.DateModified)} IS NOT NULL;");

            migrationBuilder.Sql($@"UPDATE Scenarios
                  SET {nameof(Scenario.DateUserModified)} = utc_timestamp()
                  WHERE {nameof(Scenario.DateModified)} IS NULL");

            migrationBuilder.Sql(@"INSERT Products (Uid, Externalidentifier, ParentExternalidentifier, Name, EffectiveStartDate, EffectiveEndDate, 
                  ClashCode, ReportingCategory)VALUES
            (N'bada0ecb-74a7-49c5-9415-00015a0dd878', N'55626', NULL, N'Altitude Mary and the Witch''s Flower', N'2018-04-12 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'DVD004', NULL),
            (N'70b854db-318c-4d6a-b1a5-00050c4d6c6c', N'44787', NULL, N'Gtech Garden Mower', N'2016-04-15 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'GAR001', NULL),
            (N'01486011-f9ff-4eb0-80ef-000842008440', N'57583', NULL, N'Vtech Secret Safe Diary and Selfie Cam', N'2018-09-13 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'TOY029', NULL),
            (N'79a6ec7f-4452-41cc-82a3-00094453b893', N'20028', NULL, N'VW Audi A6', N'2011-08-24 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'MO 004', NULL),
            (N'e0f78e23-36ea-4033-aa7b-0011dc867ee6', N'59907', NULL, N'Now 102 DRTV', N'2019-03-04 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'REC008', NULL),
            (N'502bcb30-bff6-40e9-a92f-00169e78a7ff', N'35656', NULL, N'Virgin Trains Manchester', N'2014-12-02 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'TRA001', NULL),
            (N'180b5621-1ab1-46e4-b5e1-0016b33c21fa', N'57327', NULL, N'Lidl Prosecco (NBA)', N'2018-08-29 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'AL05HF', NULL),
            (N'abf283f2-d65f-4ab0-afab-001711d4c14d', N'56230', NULL, N'Gamesloft Asphalt 9', N'2014-05-01 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'CGA003', NULL),
            (N'712f326d-5ddf-446c-ba35-0017a875a7e5', N'31135', NULL, N'Lidl Easter (IRE)', N'2014-01-31 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'R 05HF', NULL),
            (N'9d024794-f26f-469e-a5cd-002a3ce3eba9', N'56618', NULL, N'EFD Duck Duck ose DVD', N'2018-07-05 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'DVD004', NULL),
            (N'76b80b52-beb1-4fb9-b850-002bd36e916b', N'58001', NULL, N'Bethesda Fallout 76 Game', N'2018-10-03 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'CGA002', NULL),
            (N'9aaef996-9be7-40f2-bb0b-00356f84acff', N'53852', NULL, N'Lions Man With The Iron Heart DVD', N'2017-12-05 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'DVD001', NULL),
            (N'03a0dc22-0271-4b9d-bcb0-0038a973eaeb', N'62572', NULL, N'Dublin Zoo (IRE)', N'2019-10-03 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'LES006', NULL),
            (N'6076acac-e7ca-4e31-ac81-003ac3c2ad69', N'48185', NULL, N'John Lewis Nespresso Omnet', N'2016-10-31 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'RE 001', NULL),
            (N'd82ba152-c35a-4c37-8fbc-003cc9671c37', N'54210', NULL, N'Sum Up', N'2017-12-28 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'FIN007', NULL),
            (N'0a96e899-095c-48a7-9b0e-00409f6865bd', N'63513', NULL, N'Travelopia Exodus (NBA)', N'2019-12-18 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'TRA002', NULL),
            (N'0b672f60-7e8b-4d99-8e25-00436ee85a1f', N'39855', NULL, N'Villa Plus DRTV', N'2015-08-11 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'TRA002', NULL),
            (N'1bff05c1-0640-4bfa-81f9-004491f1db15', N'67', NULL, N'Halifax Current Account', N'2007-01-01 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'FIN001', NULL),
            (N'32e949f0-0e84-4e3a-87f8-00452786ba2c', N'56322', NULL, N'Subway Signature Wraps DRTV', N'2018-06-19 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'RS01HF', NULL),
            (N'ef5ca61f-ab4e-4b6a-bb03-0046104a6619', N'64393', NULL, N'Leasing Options', N'2020-04-06 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'FIL001', NULL),
            (N'e120c6c6-03b5-46aa-8f15-0046d5592966', N'55932', NULL, N'High Street TV tham Elec Indoor Direct (TES)', N'2018-05-16 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'HOU009', NULL),
            (N'f59fac35-52e8-437a-ba69-0047e9a44156', N'51860', NULL, N'Drafty.co.uk', N'2017-08-02 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'FIL004', NULL),
            (N'20412026-64ba-4676-8558-004836e7e88a', N'54868', NULL, N'Kraft Oreo Cookie Quest', N'2018-02-14 00:00:00.0000000' , N'2037-12-31 00:00:00.0000000' , N'FO02HF', NULL);");



        }
        private void DownViews(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Drop View v_productAdvertisers_and_Products;");
            migrationBuilder.Sql(@"Drop View CampaignWithProductRelations;");
            migrationBuilder.Sql(@"Drop View SpotWithCampaignAndProductRelations;");
            migrationBuilder.Sql(@"Drop View ClashExceptionDescriptions;");
        }

        private void DownStoredProcedures(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Drop Procedure spGetRecommendationAggregate;");
            migrationBuilder.Sql(@"Drop Procedure spClashExceptionProductLinkForOptimizer;");
        }

        private void UpStoredProcedures(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE spGetRecommendationAggregate(in v_ScenarioId varchar(64))
                BEGIN
	                
	                SELECT t.ExternalCampaignNumber, t.SpotRating, c.CampaignGroup, c.Name as CampaignName, c.ActualRatings, c.TargetRatings, c.EndDateTime, c.IsPercentage, ad.Name as AdvertiserName
	                FROM (
		                SELECT r.ExternalCampaignNumber,
		                SUM(CASE r.Action WHEN 'b' THEN r.SpotRating WHEN 'c' THEN -r.SpotRating END) AS SpotRating
		                FROM Recommendations AS r
		                WHERE r.ScenarioId = v_scenarioId AND (r.Action = N'B' OR r.Action = N'C')
		                GROUP BY r.ExternalCampaignNumber) as t
	                INNER JOIN Campaigns c ON c.ExternalId = t.ExternalCampaignNumber
	                INNER JOIN CampaignWithProductRelations pr ON pr.CampaignId = c.Id
	                LEFT JOIN Advertisers ad ON ad.Id = pr.AdvertiserId;

                END;");

            migrationBuilder.Sql(@"CREATE  PROCEDURE spClashExceptionProductLinkForOptimizer( In v_EndDate datetime) 
                   BEGIN
                    	SELECT ce.Id,
                             CASE ce.FromType
                                WHEN 0 THEN ce.FromValue
                                WHEN 1 THEN product_from.ExternalIdentifier
                                WHEN 2 THEN adv_from.Expr1 END AS FromValue,
                            CASE ce.ToType
                                WHEN 0 THEN ce.ToValue
                                WHEN 1 THEN product_to.ExternalIdentifier
                                WHEN 2 THEN adv_to.Expr1 END AS ToValue
                            FROM ClashExceptions AS ce
                            LEFT OUTER JOIN Products AS product_from ON ce.FromType = 1 AND product_from.Externalidentifier = ce.FromValue
                            LEFT OUTER JOIN Products AS product_to ON ce.ToType = 1 AND product_to.Externalidentifier = ce.ToValue
                            LEFT OUTER JOIN v_productAdvertisers_and_Products AS adv_from ON
				                ce.FromType = 2 AND adv_from.ExternalIdentifier = ce.FromValue AND
				                adv_from.StartDate < COALESCE (DATE_ADD( ce.EndDate, Interval 1 Day), v_EndDate, CAST( '9999-12-31' AS DATETIME)) AND adv_from.EndDate > ce.StartDate
                            LEFT OUTER JOIN v_productAdvertisers_and_Products AS adv_to ON
				                ce.ToType = 2 AND adv_to.ExternalIdentifier = ce.ToValue AND
				                adv_to.StartDate < COALESCE (DATE_ADD( ce.EndDate,interval 1 Day), v_EndDate, CAST('9999-12-31' AS DATETIME)) AND adv_to.EndDate > ce.StartDate
	                	WHERE FromValue IS NOT NULL and ToValue IS NOT NULL;
                	END");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AlterColumns(migrationBuilder);
            UpTriggers(migrationBuilder);
            UpViews(migrationBuilder);
            UpStoredProcedures(migrationBuilder);
            UpFunctions(migrationBuilder);
            UpData(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DownViews(migrationBuilder);
            DownStoredProcedures(migrationBuilder);
        }


    }
}
