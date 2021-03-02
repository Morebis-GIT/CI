using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Generic.Validation
{
    /// <summary>
    /// Required field validation
    /// Eg: If Property X is Required field in ClassA ,then need to load and execute the RequiredFieldValidation-class property and method
    /// Field = ( FieldName ="Property Name X" ,FieldToValidate =X)
    /// RequiredFieldValidationClassObject.Execute();
    /// </summary>
    public class RequiredFieldValidation : IValidation
    {
        public List<ValidationInfo> Field { get; set; }

        public bool Execute(ValidationInfo info)
        {
            var value = info.FieldToValidate;

            if (info.IsNullable)
            {
                return !info.HasValue;
            }

            switch (value)
            {
                case string v:
                    return string.IsNullOrWhiteSpace(v);

                case List<string> list:
                    return list.Count == 0 || list.Any(string.IsNullOrWhiteSpace);

                case double v:
                    return Double.NaN.CompareTo(v) == 0;

                case null:
                    return true;

                default:
                    return IsDefault(value);
            }
        }

        private bool IsDefault<T>(T value) where T : new()
        {
            if (typeof(T).IsValueType)
            {
                return value.Equals(default(T));
            }

            if (!typeof(T).IsValueType && typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                return ((IList)value).Count == 0;
            }

            return false;
        }

        public void Execute()
        {
            foreach (var o in Field)
            {
                if (o.FieldToValidate is null || Execute(o))
                {
                    throw new ArgumentNullException(o.FieldName);
                }
            }
        }
    }
}
