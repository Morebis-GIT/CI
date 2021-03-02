using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using NUnit.Framework;
using xggameplan.tests.ValidationTests.Runs.Data;
using xggameplan.Validations.Runs;

namespace xggameplan.tests.ValidationTests.Runs
{
    [TestFixture(Category = "Validations :: RunsValidations")]
    public class RunsValidationsTests
    {
        private List<Scenario> _dummyScenarioList;
        private Scenario _dummyScenario;
        private List<CampaignPassPriority> _dummyCampaignPassPriorityList;
        private List<PassReference> _dummyPassReferenceList;
        private TenantSettings _dummyTenantSettings;
        private PassSalesAreaPriority _dummyPassSalesAreaPriorityAllTrue;
        private PassSalesAreaPriority _dummyPassSalesAreaPriorityAllFalse;
        private Run _dummyRun;
        private Pass _dummyPass;
        private List<SalesAreaPriority> _dummyRunSalesAreasPriority;
        private List<SalesArea> _dummySalesAreas;
        private List<string> _dummyNotExcludedSalesAreas;

        [SetUp]
        public void Init()
        {
            _dummyScenarioList = RunsValidationsDummyData.GetScenarios();
            _dummyScenario = RunsValidationsDummyData.GetScenario();
            _dummyCampaignPassPriorityList = RunsValidationsDummyData.GetCampaignPassPriorities();
            _dummyPassReferenceList = RunsValidationsDummyData.GetPassReferences();
            _dummyTenantSettings = RunsValidationsDummyData.GetTenantSettings();
            _dummyPassSalesAreaPriorityAllTrue = RunsValidationsDummyData.GetPassSalesAreaPriorityWithFlagsTrue();
            _dummyPassSalesAreaPriorityAllFalse = RunsValidationsDummyData.GetPassSalesAreaPriorityWithFlagsFalse();
            _dummyRun = RunsValidationsDummyData.GetRun();
            _dummyPass = RunsValidationsDummyData.GetPass();
            _dummyRunSalesAreasPriority = RunsValidationsDummyData.GetRunSalesAreaPriorities();
            _dummySalesAreas = RunsValidationsDummyData.GetSalesAreas();
            _dummyNotExcludedSalesAreas = _dummyRunSalesAreasPriority
                .Where(o => o.Priority != SalesAreaPriorityType.Exclude)
                .Select(o => o.SalesArea)
                .ToList();
        }

        #region ValidateCampaignPassPriorities Tests

        [Test(Description = "Given valid list of CampaignPassPriority and PassReference then error message should be empty")]
        public void GivenValidListOfCampaignPassPriorityAndPassReferenceThenErrorMessageShouldBeEmpty()
        {
            // Act
            var result = RunsValidations.ValidateCampaignPassPriorities(_dummyCampaignPassPriorityList, _dummyPassReferenceList);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Test(Description = "Given empty list of CampaignPassPriority and valid list of PassReference then error message should be empty")]
        public void GivenEmptyListOfCampaignPassPriorityAndValidListOfPassReferenceThenErrorMessageShouldBeEmpty()
        {
            // Arrange
            _dummyCampaignPassPriorityList = new List<CampaignPassPriority>();

            // Act
            var result = RunsValidations.ValidateCampaignPassPriorities(_dummyCampaignPassPriorityList, _dummyPassReferenceList);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Test(Description = "Given valid list of CampaignPassPriority with invalid item and valid list of PassReference then error message should not be empty")]
        public void GivenValidListOfCampaignPassPriorityWithInvalidItemAndValidListOfPassReferenceThenErrorMessageShouldNotBeEmpty()
        {
            // Arrange
            var presetGuid = Guid.NewGuid();
            _dummyCampaignPassPriorityList[1].Campaign.ExternalId = presetGuid.ToString();
            _dummyCampaignPassPriorityList[1].PassPriorities = new List<PassPriority>();
            int itemsExpected = RunsValidationsDummyData.NumberOfItemsToCreate;

            // Act
            var result = RunsValidations.ValidateCampaignPassPriorities(_dummyCampaignPassPriorityList, _dummyPassReferenceList);

            // Assert
            _ = result.Should().Contain($"CampaignExternalId: {presetGuid} should contain {itemsExpected.ToString()} PassPriorities.");
        }

