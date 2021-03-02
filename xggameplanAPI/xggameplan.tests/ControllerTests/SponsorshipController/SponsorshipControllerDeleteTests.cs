using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Sponsorships")]
    public class SponsorshipControllerDeleteTests : SponsorshipControllerTestBase
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
        public void ReturnNotFoundResultWhenNoRecordIsFoundForTheSuppliedIdForDelete()
        {
            string sponsorshipId = AssumeValidIdIsSupplied();
            AssumeSponsorshipRepositoryGetReturnsAMatchingSponsoship(null);

            IHttpActionResult result = AssumeDeleteByIdIsRequested(sponsorshipId);

            _ = result.Should().BeAssignableTo<NotFoundResult>("SponsorshipsController Delete By Id Failed to return NotFoundResult");
        }

        [Test]
        public void ReturnBadRequestResultWhenAnEmptyIdIsSuppliedForDelete()
        {
            string sponsorshipId = AssumeEmptyIdIsSupplied();

            IHttpActionResult result = AssumeDeleteByIdIsRequested(sponsorshipId);

            _ = result.Should().BeAssignableTo<BadRequestErrorMessageResult>("SponsorshipsController Delete By By Id Failed to return BadRequestErrorMessageResult");
        }

        [Test]
        public void ReturnNoContentResultWhenASponsorshipIsFoundForTheSuppliedIdAndSuccessfullyDeleted()
        {
            string sponsorshipId = AssumeValidIdIsSupplied();
            var sponsorship = CreateValidSponsorship(sponsorshipId);

            AssumeSponsorshipRepositoryGetReturnsAMatchingSponsoship(sponsorship);

            var result = AssumeDeleteByIdIsRequested(sponsorshipId) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "SponsorshipController Delete By Id Failed to return NoContent result");
                SponsorshipRepository.Verify(a => a.Get(It.IsAny<string>()), Times.Once(),
                                   "Failed to use SponsorshipRepository to Get the matching sponsorship for deletion");
                SponsorshipRepository.Verify(a => a.Delete(It.IsAny<string>()), Times.Once(),
                                   "Failed to use SponsorshipRepository to delete the sponsorship");
            }
        }

        [Test]
        public async Task ReturnOkResultWhenDeleteAllAsync()
        {
            IHttpActionResult result = await AssumeDeleteAllAsync().ConfigureAwait(false);

            _ = result.Should().BeAssignableTo<OkResult>("DeleteAllAsync did not return OkResult");
        }
    }
}
