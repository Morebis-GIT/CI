using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using NodaTime;
using xggameplan.model;

namespace xggameplan.Model
{
    public class CreateBreak
        : ISelfValidate
    {
        public DateTime ScheduledDate { get; set; }
        public DateTime? BroadcastDate { get; set; }
        public int? ClockHour { get; set; }
        public string SalesArea { get; set; }
        public string BreakType { get; set; }
        public Duration Duration { get; set; }
        public bool Optimize { get; set; }
        public string ExternalBreakRef { get; set; }
        public string Description { get; set; }
        public string ExternalProgRef { get; set; }

        /// <summary>
        /// Validates this instance follows business rules for the
        /// object to be created. If the validation fails an exception will be
        /// thrown.
        /// </summary>
        public void Validate()
        {
            ValidateRequiredFields();
            ValidateClockHour();
            ValidateBreakType();
        }

        private void ValidateBreakType()
        {
            if (String.IsNullOrWhiteSpace(BreakType))
            {
                var guruMeditation = new BreakTypeNullException();
                guruMeditation.Data.Add(nameof(ExternalBreakRef), ExternalBreakRef);

                throw guruMeditation;
            }

            if (BreakType.Length < BreakConstants.MinimumBreakTypeLength)
            {
                var guruMeditation = new BreakTypeTooShortException();
                guruMeditation.Data.Add(nameof(ExternalBreakRef), ExternalBreakRef);

                throw guruMeditation;
            }

            // Check that the first two characters are would be long enough.
            // Optimiser uses just the first two character so they must be valid
            // in isolation.
            var prefix = BreakType.Substring(0, 2).Trim();
            if (prefix.Length < BreakConstants.MinimumBreakTypeLength)
            {
                var guruMeditation = new BreakTypePrefixTooShortException();
                guruMeditation.Data.Add(nameof(ExternalBreakRef), ExternalBreakRef);

                throw guruMeditation;
            }
        }

        private void ValidateClockHour()
        {
            if (ClockHour is null)
            {
                return;
            }

            IValidation rangeValidation = new RangeValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo {
                        FieldName = "ClockHour",
                        FieldToValidate = ClockHour.Value,
                        MinimumValue = 0,
                        MaximumValue = 99
                    }
                }
            };

            rangeValidation.Execute();
        }

        private void ValidateRequiredFields()
        {
            IValidation requiredFieldValidation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Scheduled Date", FieldToValidate = ScheduledDate },
                    new ValidationInfo() { FieldName = "Sales Area", FieldToValidate = SalesArea },
                    new ValidationInfo() { FieldName = "Duration", FieldToValidate = Duration },
                    new ValidationInfo() { FieldName = "External Break Reference", FieldToValidate = ExternalBreakRef }
                }
            };

            requiredFieldValidation.Execute();
        }
    }
}
