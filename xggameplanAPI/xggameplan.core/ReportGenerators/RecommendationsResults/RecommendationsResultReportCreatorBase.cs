using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.ReportSystem.Excel;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig;
using xggameplan.core.Extensions;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignResults
{
    /// <summary>
    /// Base class for recommendation results report creator.
    /// </summary>
    /// <seealso cref="xggameplan.core.ReportGenerators.Interfaces.IRecommendationsResultReportCreator" />
    public abstract class RecommendationsResultReportCreatorBase : IRecommendationsResultReportCreator
    {
        private const string SheetName = "Recommendations";
        private const string DefaultTimeFormat = "000000";
        private const string SmoothProcessorName = "SMOOTH";
        private const int PageSize = 1_000_000;

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationsResultReportCreatorBase" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        protected RecommendationsResultReportCreatorBase(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="source">The source.</param>
        protected abstract void PrepareRelatedData(IReadOnlyCollection<Recommendation> source);

        /// <summary>Resolves the tenant settings.</summary>
        /// <returns></returns>
        protected abstract TenantSettings ResolveTenantSettings();

        /// <summary>Resolves the campaign.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract Campaign ResolveCampaign(Recommendation item);

        /// <summary>Resolves the sales area.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract SalesArea ResolveSalesArea(Recommendation item);

        /// <summary>Resolves the demographic.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract Demographic ResolveDemographic(Recommendation item);

        /// <summary>Resolves the product.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract Product ResolveProduct(Recommendation item);

        /// <summary>Resolves the product clash.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract Clash ResolveClash(Recommendation item);

        /// <summary>Resolves the product parent clash.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract Clash ResolveParentClash(Recommendation item);

        /// <summary>Gets the day part.</summary>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="peakStartTime">The peak start time.</param>
        /// <param name="peakEndTime">The peak end time.</param>
        /// <param name="midnightStartTime">The midnight start time.</param>
        /// <param name="midnightEndTime">The midnight end time.</param>
        /// <returns></returns>
        private static string GetDayPart(DateTime startDateTime, TimeSpan peakStartTime, TimeSpan peakEndTime, TimeSpan midnightStartTime, TimeSpan midnightEndTime)
        {
            var time = startDateTime.TimeOfDay;
            if (time >= peakStartTime && time < peakEndTime)
            {
                return "Peak";
            }
            if (time >= midnightStartTime && time < midnightEndTime)
            {
                return "Midnight to Dawn";
            }
            return "Off Peak";
        }

        /// <summary>Gets the week commencing date.</summary>
        /// <param name="startDayOfWeek">The start day of week.</param>
        /// <param name="startDate">The start date.</param>
        /// <returns></returns>
        private static DateTime GetWeekCommencingDate(DayOfWeek startDayOfWeek, DateTime startDate)
        {
            return startDate.Date.StartAndEndOfWeekDate(startDayOfWeek).startDate;
        }

        /// <summary>Generates the report data.</summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public IReadOnlyCollection<RecommendationExtendedModel> GenerateReportData(IReadOnlyCollection<Recommendation> source)
        {
            if ((source?.Count ?? 0) == 0)
            {
                return Array.Empty<RecommendationExtendedModel>();
            }

            PrepareRelatedData(source);

            var tenantSettings = ResolveTenantSettings();
            var startDayOfWeek = tenantSettings?.StartDayOfWeek ?? DayOfWeek.Monday;

            var peakStartTime = AgConversions.ParseTotalHHMMSSFormat(tenantSettings?.PeakStartTime ?? DefaultTimeFormat);
            var peakEndTime = AgConversions.ParseTotalHHMMSSFormat(tenantSettings?.PeakEndTime ?? DefaultTimeFormat);
            var midnightStartTime = new TimeSpan(0, 0, 0);
            var midnightEndTime = new TimeSpan(6, 0, 0);

            return source
                .AsParallel()
                .Select(item =>
                {
                    var recommendationExtendedModel = new RecommendationExtendedModel();
                    _ = _mapper.Map(item, recommendationExtendedModel);

                    recommendationExtendedModel.StartDayOfWeek = startDayOfWeek.ToString();
                    recommendationExtendedModel.WeekCommencingDate =
                        GetWeekCommencingDate(startDayOfWeek, item.StartDateTime);
                    recommendationExtendedModel.DayPart = GetDayPart(item.StartDateTime, peakStartTime, peakEndTime,
                        midnightStartTime, midnightEndTime);
                    recommendationExtendedModel.StartDate = item.StartDateTime.Date;
                    recommendationExtendedModel.StartTime = item.StartDateTime.TimeOfDay;
                    recommendationExtendedModel.EndDate = item.EndDateTime.Date;
                    recommendationExtendedModel.EndTime = item.EndDateTime.TimeOfDay;
                    recommendationExtendedModel.SpotLengthInSec = item.SpotLength.TotalSeconds;

                    if (String.Equals(item.Processor, SmoothProcessorName, StringComparison.OrdinalIgnoreCase))
                    {
                        recommendationExtendedModel.ExternalBreakNo = String.IsNullOrEmpty(item.ExternalBreakNo)
                            ? Globals.UnplacedBreakString
                            : recommendationExtendedModel.ExternalBreakNo;
                    }

                    var campaign = ResolveCampaign(item);
                    if (!(campaign is null))
                    {
                        recommendationExtendedModel.SalesAreaGroupName = campaign.SalesAreaCampaignTarget?
                            .Select(sact => sact.SalesAreaGroup)
                            .FirstOrDefault(sag => sag?.SalesAreas?.Any(sa => sa.Equals(item.SalesArea)) ?? false)
                            ?.GroupName;
                    }

                    if (recommendationExtendedModel.SalesAreaGroupName is null)
                    {
                        var salesArea = ResolveSalesArea(item);
                        recommendationExtendedModel.SalesAreaGroupName = salesArea?.TargetAreaName;
                    }

                    var demographic = ResolveDemographic(item);
                    if (!(demographic is null))
                    {
                        recommendationExtendedModel.DemographicName = demographic.Name;
                    }

                    var product = ResolveProduct(item);
                    if (!(product is null))
                    {
                        recommendationExtendedModel.ClientName = product.AgencyName;
                        recommendationExtendedModel.ProductName = product.Name;

                        var childClash = ResolveClash(item);
                        if (!(childClash is null))
                        {
                            recommendationExtendedModel.ClashName = childClash.Description;

                            var parentClash = ResolveParentClash(item);
                            if (!(parentClash is null))
                            {
                                recommendationExtendedModel.ParentClashName = parentClash.Description;
                            }
                        }
                    }

                    return recommendationExtendedModel;
                }).ToArray();
        }

        /// <summary>Generates the report.</summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public Stream GenerateReport(IReadOnlyCollection<Recommendation> source)
        {
            var extendedData = GenerateReportData(source);

            var sheetName = SheetName;
            var sheetConfiguration = CreateExcelConfigurationBuilder().BuildConfiguration();

            var stream = new MemoryStream();
            using (var reportBuilder = new OneTableExcelReportBuilder(new ExcelStyleApplier())
                .PredefineStyles(GamePlanReportStyles.RecommendationReportPredefineStyles))
            {
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
                            sheetBuilder
                                .Freeze(1, 2)
                                .DataContent(extendedData.Skip(j).Take(PageSize).ToArray(), sheetConfiguration);
                        });

                        sheetName = $"{SheetName} ({page})";
                    }
                }

                reportBuilder.SaveAs(stream);
            }

            _ = stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>Creates the excel configuration builder.</summary>
        /// <returns></returns>
        private static OneTableExcelConfigurationBuilder<RecommendationExtendedModel> CreateExcelConfigurationBuilder()
        {
            var confBuilder = new OneTableExcelConfigurationBuilder<RecommendationExtendedModel>()
                .SetDefaultStyles();

            return confBuilder
                .OrderMembersAsDescribed()
                .IgnoreNotDescribed()
                .ForMember(r => r.SalesAreaGroupName, builder => builder.Width(21))
                .ForMember(r => r.DemographicName, builder => builder.Width(18))
                .ForMember(r => r.ClientName, builder => builder.Width(18))
                .ForMember(r => r.ParentClashName, builder => builder.Width(32))
                .ForMember(r => r.ClashName, builder => builder.Width(30))
                .ForMember(r => r.ProductName, builder => builder.Width(72))
                .ForMember(r => r.ScenarioId, builder => builder.Width(38))
                .ForMember(r => r.ExternalSpotRef, builder => builder.Width(16))
                .ForMember(r => r.ExternalCampaignNumber, builder => builder.Width(24))
                .ForMember(r => r.SpotLengthInSec, builder => builder.Width(12).Header("Spot Length"))
                .ForMember(r => r.Product, builder => builder.Width(10))
                .ForMember(r => r.Demographic, builder => builder.Width(14))
                .ForMember(r => r.BreakBookingPosition, builder => builder.Width(20))
                .ForMember(r => r.SalesArea, builder => builder.Width(10))
                .ForMember(r => r.ExternalProgrammeReference,
                    builder => builder.Width(10).Header("Programme External Reference"))
                .ForMember(r => r.ProgrammeNo, builder => builder.Width(14))
                .ForMember(r => r.ProgrammeName, builder => builder.Width(72))
                .ForMember(r => r.StartDateTime, builder => builder.Width(22).Formatter(ReportFormatter.ConvertToDateTime))
                .ForMember(r => r.StartDate, builder => builder.Width(12).Formatter(ReportFormatter.ConvertToShortDate))
                .ForMember(r => r.StartTime, builder => builder.Width(12).Formatter(ReportFormatter.ConvertToTime))
                .ForMember(r => r.DayPart, builder => builder.Width(12).Header("DayPart"))
                .ForMember(r => r.WeekCommencingDate, builder => builder.Width(12).Formatter(ReportFormatter.ConvertToShortDate))
                .ForMember(r => r.StartDayOfWeek, builder => builder.Width(10))
                .ForMember(r => r.EndDateTime, builder => builder.Width(22).Formatter(ReportFormatter.ConvertToDateTime))
                .ForMember(r => r.EndDate, builder => builder.Width(12).Formatter(ReportFormatter.ConvertToShortDate))
                .ForMember(r => r.EndTime, builder => builder.Width(12).Formatter(ReportFormatter.ConvertToTime))
                .ForMember(r => r.BreakType, builder => builder.Width(10))
                .ForMember(r => r.SpotRating, builder => builder.Width(12))
                .ForMember(r => r.SpotEfficiency, builder => builder.Width(14))
                .ForMember(r => r.RatingPoints, builder => builder.Width(14))
                .ForMember(r => r.Action, builder => builder.Width(10))
                .ForMember(r => r.Processor, builder => builder.Width(12))
                .ForMember(r => r.ProcessorDateTime,
                    builder => builder.Width(22).Formatter(ReportFormatter.ConvertToDateTime))
                .ForMember(r => r.GroupCode, builder => builder.Width(12))
                .ForMember(r => r.ClientPicked, builder => builder.Width(12))
                .ForMember(r => r.MultipartSpot, builder => builder.Width(14))
                .ForMember(r => r.MultipartSpotPosition, builder => builder.Width(22))
                .ForMember(r => r.MultipartSpotRef, builder => builder.Width(18))
                .ForMember(r => r.RequestedPositionInBreak, builder => builder.Width(26))
                .ForMember(r => r.ActualPositionInBreak, builder => builder.Width(22))
                .ForMember(r => r.ExternalBreakNo, builder => builder.Width(18))
                .ForMember(r => r.Filler, builder => builder.Width(10))
                .ForMember(r => r.Sponsored, builder => builder.Width(12))
                .ForMember(r => r.Preemptable, builder => builder.Width(14))
                .ForMember(r => r.Preemptlevel, builder => builder.Width(14).Header("Preempt Level"))
                .ForMember(r => r.PassSequence, builder => builder.Width(14))
                .ForMember(r => r.PassIterationSequence, builder => builder.Width(22))
                .ForMember(r => r.PassName, builder => builder.Width(40))
                .ForMember(r => r.OptimiserPassSequenceNumber, builder => builder.Width(32))
                .ForMember(r => r.CampaignPassPriority, builder => builder.Width(22))
                .ForMember(r => r.RankOfEfficiency, builder => builder.Width(18))
                .ForMember(r => r.RankOfCampaign, builder => builder.Width(18))
                .ForMember(r => r.CampaignWeighting, builder => builder.Width(20))
                .ForMember(r => r.SpotSequenceNumber, builder => builder.Width(22));
        }
    }
}
