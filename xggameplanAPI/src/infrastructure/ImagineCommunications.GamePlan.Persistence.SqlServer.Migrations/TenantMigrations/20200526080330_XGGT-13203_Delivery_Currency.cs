using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13203_Delivery_Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryCurrency",
                table: "Campaigns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Campaigns SET DeliveryCurrency = IIF(DeliveryType = 0, 6, 7)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryCurrency",
                table: "Campaigns");
        }
    }
}
