using System.Net;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using SponsorshipDomain = ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Sponsorships")]
    public class SponsorshipControllerUpdateTests : SponsorshipControllerTestBase
    {
        [SetUp]
        public void SetUp()
        {
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised();
        }

        [TearDown]
        public void TearDown()
        {
            SponsorshipRepository = null;
            Target?.Dispose();
            Target = null;
        }

        [Test]
        public void ReturnNotFoundResultWhenUpdateWithNonExistentSingleValidModel()
        {
            AssumeModelValidationForUpdatePasses();
            _ = SponsorshipRepository.Setup(s => s.Get(It.IsAny<string>())).Returns((SponsorshipDomain.Sponsorship)null);

            var result = AssumePut(SetupUpdateSponsorshipModel());
            _ = result.Should().BeAssignableTo<NotFoundResult>("NotFound response expected.");
        }

        [Test]
        public void ReturnBadRequestResultWhenUpdateContainsInValidModel()
        {
            AssumeModelValidationForUpdateFails();
            var result = AssumePut(SetupUpdateSponsorshipModel()) as ResponseMessageResult;
            _ = result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "ResponseMessageResult expected with bad request reason");
        }

        [Test]
        public void ReturnOKResultWhenUpdateWithExistingValidModel()
        {
            UpdateSponsorshipModel updateSponsorship = SetupUpdateSponsorshipModel();
            SponsorshipDomain.Sponsorship sponsorship = Mapper.Map<SponsorshipDomain.Sponsorship>(updateSponsorship);
            _ = SponsorshipRepository.Setup(s => s.Get(It.IsAny<string>())).Returns(sponsorship);
            AssumeModelValidationForUpdatePasses();

            var result = AssumePut(updateSponsorship);
            _ = result.Should().BeAssignableTo<OkResult>("OK response expected.");
        }
    }
}
