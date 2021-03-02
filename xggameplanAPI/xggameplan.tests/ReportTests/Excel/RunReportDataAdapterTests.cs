using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using Newtonsoft.Json;
using NodaTime;
using NUnit.Framework;
using xggameplan.core.Configuration;
using xggameplan.core.ReportGenerators;
using xggameplan.Model;
using xggameplan.Reports.DataAdapters;
using xggameplan.Reports.Models;

namespace xggameplan.tests.ReportTests.Excel
{
    [TestFixture(Category = "Run report data adapter")]
    public class RunReportDataAdapterTests
    {
        private int _index;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);
            _index = 1;
        }

        [Test]
        public void RunReportDataAdapterSetCorrectNumberOfScenarios()
        {
            var model = new RunExcelReportDataAdapter().Map(GetRun(), GeDemographics(), GetReportDate());
            Assert.AreEqual(1, model.Scenarios.Count);
        }

        [Test]
        public void RunReportDataAdapterSetTheMaxColumnsCountProperly()
        {
            var model = new RunExcelReportDataAdapter().Map(GetRun(), GeDemographics(), GetReportDate());
            Assert.AreEqual(37, model.Scenarios[0].MaxColumnsCount);
        }

        [Test]
        public void RunReportDataAdapterSetTheScenarioDetailsDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "ScenarioDetails");
                AssertGridData(expectedData, scenario.ScenarioDetails);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheSalesAreaPassPrioritiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "SalesAreaPassPriorities");
                AssertGridData(expectedData, scenario.SalesAreaPassPriorities);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheGeneralPropertiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "General");
                AssertGridData(expectedData, scenario.General);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheWeightingsPropertiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "Weighting");
                AssertGridData(expectedData, scenario.Weighting);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheTolerancePropertiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "Tolerance");
                AssertGridData(expectedData, scenario.Tolerance);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheRulesPropertiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "Rules");
                AssertGridData(expectedData, scenario.Rules);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheProgrammeRepetitionsPropertiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "ProgrammeRepetitions");
                AssertGridData(expectedData, scenario.ProgrammeRepetitions);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheBreakExclusionsPropertiesDataProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                var expectedData = GetExpectedData(_index++, "BreakExclusions");
                AssertGridData(expectedData, scenario.BreakExclusions);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheScenarioDetailsStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.ScenarioDetails, ExcelHorizontalAlignment.Left, GamePlanReportStyles.LightHeaderStyle.Name, GamePlanReportStyles.LightHeaderStyle.Name, false);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheSalesAreaPassPrioritiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.SalesAreaPassPriorities, ExcelHorizontalAlignment.Center, GamePlanReportStyles.DataCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheGeneralPropertiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.General, ExcelHorizontalAlignment.Center, GamePlanReportStyles.DataCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheWeightingsPropertiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.Weighting, ExcelHorizontalAlignment.Center, GamePlanReportStyles.DataCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheTolerancePropertiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.Tolerance, ExcelHorizontalAlignment.Center, GamePlanReportStyles.DataCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheRulesPropertiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.Rules, ExcelHorizontalAlignment.Center, GamePlanReportStyles.DataCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheProgrammeRepetitionsPropertiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.ProgrammeRepetitions, ExcelHorizontalAlignment.Center, GamePlanReportStyles.DataCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresRunObjectProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(GetRunId(), excelReportRunModel.Run.Id);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresReportDateProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(GetReportDate(), excelReportRunModel.ReportDate);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresDataProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(2, excelReportRunModel.SmoothFailures.Count);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresMessagesProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual("Message1, Message2, 3", excelReportRunModel.SmoothFailures[0].SmoothFailureMessages);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresExternalSpotReferenceTypeSetToIntProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual("System.Int32", excelReportRunModel.SmoothFailures[0].ExternalSpotReference.GetType().ToString());
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresExternalSpotReferenceTypeSetToStringProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual("System.String", excelReportRunModel.SmoothFailures[1].ExternalSpotReference.GetType().ToString());
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresNullRestrictionStartDateIsSetToEmptyString()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(string.Empty, excelReportRunModel.SmoothFailures[1].RestrictionStartDate);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresNullRestrictionEndDateIsSetToEmptyString()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(string.Empty, excelReportRunModel.SmoothFailures[1].RestrictionEndDate);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresNullRestrictionStartTimeIsSetToEmptyString()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(string.Empty, excelReportRunModel.SmoothFailures[1].RestrictionStartTime);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresNullRestrictionEndTimeIsSetToEmptyString()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual(string.Empty, excelReportRunModel.SmoothFailures[1].RestrictionEndTime);
        }

        [Test]
        public void RunReportDataAdapterSetTheSmoothFailuresPropertiesProperly()
        {
            var excelReportRunModel = GetExcelReportSmoothFailuresModel();
            Assert.AreEqual("Message1, Message2, 3", excelReportRunModel.SmoothFailures[0].SmoothFailureMessages);
            Assert.AreEqual("20/12/2003 14:13:22", excelReportRunModel.SmoothFailures[0].BreakDateTime);
            Assert.AreEqual("20/12/2003", excelReportRunModel.SmoothFailures[0].BreakDate);
            Assert.AreEqual("14:13:22", excelReportRunModel.SmoothFailures[0].BreakTime);
            Assert.AreEqual("0:00:10:00", excelReportRunModel.SmoothFailures[0].SpotLength.ToString());
            Assert.AreEqual("ExternalBreakRef", excelReportRunModel.SmoothFailures[0].ExternalBreakRef);
            Assert.AreEqual("ExternalCampaignRef", excelReportRunModel.SmoothFailures[0].ExternalCampaignRef);
            Assert.AreEqual(123, excelReportRunModel.SmoothFailures[0].ExternalSpotReference);
            Assert.AreEqual("CampaignName", excelReportRunModel.SmoothFailures[0].CampaignName);
            Assert.AreEqual("CampaignGroup", excelReportRunModel.SmoothFailures[0].CampaignGroup);
            Assert.AreEqual("AdvertiserName", excelReportRunModel.SmoothFailures[0].AdvertiserName);
            Assert.AreEqual("ProductName", excelReportRunModel.SmoothFailures[0].ProductName);
            Assert.AreEqual("ClashDescription", excelReportRunModel.SmoothFailures[0].ClashDescription);
            Assert.AreEqual("IndustryCode", excelReportRunModel.SmoothFailures[0].IndustryCode);
            Assert.AreEqual("ClearanceCode", excelReportRunModel.SmoothFailures[0].ClearanceCode);
            Assert.AreEqual("03/02/0001", excelReportRunModel.SmoothFailures[0].RestrictionStartDate);
            Assert.AreEqual("06/05/0004", excelReportRunModel.SmoothFailures[0].RestrictionEndDate);
            Assert.AreEqual("07:08:09", excelReportRunModel.SmoothFailures[0].RestrictionStartTime);
            Assert.AreEqual("10:11:12", excelReportRunModel.SmoothFailures[0].RestrictionEndTime);
            Assert.AreEqual("SalesArea", excelReportRunModel.SmoothFailures[0].SalesArea);
            Assert.AreEqual("d1,d2", excelReportRunModel.SmoothFailures[0].RestrictionDays);
        }

        [Test]
        public void RunReportDataAdapterSetTheBreakExclusionsPropertiesStyleProperly()
        {
            var excelReportRunModel = GetExcelReportRunModel();
            excelReportRunModel.Scenarios.ForEach(scenario =>
            {
                AssertGridStyle(scenario.BreakExclusions, ExcelHorizontalAlignment.Center, GamePlanReportStyles.CourierNewFontCellStyle.Name, GamePlanReportStyles.DataCellStyle.Name, true);
            });
        }

        private void AssertGridData(string[] expectedData, ExcelReportGrid grid)
        {
            var index = 0;
            grid.HeaderRows.ForEach(r =>
            {
                Assert.AreEqual(expectedData[index++], GetRowValue(r));
            });

            grid.BodyRows.ForEach(r =>
            {
                Assert.AreEqual(expectedData[index++], GetRowValue(r));
            });
        }

        private void AssertGridStyle(ExcelReportGrid grid, ExcelHorizontalAlignment secondColumnAlignmnet, string firstColumnStyleName, string otherColumnStyleName, bool alternateColor)
        {
            var rowIndex = 0;
            grid.HeaderRows.ForEach(r =>
            {
                var cellIndex = 0;
                r.Cells.ForEach(cell => AssertHeaderCellStyle(cell, secondColumnAlignmnet, rowIndex, cellIndex++));
                rowIndex++;
            });

            rowIndex = 0;
            grid.BodyRows.ForEach(r =>
            {
                var cellIndex = 0;
                r.Cells.ForEach(cell => AssertBodyCellStyle(cell, firstColumnStyleName, otherColumnStyleName, secondColumnAlignmnet, rowIndex, cellIndex++, alternateColor));
                rowIndex++;
            });
        }

        private void AssertBodyCellStyle(ExcelReportCell cell, string firstColumnStyleName, string otherColumnStyleName, ExcelHorizontalAlignment secondColumnAlignmnet, int rowIndex, int cellIndex, bool alternateColor)
        {
            var expectedAlignment = cellIndex == 0 ? ExcelHorizontalAlignment.Left : secondColumnAlignmnet;
            var styleName = cellIndex == 0 ? firstColumnStyleName : otherColumnStyleName;
            var expectedAlternateColor = alternateColor ? rowIndex % 2 == 1 : false;
            Assert.AreEqual(expectedAlignment, cell.Alignment);
            Assert.AreEqual(styleName, cell.StyleName);
            Assert.AreEqual(expectedAlternateColor, cell.AlternateBackground);
        }

        private void AssertHeaderCellStyle(ExcelReportCell cell, ExcelHorizontalAlignment secondColumnAlignmnet, int rowIndex, int cellIndex)
        {
            var expectedAlignment = cellIndex == 0 ? ExcelHorizontalAlignment.Left : secondColumnAlignmnet;
            var expectedStyleName = rowIndex == 0 ? GamePlanReportStyles.HeaderStyle.Name : GamePlanReportStyles.LightHeaderStyle.Name;
            Assert.AreEqual(expectedAlignment, cell.Alignment);
            Assert.AreEqual(expectedStyleName, cell.StyleName);
            Assert.AreEqual(false, cell.AlternateBackground);
        }

        private string GetRowValue(ExcelReportRow r)
        {
            return String.Join(",", r.Cells.Select(c => c.Value != null ? c.Value.ToString() : "").ToArray());
        }

        private RunModel GetRun()
        {
            return JsonConvert.DeserializeObject<RunModel>(File.ReadAllText(GetDataPath() + @"\ReportTests\Excel\Data\runModel.json"));
        }

        private string[] GetExpectedData(int index, string name)
        {
            string fileName = string.Format(CultureInfo.InvariantCulture, @"{0}\ReportTests\Excel\Data\Scenario{1}_{2}.csv", GetDataPath(), index, name);
            return File.ReadAllLines(fileName);
        }

        private List<Demographic> GeDemographics()
        {
            return JsonConvert.DeserializeObject<List<Demographic>>(File.ReadAllText(GetDataPath() + @"\ReportTests\Excel\Data\demographics.json"));
        }

        private string GetDataPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var uri = new UriBuilder(assembly.CodeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }

        private DateTime GetReportDate()
        {
            return new DateTime(2003, 12, 20, 14, 13, 22);
        }

        private ExcelReportRunModel GetExcelReportRunModel()
        {
            return new RunExcelReportDataAdapter().Map(GetRun(), GeDemographics(), GetReportDate());
        }

        private ExcelReportSmoothFailuresModel GetExcelReportSmoothFailuresModel()
        {
            var run = new Run() { Id = GetRunId() };
            var smoothFailuresModel = GetSmoothFailuresModel();
            var failureMessages = GetFailureMessages();
            return new RunExcelReportDataAdapter().Map(run, smoothFailuresModel, failureMessages, GetReportDate(), _mapper);
        }

        private List<SmoothFailureMessage> GetFailureMessages()
        {
            var result = new List<SmoothFailureMessage>();
            result.Add(new SmoothFailureMessage
            {
                Id = 1,
                Description = new Dictionary<string, string>() { { "ENG", "Message1" }, { "Foo", "Bar" } }
            });
            result.Add(new SmoothFailureMessage
            {
                Id = 2,
                Description = new Dictionary<string, string>() { { "ENG", "Message2" }, { "Foo", "Bar" } }
            });
            result.Add(new SmoothFailureMessage
            {
                Id = 3,
                Description = new Dictionary<string, string>() { { "Foo", "Bar" } }
            });

            return result;
        }

        private List<SmoothFailureModel> GetSmoothFailuresModel()
        {
            var result = new List<SmoothFailureModel>();
            result.Add(new SmoothFailureModel
            {
                SalesArea = "SalesArea",
                SalesAreaShortName = "SalesAreaShortName",
                ExternalSpotRef = "123",
                ExternalBreakRef = "ExternalBreakRef",
                ExternalCampaignRef = "ExternalCampaignRef",
                CampaignName = "CampaignName",
                CampaignGroup = "CampaignGroup",
                AdvertiserIdentifier = "AdvertiserIdentifier",
                AdvertiserName = "AdvertiserName",
                ProductName = "ProductName",
                ClashCode = "ClashCode",
                ClashDescription = "ClashDescription",
                IndustryCode = "IndustryCode",
                ClearanceCode = "ClearanceCode",
                BreakDateTime = GetReportDate(),
                MessageIds = new List<int>() { 1, 2, 3 },
                SpotLength = Duration.FromMinutes(10),
                RestrictionStartDate = new DateTime(1, 2, 3),
                RestrictionEndDate = new DateTime(4, 5, 6),
                RestrictionStartTime = new TimeSpan(7, 8, 9),
                RestrictionEndTime = new TimeSpan(10, 11, 12),
                RestrictionDays = "d1,d2"
            });
            result.Add(new SmoothFailureModel
            {
                SalesArea = "SalesArea",
                SalesAreaShortName = "SalesAreaShortName",
                ExternalSpotRef = "Test123",
                ExternalBreakRef = "ExternalBreakRef",
                ExternalCampaignRef = "ExternalCampaignRef",
                CampaignName = "CampaignName",
                CampaignGroup = "CampaignGroup",
                AdvertiserIdentifier = "AdvertiserIdentifier",
                AdvertiserName = "AdvertiserName",
                ProductName = "ProductName",
                ClashCode = "ClashCode",
                ClashDescription = "ClashDescription",
                IndustryCode = "IndustryCode",
                ClearanceCode = "ClearanceCode",
                BreakDateTime = GetReportDate(),
                MessageIds = new List<int>() { 1, 2, 3, 4 },
                SpotLength = Duration.FromMinutes(10),
                RestrictionDays = "d1,d2"
            });
            return result;
        }

        private Guid GetRunId()
        {
            return new Guid("9C59BF69-1062-4B7B-9947-260550A3C675");
        }
    }
}
