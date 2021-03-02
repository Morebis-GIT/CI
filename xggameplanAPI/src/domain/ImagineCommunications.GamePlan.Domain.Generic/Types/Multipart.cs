using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class Multipart : ICloneable
    {
        public int MultipartNumber { get; set; }
        public List<MultipartLength> Lengths { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Validation(List<MultipartLength> lengths)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "Multipart Lengths", FieldToValidate = lengths}
                }
            };
            validation.Execute();
        }
    }
}
