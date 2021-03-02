using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImagineCommunications.GamePlan.Domain.Generic.Validation
{
    /// <summary>
    /// Automated Validation for(Mandatory /regex /compare/ ..etc)
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// It consist of property details(property name ,error msg to display, field to validate,regex pattern etc)
        /// </summary>
        List<ValidationInfo> Field { get; set; }

        /// <summary>
        /// check the value and return the validation result
        /// </summary>
        /// <param name="info">Information for validation</param>
        /// <returns>Validation result</returns>
        bool Execute(ValidationInfo info);

        /// <summary>
        /// Iterate the each property in field and apply the validation
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// Empty/blank Field Validation
    /// Eg: If Property X is blank/empty field in ClassA ,then need to load and execute the EmptyFieldValidation-class property and method
    /// Field = ( ErrorMessage ="Property Name X with error msg" ,FieldToValidate =X)
    /// EmptyFieldValidationClassObject.Execute();
    /// </summary>
    public class EmptyFieldValidation : IValidation
    {
        public List<ValidationInfo> Field { get; set; }

        public bool Execute(ValidationInfo info)
        {
            var value = info.FieldToValidate;
            if (value is string)
            {
                return string.IsNullOrWhiteSpace(value as string);
            }
            if (value is List<string>)
            {
                var list = (value as List<string>);
                return !list.Any();
            }
            return value == null || IsDefault(value);
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
                if (o.FieldToValidate != null && !Execute(o))
                {
                    throw new InvalidDataException(o.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// Regex validation
    /// Eg: If Property X must match a regex condition in ClassA ,then need to load and execute the RegexValidation-class property and method
    /// Field = (ErrorMessage ="Error Message" ,FieldToValidate =X.value,RegexPattern ="Pattern in string")
    /// RequiredFieldValidationClassObject.Execute();
    /// </summary>
    public class RegexValidation : IValidation
    {
        public List<ValidationInfo> Field { get; set; }

        public bool Execute(ValidationInfo info)
        {
            if (info.FieldToValidate is string)
            {
                var inputValue = (string)info.FieldToValidate;
                if (string.IsNullOrWhiteSpace(inputValue))
                {
                    throw new ArgumentNullException(null, info.ErrorMessage);
                }
                return !Regex.IsMatch(inputValue, info.RegexPattern);
            }

            if (info.FieldToValidate is List<string>) //check for List<string>
            {
                var inputValue = (List<string>)info.FieldToValidate;
                if (inputValue == null || !inputValue.Any() || inputValue.Any(string.IsNullOrWhiteSpace))
                {
                    throw new ArgumentNullException(null, info.ErrorMessage);
                }
                return !inputValue.All(t => Regex.IsMatch(t, info.RegexPattern));
            }
            return !Regex.IsMatch(info.FieldToValidate, info.RegexPattern);
        }

        public void Execute()
        {
            foreach (var o in Field)
            {
                if (o.FieldToValidate == null && string.IsNullOrWhiteSpace(o.RegexPattern))
                {
                    throw new ArgumentNullException(null, o.ErrorMessage);
                }
                if (Execute(o))
                {
                    throw new RegexMatchTimeoutException(o.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// Range validation
    /// Eg: If Property X must match a Range[ie. it should less than 10, greater than 5] condition in ClassA ,then need to load and execute the RangeValidation-class property and method
    /// Field = (ErrorMessage="Error Message" , FieldToValidate =X.value,MinimumValue= 5,MaximumValue = 10)
    /// RangeValidationClassObject.Execute();
    /// </summary>
    public class RangeValidation : IValidation
    {
        public List<ValidationInfo> Field { get; set; }

        public bool Execute(ValidationInfo value)
        {
            return !(value.FieldToValidate >= value.MinimumValue && value.FieldToValidate <= value.MaximumValue);
        }

        public void Execute()
        {
            foreach (var o in Field)
            {
                if (o.FieldToValidate == null || o.MinimumValue == null || o.MaximumValue == null)
                {
                    throw new ArgumentNullException(null, o.ErrorMessage);
                }
                if (Execute(o))
                {
                    o.ErrorMessage = $"{o.FieldName} : range must be between {o.MinimumValue} and {o.MaximumValue}";
                    throw new InvalidDataException(o.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// Compare validation
    /// Eg: If Property X must match a Y[ie. X should be less than Y] condition in ClassA ,then need to load and execute the CompareValidation-class property and method
    /// Field = (ErrorMessage="Error Message" , FieldToValidate =X.value, FieldToCompare= Y.Value,Operator = LessThan)
    /// CompareValidationClassObject.Execute();
    /// </summary>
    public class CompareValidation : IValidation
    {
        public List<ValidationInfo> Field { get; set; }

        public bool Execute(ValidationInfo value)
        {
            var isValid = false;
            switch (value.Operator)
            {
                case Operator.Equal:
                    isValid = value.FieldToValidate.Equal(value.FieldToCompare);
                    break;

                case Operator.GreaterThan:
                    isValid = value.FieldToValidate > value.FieldToCompare;
                    break;

                case Operator.GreaterThanEqual:
                    isValid = value.FieldToValidate >= value.FieldToCompare;
                    break;

                case Operator.LessThan:
                    isValid = value.FieldToValidate < value.FieldToCompare;
                    break;

                case Operator.LessThanEqual:
                    isValid = value.FieldToValidate <= value.FieldToCompare;
                    break;

                case Operator.NotEqual:
                    isValid = !value.FieldToValidate.Equal(value.FieldToCompare);
                    break;

                case Operator.DataTypeCheck:
                    isValid = value.FieldToValidate.GetType().Equals(value.FieldToCompare.GetType());
                    break;
            }
            return !isValid;
        }

        public void Execute()
        {
            foreach (var o in Field)
            {
                if (o.FieldToValidate == null || o.FieldToCompare == null)
                {
                    throw new ArgumentNullException(null, o.ErrorMessage);
                }
                if (Execute(o))
                {
                    throw new InvalidDataException(o.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// Length validation
    /// Eg: If Property X length must match a yy[ie. X.length should be less than yy] condition in ClassA ,then need to load and execute the LengthValidation-class property and method
    /// Field = (ErrorMessage="Error Message" , FieldToValidate =X.value, LengthToCompare= yy,Operator = LessThan)
    /// LengthValidationClassObject.Execute();
    /// </summary>
    public class LengthValidation : IValidation
    {
        public List<ValidationInfo> Field { get; set; }

        public bool Execute(ValidationInfo value)
        {
            var isValid = false;
            switch (value.Operator)
            {
                case Operator.Equal:
                    isValid = value.FieldToValidate.Length == (value.LengthToCompare);
                    break;

                case Operator.GreaterThan:
                    isValid = value.FieldToValidate.Length > value.LengthToCompare;
                    break;

                case Operator.GreaterThanEqual:
                    isValid = value.FieldToValidate.Length >= value.LengthToCompare;
                    break;

                case Operator.LessThan:
                    isValid = value.FieldToValidate.Length < value.LengthToCompare;
                    break;

                case Operator.LessThanEqual:
                    isValid = value.FieldToValidate.Length <= value.LengthToCompare;
                    break;

                case Operator.NotEqual:
                    isValid = !value.FieldToValidate.length.Equal(value.LengthToCompare);
                    break;

                case Operator.DataTypeCheck:
                    throw new NotImplementedException("DataTypeCheck Operator has no Implemention");
            }
            return !isValid;
        }

        public void Execute()
        {
            foreach (var o in Field)
            {
                if (o.FieldToValidate == null || o.LengthToCompare == 0)
                {
                    throw new ArgumentNullException(null, o.ErrorMessage);
                }
                if (Execute(o))
                {
                    throw new InvalidDataException(o.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// Information to validation
    /// </summary>
    public class ValidationInfo
    {
        /// <summary>
        /// Set to true if the value to validate is nullable
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// Set to true is the value to validate is nullable and has value
        /// </summary>
        public bool HasValue { get; set; }
        /// <summary>
        /// Property value to validate (This field is Required)
        /// </summary>
        public dynamic FieldToValidate { get; set; }
        /// <summary>
        /// Property Name (This field is required)
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Error message (This field is required)
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// RegexPattern (This field is required only for Regex Validation)
        /// </summary>
        public string RegexPattern { get; set; }
        /// <summary>
        /// Minimum value to compare (This field is required only for Range Validation)
        /// </summary>
        public dynamic MinimumValue { get; set; }
        /// <summary>
        /// Maximum value to compare (This field is required only for Range Validation)
        /// </summary>
        public dynamic MaximumValue { get; set; }
        /// <summary>
        /// Specifies the value used when performing the comparison. (This field is required only for Compare Validation)
        /// </summary>
        public dynamic FieldToCompare { get; set; }
        /// <summary>
        /// comparison operator to use when performing comparisons (This field is required only for Compare Validation)
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// Specifies the value when perfiorm length check (this field is required only for length validation)
        /// </summary>
        public int LengthToCompare { get; set; }
    }
}
