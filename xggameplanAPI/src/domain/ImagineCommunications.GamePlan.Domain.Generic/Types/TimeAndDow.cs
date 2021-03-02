using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    /// <summary>
    /// Represents a time and days of the week.
    /// </summary>
    public class TimeAndDow
    {
        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Days of the week that the <see cref="TimeAndDow"/> applies to, Monday to Sunday.
        /// 1 means applies and 0 means does not - this will always have 7 digits,
        /// for example ”1111111”.
        /// </summary>
        public string DaysOfWeek { get; set; }

        public void Validation(string daysOfWeek, bool isClashException = true)
        {
            var modelName = isClashException ? "Clash exception" : "Clash";
            IValidation validation = new RequiredFieldValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo {FieldName = $"{modelName} time and DOWs - days of week", FieldToValidate = daysOfWeek}
                }
            };
            validation.Execute();

            const string zeroOrOne = "^(?!0{7})[0-1]{7}$";
            validation = new RegexValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        ErrorMessage = $"{modelName} time and DOWs - Invalid days of week",
                        FieldToValidate = daysOfWeek,
                        RegexPattern = zeroOrOne
                    }
                }
            };
            validation.Execute();
        }

        /// <summary>
        /// Returns a value indicating whether a specified day of the week occurs within this
        /// <see cref="TimeAndDow"/> instance.
        /// </summary>
        /// <param name="dayOfWeek">A day of the week to search for.</param>
        /// <returns>Returns true if the given day of the week occurs within this instance,
        /// otherwise returns false.</returns>
        public bool Contains(DayOfWeek dayOfWeek)
        {
            // Someone decided Gameplan would use Monday to Sunday as 0 - 6.
            // .NET uses Sunday to Monday as 0 to 6.
            // Convert between the two.
            int shiftedDayOfWeekToMatchGamePlan = ((int)dayOfWeek - 1 + 7) % 7;

            return DaysOfWeek?[shiftedDayOfWeekToMatchGamePlan] == '1';
        }
    }
}
