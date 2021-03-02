using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using Moq;
using NUnit.Framework;

namespace xggameplan.tests.ControllerTests
{
    public partial class BreaksControllerTests : IDisposable
    {
        [Test]
        [Description("Delete()::Given nonexistent date range and sales area when deleting Breaks then Not Found must be returned")]
        public void DeleteBreaksByNonExistentDateRangeAndSalesAreasReturnsNotFound()
        {
            var result = _controller.Delete(
                DateTime.Now,
                DateTime.Now.AddDays(1),
                new List<string> { "test" }
                );

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [Description("Delete()::Given date range and sales area when deleting Breaks then result must be successful")]
        public void DeleteBreaksByExistingDateRangeAndSalesAreasReturnsOk()
        {
            var breaksList = new List<Break> { new Break() };

            _ = _fakeBreakRepository
                .Setup(r => r.Search(It.IsAny<DateTimeRange>(), It.IsAny<List<string>>()))
                .Returns(breaksList);

            _ = _fakeScheduleRepository
                .Setup(r => r.FindByBreakIds(It.IsAny<IEnumerable<Guid>>()))
                .Returns(new List<Schedule> { new Schedule { Breaks = breaksList } });

            var result = _controller.Delete(
                DateTime.Now,
                DateTime.Now.AddDays(1),
                new List<string> { "test" }
                );

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        [Description("Delete()::Given missing or default params when deleting Breaks then correct validation message must be returned")]
        public void DeleteBreaksByMissingOrDefaultParamsReturnsBadRequest(
            [ValueSource(nameof(_deleteByRangeTestCases))] DeleteByRangeTestData testData)
        {
            var (dateFrom, dateTo) = testData.ScheduledDatesRange;
            var result = _controller.Delete(dateFrom, dateTo, testData.SalesAreaNames) as InvalidModelStateResult;

            Assert.IsFalse(result.ModelState.IsValid);
            Assert.IsTrue(result.ModelState.ContainsKey(testData.ParamName));
        }

        private static readonly DeleteByRangeTestData[] _deleteByRangeTestCases =
        {
            new DeleteByRangeTestData
            {
                ScheduledDatesRange = new DateTimeRange(default, DateTime.Now), SalesAreaNames = new List<string> {"test"},
                ParamName = "dateRangeStart"
            },
            new DeleteByRangeTestData
            {
                ScheduledDatesRange = new DateTimeRange(DateTime.Now, default), SalesAreaNames = new List<string> {"test"},
                ParamName = "dateRangeEnd"
            },
            new DeleteByRangeTestData
            {
                ScheduledDatesRange = new DateTimeRange(DateTime.Now, DateTime.Now), SalesAreaNames = new List<string>(),
                ParamName = "salesAreaNames"
            }
        };
    }
}
