using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.tests.ValidationTests.RunTypes.Data;
using xggameplan.Validations.RunTypes;

namespace xggameplan.tests.ValidationTests.RunTypes
{
    [TestFixture(Category = "Validations :: RunTypesValidations")]
    public class RunTypesValidationsTests
    {
        private static Fixture _fixture;
        private List<CreateRunTypeAnalysisGroupModel> _dummyCreateRunTypeAnalysisGroupModels;
        private List<AnalysisGroup> _dummyAnalysisGroups;

        [SetUp]
        public void Init()
        {
            _fixture = new SafeFixture();

            _dummyCreateRunTypeAnalysisGroupModels = _fixture
                .Build<CreateRunTypeAnalysisGroupModel>()
                .With(o => o.KPI, RunTypeAnalysisGroupKPINames.Spots)
                .CreateMany(3)
                .ToList();

            _dummyAnalysisGroups = _fixture
                .Build<AnalysisGroup>()
                .CreateMany(3)
                .ToList();

            _dummyCreateRunTypeAnalysisGroupModels[0].AnalysisGroupId = _dummyAnalysisGroups[0].Id;
            _dummyCreateRunTypeAnalysisGroupModels[1].AnalysisGroupId = _dummyAnalysisGroups[1].Id;
            _dummyCreateRunTypeAnalysisGroupModels[2].AnalysisGroupId = _dummyAnalysisGroups[2].Id;
        }

        [Test(Description = "When name value is null or empty then return an error message")]
        [TestCase(null)]
        [TestCase("")]
        public void EmptyNameInvalid(string passedInName)
        {
            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateRunTypeName(passedInName);

            // Assert
            _ = isValid.Should().BeFalse(becauseArgs: null);
            _ = errorMessage.Should().Be("Name cannot be empty", becauseArgs: null);
        }

        [Test(Description = "When KPI value is passed in then correct tuple is returned")]
        [TestCaseSource(typeof(RunTypesValidationsTestData), "ValidateKPINameTestCases")]
        public void ValidateKPIName(string passedInKPI, bool expectedtIsValid, string expectedErrorMessage)
        {
            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateKPIName(passedInKPI);

            // Assert
            _ = isValid.Should().Be(expectedtIsValid, becauseArgs: null);
            _ = errorMessage.Should().Be(expectedErrorMessage, becauseArgs: null);
        }

        [Test(Description = "When default KPI value is passed in then correct tuple is returned")]
        [TestCaseSource(typeof(RunTypesValidationsTestData), "ValidateDefaultKPINameTestCases")]
        public void ValidateDefaultKPIName(string passedInKPI, bool expectedtIsValid, string expectedErrorMessage)
        {
            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateDefaultKPIName(passedInKPI);

            // Assert
            _ = isValid.Should().Be(expectedtIsValid, becauseArgs: null);
            _ = errorMessage.Should().Be(expectedErrorMessage, becauseArgs: null);
        }

        [Test(Description = "Given that list of RunTypeAnalysisGroup records has duplicate AnalysisGroupId and KPI value pairs then error message should be returned")]
        public void ValidateDuplicatePairs()
        {
            // Arrange
            _dummyCreateRunTypeAnalysisGroupModels[1].AnalysisGroupId = _dummyCreateRunTypeAnalysisGroupModels[2].AnalysisGroupId;
            _dummyCreateRunTypeAnalysisGroupModels[1].AnalysisGroupName = _dummyCreateRunTypeAnalysisGroupModels[2].AnalysisGroupName;
            string expectedErrorMessage =
                $"There are duplicate Analysis Group and KPI combinations: {_dummyCreateRunTypeAnalysisGroupModels[2].AnalysisGroupName} - {_dummyCreateRunTypeAnalysisGroupModels[2].KPI}";

            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateDuplicateAnalysisGroupAndKPIPairs(_dummyCreateRunTypeAnalysisGroupModels);

            // Assert
            _ = isValid.Should().BeFalse(becauseArgs: null);
            _ = errorMessage.Should().Be(expectedErrorMessage, becauseArgs: null);
        }

        [Test(Description = "When list is null or empty then validation should pass without error")]
        [TestCaseSource(typeof(RunTypesValidationsTestData), "validateEmptyOrNullListOfRunTypeAnalysisGroups")]
        public void ValidateRunTypeAnalysisGroupListWhenListIsNullOrEmpty(IEnumerable<CreateRunTypeAnalysisGroupModel> runTypeAnalysisGroups)
        {
            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateRunTypeAnalysisGroupList(runTypeAnalysisGroups, _dummyAnalysisGroups);

            // Assert
            _ = isValid.Should().BeTrue(becauseArgs: null);
            _ = errorMessage.Should().BeEmpty(becauseArgs: null);
        }

        [Test(Description = "When runTypeAnalysisGroups item AnalysisGroupId does not exist in the AnalysisGroups then error message should be returned")]
        public void ValidateRunTypeAnalysisGroupListVsAnalysisGroupsFail()
        {
            // Arrange
            var firstItem = _dummyCreateRunTypeAnalysisGroupModels.FirstOrDefault();
            firstItem.AnalysisGroupId = 0;
            var expectedErrorMessage = $"Analysis group \"{firstItem.AnalysisGroupName}\" no longer exist";

            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateRunTypeAnalysisGroupList(_dummyCreateRunTypeAnalysisGroupModels, _dummyAnalysisGroups);

            // Assert
            _ = isValid.Should().BeFalse(becauseArgs: null);
            _ = errorMessage.Should().Be(expectedErrorMessage, becauseArgs: null);
        }

        [Test(Description = "When runTypeAnalysisGroups item AnalysisGroupId exist in the AnalysisGroups then validations should pass")]
        public void ValidateRunTypeAnalysisGroupListVsAnalysisGroupsPass()
        {
            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateRunTypeAnalysisGroupList(_dummyCreateRunTypeAnalysisGroupModels, _dummyAnalysisGroups);

            // Assert
            _ = isValid.Should().BeTrue(becauseArgs: null);
            _ = errorMessage.Should().Be("", becauseArgs: null);
        }

        [Test(Description = "When runTypeAnalysisGroups item AnalysisGroupId exist in the AnalysisGroups then validations should pass")]
        [TestCase("", false, "RunType-AnalysisGroup: KPI must have a value")]
        [TestCase(null, false, "RunType-AnalysisGroup: KPI must have a value")]
        [TestCase("NotReallyAKPI", false, "RunType-AnalysisGroup: NotReallyAKPI KPI is not valid")]
        public void ValidateRunTypeAnalysisGroupListInvalidKPI(string passedInKPI, bool expectedtIsValid, string expectedErrorMessage)
        {
            // Arrange
            _dummyCreateRunTypeAnalysisGroupModels[0].KPI = passedInKPI;

            // Act
            (bool isValid, string errorMessage) = RunTypesValidations.ValidateRunTypeAnalysisGroupList(_dummyCreateRunTypeAnalysisGroupModels, _dummyAnalysisGroups);

            // Assert
            _ = isValid.Should().Be(expectedtIsValid, becauseArgs: null);
            _ = errorMessage.Should().Be(expectedErrorMessage, becauseArgs: null);
        }

        [TearDown]
        public void Cleanup()
        {
            _fixture = null;
        }
    }
}
