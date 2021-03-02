using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10396_SalesArea_Collate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateDatabase(migrationBuilder, () =>
            {
                migrationBuilder.Sql(@"
                    ALTER TABLE [SalesAreas]
                    ALTER COLUMN [ShortName] NVARCHAR(256) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [dbo].[SalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SalesAreaDemographics]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Breaks]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignSalesAreaTargets]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ClashDifferences]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [FailureItems]
                    ALTER COLUMN [SalesAreaName] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [IndexTypes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [InventoryLocks]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ISRSettings]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [PassBreakExclusions]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [PassSalesAreaPriorities]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [PredictionSchedules]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Programmes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Recommendations]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [RestrictionsSalesAreas]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [RSSettings]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [RunSalesAreaPriorities]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SalesAreaPriorities]
                    ALTER COLUMN [SalesArea] NVARCHAR(256) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ScenarioCampaignResults]
                    ALTER COLUMN [SalesAreaName] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ScheduleBreaks]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ScheduleProgrammes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Schedules]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SmoothDiagnosticConfigurations]
                    ALTER COLUMN [SpotSalesAreas] NVARCHAR(MAX) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SmoothFailures]
                    ALTER COLUMN [SalesArea] NVARCHAR(MAX) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SponsorshipItems]
                    ALTER COLUMN [SalesAreas] NVARCHAR(MAX) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Spots]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [StandardDayPartGroups]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [StandardDayParts]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [TotalRatings]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Universes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [AgCampaignProgrammes]
                    ALTER COLUMN [SalesAreas] NVARCHAR(MAX) COLLATE Latin1_General_100_CS_AI NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignBookingPositionGroupSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(512) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignProgrammeRestrictionsSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignSalesAreaTargetGroupSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignTimeRestrictionsSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(64) COLLATE Latin1_General_100_CS_AI NOT NULL
                    GO
                ");

            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpdateDatabase(migrationBuilder, () =>
            {
                migrationBuilder.Sql(@"
                    ALTER TABLE [SalesAreas]
                    ALTER COLUMN [ShortName] NVARCHAR(256) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [dbo].[SalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SalesAreaDemographics]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Breaks]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignSalesAreaTargets]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ClashDifferences]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [FailureItems]
                    ALTER COLUMN [SalesAreaName] NVARCHAR(512) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [IndexTypes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [InventoryLocks]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ISRSettings]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [PassBreakExclusions]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [PassSalesAreaPriorities]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [PredictionSchedules]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Programmes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Recommendations]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [RestrictionsSalesAreas]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [RSSettings]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [RunSalesAreaPriorities]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SalesAreaPriorities]
                    ALTER COLUMN [SalesArea] NVARCHAR(256) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ScenarioCampaignResults]
                    ALTER COLUMN [SalesAreaName] NVARCHAR(512) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ScheduleBreaks]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [ScheduleProgrammes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Schedules]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SmoothDiagnosticConfigurations]
                    ALTER COLUMN [SpotSalesAreas] NVARCHAR(MAX) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SmoothFailures]
                    ALTER COLUMN [SalesArea] NVARCHAR(MAX) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [SponsorshipItems]
                    ALTER COLUMN [SalesAreas] NVARCHAR(MAX) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Spots]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [StandardDayPartGroups]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [StandardDayParts]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [TotalRatings]
                    ALTER COLUMN [SalesArea] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [Universes]
                    ALTER COLUMN [SalesArea] NVARCHAR(64) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [AgCampaignProgrammes]
                    ALTER COLUMN [SalesAreas] NVARCHAR(MAX) NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignBookingPositionGroupSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(512) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignProgrammeRestrictionsSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignSalesAreaTargetGroupSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(64) NOT NULL
                    GO
                ");

                migrationBuilder.Sql(@"
                    ALTER TABLE [CampaignTimeRestrictionsSalesAreas]
                    ALTER COLUMN [Name] NVARCHAR(64) NOT NULL
                    GO
                ");
            });
        }

        private void UpdateDatabase(MigrationBuilder migrationBuilder, Action action)
        {
            migrationBuilder.DropIndex("IX_SalesAreas_ShortName", "SalesAreas");

            migrationBuilder.DropForeignKey("FK_SalesAreaDemographics_SalesAreas_SalesArea", "SalesAreaDemographics");

            migrationBuilder.DropUniqueConstraint(
               name: "AK_SalesAreas_Name",
               table: "SalesAreas");

            migrationBuilder.DropIndex("IX_SalesAreas_Name", "SalesAreas");

            migrationBuilder.DropIndex("IX_SalesAreaDemographics_SalesArea", "SalesAreaDemographics");




            migrationBuilder.DropIndex("IX_Breaks_SalesArea", "Breaks");

            migrationBuilder.DropIndex("IX_ScheduleBreaks_Summary", "Breaks");

            migrationBuilder.DropIndex("IX_CampaignSalesAreaTargets_SalesArea", "CampaignSalesAreaTargets");

            migrationBuilder.DropIndex("IX_ClashDifferences_SalesArea", "ClashDifferences");

            migrationBuilder.DropIndex("IX_IndexTypes_SalesArea", "IndexTypes");

            migrationBuilder.DropIndex("IX_ISRSettings_SalesArea", "ISRSettings");

            migrationBuilder.DropIndex("IX_PredictionSchedules_SalesArea", "PredictionSchedules");

            migrationBuilder.DropIndex("IX_PredictionSchedules_SalesArea_ScheduleDay", "PredictionSchedules");

            migrationBuilder.DropIndex("IX_Programmes_SalesArea", "Programmes");

            migrationBuilder.DropIndex("IX_RSSettings_SalesArea", "RSSettings");

            migrationBuilder.DropIndex("IX_RunSalesAreaPriorities_SalesArea", "RunSalesAreaPriorities");

            migrationBuilder.DropIndex("IX_ScheduleBreaks_SalesArea", "ScheduleBreaks");

            migrationBuilder.DropIndex("IX_ScheduleProgrammes_SalesArea", "ScheduleProgrammes");

            migrationBuilder.DropIndex("IX_Schedules_SalesArea", "Schedules");

            migrationBuilder.DropIndex("IX_Schedules_Date_SalesArea", "Schedules");

            migrationBuilder.DropIndex("IX_Spots_SalesArea", "Spots");

            migrationBuilder.DropIndex("IX_Universes_SalesArea", "Universes");

            migrationBuilder.DropIndex("IX_CampaignBookingPositionGroupSalesAreas_Name", "CampaignBookingPositionGroupSalesAreas");

            migrationBuilder.DropIndex("IX_CampaignProgrammeRestrictionsSalesAreas_Name", "CampaignProgrammeRestrictionsSalesAreas");

            migrationBuilder.DropIndex("IX_CampaignSalesAreaTargetGroupSalesAreas_Name", "CampaignSalesAreaTargetGroupSalesAreas");

            migrationBuilder.DropIndex("IX_CampaignTimeRestrictionsSalesAreas_Name", "CampaignTimeRestrictionsSalesAreas");

            migrationBuilder.DropIndex("IX_RestrictionsSalesAreas_SalesArea", "RestrictionsSalesAreas");



            action();

            migrationBuilder.CreateIndex(
              name: "IX_RestrictionsSalesAreas_SalesArea",
              table: "RestrictionsSalesAreas",
              column: "SalesArea");

            migrationBuilder.CreateIndex(
              name: "IX_SalesAreaDemographics_SalesArea",
              table: "SalesAreaDemographics",
              column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_Name",
                table: "SalesAreas",
                column: "Name",
                unique: true);

            migrationBuilder.AddUniqueConstraint(
               name: "AK_SalesAreas_Name",
               table: "SalesAreas",
               column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesAreaDemographics_SalesAreas_SalesArea",
                table: "SalesAreaDemographics",
                column: "SalesArea",
                principalTable: "SalesAreas",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.CreateIndex(
               name: "IX_SalesAreas_ShortName",
               table: "SalesAreas",
               column: "ShortName",
               unique: true,
               filter: "[ShortName] IS NOT NULL");




            migrationBuilder.CreateIndex(
               name: "IX_Breaks_SalesArea",
               table: "Breaks",
               column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesArea", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });

            migrationBuilder.CreateIndex(
              name: "IX_CampaignSalesAreaTargets_SalesArea",
              table: "CampaignSalesAreaTargets",
              column: "SalesArea");

            migrationBuilder.CreateIndex(
               name: "IX_ClashDifferences_SalesArea",
               table: "ClashDifferences",
               column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_SalesArea",
                table: "IndexTypes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
               name: "IX_ISRSettings_SalesArea",
               table: "ISRSettings",
               column: "SalesArea");

            migrationBuilder.CreateIndex(
               name: "IX_PredictionSchedules_SalesArea",
               table: "PredictionSchedules",
               column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesArea_ScheduleDay",
                table: "PredictionSchedules",
                columns: new[] { "SalesArea", "ScheduleDay" });

            migrationBuilder.CreateIndex(
               name: "IX_Programmes_SalesArea",
               table: "Programmes",
               column: "SalesArea");

            migrationBuilder.CreateIndex(
               name: "IX_RSSettings_SalesArea",
               table: "RSSettings",
               column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_RunSalesAreaPriorities_SalesArea",
                table: "RunSalesAreaPriorities",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_SalesArea",
                table: "ScheduleBreaks",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleProgrammes_SalesArea",
                table: "ScheduleProgrammes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SalesArea",
                table: "Schedules",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Date_SalesArea",
                table: "Schedules",
                columns: new[] { "Date", "SalesArea" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spots_SalesArea",
                table: "Spots",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Universes_SalesArea",
                table: "Universes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_Name",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_Name",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_Name",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_Name",
                table: "CampaignTimeRestrictionsSalesAreas",
                column: "Name");
        }
    }
}
