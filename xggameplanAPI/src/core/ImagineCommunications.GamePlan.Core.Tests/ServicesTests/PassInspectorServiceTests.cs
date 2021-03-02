using System;
using FluentAssertions;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using xggameplan.Model;
using Xunit;

namespace xggameplan.core.tests.Services
{
    [Trait("Core", "Pass Inspector Service")]
    public class PassInspectorServiceTests
    {
        private readonly IPassInspectorService _inspectorService;
        private readonly PassSalesAreaPriorityModel _salesAreaPriorityModel;

        public PassInspectorServiceTests()
        {
            _inspectorService = new PassInspectorService();
            _salesAreaPriorityModel = new PassSalesAreaPriorityModel()
            {
                StartTime = new TimeSpan(),
                EndTime = new TimeSpan(),
                DaysOfWeek = "1111111",
            };
        }

        [Fact(DisplayName =
            "Given inspecting a pass " +
            "When pass sales area priority start time is null " +
            "Then inspector will return an error message")]
        public void InspectPassSalesAreaPriorities_StartTimeIsNull_ErrorShouldBeReturned()
        {
            // Arrange
            _salesAreaPriorityModel.StartTime = null;

            // Act
            bool res = _inspectorService.InspectPassSalesAreaPriorities(
                _salesAreaPriorityModel,
                out string errorMessage);

            //Assert
            _ = res.Should().BeTrue();
            _ = errorMessage.Should().NotBeNull();
        }

        [Fact(DisplayName =
            "Given inspecting a pass " +
            "When pass sales area priority end time is null " +
            "Then inspector will return an error message")]
        public void InspectPassSalesAreaPriorities_EndTimeIsNull_ErrorShouldBeReturned()
        {
            // Arrange
            _salesAreaPriorityModel.EndTime = null;

            // Act
            bool res = _inspectorService.InspectPassSalesAreaPriorities(
                _salesAreaPriorityModel,
                out string errorMessage);

            // Assert
            _ = res.Should().BeTrue();
            _ = errorMessage.Should().NotBeNull();
        }

        [Fact(DisplayName =
            "Given inspecting a pass " +
            "When pass sales area priority day of week is null " +
            "Then inspector will return an error message")]
        public void InspectPassSalesAreaPriorities_DayOfWeekIsNull_ErrorShouldBeReturned()
        {
            // Arrange
            _salesAreaPriorityModel.DaysOfWeek = null;

            // Act
            bool res = _inspectorService.InspectPassSalesAreaPriorities(
                _salesAreaPriorityModel,
                out string errorMessage);

            //Assert
            _ = res.Should().BeTrue();
            _ = errorMessage.Should().NotBeNull();
        }

        [Fact(DisplayName =
            "Given inspecting a pass " +
            "When pass sales area priority day of week is present " +
            "And pass sales area priority start and end time are present " +
            "Then inspector will not return an error message")]
        public void InspectPassSalesAreaPriorities_DayOfWeekIsPresent_NoErrorShouldBeReturned()
        {
            // Arrange

            // Act
            bool res = _inspectorService.InspectPassSalesAreaPriorities(
                _salesAreaPriorityModel,
                out string errorMessage);

            // Assert
            _ = res.Should().BeFalse();
            _ = errorMessage.Should().BeNull();
        }
    }
}
