using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class SalesAreaGroup : ICloneable
    {
        public string GroupName { get; set; }
        public List<string> SalesAreas { get; set; }

        public object Clone()
        {
            SalesAreaGroup salesAreaGroup = (SalesAreaGroup)MemberwiseClone();

            salesAreaGroup.GroupName = GroupName;

            if (SalesAreas != null)
            {
                salesAreaGroup.SalesAreas = new List<string>();
                SalesAreas.ForEach(sa => salesAreaGroup.SalesAreas.Add((string)sa.Clone()));
            }

            return salesAreaGroup;
        }

        public void Validation(string groupName, List<string> salesAreas)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "SalesArea Group Name", FieldToValidate = groupName},
                    new ValidationInfo()
                    {
                        FieldName = "SalesArea name list",
                        FieldToValidate = salesAreas
                    }
                }
            };
            validation.Execute();
        }
    }
}
