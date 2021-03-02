using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterDbMigrations
{
    public partial class InitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {   //seed the tenant table so the below inserts will work
            migrationBuilder.Sql(@" insert into Tenants(name, DefaultTheme, TenantDb_Provider, TenantDb_ConnectionString)
            Values('Development', 'Default', 0, 'Url = http://localhost:8080;Database=TenantDB');");

            //20200319095206_XGGT - 10713_Skip - Locked - Breaks - Seed - MySQL
            migrationBuilder.Sql(@"INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared)" +
                "SELECT Id, 'SkipLockedBreaks', 0, 1 FROM Tenants " +
                " WHERE  EXISTS(SELECT 1 FROM TenantProductFeatures);");


            //20200320104432_XGGT-9241_SpotBookingRules_Feature_Seed - MySQL
            migrationBuilder.Sql(@"INSERT INTO TenantProductFeatures(TenantId, Name, Enabled, IsShared)  " +
                   " SELECT id,'SpotBookingRule', 0,0 FROM Tenants " +
                 "  WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");


            //20200323093255_XGGT-11538_FeatureFlag_StrikeWeightDayPartsMerge - MySQL
            migrationBuilder.Sql($@"INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared) " +
                                 "SELECT Id, '{nameof(ProductFeature.StrikeWeightDayPartsMerge)}', 0, 0 FROM Tenants " +
                                 "WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");

            //20200323175649_XGGT-9240_Length_Factors_Feature_Seed - MySQL
            migrationBuilder.Sql(@"
                  INSERT INTO TenantProductFeatures(TenantId, Name, Enabled, IsShared)
                   SELECT Id, 'LengthFactor', 0,0 FROM Tenants  " +
                 " WHERE EXISTS(SELECT 1 FROM TenantProductFeatures);");


            //20200407160053_Integration_Add_Feature_Flag - MySQL
            migrationBuilder.Sql(
                 "INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared) " +
                "SELECT Id, 'LandmarkBooking', 0, 1 FROM Tenants " +
                 "WHERE  EXISTS(SELECT 1 FROM TenantProductFeatures);");


            //20200416092827_XGGT-12670_Adding_Sky_Specific_Feature_Flags - MySQL
            migrationBuilder.Sql(@"
                    INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared)
                        SELECT Id, 'NineValidationMinSpot', 1, 0 FROM Tenants
                            WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");

            migrationBuilder.Sql(@"
                    INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared)
                        SELECT Id, 'NineValidationRatingPredictions', 1, 0 FROM Tenants
                            WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");
            migrationBuilder.Sql(@"
            INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared)
                        SELECT Id, 'IncludeChannelGroupFileForOptimiser', 1, 0 FROM Tenants
                            WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");

            //20200423044207_XGGT-12807_RatingsPredictions_Logic_Changes - MySQL
            migrationBuilder.Sql(@"
                INSERT INTO TenantProductFeatures(TenantId, Name, Enabled, IsShared)
                    SELECT id, 'ExactBreaksRatingsTimeMatching', 0,  0  FROM Tenants" +
                        " WHERE EXISTS(SELECT 1 FROM TenantProductFeatures);");

            //20200508085422_XGGT-12303_Add_Run_Campaign_List_On_Creation_Product_Feature - MySQL
            migrationBuilder.Sql(
               "INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared)" +
               "SELECT Id, 'RunCampaignListOnCreation', 1, 1 FROM Tenants " +
               " WHERE EXISTS(SELECT 1 FROM TenantProductFeatures);");

            //20200515091209_AddDataLoadFeatureFlag - MySQL
            migrationBuilder.Sql(
                "INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared) " +
                "SELECT Id, 'PrePostCampaignResults', 0, 1 FROM Tenants " +
                " WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");

            //20200520180754_XGGT-13351_Add_Smooth_Feature_Flag - MySQL
            migrationBuilder.Sql(
                "INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared) " +
                "SELECT Id, 'Smooth', 0, 1 FROM Tenants " +
                " WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");

            //20200529165924_XGGT-12543-Add_Run_Type_Feature_Flag - MySQL
            migrationBuilder.Sql(@"
                       INSERT INTO TenantProductFeatures 
                        (TenantId, Name, Enabled, IsShared)
	                        SELECT Id, 'RunType', 1, 1 
                            FROM Tenants 
	                        WHERE ID NOT IN 
                                    (SELECT TenantId 
                                     FROM TenantProductFeatures 
                                     WHERE Name = 'RunType');");


            //20200707065634_XGGT-14208_Add_Scenario_Performance_Measurement_KPIs_Product_Feature - MySQL
            migrationBuilder.Sql(
               "INSERT INTO TenantProductFeatures (TenantId, Name, Enabled, IsShared) " +
               "SELECT Id, 'ScenarioPerformanceMeasurementKPIs', 1, 1 FROM Tenants " +
               " WHERE EXISTS (SELECT 1 FROM TenantProductFeatures);");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from Tenants");
            migrationBuilder.Sql(@"delete from TenantProductFeatures");
        }
    }
}
