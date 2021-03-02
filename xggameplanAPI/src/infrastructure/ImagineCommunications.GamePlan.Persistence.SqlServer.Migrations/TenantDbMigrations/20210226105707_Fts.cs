using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;


namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantDbMigrations
{
    public partial class Fts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AlterColumn<string>(
                name: "TokenizedName",
                table: "Passes",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 300,
                oldNullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "SearchTokens",
                table: "Campaigns",
                maxLength: 500,
                nullable: true);

            _ = migrationBuilder.CreateFtsFields(TargetModel);
            //make expandable for further fts adding
            //make for runs
            //make tree-based refactoring in migrabuilder
            //also add a new migration for runs and maybe programmes
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchTokens",
                table: "Campaigns");

            migrationBuilder.AlterColumn<string>(
                name: "TokenizedName",
                table: "Passes",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
