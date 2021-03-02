using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11358_Sky_Initial_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesAreas_Name",
                table: "SalesAreas");

            migrationBuilder.AddColumn<double>(
                name: "AutoBookTargetedZeroRatedBreaks",
                table: "TenantSettings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BookingPosition",
                table: "Spots",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "NominalPrice",
                table: "Spots",
                type: "DECIMAL(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "EpisodeId",
                table: "ScheduleProgrammes",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetRatings",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualRatings",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryType",
                table: "ScenarioCompactCampaigns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TargetZeroRatedBreaks",
                table: "ScenarioCompactCampaigns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SalesAreas",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAreaName",
                table: "SalesAreas",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BookTargetArea",
                table: "Runs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreZeroPercentageSplit",
                table: "Runs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryCappingGroupId",
                table: "RunCampaignProcessesSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ClockNumber",
                table: "Restrictions",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "EpisodeNumber",
                table: "Restrictions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "SpotRating",
                table: "Recommendations",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<Guid>(
                name: "EpisodeId",
                table: "Programmes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampaignType",
                table: "PassRules",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BookTargetArea",
                table: "PassRules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortIndex",
                table: "FlexibilityLevels",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExternalRef",
                table: "ClashExceptions",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeights",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeights",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeightLengths",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeightLengths",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeightDayParts",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeightDayParts",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<decimal>(
                name: "CampaignPrice",
                table: "CampaignTargetStrikeWeightDayParts",
                type: "DECIMAL(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeightDayPartLengths",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeightDayPartLengths",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "DesiredPercentageSplit",
                table: "CampaignSalesAreaTargetMultiparts",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentPercentageSplit",
                table: "CampaignSalesAreaTargetMultiparts",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetRatings",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualRatings",
                table: "Campaigns",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryType",
                table: "Campaigns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TargetZeroRatedBreaks",
                table: "Campaigns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "AgRestriction_CopyCode",
                table: "AutoBookDefaultParameters",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SalesAreas_Name",
                table: "SalesAreas",
                column: "Name");

            migrationBuilder.CreateTable(
                name: "BookingPositionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    UserDefined = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPositionGroups", x => x.Id);
                    table.UniqueConstraint("AK_BookingPositionGroups_GroupId", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "BookingPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Position = table.Column<int>(nullable: false),
                    Abbreviation = table.Column<string>(maxLength: 64, nullable: true),
                    BookingOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPositions", x => x.Id);
                    table.UniqueConstraint("AK_BookingPositions_Position", x => x.Position);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBookingPositionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    DiscountSurchargePercentage = table.Column<double>(nullable: false),
                    DesiredPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CurrentPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBookingPositionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBookingPositionGroups_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryCappingGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 256, nullable: false),
                    Percentage = table.Column<int>(nullable: false),
                    ApplyToPrice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryCappingGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 32, nullable: false),
                    Description = table.Column<string>(maxLength: 64, nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    InventoryCode = table.Column<string>(type: "NCHAR(10)", nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<long>(nullable: false),
                    EndTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InventoryCode = table.Column<string>(type: "NCHAR(10)", nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    System = table.Column<string>(type: "NCHAR(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LockTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LockType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeCategoryHierarchy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    ExternalRef = table.Column<string>(maxLength: 64, nullable: true),
                    ParentExternalRef = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeCategoryHierarchy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeEpisodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    ProgrammeExternalReference = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeEpisodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RunInventoryStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    InventoryCode = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunInventoryStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunInventoryStatus_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesAreaDemographics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    ExternalRef = table.Column<string>(maxLength: 64, nullable: false),
                    Exclude = table.Column<bool>(nullable: false),
                    SupplierCode = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAreaDemographics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAreaDemographics_SalesAreas_SalesArea",
                        column: x => x.SalesArea,
                        principalTable: "SalesAreas",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandardDayPartGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    Demographic = table.Column<string>(maxLength: 64, nullable: false),
                    Optimizer = table.Column<bool>(nullable: false),
                    Policy = table.Column<bool>(nullable: false),
                    RatingReplacement = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardDayPartGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandardDayParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DayPartId = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardDayParts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    Demograph = table.Column<string>(maxLength: 64, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DaypartGroup = table.Column<int>(nullable: false),
                    Daypart = table.Column<int>(nullable: false),
                    TotalRatings = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalRatings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionGroupAssociations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BookingPosition = table.Column<int>(nullable: false),
                    BookingOrder = table.Column<int>(nullable: false),
                    BookingPositionGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionGroupAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionGroupAssociations_BookingPositions_BookingPosition",
                        column: x => x.BookingPosition,
                        principalTable: "BookingPositions",
                        principalColumn: "Position",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionGroupAssociations_BookingPositionGroups_BookingPositionGroupId",
                        column: x => x.BookingPositionGroupId,
                        principalTable: "BookingPositionGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBookingPositionGroupSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignBookingPositionGroupId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBookingPositionGroupSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBookingPositionGroupSalesAreas_CampaignBookingPositionGroups_CampaignBookingPositionGroupId",
                        column: x => x.CampaignBookingPositionGroupId,
                        principalTable: "CampaignBookingPositionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTypeLockTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InventoryTypeId = table.Column<int>(nullable: false),
                    LockType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTypeLockTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTypeLockTypes_InventoryTypes_InventoryTypeId",
                        column: x => x.InventoryTypeId,
                        principalTable: "InventoryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandardDayPartSplits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    DayPartId = table.Column<int>(nullable: false),
                    Split = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardDayPartSplits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardDayPartSplits_StandardDayPartGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "StandardDayPartGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandardDayPartTimeslices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DayPartId = table.Column<int>(nullable: false),
                    StartDay = table.Column<int>(nullable: false),
                    EndDay = table.Column<int>(nullable: false),
                    StartTime = table.Column<long>(nullable: false),
                    EndTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardDayPartTimeslices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardDayPartTimeslices_StandardDayParts_DayPartId",
                        column: x => x.DayPartId,
                        principalTable: "StandardDayParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleProgrammes_EpisodeId",
                table: "ScheduleProgrammes",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_Name",
                table: "SalesAreas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_EpisodeId",
                table: "Programmes",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPositionGroups_GroupId",
                table: "BookingPositionGroups",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPositions_Position",
                table: "BookingPositions",
                column: "Position",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroups_CampaignId",
                table: "CampaignBookingPositionGroups",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_CampaignBookingPositionGroupId",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "CampaignBookingPositionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_Name",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTypeLockTypes_InventoryTypeId",
                table: "InventoryTypeLockTypes",
                column: "InventoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionGroupAssociations_BookingPositionGroupId",
                table: "PositionGroupAssociations",
                column: "BookingPositionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionGroupAssociations_BookingPosition_BookingPositionGroupId",
                table: "PositionGroupAssociations",
                columns: new[] { "BookingPosition", "BookingPositionGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeEpisodes_Id_ProgrammeExternalReference_Number",
                table: "ProgrammeEpisodes",
                columns: new[] { "Id", "ProgrammeExternalReference", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunInventoryStatus_RunId",
                table: "RunInventoryStatus",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreaDemographics_SalesArea",
                table: "SalesAreaDemographics",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_StandardDayPartSplits_GroupId",
                table: "StandardDayPartSplits",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardDayPartTimeslices_DayPartId",
                table: "StandardDayPartTimeslices",
                column: "DayPartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Programmes_ProgrammeEpisodes_EpisodeId",
                table: "Programmes",
                column: "EpisodeId",
                principalTable: "ProgrammeEpisodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleProgrammes_ProgrammeEpisodes_EpisodeId",
                table: "ScheduleProgrammes",
                column: "EpisodeId",
                principalTable: "ProgrammeEpisodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programmes_ProgrammeEpisodes_EpisodeId",
                table: "Programmes");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleProgrammes_ProgrammeEpisodes_EpisodeId",
                table: "ScheduleProgrammes");

            migrationBuilder.DropTable(
                name: "CampaignBookingPositionGroupSalesAreas");

            migrationBuilder.DropTable(
                name: "DeliveryCappingGroup");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "InventoryLocks");

            migrationBuilder.DropTable(
                name: "InventoryTypeLockTypes");

            migrationBuilder.DropTable(
                name: "LockTypes");

            migrationBuilder.DropTable(
                name: "PositionGroupAssociations");

            migrationBuilder.DropTable(
                name: "ProgrammeCategoryHierarchy");

            migrationBuilder.DropTable(
                name: "ProgrammeEpisodes");

            migrationBuilder.DropTable(
                name: "RunInventoryStatus");

            migrationBuilder.DropTable(
                name: "SalesAreaDemographics");

            migrationBuilder.DropTable(
                name: "StandardDayPartSplits");

            migrationBuilder.DropTable(
                name: "StandardDayPartTimeslices");

            migrationBuilder.DropTable(
                name: "TotalRatings");

            migrationBuilder.DropTable(
                name: "CampaignBookingPositionGroups");

            migrationBuilder.DropTable(
                name: "InventoryTypes");

            migrationBuilder.DropTable(
                name: "BookingPositions");

            migrationBuilder.DropTable(
                name: "BookingPositionGroups");

            migrationBuilder.DropTable(
                name: "StandardDayPartGroups");

            migrationBuilder.DropTable(
                name: "StandardDayParts");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleProgrammes_EpisodeId",
                table: "ScheduleProgrammes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SalesAreas_Name",
                table: "SalesAreas");

            migrationBuilder.DropIndex(
                name: "IX_SalesAreas_Name",
                table: "SalesAreas");

            migrationBuilder.DropIndex(
                name: "IX_Programmes_EpisodeId",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "AutoBookTargetedZeroRatedBreaks",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "BookingPosition",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "NominalPrice",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "EpisodeId",
                table: "ScheduleProgrammes");

            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "TargetZeroRatedBreaks",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "TargetAreaName",
                table: "SalesAreas");

            migrationBuilder.DropColumn(
                name: "BookTargetArea",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "IgnoreZeroPercentageSplit",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "DeliveryCappingGroupId",
                table: "RunCampaignProcessesSettings");

            migrationBuilder.DropColumn(
                name: "EpisodeNumber",
                table: "Restrictions");

            migrationBuilder.DropColumn(
                name: "EpisodeId",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "CampaignType",
                table: "PassRules");

            migrationBuilder.DropColumn(
                name: "BookTargetArea",
                table: "PassRules");

            migrationBuilder.DropColumn(
                name: "SortIndex",
                table: "FlexibilityLevels");

            migrationBuilder.DropColumn(
                name: "ExternalRef",
                table: "ClashExceptions");

            migrationBuilder.DropColumn(
                name: "CampaignPrice",
                table: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "TargetZeroRatedBreaks",
                table: "Campaigns");

            migrationBuilder.AlterColumn<double>(
                name: "TargetRatings",
                table: "ScenarioCompactCampaigns",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<double>(
                name: "ActualRatings",
                table: "ScenarioCompactCampaigns",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SalesAreas",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<int>(
                name: "ClockNumber",
                table: "Restrictions",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<double>(
                name: "SpotRating",
                table: "Recommendations",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeights",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeights",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeightLengths",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeightLengths",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "DesiredPercentageSplit",
                table: "CampaignTargetStrikeWeightDayPartLengths",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPercentageSplit",
                table: "CampaignTargetStrikeWeightDayPartLengths",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "DesiredPercentageSplit",
                table: "CampaignSalesAreaTargetMultiparts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPercentageSplit",
                table: "CampaignSalesAreaTargetMultiparts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<double>(
                name: "TargetRatings",
                table: "Campaigns",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<double>(
                name: "ActualRatings",
                table: "Campaigns",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "AgRestriction_CopyCode",
                table: "AutoBookDefaultParameters",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_Name",
                table: "SalesAreas",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
