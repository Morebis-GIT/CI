using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class DayPart : ICampaignKpiData, ICloneable
    {
        public string DayPartName { get; set; } = "NotSupplied";
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public List<Timeslice> Timeslices { get; set; }
        public List<DayPartLength> Lengths { get; set; }
        public int SpotMaxRatings { get; set; }
        public decimal CampaignPrice { get; set; }
        public int TotalSpotCount { get; set; }
        public int ZeroRatedSpotCount { get; set; }
        public double Ratings { get; set; }
        public double BaseDemographRatings { get; set; }
        public double NominalValue { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }

        public void Validation()
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Time Slices",FieldToValidate = Timeslices }
                }
            };
            validation.Execute();
            if (Timeslices != null && Timeslices.Any())
            {
                Timeslices.ForEach(t => t.RequiredFieldValidation(t.FromTime, t.ToTime, t.DowPattern));
                Timeslices.ForEach(t => t.RegexValidation(t.FromTime, t.ToTime, t.DowPattern));
            }

            validation = new CompareValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Day Part Spot Max Ratings should be greater than or equal to 0",
                        FieldToValidate = SpotMaxRatings,
                        FieldToCompare = 0,
                        Operator = Operator.GreaterThanEqual
                    },
                    new ValidationInfo()
                    {
                        ErrorMessage = "Day Part Campaign Price should be greater than or equal to 0",
                        FieldToValidate = CampaignPrice,
                        FieldToCompare = 0,
                        Operator = Operator.GreaterThanEqual
                    }
                }
            };
            validation.Execute();
        }

        public object Clone()
        {
            DayPart dayPart = (DayPart)MemberwiseClone();

            if (Timeslices != null)
            {
                dayPart.Timeslices = new List<Timeslice>();
                Timeslices.ForEach(ts => dayPart.Timeslices.Add((Timeslice)ts.Clone()));
            }

            return dayPart;
        }
    }
}
