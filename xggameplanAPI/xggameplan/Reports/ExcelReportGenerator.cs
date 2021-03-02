using System;
using System.Collections.Generic;
using System.Drawing;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.ReportSystem.Excel;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder;
using xggameplan.core.ReportGenerators;
using xggameplan.Model;
using xggameplan.Reports.Models;

namespace xggameplan.Reports
{
    public class ExcelReportGenerator : IExcelReportGenerator
    {
        private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

        public byte[] GetSmoothFailuresExcelReport(ExcelReportSmoothFailuresModel reportModel)
        {
            using (var builder = new ExcelReportBuilder(new ExcelStyleApplier()))
            {
                return builder
                    .PredefineStyles(GamePlanReportStyles.SmoothFailuresReportPredefineStyles)
                    .Sheet("Smooth Failures", sb => GetSmoothFailures(sb, reportModel.SmoothFailures))
                    .Sheet("Report Info", sb => GetReportInfo(sb, "Smooth Failures", reportModel.Run, reportModel.ReportDate))
                    .Save();
            }
        }

        public byte[] GetRunExcelReport(ExcelReportRunModel run)
        {
            using (var reportBuilder = new ExcelReportBuilder(new ExcelStyleApplier()))
            {
                reportBuilder.PredefineStyles(GamePlanReportStyles.RunReportPredefineStyles);
                foreach (var scenario in run.Scenarios)
                {
                    reportBuilder.Sheet(scenario.Name, sb =>
                    {
                        var columnsCount = scenario.MaxColumnsCount;
                        SetColumnsWidthForRunReport(sb, columnsCount);
                        WriteGrid(sb, scenario.ScenarioDetails);
                        sb.Skip();
                        WriteGrid(sb, scenario.SalesAreaPassPriorities);
                        sb.Skip();
                        WriteGrid(sb, scenario.General);
                        sb.Skip();
                        WriteGrid(sb, scenario.Weighting);
                        sb.Skip();
                        WriteGrid(sb, scenario.Tolerance);
                        sb.Skip();
                        WriteGrid(sb, scenario.Rules);
                        sb.Skip();
                        WriteGrid(sb, scenario.ProgrammeRepetitions);
                        sb.Skip();
                        WriteGrid(sb, scenario.MinRatingPoints);
                        sb.Skip();
                        WriteGrid(sb, scenario.BreakExclusions);
                        sb.Skip();
                        WriteGrid(sb, scenario.SlottingLimits);
                    });
                }
                return reportBuilder.Save();
            }
        }

        private void GetSmoothFailures(ISheetBuilder sheetBuilder, List<SmoothFailureExtendedModel> smoothFailures)
        {
            sheetBuilder.Freeze(1, 2)
                .DataContent(smoothFailures, GetSmoothFailureExportConfig().BuildConfiguration())
                .AutoFitColumns(1, 21);
            if (smoothFailures.Count == 0)
            {
                WriteNoDataRow(sheetBuilder, 21);
            }
        }

        private ExcelConfigurationBuilder<SmoothFailureExtendedModel> GetSmoothFailureExportConfig()
        {
            return new ExcelConfigurationBuilder<SmoothFailureExtendedModel>()
                 .DefaultStyle(GamePlanReportStyles.DataCellStyle.Name)
                 .HeaderStyle(GamePlanReportStyles.HeaderStyle.Name)
                 .AlternateBackgroundColors(new[] { Color.FromArgb(0xF5, 0xF5, 0xF5), Color.Transparent })
                 .ForMember(m => m.SmoothFailureMessages, c => c.Header("").Order(1))
                 .ForMember(m => m.BreakDateTime, c => c.Order(2))
                 .ForMember(m => m.BreakDate, c => c.Order(3))
                 .ForMember(m => m.BreakTime, c => c.Order(4))
                 .ForMember(m => m.CampaignGroup, c => c.Order(5))
                 .ForMember(m => m.CampaignName, c => c.Header("Campaign").Order(6))
                 .ForMember(m => m.ClashDescription, c => c.Order(7))
                 .ForMember(m => m.ClearanceCode, c => c.Order(8))
                 .ForMember(m => m.AdvertiserName, c => c.Header("Client").Order(9))
                 .ForMember(m => m.ExternalBreakRef, c => c.Order(10))
                 .ForMember(m => m.ExternalCampaignRef, c => c.Order(11))
                 .ForMember(m => m.ExternalSpotReference, c => c.Header("External Spot Ref").Order(12))
                 .ForMember(m => m.IndustryCode, c => c.Order(13))
                 .ForMember(m => m.ProductName, c => c.Order(14))
                 .ForMember(m => m.RestrictionDays, c => c.Order(15))
                 .ForMember(m => m.RestrictionStartDate, c => c.Order(16))
                 .ForMember(m => m.RestrictionStartTime, c => c.Order(17))
                 .ForMember(m => m.RestrictionEndDate, c => c.Order(18))
                 .ForMember(m => m.RestrictionEndTime, c => c.Order(19))
                 .ForMember(m => m.SalesArea, c => c.Order(20))
                 .ForMember(m => m.SpotLength, c => c.Order(21));
        }

