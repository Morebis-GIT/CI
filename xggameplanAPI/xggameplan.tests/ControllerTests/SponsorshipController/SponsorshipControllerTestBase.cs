using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoFixture;
using AutoMapper;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations;

namespace xggameplan.tests.ControllerTests
{
    public abstract class SponsorshipControllerTestBase
    {
        protected Fixture Fixture { get; private set; }
        protected IMapper Mapper { get; private set; }
        internal Mock<IDataChangeValidator> _dataChangeValidator;
        protected SponsorshipsController Target { get; set; }
        protected Mock<ISponsorshipRepository> SponsorshipRepository { get; set; }
        private Mock<CreateSponsorshipsModelValidator> _validatorForCreate;
        private Mock<UpdateSponsorshipModelValidator> _validatorForUpdate;

        [OneTimeSetUp]
        public void OnInit()
        {
            Fixture = new Fixture();
            Mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);
            _dataChangeValidator = new Mock<IDataChangeValidator>();
        }

        [OneTimeTearDown]
        public void OnDestroy()
        {
            Fixture = null;
            Mapper = null;
        }

        protected void AssumeDependenciesAreSupplied()
        {
            SponsorshipRepository = new Mock<ISponsorshipRepository>();
            var _createValidation = new Mock<IValidator<IEnumerable<CreateSponsorshipModel>>>();
            _validatorForCreate = new Mock<CreateSponsorshipsModelValidator>(_createValidation.Object);
            var _updateValidation = new Mock<IValidator<UpdateSponsorshipModel>>();
            _validatorForUpdate = new Mock<UpdateSponsorshipModelValidator>(_updateValidation.Object);
        }

        protected void AssumeTargetIsInitialised()
        {
            Target = new SponsorshipsController(SponsorshipRepository.Object, _dataChangeValidator.Object, Mapper, _validatorForCreate.Object, _validatorForUpdate.Object);
        }

        protected Sponsorship CreateValidSponsorship(string ExternalReferenceID)
        {
            return Fixture.Build<Sponsorship>()
                           .With(a => a.ExternalReferenceId, ExternalReferenceID)
                           .Create();
        }

        protected void AssumeSponsorshipRepositoryGetReturnsAMatchingSponsoship(Sponsorship sponsorship)
        {
            _ = SponsorshipRepository.Setup(a => a.Get(It.IsAny<string>())).Returns(sponsorship);
        }

        protected void AssumeModelValidationForPostFails()
        {
            SetupTheCreateValidatorIsValidToReturn(false);
        }

        protected void AssumeModelValidationForPostPasses()
        {
            SetupTheCreateValidatorIsValidToReturn(true);
        }

        private void SetupTheCreateValidatorIsValidToReturn(bool valueToReturn)
        {
            _ = _validatorForCreate.Setup(a => a.IsValid(It.IsAny<IEnumerable<CreateSponsorshipModel>>())).Returns(valueToReturn);
        }

        protected CreateSponsorshipModel SetupCreateSponsorshipModel()
        {
            return CreateValidCreateSponsorshipModel(Fixture.Create<string>());
        }

        protected UpdateSponsorshipModel SetupUpdateSponsorshipModel()
        {
            return CreateValidUpdateSponsorshipModel(Fixture.Create<string>());
        }

        protected List<CreateSponsorshipModel> SetupListToPost(int howMany)
        {
            var csm = new List<CreateSponsorshipModel>();
            for (int i = 1; i <= howMany; i++)
            {
                CreateSponsorshipModel createSponsorship = CreateValidCreateSponsorshipModel(Fixture.Create<string>());
                csm.Add(createSponsorship);
            }
            return csm;
        }

        protected static string AssumeEmptyIdIsSupplied()
        {
            return String.Empty;
        }

        protected string AssumeValidIdIsSupplied()
        {
            return Fixture.Create<string>();
        }

        protected IHttpActionResult AssumePost(IEnumerable<CreateSponsorshipModel> csm)
        {
            return Target.Post(csm);
        }

        protected IHttpActionResult AssumePut(UpdateSponsorshipModel updateSponsorshipModel)
        {
            return Target.Put(updateSponsorshipModel);
        }

        protected IHttpActionResult AssumeDeleteByIdIsRequested(string sponsorshipId)
        {
            return Target.Delete(sponsorshipId);
        }

        protected async Task<IHttpActionResult> AssumeDeleteAllAsync()
        {
            return await Target.DeleteAll().ConfigureAwait(false);
        }

        private CreateSponsorshipModel CreateValidCreateSponsorshipModel(string ExternalReferenceID)
        {
            return Fixture.Build<CreateSponsorshipModel>()
                           .With(a => a.ExternalReferenceId, ExternalReferenceID)
                           .Create();
        }

        private UpdateSponsorshipModel CreateValidUpdateSponsorshipModel(string ExternalReferenceID)
        {
            return Fixture.Build<UpdateSponsorshipModel>()
                           .With(a => a.ExternalReferenceId, ExternalReferenceID)
                           .Create();
        }

        protected void AssumeModelValidationForUpdateFails()
        {
            SetupTheUpdateValidatorIsValidToReturn(false);
        }

        protected void AssumeModelValidationForUpdatePasses()
        {
            SetupTheUpdateValidatorIsValidToReturn(true);
        }

        private void SetupTheUpdateValidatorIsValidToReturn(bool valueToReturn)
        {
            _ = _validatorForUpdate.Setup(a => a.IsValid(It.IsAny<UpdateSponsorshipModel>())).Returns(valueToReturn);
        }       
    }
}
