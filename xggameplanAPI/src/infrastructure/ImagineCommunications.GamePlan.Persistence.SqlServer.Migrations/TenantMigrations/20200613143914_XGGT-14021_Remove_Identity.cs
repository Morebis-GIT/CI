using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14021_Remove_Identity : Migration
    {
        private const string CampaignSequenceName = "CampaignNoIdentity";
        private const string SalesAreaSequenceName = "SalesAreaNoIdentity";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RemoveIdentityForSalesAreas(migrationBuilder);
            RemoveIdentityForCampaigns(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RevertSalesAreasIdentity(migrationBuilder);
            RevertCampainsIdentity(migrationBuilder);
        }

        private void RevertSalesAreasIdentity(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomIdWithIdentity",
                table: "SalesAreas",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.DropIndex(
                name: "IX_SalesAreas_CustomId",
                table: "SalesAreas");

            migrationBuilder.DropColumn(
                name: "CustomId",
                table: "SalesAreas");

            migrationBuilder.RenameColumn(
                name: "CustomIdWithIdentity",
                table: "SalesAreas",
                newName: "CustomId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_CustomId",
                table: "SalesAreas",
                column: "CustomId",
                unique: true);

            migrationBuilder.DropSequence(SalesAreaSequenceName);
        }

        private void RevertCampainsIdentity(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomIdWithIdentity",
                table: "Campaigns",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CustomId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CustomId",
                table: "Campaigns");

            migrationBuilder.RenameColumn(
                name: "CustomIdWithIdentity",
                table: "Campaigns",
                newName: "CustomId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CustomId",
                table: "Campaigns",
                column: "CustomId",
                unique: true);

            migrationBuilder.DropSequence(CampaignSequenceName);
        }

        private void RemoveIdentityForSalesAreas(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomIdWithoutIdentity",
                table: "SalesAreas",
                nullable: true);

            migrationBuilder.Sql("UPDATE SalesAreas SET CustomIdWithoutIdentity = CustomId");

            migrationBuilder.DropIndex(
                name: "IX_SalesAreas_CustomId",
                table: "SalesAreas");

            migrationBuilder.DropColumn(
                name: "CustomId",
                table: "SalesAreas");

            migrationBuilder.RenameColumn(
                name: "CustomIdWithoutIdentity",
                table: "SalesAreas",
                newName: "CustomId");

            migrationBuilder.AlterColumn<int>(name: "CustomId",
                table: "SalesAreas",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_CustomId",
                table: "SalesAreas",
                column: "CustomId",
                unique: true);

            migrationBuilder.CreateSequence<int>(SalesAreaSequenceName);

            RestartSequenceWithMaxValue(migrationBuilder, SalesAreaSequenceName, "SalesAreas", "CustomId");
        }

        private void RemoveIdentityForCampaigns(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomIdWithoutIdentity",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.Sql("UPDATE Campaigns SET CustomIdWithoutIdentity = CustomId");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CustomId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CustomId",
                table: "Campaigns");

            migrationBuilder.RenameColumn(
                name: "CustomIdWithoutIdentity",
                table: "Campaigns",
                newName: "CustomId");

            migrationBuilder.AlterColumn<int>(name: "CustomId",
                table: "Campaigns",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CustomId",
                table: "Campaigns",
                column: "CustomId",
                unique: true);

            migrationBuilder.CreateSequence<int>(CampaignSequenceName);

            RestartSequenceWithMaxValue(migrationBuilder, CampaignSequenceName, "Campaigns", "CustomId");
        }

        private void RestartSequenceWithMaxValue(MigrationBuilder migrationBuilder, string sequence, string table, string column)
        {
            var expressionName = $"@s{sequence}";
            var maxIdName = $"@maxId{table}{column}";
            migrationBuilder.Sql(@$"
                declare {maxIdName} int;
                select {maxIdName} = max({column}) + 1 from {table};
                DECLARE {expressionName} nvarchar(1000);

                SET {expressionName} = N'
                ALTER SEQUENCE
                {sequence}
                RESTART WITH ' + CAST({maxIdName} AS nvarchar(10));

                exec({expressionName});");
        }
    }
}
