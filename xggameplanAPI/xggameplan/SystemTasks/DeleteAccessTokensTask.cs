using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using xggameplan.AuditEvents;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Task for deleting old access tokens
    /// </summary>
    internal class DeleteAccessTokensTask : ISystemTask
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAccessTokensRepository _accessTokensRepository;

        public DeleteAccessTokensTask(IAuditEventRepository auditEventRepository, IAccessTokensRepository accessTokensRepository)
        {
            _auditEventRepository = auditEventRepository;
            _accessTokensRepository = accessTokensRepository;
        }

        public List<SystemTaskResult> Execute()
        {
            List<SystemTaskResult> results = new List<SystemTaskResult>();

            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Deleting access tokens"));
                var accessTokens = _accessTokensRepository.RemoveAllExpired();
                _accessTokensRepository.SaveChanges();
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Deleted {accessTokens.Count()} access tokens"));
            }
            catch (Exception exception)
            {
                results.Add(new SystemTaskResult(SystemTaskResult.ResultTypes.Error, Id, $"Error deleting access tokens: {exception.Message}"));
            }
            return results;
        }

        public string Id => "DeleteAccessTokensTask";
    }
}
