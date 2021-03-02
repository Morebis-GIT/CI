using System;
using System.Collections.Generic;
using xggameplan.AuditEvents.ValueConverter;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Defines converter to use for audit event values
    /// </summary>
    public class AuditEventValueConverter
    {
        public List<int> _valueTypeIds = new List<int>();               // Value types applicable to (AuditEventValueType.ID)
        public IValueConverter ValueConverter { get; }                  // Value converter to use

        public AuditEventValueConverter(List<int> valueTypeIds, IValueConverter valueConverter)
        {
            if (valueTypeIds.Count == 0)
            {
                throw new ArgumentException("No value types were specified", nameof(valueTypeIds));
            }
            _valueTypeIds = valueTypeIds;
            ValueConverter = valueConverter;
        }

        /// <summary>
        /// Returns whether our value converter can convert values of this type
        /// </summary>
        /// <param name="valueTypeId"></param>
        /// <returns></returns>
        public bool Handles(int valueTypeId)
        {
            return _valueTypeIds.Contains(valueTypeId);
        }
    }
}
