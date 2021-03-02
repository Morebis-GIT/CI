using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Model;
using xggameplan.Model.AutoGen;

[assembly: InternalsVisibleTo("xggameplan.core.tests")]
[assembly: InternalsVisibleTo("ImagineCommunications.GamePlan.Core.Tests")]

namespace xggameplan.Profile
{
    internal class ClashProfile : AutoMapper.Profile
    {
        public ClashProfile()
        {
            _ = CreateMap<CreateClash, Clash>();
            _ = CreateMap<Tuple<Clash, Clash>, ClashModel>().ConstructUsing(t => LoadClashModel(t.Item1, t.Item2));

            _ = CreateMap<Tuple<List<Clash>, DateTime, DateTime, TimeSpan?, TimeSpan?, List<SalesArea>, AgExposure>, List<AgExposure>>()
                .ConstructUsing(t => LoadAgExposures(
                    t.Item1,
                    t.Item2,
                    t.Item3,
                    t.Item4,
                    t.Item5,
                    t.Item6,
                    t.Item7));

            _ = CreateMap<Clash, ClashNameModel>();
        }

        private static readonly TimeSpan _defaultDayStartTime = new TimeSpan(6, 0, 0);
        private static readonly TimeSpan _defaultDayEndTime = new TimeSpan(99, 59, 59); // 05:59:59 AM

        private const int AllSalesAreas = 0;
        private const int StartDay = 1;
        private const int EndDay = 7;

        private static TimeSpan? _peakStartTime;
        private static TimeSpan? _peakEndTime;

        private static List<TimeSpan> _breakTimes;

        private static List<SalesArea> _salesAreas;

        private ClashModel LoadClashModel(Clash clash, Clash parentClash)
        {
            return new ClashModel
            {
                ParentExternalidentifier = clash.ParentExternalidentifier,
                Description = clash.Description,
                DefaultPeakExposureCount = clash.DefaultPeakExposureCount,
                DefaultOffPeakExposureCount = clash.DefaultOffPeakExposureCount,
                Externalref = clash.Externalref,
                ParentClashDescription = parentClash?.Description,
                ParentPeakExposureCount = parentClash?.DefaultPeakExposureCount,
                ParentOffPeakExposureCount = parentClash?.DefaultOffPeakExposureCount,
                Differences = clash.Differences ?? new List<ClashDifference>(),
                Uid = clash.Uid
            };
        }

        private static List<AgExposure> LoadAgExposures(
            List<Clash> clashes,
            DateTime startDate,
            DateTime endDate,
            TimeSpan? peakStartTime,
            TimeSpan? peakEndTime,
            List<SalesArea> salesAreas,
            AgExposure agExposure)
        {
            #region Preset clashes data

            _salesAreas = salesAreas;

            _peakStartTime = peakStartTime;
            _peakEndTime = peakEndTime;

            _breakTimes = new List<TimeSpan> { _defaultDayStartTime, _defaultDayEndTime };

            if (_peakStartTime.HasValue && _peakEndTime.HasValue)
            {
                _breakTimes.AddRange(new List<TimeSpan> { _peakStartTime.Value, _peakEndTime.Value });
                _breakTimes = _breakTimes.OrderBy(r => r).ToList();
            }

            var exposures = new List<AgExposure>();

            #endregion Preset clashes data

            foreach (var clash in clashes)
            {
                var clashExposures = new List<AgUnformattedExposure>();

                var masterClashCode = clash.ParentExternalidentifier ?? clash.Externalref;

                clashExposures.AddRange(CreateExposures(AllSalesAreas, clash.Externalref, masterClashCode, clash.DefaultPeakExposureCount, clash.DefaultOffPeakExposureCount,
                    startDate, endDate, _defaultDayStartTime, _defaultDayEndTime, StartDay, EndDay));

                if (clash.Differences != null)
                {
                    var differencesForAllSalesAreas =
                        clash.Differences.Where(d => string.IsNullOrWhiteSpace(d.SalesArea)).ToList();

                    foreach (var difference in differencesForAllSalesAreas)
                    {
                        var differenceExposures = GenerateDifferenceExposures(difference, clash.Externalref, masterClashCode,
                            clash.DefaultPeakExposureCount, clash.DefaultOffPeakExposureCount, startDate, endDate);

                        foreach (var differenceExposure in differenceExposures)
                        {
                            var exposuresForAllSalesAreas =
                                clashExposures.Where(e => e.BreakSalesAreaNo == AllSalesAreas).ToList();

                            var processResult = ProcessOverlappedExposure(exposuresForAllSalesAreas, differenceExposure);

                            if (processResult is null)
                            {
                                clashExposures.Add(differenceExposure);
                            }
                            else
                            {
                                _ = clashExposures.Remove(processResult.Item2);
                                clashExposures.AddRange(processResult.Item1);
                            }
                        }
                    }

                    var differencesForParticularSalesArea =
                        clash.Differences.Where(d => !string.IsNullOrWhiteSpace(d.SalesArea)).ToList();

                    foreach (var difference in differencesForParticularSalesArea)
                    {
                        var differenceExposures = GenerateDifferenceExposures(difference, clash.Externalref, masterClashCode,
                            clash.DefaultPeakExposureCount, clash.DefaultOffPeakExposureCount, startDate, endDate);

                        clashExposures.AddRange(differenceExposures);
                    }
                }

                exposures.AddRange(ConvertToAgExposures(clashExposures));
            }

            return exposures;
        }

