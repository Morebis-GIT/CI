using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignProgramme : ICloneable
    {
        public IEnumerable<string> CategoryOrProgramme { get; set; }
        public IEnumerable<string> SalesAreas { get; set; }

        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<Timeslice> Timeband { get; set; }

        private string _isCategoryOrProgramme;
        public string IsCategoryOrProgramme
        {
            get => string.IsNullOrWhiteSpace(_isCategoryOrProgramme) ? _isCategoryOrProgramme : _isCategoryOrProgramme.ToUpperInvariant();
            set => _isCategoryOrProgramme = value;
        }

        public object Clone()
        {
            var model = (CampaignProgramme)MemberwiseClone();

            model.SalesAreas = SalesAreas?.ToList();
            model.CategoryOrProgramme = CategoryOrProgramme?.ToList();
            model.Timeband = Timeband?.Select(x => (Timeslice)x.Clone()).ToList();

            return model;
        }

        public void RequiredFieldValidation(string categoryOrProgramme)
        {
            IValidation validation = new RequiredFieldValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        FieldName = "Campaign Programme Category Or Programme",
                        FieldToValidate = categoryOrProgramme
                    }
                }
            };
            validation.Execute();

        }

        public void RegexValidation(string categoryOrProgramme)
        {
            const string isCorP = "^(C|P)$";
            IValidation validation = new RegexValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        ErrorMessage = "Invalid Category Or Programme(C/P)",
                        FieldToValidate = categoryOrProgramme, RegexPattern = isCorP
                    }
                }
            };
            validation.Execute();
        }
    }
}
