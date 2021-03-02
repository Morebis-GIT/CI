using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges
{
    public class Timeslice : ICloneable
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public List<string> DowPattern { get; set; }

        public object Clone()
        {
            Timeslice timeSlice = (Timeslice)MemberwiseClone();

            if (DowPattern != null)
            {
                timeSlice.DowPattern = new List<string>();
                DowPattern.ForEach(dp => timeSlice.DowPattern.Add((string)dp.Clone()));
            }

            return timeSlice;
        }

        public void RequiredFieldValidation(string fromTime, string toTime, List<string> dowPattern)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Time Slice From Time",FieldToValidate =  fromTime},
                    new ValidationInfo() { FieldName = "Time Slice To Time", FieldToValidate = toTime},
                    new ValidationInfo() { FieldName = "Dow Pattern", FieldToValidate = dowPattern}
                }
            };
            validation.Execute();
        }

        public void RegexValidation(string fromTime, string toTime, List<string> dowPattern)
        {
            const string hhmmFormat = "^([0-9]|0[0-9]|1?[0-9]|2[0-3]):[0-5][0-9]$";
            const string dayOfWeek = "^\\b(?i)(Sun|Mon|Tue|Wed|Thu|Fri|Sat)\\b$";
            IValidation validation = new RegexValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { ErrorMessage = "Invalid Time Slice From Time",FieldToValidate = fromTime,RegexPattern = hhmmFormat},
                    new ValidationInfo() { ErrorMessage = "Invalid Time Slice To Time", FieldToValidate = ToTime,RegexPattern = hhmmFormat},
                    new ValidationInfo() { ErrorMessage = "Invalid Dow Pattern", FieldToValidate = dowPattern, RegexPattern = dayOfWeek}
                }
            };
            validation.Execute();
        }
    }
}
