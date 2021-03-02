using System;

namespace xggameplan.AuditEvents.ValueConverter
{
    public class ExceptionToStringConverter : IValueConverter
    {
        public bool CanConvert(Type fromType, Type toType)
        {
            return fromType == typeof(ExceptionModel) && toType == typeof(String);
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == toType)
            {
                return value;
            }
            ExceptionModel currentException = (ExceptionModel)value;
            return string.Format("Message: {0}; Source: {1}; Stack: {2}", currentException.Message, currentException.Source, currentException.StackTrace);
        }
    }
}
