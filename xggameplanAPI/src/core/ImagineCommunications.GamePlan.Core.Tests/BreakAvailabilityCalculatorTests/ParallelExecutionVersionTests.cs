using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using Moq;
using NodaTime;
using xggameplan.core.Interfaces;
using xggameplan.core.RunManagement.BreakAvailabilityCalculator;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    /// <summary>
    /// There is another SQL Server version of the BreakAvailabilityCalculator
    /// which uses Tasks.
    /// </summary>
    [Trait("Service", "Break Availability Calculator :: Original parallel execution version")]
    public sealed class ParallelExecutionVersionTests
        : IDisposable
    {
        private Mock<IBreakRepository> _mockBreakRepository;
        private Mock<IProgrammeRepository> _mockProgrammeRepository;
        private Mock<IScheduleRepository> _mockScheduleRepository;
        private Mock<ISpotRepository> _mockSpotRepository;
        private Mock<IRepositoryFactory> _mockRepositoryFactory;
        private Mock<IRepositoryScope> _mockRepositoryScope;

        private Fixture _fixture;
        private SalesArea[] _salesAreas;
        private readonly string _salesAreaName;

        private readonly DateTimeRange _period = new DateTimeRange(
                start: new DateTime(2020, 12, 24, 6, 0, 0, DateTimeKind.Utc),
                end: new DateTime(2020, 12, 25, 5, 59, 59, DateTimeKind.Utc)
                );

        private StubLogger<IRecalculateBreakAvailabilityService> stubLogger { get; }

        public ParallelExecutionVersionTests(ITestOutputHelper output)
        {
            _fixture = new SafeFixture();
            _salesAreas = _fixture.CreateMany<SalesArea>(1).ToArray();
            _salesAreaName = _salesAreas[0].Name;

            _mockRepositoryScope = new Mock<IRepositoryScope>();

            _mockRepositoryFactory = new Mock<IRepositoryFactory>();
            _ = _mockRepositoryFactory
                .Setup(m => m.BeginRepositoryScope())
                .Returns(_mockRepositoryScope.Object);

            _mockSpotRepository = new Mock<ISpotRepository>();
            _mockProgrammeRepository = new Mock<IProgrammeRepository>();
            _mockBreakRepository = new Mock<IBreakRepository>();
            _mockScheduleRepository = new Mock<IScheduleRepository>();

            _ = _mockRepositoryScope
                .Setup(m => m.CreateRepository<ISpotRepository>())
                .Returns(_mockSpotRepository.Object);

            _ = _mockRepositoryScope
                .Setup(m => m.CreateRepository<IProgrammeRepository>())
                .Returns(_mockProgrammeRepository.Object);

            _ = _mockRepositoryScope
                .Setup(m => m.CreateRepository<IBreakRepository>())
                .Returns(_mockBreakRepository.Object);

            _ = _mockRepositoryScope
                .Setup(m => m.CreateRepository<IScheduleRepository>())
                .Returns(_mockScheduleRepository.Object);

            stubLogger = new StubLogger<IRecalculateBreakAvailabilityService>(output);
        }

        [Fact(DisplayName = "Given no sales areas, a warning is logged")]
        public void GivenNoSalesAreasAWarningIsLogged()
        {
            // Arrange
            var service = new RecalculateBreakAvailabilityService(
                _mockRepositoryFactory.Object,
                stubLogger
                );

            // Act
            service.Execute(
                _period,
                Enumerable.Empty<SalesArea>()
                );

            // Assert
            _ = stubLogger.Messages
                .Should()
                .ContainSingle(x => x.Equals("No sales areas were passed to the calculator."), null);
        }

        [Fact(DisplayName = "Given many spots, an information message is logged with the number of spots")]
        public void GivenManySpotsInformationOnManySpotsIsLogged()
        {
            // Arrange
            var mockSalesAreaName = _fixture.Create<string>();

            var mockSpots = _fixture
                .Build<Spot>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.StartDateTime, _period.Start)
                .CreateMany(4)
                .ToList();

            var mockProgrammes = _fixture
                .Build<Programme>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.StartDateTime, _period.Start)
                .CreateMany(2)
                .ToList();

            var mockBreaks = _fixture
                .Build<Break>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.ScheduledDate, _period.Start)
                .With(p => p.Duration, Duration.FromSeconds(180))
                .CreateMany()
                .ToList();

            _ = _mockBreakRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(mockBreaks);

            _ = _mockSpotRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<string>>()))
                .Returns(mockSpots);

            _ = _mockProgrammeRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(mockProgrammes);

            var service = new RecalculateBreakAvailabilityService(
                _mockRepositoryFactory.Object,
                stubLogger
                );

            var mockSalesArea = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, mockSalesAreaName)
                .Create();

            // Act
            service.Execute(
                _period,
                new List<SalesArea>(1) { mockSalesArea }
                );

            // Assert
            _ = stubLogger.Messages
                .Should()
                .Contain(x => x.StartsWith($"Found {mockSpots.Count.ToString()} spots for sales area {mockSalesAreaName}"), null);
        }

        [Fact(DisplayName =
@"Given a break has one booked spot
And the scheduled break availability has not been reduced for the spot
Then the schedule break availability must be reduced to the same availability as the break"
            )]
        public void AScheduleBreakMustHaveTheSameAvailabilityAsTheOrdinaryBreak()
        {
            // Arrange
            var mockSalesAreaName = _fixture.Create<string>();

            var mockSpots = _fixture
                .Build<Spot>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.StartDateTime, _period.Start)
                .With(p => p.SpotLength, Duration.FromSeconds(30))
                .With(p => p.ClientPicked, true)
                .Without(p => p.ExternalBreakNo)
                .CreateMany(4)
                .ToList();

            var mockProgramme = _fixture
                .Build<Programme>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.StartDateTime, _period.Start)
                .With(p => p.Duration, Duration.FromHours(1))
                .Create();

            var mockBreaks = _fixture
                .Build<Break>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.ScheduledDate, _period.Start)
                .With(p => p.Duration, Duration.FromSeconds(180))
                .With(p => p.Avail, Duration.FromSeconds(180))
                .With(p => p.OptimizerAvail, Duration.FromSeconds(180))
                .CreateMany()
                .ToList();

            var mockSchedule = _fixture
                .Build<Schedule>()
                .With(p => p.SalesArea, mockSalesAreaName)
                .With(p => p.Date, _period.Start)
                .Create();

            CreateTheSchedule(mockProgramme, mockBreaks, mockSchedule);
            AddSpotToBreak(mockSpots, mockBreaks, mockSchedule);

            _ = mockSchedule.Breaks[0].Avail.Should().NotBe(mockBreaks[0].Avail, null);

            _ = _mockBreakRepository
                .Setup(m =>
                    m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(mockBreaks);

            _ = _mockScheduleRepository
                .Setup(m =>
                    m.GetSchedule(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(mockSchedule);

            _mockScheduleRepository
                .Setup(m => m.Update(It.IsAny<Schedule>()))
                .Verifiable();

            _ = _mockSpotRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<string>>()))
                .Returns(mockSpots);

            _ = _mockProgrammeRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(new List<Programme>(1) { mockProgramme });

            var service = new RecalculateBreakAvailabilityService(
                _mockRepositoryFactory.Object,
                stubLogger
                );

            var mockSalesArea = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, mockSalesAreaName)
                .Create();

            // Act
            service.Execute(
                _period,
                new List<SalesArea>(1) { mockSalesArea }
                );

            // Assert
            _ = stubLogger.Messages
                .Should()
                .Contain(x => x.StartsWith("Found schedule for sales area"), null);

            _ = mockSchedule.Breaks[0].Avail.Should().Be(mockBreaks[0].Avail, null);

            var targetBreak = mockBreaks.First(y =>
                y.ExternalBreakRef.Equals("Break_0"));

            var targetScheduleBreak = mockSchedule.Breaks.First(y =>
                y.ExternalBreakRef.Equals("Break_0"));

            _mockScheduleRepository.Verify(m =>
                m.Update(
                    It.Is<Schedule>(x => x.Breaks.Contains(targetScheduleBreak))
                    ));

            _ = targetBreak.Avail.Should().Be(Duration.FromSeconds(150), null);
            _ = targetBreak.OptimizerAvail.Should().Be(Duration.FromSeconds(120), null);

            _ = targetScheduleBreak.Avail.Should().Be(Duration.FromSeconds(150), null);
            _ = targetScheduleBreak.OptimizerAvail.Should().Be(Duration.FromSeconds(120), null);
        }

        [Fact(DisplayName =
@"Given a programme that starts and finishes before midnight
When the break availabilities are recalculated
Then all of the breaks are recalculated"
)]
        public void ProgrammeStartsAndFinishesBeforeMidnight()
        {
            // Arrange
            Programme programme1 = CreateProgramme(
                _period.Start.Date.AddHours(23),
                Duration.FromMinutes(29)
                );

            Programme programme2 = CreateProgramme(
                _period.Start.Date.AddHours(23).AddMinutes(30),
                Duration.FromMinutes(29)
                );

            var breakOneStart = programme1.StartDateTime.AddMinutes(5);
            var breakTwoStart = programme1.StartDateTime.AddMinutes(20);

            List<Break> breaks = CreateBreaks(breakOneStart, breakTwoStart);
            List<Break> scheduleBreaks = CreateBreaks(breakOneStart, breakTwoStart);

            LinkBreaks(breaks, scheduleBreaks);

            List<Spot> spots = CreateSpots(breaks);
            List<Schedule> schedules = CreateSchedules(programme1, programme2, scheduleBreaks);

            _ = _mockProgrammeRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsNotNull<string>()))
                .Returns(new List<Programme> { programme1, programme2 });

            _ = _mockSpotRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsNotNull<List<string>>()))
                .Returns(spots);

            _ = _mockBreakRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsNotNull<string>()))
                .Returns(breaks);

            _ = _mockScheduleRepository
                .Setup(m => m.GetSchedule(It.IsNotNull<string>(), It.IsAny<DateTime>()))
                .Returns(schedules[0]);

            var service = new RecalculateBreakAvailabilityService(
                _mockRepositoryFactory.Object,
                stubLogger);

            // Act
            service.Execute(_period, _salesAreas);

            // Assert
            _mockBreakRepository.Verify(m => m.SaveChanges(), Times.Once);
            _mockScheduleRepository.Verify(m => m.SaveChanges(), Times.Once);

            _mockBreakRepository.Verify(m =>
                m.Add(It.Is<Break>(x =>
                    x.ScheduledDate.Date == breakOneStart.Date &&
                    x.Avail == Duration.FromSeconds(150) &&
                    x.OptimizerAvail == Duration.FromSeconds(120)
                    )),
                Times.Exactly(4)
                );

            _mockBreakRepository.Verify(m =>
                 m.Add(It.Is<Break>(x =>
                     x.ScheduledDate.Date == breakTwoStart.Date &&
                     x.Avail == Duration.FromSeconds(150) &&
                     x.OptimizerAvail == Duration.FromSeconds(120)
                     )),
                 Times.Exactly(4)
                 );

            _mockScheduleRepository.Verify(m =>
                m.Update(It.Is<Schedule>(x =>
                    x.Date.Date == breakOneStart.Date &&
                    x.Breaks[0].Avail == Duration.FromSeconds(150) &&
                    x.Breaks[0].OptimizerAvail == Duration.FromSeconds(120)
                    )),
                Times.Once
                );

            _mockScheduleRepository.Verify(m =>
                m.Update(It.Is<Schedule>(x =>
                    x.Date.Date == breakTwoStart.Date &&
                    x.Breaks[0].Avail == Duration.FromSeconds(150) &&
                    x.Breaks[0].OptimizerAvail == Duration.FromSeconds(120)
                    )),
                Times.Once
                );
        }

        [Fact(DisplayName =
@"Given a programme that starts before midnight and finishes after midnight
When the break availabilities are recalculated
Then all of the breaks are recalculated"
)]
        public void ProgrammeStartsBeforeMidnightAndFinishesAfterMidnight()
        {
            // Arrange
            Programme programme1 = CreateProgramme(
                _period.Start.Date.AddHours(23),
                Duration.FromMinutes(30)
                );

            Programme programme2 = CreateProgramme(
                _period.Start.Date.AddHours(23).AddMinutes(30),
                Duration.FromMinutes(60)
                );

            var breakOneStart = programme2.StartDateTime.AddMinutes(5);
            var breakTwoStart = programme2.StartDateTime.AddMinutes(45);

            List<Break> breaks = CreateBreaks(breakOneStart, breakTwoStart);
            List<Break> scheduleBreaks = CreateBreaks(breakOneStart, breakTwoStart);

            LinkBreaks(breaks, scheduleBreaks);

            List<Spot> spots = CreateSpots(breaks);
            List<Schedule> schedules = CreateSchedules(programme1, programme2, scheduleBreaks);

            _ = _mockProgrammeRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsNotNull<string>()))
                .Returns(new List<Programme> { programme1, programme2 });

            _ = _mockSpotRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsNotNull<List<string>>()))
                .Returns(spots);

            _ = _mockBreakRepository
                .Setup(m => m.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsNotNull<string>()))
                .Returns(breaks);

            _ = _mockScheduleRepository
                .Setup(m => m.GetSchedule(It.IsNotNull<string>(), It.Is<DateTime>(x => x.Date == breakOneStart.Date)))
                .Returns(schedules[0]);

            _ = _mockScheduleRepository
                .Setup(m => m.GetSchedule(It.IsNotNull<string>(), It.Is<DateTime>(x => x.Date == breakTwoStart.Date)))
                .Returns(schedules[1]);

            var service = new RecalculateBreakAvailabilityService(_mockRepositoryFactory.Object, stubLogger);

            // Act
            service.Execute(_period, _salesAreas);

            // Assert
            _mockBreakRepository.Verify(m => m.SaveChanges(), Times.Once);
            _mockScheduleRepository.Verify(m => m.SaveChanges(), Times.Once);

            _mockBreakRepository.Verify(m =>
                m.Add(It.Is<Break>(x =>
                    x.ScheduledDate.Date == breakOneStart.Date &&
                    x.Avail == Duration.FromSeconds(150) &&
                    x.OptimizerAvail == Duration.FromSeconds(120)
                    )),
                Times.Exactly(4)
                );

            _mockBreakRepository.Verify(m =>
                 m.Add(It.Is<Break>(x =>
                     x.ScheduledDate.Date == breakTwoStart.Date &&
                     x.Avail == Duration.FromSeconds(150) &&
                     x.OptimizerAvail == Duration.FromSeconds(120)
                     )),
                 Times.Exactly(4)
                 );

            _mockScheduleRepository.Verify(m =>
                m.Update(It.Is<Schedule>(x =>
                    x.Date.Date == breakOneStart.Date &&
                    x.Breaks[0].Avail == Duration.FromSeconds(150) &&
                    x.Breaks[0].OptimizerAvail == Duration.FromSeconds(120)
                    )),
                Times.Once
                );

            _mockScheduleRepository.Verify(m =>
                m.Update(It.Is<Schedule>(x =>
                    x.Date.Date == breakTwoStart.Date &&
                    x.Breaks[0].Avail == Duration.FromSeconds(150) &&
                    x.Breaks[0].OptimizerAvail == Duration.FromSeconds(120)
                    )),
                Times.Exactly(2)
                );
        }

        private List<Schedule> CreateSchedules(
            Programme programme1,
            Programme programme2,
            IReadOnlyList<Break> scheduleBreaks)
        {
            if (scheduleBreaks[0].ScheduledDate.Date == scheduleBreaks[1].ScheduledDate.Date)
            {
                return _fixture.Build<Schedule>()
                    .With(p => p.Date, _period.Start)
                    .With(p => p.SalesArea, _salesAreaName)
                    .With(p => p.Breaks, scheduleBreaks)
                    .With(p => p.Programmes, new List<Programme> { programme1, programme2 })
                    .CreateMany(1)
                    .ToList();
            }

            // A programme spans midnight so the breaks need to span two schedules.
            return new List<Schedule> {
                _fixture.Build<Schedule>()
                    .With(p => p.Date, scheduleBreaks[0].ScheduledDate.Date)
                    .With(p => p.SalesArea, _salesAreaName)
                    .With(p => p.Breaks, new List<Break> { scheduleBreaks[0] })
                    .With(p => p.Programmes, new List<Programme> { programme1, programme2 })
                    .Create(),
                _fixture.Build<Schedule>()
                    .With(p => p.Date, scheduleBreaks[1].ScheduledDate.Date)
                    .With(p => p.SalesArea, _salesAreaName)
                    .With(p => p.Breaks, new List<Break> { scheduleBreaks[1] })
                    .Without(p => p.Programmes)
                    .Create()
            };
        }

        private List<Spot> CreateSpots(IReadOnlyList<Break> breaks)
        {
            var spots = _fixture.Build<Spot>()
                .With(p => p.SalesArea, _salesAreaName)
                .With(p => p.SpotLength, Duration.FromSeconds(30))
                .Without(p => p.ExternalBreakNo)
                .CreateMany(4)
                .ToList();

            spots[0].StartDateTime = breaks[0].ScheduledDate;
            spots[1].StartDateTime = breaks[0].ScheduledDate;
            spots[2].StartDateTime = breaks[1].ScheduledDate;
            spots[3].StartDateTime = breaks[1].ScheduledDate;

            // Don't place the second spot in each break so its duration can be
            // reallocated across the breaks.
            spots[0].ExternalBreakNo = breaks[0].ExternalBreakRef;
            spots[2].ExternalBreakNo = breaks[1].ExternalBreakRef;

            return spots;
        }

        /// <summary>
        /// Ensure the Breaks and Schedule Breaks have the same external break references.
        /// </summary>
        /// <param name="breaks"></param>
        /// <param name="scheduleBreaks"></param>
        private static void LinkBreaks(List<Break> breaks, List<Break> scheduleBreaks)
        {
            scheduleBreaks[0].ExternalBreakRef = breaks[0].ExternalBreakRef;
            scheduleBreaks[1].ExternalBreakRef = breaks[1].ExternalBreakRef;
        }

        private Programme CreateProgramme(DateTime startTime, Duration duration)
        {
            return _fixture.Build<Programme>()
                .With(p => p.SalesArea, _salesAreaName)
                .With(p => p.StartDateTime, startTime)
                .With(p => p.Duration, duration)
                .Create();
        }

        private List<Break> CreateBreaks(DateTime breakOneStart, DateTime breakTwoStart)
        {
            var breaks = _fixture.Build<Break>()
                .With(p => p.SalesArea, _salesAreaName)
                .With(p => p.Duration, Duration.FromMinutes(3))
                .CreateMany(2)
                .ToList();

            breaks[0].ScheduledDate = breakOneStart;
            breaks[0].Avail = Duration.FromMinutes(1);
            breaks[0].OptimizerAvail = Duration.FromMinutes(1);

            breaks[1].ScheduledDate = breakTwoStart;
            breaks[1].Avail = Duration.FromMinutes(1);
            breaks[1].OptimizerAvail = Duration.FromMinutes(1);

            return breaks;
        }

        private static void AddSpotToBreak(List<Spot> mockSpots, List<Break> mockBreaks, Schedule mockSchedule)
        {
            mockSchedule.Breaks[0].Avail -= Duration.FromSeconds(60);
            mockSpots[0].ExternalBreakNo = mockBreaks[0].ExternalBreakRef;
        }

        private static void CreateTheSchedule(Programme mockProgramme, List<Break> mockBreaks, Schedule mockSchedule)
        {
            mockSchedule.Breaks.Clear();
            mockSchedule.Programmes.Clear();

            mockBreaks[0].ExternalBreakRef = "Break_0";

            foreach (var item in mockBreaks)
            {
                mockSchedule.Breaks.Add((Break)item.Clone());
            }

            mockSchedule.Programmes.Add(mockProgramme);
        }

        public void Dispose()
        {
            _fixture = null;
            _salesAreas = null;

            _mockRepositoryScope = null;
            _mockRepositoryFactory = null;
            _mockSpotRepository = null;
            _mockProgrammeRepository = null;
            _mockBreakRepository = null;
            _mockScheduleRepository = null;
        }
    }
}