        private static IEnumerable<AgUnformattedExposure> GenerateDifferenceExposures(ClashDifference difference, string clashCode, string masterClashCode,
            int defaultPeakExposureCount, int defaultOffPeakExposureCount, DateTime startDate, DateTime endDate)
        {
            #region Preset difference data

            var differenceExposures = new List<AgUnformattedExposure>();

            var salesArea = !string.IsNullOrWhiteSpace(difference.SalesArea)
                ? _salesAreas.FirstOrDefault(s => s.Name == difference.SalesArea) : null;

            var salesAreaNumber = salesArea?.CustomId ?? AllSalesAreas;

            var differenceStartDate = difference.StartDate ?? startDate;
            var differenceEndDate = difference.EndDate ?? endDate;

            var differenceStartTime = difference.TimeAndDow.StartTime ?? _defaultDayStartTime;
            var differenceEndTime = difference.TimeAndDow.EndTime ?? _defaultDayEndTime;

            var peakExposureCount = difference.PeakExposureCount ?? defaultPeakExposureCount;
            var offPeakExposureCount = difference.OffPeakExposureCount ?? defaultOffPeakExposureCount;

            #endregion Preset difference data

            foreach (var dowPeriod in GetDowPeriods(difference.TimeAndDow.DaysOfWeekBinary))
            {
                var differenceStartDay = dowPeriod[0];
                var differenceEndDay = dowPeriod[1];

                differenceExposures.AddRange(CreateExposures(salesAreaNumber, clashCode, masterClashCode, peakExposureCount, offPeakExposureCount,
                    differenceStartDate, differenceEndDate, differenceStartTime, differenceEndTime, differenceStartDay, differenceEndDay));
            }

            return differenceExposures;
        }

