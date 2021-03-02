using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Restrictions")]
    public class RestrictionsControllerTests : IDisposable
    {
        private Mock<IRestrictionRepository> _fakeRestrictionRepository;
        private Mock<ISalesAreaRepository> _fakeSalesAreaRepository;
        private Mock<IProgrammeCategoryHierarchyRepository> _fakeProgrammeCategoryRepository;
        private Mock<IClearanceRepository> _fakeClearanceRepository;
        private Fixture _fixture;
        private IMapper _mapper;

        private const string ValidExternalIdentifier = "YH000000001";

        private RestrictionController _controller;
        private CreateRestriction _validCreateRestriction;
        private UpdateRestrictionModel _validUpdateRestriction;
        private List<CreateRestriction> _validCreateRestrictions;

        [SetUp]
        public void Init()
        {
            _fakeRestrictionRepository = new Mock<IRestrictionRepository>();
            _fakeSalesAreaRepository = new Mock<ISalesAreaRepository>();
            _fakeClearanceRepository = new Mock<IClearanceRepository>();
            _fakeProgrammeCategoryRepository = new Mock<IProgrammeCategoryHierarchyRepository>();

            var salesAreaNames = new List<string> { "sa1" };
            var salesArea = new SalesArea { Name = "sa1" };
            var programmeCategory = new ProgrammeCategoryHierarchy { Id = 2, Name = "CHILDREN" };
            _ = _fakeSalesAreaRepository.Setup(m => m.FindByNames(salesAreaNames)).Returns(
                new List<SalesArea> { salesArea }
            );
            _ = _fakeProgrammeCategoryRepository.Setup(p => p.GetAll()).Returns(
                new List<ProgrammeCategoryHierarchy> { programmeCategory });

            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            _controller = new RestrictionController(
                _fakeRestrictionRepository.Object,
                _fakeSalesAreaRepository.Object,
                _fakeClearanceRepository.Object,
                _fakeProgrammeCategoryRepository.Object,
                _mapper);

            _validCreateRestriction = CreateValidModel();
            _validUpdateRestriction = UpdateValidModel();
            _validCreateRestrictions = new List<CreateRestriction>();
        }

        [Test]
        [Description("Given more than one valid CreateRestriction model when Posting then result must be successful")]
        public void InsertMultipleRestrictionByValidRestrictionWithNewUidReturnsOk()
        {
            _validCreateRestrictions = BuildMultipleRestrictions(3);

            Assert.IsInstanceOf<OkNegotiatedContentResult<List<RestrictionModel>>>(_controller.Post(_validCreateRestrictions));
        }

        [Test]
        [Description("Given single valid CreateRestriction model when Posting then result must be successful")]
        public void InsertRestrictionByValidRestrictionWithNewUidReturnsOk()
        {
            _validCreateRestrictions.Add(_validCreateRestriction);

            Assert.IsInstanceOf<OkNegotiatedContentResult<List<RestrictionModel>>>(_controller.Post(_validCreateRestrictions));
        }

        [Test]
        [Description("Given single valid CreateRestriction model when Posting then result must be successful")]
        public void InsertRestrictionByValidRestrictionWithSameStartDateAndEndDateReturnsOk()
        {
            _validCreateRestriction.EndDate = _validCreateRestriction.StartDate;
            _validCreateRestrictions.Add(_validCreateRestriction);

            Assert.IsInstanceOf<OkNegotiatedContentResult<List<RestrictionModel>>>(_controller.Post(_validCreateRestrictions));
        }

        [Test]
        [Description("Given valid UpdateRestriction model when Putting then property must be updated")]
        public void UpdateRestrictionByValidRestrictionByExternalIdentifierUpdatesProperty()
        {
            var restriction = _mapper.Map<Restriction>(_validCreateRestriction);
            restriction.ExternalIdentifier = ValidExternalIdentifier;

            _ = _fakeRestrictionRepository.Setup(r => r.Get(restriction.ExternalIdentifier)).Returns(restriction);

            var result = _controller.Put(restriction.ExternalIdentifier, _validUpdateRestriction) as OkNegotiatedContentResult<RestrictionModel>;

            Assert.AreEqual(_validUpdateRestriction.SchoolHolidayIndicator, result.Content.SchoolHolidayIndicator);
        }

        [Test]
        [Description("Given valid UpdateRestriction model when Putting new restriction then result must be successful")]
        public void CreateRestrictionByValidRestrictionByExternalIdentifierUpdatesProperty()
        {
            var restriction = _mapper.Map<Restriction>(_validCreateRestriction);
            restriction.ExternalIdentifier = ValidExternalIdentifier;

            _ = _fakeRestrictionRepository.Setup(r => r.Get(restriction.ExternalIdentifier)).Returns((Restriction)null);

            var result = _controller.Put(restriction.ExternalIdentifier, _validUpdateRestriction) as OkNegotiatedContentResult<RestrictionModel>;

            Assert.AreEqual(_validUpdateRestriction.SchoolHolidayIndicator, result.Content.SchoolHolidayIndicator);
        }

        [Test]
        [Description("Given not valid CreateProduct when Posting then correct validation message must be returned")]
        public void InsertRestrictionWithInvalidRestrictionDaysCorrectMessageReturns()
        {
            _validCreateRestriction.RestrictionDays = "1112121";
            _validCreateRestrictions.Add(_validCreateRestriction);

            Assert.That(() => _controller.Post(_validCreateRestrictions),
                Throws.Exception.TypeOf<RegexMatchTimeoutException>().With.Property("Message").EqualTo("Invalid Restriction Days"));
        }

        [Test]
        [Description("Given not valid CreateProduct when Posting then correct validation message must be returned")]
        public void InsertRestrictionWithEndDateLessThenStartDateCorrectMessageReturns()
        {
            _validCreateRestriction.StartDate = DateTime.Now;
            _validCreateRestriction.EndDate = _validCreateRestriction.StartDate.AddDays(-1);
            _validCreateRestrictions.Add(_validCreateRestriction);

            Assert.That(() => _controller.Post(_validCreateRestrictions),
                Throws.Exception.TypeOf<InvalidDataException>().With.Property("Message").EqualTo("Restriction start date should be less than or equal to end date"));
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
        }

        protected virtual CreateRestriction CreateValidModel()
        {
            return _fixture.Build<CreateRestriction>()
                .With(a => a.ClashCode, "P576")
                .With(a => a.ProgrammeClassificationIndicator, IncludeOrExclude.I)
                .With(a => a.SchoolHolidayIndicator, IncludeOrExcludeOrEither.E)
                .With(a => a.PublicHolidayIndicator, IncludeOrExcludeOrEither.I)
                .With(a => a.PublicHolidayIndicator, IncludeOrExcludeOrEither.I)
                .With(a => a.LiveProgrammeIndicator, IncludeOrExclude.E)
                .With(a => a.RestrictionDays, "1111111")
                .With(a => a.RestrictionType, RestrictionType.ProgrammeCategory)
                .With(a => a.RestrictionBasis, RestrictionBasis.Clash)
                .With(a => a.ProgrammeCategory, "CHILDREN")
                .With(a => a.StartDate, DateTime.Now)
                .With(a => a.EndDate, DateTime.Now.AddDays(10))
                .With(a => a.SalesAreas, new List<string> { "sa1" })
                .With(a => a.ProductCode, 0)
                .With(a => a.ClockNumber, string.Empty)
                .With(a => a.ClearanceCode, "")
                .With(a => a.ExternalProgRef, "")
                .With(a => a.IndexType, 0)
                .With(a => a.IndexThreshold, 0)
                .With(a => a.ProgrammeClassification, "")
                .Create();
        }

        protected virtual UpdateRestrictionModel UpdateValidModel()
        {
            return _fixture.Build<UpdateRestrictionModel>()
                .With(a => a.ClashCode, "P576")
                .With(a => a.ProgrammeClassificationIndicator, IncludeOrExclude.I)
                .With(a => a.SchoolHolidayIndicator, IncludeOrExcludeOrEither.E)
                .With(a => a.PublicHolidayIndicator, IncludeOrExcludeOrEither.I)
                .With(a => a.PublicHolidayIndicator, IncludeOrExcludeOrEither.I)
                .With(a => a.LiveProgrammeIndicator, IncludeOrExclude.E)
                .With(a => a.RestrictionDays, "1111111")
                .With(a => a.RestrictionType, RestrictionType.ProgrammeCategory)
                .With(a => a.RestrictionBasis, RestrictionBasis.Clash)
                .With(a => a.ProgrammeCategory, "CHILDREN")
                .With(a => a.StartDate, DateTime.Now)
                .With(a => a.EndDate, DateTime.Now.AddDays(10))
                .With(a => a.SalesAreas, new List<string> { "sa1" })
                .With(a => a.ProductCode, 0)
                .With(a => a.ClockNumber, string.Empty)
                .With(a => a.ClearanceCode, "")
                .With(a => a.ExternalProgRef, "")
                .With(a => a.IndexType, 0)
                .With(a => a.IndexThreshold, 0)
                .With(a => a.ProgrammeClassification, "")
                .Create();
        }

        private List<CreateRestriction> BuildMultipleRestrictions(int numberOfRestrictions = 1)
        {
           return _fixture.Build<CreateRestriction>()
                .With(a => a.RestrictionDays, "1111111")
                .With(a => a.RestrictionType, RestrictionType.ProgrammeCategory)
                .With(a => a.RestrictionBasis, RestrictionBasis.Clash)
                .With(a => a.ProgrammeCategory, "CHILDREN")
                .With(a => a.StartDate, DateTime.Now)
                .With(a => a.EndDate, DateTime.Now.AddDays(10))
                .With(a => a.SalesAreas, new List<string> { "sa1" })
                .With(a => a.ProductCode, 0)
                .With(a => a.ClockNumber, string.Empty)
                .With(a => a.ClearanceCode, "")
                .With(a => a.ExternalProgRef, "")
                .With(a => a.IndexType, 0)
                .With(a => a.IndexThreshold, 0)
                .With(a => a.ProgrammeClassification, "")
                .CreateMany(numberOfRestrictions)
                .ToList();
        }

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}