        [Test(Description = "Given valid list of CampaignPassPriority and invalid list of PassReference then error message should not be empty")]
        public void GivenValidListOfCampaignPassPriorityAndInvalidListOfPassReferenceThenErrorMessageShouldNotBeEmpty()
        {
            // Arrange
            _dummyPassReferenceList.RemoveAt(1);
            var expectedErrorMessage = new StringBuilder();
            foreach (var cpp in _dummyCampaignPassPriorityList)
            {
                _ = expectedErrorMessage.AppendLine($"CampaignExternalId: {cpp.Campaign.ExternalId} should contain {_dummyPassReferenceList.Count} PassPriorities.");
            }

            // Act
            var result = RunsValidations.ValidateCampaignPassPriorities(_dummyCampaignPassPriorityList, _dummyPassReferenceList);

            // Assert
            _ = result.Should().Be(expectedErrorMessage.ToString());
        }

        #endregion ValidateCampaignPassPriorities Tests

        #region ValidateScenario Tests

        [Test(Description = "Given Scenario is valid then error message should be empty")]
        public void GivenScenarioIsValidThenErrorMessageShouldBeEmpty()
        {
            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Test(Description = "Given Scenario is null then error message should be empty")]
        public void GivenScenarioIsNullThenErrorMessageShouldBeEmpty()
        {
            // Arrange
            _dummyScenario = null;

            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Test(Description = "Given Scenario is valid and has no CampaignPassPriorities then error message should be empty")]
        public void GivenScenarioIsValidAndHasNoCampaignPassPrioritiesThenErrorMessageShouldBeEmpty()
        {
            // Arrange
            _dummyScenario.CampaignPassPriorities = null;

            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Test(Description = "Given Scenario has empty name then error message should not be empty")]
        public void GivenScenarioHasEmptyNameThenErrorMessageShouldNotBeEmpty()
        {
            // Arrange
            _dummyScenario.Name = string.Empty;

            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().Contain("Scenario name is required.");
        }

        [Test(Description = "Given Scenario has no passes then error message should not be empty")]
        public void GivenScenarioHasNoPassesThenErrorMessageShouldNotBeEmpty()
        {
            // Arrange
            _dummyScenario.Passes = null;

            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().Contain($"Scenario: {_dummyScenario.Name} contains no passes, One or more passes is required.");
        }

        [Test(Description = "Given Scenario has duplicate passes then error message should not be empty")]
        public void GivenScenarioHasDuplicatePassesThenErrorMessageShouldNotBeEmpty()
        {
            // Arrange
            _dummyPassReferenceList[1].Id = _dummyPassReferenceList[0].Id;
            _dummyScenario.Passes = _dummyPassReferenceList;

            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().Contain($"Scenario: {_dummyScenario.Name} contains duplicate passes.");
        }

        [Test(Description = "Given Scenario has no name or passes then error message should not be empty")]
        public void GivenScenarioHasNoNameOrPassesThenErrorMessageShouldNotBeEmpty()
        {
            // Arrange
            _dummyScenario.Name = string.Empty;
            _dummyScenario.Passes = null;
            var noName = "with no Name.";

            // Act
            var result = RunsValidations.ValidateScenario(_dummyScenario);

            // Assert
            _ = result.Should().Contain("Scenario name is required.");
            _ = result.Should().Contain($"Scenario: {noName} contains no passes, One or more passes is required.");
        }

        #endregion ValidateScenario Tests

        #region ValidateScenarios Tests

        [Test(Description = "Given Scenario list is valid then exception should not be thrown")]
        public void GivenScenarioListIsValidThenExceptionShouldNotBeThrown()
        {
            // Act
            Action action = () => RunsValidations.ValidateScenarios(_dummyScenarioList);

            // Assert
            action.Should().NotThrow();
        }

