using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: LibrarySalesAreaPassPriorities")]
    public class LibrarySalesAreaPassPrioritiesControllerDefaultTests : LibrarySalesAreaPassPrioritiesControllerBaseTests
    {
        /// <summary>
        /// Scenario 2: (API) Verify that the Default assignment for a Sales Area Pass Priority can be moved from 1 SAPP to another SAPP via a PUT API request method
        ///     Given: I have a Sales Area Pass Priority that is not currently the Default
        ///         And: I know the Id of this SAPP
        ///         And: I know the Id of the Default SAPP
        ///     When: I send a PUT request that Includes the Id of the intended Default SAPP
        ///     Then: return a Status code 200
        /// </summary>
        [Test]
        public async Task Set_sales_area_pass_priority_as_default_success_200()
        {
            _ = FakeLibrarySalesAreaPassPrioritiesRepository.Setup(m => m.GetAsync(FakeSalesAreaPassPriority.Uid)).ReturnsAsync(FakeSalesAreaPassPriority);
            _ = FakeTenantSettingsRepository.Setup(x => x.Get()).Returns(FakeTenantSettings);

            using (var controller = new LibrarySalesAreaPassPrioritiesController(
                FakeLibrarySalesAreaPassPrioritiesRepository.Object,
                FakeTenantSettingsRepository.Object,
                Mapper, null, null
                ))
            {
                // Act
                IHttpActionResult actionResult = await controller.SetDefault(FakeSalesAreaPassPriority.Uid);
                var contentResult = actionResult as OkResult;

                // Assert
                Assert.NotNull(contentResult);
            }
        }

        /// <summary>
        /// Given: I have valid guid which dosen't exist
        /// When: I send a PUT request that Includes the Id of the intended Default SAPP
        /// Then: I expect not found status code (404)
        /// </summary>
        [Test]
        public async Task Set_sales_area_pass_priority_nonexisting_id_404()
        {
            var nonExistingSalesAreaPassPriorityId = Guid.NewGuid();
            _ = FakeLibrarySalesAreaPassPrioritiesRepository.Setup(m => m.GetAsync(FakeSalesAreaPassPriority.Uid)).ReturnsAsync(FakeSalesAreaPassPriority);
            _ = FakeTenantSettingsRepository.Setup(x => x.Get()).Returns(FakeTenantSettings);

            using (var controller = new LibrarySalesAreaPassPrioritiesController(
                FakeLibrarySalesAreaPassPrioritiesRepository.Object,
                FakeTenantSettingsRepository.Object,
                Mapper, null, null
                ))
            {
                // Act
                IHttpActionResult actionResult = await controller.SetDefault(nonExistingSalesAreaPassPriorityId);
                var notFoundResult = actionResult as NotFoundResult;

                // Assert
                Assert.NotNull(notFoundResult);
            }
        }
    }
}
