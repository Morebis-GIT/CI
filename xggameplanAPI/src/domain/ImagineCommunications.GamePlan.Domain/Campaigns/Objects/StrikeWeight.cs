using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class StrikeWeight : ICloneable
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public List<Length> Lengths { get; set; }
        public List<DayPart> DayParts { get; set; }
        public int SpotMaxRatings { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }

        public object Clone()
        {
            StrikeWeight strikeWeight = (StrikeWeight)MemberwiseClone();

            if (Lengths != null)
            {
                strikeWeight.Lengths = new List<Length>();
                Lengths.ForEach(le => strikeWeight.Lengths.Add((Length)le.Clone()));
            }
            if (DayParts != null)
            {
                strikeWeight.DayParts = new List<DayPart>();
                DayParts.ForEach(dp => strikeWeight.DayParts.Add((DayPart)dp.Clone()));
            }

            return strikeWeight;
        }

        public void RequiredFieldValidation(DateTime startDate, DateTime endDate, List<Length> lengths, List<DayPart> dayParts, int spotMaxRatings)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Strike Weight Start Date",FieldToValidate = startDate},
                    new ValidationInfo() { FieldName = "Strike Weight End Date", FieldToValidate = endDate},
                    new ValidationInfo() { FieldName = "Strike Weight Lengths", FieldToValidate = lengths},
                    new ValidationInfo() { FieldName = "Day Parts", FieldToValidate = dayParts}
                }
            };
            validation.Execute();
            if (lengths != null && lengths.Any())
            {
                lengths.ForEach(l => l.Validation(l.length));
            }

            if (dayParts != null && dayParts.Any())
            {
                dayParts.ForEach(d => d.Validation());
            }

            validation = new CompareValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Strike Weight Spot Max Ratings should be greater than or equal to 0",
                        FieldToValidate = spotMaxRatings,
                        FieldToCompare = 0,
                        Operator = Operator.GreaterThanEqual
                    }
                }
            };
            validation.Execute();
        }

        public void RangeValidation(DateTime startDate, DateTime endDate, DateTime campaignStartDateTime,
            DateTime campaignEndDateTime)
        {
            IValidation validation = new RangeValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Strike weight start date must be within campaign start and end dates",
                        FieldToValidate = startDate,
                        MinimumValue = campaignStartDateTime,
                        MaximumValue = campaignEndDateTime
                    },
                    new ValidationInfo()
                    {
                        ErrorMessage = "Strike weight end date must be within campaign start and end dates",
                        FieldToValidate = endDate,
                        MinimumValue = campaignStartDateTime,
                        MaximumValue = campaignEndDateTime
                    }
                }
            };
            validation.Execute();
        }

        public void CompareValidation(DateTime startDate, DateTime endDate)
        {
            IValidation validation = new CompareValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Strike weight start date should be less than or equal to end date",
                        FieldToValidate = startDate,
                        FieldToCompare =endDate,
                        Operator = Operator.LessThanEqual
                    }
                }
            };
            validation.Execute();
        }
    }
}
