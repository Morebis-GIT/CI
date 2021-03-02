using System;
using System.Globalization;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.core.Services
{
    public class SystemLogicalDateService : ISystemLogicalDateService
    {
        private const string SystemLogicalDateFormat = "MMddyyyy";

        private DateTime _systemLogicalDate;
        private bool _isSystemLogicalDateUsed = false;
        private readonly IClock _clock;

        public SystemLogicalDateService(ITenantSettingsRepository tenantSettingsRepository, IFeatureManager featureManager, IClock clock, IAuditEventRepository auditEventRepository)
        {
            _clock = clock;

            var tenantSettings = tenantSettingsRepository.Get();

            var useSystemLogicalDate = featureManager.IsEnabled(nameof(ProductFeature.UseSystemLogicalDate));

            if(useSystemLogicalDate)
            {
                var isDateValid = DateTime.TryParseExact(tenantSettings.SystemLogicalDate, SystemLogicalDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var systemLogicalDate);

                if (isDateValid)
                {
                    _isSystemLogicalDateUsed = true;
                    _systemLogicalDate = systemLogicalDate.Add(_clock.GetCurrentInstant().ToDateTimeUtc().TimeOfDay);
                }
                else
                {
                    auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"System Logical Date is not it valid format. " +
                        $"Date set: '{tenantSettings.SystemLogicalDate}', but valid format is: '{SystemLogicalDateFormat}'. Using current Date instead."));
                }
            }
        }

        public DateTime GetSystemLogicalDate()
        {
            return _isSystemLogicalDateUsed ? _systemLogicalDate : _clock.GetCurrentInstant().ToDateTimeUtc();
        }
    }
}
