using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.Interfaces;
using xggameplan.core.Validators;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Passes")]
    public class PassesControllerTests : IDisposable
    {
        private Mock<IScenarioRepository> _fakeScenarioRepository;
        private Mock<IPassRepository> _fakePassRepository;
        private Mock<IIdentityGeneratorResolver> _fakeIdentityGeneratorResolver;
        private Mock<IDataManipulator> _fakeDataManipulator;
        private Mock<ISalesAreaRepository> _fakeSalesAreaRepository;
        private Mock<IPassInspectorService> _fakePassInspectorService;
        private Fixture _fixture;
        private IMapper _mapper;

        private PassesController _controller;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();
            _fakeScenarioRepository = new Mock<IScenarioRepository>();
            _fakePassRepository = new Mock<IPassRepository>();
            _fakeIdentityGeneratorResolver = new Mock<IIdentityGeneratorResolver>();
            _fakeDataManipulator = new Mock<IDataManipulator>();
            _fakeSalesAreaRepository = new Mock<ISalesAreaRepository>();
            _fakePassInspectorService = new Mock<IPassInspectorService>();

            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            IEnumerable<Pass> passWithDateModifiedNull = _fixture
                           .Build<Pass>()
                           .Without(p => p.DateCreated)
                           .CreateMany(1);

            _ = _fakePassRepository
                .Setup(m => m.GetAll())
                .Returns(passWithDateModifiedNull);

            _ = _fakePassRepository
                .Setup(m => m.Get(It.IsAny<int>()))
                .Returns<Pass>(null);

            _ = _fakeSalesAreaRepository.Setup(m => m.GetAll()).Returns(
                    new List<SalesArea>() { new SalesArea() { Name = "sa1" }, new SalesArea() { Name = "sa2" } }
                    );

            _controller = new PassesController(
                _fakeScenarioRepository.Object,
                _fakePassRepository.Object,
                _mapper,
                _fakeDataManipulator.Object,
                _fakeSalesAreaRepository.Object,
                _fakeIdentityGeneratorResolver.Object,
                _fakePassInspectorService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._controller = null;
            this._mapper = null;
        }

        [Test]
        [Description("Given a legacy pass with a null DateCreated when getting all passes then the DateCreated is null")]
        public void Given_a_legacy_pass_with_a_null_DateCreated_when_getting_all_passes_then_the_DateCreated_is_null()
        {
            IEnumerable<PassModel> result = this._controller.GetAll();
            Assert.That(result.First().DateCreated, Is.Null);
        }

        //[Test]
        //[Description("Given CreatePassModel when Posting then the DataManipulator is called to set some of properties")]
        //public void DataManipulatorIsCalledWhenCreatingANewPass()
        //{
        //    CreatePassModel model = this.GetPassModel();
        //    model.Name = null;
        //    fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
        //    Assert.That(() => this.controller.Post(model), Throws.Exception.TypeOf<Exception>().With.Property("Message").EqualTo("Pass name must be set"));
        //    this.fakeDataManipulator.Verify(m => m.Manipulate(model), Times.Once);
        //}

        [Test]
        [Description("Given CreatePassModel when Posting With Null Value for PassSalesAreaPriority DaysOfWeek then the exception is raised")]
        public void PassSalesAreaPriorityNullDaysOfWeekValidation()
        {
            CreatePassModel model = this.GetCreatePassModel();
            model.PassSalesAreaPriorities.DaysOfWeek = null;
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.That(() => this._controller.Post(model), Throws.Exception.TypeOf<ArgumentNullException>().With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: PassSalesAreaPriority DaysOfWeek"));
        }

        [Test]
        [Description("Given CreatePassModel when Posting With invalid Value for PassSalesAreaPriority DaysOfWeek then the exception is raised")]
        public void PassSalesAreaPriorityInvalidDaysOfWeekValidation()
        {
            CreatePassModel model = this.GetCreatePassModel();
            model.PassSalesAreaPriorities.DaysOfWeek = "InvalidValue";
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.That(() => this._controller.Post(model), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>().With.Property("Message").EqualTo("Invalid PassSalesAreaPriority DaysOfWeek"));
        }

        [Test]
        [Description("Given CreatePassModel when Posting With null Value for PassSalesAreaPriority SalesAreaPriorities then the exception is raised")]
        public void PassSalesAreaPriorityNullValueForSalesAreaPrioritiesValidation()
        {
            CreatePassModel model = this.GetCreatePassModel();
            model.PassSalesAreaPriorities.SalesAreaPriorities = null;
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.That(() => this._controller.Post(model), Throws.Exception.TypeOf<ArgumentNullException>().With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: PassSalesAreaPriority SalesAreaPriorities"));
        }

        [Test]
        [Description("Given CreatePassModel when Posting with PassSalesAreaPriority SalesAreaPriorities then the SalesAreaPriorities is sub set of all Sales areas")]
        public void PassSalesAreaPrioritySalesAreaPrioritiesIsASubSetOfAllSalesAreasValidation()
        {
            CreatePassModel model = this.GetCreatePassModel();
            model.PassSalesAreaPriorities.SalesAreaPriorities[0] = new SalesAreaPriorityModel() { SalesArea = "foo" };
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.That(() => this._controller.Post(model), Throws.Exception.TypeOf<Exception>().With.Property("Message").EqualTo("foo is not a valid SalesArea"));
        }

        [Test]
        [Description("Given CreatePassModel when Posting with PassSalesAreaPriority SalesAreaPriorities then the SalesAreaPriorities doesnot have duplicated values")]
        public void PassSalesAreaPrioritySalesAreaPrioritiesDoesNotHaveDuplicates()
        {
            CreatePassModel model = this.GetCreatePassModel();
            model.PassSalesAreaPriorities.SalesAreaPriorities.Add(new SalesAreaPriorityModel() { SalesArea = "foo" });
            model.PassSalesAreaPriorities.SalesAreaPriorities.Add(new SalesAreaPriorityModel() { SalesArea = "foo" });
            model.PassSalesAreaPriorities.SalesAreaPriorities.Add(new SalesAreaPriorityModel() { SalesArea = "bar" });
            model.PassSalesAreaPriorities.SalesAreaPriorities.Add(new SalesAreaPriorityModel() { SalesArea = "bar" });
            model.PassSalesAreaPriorities.SalesAreaPriorities.Add(new SalesAreaPriorityModel() { SalesArea = "bar" });
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.That(() => this._controller.Post(model), Throws.Exception.TypeOf<Exception>().With.Property("Message").EqualTo("Duplicate SalesAreas: foo,bar"));
        }

        [Test]
        [Description(@"Given no times are provided in break exclusions
                       When validating the payload
                       Then should save pass to the database")]
        public void BreakExclusionsPost_Fails_NoTimesProvided()
        {
            CreatePassModel model = GetCreatePassModel();
            model.BreakExclusions.Add(new BreakExclusionModel()
            {
                EndDate = DateTime.UtcNow.AddDays(1),
                EndTime = null,
                StartDate = DateTime.UtcNow,
                StartTime = null
            });
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PassModel>>(_controller.Post(model));
        }

        [Test]
        [Description(@"Given start date time is after end date time are provided in break exclusions
                       When validating the payload
                       Then should return bad request explaining why")]
        public void BreakExclusionsPost_Fails_ForOverlappingTimes()
        {
            CreatePassModel model = GetCreatePassModel();
            model.BreakExclusions.Add(new BreakExclusionModel()
            {
                SalesArea = "sa1",
                EndDate = DateTime.UtcNow,
                EndTime = new TimeSpan(6, 0, 0),
                StartDate = DateTime.UtcNow,
                StartTime = new TimeSpan(8, 0, 0)
            });
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.That(() => _controller.Post(model), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Description(@"Given that break exclusions is not provided
                       When validating the payload
                       Then should save pass to the database")]
        public void BreakExclusionsPost_Passes_WhenNull()
        {
            CreatePassModel model = GetCreatePassModel();
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PassModel>>(_controller.Post(model));
        }

        [Test]
        [Description(@"Given that break exclusions are provided
                       When validating the payload
                       Then should save pass to the database")]
        public void BreakExclusionsPost_Passes_WithCorrectDateTime()
        {
            CreatePassModel model = GetCreatePassModel();
            model.BreakExclusions.Add(new BreakExclusionModel()
            {
                EndDate = DateTime.UtcNow,
                EndTime = new TimeSpan(8, 0, 0),
                StartDate = DateTime.UtcNow,
                StartTime = new TimeSpan(6, 0, 0)
            });
            _ = _fakeDataManipulator.Setup(m => m.Manipulate(It.IsAny<CreatePassModel>())).Returns(model);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PassModel>>(_controller.Post(model));
        }

        [Test]
        [Description(@"Given there is data in the database
                       When doing a minimal data search
                       Then should return the data in minimal data")]
        public void SearchLibraryItems_Passes_OkWithItems()
        {
            _ = _fakePassRepository
              .Setup(p => p.MinimalDataSearch(It.IsAny<SearchQueryDto>(), true))
              .Returns(new SearchResultModel<PassDigestListItem>()
              {
                  TotalCount = 1
              });

            Assert.IsInstanceOf
                <OkNegotiatedContentResult<SearchResultModel<PassDigestListItem>>>
                (_controller.SearchLibraryItems(null));
        }

        [Test]
        [Description(@"Given there are no data in the database
                       When doing a minimal data search
                       Then should return not found")]
        public void SearchLibraryItems_Passes_NotFound()
        {
            _ = _fakePassRepository
                .Setup(p => p.MinimalDataSearch(It.IsAny<SearchQueryDto>(), true))
                .Returns(new SearchResultModel<PassDigestListItem>());

            Assert.IsInstanceOf<NotFoundResult>(_controller.SearchLibraryItems(null));
        }

        private CreatePassModel GetCreatePassModel()
        {
            CreatePassModel model = new CreatePassModel();
            model.Id = 1;
            model.Name = "Name";
            model.General = new List<GeneralModel>() { new GeneralModel() { RuleId = (int)RuleID.MinimumEfficiency, Value = "1" } };
            model.Weightings = new List<WeightingModel>() { new WeightingModel() };
            model.Tolerances = new List<ToleranceModel>() { new ToleranceModel() };
            model.Rules = new List<PassRuleModel>() { new PassRuleModel() };
            model.PassSalesAreaPriorities = new PassSalesAreaPriorityModel();
            model.PassSalesAreaPriorities.StartTime = new TimeSpan(1, 1, 1);
            model.PassSalesAreaPriorities.EndTime = new TimeSpan(2, 2, 2);
            // NOTE: the below StartDate and EndDate are added since the tests were failing
            // The tests were failing since validations were added for StartDate and EndDate
            // but the test was not updated by the team member who did the change
            model.PassSalesAreaPriorities.StartDate = DateTime.Now;
            model.PassSalesAreaPriorities.EndDate = DateTime.Now.AddDays(6);
            model.PassSalesAreaPriorities.DaysOfWeek = "1111111";
            model.PassSalesAreaPriorities.SalesAreaPriorities = new List<SalesAreaPriorityModel>() { new SalesAreaPriorityModel() { SalesArea = "sa1" } };
            return model;
        }

        private PassModel GetPassModel()
        {
            var model = new PassModel();
            model.Id = 1;
            model.Name = "Name";
            model.General = new List<GeneralModel>() { new GeneralModel() { RuleId = (int)RuleID.MinimumEfficiency, Value = "1" } };
            model.Weightings = new List<WeightingModel>() { new WeightingModel() };
            model.Tolerances = new List<ToleranceModel>() { new ToleranceModel() };
            model.Rules = new List<PassRuleModel>() { new PassRuleModel() };
            model.PassSalesAreaPriorities = new PassSalesAreaPriorityModel();
            model.PassSalesAreaPriorities.StartTime = new TimeSpan(1, 1, 1);
            model.PassSalesAreaPriorities.EndTime = new TimeSpan(2, 2, 2);
            // NOTE: the below StartDate and EndDate are added since the tests were failing
            // The tests were failing since validations were added for StartDate and EndDate
            // but the test was not updated by the team member who did the change
            model.PassSalesAreaPriorities.StartDate = DateTime.Now;
            model.PassSalesAreaPriorities.EndDate = DateTime.Now.AddDays(6);
            model.PassSalesAreaPriorities.DaysOfWeek = "1111111";
            model.PassSalesAreaPriorities.SalesAreaPriorities = new List<SalesAreaPriorityModel>() { new SalesAreaPriorityModel() { SalesArea = "sa1" } };
            return model;
        }

        public void Dispose()
        {
            this._controller = null;
        }
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
