using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects
{
    public class Clash : ICloneable
    {
        private List<ClashDifference> _differences;

        public Guid Uid { get; set; }
        public string Externalref { get; set; }
        public string ParentExternalidentifier { get; set; }
        public string Description { get; set; }

        [Obsolete("Use DefaultOffPeakExposureCount. Delete this property once all references (except for update script) are gone.")]
        public int ExposureCount { get; set; }

        /// <summary>
        /// The default peak time exposure count. Use a clash exposure count calculator rather than
        /// this value directly.
        /// </summary>
        public int DefaultPeakExposureCount { get; set; }

        /// <summary>
        /// The default off-peak time exposure count. Use a clash exposure count calculator rather
        /// than this value directly.
        /// </summary>
        public int DefaultOffPeakExposureCount { get; set; }

        /// <summary>
        /// <para>Differences in exposure counts. This is a mandatory property and should never be
        /// null.</para>
        /// <para>Use a clash exposure count calculator rather than these values directly.</para>
        /// </summary>
        public List<ClashDifference> Differences
        {
            get
            {
                if (_differences is null)
                {
                    Differences = new List<ClashDifference>();
                }

                return _differences;
            }
            set => _differences = value;
        }

        public static void Validation(string externalRef, string description, int defaultPeakExposureCount, int defaultOffPeakExposureCount)
        {
            IValidation validation = new RequiredFieldValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo {FieldName = "Clash External Ref", FieldToValidate = externalRef},
                    new ValidationInfo {FieldName = "Clash Description", FieldToValidate = description}
                }
            };

            validation.Execute();

            validation = new LengthValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        ErrorMessage = $"Clash code can't be more than 6 characters for clash {externalRef}",
                        FieldToValidate = externalRef,
                        LengthToCompare = 6,
                        Operator = Operator.LessThanEqual
                    }
                }
            };

            validation.Execute();

            validation = new CompareValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        ErrorMessage = $"Default peak exposure count should at least be 1 for clash {externalRef}",
                        FieldToValidate = defaultPeakExposureCount,
                        FieldToCompare = 1,
                        Operator = Operator.GreaterThanEqual
                    }
                }
            };

            validation.Execute();

            validation = new CompareValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        ErrorMessage = $"Default non-peak exposure count should at least be 1 for clash {externalRef}",
                        FieldToValidate = defaultOffPeakExposureCount,
                        FieldToCompare = 1,
                        Operator = Operator.GreaterThanEqual
                    }
                }
            };

            validation.Execute();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString() => $"Clash External Reference: {Externalref}";
    }
}
