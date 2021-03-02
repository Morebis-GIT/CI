using System;
using System.Collections.Generic;
using xggameplan.AuditEvents.ValueConverter;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Configuration for CSV events
    /// </summary>
    internal class CSVConfiguration : IAuditEventRepositoryCreator
    {
        private readonly string _folder;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private readonly ICSVAuditEventSettingsRepository _csvAuditEventSettingsRepository;

        public CSVConfiguration(
            IAuditEventTypeRepository auditEventTypeRepository,
            IAuditEventValueTypeRepository auditEventValueTypeRepository,
            ICSVAuditEventSettingsRepository csvAuditEventSettingsRepository,
            string folder)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _csvAuditEventSettingsRepository = csvAuditEventSettingsRepository;
            _folder = folder;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new CSVAuditEventRepository(
                _folder,
                GetValueConverters(),
                _csvAuditEventSettingsRepository.GetAll(),
                _auditEventValueTypeRepository);
        }

        private List<AuditEventValueConverter> GetValueConverters()
        {
            List<AuditEventValueConverter> valueConverters = new List<AuditEventValueConverter>();

            // Serialize all value types to base 64 JSON
            foreach (var auditEventValueType in _auditEventValueTypeRepository.GetAll())
            {
                if (auditEventValueType.Type == typeof(String))    // Base 64 encode to enable storage in CSV
                {
                    valueConverters.Add(new AuditEventValueConverter(new List<int>() { auditEventValueType.ID }, new StringToBase64Converter(true)));
                }
                else if (Array.IndexOf(new List<Type>() { typeof(Boolean), typeof(Byte), typeof(Char), typeof(DateTime), typeof(DateTimeOffset),
                                                        typeof(double),typeof(float), typeof(Single),
                                                        typeof(Int16), typeof(Int32), typeof(Int64),
                                                        typeof(UInt16), typeof(UInt32), typeof(UInt64),
                                                        typeof(Guid) }.ToArray(), auditEventValueType) != -1)    // Simple types
                {
                    valueConverters.Add(new AuditEventValueConverter(new List<int>() { auditEventValueType.ID }, new BaseTypeConverter()));
                }
                else      // Serialized to JSON, Base 64 encode
                {
                    valueConverters.Add(new AuditEventValueConverter(new List<int>() { auditEventValueType.ID }, new ObjectToBase64JSONConverter(true)));
                }
            }
            return valueConverters;
        }
    }
}