        private static Tuple<List<AgUnformattedExposure>, AgUnformattedExposure> ProcessOverlappedExposure(IEnumerable<AgUnformattedExposure> exposures, AgUnformattedExposure differenceExposure)
        {
            var overlappedExposure = exposures.FirstOrDefault(e => differenceExposure.StartDate <= e.EndDate &&
                differenceExposure.EndDate >= e.StartDate &&
                differenceExposure.StartDay <= e.EndDay &&
                differenceExposure.EndDay >= e.StartDay &&
                CheckFirstTimeHigherOrEqual(e.EndTime, differenceExposure.StartTime) &&
                CheckFirstTimeHigherOrEqual(differenceExposure.EndTime, e.StartTime));

            if (overlappedExposure is null)
            {
                return null;
            }

            var processedExposures = new List<AgUnformattedExposure>();

            var isOverlappedBeforeDifferenceDate = overlappedExposure.StartDate < differenceExposure.StartDate &&
                overlappedExposure.EndDate >= differenceExposure.StartDate;

            if (isOverlappedBeforeDifferenceDate)
            {
                processedExposures.Add(CreateExposure(overlappedExposure.BreakSalesAreaNo, overlappedExposure.ClashCode,
                    overlappedExposure.MasterClashCode, overlappedExposure.NoOfExposures,
                    overlappedExposure.StartDate, differenceExposure.StartDate.AddDays(-1), overlappedExposure.StartTime,
                    overlappedExposure.EndTime, overlappedExposure.StartDay, overlappedExposure.EndDay));
            }

            var isOverlappedAfterDifferenceDate = overlappedExposure.EndDate > differenceExposure.EndDate &&
                overlappedExposure.StartDate <= differenceExposure.EndDate;

            if (isOverlappedAfterDifferenceDate)
            {
                processedExposures.Add(CreateExposure(overlappedExposure.BreakSalesAreaNo, overlappedExposure.ClashCode,
                    overlappedExposure.MasterClashCode, overlappedExposure.NoOfExposures,
                    differenceExposure.EndDate.AddDays(1), overlappedExposure.EndDate, overlappedExposure.StartTime,
                    overlappedExposure.EndTime, overlappedExposure.StartDay, overlappedExposure.EndDay));
            }

            var isOverlappedBeforeDifferenceDay = overlappedExposure.StartDay < differenceExposure.StartDay &&
                overlappedExposure.EndDay >= differenceExposure.StartDay;

            if (isOverlappedBeforeDifferenceDay)
            {
                processedExposures.Add(CreateExposure(overlappedExposure.BreakSalesAreaNo, overlappedExposure.ClashCode,
                    overlappedExposure.MasterClashCode, overlappedExposure.NoOfExposures,
                    differenceExposure.StartDate, differenceExposure.EndDate, overlappedExposure.StartTime,
                    overlappedExposure.EndTime, overlappedExposure.StartDay, differenceExposure.StartDay - 1));
            }

            var isOverlappedAfterDifferenceDay = overlappedExposure.EndDay > differenceExposure.EndDay &&
                overlappedExposure.StartDay <= differenceExposure.EndDay;

            if (isOverlappedAfterDifferenceDay)
            {
                processedExposures.Add(CreateExposure(overlappedExposure.BreakSalesAreaNo, overlappedExposure.ClashCode,
                    overlappedExposure.MasterClashCode, overlappedExposure.NoOfExposures,
                    differenceExposure.StartDate, differenceExposure.EndDate, overlappedExposure.StartTime,
                    overlappedExposure.EndTime, differenceExposure.EndDay + 1, overlappedExposure.EndDay));
            }

            var isOverlappedBeforeDifferenceTime =
                CheckFirstTimeHigher(differenceExposure.StartTime, overlappedExposure.StartTime) &&
                CheckFirstTimeHigherOrEqual(overlappedExposure.EndTime, differenceExposure.StartTime);

            if (isOverlappedBeforeDifferenceTime)
            {
                processedExposures.Add(CreateExposure(overlappedExposure.BreakSalesAreaNo, overlappedExposure.ClashCode,
                    overlappedExposure.MasterClashCode, overlappedExposure.NoOfExposures,
                    differenceExposure.StartDate, differenceExposure.EndDate, overlappedExposure.StartTime,
                    differenceExposure.StartTime.Add(TimeSpan.FromSeconds(-1)), differenceExposure.StartDay, differenceExposure.EndDay));
            }

            var isOverlappedAfterDifferenceTime =
                CheckFirstTimeHigher(overlappedExposure.EndTime, differenceExposure.EndTime) &&
                CheckFirstTimeHigherOrEqual(differenceExposure.EndTime, overlappedExposure.StartTime);

            if (isOverlappedAfterDifferenceTime)
            {
                processedExposures.Add(CreateExposure(overlappedExposure.BreakSalesAreaNo, overlappedExposure.ClashCode,
                    overlappedExposure.MasterClashCode, overlappedExposure.NoOfExposures,
                    differenceExposure.StartDate, differenceExposure.EndDate, differenceExposure.EndTime.Add(TimeSpan.FromSeconds(1)),
                    overlappedExposure.EndTime, differenceExposure.StartDay, differenceExposure.EndDay));
            }

            processedExposures.Add(differenceExposure);

            return new Tuple<List<AgUnformattedExposure>, AgUnformattedExposure>(processedExposures, overlappedExposure);
        }

        private static AgUnformattedExposure CreateExposure(int breakSalesAreaNo, string clashCode, string masterClashCode,
            int noOfExposures, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, int startDay,
            int endDay)
        {
            return new AgUnformattedExposure
            {
                BreakSalesAreaNo = breakSalesAreaNo,
                ClashCode = clashCode,
                MasterClashCode = masterClashCode,
                NoOfExposures = noOfExposures,
                StartDate = startDate,
                EndDate = endDate,
                StartTime = startTime,
                EndTime = endTime,
                StartDay = startDay,
                EndDay = endDay
            };
        }

