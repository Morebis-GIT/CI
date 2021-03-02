using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGPT1093_Seed_BRS_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[KPIPriorities]) 
                BEGIN
                    SET IDENTITY_INSERT [dbo].[KPIPriorities] ON

                    INSERT INTO [dbo].[KPIPriorities] (Id, Name, WeightingFactor) VALUES (1, 'Exclude',0)
                    INSERT INTO [dbo].[KPIPriorities] (Id, Name, WeightingFactor) VALUES (2, 'Extremely Low',0.3)
                    INSERT INTO [dbo].[KPIPriorities] (Id, Name, WeightingFactor) VALUES (3, 'Low',0.7)
                    INSERT INTO [dbo].[KPIPriorities] (Id, Name, WeightingFactor) VALUES (4, 'Medium',1)
                    INSERT INTO [dbo].[KPIPriorities] (Id, Name, WeightingFactor) VALUES (5, 'High',1.3)
                    INSERT INTO [dbo].[KPIPriorities] (Id, Name, WeightingFactor) VALUES (6, 'Extremely High',1.7)

                    SET IDENTITY_INSERT [dbo].[KPIPriorities] OFF
                END

                IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[BRSConfigurationTemplates])
                BEGIN
                    SET IDENTITY_INSERT [dbo].[BRSConfigurationTemplates] ON

                    INSERT INTO [dbo].[BRSConfigurationTemplates] (Id,Name,LastModified,IsDefault) VALUES (1, 'Default template', GETDATE(), 1)

                    SET IDENTITY_INSERT [dbo].[BRSConfigurationTemplates] OFF

                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'percent95to105', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'percentbelow75', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'averageEfficiency', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'totalSpotsBooked', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'remainaudience', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'remainingAvailability', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'standardAverageCompletion', 4)
                    INSERT INTO [dbo].[BRSConfigurationForKPIs] VALUES (1, 'weightedAverageCompletion', 4)
                END

            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [dbo].[BRSConfigurationForKPIs]
                DELETE FROM [dbo].[BRSConfigurationTemplates]
                DELETE FROM [dbo].[KPIPriorities]
            ");
        }
    }
}
