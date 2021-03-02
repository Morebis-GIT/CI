using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.Controllers.CustomHttpResults;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: LibrarySalesAreaPassPriorities")]
    public partial class LibrarySalesAreaPassPrioritiesControllerDeleteTests : LibrarySalesAreaPassPrioritiesControllerBaseTests
    {
        // (Scenario: Invalid Id supplied)
        // Given: I have a valid GUID but invalid Id for a Sales Area Pass Priority(i.e.does not exist in target Gameplan instance)
        // When: I send this Id in a DELETE request of the form DELETE /Library/SalesAreaPassPriorities/{Id}
        // Then: do not return a Status code of 200 to indicate that this record has been deleted
        //  And: return some indication that request cannot be serviced
        [Test]
        public async Task Delete_sales_area_pass_priority_with_valid_guid_which_not_exists_status_code_200_and_request_cannot_be_serviced()
        {
            var undefinedId = Guid.NewGuid();
            _ = FakeLibrarySalesAreaPassPrioritiesRepository.Setup(m => m.GetAsync(FakeSalesAreaPassPriority.Uid)).ReturnsAsync(FakeSalesAreaPassPriority);
            _ = FakeTenantSettingsRepository.Setup(x => x.GetDefaultSalesAreaPassPriorityId()).Returns(FakeTenantSettings.DefaultSalesAreaPassPriorityId);

            using (var controller = new LibrarySalesAreaPassPrioritiesController(
                FakeLibrarySalesAreaPassPrioritiesRepository.Object,
                FakeTenantSettingsRepository.Object,
                Mapper, null, null
                ))
            {
                // Act
                IHttpActionResult actionResult = await controller.Delete(undefinedId);

                // Assert
                Assert.That(actionResult is NotFoundResult);
            }
        }

        // (Scenario: Attempt to delete Default SAPP)
        // Given: I have an Id for a Sales Area Pass Priority that matches that of the Default SAPP
        // When: I send this Id in a DELETE request of the form DELETE /Library/SalesAreaPassPriorities/{Id}
        // Then: do not return a Status code of 200 to indicate that this record has been deleted
        // And: return an indication that the default SAPP cannot be deleted
        [Test]
        public async Task Delete_sales_area_pass_priority_which_is_default_expect_422_with_message()
        {
            FakeSalesAreaPassPriority.Uid = FakeTenantSettings.DefaultSalesAreaPassPriorityId;
            _ = FakeLibrarySalesAreaPassPrioritiesRepository.Setup(m => m.GetAsync(FakeSalesAreaPassPriority.Uid)).ReturnsAsync(FakeSalesAreaPassPriority);
            _ = FakeTenantSettingsRepository.Setup(x => x.GetDefaultSalesAreaPassPriorityId()).Returns(FakeTenantSettings.DefaultSalesAreaPassPriorityId);

            using (var controller = new LibrarySalesAreaPassPrioritiesController(
                FakeLibrarySalesAreaPassPrioritiesRepository.Object,
                FakeTenantSettingsRepository.Object,
                Mapper, null, null
                ))
            {
                // Act
                IHttpActionResult actionResult = await controller.Delete(FakeTenantSettings.DefaultSalesAreaPassPriorityId);
                var response = actionResult as UnprocessableEntityResult;

                // Assert
                Assert.NotNull(response);
            }
        }

        // (Scenario: Valid Id supplied, and SAPP not carrying a dependency on a Run)
        // Given: I have a valid Id for a Sales Area Pass Priority(that is not the Default SAPP)
        // When: I send this Id in a DELETE request of the form DELETE /Library/SalesAreaPassPriorities/{Id}
        // Then: return a Status code of 200 to indicate that this record has been deleted
        [Test]
        public async Task Delete_sales_area_pass_priority_without_dependency()
        {
            _ = FakeLibrarySalesAreaPassPrioritiesRepository.Setup(m => m.GetAsync(FakeSalesAreaPassPriority.Uid)).ReturnsAsync(FakeSalesAreaPassPriority);
            _ = FakeTenantSettingsRepository.Setup(x => x.GetDefaultSalesAreaPassPriorityId()).Returns(Guid.NewGuid());

            using (var controller = new LibrarySalesAreaPassPrioritiesController(
                FakeLibrarySalesAreaPassPrioritiesRepository.Object,
                FakeTenantSettingsRepository.Object,
                Mapper, null, null
                ))
            {
                // Act
                IHttpActionResult actionResult = await controller.Delete(FakeSalesAreaPassPriority.Uid);
                var response = actionResult as StatusCodeResult;

                // Assert
                Assert.That(response.StatusCode == HttpStatusCode.NoContent);
            }
        }
    }
}
