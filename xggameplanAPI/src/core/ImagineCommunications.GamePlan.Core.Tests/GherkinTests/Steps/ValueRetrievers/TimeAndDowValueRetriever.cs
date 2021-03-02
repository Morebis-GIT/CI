using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Core.Tests.GherkinTests
{
    public class TimeAndDowValueRetriever
        : IValueRetriever
    {
        public bool CanRetrieve(
            KeyValuePair<string, string> keyValuePair,
            Type targetType,
            Type propertyType
            ) =>
            propertyType == typeof(TimeAndDowAPI);

        public object Retrieve(
            KeyValuePair<string, string> keyValuePair,
            Type targetType,
            Type propertyType
            )
        {
            string value = keyValuePair.Value;

            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var result = new TimeAndDowAPI();

            /*
             * The table column for a TimeAndDow field should contain either nothing or these
             * optional comma separated fields. Only one of each field will be considered.
             *
             * startTime=HH:MM:SS
             * endTime=HH:MM:SS
             * daysOfWeek=XXXXXXX
             *
             * For example,
             *      startTime=HH:MM:SS
             *      startTime=HH:MM:SS, endTime=HH:MM:SS
             *      startTime=HH:MM:SS, endTime=HH:MM:SS, daysOfWeek=XXXXXXX
             *      daysOfWeek=XXXXXXX
             *
             */

            var timeAndDowPartSeparator = new[] { "," };
            var fieldSeparator = new[] { '=' };

            var timeAndDowParts = value.Split(timeAndDowPartSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in timeAndDowParts)
            {
                var cleanPart = part.Trim().ToUpperInvariant();

                var field = cleanPart.Split(fieldSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (field.Length != 2)
                {
                    continue;
                }

                switch (field[0].Trim())
                {
                    case "STARTTIME":
                        {
                            string timeValue = field[1]?.Trim() ?? String.Empty;
                            result.StartTime = TimeSpan.TryParse(timeValue, out TimeSpan time)
                                ? time
                                : (TimeSpan?)null;

                            break;
                        }

                    case "ENDTIME":
                        {
                            string timeValue = field[1]?.Trim() ?? String.Empty;
                            result.EndTime = TimeSpan.TryParse(timeValue, out TimeSpan time)
                                ? time
                                : (TimeSpan?)null;

                            break;
                        }

                    case "DAYSOFWEEK":
                        result.SetDaysOfWeek(field[1]?.Trim());
                        break;

                    default:
                        continue;
                }
            }

            return result;
        }
    }
}
