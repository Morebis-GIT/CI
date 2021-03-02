using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    public class PassSalesAreaPriority
    {
        private const string ZeroOrOne = "^(?!0{7})[0-1]{7}$";

        public List<SalesAreaPriority> SalesAreaPriorities = new List<SalesAreaPriority>();

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public bool IsPeakTime { get; set; }
        public bool IsOffPeakTime { get; set; }
        public bool IsMidnightTime { get; set; }
        public bool AreDatesRetained { get; set; }
        public bool AreTimesRetained { get; set; }

        public string DaysOfWeek { get; set; } //0 or 1 for each day of week

        internal void Validate(IEnumerable<SalesArea> validSalesAreas)
        {
            ValidateDates();

            ValidateTimes();

            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "PassSalesAreaPriority DaysOfWeek", FieldToValidate = DaysOfWeek},
                    new ValidationInfo() {FieldName = "PassSalesAreaPriority SalesAreaPriorities", FieldToValidate = SalesAreaPriorities},
                }
            };
            validation.Execute();

            validation = new RegexValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Invalid PassSalesAreaPriority DaysOfWeek",
                        FieldToValidate = DaysOfWeek,
                        RegexPattern = ZeroOrOne
                    }
                }
            };
            validation.Execute();

            var allSalesAreaPriorities = SalesAreaPriorities.Select(x => x.SalesArea);
            var duplicateSalesAreaPriorities = allSalesAreaPriorities.GroupBy(salesArea => salesArea).Where(g => g.Count() > 1).Select(g => g.Key);
            if (duplicateSalesAreaPriorities.Count() > 0)
            {
                throw new Exception("Duplicate SalesAreas: " + string.Join(",", duplicateSalesAreaPriorities.ToArray()));
            }

            if (validSalesAreas != null)
            {
                SalesAreaPriorities.ForEach(salesAreaPriority =>
                {
                    if (!validSalesAreas.Any(validSalesArea => string.Equals(validSalesArea.Name, salesAreaPriority.SalesArea, StringComparison.CurrentCulture)))
                    {
                        throw new Exception(salesAreaPriority.SalesArea + " is not a valid SalesArea");
                    }
                });
            }
        }

        private void ValidateDates()
        {
            // If the pass is supposed to retain its own dates, then it must have both start and end dates specified
            if (AreDatesRetained && (!StartDate.HasValue || !EndDate.HasValue))
            {
                throw new ArgumentException("Retain dates flag set to true. Please set Pass Start and End Dates");
            }

            // Any pass should either have specified values or null values for both dates
            if ((StartDate.HasValue && !EndDate.HasValue)
                || (!StartDate.HasValue && EndDate.HasValue))
            {
                throw new ArgumentException("PassSalesAreaPriority StartDate/EndDate are not valid");
            }

            // Any comparison involving null DateTime results in false
            // So in case of null StartDate/EndDate this check will be skipped, as required
            if (StartDate > EndDate)
            {
                throw new ArgumentException("PassSalesAreaPriority StartDate is greater than EndDate");
            }
        }

        private void ValidateTimes()
        {
            // If the pass is supposed to retain its own times, then it must have both start and end times specified
            if (AreTimesRetained && (!StartTime.HasValue || !EndTime.HasValue))
            {
                throw new ArgumentException("Retain times flag set to true. Please set Pass Start and End Times");
            }

            // Any pass should either have specified values or null values for both times
            if ((StartTime.HasValue && !EndTime.HasValue)
                || (!StartTime.HasValue && EndTime.HasValue))
            {
                throw new ArgumentException("PassSalesAreaPriority StartTime/EndTime are not valid");
            }

            if (!StartTime.HasValue && !EndTime.HasValue)
            {
                return;
            }

            // XGGT-17121: Request to allow midnight-to-midnight runs and passes
            var passStartTime = StartTime.Value;
            var passEndTime = EndTime.Value;
            if (passStartTime != new TimeSpan(0, 0, 0)
                && passEndTime != new TimeSpan(23, 59, 59))
            {
                passStartTime = TimeHelper.ConvertToBroadcast(StartTime.Value);
                passEndTime = TimeHelper.ConvertToBroadcast(EndTime.Value);
            }

            if (passStartTime > passEndTime)
            {
                throw new ArgumentException("PassSalesAreaPriority StartTime is greater than EndTime");
            }
        }
    }
}
