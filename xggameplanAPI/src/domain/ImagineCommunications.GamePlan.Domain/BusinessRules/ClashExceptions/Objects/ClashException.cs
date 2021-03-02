using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects
{
    public class ClashException
    {
        /// <summary>
        /// Raven Unique Id for the ClashException.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Start date of ClashException
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of ClashException
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// From Type of ClashException
        /// </summary>
        public ClashExceptionType FromType { get; set; }

        /// <summary>
        /// To Type of ClashException
        /// </summary>
        public ClashExceptionType ToType { get; set; }

        /// <summary>
        /// Include or Exclude
        /// </summary>
        public IncludeOrExclude IncludeOrExclude { get; set; }

        /// <summary>
        /// From Value of ClashException
        /// </summary>
        public string FromValue { get; set; }

        /// <summary>
        /// To Value of ClashException
        /// </summary>
        public string ToValue { get; set; }

        /// <summary>
        /// To Value of ClashException
        /// </summary>
        public List<TimeAndDow> TimeAndDows { get; set; }

        public string ExternalRef { get; set; }


        public static void Validation(DateTime startDate, DateTime? endDate, string fromValue, string toValue, List<TimeAndDow> timeAndDows)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Clash exception start date", FieldToValidate = startDate},
                    new ValidationInfo() { FieldName = "Clash exception from value", FieldToValidate = fromValue},
                    new ValidationInfo() { FieldName = "Clash exception to value", FieldToValidate = toValue},
                    new ValidationInfo() { FieldName = "Clash exception time and DOWs", FieldToValidate = timeAndDows}
                }
            };
            validation.Execute();

            if (timeAndDows.Any())
            {
                timeAndDows.ForEach(t => t.Validation(t.DaysOfWeek));
            }

            if (endDate == null)
            {
                validation = new CompareValidation()
                {
                    Field = new List<ValidationInfo>()
                    {
                        new ValidationInfo()
                        {
                            ErrorMessage = "Clash exception start date should not be a date in the past",
                            FieldToValidate = startDate,
                            FieldToCompare =DateTime.Today.Date,
                            Operator = Operator.GreaterThanEqual
                        }
                    }
                };
                validation.Execute();
            }
            else
            {
                validation = new CompareValidation()
                {
                    Field = new List<ValidationInfo>()
                    {
                        new ValidationInfo()
                        {
                            ErrorMessage = "Clash exception start date should not be a date in the past",
                            FieldToValidate = startDate,
                            FieldToCompare =DateTime.Today.Date,
                            Operator = Operator.GreaterThanEqual
                        },
                        new ValidationInfo()
                        {
                            ErrorMessage = "Clash exception start date should be less than or equal to end date",
                            FieldToValidate = startDate.Date,
                            FieldToCompare =endDate.Value.Date,
                            Operator = Operator.LessThanEqual
                        }
                    }
                };
                validation.Execute();
            }
        }

        public static void Validation(DateTime startDate, DateTime? endDate, List<TimeAndDow> timeAndDows)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Clash exception time and DOWs", FieldToValidate = timeAndDows}
                }
            };
            validation.Execute();

            if (timeAndDows.Any())
            {
                timeAndDows.ForEach(t => t.Validation(t.DaysOfWeek));
            }

            if (endDate != null)
            {
                validation = new CompareValidation()
                {
                    Field = new List<ValidationInfo>()
                    {
                        new ValidationInfo()
                        {
                            ErrorMessage = "Clash exception end date should not be a date in the past",
                            FieldToValidate = endDate.Value.Date,
                            FieldToCompare =DateTime.Today.Date,
                            Operator = Operator.GreaterThanEqual
                        },
                        new ValidationInfo()
                        {
                            ErrorMessage = "Clash exception end date should be greater than or equal to start date",
                            FieldToValidate = endDate.Value.Date,
                            FieldToCompare =startDate.Date,
                            Operator = Operator.GreaterThanEqual
                        }
                    }
                };
                validation.Execute();
            }
        }

        public override bool Equals(object clashException)
        {
            if (clashException is null)
            {
                return false;
            }

            if (ReferenceEquals(this, clashException))
            {
                return true;
            }

            if (clashException.GetType() != GetType())
            {
                return false;
            }

            return Equals((ClashException)clashException);
        }

        protected bool Equals(ClashException other)
        {
            return FromType == other.FromType
                   && ToType == other.ToType
                   && string.Equals(FromValue, other.FromValue, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(ToValue, other.ToValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ FromType.GetHashCode();
                hashCode = (hashCode * 397) ^ ToType.GetHashCode();
                hashCode = (hashCode * 397) ^ (FromValue != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(FromValue) : 0);
                hashCode = (hashCode * 397) ^ (ToValue != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(ToValue) : 0);
                hashCode = (hashCode * 397) ^ (TimeAndDows != null ? TimeAndDows.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ClashException left, ClashException right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClashException left, ClashException right)
        {
            return !Equals(left, right);
        }
    }
}
