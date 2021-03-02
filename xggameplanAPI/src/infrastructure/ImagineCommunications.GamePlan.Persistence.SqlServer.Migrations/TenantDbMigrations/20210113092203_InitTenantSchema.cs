using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantDbMigrations
{
    public partial class InitTenantSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advertisers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true),
                    ExternalIdentifier = table.Column<string>(maxLength: 64, nullable: false),
                    TokenizedName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true),
                    ExternalIdentifier = table.Column<string>(maxLength: 64, nullable: false),
                    TokenizedName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgencyGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true),
                    Code = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FilterAdvertiserExternalIds = table.Column<string>(nullable: true),
                    FilterAgencyExternalIds = table.Column<string>(nullable: true),
                    FilterAgencyGroupCodes = table.Column<string>(nullable: true),
                    FilterBusinessTypes = table.Column<string>(nullable: true),
                    FilterCampaignExternalIds = table.Column<string>(nullable: true),
                    FilterClashExternalRefs = table.Column<string>(nullable: true),
                    FilterProductExternalIds = table.Column<string>(nullable: true),
                    FilterReportingCategories = table.Column<string>(nullable: true),
                    FilterSalesExecExternalIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoBookDefaultParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(maxLength: 64, nullable: false),
                    AgBreak_SalesAreaNo = table.Column<int>(nullable: false),
                    AgBreak_SalesAreaId = table.Column<int>(nullable: false),
                    AgBreak_ScheduledDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgBreak_BreakNo = table.Column<int>(nullable: false),
                    AgBreak_ExternalNo = table.Column<string>(maxLength: 64, nullable: true),
                    AgBreak_NominalTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgBreak_Uid = table.Column<int>(nullable: false),
                    AgBreak_ProgEventNo = table.Column<int>(nullable: false),
                    AgBreak_Duration = table.Column<int>(nullable: false),
                    AgBreak_Solus = table.Column<string>(maxLength: 32, nullable: true),
                    AgBreak_DayNumber = table.Column<int>(nullable: false),
                    AgBreak_PositionInProg = table.Column<string>(maxLength: 64, nullable: true),
                    AgBreak_ProgNo = table.Column<int>(nullable: false),
                    AgBreak_EpisNo = table.Column<int>(nullable: false),
                    AgBreak_BreakTypeCode = table.Column<string>(maxLength: 32, nullable: true),
                    AgBreak_NbrBkrgs = table.Column<int>(nullable: false),
                    AgBreak_NbrZeroBkrgs = table.Column<int>(nullable: false),
                    AgBreak_NbrPreds = table.Column<int>(nullable: false),
                    AgBreak_NbrAvals = table.Column<int>(nullable: false),
                    AgBreak_NbrPrgcs = table.Column<int>(nullable: false),
                    AgBreak_AgProgCategories = table.Column<string>(nullable: true),
                    AgBreak_MaxPrgcs = table.Column<int>(nullable: false),
                    AgBreak_AgSalesAreaPtrRef_SalesAreaNo = table.Column<int>(nullable: false),
                    AgBreak_AgSalesAreaPtrRef_ClassId = table.Column<int>(nullable: false),
                    AgBreak_LongForm = table.Column<string>(maxLength: 512, nullable: true),
                    AgCampaign_CampaignNo = table.Column<int>(nullable: false),
                    AgCampaign_ExternalNo = table.Column<string>(maxLength: 64, nullable: true),
                    AgCampaign_DemographicNo = table.Column<int>(nullable: false),
                    AgCampaign_DealNo = table.Column<int>(nullable: false),
                    AgCampaign_ProductCode = table.Column<int>(nullable: false),
                    AgCampaign_ClearanceCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgCampaign_BusinesssAreaNo = table.Column<int>(nullable: false),
                    AgCampaign_RevenueBudget = table.Column<int>(nullable: false),
                    AgCampaign_DeliveryCurrency = table.Column<int>(nullable: false),
                    AgCampaign_MultiPartFlag = table.Column<string>(maxLength: 64, nullable: true),
                    AgCampaign_StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgCampaign_EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgCampaign_RootClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgCampaign_ClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgCampaign_AdvertiserIdentifier = table.Column<string>(maxLength: 256, nullable: true),
                    AgCampaign_ClashNo = table.Column<int>(nullable: false),
                    AgCampaign_AgCampaignRequirement_Required = table.Column<double>(nullable: false),
                    AgCampaign_AgCampaignRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgCampaign_AgCampaignRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgCampaign_AgCampaignRequirement_Supplied = table.Column<double>(nullable: false),
                    AgCampaign_IncludeFunctions = table.Column<int>(nullable: false),
                    AgCampaign_NbrAgCampagignSalesArea = table.Column<int>(nullable: false),
                    AgCampaign_MaxAgCampagignSalesArea = table.Column<int>(nullable: false),
                    AgCampaign_CampaignSpotMaxRatings = table.Column<int>(nullable: false),
                    AgCampaign_NbrAgCampaignProgrammeList = table.Column<int>(nullable: false),
                    AgExposure_BreakSalesAreaNo = table.Column<int>(nullable: false),
                    AgExposure_ClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgExposure_MasterClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgExposure_StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgExposure_EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgExposure_StartTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgExposure_EndTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgExposure_StartDay = table.Column<int>(nullable: false),
                    AgExposure_EndDay = table.Column<int>(nullable: false),
                    AgExposure_NoOfExposures = table.Column<int>(nullable: false),
                    AgISRTimeBand_StartTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgISRTimeBand_EndTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgISRTimeBand_Days = table.Column<int>(nullable: false),
                    AgISRTimeBand_Exclude = table.Column<string>(maxLength: 16, nullable: true),
                    AgPeakStartEndTime_SalesArea = table.Column<int>(nullable: false),
                    AgPeakStartEndTime_ScenarioNumber = table.Column<int>(nullable: false),
                    AgPeakStartEndTime_DayPartNumber = table.Column<int>(nullable: false),
                    AgPeakStartEndTime_StartDayOfDayPart = table.Column<int>(nullable: false),
                    AgPeakStartEndTime_EndDayOfDayPart = table.Column<int>(nullable: false),
                    AgPeakStartEndTime_StartTimeOfDayPart = table.Column<string>(maxLength: 32, nullable: true),
                    AgPeakStartEndTime_EndTimeOfDayPart = table.Column<string>(maxLength: 32, nullable: true),
                    AgPeakStartEndTime_MidPoint = table.Column<int>(nullable: false),
                    AgPeakStartEndTime_Name = table.Column<string>(maxLength: 256, nullable: true),
                    AgProgRestriction_CampaignNo = table.Column<int>(nullable: false),
                    AgProgRestriction_SalesAreaNo = table.Column<int>(nullable: false),
                    AgProgRestriction_ProgNo = table.Column<int>(nullable: false),
                    AgProgRestriction_PrgcNo = table.Column<int>(nullable: false),
                    AgProgRestriction_IncludeExcludeFlag = table.Column<string>(maxLength: 64, nullable: true),
                    AgProgRestriction_EpisNo = table.Column<int>(nullable: false),
                    AgProgTxDetail_ProgrammeNo = table.Column<int>(nullable: false),
                    AgProgTxDetail_EpisodeNo = table.Column<int>(nullable: false),
                    AgProgTxDetail_SalesAreaNo = table.Column<int>(nullable: false),
                    AgProgTxDetail_TregNo = table.Column<int>(nullable: false),
                    AgProgTxDetail_TxDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgProgTxDetail_ScheduledStartTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgProgTxDetail_ScheduledEndTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgProgTxDetail_ProgCategoryNo = table.Column<int>(nullable: false),
                    AgProgTxDetail_ClassCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgProgTxDetail_LiveBroadcast = table.Column<string>(maxLength: 16, nullable: true),
                    AgRestriction_ProductCode = table.Column<int>(nullable: false),
                    AgRestriction_ClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgRestriction_CopyCode = table.Column<string>(maxLength: 64, nullable: false),
                    AgRestriction_ClearanceCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgRestriction_SalesAreaNo = table.Column<int>(nullable: false),
                    AgRestriction_ProgCategoryNo = table.Column<int>(nullable: false),
                    AgRestriction_ProgrammeNo = table.Column<int>(nullable: false),
                    AgRestriction_EpisodeNo = table.Column<int>(nullable: false),
                    AgRestriction_StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgRestriction_EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgRestriction_IndexType = table.Column<int>(nullable: false),
                    AgRestriction_IndexThreshold = table.Column<int>(nullable: false),
                    AgRestriction_PublicHolidayIndicator = table.Column<string>(maxLength: 64, nullable: true),
                    AgRestriction_SchoolHolidayIndicator = table.Column<string>(maxLength: 64, nullable: true),
                    AgRestriction_RestrictionType = table.Column<int>(nullable: false),
                    AgRestriction_RestrictionDays = table.Column<int>(nullable: false),
                    AgRestriction_StartTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgRestriction_EndTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgRestriction_TimeToleranceMinsBefore = table.Column<int>(nullable: false),
                    AgRestriction_TimeToleranceMinsAfter = table.Column<int>(nullable: false),
                    AgRestriction_ProgClassCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgRestriction_ProgClassFlag = table.Column<string>(maxLength: 16, nullable: true),
                    AgRestriction_LiveBroadcastFlag = table.Column<string>(maxLength: 16, nullable: true),
                    AgSpot_BreakSalesAreaNo = table.Column<int>(nullable: false),
                    AgSpot_BreakDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgSpot_BreakTime = table.Column<string>(maxLength: 32, nullable: true),
                    AgSpot_BreakNo = table.Column<int>(nullable: false),
                    AgSpot_CampaignNo = table.Column<int>(nullable: false),
                    AgSpot_SpotNo = table.Column<int>(nullable: false),
                    AgSpot_Status = table.Column<string>(maxLength: 32, nullable: true),
                    AgSpot_MultipartIndicator = table.Column<string>(maxLength: 16, nullable: true),
                    AgSpot_PreempteeStatus = table.Column<int>(nullable: false),
                    AgSpot_PreemptorStatus = table.Column<int>(nullable: false),
                    AgSpot_BookingPosition = table.Column<int>(nullable: false),
                    AgSpot_SpotLength = table.Column<int>(nullable: false),
                    AgSpot_SpotSalesAreaNo = table.Column<int>(nullable: false),
                    AgSpot_PriceFactor = table.Column<double>(nullable: false),
                    AgSpot_BonusSpot = table.Column<string>(maxLength: 64, nullable: true),
                    AgSpot_ClientPicked = table.Column<string>(maxLength: 16, nullable: true),
                    AgSpot_ISRLocked = table.Column<int>(nullable: false),
                    AgSpot_ProductCode = table.Column<int>(nullable: false),
                    AgSpot_ClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    AgSpot_AdvertiserIdentifier = table.Column<string>(maxLength: 256, nullable: true),
                    AgSpot_RootClashCode = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookDefaultParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoBookInstanceConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    CloudProvider = table.Column<byte>(nullable: false),
                    InstanceType = table.Column<string>(nullable: true),
                    StorageSizeGb = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookInstanceConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoBooks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookId = table.Column<string>(maxLength: 64, nullable: false),
                    Api = table.Column<string>(maxLength: 256, nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Locked = table.Column<bool>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    LastRunStarted = table.Column<DateTime>(nullable: false),
                    LastRunCompleted = table.Column<DateTime>(nullable: false),
                    InstanceConfigurationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBooks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoBookSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Locked = table.Column<bool>(nullable: false),
                    ProvisioningAPIURL = table.Column<string>(maxLength: 256, nullable: true),
                    AutoProvisioning = table.Column<bool>(nullable: false),
                    AutoProvisioningLastActive = table.Column<DateTime>(nullable: false),
                    MinLifetime = table.Column<long>(nullable: false),
                    MaxLifetime = table.Column<long>(nullable: false),
                    CreationTimeout = table.Column<long>(nullable: false),
                    MinInstances = table.Column<int>(nullable: false),
                    MaxInstances = table.Column<int>(nullable: false),
                    SystemMaxInstances = table.Column<int>(nullable: false),
                    ApplicationVersion = table.Column<string>(maxLength: 256, nullable: true),
                    BinariesVersion = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoBookTaskReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: false),
                    Region = table.Column<string>(maxLength: 64, nullable: false),
                    BinariesVersion = table.Column<string>(maxLength: 64, nullable: false),
                    Version = table.Column<string>(maxLength: 64, nullable: false),
                    FullName = table.Column<string>(maxLength: 64, nullable: false),
                    InstanceType = table.Column<string>(maxLength: 64, nullable: false),
                    StorageSizeGB = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookTaskReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutopilotRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuleId = table.Column<int>(nullable: false),
                    RuleTypeId = table.Column<int>(nullable: false),
                    FlexibilityLevelId = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    LoosenBit = table.Column<int>(nullable: false),
                    LoosenLot = table.Column<int>(nullable: false),
                    TightenBit = table.Column<int>(nullable: false),
                    TightenLot = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutopilotRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutopilotSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DefaultFlexibilityLevelId = table.Column<int>(nullable: false),
                    ScenariosToGenerate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutopilotSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AWSInstanceConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InstanceType = table.Column<string>(maxLength: 256, nullable: false),
                    StorageSizeGb = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AWSInstanceConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingPositionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "Breaks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomId = table.Column<int>(nullable: false),
                    ScheduledDate = table.Column<DateTime>(nullable: false),
                    BroadcastDate = table.Column<DateTime>(nullable: true),
                    ClockHour = table.Column<int>(nullable: true),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    BreakType = table.Column<string>(maxLength: 32, nullable: true),
                    Duration = table.Column<long>(nullable: false),
                    ReserveDuration = table.Column<TimeSpan>(nullable: false),
                    Avail = table.Column<long>(nullable: false),
                    OptimizerAvail = table.Column<long>(nullable: false),
                    Optimize = table.Column<bool>(nullable: false),
                    ExternalBreakRef = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    ExternalProgRef = table.Column<string>(maxLength: 64, nullable: true),
                    PositionInProg = table.Column<int>(nullable: false),
                    BreakPrice = table.Column<double>(nullable: false),
                    FloorRate = table.Column<double>(nullable: false),
                    Solus = table.Column<bool>(nullable: false),
                    PremiumCategory = table.Column<string>(nullable: true),
                    AllowSplit = table.Column<bool>(nullable: false),
                    NationalRegionalSplit = table.Column<bool>(nullable: false),
                    ExcludePackages = table.Column<bool>(nullable: false),
                    BonusAllowed = table.Column<bool>(nullable: false),
                    LongForm = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BRSConfigurationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRSConfigurationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IncludeConversionIndex = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    Demographic = table.Column<string>(maxLength: 64, nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    Product = table.Column<string>(maxLength: 64, nullable: true),
                    RevenueBudget = table.Column<double>(nullable: false),
                    TargetRatings = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    ActualRatings = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CampaignGroup = table.Column<string>(maxLength: 32, nullable: true),
                    IsPercentage = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    DeliveryType = table.Column<int>(nullable: false),
                    DeliveryCurrency = table.Column<int>(nullable: false),
                    BusinessType = table.Column<string>(maxLength: 32, nullable: true),
                    IncludeOptimisation = table.Column<bool>(nullable: false),
                    TargetZeroRatedBreaks = table.Column<bool>(nullable: false),
                    InefficientSpotRemoval = table.Column<bool>(nullable: false),
                    IncludeRightSizer = table.Column<bool>(nullable: false),
                    RightSizerLevel = table.Column<int>(nullable: true),
                    ExpectedClearanceCode = table.Column<string>(maxLength: 64, nullable: true),
                    CampaignPassPriority = table.Column<int>(nullable: false),
                    CampaignSpotMaxRatings = table.Column<int>(nullable: false),
                    StopBooking = table.Column<bool>(nullable: false),
                    ActiveLength = table.Column<string>(maxLength: 2147483647, nullable: true),
                    RatingsDifferenceExcludingPayback = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    ValueDifference = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    ValueDifferenceExcludingPayback = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    AchievedPercentageTargetRatings = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    AchievedPercentageRevenueBudget = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    TargetXP = table.Column<double>(nullable: true),
                    RevenueBooked = table.Column<double>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: true),
                    AutomatedBooked = table.Column<bool>(nullable: true),
                    TopTail = table.Column<int>(nullable: true),
                    Spots = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignExternalId = table.Column<string>(nullable: true),
                    InefficientSpotRemoval = table.Column<bool>(nullable: false),
                    IncludeRightSizer = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clashes",
                columns: table => new
                {
                    Uid = table.Column<Guid>(nullable: false),
                    Externalref = table.Column<string>(maxLength: 64, nullable: true),
                    ParentExternalidentifier = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    DefaultPeakExposureCount = table.Column<int>(nullable: false),
                    DefaultOffPeakExposureCount = table.Column<int>(nullable: false),
                    TokenizedName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clashes", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "ClashExceptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    FromType = table.Column<int>(nullable: false),
                    ToType = table.Column<int>(nullable: false),
                    IncludeOrExclude = table.Column<int>(nullable: false),
                    FromValue = table.Column<string>(maxLength: 64, nullable: true),
                    ToValue = table.Column<string>(maxLength: 64, nullable: true),
                    ExternalRef = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashExceptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClearanceCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClearanceCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryCappingGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 256, nullable: false),
                    Percentage = table.Column<int>(nullable: false),
                    ApplyToPrice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryCappingGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demographics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExternalRef = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ShortName = table.Column<string>(maxLength: 128, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Gameplan = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demographics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EfficiencySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EfficiencyCalculationPeriod = table.Column<int>(nullable: false),
                    DefaultNumberOfWeeks = table.Column<int>(nullable: true),
                    PersistEfficiency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfficiencySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailAuditEventSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventTypeId = table.Column<int>(nullable: false),
                    EmailCreatorId = table.Column<string>(maxLength: 64, nullable: true),
                    NotificationSettings_Enabled = table.Column<bool>(nullable: false),
                    NotificationSettings_SenderAddress = table.Column<string>(maxLength: 256, nullable: true),
                    NotificationSettings_RecipientAddresses = table.Column<string>(nullable: true),
                    NotificationSettings_CCAddresses = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAuditEventSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 32, nullable: false),
                    Description = table.Column<string>(maxLength: 64, nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Failures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Failures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FaultTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlexibilityLevels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SortIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlexibilityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FunctionalAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionalAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    DemographicNo = table.Column<string>(maxLength: 64, nullable: true),
                    BaseDemographicNo = table.Column<string>(maxLength: 64, nullable: true),
                    BreakScheduleDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    InventoryCode = table.Column<string>(type: "CHAR(10)", nullable: false),
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
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InventoryCode = table.Column<string>(type: "CHAR(10)", nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    System = table.Column<string>(type: "CHAR(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ISRGlobalSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExcludeSpotsBookedByProgrammeRequirements = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISRGlobalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ISRSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    DefaultEfficiencyThreshold = table.Column<int>(nullable: false),
                    BreakType = table.Column<string>(maxLength: 64, nullable: true),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    ExcludePublicHolidays = table.Column<bool>(nullable: false),
                    ExcludeSchoolHolidays = table.Column<bool>(nullable: false),
                    SelectableDays = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISRSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KPIComparisonConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KPIName = table.Column<string>(maxLength: 256, nullable: true),
                    DiscernibleDifference = table.Column<float>(nullable: false),
                    HigherIsBest = table.Column<bool>(nullable: false),
                    Ranked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIComparisonConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KPIPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    WeightingFactor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandmarkRunQueues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandmarkRunQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Alpha3b = table.Column<string>(maxLength: 32, nullable: true),
                    Alpha2 = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LengthFactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    Duration = table.Column<long>(nullable: false),
                    Factor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LengthFactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibrarySalesAreaPassPriorities",
                columns: table => new
                {
                    Uid = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 0),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    DowPattern = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibrarySalesAreaPassPriorities", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "LockTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LockType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetadataCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MSTeamsAuditEventSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventTypeId = table.Column<int>(nullable: false),
                    MessageCreatorId = table.Column<string>(maxLength: 64, nullable: false),
                    PostMessageSettings_Enabled = table.Column<bool>(nullable: false),
                    PostMessageSettings_Url = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MSTeamsAuditEventSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutputFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileId = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    AutoBookFileName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsLibraried = table.Column<bool>(nullable: true),
                    TokenizedName = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    ExternalIdentifier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PipelineAuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    EventTypeId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineAuditEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PredictionSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false),
                    ScheduleDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    uid = table.Column<Guid>(nullable: false),
                    Externalidentifier = table.Column<string>(maxLength: 64, nullable: true),
                    ParentExternalidentifier = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    EffectiveStartDate = table.Column<DateTime>(nullable: false),
                    EffectiveEndDate = table.Column<DateTime>(nullable: false),
                    ClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    ReportingCategory = table.Column<string>(maxLength: 256, nullable: true),
                    TokenizedName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.uid);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeCategoryHierarchy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    ExternalRef = table.Column<string>(maxLength: 64, nullable: true),
                    ParentExternalRef = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeCategoryHierarchy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeDictionaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExternalReference = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Classification = table.Column<string>(maxLength: 64, nullable: true),
                    TokenizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeDictionaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammesClassifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammesClassifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(maxLength: 64, nullable: false),
                    ExternalSpotRef = table.Column<string>(maxLength: 64, nullable: true),
                    ExternalCampaignNumber = table.Column<string>(maxLength: 64, nullable: true),
                    SpotLength = table.Column<long>(nullable: false),
                    Product = table.Column<string>(maxLength: 64, nullable: true),
                    Demographic = table.Column<string>(maxLength: 64, nullable: true),
                    BreakBookingPosition = table.Column<int>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    ExternalProgrammeReference = table.Column<string>(maxLength: 64, nullable: true),
                    ProgrammeName = table.Column<string>(maxLength: 128, nullable: true),
                    BreakType = table.Column<string>(maxLength: 32, nullable: true),
                    NominalPrice = table.Column<double>(nullable: false),
                    SpotRating = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    SpotEfficiency = table.Column<double>(nullable: false),
                    RatingPoints = table.Column<double>(nullable: false),
                    Action = table.Column<string>(maxLength: 1, nullable: true),
                    Processor = table.Column<string>(maxLength: 64, nullable: true),
                    ProcessorDateTime = table.Column<DateTime>(nullable: false),
                    GroupCode = table.Column<string>(maxLength: 64, nullable: true),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    ClientPicked = table.Column<bool>(nullable: false),
                    MultipartSpot = table.Column<string>(maxLength: 64, nullable: true),
                    MultipartSpotPosition = table.Column<string>(maxLength: 64, nullable: true),
                    MultipartSpotRef = table.Column<string>(nullable: true),
                    RequestedPositionInBreak = table.Column<string>(maxLength: 64, nullable: true),
                    ActualPositionInBreak = table.Column<string>(maxLength: 64, nullable: true),
                    ExternalBreakNo = table.Column<string>(maxLength: 64, nullable: true),
                    Filler = table.Column<bool>(nullable: false),
                    Sponsored = table.Column<bool>(nullable: false),
                    Preemptable = table.Column<bool>(nullable: false),
                    Preemptlevel = table.Column<int>(nullable: false),
                    PassSequence = table.Column<int>(nullable: false),
                    PassIterationSequence = table.Column<int>(nullable: false),
                    PassName = table.Column<string>(maxLength: 256, nullable: true),
                    OptimiserPassSequenceNumber = table.Column<int>(nullable: false),
                    CampaignPassPriority = table.Column<int>(nullable: false),
                    RankOfEfficiency = table.Column<long>(nullable: false),
                    RankOfCampaign = table.Column<int>(nullable: false),
                    CampaignWeighting = table.Column<double>(nullable: false),
                    SpotSequenceNumber = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    RestrictionDays = table.Column<string>(maxLength: 7, nullable: true),
                    SchoolHolidayIndicator = table.Column<int>(nullable: false),
                    PublicHolidayIndicator = table.Column<int>(nullable: false),
                    LiveProgrammeIndicator = table.Column<int>(nullable: false),
                    RestrictionType = table.Column<int>(nullable: false),
                    RestrictionBasis = table.Column<int>(nullable: false),
                    ExternalProgRef = table.Column<string>(maxLength: 64, nullable: true),
                    ProgrammeCategory = table.Column<string>(maxLength: 64, nullable: true),
                    ProgrammeClassification = table.Column<string>(maxLength: 64, nullable: true),
                    ProgrammeClassificationIndicator = table.Column<int>(nullable: false),
                    TimeToleranceMinsBefore = table.Column<int>(nullable: false),
                    TimeToleranceMinsAfter = table.Column<int>(nullable: false),
                    IndexType = table.Column<int>(nullable: false),
                    IndexThreshold = table.Column<int>(nullable: false),
                    ProductCode = table.Column<string>(nullable: true),
                    ClashCode = table.Column<string>(maxLength: 64, nullable: true),
                    ClearanceCode = table.Column<string>(maxLength: 64, nullable: true),
                    ClockNumber = table.Column<string>(maxLength: 64, nullable: false),
                    ExternalIdentifier = table.Column<string>(maxLength: 64, nullable: true),
                    EpisodeNumber = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    FileId = table.Column<string>(maxLength: 64, nullable: false),
                    IsCompressed = table.Column<bool>(nullable: false),
                    FileContent = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RSGlobalSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExcludeSpotsBookedByProgrammeRequirements = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSGlobalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RSSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuleId = table.Column<int>(nullable: false),
                    RuleTypeId = table.Column<int>(nullable: false),
                    InternalType = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: false),
                    Type = table.Column<string>(maxLength: 64, nullable: false),
                    CampaignType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    AllowedForAutopilot = table.Column<bool>(nullable: false),
                    IsCustom = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<long>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<long>(nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(nullable: false),
                    ExecuteStartedDateTime = table.Column<DateTime>(nullable: true),
                    IsLocked = table.Column<bool>(nullable: false),
                    Real = table.Column<bool>(nullable: false),
                    Smooth = table.Column<bool>(nullable: false),
                    SmoothDateStart = table.Column<DateTime>(nullable: false),
                    SmoothDateEnd = table.Column<DateTime>(nullable: false),
                    ISR = table.Column<bool>(nullable: false),
                    ISRDateStart = table.Column<DateTime>(nullable: false),
                    ISRDateEnd = table.Column<DateTime>(nullable: false),
                    Optimisation = table.Column<bool>(nullable: false),
                    OptimisationDateStart = table.Column<DateTime>(nullable: false),
                    OptimisationDateEnd = table.Column<DateTime>(nullable: false),
                    RightSizer = table.Column<bool>(nullable: false),
                    RightSizerDateStart = table.Column<DateTime>(nullable: false),
                    RightSizerDateEnd = table.Column<DateTime>(nullable: false),
                    SpreadProgramming = table.Column<bool>(nullable: false),
                    SkipLockedBreaks = table.Column<bool>(nullable: false),
                    IgnorePremiumCategoryBreaks = table.Column<bool>(nullable: false),
                    ExcludeBankHolidays = table.Column<bool>(nullable: false),
                    ExcludeSchoolHolidays = table.Column<bool>(nullable: false),
                    IncludeEfficiencyFactor = table.Column<bool>(nullable: true),
                    Objectives = table.Column<string>(nullable: true),
                    EfficiencyPeriod = table.Column<int>(nullable: false),
                    NumberOfWeeks = table.Column<int>(nullable: true),
                    PositionInProgramme = table.Column<int>(nullable: false),
                    IgnoreZeroPercentageSplit = table.Column<bool>(nullable: false),
                    BookTargetArea = table.Column<bool>(nullable: false),
                    FailureTypes = table.Column<string>(maxLength: 2147483647, nullable: true),
                    Manual = table.Column<bool>(nullable: false),
                    RunStatus = table.Column<int>(nullable: false),
                    CreatedOrExecuteDateTime = table.Column<DateTime>(nullable: false),
                    BRSConfigurationTemplateId = table.Column<int>(nullable: false),
                    RunTypeId = table.Column<int>(nullable: false),
                    TokenizedName = table.Column<string>(maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RunTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Hidden = table.Column<bool>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    DefaultKPI = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    ShortName = table.Column<string>(maxLength: 255, nullable: true),
                    CurrencyCode = table.Column<string>(maxLength: 64, nullable: true),
                    BaseDemographic1 = table.Column<string>(maxLength: 64, nullable: true),
                    BaseDemographic2 = table.Column<string>(maxLength: 64, nullable: true),
                    TargetAreaName = table.Column<string>(maxLength: 512, nullable: true),
                    UniverseId = table.Column<int>(nullable: false),
                    StartOffset = table.Column<long>(nullable: false),
                    DayDuration = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAreas", x => x.Id);
                    table.UniqueConstraint("AK_SalesAreas_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignFailures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    ExternalCampaignId = table.Column<string>(maxLength: 64, nullable: true),
                    SalesAreaGroup = table.Column<string>(maxLength: 64, nullable: true),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    Length = table.Column<long>(nullable: false),
                    MultipartNo = table.Column<int>(nullable: false),
                    StrikeWeightStartDate = table.Column<DateTime>(nullable: false),
                    StrikeWeightEndDate = table.Column<DateTime>(nullable: false),
                    DayPartStartTime = table.Column<long>(nullable: false),
                    DayPartEndTime = table.Column<long>(nullable: false),
                    DayPartDays = table.Column<string>(maxLength: 7, nullable: true),
                    FailureType = table.Column<int>(nullable: false),
                    FailureCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignFailures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    CampaignExternalId = table.Column<string>(maxLength: 64, nullable: true),
                    TotalSpots = table.Column<int>(nullable: false),
                    ZeroRatedSpots = table.Column<int>(nullable: false),
                    NominalValue = table.Column<double>(nullable: false),
                    TotalNominalValue = table.Column<double>(nullable: false),
                    DifferenceValueDelivered = table.Column<double>(nullable: false),
                    DifferenceValueDeliveredPercentage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    CampaignExternalId = table.Column<string>(maxLength: 64, nullable: true),
                    SalesAreaName = table.Column<string>(maxLength: 128, nullable: true),
                    SpotLength = table.Column<int>(nullable: false),
                    StrikeWeightStartDate = table.Column<DateTime>(nullable: false),
                    StrikeWeightEndDate = table.Column<DateTime>(nullable: false),
                    DaypartName = table.Column<string>(maxLength: 512, nullable: true),
                    TargetRatings = table.Column<double>(nullable: false),
                    PreRunRatings = table.Column<double>(nullable: false),
                    ISRCancelledRatings = table.Column<double>(nullable: false),
                    ISRCancelledSpots = table.Column<double>(nullable: false),
                    RSCancelledRatings = table.Column<double>(nullable: false),
                    RSCancelledSpots = table.Column<double>(nullable: false),
                    OptimiserRatings = table.Column<double>(nullable: false),
                    OptimiserBookedSpots = table.Column<double>(nullable: false),
                    PassThatDelivered100Percent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    TimeCompleted = table.Column<DateTime>(nullable: false),
                    BRSIndicator = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    CustomId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    DateUserModified = table.Column<DateTime>(nullable: false),
                    IsAutopilot = table.Column<bool>(nullable: false),
                    IsLibraried = table.Column<bool>(nullable: true),
                    TokenizedName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<string>(nullable: false),
                    RestrictionCheckEnabled = table.Column<bool>(nullable: false),
                    ClashExceptionCheckEnabled = table.Column<bool>(nullable: false),
                    ExternalCampaignRefsToExclude = table.Column<string>(nullable: false),
                    RecommendationsForExcludedCampaigns = table.Column<bool>(nullable: false),
                    SmoothFailuresForExcludedCampaigns = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmoothFailureMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothFailureMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmoothFailures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(nullable: true),
                    ExternalSpotRef = table.Column<string>(nullable: true),
                    ExternalBreakRef = table.Column<string>(nullable: true),
                    BreakDateTime = table.Column<DateTime>(nullable: false),
                    SpotLength = table.Column<long>(nullable: false),
                    ExternalCampaignRef = table.Column<string>(nullable: true),
                    CampaignName = table.Column<string>(nullable: true),
                    CampaignGroup = table.Column<string>(nullable: true),
                    AdvertiserIdentifier = table.Column<string>(nullable: true),
                    AdvertiserName = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ClashCode = table.Column<string>(nullable: true),
                    ClashDescription = table.Column<string>(nullable: true),
                    IndustryCode = table.Column<string>(nullable: true),
                    ClearanceCode = table.Column<string>(nullable: true),
                    RestrictionStartDate = table.Column<DateTime>(nullable: true),
                    RestrictionEndDate = table.Column<DateTime>(nullable: true),
                    RestrictionStartTime = table.Column<long>(nullable: true),
                    RestrictionEndTime = table.Column<long>(nullable: true),
                    RestrictionDays = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothFailures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sponsorships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<Guid>(nullable: false),
                    ExternalReferenceId = table.Column<string>(maxLength: 64, nullable: true),
                    RestrictionLevel = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsorships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpotBookingRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SpotLength = table.Column<long>(nullable: false),
                    MinBreakLength = table.Column<long>(nullable: false),
                    MaxBreakLength = table.Column<long>(nullable: false),
                    MaxSpots = table.Column<int>(nullable: false),
                    BreakType = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotBookingRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpotPlacements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    ExternalSpotRef = table.Column<string>(maxLength: 64, nullable: true),
                    ExternalBreakRef = table.Column<string>(maxLength: 64, nullable: true),
                    ResetExternalBreakRef = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotPlacements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Uid = table.Column<Guid>(nullable: false),
                    ExternalCampaignNumber = table.Column<string>(maxLength: 64, nullable: true),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    GroupCode = table.Column<string>(maxLength: 32, nullable: true),
                    ExternalSpotRef = table.Column<string>(maxLength: 64, nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    SpotLength = table.Column<long>(nullable: false),
                    BreakType = table.Column<string>(maxLength: 32, nullable: true),
                    Product = table.Column<string>(maxLength: 64, nullable: true),
                    Demographic = table.Column<string>(maxLength: 64, nullable: true),
                    ClientPicked = table.Column<bool>(nullable: false),
                    MultipartSpot = table.Column<string>(maxLength: 16, nullable: true),
                    MultipartSpotPosition = table.Column<string>(maxLength: 16, nullable: true),
                    MultipartSpotRef = table.Column<string>(nullable: true),
                    RequestedPositioninBreak = table.Column<string>(maxLength: 16, nullable: true),
                    ActualPositioninBreak = table.Column<string>(maxLength: 16, nullable: true),
                    BreakRequest = table.Column<string>(maxLength: 32, nullable: true),
                    ExternalBreakNo = table.Column<string>(maxLength: 64, nullable: true),
                    Sponsored = table.Column<bool>(nullable: false),
                    Preemptable = table.Column<bool>(nullable: false),
                    Preemptlevel = table.Column<int>(nullable: false),
                    BookingPosition = table.Column<int>(nullable: false),
                    IndustryCode = table.Column<string>(maxLength: 32, nullable: true),
                    ClearanceCode = table.Column<string>(maxLength: 64, nullable: true),
                    NominalPrice = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandardDayPartGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "TenantSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DefaultScenarioId = table.Column<Guid>(nullable: false),
                    DefaultSalesAreaPassPriorityId = table.Column<Guid>(nullable: false),
                    AutoBookTargetedZeroRatedBreaks = table.Column<double>(nullable: false),
                    PeakStartTime = table.Column<long>(nullable: false),
                    PeakEndTime = table.Column<long>(nullable: false),
                    MidnightStartTime = table.Column<long>(nullable: false),
                    MidnightEndTime = table.Column<long>(nullable: false),
                    StartDayOfWeek = table.Column<int>(nullable: false),
                    SystemLogicalDate = table.Column<string>(nullable: true),
                    Debug = table.Column<bool>(nullable: false),
                    NoOfRatingsPerSalesDayDemo = table.Column<int>(nullable: false),
                    OpenAirtimeFactor = table.Column<double>(nullable: true),
                    MinimumRunSizeDocumentRestrictionBreaks = table.Column<int>(nullable: false),
                    MinimumRunSizeDocumentRestrictionSpots = table.Column<int>(nullable: false),
                    MinimumRunSizeDocumentRestrictionProgrammes = table.Column<int>(nullable: false),
                    MinimumDocumentRestrictionCampaigns = table.Column<int>(nullable: false),
                    MinimumDocumentRestrictionClashes = table.Column<int>(nullable: false),
                    MinimumDocumentRestrictionClearanceCodes = table.Column<int>(nullable: false),
                    MinimumDocumentRestrictionDemographics = table.Column<int>(nullable: false),
                    MinimumDocumentRestrictionProducts = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "Universes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    Demographic = table.Column<string>(maxLength: 64, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    UniverseValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgAvals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    OpenAvail = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgAvals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgAvals_AutoBookDefaultParameters_AutoBookDefaultParametersId",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgCampaignProgrammes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgCampaignProgrammeRequirement_Required = table.Column<double>(nullable: false),
                    AgCampaignProgrammeRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgCampaignProgrammeRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgCampaignProgrammeRequirement_Supplied = table.Column<double>(nullable: false),
                    NbrCategoryOrProgrammeList = table.Column<int>(nullable: false),
                    NbrSalesAreas = table.Column<int>(nullable: false),
                    SalesAreas = table.Column<string>(nullable: true),
                    NumberTimeBands = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgCampaignProgrammes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgCampaignProgrammes_AutoBookDefaultParameters_AutoBookDefau~",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgCampaignSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    ChannelGroupNo = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    RevenuePercentage = table.Column<double>(nullable: false),
                    MultiPartOnly = table.Column<int>(nullable: false),
                    AgCampaignSalesAreaPtrRef_SalesAreaNo = table.Column<int>(nullable: false),
                    AgCampaignSalesAreaPtrRef_ClassId = table.Column<int>(nullable: false),
                    AgSalesAreaCampaignRequirement_Required = table.Column<double>(nullable: false),
                    AgSalesAreaCampaignRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgSalesAreaCampaignRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgSalesAreaCampaignRequirement_Supplied = table.Column<double>(nullable: false),
                    NbrAgLengths = table.Column<int>(nullable: false),
                    MaxBreaks = table.Column<int>(nullable: false),
                    NbrAgStrikeWeights = table.Column<int>(nullable: false),
                    NbrAgDayParts = table.Column<int>(nullable: false),
                    NbrParts = table.Column<int>(nullable: false),
                    NbrPartsLengths = table.Column<int>(nullable: false),
                    CentreBreakRatio = table.Column<int>(nullable: false),
                    EndBreakRatio = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgCampaignSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgCampaignSalesAreas_AutoBookDefaultParameters_AutoBookDefau~",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgHfssDemos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    IndexType = table.Column<int>(nullable: false),
                    BaseDemoNo = table.Column<int>(nullable: false),
                    IndexDemoNo = table.Column<int>(nullable: false),
                    BreakScheduledDate = table.Column<string>(maxLength: 32, nullable: true),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgHfssDemos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgHfssDemos_AutoBookDefaultParameters_AutoBookDefaultParamet~",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: false),
                    DemographicNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    NoOfRtgs = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgPredictions_AutoBookDefaultParameters_AutoBookDefaultParam~",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgRegionalBreaks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookDefaultParametersId = table.Column<Guid>(nullable: false),
                    TregNo = table.Column<int>(nullable: false),
                    OpenAvail = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgRegionalBreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgRegionalBreaks_AutoBookDefaultParameters_AutoBookDefaultPa~",
                        column: x => x.AutoBookDefaultParametersId,
                        principalTable: "AutoBookDefaultParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutoBookInstanceConfigurationCriterias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AutoBookInstanceConfigurationId = table.Column<int>(nullable: false),
                    MaxDays = table.Column<int>(nullable: true),
                    MaxSalesAreas = table.Column<int>(nullable: true),
                    MaxDemographics = table.Column<int>(nullable: true),
                    MaxCampaigns = table.Column<int>(nullable: true),
                    MaxBreaks = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookInstanceConfigurationCriterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoBookInstanceConfigurationCriterias_AutoBookInstanceConfi~",
                        column: x => x.AutoBookInstanceConfigurationId,
                        principalTable: "AutoBookInstanceConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutoBookTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    AutoBookId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoBookTasks_AutoBooks_AutoBookId",
                        column: x => x.AutoBookId,
                        principalTable: "AutoBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionGroupAssociations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        name: "FK_PositionGroupAssociations_BookingPositionGroups_BookingPosit~",
                        column: x => x.BookingPositionGroupId,
                        principalTable: "BookingPositionGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BreaksEfficiencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BreakId = table.Column<Guid>(nullable: false),
                    Demographic = table.Column<string>(maxLength: 64, nullable: false),
                    Efficiency = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreaksEfficiencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreaksEfficiencies_Breaks_BreakId",
                        column: x => x.BreakId,
                        principalTable: "Breaks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BRSConfigurationForKPIs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BRSConfigurationTemplateId = table.Column<int>(nullable: false),
                    KPIName = table.Column<string>(maxLength: 50, nullable: false),
                    PriorityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRSConfigurationForKPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BRSConfigurationForKPIs_BRSConfigurationTemplates_BRSConfigu~",
                        column: x => x.BRSConfigurationTemplateId,
                        principalTable: "BRSConfigurationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBookingPositionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "CampaignBreakRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBreakRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBreakRequirements_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBreakTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBreakTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBreakTypes_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignPaybacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPaybacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignPaybacks_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignProgrammeRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    IsCategoryOrProgramme = table.Column<int>(nullable: false),
                    IsIncludeOrExclude = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignProgrammeRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignProgrammeRestrictions_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSalesAreaTargets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    StopBooking = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSalesAreaTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSalesAreaTargets_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTimeRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    IsIncludeOrExclude = table.Column<int>(nullable: false),
                    DowPattern = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTimeRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTimeRestrictions_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClashDifferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClashId = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    PeakExposureCount = table.Column<int>(nullable: true),
                    OffPeakExposureCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashDifferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClashDifferences_Clashes_ClashId",
                        column: x => x.ClashId,
                        principalTable: "Clashes",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClashExceptionsTimeAndDows",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClashExceptionId = table.Column<int>(nullable: false),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    DaysOfWeek = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashExceptionsTimeAndDows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClashExceptionsTimeAndDows_ClashExceptions_ClashExceptionId",
                        column: x => x.ClashExceptionId,
                        principalTable: "ClashExceptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FailureItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FailureId = table.Column<int>(nullable: false),
                    Campaign = table.Column<long>(nullable: false),
                    CampaignName = table.Column<string>(maxLength: 256, nullable: true),
                    ExternalId = table.Column<string>(maxLength: 256, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Failures = table.Column<long>(nullable: false),
                    SalesAreaName = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailureItems_Failures_FailureId",
                        column: x => x.FailureId,
                        principalTable: "Failures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaultTypeDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LanguageAbbreviation = table.Column<string>(maxLength: 3, nullable: false),
                    ShortName = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(nullable: false),
                    FaultTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultTypeDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaultTypeDescriptions_FaultTypes_FaultTypeId",
                        column: x => x.FaultTypeId,
                        principalTable: "FaultTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunctionalAreaDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LanguageAbbreviation = table.Column<string>(maxLength: 3, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    FunctionalAreaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionalAreaDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FunctionalAreaDescriptions_FunctionalAreas_FunctionalAreaId",
                        column: x => x.FunctionalAreaId,
                        principalTable: "FunctionalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunctionalAreaFaultTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FunctionalAreaId = table.Column<Guid>(nullable: false),
                    FaultTypeId = table.Column<int>(nullable: false),
                    IsSelected = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionalAreaFaultTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FunctionalAreaFaultTypes_FaultTypes_FaultTypeId",
                        column: x => x.FaultTypeId,
                        principalTable: "FaultTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunctionalAreaFaultTypes_FunctionalAreas_FunctionalAreaId",
                        column: x => x.FunctionalAreaId,
                        principalTable: "FunctionalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTypeLockTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "ISRSettingsDemographics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DemographicId = table.Column<string>(maxLength: 64, nullable: true),
                    EfficiencyThreshold = table.Column<int>(nullable: false),
                    ISRSettingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISRSettingsDemographics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ISRSettingsDemographics_ISRSettings_ISRSettingId",
                        column: x => x.ISRSettingId,
                        principalTable: "ISRSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesAreaPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LibrarySalesAreaPassPriorityUid = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 256, nullable: true),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAreaPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAreaPriorities_LibrarySalesAreaPassPriorities_LibrarySa~",
                        column: x => x.LibrarySalesAreaPassPriorityUid,
                        principalTable: "LibrarySalesAreaPassPriorities",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetadataValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    ValueId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetadataValues_MetadataCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MetadataCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutputFileColumns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OutputFileId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    DataType = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputFileColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutputFileColumns_OutputFiles_OutputFileId",
                        column: x => x.OutputFileId,
                        principalTable: "OutputFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassBreakExclusions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    SelectableDays = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassBreakExclusions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassBreakExclusions_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassProgrammeRepetitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    Minutes = table.Column<int>(nullable: false),
                    Factor = table.Column<double>(nullable: false),
                    PeakFactor = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassProgrammeRepetitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassProgrammeRepetitions_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassRatingPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    SalesAreas = table.Column<string>(nullable: true),
                    OffPeakValue = table.Column<double>(nullable: true),
                    PeakValue = table.Column<double>(nullable: true),
                    MidnightToDawnValue = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassRatingPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassRatingPoints_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    RuleId = table.Column<int>(nullable: false),
                    InternalType = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Value = table.Column<string>(maxLength: 256, nullable: true),
                    Type = table.Column<string>(maxLength: 64, nullable: true),
                    Discriminator = table.Column<int>(nullable: false),
                    Ignore = table.Column<bool>(nullable: true),
                    PeakValue = table.Column<string>(maxLength: 64, nullable: true),
                    CampaignType = table.Column<int>(nullable: true),
                    Under = table.Column<int>(nullable: true),
                    Over = table.Column<int>(nullable: true),
                    ForceOverUnder = table.Column<int>(nullable: true),
                    BookTargetArea = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassRules_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PassRules_Passes_PassId1",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PassRules_Passes_PassId2",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PassRules_Passes_PassId3",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassSalesAreaPriorityCollection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    IsPeakTime = table.Column<bool>(nullable: false, defaultValue: false),
                    IsOffPeakTime = table.Column<bool>(nullable: false, defaultValue: false),
                    IsMidnightTime = table.Column<bool>(nullable: false, defaultValue: false),
                    AreDatesRetained = table.Column<bool>(nullable: false),
                    AreTimesRetained = table.Column<bool>(nullable: false),
                    DaysOfWeek = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassSalesAreaPriorityCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassSalesAreaPriorityCollection_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassSlottingLimits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    Demographic = table.Column<string>(maxLength: 256, nullable: false),
                    MinimumEfficiency = table.Column<int>(nullable: false),
                    MaximumEfficiency = table.Column<int>(nullable: false),
                    BandingTolerance = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassSlottingLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassSlottingLimits_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionScheduleRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PredictionScheduleId = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Demographic = table.Column<string>(maxLength: 64, nullable: false),
                    NoOfRatings = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionScheduleRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionScheduleRatings_PredictionSchedules_PredictionSche~",
                        column: x => x.PredictionScheduleId,
                        principalTable: "PredictionSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAdvertisers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<Guid>(nullable: false),
                    AdvertiserId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdvertisers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisers_Advertisers_AdvertiserId",
                        column: x => x.AdvertiserId,
                        principalTable: "Advertisers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisers_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAgencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<Guid>(nullable: false),
                    AgencyId = table.Column<int>(nullable: false),
                    AgencyGroupId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAgencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAgencies_AgencyGroups_AgencyGroupId",
                        column: x => x.AgencyGroupId,
                        principalTable: "AgencyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAgencies_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAgencies_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPersons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<Guid>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPersons_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPersons_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeEpisodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Number = table.Column<int>(nullable: false),
                    ProgrammeDictionaryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeEpisodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammeEpisodes_ProgrammeDictionaries_ProgrammeDictionaryId",
                        column: x => x.ProgrammeDictionaryId,
                        principalTable: "ProgrammeDictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestrictionsSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestrictionId = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictionsSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestrictionsSalesAreas_Restrictions_RestrictionId",
                        column: x => x.RestrictionId,
                        principalTable: "Restrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RSSettingsDefaultDeliverySettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RSSettingId = table.Column<int>(nullable: false),
                    DaysToCampaignEnd = table.Column<int>(nullable: false),
                    UpperLimitOfOverDelivery = table.Column<int>(nullable: false),
                    LowerLimitOfOverDelivery = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSSettingsDefaultDeliverySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RSSettingsDefaultDeliverySettings_RSSettings_RSSettingId",
                        column: x => x.RSSettingId,
                        principalTable: "RSSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RSSettingsDemographicsSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RSSettingId = table.Column<int>(nullable: false),
                    DemographicId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSSettingsDemographicsSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RSSettingsDemographicsSettings_RSSettings_RSSettingId",
                        column: x => x.RSSettingId,
                        principalTable: "RSSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunAnalysisGroupTargets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    AnalysisGroupTargetId = table.Column<Guid>(nullable: false),
                    AnalysisGroupId = table.Column<int>(nullable: false),
                    KPI = table.Column<string>(maxLength: 64, nullable: false),
                    Target = table.Column<double>(nullable: false),
                    SortIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunAnalysisGroupTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunAnalysisGroupTargets_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunAuthors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunAuthors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunAuthors_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunCampaignProcessesSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    ExternalId = table.Column<string>(maxLength: 64, nullable: false),
                    InefficientSpotRemoval = table.Column<bool>(nullable: true),
                    IncludeRightSizer = table.Column<bool>(nullable: true),
                    RightSizerLevel = table.Column<int>(nullable: true),
                    DeliveryCappingGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunCampaignProcessesSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunCampaignProcessesSettings_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunCampaignReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    ExternalId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunCampaignReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunCampaignReferences_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunExcludedInventoryStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    InventoryCode = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunExcludedInventoryStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunExcludedInventoryStatuses_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunInventoryLocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    Locked = table.Column<bool>(nullable: false),
                    ChosenScenarioId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunInventoryLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunInventoryLocks_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunSalesAreaPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunSalesAreaPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunSalesAreaPriorities_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QueueName = table.Column<string>(maxLength: 64, nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 512, nullable: true),
                    RunId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunScheduleSettings_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunLandmarkScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunTypeId = table.Column<int>(nullable: false),
                    SendToLandmarkAutomatically = table.Column<bool>(nullable: false),
                    QueueName = table.Column<string>(maxLength: 64, nullable: true),
                    DaysOfWeek = table.Column<string>(maxLength: 7, nullable: false),
                    ScheduledTime = table.Column<long>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunLandmarkScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunLandmarkScheduleSettings_RunTypes_RunTypeId",
                        column: x => x.RunTypeId,
                        principalTable: "RunTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunTypeAnalysisGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunTypeId = table.Column<int>(nullable: false),
                    AnalysisGroupId = table.Column<int>(nullable: false),
                    KPI = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunTypeAnalysisGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunTypeAnalysisGroups_AnalysisGroups_AnalysisGroupId",
                        column: x => x.AnalysisGroupId,
                        principalTable: "AnalysisGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RunTypeAnalysisGroups_RunTypes_RunTypeId",
                        column: x => x.RunTypeId,
                        principalTable: "RunTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesAreaDemographics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 255, nullable: false),
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
                name: "SalesAreasChannelGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesAreaId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAreasChannelGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAreasChannelGroups_SalesAreas_SalesAreaId",
                        column: x => x.SalesAreaId,
                        principalTable: "SalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesAreasHolidays",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalesAreaId = table.Column<Guid>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAreasHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAreasHolidays_SalesAreas_SalesAreaId",
                        column: x => x.SalesAreaId,
                        principalTable: "SalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisGroupTargetMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioResultId = table.Column<int>(nullable: false),
                    AnalysisGroupTargetId = table.Column<Guid>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisGroupTargetMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisGroupTargetMetrics_ScenarioResults_ScenarioResultId",
                        column: x => x.ScenarioResultId,
                        principalTable: "ScenarioResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioResultMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioResultId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    DisplayFormat = table.Column<string>(maxLength: 32, nullable: false),
                    Value = table.Column<double>(nullable: false),
                    ResultSource = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioResultMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioResultMetrics_ScenarioResults_ScenarioResultId",
                        column: x => x.ScenarioResultId,
                        principalTable: "ScenarioResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunScenarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    StartedDateTime = table.Column<DateTime>(nullable: true),
                    CompletedDateTime = table.Column<DateTime>(nullable: true),
                    Progress = table.Column<string>(maxLength: 32, nullable: true),
                    Status = table.Column<short>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Order = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunScenarios_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RunScenarios_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignPassPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignPassPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCampaignPassPriorities_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignPriorityRoundCollections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    ContainsInclusionRound = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignPriorityRoundCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCampaignPriorityRoundCollections_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioPassReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassId = table.Column<int>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioPassReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioPassReferences_Passes_PassId",
                        column: x => x.PassId,
                        principalTable: "Passes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioPassReferences_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleBreaks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomId = table.Column<int>(nullable: false),
                    ScheduledDate = table.Column<DateTime>(nullable: false),
                    BroadcastDate = table.Column<DateTime>(nullable: true),
                    ClockHour = table.Column<int>(nullable: true),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    BreakType = table.Column<string>(maxLength: 32, nullable: true),
                    Duration = table.Column<long>(nullable: false),
                    ReserveDuration = table.Column<long>(nullable: false),
                    Avail = table.Column<long>(nullable: false),
                    OptimizerAvail = table.Column<long>(nullable: false),
                    Optimize = table.Column<bool>(nullable: false),
                    ExternalBreakRef = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    ExternalProgRef = table.Column<string>(maxLength: 64, nullable: true),
                    PositionInProg = table.Column<int>(nullable: false),
                    ScheduleId = table.Column<int>(nullable: false),
                    BreakPrice = table.Column<double>(nullable: false),
                    FloorRate = table.Column<double>(nullable: false),
                    Solus = table.Column<bool>(nullable: false),
                    PremiumCategory = table.Column<string>(nullable: true),
                    AllowSplit = table.Column<bool>(nullable: false),
                    NationalRegionalSplit = table.Column<bool>(nullable: false),
                    ExcludePackages = table.Column<bool>(nullable: false),
                    BonusAllowed = table.Column<bool>(nullable: false),
                    LongForm = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleBreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleBreaks_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationBestBreakFactorGroupRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothConfigurationId = table.Column<int>(nullable: false),
                    SpotsCriteriaHasSponsoredSpots = table.Column<bool>(nullable: true),
                    SpotsCriteriaHasFIBORLIBRequests = table.Column<bool>(nullable: true),
                    SpotsCriteriaHasBreakRequest = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationBestBreakFactorGroupRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationBestBreakFactorGroupRecords_SmoothConfigu~",
                        column: x => x.SmoothConfigurationId,
                        principalTable: "SmoothConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationSmoothPasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothConfigurationId = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    SmoothPassType = table.Column<int>(nullable: false),
                    Sponsored = table.Column<bool>(nullable: true),
                    HasMultipartSpots = table.Column<bool>(nullable: true),
                    Preemptable = table.Column<bool>(nullable: true),
                    BreakRequests = table.Column<string>(nullable: true),
                    HasProductClashCode = table.Column<bool>(nullable: true),
                    CanSplitMultipartSpots = table.Column<bool>(nullable: true),
                    HasSpotEndTime = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationSmoothPasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationSmoothPasses_SmoothConfigurations_SmoothC~",
                        column: x => x.SmoothConfigurationId,
                        principalTable: "SmoothConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationSmoothPassIterationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothConfigurationId = table.Column<int>(nullable: false),
                    SpotsCriteriaHasSponsoredSpots = table.Column<bool>(nullable: true),
                    SpotsCriteriaHasFIBORLIBRequests = table.Column<bool>(nullable: true),
                    SpotsCriteriaHasBreakRequest = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationSmoothPassIterationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationSmoothPassIterationRecords_SmoothConfigur~",
                        column: x => x.SmoothConfigurationId,
                        principalTable: "SmoothConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothDiagnosticConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothConfigurationId = table.Column<int>(nullable: false),
                    SpotSalesAreas = table.Column<string>(nullable: false),
                    SpotDemographics = table.Column<string>(nullable: false),
                    SpotExternalRefs = table.Column<string>(nullable: false),
                    SpotExternalCampaignRefs = table.Column<string>(nullable: false),
                    SpotMultipartSpots = table.Column<string>(nullable: false),
                    SpotMinStartTime = table.Column<DateTime>(nullable: true),
                    SpotMaxStartTime = table.Column<DateTime>(nullable: true),
                    SpotMinPreemptLevel = table.Column<int>(nullable: true),
                    SpotMaxPreemptLevel = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothDiagnosticConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothDiagnosticConfigurations_SmoothConfigurations_SmoothCo~",
                        column: x => x.SmoothConfigurationId,
                        principalTable: "SmoothConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothFailureMessageDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothFailureMessageId = table.Column<int>(nullable: false),
                    LanguageAbbreviation = table.Column<string>(maxLength: 3, nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothFailureMessageDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothFailureMessageDescriptions_SmoothFailureMessages_Smoot~",
                        column: x => x.SmoothFailureMessageId,
                        principalTable: "SmoothFailureMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothFailuresSmoothFailureMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothFailureId = table.Column<int>(nullable: false),
                    SmoothFailureMessageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothFailuresSmoothFailureMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothFailuresSmoothFailureMessages_SmoothFailures_SmoothFai~",
                        column: x => x.SmoothFailureId,
                        principalTable: "SmoothFailures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsoredItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsorshipId = table.Column<int>(nullable: false),
                    CalculationType = table.Column<int>(nullable: false),
                    RestrictionType = table.Column<int>(nullable: true),
                    RestrictionValue = table.Column<int>(nullable: true),
                    Applicability = table.Column<int>(nullable: true),
                    Products = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsoredItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsoredItems_Sponsorships_SponsorshipId",
                        column: x => x.SponsorshipId,
                        principalTable: "Sponsorships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpotBookingRuleSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SpotBookingRuleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotBookingRuleSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpotBookingRuleSalesAreas_SpotBookingRules_SpotBookingRuleId",
                        column: x => x.SpotBookingRuleId,
                        principalTable: "SpotBookingRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandardDayPartSplits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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

            migrationBuilder.CreateTable(
                name: "TenantFeatureSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdValue = table.Column<string>(maxLength: 256, nullable: true),
                    TenantSettingsId = table.Column<Guid>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Settings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantFeatureSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantFeatureSettings_TenantSettings_TenantSettingsId",
                        column: x => x.TenantSettingsId,
                        principalTable: "TenantSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantRunEventSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantSettingsId = table.Column<Guid>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    EmailNotificationEnabled = table.Column<bool>(nullable: false),
                    EmailNotificationSenderAddress = table.Column<string>(maxLength: 256, nullable: true),
                    EmailNotificationRecipientAddresses = table.Column<string>(nullable: true),
                    EmailNotificationCCAddresses = table.Column<string>(nullable: true),
                    HTTPNotificationEnabled = table.Column<bool>(nullable: false),
                    HTTPNotificationMethod = table.Column<string>(maxLength: 16, nullable: true),
                    HTTPNotificationURL = table.Column<string>(nullable: true),
                    HTTPNotificationContentTemplate = table.Column<string>(nullable: true),
                    HTTPNotificationHeaders = table.Column<string>(nullable: true),
                    HTTPNotificationSucccessStatusCodes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRunEventSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRunEventSettings_TenantSettings_TenantSettingsId",
                        column: x => x.TenantSettingsId,
                        principalTable: "TenantSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantWebhookSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantSettingsId = table.Column<Guid>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    HTTPNotificationEnabled = table.Column<bool>(nullable: false),
                    HTTPNotificationMethod = table.Column<string>(maxLength: 16, nullable: true),
                    HTTPNotificationURL = table.Column<string>(nullable: true),
                    HTTPNotificationContentTemplate = table.Column<string>(nullable: true),
                    HTTPNotificationHeaders = table.Column<string>(nullable: true),
                    HTTPNotificationSucccessStatusCodes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantWebhookSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantWebhookSettings_TenantSettings_TenantSettingsId",
                        column: x => x.TenantSettingsId,
                        principalTable: "TenantSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgCampaignProgrammeProgrammeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignProgrammeId = table.Column<int>(nullable: false),
                    ProgrammeNumber = table.Column<int>(nullable: false),
                    CategoryNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgCampaignProgrammeProgrammeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgCampaignProgrammeProgrammeCategories_AgCampaignProgrammes_~",
                        column: x => x.AgCampaignProgrammeId,
                        principalTable: "AgCampaignProgrammes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgTimeBands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignProgrammeId = table.Column<int>(nullable: false),
                    StartTime = table.Column<string>(maxLength: 32, nullable: true),
                    EndTime = table.Column<string>(maxLength: 32, nullable: true),
                    Days = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgTimeBands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgTimeBands_AgCampaignProgrammes_AgCampaignProgrammeId",
                        column: x => x.AgCampaignProgrammeId,
                        principalTable: "AgCampaignProgrammes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgDayParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignSalesAreaId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    DayPartNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgDayPartRequirement_Required = table.Column<double>(nullable: false),
                    AgDayPartRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgDayPartRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgDayPartRequirement_Supplied = table.Column<double>(nullable: false),
                    NbrAgTimeSlices = table.Column<int>(nullable: false),
                    SpotMaxRatings = table.Column<int>(nullable: false),
                    NbrAgDayPartLengths = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgDayParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgDayParts_AgCampaignSalesAreas_AgCampaignSalesAreaId",
                        column: x => x.AgCampaignSalesAreaId,
                        principalTable: "AgCampaignSalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignSalesAreaId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    SpotLength = table.Column<int>(nullable: false),
                    MultiPartNo = table.Column<int>(nullable: false),
                    NoOfSpots = table.Column<int>(nullable: false),
                    PriceFactor = table.Column<double>(nullable: false),
                    AgLengthRequirement_Required = table.Column<double>(nullable: false),
                    AgLengthRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgLengthRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgLengthRequirement_Supplied = table.Column<double>(nullable: false),
                    NbrAgMultiParts = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgLengths_AgCampaignSalesAreas_AgCampaignSalesAreaId",
                        column: x => x.AgCampaignSalesAreaId,
                        principalTable: "AgCampaignSalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgPartLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignSalesAreaId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    DayPartType = table.Column<int>(nullable: false),
                    DayPartNo = table.Column<int>(nullable: false),
                    SpotLength = table.Column<long>(nullable: false),
                    MultipartNumber = table.Column<int>(nullable: false),
                    AgPartLengthRequirement_Required = table.Column<double>(nullable: false),
                    AgPartLengthRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgPartLengthRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgPartLengthRequirement_Supplied = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgPartLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgPartLengths_AgCampaignSalesAreas_AgCampaignSalesAreaId",
                        column: x => x.AgCampaignSalesAreaId,
                        principalTable: "AgCampaignSalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignSalesAreaId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    DayPartNo = table.Column<int>(nullable: false),
                    AgPartRequirement_Required = table.Column<double>(nullable: false),
                    AgPartRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgPartRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgPartRequirement_Supplied = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgParts_AgCampaignSalesAreas_AgCampaignSalesAreaId",
                        column: x => x.AgCampaignSalesAreaId,
                        principalTable: "AgCampaignSalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgStrikeWeights",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgCampaignSalesAreaId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    NbrAgStrikeWeightLengths = table.Column<int>(nullable: false),
                    SpotMaxRatings = table.Column<int>(nullable: false),
                    AgStikeWeightRequirement_Required = table.Column<double>(nullable: false),
                    AgStikeWeightRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgStikeWeightRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgStikeWeightRequirement_Supplied = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgStrikeWeights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgStrikeWeights_AgCampaignSalesAreas_AgCampaignSalesAreaId",
                        column: x => x.AgCampaignSalesAreaId,
                        principalTable: "AgCampaignSalesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBookingPositionGroupSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignBookingPositionGroupId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBookingPositionGroupSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBookingPositionGroupSalesAreas_CampaignBookingPositi~",
                        column: x => x.CampaignBookingPositionGroupId,
                        principalTable: "CampaignBookingPositionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBreakRequirementItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignBreakRequirementId = table.Column<int>(nullable: false),
                    CurrentPercentageSplit = table.Column<double>(nullable: false),
                    DesiredPercentageSplit = table.Column<double>(nullable: false),
                    Discriminator = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBreakRequirementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBreakRequirementItems_CampaignBreakRequirements_Camp~",
                        column: x => x.CampaignBreakRequirementId,
                        principalTable: "CampaignBreakRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignBreakRequirementItems_CampaignBreakRequirements_Cam~1",
                        column: x => x.CampaignBreakRequirementId,
                        principalTable: "CampaignBreakRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignProgrammeRestrictionsCategoryOrProgramme",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignProgrammeRestrictionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignProgrammeRestrictionsCategoryOrProgramme", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignProgrammeRestrictionsCategoryOrProgramme_CampaignPro~",
                        column: x => x.CampaignProgrammeRestrictionId,
                        principalTable: "CampaignProgrammeRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignProgrammeRestrictionsSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignProgrammeRestrictionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignProgrammeRestrictionsSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignProgrammeRestrictionsSalesAreas_CampaignProgrammeRes~",
                        column: x => x.CampaignProgrammeRestrictionId,
                        principalTable: "CampaignProgrammeRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSalesAreaTargetGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignSalesAreaTargetId = table.Column<int>(nullable: false),
                    GroupName = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSalesAreaTargetGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSalesAreaTargetGroups_CampaignSalesAreaTargets_Campa~",
                        column: x => x.CampaignSalesAreaTargetId,
                        principalTable: "CampaignSalesAreaTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSalesAreaTargetMultiparts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignSalesAreaTargetId = table.Column<int>(nullable: false),
                    MultipartNumber = table.Column<int>(nullable: false),
                    DesiredPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CurrentPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSalesAreaTargetMultiparts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSalesAreaTargetMultiparts_CampaignSalesAreaTargets_C~",
                        column: x => x.CampaignSalesAreaTargetId,
                        principalTable: "CampaignSalesAreaTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignSalesAreaTargetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_CampaignSalesAreaTargets_CampaignSalesAreaTa~",
                        column: x => x.CampaignSalesAreaTargetId,
                        principalTable: "CampaignSalesAreaTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTimeRestrictionsSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignTimeRestrictionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTimeRestrictionsSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTimeRestrictionsSalesAreas_CampaignTimeRestrictions_~",
                        column: x => x.CampaignTimeRestrictionId,
                        principalTable: "CampaignTimeRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClashDifferenceTimeAndDows",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClashDifferenceId = table.Column<int>(nullable: false),
                    StartTime = table.Column<long>(nullable: true),
                    EndTime = table.Column<long>(nullable: true),
                    DaysOfWeek = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashDifferenceTimeAndDows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClashDifferenceTimeAndDows_ClashDifferences_ClashDifferenceId",
                        column: x => x.ClashDifferenceId,
                        principalTable: "ClashDifferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassSalesAreaPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassSalesAreaPriorityCollectionId = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassSalesAreaPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassSalesAreaPriorities_PassSalesAreaPriorityCollection_Pass~",
                        column: x => x.PassSalesAreaPriorityCollectionId,
                        principalTable: "PassSalesAreaPriorityCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Programmes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PrgtNo = table.Column<int>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<long>(nullable: false),
                    LiveBroadcast = table.Column<bool>(nullable: false),
                    ProgrammeDictionaryId = table.Column<int>(nullable: false),
                    EpisodeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programmes_ProgrammeEpisodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "ProgrammeEpisodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programmes_ProgrammeDictionaries_ProgrammeDictionaryId",
                        column: x => x.ProgrammeDictionaryId,
                        principalTable: "ProgrammeDictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RSSettingsDemographicsDeliverySettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RSSettingsDemographicsSettingId = table.Column<int>(nullable: false),
                    DaysToCampaignEnd = table.Column<int>(nullable: false),
                    UpperLimitOfOverDelivery = table.Column<int>(nullable: false),
                    LowerLimitOfOverDelivery = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSSettingsDemographicsDeliverySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RSSettingsDemographicsDeliverySettings_RSSettingsDemographic~",
                        column: x => x.RSSettingsDemographicsSettingId,
                        principalTable: "RSSettingsDemographicsSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalRunInfo",
                columns: table => new
                {
                    RunScenarioId = table.Column<int>(nullable: false),
                    ExternalRunId = table.Column<Guid>(nullable: false),
                    ExternalStatus = table.Column<int>(nullable: false),
                    ExternalStatusModifiedDate = table.Column<DateTime>(nullable: false),
                    QueueName = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: true),
                    ScheduledDateTime = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    CreatorId = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalRunInfo", x => x.RunScenarioId);
                    table.ForeignKey(
                        name: "FK_ExternalRunInfo_RunScenarios_RunScenarioId",
                        column: x => x.RunScenarioId,
                        principalTable: "RunScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCompactCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioCampaignPassPriorityId = table.Column<int>(nullable: false),
                    Uid = table.Column<Guid>(nullable: false),
                    CustomId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 256, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ExternalId = table.Column<string>(maxLength: 256, nullable: true),
                    CampaignGroup = table.Column<string>(maxLength: 256, nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    ProductExternalRef = table.Column<string>(maxLength: 256, nullable: true),
                    ProductName = table.Column<string>(maxLength: 256, nullable: true),
                    AdvertiserName = table.Column<string>(maxLength: 256, nullable: true),
                    AgencyName = table.Column<string>(maxLength: 256, nullable: true),
                    BusinessType = table.Column<string>(maxLength: 256, nullable: true),
                    DeliveryType = table.Column<int>(nullable: false),
                    Demographic = table.Column<string>(maxLength: 256, nullable: true),
                    RevenueBudget = table.Column<double>(nullable: false),
                    TargetRatings = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    ActualRatings = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    IsPercentage = table.Column<bool>(nullable: false),
                    IncludeOptimisation = table.Column<bool>(nullable: false),
                    TargetZeroRatedBreaks = table.Column<bool>(nullable: false),
                    InefficientSpotRemoval = table.Column<bool>(nullable: false),
                    IncludeRightSizer = table.Column<int>(nullable: false),
                    DefaultCampaignPassPriority = table.Column<int>(nullable: false),
                    ClashCode = table.Column<string>(maxLength: 256, nullable: true),
                    ClashDescription = table.Column<string>(maxLength: 256, nullable: true),
                    AgencyGroupShortName = table.Column<string>(maxLength: 256, nullable: true),
                    AgencyGroupCode = table.Column<string>(maxLength: 256, nullable: true),
                    ReportingCategory = table.Column<string>(maxLength: 256, nullable: true),
                    SalesExecutiveName = table.Column<string>(maxLength: 256, nullable: true),
                    StopBooking = table.Column<bool>(nullable: false),
                    TargetXP = table.Column<double>(nullable: true),
                    RevenueBooked = table.Column<double>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: true),
                    AutomatedBooked = table.Column<bool>(nullable: true),
                    TopTail = table.Column<int>(nullable: true),
                    Spots = table.Column<int>(nullable: true),
                    ActiveLength = table.Column<string>(maxLength: 2147483647, nullable: true),
                    RatingsDifferenceExcludingPayback = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    ValueDifference = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    ValueDifferenceExcludingPayback = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    AchievedPercentageTargetRatings = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true),
                    AchievedPercentageRevenueBudget = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCompactCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCompactCampaigns_ScenarioCampaignPassPriorities_Scen~",
                        column: x => x.ScenarioCampaignPassPriorityId,
                        principalTable: "ScenarioCampaignPassPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioPassPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioCampaignPassPriorityId = table.Column<int>(nullable: false),
                    PassId = table.Column<int>(nullable: false),
                    PassName = table.Column<string>(maxLength: 256, nullable: true),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioPassPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioPassPriorities_ScenarioCampaignPassPriorities_Scenar~",
                        column: x => x.ScenarioCampaignPassPriorityId,
                        principalTable: "ScenarioCampaignPassPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignPriorityRounds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioCampaignPriorityRoundCollectionId = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    PriorityFrom = table.Column<int>(nullable: false),
                    PriorityTo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignPriorityRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCampaignPriorityRounds_ScenarioCampaignPriorityRound~",
                        column: x => x.ScenarioCampaignPriorityRoundCollectionId,
                        principalTable: "ScenarioCampaignPriorityRoundCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleBreakEfficiencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScheduleBreakId = table.Column<Guid>(nullable: false),
                    Demographic = table.Column<string>(maxLength: 64, nullable: false),
                    Efficiency = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleBreakEfficiencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleBreakEfficiencies_ScheduleBreaks_ScheduleBreakId",
                        column: x => x.ScheduleBreakId,
                        principalTable: "ScheduleBreaks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothBestBreakFactorGroupRecordPassSequences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestBreakFactorGroupRecordId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothBestBreakFactorGroupRecordPassSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothBestBreakFactorGroupRecordPassSequences_SmoothConfigur~",
                        column: x => x.BestBreakFactorGroupRecordId,
                        principalTable: "SmoothConfigurationBestBreakFactorGroupRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationBestBreakFactorGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestBreakFactorGroupRecordId = table.Column<int>(nullable: false),
                    Evaluation = table.Column<byte>(nullable: false),
                    SameBreakGroupScoreAction = table.Column<byte>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationBestBreakFactorGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationBestBreakFactorGroups_SmoothConfiguration~",
                        column: x => x.BestBreakFactorGroupRecordId,
                        principalTable: "SmoothConfigurationBestBreakFactorGroupRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothPassDefaultIterations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothPassIterationRecordId = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    RespectCampaignClash = table.Column<bool>(nullable: false),
                    ProductClashRules = table.Column<byte>(nullable: false),
                    RespectSpotTime = table.Column<bool>(nullable: false),
                    BreakPositionRules = table.Column<byte>(nullable: false),
                    RequestedPositionInBreakRules = table.Column<byte>(nullable: false),
                    RespectRestrictions = table.Column<bool>(nullable: false),
                    RespectClashExceptions = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothPassDefaultIterations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothPassDefaultIterations_SmoothConfigurationSmoothPassIte~",
                        column: x => x.SmoothPassIterationRecordId,
                        principalTable: "SmoothConfigurationSmoothPassIterationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothPassIterationRecordPassSequences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothPassIterationRecordId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothPassIterationRecordPassSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothPassIterationRecordPassSequences_SmoothConfigurationSm~",
                        column: x => x.SmoothPassIterationRecordId,
                        principalTable: "SmoothConfigurationSmoothPassIterationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothPassUnplacedIterations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SmoothPassIterationRecordId = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    RespectSpotTime = table.Column<bool>(nullable: false),
                    RespectCampaignClash = table.Column<bool>(nullable: false),
                    ProductClashRule = table.Column<byte>(nullable: false),
                    RespectRestrictions = table.Column<bool>(nullable: false),
                    RespectClashExceptions = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothPassUnplacedIterations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothPassUnplacedIterations_SmoothConfigurationSmoothPassIt~",
                        column: x => x.SmoothPassIterationRecordId,
                        principalTable: "SmoothConfigurationSmoothPassIterationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsorshipAdvertiserExclusivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsoredItemId = table.Column<int>(nullable: false),
                    AdvertiserIdentifier = table.Column<string>(maxLength: 64, nullable: true),
                    RestrictionType = table.Column<int>(nullable: true),
                    RestrictionValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorshipAdvertiserExclusivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorshipAdvertiserExclusivities_SponsoredItems_SponsoredI~",
                        column: x => x.SponsoredItemId,
                        principalTable: "SponsoredItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsorshipClashExclusivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsoredItemId = table.Column<int>(nullable: false),
                    ClashExternalRef = table.Column<string>(maxLength: 64, nullable: true),
                    RestrictionType = table.Column<int>(nullable: true),
                    RestrictionValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorshipClashExclusivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorshipClashExclusivities_SponsoredItems_SponsoredItemId",
                        column: x => x.SponsoredItemId,
                        principalTable: "SponsoredItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsorshipItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsoredItemId = table.Column<int>(nullable: false),
                    SalesAreas = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ProgrammeName = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorshipItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorshipItems_SponsoredItems_SponsoredItemId",
                        column: x => x.SponsoredItemId,
                        principalTable: "SponsoredItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgDayPartLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgDayPartId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    DayPartType = table.Column<int>(nullable: false),
                    DayPartNo = table.Column<int>(nullable: false),
                    SpotLength = table.Column<long>(nullable: false),
                    MultipartNumber = table.Column<int>(nullable: false),
                    AgPartLengthRequirement_Required = table.Column<double>(nullable: false),
                    AgPartLengthRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgPartLengthRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgPartLengthRequirement_Supplied = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgDayPartLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgDayPartLengths_AgDayParts_AgDayPartId",
                        column: x => x.AgDayPartId,
                        principalTable: "AgDayParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgTimeSlices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgDayPartId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    DayPartNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    StartDay = table.Column<int>(nullable: false),
                    EndDay = table.Column<int>(nullable: false),
                    StartTime = table.Column<string>(maxLength: 32, nullable: true),
                    EndTime = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgTimeSlices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgTimeSlices_AgDayParts_AgDayPartId",
                        column: x => x.AgDayPartId,
                        principalTable: "AgDayParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgMultiParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgLengthId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    MultiPartNo = table.Column<int>(nullable: false),
                    SeqNo = table.Column<int>(nullable: false),
                    SpotLength = table.Column<int>(nullable: false),
                    BookingPosition = table.Column<int>(nullable: false),
                    PriceFactor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgMultiParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgMultiParts_AgLengths_AgLengthId",
                        column: x => x.AgLengthId,
                        principalTable: "AgLengths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgStrikeWeightLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgStrikeWeightId = table.Column<int>(nullable: false),
                    CampaignNo = table.Column<int>(nullable: false),
                    SalesAreaNo = table.Column<int>(nullable: false),
                    SpotLength = table.Column<int>(nullable: false),
                    MultiPartNo = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 32, nullable: true),
                    EndDate = table.Column<string>(maxLength: 32, nullable: true),
                    AgStrikeWeightLengthRequirement_Required = table.Column<double>(nullable: false),
                    AgStrikeWeightLengthRequirement_TgtRequired = table.Column<double>(nullable: false),
                    AgStrikeWeightLengthRequirement_SareRequired = table.Column<double>(nullable: false),
                    AgStrikeWeightLengthRequirement_Supplied = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgStrikeWeightLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgStrikeWeightLengths_AgStrikeWeights_AgStrikeWeightId",
                        column: x => x.AgStrikeWeightId,
                        principalTable: "AgStrikeWeights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSalesAreaTargetGroupSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignSalesAreaTargetGroupId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSalesAreaTargetGroupSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSalesAreaTargetGroupSalesAreas_CampaignSalesAreaTarg~",
                        column: x => x.CampaignSalesAreaTargetGroupId,
                        principalTable: "CampaignSalesAreaTargetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSalesAreaTargetMultipartLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignSalesAreaTargetMultipartId = table.Column<int>(nullable: false),
                    Length = table.Column<long>(nullable: false),
                    BookingPosition = table.Column<int>(nullable: false),
                    Sequencing = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSalesAreaTargetMultipartLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSalesAreaTargetMultipartLengths_CampaignSalesAreaTar~",
                        column: x => x.CampaignSalesAreaTargetMultipartId,
                        principalTable: "CampaignSalesAreaTargetMultiparts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargetStrikeWeights",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignTargetId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    DesiredPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CurrentPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    SpotMaxRatings = table.Column<int>(nullable: false),
                    Payback = table.Column<double>(nullable: true),
                    RevenueBudget = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargetStrikeWeights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargetStrikeWeights_CampaignTargets_CampaignTargetId",
                        column: x => x.CampaignTargetId,
                        principalTable: "CampaignTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeCategoryLinks",
                columns: table => new
                {
                    ProgrammeId = table.Column<Guid>(nullable: false),
                    ProgrammeCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeCategoryLinks", x => new { x.ProgrammeId, x.ProgrammeCategoryId });
                    table.ForeignKey(
                        name: "FK_ProgrammeCategoryLinks_ProgrammeCategories_ProgrammeCategory~",
                        column: x => x.ProgrammeCategoryId,
                        principalTable: "ProgrammeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgrammeCategoryLinks_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleProgrammes",
                columns: table => new
                {
                    ProgrammeId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleProgrammes", x => x.ProgrammeId);
                    table.ForeignKey(
                        name: "FK_ScheduleProgrammes_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleProgrammes_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCompactCampaignPaybacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioCompactCampaignId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCompactCampaignPaybacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCompactCampaignPaybacks_ScenarioCompactCampaigns_Sce~",
                        column: x => x.ScenarioCompactCampaignId,
                        principalTable: "ScenarioCompactCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationBestBreakFactorGroupItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestBreakFactorGroupId = table.Column<int>(nullable: false),
                    AllFilterFactorsMustBeNonZero = table.Column<bool>(nullable: false),
                    UseZeroScoresInEvaluation = table.Column<bool>(nullable: false),
                    Evaluation = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationBestBreakFactorGroupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationBestBreakFactorGroupItems_SmoothConfigura~",
                        column: x => x.BestBreakFactorGroupId,
                        principalTable: "SmoothConfigurationBestBreakFactorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationSameBreakGroupScoreFactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestBreakFactorGroupId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Factor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationSameBreakGroupScoreFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationSameBreakGroupScoreFactors_SmoothConfigur~",
                        column: x => x.BestBreakFactorGroupId,
                        principalTable: "SmoothConfigurationBestBreakFactorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsoredDayParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsorshipItemId = table.Column<int>(nullable: false),
                    StartTime = table.Column<long>(nullable: false),
                    EndTime = table.Column<long>(nullable: false),
                    DaysOfWeek = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsoredDayParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsoredDayParts_SponsorshipItems_SponsorshipItemId",
                        column: x => x.SponsorshipItemId,
                        principalTable: "SponsorshipItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargetStrikeWeightDayParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DayPartName = table.Column<string>(maxLength: 256, nullable: true),
                    CampaignTargetStrikeWeightId = table.Column<int>(nullable: false),
                    DesiredPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CurrentPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    SpotMaxRatings = table.Column<int>(nullable: false),
                    CampaignPrice = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    TotalSpotCount = table.Column<int>(nullable: false),
                    ZeroRatedSpotCount = table.Column<int>(nullable: false),
                    Ratings = table.Column<double>(nullable: false),
                    BaseDemographRatings = table.Column<double>(nullable: false),
                    NominalValue = table.Column<double>(nullable: false),
                    Payback = table.Column<double>(nullable: true),
                    RevenueBudget = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargetStrikeWeightDayParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargetStrikeWeightDayParts_CampaignTargetStrikeWeigh~",
                        column: x => x.CampaignTargetStrikeWeightId,
                        principalTable: "CampaignTargetStrikeWeights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargetStrikeWeightLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignTargetStrikeWeightId = table.Column<int>(nullable: false),
                    MultipartNumber = table.Column<int>(nullable: false),
                    Length = table.Column<long>(nullable: false),
                    DesiredPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CurrentPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargetStrikeWeightLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargetStrikeWeightLengths_CampaignTargetStrikeWeight~",
                        column: x => x.CampaignTargetStrikeWeightId,
                        principalTable: "CampaignTargetStrikeWeights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationBestBreakFactorGroupItemDefaultFactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestBreakFactorGroupItemId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Factor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationBestBreakFactorGroupItemDefaultFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationBestBreakFactorGroupItemDefaultFactors_Sm~",
                        column: x => x.BestBreakFactorGroupItemId,
                        principalTable: "SmoothConfigurationBestBreakFactorGroupItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmoothConfigurationBestBreakFactorGroupItemFilterFactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BestBreakFactorGroupItemId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Factor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmoothConfigurationBestBreakFactorGroupItemFilterFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmoothConfigurationBestBreakFactorGroupItemFilterFactors_Smo~",
                        column: x => x.BestBreakFactorGroupItemId,
                        principalTable: "SmoothConfigurationBestBreakFactorGroupItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargetStrikeWeightDayPartLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignTargetStrikeWeightDayPartId = table.Column<int>(nullable: false),
                    Length = table.Column<long>(nullable: false),
                    MultipartNumber = table.Column<int>(nullable: false),
                    DesiredPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false),
                    CurrentPercentageSplit = table.Column<decimal>(type: "DECIMAL(28,18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargetStrikeWeightDayPartLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargetStrikeWeightDayPartLengths_CampaignTargetStrik~",
                        column: x => x.CampaignTargetStrikeWeightDayPartId,
                        principalTable: "CampaignTargetStrikeWeightDayParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargetStrikeWeightDayPartTimeSlices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignTargetStrikeWeightDayPartId = table.Column<int>(nullable: false),
                    FromTime = table.Column<long>(nullable: false),
                    ToTime = table.Column<long>(nullable: false),
                    DowPattern = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargetStrikeWeightDayPartTimeSlices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargetStrikeWeightDayPartTimeSlices_CampaignTargetSt~",
                        column: x => x.CampaignTargetStrikeWeightDayPartId,
                        principalTable: "CampaignTargetStrikeWeightDayParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisers_ExternalIdentifier",
                table: "Advertisers",
                column: "ExternalIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgAvals_AutoBookDefaultParametersId",
                table: "AgAvals",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.CreateIndex(
                name: "IX_AgCampaignProgrammeProgrammeCategories_AgCampaignProgrammeId",
                table: "AgCampaignProgrammeProgrammeCategories",
                column: "AgCampaignProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_AgCampaignProgrammes_AutoBookDefaultParametersId",
                table: "AgCampaignProgrammes",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.CreateIndex(
                name: "IX_AgCampaignSalesAreas_AutoBookDefaultParametersId",
                table: "AgCampaignSalesAreas",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.CreateIndex(
                name: "IX_AgCampaignSalesAreas_Type",
                table: "AgCampaignSalesAreas",
                column: "Type",
                unique: true,
                filter: "Type = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AgDayPartLengths_AgDayPartId",
                table: "AgDayPartLengths",
                column: "AgDayPartId");

            migrationBuilder.CreateIndex(
                name: "IX_AgDayParts_AgCampaignSalesAreaId",
                table: "AgDayParts",
                column: "AgCampaignSalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_ExternalIdentifier",
                table: "Agencies",
                column: "ExternalIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyGroups_ShortName_Code",
                table: "AgencyGroups",
                columns: new[] { "ShortName", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgHfssDemos_AutoBookDefaultParametersId",
                table: "AgHfssDemos",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.CreateIndex(
                name: "IX_AgLengths_AgCampaignSalesAreaId",
                table: "AgLengths",
                column: "AgCampaignSalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgMultiParts_AgLengthId",
                table: "AgMultiParts",
                column: "AgLengthId");

            migrationBuilder.CreateIndex(
                name: "IX_AgPartLengths_AgCampaignSalesAreaId",
                table: "AgPartLengths",
                column: "AgCampaignSalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgParts_AgCampaignSalesAreaId",
                table: "AgParts",
                column: "AgCampaignSalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgPredictions_AutoBookDefaultParametersId",
                table: "AgPredictions",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.CreateIndex(
                name: "IX_AgRegionalBreaks_AutoBookDefaultParametersId",
                table: "AgRegionalBreaks",
                column: "AutoBookDefaultParametersId");

            migrationBuilder.CreateIndex(
                name: "IX_AgStrikeWeightLengths_AgStrikeWeightId",
                table: "AgStrikeWeightLengths",
                column: "AgStrikeWeightId");

            migrationBuilder.CreateIndex(
                name: "IX_AgStrikeWeights_AgCampaignSalesAreaId",
                table: "AgStrikeWeights",
                column: "AgCampaignSalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgTimeBands_AgCampaignProgrammeId",
                table: "AgTimeBands",
                column: "AgCampaignProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_AgTimeSlices_AgDayPartId",
                table: "AgTimeSlices",
                column: "AgDayPartId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisGroups_IsDeleted",
                table: "AnalysisGroups",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisGroupTargetMetrics_ScenarioResultId_AnalysisGroupTar~",
                table: "AnalysisGroupTargetMetrics",
                columns: new[] { "ScenarioResultId", "AnalysisGroupTargetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutoBookInstanceConfigurationCriterias_AutoBookInstanceConfi~",
                table: "AutoBookInstanceConfigurationCriterias",
                column: "AutoBookInstanceConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoBooks_AutoBookId",
                table: "AutoBooks",
                column: "AutoBookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutoBookTaskReports_RunId",
                table: "AutoBookTaskReports",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoBookTaskReports_ScenarioId",
                table: "AutoBookTaskReports",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoBookTasks_AutoBookId",
                table: "AutoBookTasks",
                column: "AutoBookId",
                unique: true);

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
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks",
                column: "ExternalBreakRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ExternalProgRef",
                table: "Breaks",
                column: "ExternalProgRef");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_SalesArea",
                table: "Breaks",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ScheduledDate",
                table: "Breaks",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesArea", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });

            migrationBuilder.CreateIndex(
                name: "IX_BreaksEfficiencies_BreakId",
                table: "BreaksEfficiencies",
                column: "BreakId");

            migrationBuilder.CreateIndex(
                name: "IX_BreaksEfficiencies_Demographic",
                table: "BreaksEfficiencies",
                column: "Demographic");

            migrationBuilder.CreateIndex(
                name: "IX_BRSConfigurationForKPIs_BRSConfigurationTemplateId",
                table: "BRSConfigurationForKPIs",
                column: "BRSConfigurationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessTypes_Code",
                table: "BusinessTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroups_CampaignId",
                table: "CampaignBookingPositionGroups",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_CampaignBookingPositi~",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "CampaignBookingPositionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBookingPositionGroupSalesAreas_Name",
                table: "CampaignBookingPositionGroupSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakRequirementItems_CampaignBreakRequirementId",
                table: "CampaignBreakRequirementItems",
                column: "CampaignBreakRequirementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakRequirementItems_CampaignBreakRequirementId1",
                table: "CampaignBreakRequirementItems",
                column: "CampaignBreakRequirementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakRequirements_CampaignId",
                table: "CampaignBreakRequirements",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakTypes_CampaignId",
                table: "CampaignBreakTypes",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPaybacks_CampaignId",
                table: "CampaignPaybacks",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictions_CampaignId",
                table: "CampaignProgrammeRestrictions",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsCategoryOrProgramme_CampaignPro~",
                table: "CampaignProgrammeRestrictionsCategoryOrProgramme",
                column: "CampaignProgrammeRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsCategoryOrProgramme_Name",
                table: "CampaignProgrammeRestrictionsCategoryOrProgramme",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_CampaignProgrammeRes~",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                column: "CampaignProgrammeRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProgrammeRestrictionsSalesAreas_Name",
                table: "CampaignProgrammeRestrictionsSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CampaignGroup",
                table: "Campaigns",
                column: "CampaignGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CustomId",
                table: "Campaigns",
                column: "CustomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Demographic",
                table: "Campaigns",
                column: "Demographic");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ExpectedClearanceCode",
                table: "Campaigns",
                column: "ExpectedClearanceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ExternalId",
                table: "Campaigns",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Product",
                table: "Campaigns",
                column: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetGroups_CampaignSalesAreaTargetId",
                table: "CampaignSalesAreaTargetGroups",
                column: "CampaignSalesAreaTargetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_CampaignSalesAreaTarg~",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                column: "CampaignSalesAreaTargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetGroupSalesAreas_Name",
                table: "CampaignSalesAreaTargetGroupSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetMultipartLengths_CampaignSalesAreaTar~",
                table: "CampaignSalesAreaTargetMultipartLengths",
                column: "CampaignSalesAreaTargetMultipartId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargetMultiparts_CampaignSalesAreaTargetId",
                table: "CampaignSalesAreaTargetMultiparts",
                column: "CampaignSalesAreaTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargets_CampaignId",
                table: "CampaignSalesAreaTargets",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSalesAreaTargets_SalesArea",
                table: "CampaignSalesAreaTargets",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSettings_CampaignExternalId",
                table: "CampaignSettings",
                column: "CampaignExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_CampaignSalesAreaTargetId",
                table: "CampaignTargets",
                column: "CampaignSalesAreaTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargetStrikeWeightDayPartLengths_CampaignTargetStrik~",
                table: "CampaignTargetStrikeWeightDayPartLengths",
                column: "CampaignTargetStrikeWeightDayPartId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargetStrikeWeightDayParts_CampaignTargetStrikeWeigh~",
                table: "CampaignTargetStrikeWeightDayParts",
                column: "CampaignTargetStrikeWeightId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargetStrikeWeightDayPartTimeSlices_CampaignTargetSt~",
                table: "CampaignTargetStrikeWeightDayPartTimeSlices",
                column: "CampaignTargetStrikeWeightDayPartId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargetStrikeWeightLengths_CampaignTargetStrikeWeight~",
                table: "CampaignTargetStrikeWeightLengths",
                column: "CampaignTargetStrikeWeightId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargetStrikeWeights_CampaignTargetId",
                table: "CampaignTargetStrikeWeights",
                column: "CampaignTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTimeRestrictions_CampaignId",
                table: "CampaignTimeRestrictions",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_CampaignTimeRestrictionId",
                table: "CampaignTimeRestrictionsSalesAreas",
                column: "CampaignTimeRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTimeRestrictionsSalesAreas_Name",
                table: "CampaignTimeRestrictionsSalesAreas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Name",
                table: "Channels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ShortName",
                table: "Channels",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClashDifferences_ClashId",
                table: "ClashDifferences",
                column: "ClashId");

            migrationBuilder.CreateIndex(
                name: "IX_ClashDifferences_SalesArea",
                table: "ClashDifferences",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ClashDifferenceTimeAndDows_ClashDifferenceId",
                table: "ClashDifferenceTimeAndDows",
                column: "ClashDifferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clashes_Description",
                table: "Clashes",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Clashes_Externalref",
                table: "Clashes",
                column: "Externalref");

            migrationBuilder.CreateIndex(
                name: "IX_Clashes_ParentExternalidentifier",
                table: "Clashes",
                column: "ParentExternalidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ClashExceptionsTimeAndDows_ClashExceptionId",
                table: "ClashExceptionsTimeAndDows",
                column: "ClashExceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_ExternalRef",
                table: "Demographics",
                column: "ExternalRef");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAuditEventSettings_EventTypeId",
                table: "EmailAuditEventSettings",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FailureItems_FailureId",
                table: "FailureItems",
                column: "FailureId");

            migrationBuilder.CreateIndex(
                name: "IX_Failures_ScenarioId",
                table: "Failures",
                column: "ScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaultTypeDescriptions_FaultTypeId",
                table: "FaultTypeDescriptions",
                column: "FaultTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FlexibilityLevels_Name",
                table: "FlexibilityLevels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FunctionalAreaDescriptions_FunctionalAreaId",
                table: "FunctionalAreaDescriptions",
                column: "FunctionalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionalAreaFaultTypes_FaultTypeId",
                table: "FunctionalAreaFaultTypes",
                column: "FaultTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FunctionalAreaFaultTypes_FunctionalAreaId",
                table: "FunctionalAreaFaultTypes",
                column: "FunctionalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_BaseDemographicNo",
                table: "IndexTypes",
                column: "BaseDemographicNo");

            migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_DemographicNo",
                table: "IndexTypes",
                column: "DemographicNo");

            migrationBuilder.CreateIndex(
                name: "IX_IndexTypes_SalesArea",
                table: "IndexTypes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTypeLockTypes_InventoryTypeId",
                table: "InventoryTypeLockTypes",
                column: "InventoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ISRSettings_SalesArea",
                table: "ISRSettings",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ISRSettingsDemographics_DemographicId",
                table: "ISRSettingsDemographics",
                column: "DemographicId");

            migrationBuilder.CreateIndex(
                name: "IX_ISRSettingsDemographics_ISRSettingId",
                table: "ISRSettingsDemographics",
                column: "ISRSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_LibrarySalesAreaPassPriorities_Name",
                table: "LibrarySalesAreaPassPriorities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetadataCategories_Name",
                table: "MetadataCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetadataValues_CategoryId",
                table: "MetadataValues",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MSTeamsAuditEventSettings_EventTypeId",
                table: "MSTeamsAuditEventSettings",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputFileColumns_OutputFileId",
                table: "OutputFileColumns",
                column: "OutputFileId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputFiles_FileId",
                table: "OutputFiles",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassBreakExclusions_PassId",
                table: "PassBreakExclusions",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_IsLibraried",
                table: "Passes",
                column: "IsLibraried");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_Name",
                table: "Passes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PassProgrammeRepetitions_PassId",
                table: "PassProgrammeRepetitions",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_PassRatingPoints_PassId",
                table: "PassRatingPoints",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_PassRules_PassId",
                table: "PassRules",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_PassRules_Type",
                table: "PassRules",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_PassRules_PassId1",
                table: "PassRules",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_PassRules_PassId2",
                table: "PassRules",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_PassRules_PassId3",
                table: "PassRules",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_PassSalesAreaPriorities_PassSalesAreaPriorityCollectionId",
                table: "PassSalesAreaPriorities",
                column: "PassSalesAreaPriorityCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PassSalesAreaPriorityCollection_PassId",
                table: "PassSalesAreaPriorityCollection",
                column: "PassId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassSlottingLimits_PassId",
                table: "PassSlottingLimits",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ExternalIdentifier",
                table: "Persons",
                column: "ExternalIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PipelineAuditEvents_RunId",
                table: "PipelineAuditEvents",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineAuditEvents_ScenarioId",
                table: "PipelineAuditEvents",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionGroupAssociations_BookingPositionGroupId",
                table: "PositionGroupAssociations",
                column: "BookingPositionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionGroupAssociations_BookingPosition_BookingPositionGro~",
                table: "PositionGroupAssociations",
                columns: new[] { "BookingPosition", "BookingPositionGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionScheduleRatings_Demographic",
                table: "PredictionScheduleRatings",
                column: "Demographic");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionScheduleRatings_PredictionScheduleId",
                table: "PredictionScheduleRatings",
                column: "PredictionScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesArea",
                table: "PredictionSchedules",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionSchedules_SalesArea_ScheduleDay",
                table: "PredictionSchedules",
                columns: new[] { "SalesArea", "ScheduleDay" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisers_AdvertiserId",
                table: "ProductAdvertisers",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisers_ProductId",
                table: "ProductAdvertisers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisers_ProductId_AdvertiserId_StartDate_EndDate",
                table: "ProductAdvertisers",
                columns: new[] { "ProductId", "AdvertiserId", "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_AgencyGroupId",
                table: "ProductAgencies",
                column: "AgencyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_AgencyId",
                table: "ProductAgencies",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_ProductId",
                table: "ProductAgencies",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgencies_ProductId_AgencyId_AgencyGroupId_StartDate_E~",
                table: "ProductAgencies",
                columns: new[] { "ProductId", "AgencyId", "AgencyGroupId", "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPersons_PersonId",
                table: "ProductPersons",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPersons_ProductId",
                table: "ProductPersons",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPersons_ProductId_PersonId_StartDate_EndDate",
                table: "ProductPersons",
                columns: new[] { "ProductId", "PersonId", "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ClashCode",
                table: "Products",
                column: "ClashCode");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Externalidentifier",
                table: "Products",
                column: "Externalidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ParentExternalidentifier",
                table: "Products",
                column: "ParentExternalidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeCategories_Name",
                table: "ProgrammeCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeCategoryLinks_ProgrammeCategoryId",
                table: "ProgrammeCategoryLinks",
                column: "ProgrammeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeDictionaries_ExternalReference",
                table: "ProgrammeDictionaries",
                column: "ExternalReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeEpisodes_ProgrammeDictionaryId",
                table: "ProgrammeEpisodes",
                column: "ProgrammeDictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_EpisodeId",
                table: "Programmes",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes",
                column: "ProgrammeDictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_SalesArea",
                table: "Programmes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammesClassifications_Code",
                table: "ProgrammesClassifications",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammesClassifications_Uid",
                table: "ProgrammesClassifications",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_Processor",
                table: "Recommendations",
                column: "Processor");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_ScenarioId",
                table: "Recommendations",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_ClashCode",
                table: "Restrictions",
                column: "ClashCode");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_ClearanceCode",
                table: "Restrictions",
                column: "ClearanceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_EndDate",
                table: "Restrictions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_ExternalProgRef",
                table: "Restrictions",
                column: "ExternalProgRef");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_ProgrammeCategory",
                table: "Restrictions",
                column: "ProgrammeCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_ProgrammeClassification",
                table: "Restrictions",
                column: "ProgrammeClassification");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_RestrictionType",
                table: "Restrictions",
                column: "RestrictionType");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_StartDate",
                table: "Restrictions",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_Uid",
                table: "Restrictions",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionsSalesAreas_RestrictionId",
                table: "RestrictionsSalesAreas",
                column: "RestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionsSalesAreas_SalesArea",
                table: "RestrictionsSalesAreas",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ResultFiles_ScenarioId_FileId",
                table: "ResultFiles",
                columns: new[] { "ScenarioId", "FileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RSSettings_SalesArea",
                table: "RSSettings",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_RSSettingsDefaultDeliverySettings_RSSettingId",
                table: "RSSettingsDefaultDeliverySettings",
                column: "RSSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_RSSettingsDemographicsDeliverySettings_RSSettingsDemographic~",
                table: "RSSettingsDemographicsDeliverySettings",
                column: "RSSettingsDemographicsSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_RSSettingsDemographicsSettings_DemographicId",
                table: "RSSettingsDemographicsSettings",
                column: "DemographicId");

            migrationBuilder.CreateIndex(
                name: "IX_RSSettingsDemographicsSettings_RSSettingId",
                table: "RSSettingsDemographicsSettings",
                column: "RSSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTypes_Name",
                table: "RuleTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunAnalysisGroupTargets_AnalysisGroupTargetId",
                table: "RunAnalysisGroupTargets",
                column: "AnalysisGroupTargetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunAnalysisGroupTargets_RunId",
                table: "RunAnalysisGroupTargets",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_RunAuthors_RunId",
                table: "RunAuthors",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunCampaignProcessesSettings_ExternalId",
                table: "RunCampaignProcessesSettings",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_RunCampaignProcessesSettings_RunId",
                table: "RunCampaignProcessesSettings",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_RunCampaignReferences_ExternalId",
                table: "RunCampaignReferences",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_RunCampaignReferences_RunId",
                table: "RunCampaignReferences",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_RunExcludedInventoryStatuses_RunId",
                table: "RunExcludedInventoryStatuses",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_RunInventoryLocks_ChosenScenarioId",
                table: "RunInventoryLocks",
                column: "ChosenScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RunInventoryLocks_RunId",
                table: "RunInventoryLocks",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunLandmarkScheduleSettings_RunTypeId",
                table: "RunLandmarkScheduleSettings",
                column: "RunTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunSalesAreaPriorities_RunId",
                table: "RunSalesAreaPriorities",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_RunSalesAreaPriorities_SalesArea",
                table: "RunSalesAreaPriorities",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_RunScenarios_Order",
                table: "RunScenarios",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_RunScenarios_RunId",
                table: "RunScenarios",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_RunScenarios_ScenarioId",
                table: "RunScenarios",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RunScheduleSettings_RunId",
                table: "RunScheduleSettings",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_AnalysisGroupId",
                table: "RunTypeAnalysisGroups",
                column: "AnalysisGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId",
                table: "RunTypeAnalysisGroups",
                column: "RunTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId_AnalysisGroupId_KPI",
                table: "RunTypeAnalysisGroups",
                columns: new[] { "RunTypeId", "AnalysisGroupId", "KPI" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunTypes_Name",
                table: "RunTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreaDemographics_SalesArea",
                table: "SalesAreaDemographics",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreaPriorities_LibrarySalesAreaPassPriorityUid",
                table: "SalesAreaPriorities",
                column: "LibrarySalesAreaPassPriorityUid");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_BaseDemographic1",
                table: "SalesAreas",
                column: "BaseDemographic1");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_BaseDemographic2",
                table: "SalesAreas",
                column: "BaseDemographic2");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_CustomId",
                table: "SalesAreas",
                column: "CustomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreas_ShortName",
                table: "SalesAreas",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreasChannelGroups_SalesAreaId",
                table: "SalesAreasChannelGroups",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAreasHolidays_SalesAreaId",
                table: "SalesAreasHolidays",
                column: "SalesAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_ExternalCampaignId",
                table: "ScenarioCampaignFailures",
                column: "ExternalCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_SalesArea",
                table: "ScenarioCampaignFailures",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_SalesAreaGroup",
                table: "ScenarioCampaignFailures",
                column: "SalesAreaGroup");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_ScenarioId",
                table: "ScenarioCampaignFailures",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_StrikeWeightEndDate",
                table: "ScenarioCampaignFailures",
                column: "StrikeWeightEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_StrikeWeightStartDate",
                table: "ScenarioCampaignFailures",
                column: "StrikeWeightStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignMetrics_ScenarioId",
                table: "ScenarioCampaignMetrics",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignPassPriorities_ScenarioId",
                table: "ScenarioCampaignPassPriorities",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignPriorityRoundCollections_ScenarioId",
                table: "ScenarioCampaignPriorityRoundCollections",
                column: "ScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignPriorityRounds_ScenarioCampaignPriorityRound~",
                table: "ScenarioCampaignPriorityRounds",
                column: "ScenarioCampaignPriorityRoundCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignResults_ScenarioId",
                table: "ScenarioCampaignResults",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCompactCampaignPaybacks_ScenarioCompactCampaignId",
                table: "ScenarioCompactCampaignPaybacks",
                column: "ScenarioCompactCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCompactCampaigns_ScenarioCampaignPassPriorityId",
                table: "ScenarioCompactCampaigns",
                column: "ScenarioCampaignPassPriorityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPassPriorities_ScenarioCampaignPassPriorityId",
                table: "ScenarioPassPriorities",
                column: "ScenarioCampaignPassPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPassReferences_Order",
                table: "ScenarioPassReferences",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPassReferences_PassId",
                table: "ScenarioPassReferences",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPassReferences_ScenarioId",
                table: "ScenarioPassReferences",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioResultMetrics_ScenarioResultId",
                table: "ScenarioResultMetrics",
                column: "ScenarioResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioResults_ScenarioId",
                table: "ScenarioResults",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_IsLibraried",
                table: "Scenarios",
                column: "IsLibraried");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreakEfficiencies_Demographic",
                table: "ScheduleBreakEfficiencies",
                column: "Demographic");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreakEfficiencies_ScheduleBreakId",
                table: "ScheduleBreakEfficiencies",
                column: "ScheduleBreakId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreakEfficiencies_ScheduleBreakId_Demographic_Effici~",
                table: "ScheduleBreakEfficiencies",
                columns: new[] { "ScheduleBreakId", "Demographic", "Efficiency" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks",
                column: "ExternalBreakRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ExternalProgRef",
                table: "ScheduleBreaks",
                column: "ExternalProgRef");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_SalesArea",
                table: "ScheduleBreaks",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ScheduleId",
                table: "ScheduleBreaks",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ScheduledDate",
                table: "ScheduleBreaks",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleProgrammes_ScheduleId",
                table: "ScheduleProgrammes",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SalesArea",
                table: "Schedules",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Date_SalesArea",
                table: "Schedules",
                columns: new[] { "Date", "SalesArea" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmoothBestBreakFactorGroupRecordPassSequences_BestBreakFacto~",
                table: "SmoothBestBreakFactorGroupRecordPassSequences",
                column: "BestBreakFactorGroupRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationBestBreakFactorGroupItemDefaultFactors_Be~",
                table: "SmoothConfigurationBestBreakFactorGroupItemDefaultFactors",
                column: "BestBreakFactorGroupItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationBestBreakFactorGroupItemFilterFactors_Bes~",
                table: "SmoothConfigurationBestBreakFactorGroupItemFilterFactors",
                column: "BestBreakFactorGroupItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationBestBreakFactorGroupItems_BestBreakFactor~",
                table: "SmoothConfigurationBestBreakFactorGroupItems",
                column: "BestBreakFactorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationBestBreakFactorGroupRecords_SmoothConfigu~",
                table: "SmoothConfigurationBestBreakFactorGroupRecords",
                column: "SmoothConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationBestBreakFactorGroups_BestBreakFactorGrou~",
                table: "SmoothConfigurationBestBreakFactorGroups",
                column: "BestBreakFactorGroupRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationSameBreakGroupScoreFactors_BestBreakFacto~",
                table: "SmoothConfigurationSameBreakGroupScoreFactors",
                column: "BestBreakFactorGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationSmoothPasses_SmoothConfigurationId",
                table: "SmoothConfigurationSmoothPasses",
                column: "SmoothConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothConfigurationSmoothPassIterationRecords_SmoothConfigur~",
                table: "SmoothConfigurationSmoothPassIterationRecords",
                column: "SmoothConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothDiagnosticConfigurations_SmoothConfigurationId",
                table: "SmoothDiagnosticConfigurations",
                column: "SmoothConfigurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmoothFailureMessageDescriptions_SmoothFailureMessageId",
                table: "SmoothFailureMessageDescriptions",
                column: "SmoothFailureMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothFailures_RunId",
                table: "SmoothFailures",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothFailuresSmoothFailureMessages_SmoothFailureId_SmoothFa~",
                table: "SmoothFailuresSmoothFailureMessages",
                columns: new[] { "SmoothFailureId", "SmoothFailureMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmoothPassDefaultIterations_SmoothPassIterationRecordId",
                table: "SmoothPassDefaultIterations",
                column: "SmoothPassIterationRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmoothPassIterationRecordPassSequences_SmoothPassIterationRe~",
                table: "SmoothPassIterationRecordPassSequences",
                column: "SmoothPassIterationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SmoothPassUnplacedIterations_SmoothPassIterationRecordId",
                table: "SmoothPassUnplacedIterations",
                column: "SmoothPassIterationRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredDayParts_SponsorshipItemId",
                table: "SponsoredDayParts",
                column: "SponsorshipItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredItems_SponsorshipId",
                table: "SponsoredItems",
                column: "SponsorshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorshipAdvertiserExclusivities_SponsoredItemId",
                table: "SponsorshipAdvertiserExclusivities",
                column: "SponsoredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorshipClashExclusivities_ClashExternalRef",
                table: "SponsorshipClashExclusivities",
                column: "ClashExternalRef");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorshipClashExclusivities_SponsoredItemId",
                table: "SponsorshipClashExclusivities",
                column: "SponsoredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorshipItems_SponsoredItemId",
                table: "SponsorshipItems",
                column: "SponsoredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsorships_ExternalReferenceId",
                table: "Sponsorships",
                column: "ExternalReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotBookingRuleSalesAreas_SpotBookingRuleId",
                table: "SpotBookingRuleSalesAreas",
                column: "SpotBookingRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotPlacements_ExternalBreakRef",
                table: "SpotPlacements",
                column: "ExternalBreakRef");

            migrationBuilder.CreateIndex(
                name: "IX_SpotPlacements_ExternalSpotRef",
                table: "SpotPlacements",
                column: "ExternalSpotRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpotPlacements_ModifiedTime",
                table: "SpotPlacements",
                column: "ModifiedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ExternalBreakNo",
                table: "Spots",
                column: "ExternalBreakNo");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ExternalCampaignNumber",
                table: "Spots",
                column: "ExternalCampaignNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ExternalSpotRef",
                table: "Spots",
                column: "ExternalSpotRef");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_MultipartSpot",
                table: "Spots",
                column: "MultipartSpot");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_SalesArea",
                table: "Spots",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_StartDateTime",
                table: "Spots",
                column: "StartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_Uid",
                table: "Spots",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandardDayPartSplits_GroupId",
                table: "StandardDayPartSplits",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardDayPartTimeslices_DayPartId",
                table: "StandardDayPartTimeslices",
                column: "DayPartId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatureSettings_TenantSettingsId",
                table: "TenantFeatureSettings",
                column: "TenantSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRunEventSettings_TenantSettingsId",
                table: "TenantRunEventSettings",
                column: "TenantSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantWebhookSettings_TenantSettingsId",
                table: "TenantWebhookSettings",
                column: "TenantSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Universes_Demographic",
                table: "Universes",
                column: "Demographic");

            migrationBuilder.CreateIndex(
                name: "IX_Universes_EndDate",
                table: "Universes",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Universes_SalesArea",
                table: "Universes",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Universes_StartDate",
                table: "Universes",
                column: "StartDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgAvals");

            migrationBuilder.DropTable(
                name: "AgCampaignProgrammeProgrammeCategories");

            migrationBuilder.DropTable(
                name: "AgDayPartLengths");

            migrationBuilder.DropTable(
                name: "AgHfssDemos");

            migrationBuilder.DropTable(
                name: "AgMultiParts");

            migrationBuilder.DropTable(
                name: "AgPartLengths");

            migrationBuilder.DropTable(
                name: "AgParts");

            migrationBuilder.DropTable(
                name: "AgPredictions");

            migrationBuilder.DropTable(
                name: "AgRegionalBreaks");

            migrationBuilder.DropTable(
                name: "AgStrikeWeightLengths");

            migrationBuilder.DropTable(
                name: "AgTimeBands");

            migrationBuilder.DropTable(
                name: "AgTimeSlices");

            migrationBuilder.DropTable(
                name: "AnalysisGroupTargetMetrics");

            migrationBuilder.DropTable(
                name: "AutoBookInstanceConfigurationCriterias");

            migrationBuilder.DropTable(
                name: "AutoBookSettings");

            migrationBuilder.DropTable(
                name: "AutoBookTaskReports");

            migrationBuilder.DropTable(
                name: "AutoBookTasks");

            migrationBuilder.DropTable(
                name: "AutopilotRules");

            migrationBuilder.DropTable(
                name: "AutopilotSettings");

            migrationBuilder.DropTable(
                name: "AWSInstanceConfigurations");

            migrationBuilder.DropTable(
                name: "BreaksEfficiencies");

            migrationBuilder.DropTable(
                name: "BRSConfigurationForKPIs");

            migrationBuilder.DropTable(
                name: "BusinessTypes");

            migrationBuilder.DropTable(
                name: "CampaignBookingPositionGroupSalesAreas");

            migrationBuilder.DropTable(
                name: "CampaignBreakRequirementItems");

            migrationBuilder.DropTable(
                name: "CampaignBreakTypes");

            migrationBuilder.DropTable(
                name: "CampaignPaybacks");

            migrationBuilder.DropTable(
                name: "CampaignProgrammeRestrictionsCategoryOrProgramme");

            migrationBuilder.DropTable(
                name: "CampaignProgrammeRestrictionsSalesAreas");

            migrationBuilder.DropTable(
                name: "CampaignSalesAreaTargetGroupSalesAreas");

            migrationBuilder.DropTable(
                name: "CampaignSalesAreaTargetMultipartLengths");

            migrationBuilder.DropTable(
                name: "CampaignSettings");

            migrationBuilder.DropTable(
                name: "CampaignTargetStrikeWeightDayPartLengths");

            migrationBuilder.DropTable(
                name: "CampaignTargetStrikeWeightDayPartTimeSlices");

            migrationBuilder.DropTable(
                name: "CampaignTargetStrikeWeightLengths");

            migrationBuilder.DropTable(
                name: "CampaignTimeRestrictionsSalesAreas");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "ClashDifferenceTimeAndDows");

            migrationBuilder.DropTable(
                name: "ClashExceptionsTimeAndDows");

            migrationBuilder.DropTable(
                name: "ClearanceCodes");

            migrationBuilder.DropTable(
                name: "DeliveryCappingGroup");

            migrationBuilder.DropTable(
                name: "Demographics");

            migrationBuilder.DropTable(
                name: "EfficiencySettings");

            migrationBuilder.DropTable(
                name: "EmailAuditEventSettings");

            migrationBuilder.DropTable(
                name: "ExternalRunInfo");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "FailureItems");

            migrationBuilder.DropTable(
                name: "FaultTypeDescriptions");

            migrationBuilder.DropTable(
                name: "FlexibilityLevels");

            migrationBuilder.DropTable(
                name: "FunctionalAreaDescriptions");

            migrationBuilder.DropTable(
                name: "FunctionalAreaFaultTypes");

            migrationBuilder.DropTable(
                name: "IndexTypes");

            migrationBuilder.DropTable(
                name: "InventoryLocks");

            migrationBuilder.DropTable(
                name: "InventoryTypeLockTypes");

            migrationBuilder.DropTable(
                name: "ISRGlobalSettings");

            migrationBuilder.DropTable(
                name: "ISRSettingsDemographics");

            migrationBuilder.DropTable(
                name: "KPIComparisonConfigs");

            migrationBuilder.DropTable(
                name: "KPIPriorities");

            migrationBuilder.DropTable(
                name: "LandmarkRunQueues");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "LengthFactors");

            migrationBuilder.DropTable(
                name: "LockTypes");

            migrationBuilder.DropTable(
                name: "MetadataValues");

            migrationBuilder.DropTable(
                name: "MSTeamsAuditEventSettings");

            migrationBuilder.DropTable(
                name: "OutputFileColumns");

            migrationBuilder.DropTable(
                name: "PassBreakExclusions");

            migrationBuilder.DropTable(
                name: "PassProgrammeRepetitions");

            migrationBuilder.DropTable(
                name: "PassRatingPoints");

            migrationBuilder.DropTable(
                name: "PassRules");

            migrationBuilder.DropTable(
                name: "PassSalesAreaPriorities");

            migrationBuilder.DropTable(
                name: "PassSlottingLimits");

            migrationBuilder.DropTable(
                name: "PipelineAuditEvents");

            migrationBuilder.DropTable(
                name: "PositionGroupAssociations");

            migrationBuilder.DropTable(
                name: "PredictionScheduleRatings");

            migrationBuilder.DropTable(
                name: "ProductAdvertisers");

            migrationBuilder.DropTable(
                name: "ProductAgencies");

            migrationBuilder.DropTable(
                name: "ProductPersons");

            migrationBuilder.DropTable(
                name: "ProgrammeCategoryHierarchy");

            migrationBuilder.DropTable(
                name: "ProgrammeCategoryLinks");

            migrationBuilder.DropTable(
                name: "ProgrammesClassifications");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropTable(
                name: "RestrictionsSalesAreas");

            migrationBuilder.DropTable(
                name: "ResultFiles");

            migrationBuilder.DropTable(
                name: "RSGlobalSettings");

            migrationBuilder.DropTable(
                name: "RSSettingsDefaultDeliverySettings");

            migrationBuilder.DropTable(
                name: "RSSettingsDemographicsDeliverySettings");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "RuleTypes");

            migrationBuilder.DropTable(
                name: "RunAnalysisGroupTargets");

            migrationBuilder.DropTable(
                name: "RunAuthors");

            migrationBuilder.DropTable(
                name: "RunCampaignProcessesSettings");

            migrationBuilder.DropTable(
                name: "RunCampaignReferences");

            migrationBuilder.DropTable(
                name: "RunExcludedInventoryStatuses");

            migrationBuilder.DropTable(
                name: "RunInventoryLocks");

            migrationBuilder.DropTable(
                name: "RunLandmarkScheduleSettings");

            migrationBuilder.DropTable(
                name: "RunSalesAreaPriorities");

            migrationBuilder.DropTable(
                name: "RunScheduleSettings");

            migrationBuilder.DropTable(
                name: "RunTypeAnalysisGroups");

            migrationBuilder.DropTable(
                name: "SalesAreaDemographics");

            migrationBuilder.DropTable(
                name: "SalesAreaPriorities");

            migrationBuilder.DropTable(
                name: "SalesAreasChannelGroups");

            migrationBuilder.DropTable(
                name: "SalesAreasHolidays");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignFailures");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignMetrics");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignPriorityRounds");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignResults");

            migrationBuilder.DropTable(
                name: "ScenarioCompactCampaignPaybacks");

            migrationBuilder.DropTable(
                name: "ScenarioPassPriorities");

            migrationBuilder.DropTable(
                name: "ScenarioPassReferences");

            migrationBuilder.DropTable(
                name: "ScenarioResultMetrics");

            migrationBuilder.DropTable(
                name: "ScheduleBreakEfficiencies");

            migrationBuilder.DropTable(
                name: "ScheduleProgrammes");

            migrationBuilder.DropTable(
                name: "SmoothBestBreakFactorGroupRecordPassSequences");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationBestBreakFactorGroupItemDefaultFactors");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationBestBreakFactorGroupItemFilterFactors");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationSameBreakGroupScoreFactors");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationSmoothPasses");

            migrationBuilder.DropTable(
                name: "SmoothDiagnosticConfigurations");

            migrationBuilder.DropTable(
                name: "SmoothFailureMessageDescriptions");

            migrationBuilder.DropTable(
                name: "SmoothFailuresSmoothFailureMessages");

            migrationBuilder.DropTable(
                name: "SmoothPassDefaultIterations");

            migrationBuilder.DropTable(
                name: "SmoothPassIterationRecordPassSequences");

            migrationBuilder.DropTable(
                name: "SmoothPassUnplacedIterations");

            migrationBuilder.DropTable(
                name: "SponsoredDayParts");

            migrationBuilder.DropTable(
                name: "SponsorshipAdvertiserExclusivities");

            migrationBuilder.DropTable(
                name: "SponsorshipClashExclusivities");

            migrationBuilder.DropTable(
                name: "SpotBookingRuleSalesAreas");

            migrationBuilder.DropTable(
                name: "SpotPlacements");

            migrationBuilder.DropTable(
                name: "Spots");

            migrationBuilder.DropTable(
                name: "StandardDayPartSplits");

            migrationBuilder.DropTable(
                name: "StandardDayPartTimeslices");

            migrationBuilder.DropTable(
                name: "TenantFeatureSettings");

            migrationBuilder.DropTable(
                name: "TenantRunEventSettings");

            migrationBuilder.DropTable(
                name: "TenantWebhookSettings");

            migrationBuilder.DropTable(
                name: "TotalRatings");

            migrationBuilder.DropTable(
                name: "Universes");

            migrationBuilder.DropTable(
                name: "AgLengths");

            migrationBuilder.DropTable(
                name: "AgStrikeWeights");

            migrationBuilder.DropTable(
                name: "AgCampaignProgrammes");

            migrationBuilder.DropTable(
                name: "AgDayParts");

            migrationBuilder.DropTable(
                name: "AutoBookInstanceConfigurations");

            migrationBuilder.DropTable(
                name: "AutoBooks");

            migrationBuilder.DropTable(
                name: "Breaks");

            migrationBuilder.DropTable(
                name: "BRSConfigurationTemplates");

            migrationBuilder.DropTable(
                name: "CampaignBookingPositionGroups");

            migrationBuilder.DropTable(
                name: "CampaignBreakRequirements");

            migrationBuilder.DropTable(
                name: "CampaignProgrammeRestrictions");

            migrationBuilder.DropTable(
                name: "CampaignSalesAreaTargetGroups");

            migrationBuilder.DropTable(
                name: "CampaignSalesAreaTargetMultiparts");

            migrationBuilder.DropTable(
                name: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropTable(
                name: "CampaignTimeRestrictions");

            migrationBuilder.DropTable(
                name: "ClashDifferences");

            migrationBuilder.DropTable(
                name: "ClashExceptions");

            migrationBuilder.DropTable(
                name: "RunScenarios");

            migrationBuilder.DropTable(
                name: "Failures");

            migrationBuilder.DropTable(
                name: "FaultTypes");

            migrationBuilder.DropTable(
                name: "FunctionalAreas");

            migrationBuilder.DropTable(
                name: "InventoryTypes");

            migrationBuilder.DropTable(
                name: "ISRSettings");

            migrationBuilder.DropTable(
                name: "MetadataCategories");

            migrationBuilder.DropTable(
                name: "OutputFiles");

            migrationBuilder.DropTable(
                name: "PassSalesAreaPriorityCollection");

            migrationBuilder.DropTable(
                name: "BookingPositions");

            migrationBuilder.DropTable(
                name: "BookingPositionGroups");

            migrationBuilder.DropTable(
                name: "PredictionSchedules");

            migrationBuilder.DropTable(
                name: "Advertisers");

            migrationBuilder.DropTable(
                name: "AgencyGroups");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProgrammeCategories");

            migrationBuilder.DropTable(
                name: "Restrictions");

            migrationBuilder.DropTable(
                name: "RSSettingsDemographicsSettings");

            migrationBuilder.DropTable(
                name: "AnalysisGroups");

            migrationBuilder.DropTable(
                name: "RunTypes");

            migrationBuilder.DropTable(
                name: "LibrarySalesAreaPassPriorities");

            migrationBuilder.DropTable(
                name: "SalesAreas");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignPriorityRoundCollections");

            migrationBuilder.DropTable(
                name: "ScenarioCompactCampaigns");

            migrationBuilder.DropTable(
                name: "ScenarioResults");

            migrationBuilder.DropTable(
                name: "ScheduleBreaks");

            migrationBuilder.DropTable(
                name: "Programmes");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationBestBreakFactorGroupItems");

            migrationBuilder.DropTable(
                name: "SmoothFailureMessages");

            migrationBuilder.DropTable(
                name: "SmoothFailures");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationSmoothPassIterationRecords");

            migrationBuilder.DropTable(
                name: "SponsorshipItems");

            migrationBuilder.DropTable(
                name: "SpotBookingRules");

            migrationBuilder.DropTable(
                name: "StandardDayPartGroups");

            migrationBuilder.DropTable(
                name: "StandardDayParts");

            migrationBuilder.DropTable(
                name: "TenantSettings");

            migrationBuilder.DropTable(
                name: "AgCampaignSalesAreas");

            migrationBuilder.DropTable(
                name: "CampaignTargetStrikeWeights");

            migrationBuilder.DropTable(
                name: "Clashes");

            migrationBuilder.DropTable(
                name: "Runs");

            migrationBuilder.DropTable(
                name: "Passes");

            migrationBuilder.DropTable(
                name: "RSSettings");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignPassPriorities");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "ProgrammeEpisodes");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationBestBreakFactorGroups");

            migrationBuilder.DropTable(
                name: "SponsoredItems");

            migrationBuilder.DropTable(
                name: "AutoBookDefaultParameters");

            migrationBuilder.DropTable(
                name: "CampaignTargets");

            migrationBuilder.DropTable(
                name: "Scenarios");

            migrationBuilder.DropTable(
                name: "ProgrammeDictionaries");

            migrationBuilder.DropTable(
                name: "SmoothConfigurationBestBreakFactorGroupRecords");

            migrationBuilder.DropTable(
                name: "Sponsorships");

            migrationBuilder.DropTable(
                name: "CampaignSalesAreaTargets");

            migrationBuilder.DropTable(
                name: "SmoothConfigurations");

            migrationBuilder.DropTable(
                name: "Campaigns");
        }
    }
}
