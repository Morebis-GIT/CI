using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.AutoGen.AgDataPopulation;
using xggameplan.core.Extensions;
using xggameplan.Model.AutoGen;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class DefaultDataPopulationTests : DataPopulationTestBase
    {
        private readonly string _defaultDaysOfWeek = "1111111";

        public DefaultDataPopulationTests()
        {
            _scenario = _fixture.Build<Scenario>().Create();
            _run = _fixture.Build<Run>().Create();
            _passes = _fixture.CreateMany<Pass>(2).ToList();

            _tenantSettings = _fixture.Build<TenantSettings>()
                .With(r => r.PeakStartTime, "120000")
                .With(r => r.PeakEndTime, "155959")
                .With(r => r.MidnightStartTime, "240000")
                .With(r => r.MidnightEndTime, "995959").Create();
        }

        [Fact(DisplayName = "Perform pass default data population, should return pass date time values when pass date time values is not null")]
        public void PerformPassDefaultDataPopulation_ShouldReturnPassDateTimeValues_WhenPassDateTimeValuesIsNotNull()
        {
            //Arrange
            var expected = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.EndDate, DateTime.Now.AddDays(1))
                .With(r => r.StartDate, DateTime.Now)
                .With(r => r.StartTime, new TimeSpan(21, 30, 0))
                .With(r => r.EndTime, new TimeSpan(22, 30, 0))
                .With(r => r.DaysOfWeek, "1000000")
                .Create();
            _passes[0].PassSalesAreaPriorities = expected;

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            _ = expected?.EndDate?.ToString("yyyyMMdd").Should().Be(result?.AgPassDefaults[0]?.EndDate, null);
            _ = expected.StartDate?.ToString("yyyyMMdd").Should().Be(result?.AgPassDefaults[0]?.StartDate, null);
            _ = AgConversions.ToAgTimeAsHHMMSS((TimeSpan)expected.EndTime).Should().Be(result?.AgPassDefaults[0]?.EndTime, null);
            _ = AgConversions.ToAgTimeAsHHMMSS((TimeSpan)expected.StartTime).Should().Be(result?.AgPassDefaults[0]?.StartTime, null);
            _ = AgConversions.ToAgDaysAsInt(expected.DaysOfWeek).Should().Be(result?.AgPassDefaults[0]?.DaysOfWeek, null);
        }

        [Fact(DisplayName = "Perform pass default data population, should return run date time values when pass date time values is null")]
        public void PerformPassDefaultDataPopulation_ShouldReturnRunDateTimeValues_WhenPassDateTimeValuesIsNull()
        {
            DateTime? nulldate = null;
            TimeSpan? nullTimeSpan = null;
            const string nullString = null;
            //Arrange
            _passes[0].PassSalesAreaPriorities = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.EndDate, nulldate)
                .With(r => r.StartDate, nulldate)
                .With(r => r.StartTime, nullTimeSpan)
                .With(r => r.EndTime, nullTimeSpan)
                .With(r => r.DaysOfWeek, nullString)
                .Create();

            var expected = _run;

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            var valueToTest =
                expected.EndTime.Minutes == 0 &&
                expected.EndTime.Hours == 0 &&
                expected.EndTime.Seconds == 0
                    ? "995959"
                    : AgConversions.ToAgTimeAsHHMMSS(expected.EndTime);

            _ = expected.EndDate.ToString("yyyyMMdd").Should().Be(result?.AgPassDefaults[0]?.EndDate, null);
            _ = expected.StartDate.ToString("yyyyMMdd").Should().Be(result?.AgPassDefaults[0]?.StartDate, null);
            _ = valueToTest.Should().Be(result?.AgPassDefaults[0]?.EndTime, null);
            _ = AgConversions.ToAgTimeAsHHMMSS(expected.StartTime).Should().Be(result?.AgPassDefaults[0]?.StartTime, null);
            _ = AgConversions.ToAgDaysAsInt(_defaultDaysOfWeek).Should().Be(result?.AgPassDefaults[0]?.DaysOfWeek, null);
        }

        [Fact(DisplayName = "Perform pass default data population, should return run date time values when pass sales area priorities date time values is null")]
        public void PerformPassDefaultDataPopulation_ShouldReturnRunDateTimeValues_WhenPassSalesAreaPrioritiesIsNull()
        {
            //Arrange
            _passes[0].PassSalesAreaPriorities = null;

            var expected = _run;

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            var valueToTest =
                expected.EndTime.Minutes == 0 &&
                expected.EndTime.Hours == 0 &&
                expected.EndTime.Seconds == 0
                    ? "995959"
                    : AgConversions.ToAgTimeAsHHMMSS(expected.EndTime);

            _ = expected.EndDate.ToString("yyyyMMdd").Should().Be(result?.AgPassDefaults[0]?.EndDate, null);
            _ = expected.StartDate.ToString("yyyyMMdd").Should().Be(result?.AgPassDefaults[0]?.StartDate, null);
            _ = valueToTest.Should().Be(result?.AgPassDefaults[0]?.EndTime, null);
            _ = AgConversions.ToAgTimeAsHHMMSS(expected.StartTime).Should().Be(result?.AgPassDefaults[0]?.StartTime, null);
            _ = AgConversions.ToAgDaysAsInt(_defaultDaysOfWeek).Should().Be(result?.AgPassDefaults[0]?.DaysOfWeek, null);
        }

        [Fact(DisplayName = "Perform pass default data population, should return run date time values when pass sales area priorities date time values is null")]
        public void PerformPassDefaultDataPopulation_ShouldReturnEnOfBroadCastingDayValue_WhenEndTimeIsDayEndTime()
        {
            //Arrange
            _passes[0].PassSalesAreaPriorities = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.EndTime, new TimeSpan(0, 0, 0))
                .Create();

            const string expected = AgConversions.broadCastDayValue;

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            _ = result?.AgPassDefaults[0]?.EndTime.Should().Be(expected, null);
        }

        [Fact(DisplayName = "Perform pass default data population, and peak daypart is not set in tenant settings should return ArgumentNullException")]
        public void PerformPassDefaultDataPopulation_AndPeakDaypartIsNotSetInTenantSettings_ShouldReturnArgumentNullException()
        {
            //Arrange
            _tenantSettings = _fixture.Build<TenantSettings>()
                .With(r => r.PeakStartTime, "")
                .With(r => r.PeakEndTime, "155959")
                .With(r => r.MidnightStartTime, "240000")
                .With(r => r.MidnightEndTime, "995959").Create();

            //Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
                _passes.ToAgPassDefault(_scenario, _run, _tenantSettings)
                );
        }

        [Fact(DisplayName = "Perform pass default data population, and midnight daypart is not set in tenant settings should return ArgumentNullException")]
        public void PerformPassDefaultDataPopulation_AndMidnightDaypartIsNotSetInTenantSettings_ShouldReturnArgumentNullException()
        {
            //Arrange
            _tenantSettings = _fixture.Build<TenantSettings>()
                .With(r => r.PeakStartTime, "120000")
                .With(r => r.PeakEndTime, "155959")
                .With(r => r.MidnightStartTime, "240000")
                .With(r => r.MidnightEndTime, "").Create();

            //Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
                _passes.ToAgPassDefault(_scenario, _run, _tenantSettings)
            );
        }

        [Fact(DisplayName = "Perform pass default data population, and pass sales area priorities contains Off Peak Time should return list of pass time slices")]
        public void PerformPassDefaultDataPopulation_AndPassSalesAreaPrioritiesContainsOffPeakTime_ShouldReturnListOfPassTimeSlices()
        {
            //Arrange
            _passes[0].PassSalesAreaPriorities = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.IsPeakTime, false)
                .With(r => r.IsOffPeakTime, true)
                .With(r => r.IsMidnightTime, false)
                .Create();

            var expected = new AgPassTimeSliceList
            {
                new AgPassTimeSlice
                {
                    StartTime = "0",
                    EndTime = "115959"
                },
                new AgPassTimeSlice
                {
                    StartTime = "160000",
                    EndTime = "235959"
                }
            };

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgPassDefaults[0]?.TimeSliceList).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform pass default data population, and pass sales area priorities contains Peak Time should return list of pass time slices")]
        public void PerformPassDefaultDataPopulation_AndPassSalesAreaPrioritiesContainsPeakTime_ShouldReturnListOfPassTimeSlices()
        {
            //Arrange
            _passes[0].PassSalesAreaPriorities = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.IsPeakTime, true)
                .With(r => r.IsOffPeakTime, false)
                .With(r => r.IsMidnightTime, false)
                .Create();

            var expected = new AgPassTimeSliceList
            {
                new AgPassTimeSlice
                {
                    StartTime = "120000",
                    EndTime = "155959"
                }
            };

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgPassDefaults[0]?.TimeSliceList).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform pass default data population, and pass sales area priorities contains Midnight Time should return list of pass time slices")]
        public void PerformPassDefaultDataPopulation_AndPassSalesAreaPrioritiesContainsMidnightTime_ShouldReturnListOfPassTimeSlices()
        {
            //Arrange
            _passes[0].PassSalesAreaPriorities = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.IsPeakTime, false)
                .With(r => r.IsOffPeakTime, false)
                .With(r => r.IsMidnightTime, true)
                .Create();

            var expected = new AgPassTimeSliceList
            {
                new AgPassTimeSlice
                {
                    StartTime = "240000",
                    EndTime = "995959"
                }
            };

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgPassDefaults[0]?.TimeSliceList).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform pass default data population, and pass sales area priorities contains all dayparts should return list of pass time slices")]
        public void PerformPassDefaultDataPopulation_AndPassSalesAreaPrioritiesContainsAllDayparts_ShouldReturnListOfPassTimeSlices()
        {
            //Arrange
            _passes[0].PassSalesAreaPriorities = _fixture.Build<PassSalesAreaPriority>()
                .With(r => r.IsPeakTime, true)
                .With(r => r.IsOffPeakTime, true)
                .With(r => r.IsMidnightTime, true)
                .Create();

            var expected = new AgPassTimeSliceList
            {
                new AgPassTimeSlice
                {
                    StartTime = "0",
                    EndTime = "115959"
                },
                new AgPassTimeSlice
                {
                    StartTime = "120000",
                    EndTime = "155959"
                },
                new AgPassTimeSlice
                {
                    StartTime = "160000",
                    EndTime = "235959"
                },
                new AgPassTimeSlice
                {
                    StartTime = "240000",
                    EndTime = "995959"
                }
            };

            //Act
            var result = _passes.ToAgPassDefault(_scenario, _run, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgPassDefaults[0]?.TimeSliceList).Should().BeTrue(null);
        }

        private static bool CheckIfEqual(AgPassTimeSliceList expected, AgPassTimeSliceList result)
        {
            foreach (AgPassTimeSlice timeSlice in expected)
            {
                if (!result.Any(t =>
                    t.StartTime == timeSlice.StartTime &&
                    t.EndTime == timeSlice.EndTime))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
