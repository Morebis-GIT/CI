using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14791_KPI_Precision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ValueDifferenceExcludingPayback",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValueDifference",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RatingsDifferenceExcludingPayback",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AchievedPercentageTargetRatings",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AchievedPercentageRevenueBudget",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ValueDifferenceExcludingPayback",
                table: "Campaigns",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValueDifference",
                table: "Campaigns",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RatingsDifferenceExcludingPayback",
                table: "Campaigns",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AchievedPercentageTargetRatings",
                table: "Campaigns",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AchievedPercentageRevenueBudget",
                table: "Campaigns",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)",
                oldNullable: true);
        }
    }
}