        [Test(Description = "Given Scenario list has invalid items then exception should be thrown with correct message")]
        public void GivenScenarioListHasInvalidItemsThenExceptionShouldBeThrownWithCorrectMessage()
        {
            // Arrange
            _dummyScenarioList[0].Name = string.Empty;
            _dummyScenario.Passes = null;
            _dummyScenarioList.Add(_dummyScenario);

            var expectedErrorMessage = new StringBuilder();
            _ = expectedErrorMessage.Append("Scenario name is required.");
            _ = expectedErrorMessage.AppendLine($"Scenario: {_dummyScenario.Name} contains no passes, One or more passes is required.");

            // Act
            Action act = () => RunsValidations.ValidateScenarios(_dummyScenarioList);

            // Assert
            _ = act.Should().Throw<Exception>().WithMessage(expectedErrorMessage.ToString());
        }

        #endregion ValidateScenarios Tests

        #region ValidatePassSalesAreaPrioritiesDaypart Tests

        [Test(Description = "Given a specific datepart value when start and end times are passed in then return message should reflect validity of passed in time")]
        [TestCaseSource(typeof(RunsValidationsTestData), "ValidatePassSalesAreaPrioritiesDaypartTestCases")]
        public void GivenASpecificDatepartValueWhenStartAndEndTimesArePassedInThenReturnMessageShouldReflectValidityOfPassedInTime
            (Dayparts daypart, TimeSpan runStartTime, TimeSpan runEndTime, TenantSettings tenantSettings, string passName, string expectedResult)
        {
            // Act
            var result = RunsValidations.ValidatePassSalesAreaPrioritiesDaypart(daypart, runStartTime, runEndTime, tenantSettings, passName);

            // Assert
            _ = result.Should().Be(expectedResult);
        }

        #endregion ValidatePassSalesAreaPrioritiesDaypart Tests

        #region ValidatePassSalesAreaPrioritiesDayparts Tests

