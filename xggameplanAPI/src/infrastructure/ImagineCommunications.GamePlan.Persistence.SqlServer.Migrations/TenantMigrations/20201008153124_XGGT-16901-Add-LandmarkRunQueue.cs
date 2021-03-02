using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16901AddLandmarkRunQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandmarkRunQueues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandmarkRunQueues", x => x.Id);
                });

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[LandmarkRunQueues] ([Name], [DisplayName]) VALUES ('BOOKING', 'Day time queue');
                INSERT INTO [dbo].[LandmarkRunQueues] ([Name], [DisplayName]) VALUES ('BATCHB1', 'Night time queue');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandmarkRunQueues");
        }
    }
}
