using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13063_Rename_Tarps_FaultType : Migration
    {
        private const int TarpsFaultTypeId = 82;
        private const string DescriptionOld = "Min TARPs not met";
        private const string DescriptionNew = "Min Rating Points not met";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FaultTypeDescriptions",
                keyColumn: nameof(FaultTypeDescription.FaultTypeId),
                keyValue: TarpsFaultTypeId,
                column: nameof(FaultTypeDescription.Description),
                value: DescriptionNew);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FaultTypeDescriptions",
                keyColumn: nameof(FaultTypeDescription.FaultTypeId),
                keyValue: TarpsFaultTypeId,
                column: nameof(FaultTypeDescription.Description),
                value: DescriptionOld);
        }
    }
}
