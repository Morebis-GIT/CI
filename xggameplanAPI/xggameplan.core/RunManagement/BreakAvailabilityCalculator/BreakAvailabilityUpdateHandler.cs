using System;
using System.Collections.Generic;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using Microsoft.Extensions.Logging;
using NodaTime;
using xggameplan.common;
using static xggameplan.common.Helpers.LogAsString;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public class BreakAvailabilityUpdateHandler : IBreakAvailabilityUpdateHandler<Break>
    {
        private readonly IBreakRepository _breakRepository;
        private readonly ILogger _logger;
        private readonly ScheduleUtilities _scheduleUtilities;

        public bool BreaksHaveChanges { get; private set; }

        public bool ScheduleBreaksHaveChanges { get; private set; }

        public BreakAvailabilityUpdateHandler(ILogger logger, IBreakRepository breakRepository, Schedule schedule)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _breakRepository = breakRepository ?? throw new ArgumentNullException(nameof(breakRepository));
            _scheduleUtilities = CreateScheduleUtilities(schedule);
        }

        public void UpdateAvailability(Break theBreak)
        {
            _breakRepository.Add(theBreak);
            BreaksHaveChanges = true;

            if (_scheduleUtilities is null)
            {
                return;
            }

            var breakExternalReference = theBreak.ExternalBreakRef;
            var (schedule, message) = _scheduleUtilities.FindScheduleForBreak(theBreak);
            if (schedule is null)
            {
                _logger.LogWarning(LogPrologue(breakExternalReference) + message);
                return;
            }

            var breakForSchedule = schedule.GetBreak(breakExternalReference);
            if (breakForSchedule is null)
            {
                _logger.LogWarning(LogPrologue(breakExternalReference) + "No schedule break");
                return;
            }

            if (UpdateScheduleBreakAvailIfChanged(breakForSchedule, theBreak.Avail))
            {
                ScheduleBreaksHaveChanges = true;
            }
        }

        public void UpdateOptimizerAvailability(Break theBreak)
        {
            _breakRepository.Add(theBreak);
            BreaksHaveChanges = true;

            if (_scheduleUtilities is null)
            {
                return;
            }

            var breakExternalReference = theBreak.ExternalBreakRef;

            var (schedule, message) = _scheduleUtilities.FindScheduleForBreak(theBreak);
            if (schedule is null)
            {
                _logger.LogWarning(LogPrologue(breakExternalReference) + message);
                return;
            }

            var breakForSchedule = schedule.GetBreak(breakExternalReference);
            if (breakForSchedule is null)
            {
                _logger.LogWarning(LogPrologue(breakExternalReference) + "No schedule break");
                return;
            }

            if (UpdateScheduleBreakOptimiserAvailIfChanged(breakForSchedule, theBreak.OptimizerAvail))
            {
                ScheduleBreaksHaveChanges = true;
            }
        }

        private ScheduleUtilities CreateScheduleUtilities(Schedule schedule)
        {
            if (schedule is null)
            {
                return null;
            }

            return new ScheduleUtilities(new List<ScheduleIndexed<Break, string>>
            {
                new ScheduleIndexed<Break, string>(schedule,
                    currentBreak => currentBreak,
                    currentBreak => currentBreak.ExternalBreakRef
                )
            });
        }

        private bool UpdateScheduleBreakAvailIfChanged(
            Break breakForSchedule,
            Duration breakAvailability)
        {
            var result = false;
            var infoMessage = new StringBuilder(128);
            infoMessage
                .Append(LogPrologue(breakForSchedule.ExternalBreakRef))
                .Append("Schedule Avail: ").Append(Log(breakForSchedule.Avail)).Append("s; ")
                .Append("Schedule OptimiserAvail: ").Append(Log(breakForSchedule.OptimizerAvail))
                .Append('s');

            if (breakForSchedule.HasAvailabilityChanged(breakAvailability))
            {
                breakForSchedule.Avail = breakAvailability;
                breakForSchedule.OptimizerAvail = breakAvailability;

                infoMessage
                    .Append("; Both now ")
                    .Append(Log(breakAvailability))
                    .Append('s');
                result = true;
            }
            _logger.LogInformation(infoMessage.ToString());

            return result;
        }

        private bool UpdateScheduleBreakOptimiserAvailIfChanged(
            Break breakForSchedule,
            Duration breakOptimiserAvailability)
        {
            var result = false;
            var infoMessage = new StringBuilder(128);
            infoMessage
                .Append(LogPrologue(breakForSchedule.ExternalBreakRef))
                .Append("Schedule Avail: ").Append(Log(breakForSchedule.Avail)).Append("s; ")
                .Append("Schedule OptimiserAvail: ").Append(Log(breakForSchedule.OptimizerAvail))
                .Append('s');

            if (breakForSchedule.HasAvailabilityChanged(breakOptimiserAvailability))
            {
                breakForSchedule.OptimizerAvail = breakOptimiserAvailability;

                infoMessage
                    .Append("; Schedule OptimiserAvail now ")
                    .Append(Log(breakOptimiserAvailability))
                    .Append('s');
                result = true;
            }
            _logger.LogInformation(infoMessage.ToString());

            return result;
        }

        private static string LogPrologue(string breakExternalReference) =>
            $"[Schedule Break Ext. Ref. {breakExternalReference}] ";
    }
}
