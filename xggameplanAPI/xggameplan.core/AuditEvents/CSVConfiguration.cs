using System;
using System.Collections.Generic;
using System.Linq;
using xggameplan.AuditEvents.ValueConverter;

namespace xggameplan.AuditEvents
{
    public class CSVConfiguration : IAuditEventRepositoryCreator
    {
        private readonly string _folder;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private readonly ICSVAuditEventSettingsRepository _csvAuditEventSettingsRepository;

        public CSVConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository, ICSVAuditEventSettingsRepository csvAuditEventSettingsRepository,
                                string folder)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _csvAuditEventSettingsRepository = csvAuditEventSettingsRepository;
            _folder = folder;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new CSVAuditEventRepository(_folder, GetValueConverters(), _csvAuditEventSettingsRepository == null ? null : _csvAuditEventSettingsRepository.GetAll(), _auditEventValueTypeRepository);
        }

        private List<AuditEventValueConverter> GetValueConverters()
        {
            List<AuditEventValueConverter> valueConverters = new List<AuditEventValueConverter>();
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.Exception }, new ObjectToBase64BinaryConverter(new List<Type>() { typeof(ExceptionModel) }, new List<bool>() { false }, true)));
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.GamePlanPipelineEventErrorMessage, AuditEventValueTypes.GamePlanAutoBookMessage, AuditEventValueTypes.GamePlanAutoBookLog }, new StringToBase64Converter(true)));
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.GamePlanSystemState }, new ObjectToBase64JSONConverter(true)));

            // Use default converter (base type converter) for anything else that doesn't have a specific converter above
            List<AuditEventValueType> auditEventValueTypesForDefaultConverter = _auditEventValueTypeRepository.GetAll().Where(vt => valueConverters.FindIndex(vc => vc.Handles(vt.ID) == true) == -1).ToList();
            if (auditEventValueTypesForDefaultConverter.Count > 0)
            {
                valueConverters.Add(new AuditEventValueConverter(auditEventValueTypesForDefaultConverter.Select(x => x.ID).ToList(), new BaseTypeConverter()));
            }
            return valueConverters;
        }
    }
}