        private static List<AgUnformattedExposure> CreateExposures(int breakSalesAreaNo, string clashCode, string masterClashCode,
            int peakExposureCount, int offPeakExposureCount, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, int startDay,
            int endDay)
        {
            var exposures = new List<AgUnformattedExposure>();

            var timeBreaks = new List<TimeSpan>();

            var dayEndTime = new TimeSpan(5, 59, 59);

            if (endTime < dayEndTime)
            {
                endTime = endTime.Add(TimeSpan.FromHours(24));
            }
            else if (endTime == dayEndTime)
            {
                endTime = _defaultDayEndTime;
            }

            timeBreaks.AddRange(_breakTimes);
            timeBreaks.Add(startTime);
            timeBreaks.Add(endTime);

            timeBreaks = timeBreaks.Distinct().OrderBy(b => b).ToList();

            foreach (var breakRange in GetTimeRanges(timeBreaks))
            {
                var breakStartTime = breakRange[0];
                var breakEndTime = breakRange[1];

                if (breakStartTime >= endTime || breakEndTime <= startTime)
                {
                    continue;
                }

                if (breakEndTime != _defaultDayEndTime)
                {
                    breakEndTime = breakEndTime.Add(TimeSpan.FromSeconds(-1));
                }

                var exposureCount = IsPeak(breakStartTime, breakEndTime) ? peakExposureCount : offPeakExposureCount;

                exposures.Add(CreateExposure(breakSalesAreaNo, clashCode, masterClashCode, exposureCount,
                    startDate, endDate, breakStartTime, breakEndTime, startDay, endDay));
            }

            return exposures;
        }

        private static bool IsPeak(TimeSpan breakStartTime, TimeSpan breakEndTime)
        {
            return breakStartTime >= _peakStartTime && breakEndTime <= _peakEndTime;
        }

        private static IEnumerable<AgExposure> ConvertToAgExposures(IEnumerable<AgUnformattedExposure> unformattedExposures)
        {
            var formattedExposures = new List<AgExposure>();

            foreach (var exposure in unformattedExposures)
            {
                var formattedExposure = new AgExposure
                {
                    BreakSalesAreaNo = exposure.BreakSalesAreaNo,
                    ClashCode = exposure.ClashCode,
                    MasterClashCode = exposure.MasterClashCode,
                    NoOfExposures = exposure.NoOfExposures,
                    StartDate = ConvertToAgDate(exposure.StartDate),
                    EndDate = ConvertToAgDate(exposure.EndDate),
                    StartTime = ConvertToAgTime(exposure.StartTime),
                    EndTime = ConvertToAgTime(exposure.EndTime),
                    StartDay = exposure.StartDay,
                    EndDay = exposure.EndDay
                };

                formattedExposures.Add(formattedExposure);
            }

            return formattedExposures;
        }

        private static string ConvertToAgTime(TimeSpan time)
        {
            const string agStartTime = "0";
            const string agEndTime = "995959";

            if (time == _defaultDayStartTime)
            {
                return agStartTime;
            }

            if (time == _defaultDayEndTime)
            {
                return agEndTime;
            }

            if (time < _defaultDayEndTime)
            {
                var twentyFourHours = TimeSpan.FromHours(24);
                time = time.Add(twentyFourHours);
            }

            return time.ToString("hhmmss");
        }

        private static string ConvertToAgDate(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        private static IEnumerable<int[]> GetDowPeriods(string dow)
        {
            const char includedDay = '1';
            const int emptyDayNumber = -1;

            var startDayNumber = emptyDayNumber;

            for (var i = 0; i < dow.Length; i++)
            {
                int dayNumber = i + 1;

                if (startDayNumber == emptyDayNumber && dow[i] == includedDay)
                {
                    startDayNumber = dayNumber;
                }

                if (startDayNumber != emptyDayNumber
                    && (dow[i] != includedDay || i == dow.Length - 1))
                {
                    var endDayNumber = dow[i] != includedDay ? i : dayNumber;
                    yield return new[] { startDayNumber, endDayNumber };
                    startDayNumber = emptyDayNumber;
                }
            }
        }

        private static IEnumerable<TimeSpan[]> GetTimeRanges(List<TimeSpan> times)
        {
            var emptyTime = new TimeSpan(-1);

            var startTime = emptyTime;

            for (var i = 0; i < times.Count; i++)
            {
                if (startTime == emptyTime)
                {
                    startTime = times[i];
                }
                else
                {
                    var endTime = times[i];
                    yield return new[] { startTime, endTime };
                    startTime = endTime;
                }
            }
        }

        private static bool CheckFirstTimeHigher(TimeSpan firstTime, TimeSpan secondTime)
        {
            if (firstTime < _defaultDayStartTime && (firstTime > secondTime || secondTime >= _defaultDayStartTime))
            {
                return true;
            }

            if (firstTime > secondTime && secondTime >= _defaultDayStartTime)
            {
                return true;
            }

            return false;
        }

        private static bool CheckFirstTimeHigherOrEqual(TimeSpan firstTime, TimeSpan secondTime)
        {
            return (firstTime == secondTime)
                ? true
                : CheckFirstTimeHigher(firstTime, secondTime);
        }
    }
}
