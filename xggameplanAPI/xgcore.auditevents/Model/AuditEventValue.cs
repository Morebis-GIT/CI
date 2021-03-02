using xggameplan.AuditEvents.ValueConverter;

namespace xggameplan.AuditEvents
{
    public class AuditEventValue
    {
        public AuditEventValue() { }

        public AuditEventValue(int typeId, object value) =>
            (TypeID, Value) = (typeId, value);

        /// <summary>
        /// Value Type ID.
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// Values.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets the audit event value from JSON string
        /// </summary>
        /// <param name="auditEvent"></param>
        public static object ConvertValue(int valueTypeId, object value, AuditEventValueType valueType)
        {
            if (value.GetType() != valueType.Type)    // Not correct type, convert
            {
                BaseTypeConverter baseTypeConverter = new BaseTypeConverter();
                if (baseTypeConverter.CanConvert(value.GetType(), valueType.Type))
                {
                    return baseTypeConverter.Convert(value, value.GetType(), valueType.Type);
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject(value.ToString(), valueType.Type);
            }
            return value;
        }
    }
}
