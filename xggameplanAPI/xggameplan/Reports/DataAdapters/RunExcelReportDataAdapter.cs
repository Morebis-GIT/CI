using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using xggameplan.core.ReportGenerators;
using xggameplan.Model;
using xggameplan.Reports.Models;

namespace xggameplan.Reports.DataAdapters
{
    public class RunExcelReportDataAdapter : IRunExcelReportDataAdapter
    {
        public ExcelReportRunModel Map(RunModel run, IEnumerable<Demographic> demographics, DateTime reportDate)
        {
            var model = new ExcelReportRunModel();
            foreach (var scenario in run.Scenarios)
            {
                var excelReportScenario = Map(run, scenario, demographics, reportDate);
                model.Scenarios.Add(excelReportScenario);
            }
            return model;
        }

        public ExcelReportSmoothFailuresModel Map(Run run, List<SmoothFailureModel> smoothFailuresModel,
            IEnumerable<SmoothFailureMessage> failureMessages, DateTime reportDate, IMapper mapper)
        {
            var model = new ExcelReportSmoothFailuresModel
            {
                Run = run,
                SmoothFailures = mapper.Map<List<SmoothFailureExtendedModel>>(Tuple.Create(smoothFailuresModel, failureMessages)),
                ReportDate = reportDate
            };
            return model;
        }

        private ExcelReportScenario Map(RunModel run,ScenarioModel scenario, IEnumerable<Demographic> demographics, DateTime reportDate)
        {
            var model = new ExcelReportScenario();
            model.Name = scenario.Name;
            //Some passes are displayed in 1, 2, 3, 4 coulmns
            // therfore 12 coulumns is assigned to each pass
            // so it can be divided to 1, 2, 3 or 4 easily
            //and one column for the title
            var numberOfCellForEachPass = 12;
            model.MaxColumnsCount = scenario.Passes.Count * numberOfCellForEachPass + 1;// the 1 is the title column
            model.ScenarioDetails = ScenarioDetailsGrid(run, scenario, model.MaxColumnsCount, reportDate);
            model.SalesAreaPassPriorities = SalesAreaPassPrioritiesGrid(scenario.Passes, model.MaxColumnsCount);
            model.General = GeneralGrid(scenario.Passes, model.MaxColumnsCount);
            model.Weighting = WeightingGrid(scenario.Passes, model.MaxColumnsCount);
            model.Tolerance = ToleranceGrid(scenario.Passes, model.MaxColumnsCount);
            model.Rules = RulesGrid(scenario.Passes, model.MaxColumnsCount);
            model.ProgrammeRepetitions = ProgrammeRepetitionsGrid(scenario.Passes, model.MaxColumnsCount);
            model.BreakExclusions = BreakExclusionsGrid(scenario.Passes, model.MaxColumnsCount);
            model.MinRatingPoints = MinRatingPointsGrid(scenario.Passes, model.MaxColumnsCount);
            model.SlottingLimits = SlottingLimitsGrid(scenario.Passes, demographics, model.MaxColumnsCount);
            return model;
        }

