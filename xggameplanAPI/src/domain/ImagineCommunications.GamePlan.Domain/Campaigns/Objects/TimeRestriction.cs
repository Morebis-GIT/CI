using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class TimeRestriction : ICloneable
    {
        public List<string> SalesAreas { get; set; }
        public List<string> DowPattern { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        private string _isIncludeOrExclude;

        public object Clone()
        {
            TimeRestriction timeRestriction = (TimeRestriction)MemberwiseClone();

            if (SalesAreas != null)
            {
                timeRestriction.SalesAreas = new List<string>();
                SalesAreas.ForEach(sa => timeRestriction.SalesAreas.Add((string)sa.Clone()));
            }
            if (DowPattern != null)
            {
                timeRestriction.DowPattern = new List<string>();
                DowPattern.ForEach(dp => timeRestriction.DowPattern.Add((string)dp.Clone()));
            }

            return timeRestriction;
        }

        public string IsIncludeOrExclude
        {
            get => string.IsNullOrWhiteSpace(_isIncludeOrExclude) ? _isIncludeOrExclude : _isIncludeOrExclude.ToUpper();
            set => _isIncludeOrExclude = value;
        }

        public void RequiredFieldValidation(DateTime startDate, DateTime endDate, List<string> dowPattern, string isIncludeOrExclude)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Time Restriction Start Date",FieldToValidate = startDate},
                    new ValidationInfo() { FieldName = "Time Restriction End Date", FieldToValidate = endDate},
                    new ValidationInfo() { FieldName = "Time Restrictions DOW Pattern", FieldToValidate = dowPattern},
                    new ValidationInfo() { FieldName = "Time Restrictions Is Include Or Exclude", FieldToValidate = IsIncludeOrExclude}
                }
            };
            validation.Execute();
        }

        public void RegexValidation(List<string> dowPattern, string isIncludeOrExclude)
        {
            const string dayOfWeek = "^\\b(?i)(Sun|Mon|Tue|Wed|Thu|Fri|Sat)\\b$";
            const string isIOrE = "^(I|E)$";
            IValidation validation = new RegexValidation()
            {
                Field = new List<ValidationInfo>()
                {
             new ValidationInfo() { ErrorMessage = "Invalid Dow Pattern", FieldToValidate = dowPattern, RegexPattern = dayOfWeek},
             new ValidationInfo() { ErrorMessage = "Invalid Is Include Or Exclude(I/E)", FieldToValidate = isIncludeOrExclude, RegexPattern = isIOrE}

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
                        ErrorMessage = "Time restriction start date should be less than end date",
                        FieldToValidate = startDate,
                        FieldToCompare =endDate,
                        Operator = Operator.LessThan
                    }
                }
            };
            validation.Execute();
        }
    }
}
