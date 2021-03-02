using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    public abstract class LibrarySalesAreaPassPriorityModelValidatorTestBase<TTarget, TModel>
       where TTarget : ModelDataValidatorBase<TModel>
       where TModel : LibrarySalesAreaPassPriorityModelBase
    {
        protected Mock<ILibrarySalesAreaPassPrioritiesRepository> _librarySalesAreaPassPrioritiesRepositoryMock;
        protected Mock<ISalesAreaRepository> _salesAreaRepositoryMock;
        protected TTarget _target;
        protected Fixture _fixture;
        protected TModel _model;

        [OneTimeSetUp]
        public async Task OnInit()
        {
            _fixture = new Fixture();
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised();
        }

        [OneTimeTearDown]
        public async Task OnDestroy()
        {
            CleanUp();
        }

        [SetUp]
        public async Task BeforeEach()
        {
            AssumeDefaultsAreSetup();
        }

        [Test]
        public async Task FailWhenNameIsEmpty()
        {
            _model.Name = string.Empty;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenNameContainsOnlyWhiteSpaces()
        {
            _model.Name = "  ";

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenNameIsNotUniqueAndThatItAlreadyExistsInTheLibrary()
        {
            AssumeRepositoryIsSetupToReturnNameIsNotUnique();

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesContainNonExistingSalesAreas()
        {
            AssumeModelContainsNonExistingSalesAreas();

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task PassWhenSalesAreaPrioritiesContainOnlyExistingSalesAreas()
        {
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test]
        public async Task FailWhenDaysOfWeekIsNull()
        {
            _model.DaysOfWeek = null;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenDaysOfWeekIsEmpty()
        {
            _model.DaysOfWeek = string.Empty;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenDaysOfWeekContainsOnlyWhiteSpaces()
        {
            _model.DaysOfWeek = " ";

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenDaysOfWeekFormatIsInCorrect()
        {
            _model.DaysOfWeek = "111";

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesIsNull()
        {
            _model.SalesAreaPriorities = null;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesIsEmpty()
        {
            _model.SalesAreaPriorities = new List<SalesAreaPriorityModel>();

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesContainEmptySalesArea()
        {
            _model.SalesAreaPriorities[0].SalesArea = string.Empty;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesContainNullSalesArea()
        {
            _model.SalesAreaPriorities[0].SalesArea = null;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesContainSalesAreaWithOnlyWhiteSpaces()
        {
            _model.SalesAreaPriorities[0].SalesArea = "  ";

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesContainDuplicateSalesAreas()
        {
            _model.SalesAreaPriorities[1].SalesArea = _model.SalesAreaPriorities[0].SalesArea;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenSalesAreaPrioritiesContainAnyInvalidPriority()
        {
            _model.SalesAreaPriorities[0].Priority = (SalesAreaPriorityType)55;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task PassWhenSalesAreaPrioritiesContainOnlyValidPriority()
        {
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test]
        public async Task FailWhenItIsNotForAllDayAndContainsNoStartTime()
        {
            _model.StartTime = null;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenItIsNotForAllDayAndContainsInvalidStartTime()
        {
            _model.StartTime = "24:10";

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenItIsNotForAllDayAndContainsNoEndTime()
        {
            _model.EndTime = null;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenItIsNotForAllDayAndContainsInvalidEndTime()
        {
            _model.EndTime = "26:40";

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task PassWhenItIsNotForAllDayAndContainsValidStartTimeAndEndTime()
        {
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test]
        public async Task PassWhenItIsForAllDayAndContainsNoStartTimeAndEndTime()
        {
            _model.EndTime = null;
            _model.StartTime = null;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        protected void AssumeDependenciesAreSupplied()
        {
            _librarySalesAreaPassPrioritiesRepositoryMock = new Mock<ILibrarySalesAreaPassPrioritiesRepository>();
            _salesAreaRepositoryMock = new Mock<ISalesAreaRepository>();
        }

        protected void AssumeDefaultsAreSetup()
        {
            AssumeRepositoryIsSetupToReturnNameIsUnique();
            AssumeValidModelIsSupplied();
            AssumeSalesAreaRepositoryGetListOfNamesIsSetupToReturnNames(_model.SalesAreaPriorities.Select(a => a.SalesArea).ToList());
        }

        protected void AssumeValidModelIsSupplied()
        {
            _model = CreateValidModel();
        }

        protected void AssumeSalesAreaRepositoryGetListOfNamesIsSetupToReturnNames(List<string> namesToReturn)
        {
            _ = _salesAreaRepositoryMock.Setup(a => a.GetListOfNames()).Returns(namesToReturn);
        }

        protected void AssumeRepositoryIsSetupToReturnNameIsUnique()
        {
            //setup repository to return that the name deosn't already exist
            AssumeRepositoryIsNameUniqueIsSetupToReturn(true);
        }

        protected void AssumeRepositoryIsSetupToReturnNameIsNotUnique()
        {
            //setup repository to return that the name already exists
            AssumeRepositoryIsNameUniqueIsSetupToReturn(false);
        }

        protected void AssumeModelContainsNonExistingSalesAreas()
        {
            _model.SalesAreaPriorities.AddRange(CreateSalesAreaPriorities(5));
        }

        protected virtual TModel CreateValidModel()
        {
            return _fixture.Build<TModel>()
                           .With(a => a.DaysOfWeek, "1111111")
                           .With(a => a.StartTime, "06:00:00")
                           .With(a => a.EndTime, "06:30:00")
                           .With(a => a.SalesAreaPriorities, CreateSalesAreaPriorities(6))
                           .Create();
        }

        protected List<SalesAreaPriorityModel> CreateSalesAreaPriorities(int noOfRecords)
        {
            return _fixture.CreateMany<SalesAreaPriorityModel>(noOfRecords > 0 ? noOfRecords : 0).ToList();
        }

        protected void CleanUp()
        {
            _fixture = null;
            _librarySalesAreaPassPrioritiesRepositoryMock = null;
            _salesAreaRepositoryMock = null;
            _target = null;
        }

        protected abstract void AssumeRepositoryIsNameUniqueIsSetupToReturn(bool valueToReturn);

        protected abstract void AssumeTargetIsInitialised();
    }
}
