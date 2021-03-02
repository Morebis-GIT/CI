using System;

namespace xggameplan.AuditEvents.ValueConverter
{
    /// <summary>
    /// Interface for converting between values
    /// </summary>
    public interface IValueConverter
    {
        bool CanConvert(Type fromType, Type toType);
        object Convert(object value, Type fromType, Type toType);
    }
}