        private ExcelReportGrid ScenarioDetailsGrid(RunModel run, ScenarioModel scenario, int maxColumnsCount, DateTime reportDate)
        {
            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            var row1 = new ExcelReportRow();
            row1.Cells.Add(new ExcelReportCell() { Value = "Report Date", Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.HeaderStyle.Name });
            row1.Cells.Add(new ExcelReportCell() { Value = string.Format(CultureInfo.InvariantCulture, "{0:dd MMMM yyyy} - {0:HH:mm:ss}", reportDate), Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.HeaderStyle.Name });
            grid.HeaderRows.Add(row1);

            var row2 = new ExcelReportRow();
            row2.Cells.Add(new ExcelReportCell() { Value = "Run Name", Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            row2.Cells.Add(new ExcelReportCell() { Value = run.Description , Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            grid.BodyRows.Add(row2);

            var row3 = new ExcelReportRow();
            row3.Cells.Add(new ExcelReportCell() { Value = "Run Id", Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            row3.Cells.Add(new ExcelReportCell() { Value = run.Id, Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            grid.BodyRows.Add(row3);

            var row4 = new ExcelReportRow();
            row4.Cells.Add(new ExcelReportCell() { Value = "Scenario Name", Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            row4.Cells.Add(new ExcelReportCell() { Value = scenario.Name, Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            grid.BodyRows.Add(row4);

            return grid;
        }

        private ExcelReportGrid SalesAreaPassPrioritiesGrid(List<PassModel> passes, int maxColumnsCount)
        {
            string daysOfWeek = "Days Of The Week";
            string startDate = "Start Date";
            string endDate = "End Date";

            var data = new Dictionary<string, object[]>();
            data.Add(daysOfWeek, new object[passes.Count]);
            data.Add(startDate, new object[passes.Count]);
            data.Add(endDate, new object[passes.Count]);

            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                data[daysOfWeek][passIndex] = GetDaysCode(passes[passIndex].PassSalesAreaPriorities.DaysOfWeek);
                data[startDate][passIndex] = GetDateTimeValue(passes[passIndex].PassSalesAreaPriorities.StartDate, passes[passIndex].PassSalesAreaPriorities.StartTime);
                data[endDate][passIndex] = GetDateTimeValue(passes[passIndex].PassSalesAreaPriorities.EndDate, passes[passIndex].PassSalesAreaPriorities.EndTime);
                foreach (var item in passes[passIndex].PassSalesAreaPriorities.SalesAreaPriorities)
                {
                    if (!data.ContainsKey(item.SalesArea))
                    {
                        data.Add(item.SalesArea, new object[passes.Count]);
                    }
                    data[item.SalesArea][passIndex] = item.Priority;
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Sales Area Pass Priorities"));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid GeneralGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();
            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].General)
                {
                    if (!data.ContainsKey(item.Description))
                    {
                        data.Add(item.Description, new object[passes.Count]);
                    }
                    data[item.Description][passIndex] = item.Value;
                    if (item.RuleId == (int)RuleID.UseSponsorExclusivity
                        || item.RuleId == (int)RuleID.UseCampaignMaxSpotRatings)
                    {
                        data[item.Description][passIndex] = item.Value == "0" ? "NO" : "YES";
                    }
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "General"));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid WeightingGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();
            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].Weightings)
                {
                    if (!data.ContainsKey(item.Description))
                    {
                        data.Add(item.Description, new object[passes.Count]);
                    }
                    data[item.Description][passIndex] = item.Value;
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Weightings"));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid ToleranceGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();
            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].Tolerances)
                {
                    if (!data.ContainsKey(item.Description))
                    {
                        data.Add(item.Description, new object[passes.Count * 4]);
                    }

                    data[item.Description][passIndex * 4] = item.Under;
                    data[item.Description][(passIndex * 4) + 1] = item.Over;
                    switch (item.ForceUnderOver)
                    {
                        case ForceOverUnder.Over:
                            data[item.Description][(passIndex * 4) + 2] = "Over";
                            break;
                        case ForceOverUnder.Under:
                            data[item.Description][(passIndex * 4) + 2] = "Under";
                            break;
                        default:
                            data[item.Description][(passIndex * 4) + 2] = "None";
                            break;
                    }
                    data[item.Description][(passIndex * 4) + 3] = item.Ignore ? "YES" : "";

                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Tolerance", "Under/Over/Force/Ignore", new[] { "U", "O", "F", "I" }));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid RulesGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();
            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].Rules)
                {
                    if (!data.ContainsKey(item.Description))
                    {
                        data.Add(item.Description, new object[passes.Count * 2]);
                    }
                    data[item.Description][passIndex * 2] = item.Value;
                    data[item.Description][(passIndex * 2) + 1] = item.PeakValue;
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Rules", "Daypart (Non-Peak / Peak)", new[] { "NP", "P" }));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid ProgrammeRepetitionsGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();

            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].ProgrammeRepetitions)
                {
                    var key = item.Minutes.ToString(CultureInfo.InvariantCulture);
                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, new object[passes.Count * 2]);

                    }
                    data[key][(passIndex * 2)] = item.Factor;
                    data[key][(passIndex * 2) + 1] = item.PeakFactor;
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Programme Repetitions", "Minutes (Non-Peak Factor / Peak Factor)", new string[] { "NPF", "PF" }));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid MinRatingPointsGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();

            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].RatingPoints)
                {
                    var key = item.SalesAreas?.Count() > 0
                        ? string.Join(", ", item.SalesAreas)
                        : "All Sales Areas";
                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, new object[passes.Count * 3]);
                    }
                    data[key][(passIndex * 3)] = item.OffPeakValue;
                    data[key][(passIndex * 3) + 1] = item.PeakValue;
                    data[key][(passIndex * 3) + 2] = item.MidnightToDawnValue;
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Min Rating Points", "Sales Areas", new string[] { "OP", "P", "M2D" }));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private ExcelReportGrid BreakExclusionsGrid(List<PassModel> passes, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();

            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].BreakExclusions)
                {
                    string breakExclusionsCode = GetBreakExclusionsCode(item);
                    if (!data.ContainsKey(breakExclusionsCode))
                    {
                        data.Add(breakExclusionsCode, new object[passes.Count]);
                    }
                    data[breakExclusionsCode][passIndex] = "YES";
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Break Exclusions"));
            grid.BodyRows.AddRange(GetBodyRows(data, GamePlanReportStyles.CourierNewFontCellStyle.Name));
            return grid;
        }

