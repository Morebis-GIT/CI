using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using xggameplan.AuditEvents;
using xggameplan.common.BackgroundJobs;
using xggameplan.AutoBooks.Abstractions;

namespace xggameplan.Jobs
{
    public class AutoBookDeleteBackgroundJob : IBackgroundJob
    {
        private readonly IAutoBooks _autoBooks;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IEnumerable<ITenantDbContext> _tenantDbContexts;

        public AutoBookDeleteBackgroundJob(
            IAutoBooks autoBooks,
            IAuditEventRepository auditEventRepository,
            IEnumerable<ITenantDbContext> tenantDbContexts)
        {
            _autoBooks = autoBooks;
            _auditEventRepository = auditEventRepository;
            _tenantDbContexts = tenantDbContexts;
        }

        /// <summary>
        /// It is called dynamically.
        /// </summary>
        public async Task Execute(CancellationToken cancellationToken, AutoBook autoBook)
        {
            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Deleting AutoBook (AutoBookID={autoBook.Id})"));
                var autobookDeleted = false;
                try
                {
                    _autoBooks.Delete(autoBook);
                    autobookDeleted = true;
                }
                catch (Exception ex)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        $"Error deleting AutoBook (AutoBookID={autoBook.Id})", ex));
                }

                if (autobookDeleted)
                {
                    _auditEventRepository.Insert(
                        AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Deleted AutoBook"));
                }
            }
            finally
            {
                foreach (var dbContext in _tenantDbContexts)
                {
                    try
                    {
                        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch //ignore exception
                    {
                    }
                }
            }
        }
    }
}
