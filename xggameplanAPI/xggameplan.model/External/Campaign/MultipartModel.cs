using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using NodaTime;

namespace xggameplan.model.External.Campaign
{
    public class MultipartModel
    {
        public int MultipartNumber { get; set; }
        public List<Duration> Lengths { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Validation(List<Duration> lengths)
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
