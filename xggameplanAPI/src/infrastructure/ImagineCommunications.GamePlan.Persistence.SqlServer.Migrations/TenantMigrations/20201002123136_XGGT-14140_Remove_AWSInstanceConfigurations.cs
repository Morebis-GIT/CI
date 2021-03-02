using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14140_Remove_AWSInstanceConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "AutoBookInstanceConfigurations",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "InstanceType",
                table: "AutoBookInstanceConfigurations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageSizeGb",
                table: "AutoBookInstanceConfigurations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql($@"
            IF (OBJECT_ID('[dbo].[AWSInstanceConfigurations]') IS NOT NULL )
                BEGIN
                    UPDATE [dbo].[AutoBookInstanceConfigurations]
                    SET 
                    [dbo].[AutoBookInstanceConfigurations].InstanceType = [dbo].[AWSInstanceConfigurations].InstanceType,
                    [dbo].[AutoBookInstanceConfigurations].StorageSizeGb = [dbo].[AWSInstanceConfigurations].StorageSizeGb,
                    [dbo].[AutoBookInstanceConfigurations].Cost = [dbo].[AWSInstanceConfigurations].Cost
                    FROM [dbo].[AutoBookInstanceConfigurations], [dbo].[AWSInstanceConfigurations]
                    WHERE [dbo].[AutoBookInstanceConfigurations].Id = [dbo].[AWSInstanceConfigurations].Id
                    DROP TABLE [dbo].[AWSInstanceConfigurations]
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
            IF (OBJECT_ID('[dbo].[AutoBookInstanceConfigurations]') IS NOT NULL )
                BEGIN
                    SELECT Id, Cost, InstanceType, StorageSizeGB
                    INTO [dbo].[AWSInstanceConfigurations]
                    FROM [dbo].[AutoBookInstanceConfigurations]
                END
            ");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "AutoBookInstanceConfigurations");

            migrationBuilder.DropColumn(
                name: "InstanceType",
                table: "AutoBookInstanceConfigurations");

            migrationBuilder.DropColumn(
                name: "StorageSizeGb",
                table: "AutoBookInstanceConfigurations");
        }
    }
}
