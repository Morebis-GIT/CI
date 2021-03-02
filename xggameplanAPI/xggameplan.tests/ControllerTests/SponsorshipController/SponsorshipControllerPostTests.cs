using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using NUnit.Framework;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Sponsorships")]
    public class SponsorshipControllerPostTests : SponsorshipControllerTestBase
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
        public void ReturnOKResultWhenPostWithSingleValidModel()
        {
            AssumeModelValidationForPostPasses();
            IHttpActionResult result = AssumePost(SetupListToPost(1));
            _ = result.GetType().Should().Be(typeof(OkResult), "OK response expected.");
        }

        [Test]
        public void ReturnOKResultWhenPostWithMultipleValidModels()
        {
            AssumeModelValidationForPostPasses();
            IHttpActionResult result = AssumePost(SetupListToPost(3));
            _ = result.GetType().Should().Be(typeof(OkResult), "OK response expected.");
        }

        [Test]
        public void ReturnBadRequestResultWhenPostWithSingleInValidModel()
        {
            AssumeModelValidationForPostFails();
            IHttpActionResult result = AssumePost(SetupListToPost(1));
            _ = result.GetType().Should().Be(typeof(ResponseMessageResult), "ResponseMessageResult expected with bad request reason");
        }

        [Test]
        public void ReturnBadRequestResultWhenPostWithMultipleInValidModels()
        {
            AssumeModelValidationForPostFails();
            IHttpActionResult result = AssumePost(SetupListToPost(3));
            _ = result.GetType().Should().Be(typeof(ResponseMessageResult), "ResponseMessageResult expected with bad request reason");
        }
    }
}
