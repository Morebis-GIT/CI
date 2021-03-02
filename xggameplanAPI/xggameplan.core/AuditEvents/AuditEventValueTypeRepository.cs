using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Audit;

namespace xggameplan.AuditEvents
{
    public class AuditEventValueTypeRepository : IAuditEventValueTypeRepository
    {
        private static readonly List<AuditEventValueType> auditEventValueTypes =
            new List<AuditEventValueType> {
                new AuditEventValueType(AuditEventValueTypes.ClientIPAddress, "Client IP Address", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.Message, "Message", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.UserEmailAddress, "User email address", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.RecipientEmailAddress, "Recipient email address", typeof(string), true),
                new AuditEventValueType(AuditEventValueTypes.Exception, "Exception", typeof(ExceptionModel), false),
                new AuditEventValueType(AuditEventValueTypes.AccessToken, "Access token", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.RequestUserAgent, "User agent", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.LoginFailureReason, "Login failure reason", typeof(string), false),

                new AuditEventValueType(AuditEventValueTypes.GamePlanRunID, "Run ID", typeof(Guid), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanScenarioID, "Scenario ID", typeof(Guid), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanAutoBookID, "AutoBook ID", typeof(Guid), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanAutoBookMessage, "AutoBook message", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanPipelineEventID, "Pipeline Event ID", typeof(int), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanPipelineEventErrorMessage, "Pipeline event error message", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanSalesAreaName, "Sales area name", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanAutoBookEventID, "AutoBook Event ID", typeof(int), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanAutoBookLog, "AutoBook log", typeof(string), false),
                new AuditEventValueType(AuditEventValueTypes.GamePlanSystemState, "System state", typeof(SystemState), false)
            };

        public void Insert(List<AuditEventValueType> items) => throw new NotImplementedException();

        public List<AuditEventValueType> GetAll() => auditEventValueTypes;

        public void Update(AuditEventValueType auditEventValueType) => throw new NotImplementedException();

        public AuditEventValueType GetByID(int id) => GetAll().Find(auditEventValueType => auditEventValueType.ID == id);

        public void DeleteByID(int id) => throw new NotImplementedException();

        public void DeleteAll() => throw new NotImplementedException();
    }
}
