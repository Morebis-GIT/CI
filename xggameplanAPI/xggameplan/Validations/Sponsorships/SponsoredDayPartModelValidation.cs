using System;
using System.Collections.Generic;
using System.Globalization;
using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class SponsoredDayPartModelValidation : AbstractValidator<SponsoredDayPartModelBase>
    {
        private readonly TimeSpan DefaultBroadcastDayStartTime = new TimeSpan(6, 0, 0);
        private readonly TimeSpan DefaultBroadcastDayEndTime = new TimeSpan(5, 59, 59);
        private readonly List<string> AcceptableDaysOfTheWeek = new List<string>() { "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY", "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };

        public SponsoredDayPartModelValidation()
        {
            RuleFor(model => model).Must(ContainAnEndTimeOnOrAfterTheStartTime)
                                   .WithName(model => nameof(model.EndTime))
                                   .WithMessage("EndTime must be on or after the StartTime");

            RuleFor(model => model.DaysOfWeek)
                                  .Must(ContainValidDaysOfWeekValue)
                                  .WithMessage($"A valid {nameof(SponsoredDayPartModelBase.DaysOfWeek)} is required");
        }

        private bool ContainValidDaysOfWeekValue(string[] daysOfWeek)
        {
            if (!ContainAnyDaysOfWeekValue(daysOfWeek))
            {
                return false;
            }
            if (daysOfWeek.Length > 7)
            {
                return false;
            }
            var checkList = new List<string>();
            foreach (var item in daysOfWeek)
            {
                var day = item.ToUpper(CultureInfo.InvariantCulture);
                if (AcceptableDaysOfTheWeek.Contains(day))
                {
                    var index = AcceptableDaysOfTheWeek.IndexOf(day);
                    day = AcceptableDaysOfTheWeek[index < 7 ? index : index - 7];
                    if (!checkList.Contains(day))
                    {
                        checkList.Add(day);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool ContainAnyDaysOfWeekValue(string[] daysOfWeek)
        {
            return daysOfWeek != null && daysOfWeek.Length > 0;
        }

        private bool ContainAnEndTimeOnOrAfterTheStartTime(SponsoredDayPartModelBase model)
        {
            TimeSpan startTimeInBroadcastDayTime = ConvertTimeIntoBroadcastDayTime(model.StartTime);
            TimeSpan endTimeInBroadcastDayTime = ConvertTimeIntoBroadcastDayTime(model.EndTime.Value);

            return endTimeInBroadcastDayTime.CompareTo(startTimeInBroadcastDayTime) >= 0;
        }

        private int GetDaysToAddBasedOnBroadcastTime(TimeSpan usingTime)
        {
            return IsTimePastMidnightAndOnOrPriorToBroadcastDayEndTime(usingTime) ? 1 : 0;
        }

        private bool IsTimePastMidnightAndOnOrPriorToBroadcastDayEndTime(TimeSpan theTimeToCheck)
        {
            return theTimeToCheck.CompareTo(DefaultBroadcastDayEndTime) <= 0;
        }

        private TimeSpan ConvertTimeIntoBroadcastDayTime(TimeSpan usingTime)
        {
            return new TimeSpan(GetDaysToAddBasedOnBroadcastTime(usingTime), usingTime.Hours, usingTime.Minutes, usingTime.Seconds);
        }
    }
}