        private void GetReportInfo(ISheetBuilder sb, string reportName, Run run, DateTime reportDate)
        {
            sb.Column(1, cb => cb.Width(20))
            .Column(2, cb => cb.Width(35))
            .Block(bb =>
            {
                bb.Add("Report Date").Style(GamePlanReportStyles.HeaderStyle.Name);
                bb.Add(reportDate.ToString(DateTimeFormat)).Style(GamePlanReportStyles.HeaderStyle.Name);
            })
            .Block(bb =>
            {
                bb.Add("Report Name").Style(GamePlanReportStyles.LightHeaderStyle.Name);
                bb.Add(reportName).Style(GamePlanReportStyles.LightHeaderStyle.Name);
            })
            .Block(bb =>
            {
                bb.Add("Run Name").Style(GamePlanReportStyles.LightHeaderStyle.Name);
                bb.Add(run.Description).Style(GamePlanReportStyles.LightHeaderStyle.Name);
            })
            .Block(bb =>
            {
                bb.Add("Run Id").Style(GamePlanReportStyles.LightHeaderStyle.Name);
                bb.Add(run.Id).Style(GamePlanReportStyles.LightHeaderStyle.Name);
            });
            if (run.ExecuteStartedDateTime.HasValue)
            {
                sb.Block(bb =>
                {
                    bb.Add("Run Execute Date").Style(GamePlanReportStyles.LightHeaderStyle.Name);
                    bb.Add(run.ExecuteStartedDateTime.Value.ToString(DateTimeFormat)).Style(GamePlanReportStyles.LightHeaderStyle.Name);
                });
            }
        }

        private void WriteGrid(ISheetBuilder sb, ExcelReportGrid grid)
        {
            grid.HeaderRows.ForEach(row => WriteRow(sb, grid, row));
            if (grid.HasData)
            {
                grid.BodyRows.ForEach(row => WriteRow(sb, grid, row));
            }
            else
            {
                WriteNoDataRow(sb, grid.MaxColumnCount);
            }
        }

        private void WriteRow(ISheetBuilder sb, ExcelReportGrid grid, ExcelReportRow row)
        {
            sb.Block(bb =>
            {
                for (var i = 0; i < row.Cells.Count; i++)
                {
                    var cell = row.Cells[i];
                    var colSpan = 1;
                    if (i > 0)
                    {
                        colSpan = (grid.MaxColumnCount-1) / (row.Cells.Count - 1);
                    }
                    writeCell(bb, cell, colSpan);
                }
            });
        }

        private void writeCell(IBlockBuilder bb, ExcelReportCell cell, int colSpan)
        {
             var b = bb.Add(cell.Value)
                .HAlign(cell.Alignment)
                .Style(string.IsNullOrEmpty(cell.StyleName) ? GamePlanReportStyles.DataCellStyle.Name : cell.StyleName)
                .ColSpan(colSpan);

            if (cell.AlternateBackground)
            {
                b.Background(Color.FromArgb(0xF5, 0xF5, 0xF5));
            }
        }

        private void WriteNoDataRow(ISheetBuilder sb, int maxColumnCount)
        {
            var cell = new ExcelReportCell
            {
                Value = "No data",
                Alignment = ExcelHorizontalAlignment.Center,
                StyleName = GamePlanReportStyles.EmptyCellStyle.Name
            };

            sb.Block(bb =>
            {
                writeCell(bb, cell, maxColumnCount);
            });
        }

        private void SetColumnsWidthForRunReport(ISheetBuilder sb, int columnsCount)
        {
            sb.Column(1, cb => cb.Width(55));
            for (int i = 1; i < columnsCount; i++)
            {
                sb.Column(i + 1, cb => cb.Width(2.5));
            }
        }

    }
}
