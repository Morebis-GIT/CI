using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class ProgrammeRestriction : ICloneable
    {
        public List<string> SalesAreas { get; set; }

        public List<string> CategoryOrProgramme { get; set; }

        public object Clone()
        {
            ProgrammeRestriction programmeRestriction = (ProgrammeRestriction)MemberwiseClone();

            if (SalesAreas != null)
            {
                programmeRestriction.SalesAreas = new List<string>();
                SalesAreas.ForEach(sa => programmeRestriction.SalesAreas.Add((string)sa.Clone()));
            }
            if (CategoryOrProgramme != null)
            {
                programmeRestriction.CategoryOrProgramme = new List<string>();
                CategoryOrProgramme.ForEach(sa => programmeRestriction.CategoryOrProgramme.Add((string)sa.Clone()));
            }

            return programmeRestriction;
        }

        private string _isCategoryOrProgramme;
        public string IsCategoryOrProgramme
        {
            get => string.IsNullOrWhiteSpace(_isCategoryOrProgramme) ? _isCategoryOrProgramme : _isCategoryOrProgramme.ToUpper();
            set => _isCategoryOrProgramme = value;
        }

        private string _isIncludeOrExclude;
        public string IsIncludeOrExclude
        {
            get => string.IsNullOrWhiteSpace(_isIncludeOrExclude) ? _isIncludeOrExclude : _isIncludeOrExclude.ToUpper();
            set => _isIncludeOrExclude = value;
        }

        public void RequiredFieldValidation(string isIncludeOrExclude, string categoryOrProgramme)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                  //  new ValidationInfo() { FieldName = "Programme Restriction Sale Area Names", FieldToValidate = salesAreas},
                    new ValidationInfo() { FieldName = "Programme Restrictions Category Or Programme", FieldToValidate = categoryOrProgramme},
                    new ValidationInfo() { FieldName = "Programme Restrictions Is Include Or Exclude", FieldToValidate = isIncludeOrExclude}
                }
            };
            validation.Execute();

        }

        public void RegexValidation(string isIncludeOrExclude, string categoryOrProgramme)
        {
            const string isCorP = "^(C|P)$";
            const string isIorE = "^(I|E)$";
            IValidation validation = new RegexValidation()
            {
                Field = new List<ValidationInfo>()
                {
             new ValidationInfo() { ErrorMessage = "Invalid Category Or Programme(C/P)", FieldToValidate = categoryOrProgramme, RegexPattern = isCorP},
             new ValidationInfo() { ErrorMessage = "Invalid Is Include Or Exclude(I/E)", FieldToValidate = isIncludeOrExclude, RegexPattern = isIorE}

                }
            };
            validation.Execute();
        }
    }
}