        [Test(Description = "Given all Daypart flags are turned on in the Pass when start and end times are standard air time then return message should be empty")]
        public void GivenAllDaypartFlagsAreTurnedOnInThePassWhenStartAndEndTimesAreStandardAirTimeThenReturnMessageShouldBeEmpty()
        {
            // Arrange
            var runStartTime = new TimeSpan(0, 6, 0, 0);
            var runEndTime = new TimeSpan(1, 5, 59, 59);

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPrioritiesDayparts(
                RunsValidationsDummyData.PassName,
                _dummyPassSalesAreaPriorityAllTrue,
                runStartTime,
                runEndTime,
                _dummyTenantSettings);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Test(Description = @"Given all Daypart flags are turned on in the Pass when start and end times are constricted then return message should be an off peak error,
                    because that is checked first")]
        public void GivenAllDaypartFlagsAreTurnedOnInThePassWhenStartAndEndTimesAreConstrictedThenReturnMessageShouldBeACorrectError()
        {
            // Arrange
            var runStartTime = new TimeSpan(3, 0, 0);
            var runEndTime = new TimeSpan(4, 0, 0);
            string passName = RunsValidationsDummyData.PassName;
            var messageBuilder = new StringBuilder();
            _ = messageBuilder.AppendLine($"Pass: {passName}, SalesAreaPriorities {Dayparts.OffPeak.ToString()} daypart is not within run StartTime/EndTime");
            _ = messageBuilder.AppendLine($"Pass: {passName}, SalesAreaPriorities {Dayparts.Peak.ToString()} daypart is not within run StartTime/EndTime");
            _ = messageBuilder.AppendLine($"Pass: {passName}, SalesAreaPriorities {Dayparts.MidnightToDawn.ToString()} daypart is not within run StartTime/EndTime");

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPrioritiesDayparts(
                passName,
                _dummyPassSalesAreaPriorityAllTrue,
                runStartTime,
                runEndTime,
                _dummyTenantSettings);

            // Assert
            _ = result.Should().Be(messageBuilder.ToString());
        }

        [Test(Description = "Given all Daypart flags are turned off in the Pass when start and end times are constricted then return message should be empty")]
        public void GivenAllDaypartFlagsAreTurnedOffInThePassWhenStartAndEndTimesAreConstrictedThenReturnMessageShouldBeEmpty()
        {
            // Arrange
            _dummyPass.PassSalesAreaPriorities = _dummyPassSalesAreaPriorityAllFalse;
            var runStartTime = new TimeSpan(2, 0, 0);
            var runEndTime = new TimeSpan(3, 0, 0);

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPrioritiesDayparts(
                RunsValidationsDummyData.PassName,
                _dummyPassSalesAreaPriorityAllFalse,
                runStartTime,
                runEndTime,
                _dummyTenantSettings);

            // Assert
            _ = result.Should().BeEmpty();
        }

        #endregion ValidatePassSalesAreaPrioritiesDayparts Tests

        #region ValidatePassSalesAreaPriorities Tests

        [Test(Description = "Given times of run and pass and pass dayparts false then return message should reflect validity of given pass")]
        [TestCaseSource(typeof(RunsValidationsTestData), "ValidatePassSalesAreaPrioritiesDayPartFalseTestCases")]
        public void GivenRunAndPassTimesAndPassDayPartsFalseThenReturnMessageShouldReflectValidityOfPass(
            TimeSpan runStartTime, TimeSpan runEndTime,
            TimeSpan passStartTime, TimeSpan passEndTime,
            string expectedResult)
        {
            // Arrange
            _dummyRun.StartTime = runStartTime;
            _dummyRun.EndTime = runEndTime;
            _dummyPass.PassSalesAreaPriorities.StartTime = passStartTime;
            _dummyPass.PassSalesAreaPriorities.EndTime = passEndTime;

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPriorities(_dummyRun, _dummyPass, _dummyTenantSettings, _dummyNotExcludedSalesAreas);

            // Assert
            _ = result.Should().Be(expectedResult);
        }

        [Test(Description = "Given all valid inputs and Pass dayparts true then return message should be empty")]
        [TestCaseSource(typeof(RunsValidationsTestData), "ValidatePassSalesAreaPrioritiesDayPartTrueTestCases")]
        public void GivenAllValidInputsAndPassDayPartsTrueThenReturnMessageShouldBeEmpty(TimeSpan runStartTime, TimeSpan runEndTime, string expectedResult)
        {
            // Arrange
            _dummyRun.StartTime = runStartTime;
            _dummyRun.EndTime = runEndTime;
            _dummyPass.PassSalesAreaPriorities = _dummyPassSalesAreaPriorityAllTrue;

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPriorities(_dummyRun, _dummyPass, _dummyTenantSettings, _dummyNotExcludedSalesAreas);

            // Assert
            _ = result.Should().Be(expectedResult);
        }

        [Test(Description = "Given Pass has some SalesAreaPriorities records and run sales areas list is empty then return message should be correct error")]
        public void GivenPassHasSomeSalesAreaPrioritiesRecordsAndRunSalesAreasListIsEmptyThenReturnMessageShouldBeCorrectError()
        {
            // Arrange
            _dummyPassSalesAreaPriorityAllFalse.SalesAreaPriorities = _dummyRunSalesAreasPriority;
            var errorSalesAreas = string.Join(", ", _dummyRunSalesAreasPriority.Where(o => o.Priority != SalesAreaPriorityType.Exclude).Select(o => o.SalesArea));
            string passName = RunsValidationsDummyData.PassName;

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPriorities(_dummyRun, _dummyPass, _dummyTenantSettings, new List<string>());

            // Assert
            _ = result.Should().Be($"Pass: {passName}, contains SalesAreaPriorities for SalesArea(s):[{errorSalesAreas}] which are not found in the Run level SalesAreaPriorities{Environment.NewLine}");
        }

        [Test(Description = "Given Pass has some SalesAreaPriorities records and run sales areas list mathes them then return message should be empty")]
        public void GivenPassHasSomeSalesAreaPrioritiesRecordsAndRunSalesAreasListMatchesThemThenReturnMessageShouldBeEmpty()
        {
            // Arrange
            _dummyPassSalesAreaPriorityAllFalse.SalesAreaPriorities = _dummyRunSalesAreasPriority;

            // Act
            var result = RunsValidations.ValidatePassSalesAreaPriorities(_dummyRun, _dummyPass, _dummyTenantSettings, _dummyNotExcludedSalesAreas);

            // Assert
            _ = result.Should().BeEmpty();
        }

        #endregion ValidatePassSalesAreaPriorities Tests
    }
}
