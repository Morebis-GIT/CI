using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class ClashExposureDataPopulationTests : DataPopulationTestBase
    {
        private readonly DateTime _runStartDate = new DateTime(2019, 10, 20);
        private readonly DateTime _runEndDate = new DateTime(2019, 10, 25);

        private readonly TimeSpan? _peakStartTime = new TimeSpan(12, 0, 0);
        private readonly TimeSpan? _peakEndTime = new TimeSpan(18, 0, 0);

        private const string AgStartDate = "20191020";
        private const string AgEndDate = "20191025";

        private const string AgStartTime = "0";
        private const string AgEndTime = "995959";

        private const string AgPeakStart = "120000";
        private const string AgPeakEnd = "180000";

        private const string AgBeforePeakStart = "115959";
        private const string AgBeforePeakEnd = "175959";

        private const int StartDay = 1;
        private const int EndDay = 7;

        private const int Zero = 0;

        public ClashExposureDataPopulationTests()
        {
            _salesAreas = _fixture.CreateMany<SalesArea>(10).ToList();
            _salesAreas.ForEach(s => s.Name = _salesAreaArr[_random.Next(0, _salesAreaArr.Length)]);
        }

        //==============================================================================
        // 10/20/19                                                             10/25/19
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        // 10/20/19
        // |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for first date")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForFirstDate()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                StartDate = _runStartDate, //20191020
                EndDate = _runStartDate, //20191020
                TimeAndDow = new TimeAndDowAPI("1111111"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 10/20/19                                                             10/25/19
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        // 10/20/19
        // |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for first date with invalid peak time")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForFirstDateWithInvalidPeakTime()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                StartDate = _runStartDate, //20191020
                EndDate = _runStartDate, //20191020
                TimeAndDow = new TimeAndDowAPI("1111111"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                (TimeSpan?)null, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 10/20/19                                                             10/25/19
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                                                                      10/25/19
        //                                                                     |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for last date")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForLastDate()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                StartDate = _runStartDate.AddDays(5), //20191025
                EndDate = _runStartDate.AddDays(5), //20191025
                TimeAndDow = new TimeAndDowAPI("1111111"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191024",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191024",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191024",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191025",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191025",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191025",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 10/20/19                                                             10/25/19
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //          10/21/19                         10/23/19
        //          |||||||||||||||||||||||||||||||||||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for date period")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForDatePeriod()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                StartDate = _runStartDate.AddDays(1), //20191021
                EndDate = _runStartDate.AddDays(3), //20191023
                TimeAndDow = new TimeAndDowAPI("1111111"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191024",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191024",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191024",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 1                                                                           7
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        // 1       1
        // |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for first DoW")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForFirstDoW()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                TimeAndDow = new TimeAndDowAPI("1000000"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 2,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 2,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 2,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 1,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 1,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 1,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 1                                                                           7
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                                                                     7       7
        //                                                                     |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for last DoW")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForLastDoW()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                TimeAndDow = new TimeAndDowAPI("0000001"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 6,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 6,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 6,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 7,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 7,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 7,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 1                                                                           7
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        // 1                               3         5                                 7
        // |||||||||||||||||||||||||||||||||         |||||||||||||||||||||||||||||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for DoW")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForDoWRanges()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                TimeAndDow = new TimeAndDowAPI("1110111"),
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 4,
                    EndDay = 4,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 4,
                    EndDay = 4,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 4,
                    EndDay = 4,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 6:00                                                                     5:59
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                     12:00                      18:00
        //                       |                          |
        // 6:00-10:00
        // |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for first off-peak time")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForFirstOffPeakTime()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                TimeAndDow = new TimeAndDowAPI("1111111")
                {
                    EndTime = new TimeSpan(10, 0, 0),
                },
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = "100000",
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = "095959",
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 6:00                                                                     5:59
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                     12:00                      18:00
        //                       |                          |
        //                                                                    23:00-5:59
        //                                                                     |||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for last off-peak time")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForLastOffPeakTime()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                TimeAndDow = new TimeAndDowAPI("1111111")
                {
                    StartTime = new TimeSpan(23, 0, 0),
                },
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = "225959",
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = "230000",
                    EndTime = AgEndTime,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 6:00                                                                     5:59
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                     12:00                      18:00
        //                       |                          |
        //           10:00                                            23:00
        //           ||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for peak and off-peak time")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForPeakAndOffPeakTime()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                TimeAndDow = new TimeAndDowAPI("1111111")
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(23, 0, 0)
                },
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = "095959",
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = "230000",
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = "100000",
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = "225959",
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 10/20/19                                                             10/25/19
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //          10/21/19                         10/23/19
        //          |||||||||||||||||||||||||||||||||||||||||
        //==============================================================================
        // 1                                                                           7
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        // 1                               3         5                                 7
        // |||||||||||||||||||||||||||||||||         |||||||||||||||||||||||||||||||||||
        //==============================================================================
        // 6:00                                                                     5:59
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                     12:00                      18:00
        //                       |                          |
        //           10:00                                            23:00
        //           ||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain difference exposures for dates dows and times")]
        public void PerformClashExposureDataPopulationShouldContainDifferenceExposuresForDatesDowsAndTimes()
        {
            #region Test data

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var testDifferenceForAllSalesAreas = new ClashDifference
            {
                StartDate = _runStartDate.AddDays(1), //20191021
                EndDate = _runStartDate.AddDays(3), //20191023
                TimeAndDow = new TimeAndDowAPI("1110111")
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(23, 0, 0)
                },
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(testDifferenceForAllSalesAreas);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191024",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = AgStartTime,
                    EndTime = "095959",
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = "100000",
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191024",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = AgStartDate,
                    EndDate = "20191020",
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191024",
                    EndDate = AgEndDate,
                    StartDay = StartDay,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = "230000",
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = StartDay,
                    EndDay = 3,
                    StartTime = AgPeakEnd,
                    EndTime = "225959",
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 4,
                    EndDay = 4,
                    StartTime = AgStartTime,
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = AgStartTime,
                    EndTime = "095959",
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = "100000",
                    EndTime = AgBeforePeakStart,
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 4,
                    EndDay = 4,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testClash.DefaultPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = AgPeakStart,
                    EndTime = AgBeforePeakEnd,
                    NoOfExposures = testDifferenceForAllSalesAreas.PeakExposureCount.Value
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 4,
                    EndDay = 4,
                    StartTime = AgPeakEnd,
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = "230000",
                    EndTime = AgEndTime,
                    NoOfExposures = testClash.DefaultOffPeakExposureCount
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = testClash.Externalref,
                    MasterClashCode = testClash.ParentExternalidentifier,
                    StartDate = "20191021",
                    EndDate = "20191023",
                    StartDay = 5,
                    EndDay = EndDay,
                    StartTime = AgPeakEnd,
                    EndTime = "225959",
                    NoOfExposures = testDifferenceForAllSalesAreas.OffPeakExposureCount.Value
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, _runStartDate, _runEndDate,
                _peakStartTime, _peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        //==============================================================================
        // 10/01/19                                                             10/30/19
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //             10/10/19     10/14/19
        //             |||||||||||||||||||||
        //
        //                                       10/16/19     10/18/19
        //                                       |||||||||||||||||||||
        //==============================================================================
        // 1                                                                           7
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                      3                    4
        //                      ||||||||||||||||||||||
        //
        //                      3                    4
        //                      ||||||||||||||||||||||
        //==============================================================================
        // 6:00                                                                     5:59
        // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        //
        //                                   18:00      21:00
        //                                     |          |
        //                              16:00      20:00
        //                              ||||||||||||||||
        //
        //              10:00              18:00
        //              ||||||||||||||||||||||||
        //==============================================================================
        [Fact(DisplayName = "Perform Clash Exposure data population should contain multiple differences exposures for dates dows and times")]
        public void PerformClashExposureDataPopulationShouldContainMultipleDifferencesExposuresForDatesDowsAndTimes()
        {
            #region Test data

            var runStartDate = new DateTime(2019, 10, 1);
            var runEndDate = new DateTime(2019, 10, 30);

            TimeSpan? peakStartTime = new TimeSpan(18, 0, 0);
            TimeSpan? peakEndTime = new TimeSpan(21, 0, 0);

            const string agTestStartDate = "20191001";
            const string agTestEndDate = "20191030";

            const string agTestPeakStart = "180000";
            const string agTestPeakEnd = "210000";

            const string agBeforeTestPeakStart = "175959";
            const string agBeforeTestPeakEnd = "205959";

            var testClash = new Clash
            {
                Externalref = "extRef1",
                ParentExternalidentifier = "Par1",
                DefaultPeakExposureCount = 2,
                DefaultOffPeakExposureCount = 3,
                Differences = new List<ClashDifference>()
            };

            var firstTestDifference = new ClashDifference
            {
                StartDate = new DateTime(2019, 10, 10),
                EndDate = new DateTime(2019, 10, 14),
                TimeAndDow = new TimeAndDowAPI("0011000")
                {
                    StartTime = new TimeSpan(16, 0, 0),
                    EndTime = new TimeSpan(20, 0, 0)
                },
                OffPeakExposureCount = 4,
                PeakExposureCount = 5
            };

            testClash.Differences.Add(firstTestDifference);

            var secondTestDifference = new ClashDifference
            {
                StartDate = new DateTime(2019, 10, 16),
                EndDate = new DateTime(2019, 10, 18),
                TimeAndDow = new TimeAndDowAPI("0011000")
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0)
                },
                OffPeakExposureCount = 6,
                PeakExposureCount = 7
            };

            testClash.Differences.Add(secondTestDifference);

            var expected = new List<AgExposure>
            {
                new AgExposure
                {
                    BreakSalesAreaNo = 0,
                    ClashCode = "extRef1",
                    EndDate = agTestEndDate,
                    EndDay = EndDay,
                    EndTime = AgEndTime,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = agTestStartDate,
                    StartDay = StartDay,
                    StartTime = agTestPeakEnd
                },
                new AgExposure
                {
                    BreakSalesAreaNo = 0,
                    ClashCode = "extRef1",
                    EndDate = "20191009",
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = agTestStartDate,
                    StartDay = StartDay,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = 2,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191010",
                    StartDay = StartDay,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191010",
                    StartDay = 5,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = 4,
                    EndTime = "155959",
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191010",
                    StartDay = 3,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = 4,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 4,
                    StartDate = "20191010",
                    StartDay = 3,
                    StartTime = "160000"
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191009",
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakEnd,
                    MasterClashCode = "Par1",
                    NoOfExposures = 2,
                    StartDate = agTestStartDate,
                    StartDay = StartDay,
                    StartTime = agTestPeakStart
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = agTestEndDate,
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakEnd,
                    MasterClashCode = "Par1",
                    NoOfExposures = 2,
                    StartDate = "20191015",
                    StartDay = StartDay,
                    StartTime = agTestPeakStart
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = 2,
                    EndTime = agBeforeTestPeakEnd,
                    MasterClashCode = "Par1",
                    NoOfExposures = 2,
                    StartDate = "20191010",
                    StartDay = StartDay,
                    StartTime = agTestPeakStart
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakEnd,
                    MasterClashCode = "Par1",
                    NoOfExposures = 2,
                    StartDate = "20191010",
                    StartDay = 5,
                    StartTime = agTestPeakStart
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = 4,
                    EndTime = agBeforeTestPeakEnd,
                    MasterClashCode = "Par1",
                    NoOfExposures = 2,
                    StartDate = "20191010",
                    StartDay = 3,
                    StartTime = "200000"
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191014",
                    EndDay = 4,
                    EndTime = "195959",
                    MasterClashCode = "Par1",
                    NoOfExposures = 5,
                    StartDate = "20191010",
                    StartDay = 3,
                    StartTime = agTestPeakStart
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191015",
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191015",
                    StartDay = StartDay,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = agTestEndDate,
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191019",
                    StartDay = StartDay,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191018",
                    EndDay = 2,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191016",
                    StartDay = StartDay,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191018",
                    EndDay = EndDay,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191016",
                    StartDay = 5,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191018",
                    EndDay = 4,
                    EndTime = "095959",
                    MasterClashCode = "Par1",
                    NoOfExposures = 3,
                    StartDate = "20191016",
                    StartDay = 3,
                    StartTime = AgStartTime
                },
                new AgExposure
                {
                    BreakSalesAreaNo = Zero,
                    ClashCode = "extRef1",
                    EndDate = "20191018",
                    EndDay = 4,
                    EndTime = agBeforeTestPeakStart,
                    MasterClashCode = "Par1",
                    NoOfExposures = 6,
                    StartDate = "20191016",
                    StartDay = 3,
                    StartTime = "100000"
                }
            };

            #endregion Test data

            var testClashes = new List<Clash> { testClash };

            var agExposures = _mapper.Map<List<AgExposure>>(Tuple.Create(testClashes, runStartDate, runEndDate,
                peakStartTime, peakEndTime, _salesAreas, _agExposure));

            //Assert
            _ = CheckIfEqual(agExposures, expected).Should().BeTrue(null);
        }

        private static bool CheckIfEqual(IEnumerable<AgExposure> result, IEnumerable<AgExposure> expected)
        {
            foreach (AgExposure exposure in expected)
            {
                if (!result.Any(e =>
                    e.BreakSalesAreaNo == exposure.BreakSalesAreaNo &&
                    e.StartDate == exposure.StartDate && e.EndDate == exposure.EndDate &&
                    e.StartDay == exposure.StartDay && e.EndDay == exposure.EndDay &&
                    e.StartTime == exposure.StartTime && e.EndTime == exposure.EndTime &&
                    e.NoOfExposures == exposure.NoOfExposures))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
