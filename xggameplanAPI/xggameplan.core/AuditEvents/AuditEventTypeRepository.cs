using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    public class AuditEventTypeRepository : IAuditEventTypeRepository
    {
        private static readonly List<AuditEventType> auditEventTypes =
        new List<AuditEventType>
        {
            new AuditEventType(AuditEventTypes.BackendMethod, "Backend method"),
            new AuditEventType(AuditEventTypes.InformationMessage, "Information message"),
            new AuditEventType(AuditEventTypes.WarningMessage, "Warning message"),
            new AuditEventType(AuditEventTypes.Exception, "Exception"),
            new AuditEventType(AuditEventTypes.SystemTestsSuccess, "System tests success"),
            new AuditEventType(AuditEventTypes.SystemTestsFailed, "System tests failed"),
            new AuditEventType(AuditEventTypes.UserLoginSuccess, "Login success"),
            new AuditEventType(AuditEventTypes.UserLoginFailed, "Login failed"),
            new AuditEventType(AuditEventTypes.TenantCreate, "Tenant create"),
            new AuditEventType(AuditEventTypes.UserCreate, "User create"),
            new AuditEventType(AuditEventTypes.UserLogout, "Logout"),

            new AuditEventType(AuditEventTypes.GamePlanAutoBookRun, "Autobook API (Run)"),
            new AuditEventType(AuditEventTypes.GamePlanRun, "Gameplan API (Run)"),
            new AuditEventType(AuditEventTypes.GamePlanSmoothRun, "Smooth API (Run)"),
            new AuditEventType(AuditEventTypes.GamePlanAutoBookEvent, "AutoBook event"),
            new AuditEventType(AuditEventTypes.GamePlanAutoBookLogs, "AutoBook logs"),
            new AuditEventType(AuditEventTypes.GamePlanRunValidationFailure, "Run validation failure"),
            new AuditEventType(AuditEventTypes.GamePlanScheduleDataUploadStarted, "Schedule data upload started"),
            new AuditEventType(AuditEventTypes.GamePlanRunStarted, "Run started"),
            new AuditEventType(AuditEventTypes.GamePlanRunStartFailed, "Run start failed"),
            new AuditEventType(AuditEventTypes.GamePlanRunCompleted, "Run completed"),
            new AuditEventType(AuditEventTypes.GamePlanRunCreated, "Run created"),
            new AuditEventType(AuditEventTypes.GamePlanSystemState, "System state"),
            new AuditEventType(AuditEventTypes.GamePlanRunDeleted, "Run deleted"),
        };

        public void Insert(List<AuditEventType> item) => throw new NotImplementedException();

        public List<AuditEventType> GetAll() => auditEventTypes;

        public void Update(AuditEventType item) => throw new NotImplementedException();

        public AuditEventType GetByID(int id) =>
            GetAll().Find(auditEventType => auditEventType.ID == id);

        public void DeleteByID(int id) => throw new NotImplementedException();

        public void DeleteAll() => throw new NotImplementedException();
    }
}
