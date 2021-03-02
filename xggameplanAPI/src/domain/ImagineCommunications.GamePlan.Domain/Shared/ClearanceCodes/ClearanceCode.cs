using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes
{
    public class ClearanceCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public static void Validate(string code, string description)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "Clearance Code", FieldToValidate = code},
                    new ValidationInfo() {FieldName = "Clearance Description", FieldToValidate = description},
                }
            };
            validation.Execute();
        }
    }
}
