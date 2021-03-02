using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.ReportSystem.Excel;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig;
using xggameplan.common.Utilities;
using xggameplan.core.ReportGenerators.DataSnapshotRequirements;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignFailures
{
    public class ScenarioCampaignFailuresReportCreator
    {
        private const string SheetName = "ScenarioCampaignFailures";
        private const int PageSize = 1_000_000;

        public static string GetFilePath(string scenarioName, DateTime runStartDate, Guid scenarioId)
        {
            var fileName = $"{scenarioName.Substring(0, Math.Min(scenarioName.Length, 20))}_{runStartDate.ToString("yyyyMMdd")}_AllScenarioFailureData.xlsx";

            return $"reports/{scenarioId}/scenario-campaign-failures/{fileName}";
        }

        public IReadOnlyCollection<ScenarioCampaignFailureExportModel> GenerateReportData<T>(IReadOnlyCollection<ScenarioCampaignFailure> data, T snapshot)
            where T : IFaultTypesHolder, ICampaignsHolder
        {
            if (data is null || snapshot is null || data.Count == 0)
            {
                return Array.Empty<ScenarioCampaignFailureExportModel>();
            }

            var campaigns = snapshot.Campaigns.ToDictionary(c => c.ExternalId);
            var failureTypes = snapshot.FaultTypes.ToDictionary(c => c.Id);

            return data
                .AsParallel()
                .Select(item =>
                {
                    var scenarioCampaignResultModel = new ScenarioCampaignFailureExportModel
                    {
                        ExternalCampaignId = item.ExternalCampaignId,
                        SalesAreaGroupName = item.SalesAreaGroup,
                        SalesAreaName = item.SalesArea,
                        DurationSecs = (int)item.Length.ToTimeSpan().TotalSeconds,
                        MultipartNo = item.MultipartNo,
                        StrikeWeightStartDate = item.StrikeWeightStartDate.ToUniversalTime(),
                        StrikeWeightEndDate = item.StrikeWeightEndDate.ToUniversalTime(),
                        DayPartStartTime = item.DayPartStartTime,
                        DayPartEndTime = item.DayPartEndTime,
                        DayPartDays = item.DayPartDays,
                        FailureCount = item.FailureCount,
                        PassesEncounteringFailure = item.PassesEncounteringFailure
                    };

                    if (item.ExternalCampaignId != null && campaigns.TryGetValue(item.ExternalCampaignId, out var campaign))
                    {
                        scenarioCampaignResultModel.CampaignName = campaign.Name;
                    }

                    if (failureTypes.TryGetValue(item.FailureType, out var faultType))
                    {
                        scenarioCampaignResultModel.FailureTypeName =
                            faultType.Description?.FirstOrDefault(e => e.Key == "ENG").Value;
                    }

                    return scenarioCampaignResultModel;
                }).ToArray();
        }

        public Stream GenerateReport<T>(IReadOnlyCollection<ScenarioCampaignFailure> data, T snapshot, DayOfWeek tenantStartDayOfWeek)
            where T : IFaultTypesHolder, ICampaignsHolder
        {
            var results = GenerateReportData(data, snapshot);
            var weekDays = DaypartDayFormattingUtilities.GetWeekDaysWithCustomStart(tenantStartDayOfWeek);

            var sheetName = SheetName;
            var sheetConfiguration = CreateExcelConfigurationBuilder(weekDays, tenantStartDayOfWeek).BuildConfiguration();

            var stream = new MemoryStream();
            using (var reportBuilder = new OneTableExcelReportBuilder(new ExcelStyleApplier())
                .PredefineStyles(GamePlanReportStyles.AllPredefineStyles))
            {
                if (results.Count == 0)
                {
                    _ = reportBuilder.Sheet(sheetName, sheetBuilder =>
                    {
                        sheetBuilder.DataContent(results, sheetConfiguration);
                    });
                }
                else
                {
                    for (int i = 0, page = 1; i < results.Count; i += PageSize, page++)
                    {
                        // copy for lambda
                        int j = i;
                        _ = reportBuilder.Sheet(sheetName, sheetBuilder =>
                        {
                            sheetBuilder.DataContent(results.Skip(j).Take(PageSize).ToArray(), sheetConfiguration);
                        });

                        sheetName = $"{SheetName} ({page})";
                    }
                }

                reportBuilder.SaveAs(stream);
            }

            _ = stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private static OneTableExcelConfigurationBuilder<ScenarioCampaignFailureExportModel> CreateExcelConfigurationBuilder(string[] weekDays, DayOfWeek tenantStartDayOfWeek)
        {
            var confBuilder = new OneTableExcelConfigurationBuilder<ScenarioCampaignFailureExportModel>()
                .SetDefaultStyles();

            return confBuilder
                .OrderMembersAsDescribed()
                .IgnoreNotDescribed()
                .ForMember(m => m.ExternalCampaignId, o => o.Width(20))
                .ForMember(m => m.CampaignName, o => o.Width(25))
                .ForMember(m => m.SalesAreaGroupName, o => o.Width(25))
                .ForMember(m => m.SalesAreaName, o => o.Width(25))
                .ForMember(m => m.DurationSecs, o => o.Width(15))
                .ForMember(m => m.MultipartNo, o => o.Width(13))
                .ForMember(m => m.MultipartNo, o => o.Width(13))
                .ForMember(m => m.StrikeWeightStartDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Header("StrikeWeight Start Date").Width(25))
                .ForMember(m => m.StrikeWeightEndDate, o => o.Formatter(ReportFormatter.ConvertToShortDate).Header("StrikeWeight End Date").Width(25))
                .ForMember(m => m.DayPartStartTime,
                    o => o.Formatter(ReportFormatter.ConvertToTime).Header("Daypart Start Time").Width(18))
                .ForMember(m => m.DayPartEndTime, o => o.Formatter(ReportFormatter.ConvertToTime).Header("Daypart End Time").Width(18))
                .ForMember(m => m.DayPartDays,
                    o => o.Header("Daypart Days")
                        .Formatter(m => DaypartDayFormattingUtilities.FormatWeekDays(weekDays, ((string)m), tenantStartDayOfWeek)).Width(15))
                .ForMember(m => m.FailureTypeName, o => o.Width(25))
                .ForMember(m => m.FailureCount, o => o.Width(15))
                .ForMember(m => m.PassesEncounteringFailure, o => o.Header("Pass(s) Encountering Failure").Width(30));
        }
    }
}
