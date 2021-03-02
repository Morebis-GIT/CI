using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13154_Add_Timestamp_Of_Last_User_Modification_To_Scenario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUserModified",
                table: "Scenarios",
                nullable: true);

            //Set date of scenarios' last user modifications initial value equal to DateModified field's current value if possible
            migrationBuilder.Sql(@$"
                UPDATE Scenarios
                SET {nameof(Scenario.DateUserModified)} = {nameof(Scenario.DateModified)}
                WHERE {nameof(Scenario.DateModified)} IS NOT NULL

                UPDATE Scenarios
                SET {nameof(Scenario.DateUserModified)} = GETUTCDATE()
                WHERE {nameof(Scenario.DateModified)} IS NULL");

            //And then make new field not-nullable since it must always have a value
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateUserModified",
                table: "Scenarios",
                nullable: false,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUserModified",
                table: "Scenarios");
        }
    }
}