        private ExcelReportGrid SlottingLimitsGrid(List<PassModel> passes, IEnumerable<Demographic> demographics, int maxColumnsCount)
        {
            var data = new Dictionary<string, object[]>();

            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                foreach (var item in passes[passIndex].SlottingLimits)
                {
                    var demograpgic = demographics.First(d => d.ExternalRef == item.Demographs);
                    var demographicName = demograpgic!=null? demograpgic.Name: item.Demographs;
                    if (!data.ContainsKey(demographicName))
                    {
                        data.Add(demographicName, new object[passes.Count * 3]);

                    }
                    data[demographicName][(passIndex * 3)] = item.MinimumEfficiency;
                    data[demographicName][(passIndex * 3) + 1] = item.MaximumEfficiency;
                    data[demographicName][(passIndex * 3) + 2] = item.BandingTolerance;
                }
            }

            var grid = new ExcelReportGrid();
            grid.MaxColumnCount = maxColumnsCount;
            grid.HeaderRows.AddRange(GetGenericHeaderRowsForRunReport(passes, "Slotting Limits", "Demographs (Min Efficiency / Max Rank / Banding Tolerance)", new string[] { "ME", "MR", "BT" }));
            grid.BodyRows.AddRange(GetBodyRows(data));
            return grid;
        }

