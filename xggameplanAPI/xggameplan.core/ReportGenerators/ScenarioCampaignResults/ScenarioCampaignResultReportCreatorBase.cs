using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.ReportSystem.Excel;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.OneTable;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.KPIProcessing.KPICalculation;
using xggameplan.Model;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignResults
{
    /// <summary>
    /// Base class of scenario campaign result report creator.
    /// </summary>
    /// <seealso cref="xggameplan.core.ReportGenerators.Interfaces.IScenarioCampaignResultReportCreator" />
    public abstract class ScenarioCampaignResultReportCreatorBase : IScenarioCampaignResultReportCreator
    {
        private const string SheetName = "Scenario Campaign Results";
        private const int PageSize = 500_000;
        private bool _enablePerformanceKpiColumns;
        private bool _isCampaignLevel;

        /// <summary>Prepares the related data.</summary>
        /// <param name="source">The source.</param>
        protected abstract void PrepareRelatedData(IReadOnlyCollection<ScenarioCampaignExtendedResultItem> source);

        /// <summary>
        /// Clears cache of the related data.
        /// </summary>
        protected abstract void ClearRelatedData();

        /// <summary>Resolves the campaign.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract Campaign ResolveCampaign(ScenarioCampaignResultItem item);

        /// <summary>Resolves the demographic.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Demographic ResolveDemographic(Campaign campaign);

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Product ResolveProduct(Campaign campaign);

        /// <summary>Resolves the product clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Clash ResolveClash(Campaign campaign);

        /// <summary>Resolves the product parent clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Clash ResolveParentClash(Campaign campaign);

        /// <summary>
        /// Resolves PayPart for pre-post KPI calculation
        /// </summary>
        /// <param name="campaign"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>Example of DowTime string inside DayPartName: "2100-2659(Mon-Fri)"</remarks>
        protected abstract ICampaignKpiData ResolveDayPartKpiModel(Campaign campaign, ScenarioCampaignResultItem item);

        /// <summary>Generates the report data.</summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public IReadOnlyCollection<ScenarioCampaignResultModel> GenerateReportData(
            IReadOnlyCollection<ScenarioCampaignExtendedResultItem> source)
        {
            if (source is null || source.Count == 0)
            {
                return Array.Empty<ScenarioCampaignResultModel>();
            }

            PrepareRelatedData(source);

            var result = source
                .AsParallel()
                .Select(item =>
                {
                    var scenarioCampaignResultModel = new ScenarioCampaignResultModel
                    {
                        ExternalCampRef = item.CampaignExternalId,
                        DaypartName = item.DaypartName,
                        DurationSecs = item.SpotLength,
                        TargetRatings = item.TargetRatings,
                        PreRunRatings = item.PreRunRatings,
                        ISRCancelledRatings = item.ISRCancelledRatings,
                        ISRCancelledSpots = item.ISRCancelledSpots,
                        RSCancelledRatings = item.RSCancelledRatings,
                        RSCancelledSpots = item.RSCancelledSpots,
                        OptimiserRatings = item.OptimiserRatings,
                        StrikeWeightStartDate = item.StrikeWeightStartDate,
                        StrikeWeightEndDate = item.StrikeWeightEndDate,
                        OptimiserBookedSpots = item.OptimiserBookedSpots,
                        PreRunRatingsDiff = item.PreRunRatings - item.TargetRatings,
                        PostRunRatings = item.PreRunRatings + item.OptimiserRatings -
                                         (item.ISRCancelledRatings + item.RSCancelledRatings),
                        MediaSalesGroup = item.MediaSalesGroup,
                        ProductAssignee = item.ProductAssignee,
                        StopBooking = item.StopBooking,
                        CreationDate = item.CreationDate,
                        AutomatedBooked = item.AutomatedBooked,
                        TopTail = item.TopTail,
                        ReportingCategory = item.ReportingCategory,
                        ClashCode = item.ClashCode,
                        AgencyName = item.AgencyName,
                        PassThatDelivered100Percent = item.PassThatDelivered100Percent
                    };

                    scenarioCampaignResultModel.PostRunRatingsDiff = scenarioCampaignResultModel.PostRunRatings - item.TargetRatings;

                    var campaign = ResolveCampaign(item);
                    if (campaign is null)
                    {
                        return scenarioCampaignResultModel;
                    }

                    scenarioCampaignResultModel.CampaignName = campaign.Name;
                    scenarioCampaignResultModel.CampaignGroup = campaign.CampaignGroup;
                    scenarioCampaignResultModel.BusinessType = campaign.BusinessType;
                    scenarioCampaignResultModel.CampaignStartDate = campaign.StartDateTime;
                    scenarioCampaignResultModel.CampaignEndDate = campaign.EndDateTime;
                    scenarioCampaignResultModel.DaysToEndOfCampaign = (campaign.EndDateTime - DateTime.Now).Days;
                    scenarioCampaignResultModel.TargetAchievedPct = Math.Round(
                        item.TargetRatings == 0
                            ? 0
                            : scenarioCampaignResultModel.PostRunRatings / item.TargetRatings * 100,
                        2,
                        MidpointRounding.AwayFromZero
                    );

                    scenarioCampaignResultModel.SalesAreaGroupName = campaign.SalesAreaCampaignTarget
                        .Select(sact => sact?.SalesAreaGroup)
                        .FirstOrDefault(sag => IsSalesAreaNameEqual(sag, item))
                        ?.GroupName;

                    var demographic = ResolveDemographic(campaign);
                    if (!(demographic is null))
                    {
                        scenarioCampaignResultModel.DemographicName = demographic.Name;
                    }

                    var product = ResolveProduct(campaign);
                    if (!(product is null))
                    {
                        scenarioCampaignResultModel.AdvertiserName = product.AdvertiserName;
                        scenarioCampaignResultModel.ProductName = product.Name;

                        var childClash = ResolveClash(campaign);
                        if (!(childClash is null))
                        {
                            scenarioCampaignResultModel.ChildClashName = childClash.Description;

                            var parentClash = ResolveParentClash(campaign);
                            if (!(parentClash is null))
                            {
                                scenarioCampaignResultModel.ParentClashName = parentClash.Description;
                            }
                        }
                    }

                    if (_enablePerformanceKpiColumns)
                    {
                        if (_isCampaignLevel)
                        {
                            var campaignLevelReportData = new CampaignLevelReport
                            {
                                NominalValue = campaign.RevenueBooked.HasValue ? campaign.RevenueBooked.Value : 0,
                                RevenueBudget = campaign.RevenueBudget,
                                Payback = (double)campaign.CalculateTotalPayback()
                            };

                            PrePostCampaignKPIsCalculator.PopulatePrePostCampaignKPIs(campaignLevelReportData, item, scenarioCampaignResultModel);
                        }
                        else
                        {
                            var dayPart = ResolveDayPartKpiModel(campaign, item);

                            if (dayPart != null)
                            {
                                PrePostCampaignKPIsCalculator.PopulatePrePostCampaignKPIs(dayPart, item, scenarioCampaignResultModel);
                            }
                        }
                    }

                    return scenarioCampaignResultModel;

                    static bool IsSalesAreaNameEqual(SalesAreaGroup salesAreaGroup,
                        ScenarioCampaignResultItem scenarioCampaignResultItem) =>
                        salesAreaGroup?.SalesAreas?.Any(sa =>
                            String.Equals(sa, scenarioCampaignResultItem.SalesAreaName,
                                StringComparison.OrdinalIgnoreCase)) ?? false;
                }).ToArray();

            ClearRelatedData();

            return result;
        }

        /// <summary>Generates the report.</summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public Stream GenerateReport(IReadOnlyCollection<ScenarioCampaignExtendedResultItem> source) =>
            GenerateReport(() => source);

        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <param name="getSourceAction">Method for getting report data, used to isolate lifetime of data objects</param>
        /// <returns></returns>
        public Stream GenerateReport(Func<IReadOnlyCollection<ScenarioCampaignExtendedResultItem>> getSourceAction)
        {
            var stream = new MemoryStream();
            using (var reportBuilder = new OneTableExcelReportBuilder(new ExcelStyleApplier())
                    .PredefineStyles(GamePlanReportStyles.AllPredefineStyles))
            {
                BuildReport(reportBuilder, getSourceAction);

                reportBuilder.SaveAs(stream);
            }

            _ = stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private void BuildReport(IBaseExcelReportBuilder<IOneTableSheetBuilder> reportBuilder, Func<IReadOnlyCollection<ScenarioCampaignExtendedResultItem>> getSourceAction)
        {
            var extendedData = GenerateReportData(getSourceAction());

            var sheetName = SheetName;
            var sheetConfiguration = CreateExcelConfigurationBuilder().BuildConfiguration();

            if (extendedData.Count == 0)
            {
                _ = reportBuilder.Sheet(sheetName, sheetBuilder =>
                {
                    sheetBuilder.DataContent(extendedData, sheetConfiguration);
                });
            }
            else
            {
                for (int i = 0, page = 1; i < extendedData.Count; i += PageSize, page++)
                {
                    // copy for lambda
                    int j = i;
                    _ = reportBuilder.Sheet(sheetName, sheetBuilder =>
                    {
                        sheetBuilder.DataContent(extendedData.Skip(j).Take(PageSize).ToArray(), sheetConfiguration);
                    });

                    sheetName = $"{SheetName} ({page})";
                }
            }
        }

        /// <summary>Creates the excel configuration builder.</summary>
        /// <returns></returns>
        private OneTableExcelConfigurationBuilder<ScenarioCampaignResultModel> CreateExcelConfigurationBuilder()
        {
            var confBuilder = new OneTableExcelConfigurationBuilder<ScenarioCampaignResultModel>()
                .SetDefaultStyles();

            _ = confBuilder
                .OrderMembersAsDescribed()
                .IgnoreNotDescribed()
                .ForMember(m => m.ExternalCampRef, o => o.Width(20))
                .ForMember(m => m.CampaignName, o => o.Width(30))
                .ForMember(m => m.CampaignGroup, o => o.Width(20))
                .ForMember(m => m.BusinessType, o => o.Width(15))
                .ForMember(m => m.CampaignStartDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Width(25))
                .ForMember(m => m.CampaignEndDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Width(25))
                .ForMember(m => m.DaysToEndOfCampaign, o => o.Width(25))
                .ForMember(m => m.ChildClashName, o => o.Width(25))
                .ForMember(m => m.ParentClashName, o => o.Width(25))
                .ForMember(m => m.ClashCode, o => o.Width(25))
                .ForMember(m => m.AgencyName, o => o.Width(25))
                .ForMember(m => m.ProductName, o => o.Width(25))
                .ForMember(m => m.AdvertiserName, o => o.Width(25))
                .ForMember(m => m.DemographicName, o => o.Width(25));

            if (!_isCampaignLevel)
            {
                _ = confBuilder
                    .ForMember(m => m.SalesAreaGroupName, o => o.Width(25))
                    .ForMember(m => m.StrikeWeightStartDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Width(25))
                    .ForMember(m => m.StrikeWeightEndDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Width(25))
                    .ForMember(m => m.DaypartName, o => o.Width(25))
                    .ForMember(m => m.DurationSecs, o => o.Width(20))
                    .ForMember(m => m.PassThatDelivered100Percent, o => o.Width(30).Header("Passes Achieving 100% Delivery"));
            }

            _ = confBuilder
                .ForMember(m => m.TargetRatings, o => o.Width(25))
                .ForMember(m => m.PreRunRatings, o => o.Width(25).Header("PreRun Ratings"))
                .ForMember(m => m.PreRunRatingsDiff, o => o.Width(30).Header("PreRun Ratings"))
                .ForMember(m => m.ISRCancelledRatings, o => o.Width(30))
                .ForMember(m => m.ISRCancelledSpots, o => o.Width(25))
                .ForMember(m => m.RSCancelledRatings, o => o.Width(25))
                .ForMember(m => m.RSCancelledSpots, o => o.Width(25))
                .ForMember(m => m.OptimiserRatings, o => o.Width(25))
                .ForMember(m => m.OptimiserBookedSpots, o => o.Width(25))
                .ForMember(m => m.PostRunRatings, o => o.Width(25).Header("PostRun Ratings"))
                .ForMember(m => m.PostRunRatingsDiff, o => o.Width(30).Header("PostRun Ratings Diff"))
                .ForMember(m => m.TargetAchievedPct, o => o.Width(30))
                .ForMember(m => m.ReportingCategory, o => o.Width(30))
                .ForMember(m => m.ProductAssignee, o => o.Width(30))
                .ForMember(m => m.CreationDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Width(25))
                .ForMember(m => m.AutomatedBooked, o => o.Width(30))
                .ForMember(m => m.TopTail, o => o.Width(30).Header("Top/Tail"))
                .ForMember(m => m.StopBooking, o => o.Width(30))
                .ForMember(m => m.MediaSalesGroup, o => o.Width(30));

            if (_enablePerformanceKpiColumns)
            {
                _ = confBuilder
                    .ForMember(m => m.ZeroRatedSpots, o => o.Width(25))
                    .ForMember(m => m.NominalValue, o => o.Width(30))
                    .ForMember(m => m.TotalNominalValue, o => o.Width(30))
                    .ForMember(m => m.RevenueBudget, o => o.Width(30))
                    .ForMember(m => m.TotalPayback, o => o.Width(30))
                    .ForMember(m => m.DifferenceValueDelivered, o => o.Width(25).Header("+/- Value Delivered"))
                    .ForMember(m => m.DifferenceValueDeliveredPercentage, o => o.Width(25).Header("+/- Value Delivered (%)"))
                    .ForMember(m => m.DifferenceValueDeliveredPayback, o => o.Width(25).Header("+/- Value Delivered Incl. Payback"))
                    .ForMember(m => m.DifferenceValueDeliveredPercentagePayback, o => o.Width(25).Header("+/- Value Delivered Incl. Payback (%)"));
            }

            return confBuilder;
        }

        public void EnablePerformanceKPIColumns(bool areEnabled)
        {
            _enablePerformanceKpiColumns = areEnabled;
        }

        public void EnableCampaignLevel(bool isCampaignLevel)
        {
            _isCampaignLevel = isCampaignLevel;
        }
    }
}
