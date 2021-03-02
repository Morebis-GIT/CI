using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10786_FunctionalArea_New_FaultType : Migration
    {
        private const int TarpsFaultTypeId = 82;
        private const string EngLanguageAbbreviation = "ENG";
        private const string AraLanguageAbbreviation = "ARA";
        private const string TarpsDescription = "Min TARPs not met";
        private const string SlottingControlsFunctionalAreaId = "3152EDD5-CD20-40AC-B315-C8D5D9FE890B";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"

                IF EXISTS (SELECT TOP 1 1 FROM FaultTypes)
                BEGIN
                    insert into FaultTypes
                    values ({TarpsFaultTypeId})

                    insert into FaultTypeDescriptions(
                        {nameof(FaultTypeDescription.FaultTypeId)},
                        {nameof(FaultTypeDescription.LanguageAbbreviation)},
                        {nameof(FaultTypeDescription.Description)})
                    values  ({TarpsFaultTypeId}, '{EngLanguageAbbreviation}', '{TarpsDescription}'),
                            ({TarpsFaultTypeId}, '{AraLanguageAbbreviation}', '{TarpsDescription}')

                    IF  EXISTS (SELECT TOP 1 1 FROM dbo.FunctionalAreas WHERE Id = '{SlottingControlsFunctionalAreaId}')
                    AND NOT EXISTS (SELECT TOP 1 1 FROM dbo.FunctionalAreaFaultTypes WHERE FaultTypeId = {TarpsFaultTypeId})
                    BEGIN
                        insert into FunctionalAreaFaultTypes(
                            {nameof(FunctionalAreaFaultType.FunctionalAreaId)},
                            {nameof(FunctionalAreaFaultType.FaultTypeId)},
                            {nameof(FunctionalAreaFaultType.IsSelected)})
                        values	('{SlottingControlsFunctionalAreaId}', {TarpsFaultTypeId}, 1)
                    END
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                delete from FaultTypes
                where Id = {TarpsFaultTypeId}
            ");
        }
    }
}