        private List<ExcelReportRow> GetGenericHeaderRowsForRunReport(List<PassModel> passes, string title, string subTitleFirstColumn = null, string[] subtitleRepeatColumns = null)
        {
            var row1 = new ExcelReportRow();
            row1.Cells.Add(new ExcelReportCell() { Value = title, Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.HeaderStyle.Name });
            row1.Cells.Add(new ExcelReportCell() { Value = "Value", Alignment = ExcelHorizontalAlignment.Center, StyleName = GamePlanReportStyles.HeaderStyle.Name });

            var row2 = new ExcelReportRow();
            var row3 = new ExcelReportRow();
            var row4 = new ExcelReportRow();

            row2.Cells.Add(new ExcelReportCell() { Value = "Pass#", Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            row3.Cells.Add(new ExcelReportCell() { Value = "Pass Name", Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            if (!string.IsNullOrEmpty(subTitleFirstColumn))
            {
                row4.Cells.Add(new ExcelReportCell() { Value = subTitleFirstColumn, Alignment = ExcelHorizontalAlignment.Left, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
            }
            for (var i = 0; i < passes.Count; i++)
            {
                row2.Cells.Add(new ExcelReportCell() { Value = i + 1, Alignment = ExcelHorizontalAlignment.Center, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
                row3.Cells.Add(new ExcelReportCell() { Value = passes[i].Name, Alignment = ExcelHorizontalAlignment.Center, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
                if (subtitleRepeatColumns != null)
                {
                    foreach (var subtitle in subtitleRepeatColumns)
                    {
                        row4.Cells.Add(new ExcelReportCell() { Value = subtitle, Alignment = ExcelHorizontalAlignment.Center, StyleName = GamePlanReportStyles.LightHeaderStyle.Name });
                    }
                }

            }
            var rows = new List<ExcelReportRow>() { row1, row2, row3 };
            if (row4.HasData)
            {
                rows.Add(row4);
            }
            return rows;
        }

        private List<ExcelReportRow> GetBodyRows(Dictionary<string, object[]> data,string firstColumnStyleName = null)
        {
            var rows = new List<ExcelReportRow>();
            var alternativeColor = true;
            foreach(var item in data)
            {

                alternativeColor = !alternativeColor;
                var row = new ExcelReportRow();
                row.Cells.Add(new ExcelReportCell() { Value = ValueOf(item.Key), Alignment = ExcelHorizontalAlignment.Left, StyleName = string.IsNullOrEmpty(firstColumnStyleName) ? GamePlanReportStyles.DataCellStyle.Name : firstColumnStyleName, AlternateBackground = alternativeColor });
                foreach(var v in item.Value)
                {
                    row.Cells.Add(new ExcelReportCell() { Value = ValueOf(v), Alignment = ExcelHorizontalAlignment.Center, StyleName = GamePlanReportStyles.DataCellStyle.Name , AlternateBackground = alternativeColor });
                }
                rows.Add(row);
            }
            return rows;
        }

        private object ValueOf(object value)
        {
            if (value != null && value is string)
            {
                int i;
                if (int.TryParse(value.ToString(), out i))
                {
                    return i;
                }

                double d;
                if (double.TryParse(value.ToString(), out d))
                {
                    return d;
                }
            }
            return value;
        }

        private string GetDaysCode(List<DayOfWeek> selectableDays)
        {
            string result = "";
            var values = new Dictionary<DayOfWeek, string>() {
                {DayOfWeek.Sunday , "S" }
                ,{DayOfWeek.Monday , "M" }
                ,{DayOfWeek.Tuesday , "T" }
                ,{DayOfWeek.Wednesday , "W" }
                ,{DayOfWeek.Thursday , "H" }
                ,{DayOfWeek.Friday , "F" }
                ,{DayOfWeek.Saturday , "A" }
            };
            foreach (var value in values)
            {
                result += selectableDays.Contains(value.Key) ? value.Value : "";
            }
            return result + new string(' ', 7 - result.Length);
        }

        private string GetDaysCode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            var allDays = new List<DayOfWeek>() {
                DayOfWeek.Monday
                ,DayOfWeek.Tuesday
                ,DayOfWeek.Wednesday
                ,DayOfWeek.Thursday
                ,DayOfWeek.Friday
                ,DayOfWeek.Saturday
                ,DayOfWeek.Sunday
            };
            var selectedDays = new List<DayOfWeek>();
            for (var i = 0; i < allDays.Count; i++)
            {
                if (value[i] == '1')
                {
                    selectedDays.Add(allDays[i]);
                }
            }
            return GetDaysCode(selectedDays);
        }

        private string GetDateTimeValue(DateTime? date, TimeSpan? time)
        {
            if (date.HasValue)
            {
                var value = date.Value;
                var format = "{0:MMMM dd, yyyy}";
                if (time.HasValue)
                {
                    value.Add(time.Value);
                    format = "{0:MMMM dd, yyyy - hh:mm:ss}";
                }
                return string.Format(CultureInfo.InvariantCulture, format , value);
            }
            return string.Empty;
        }

        private string GetBreakExclusionsCode(BreakExclusionModel item)
        {
            var dateFormat = "yyyy.MM.dd HH:mm:ss";
            var startDate = item.StartTime.HasValue ? item.StartDate.Add(item.StartTime.Value) : item.StartDate;
            var endDate =item.EndTime.HasValue ? item.EndDate.Add(item.EndTime.Value) : item.EndDate;
            return $"BE {item.SalesArea} {GetDaysCode(item.SelectableDays)} {startDate.ToString(dateFormat, CultureInfo.InvariantCulture)} - {endDate.ToString(dateFormat, CultureInfo.InvariantCulture)}";
        }
    }
}
