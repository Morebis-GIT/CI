using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using xggameplan.AuditEvents;
using xggameplan.common.BackgroundJobs;
using xggameplan.AutoBooks.Abstractions;

namespace xggameplan.Jobs
{
    public class AutoBookCreateBackgroundJob : IBackgroundJob
    {
        private readonly IAutoBooks _autoBooks;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAutoBookRepository _autoBookRepository;
        private readonly IEnumerable<ITenantDbContext> _tenantDbContexts;

        public AutoBookCreateBackgroundJob(
            IAutoBooks autoBooks,
            IAuditEventRepository auditEventRepository,
            IAutoBookRepository autoBookRepository,
            IEnumerable<ITenantDbContext> tenantDbContexts)
        {
            _autoBooks = autoBooks;
            _auditEventRepository = auditEventRepository;
            _autoBookRepository = autoBookRepository;
            _tenantDbContexts = tenantDbContexts;
        }

        /// <summary>
        /// It is called dynamically.
        /// </summary>
        public async Task Execute(CancellationToken cancellationToken, AutoBook autoBook)
        {
            try
            {
                try
                {
                    _autoBooks.Create(autoBook);
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        $"Creating AutoBook instance (AutoBookID={autoBook.Id}): Waiting for Idle notification"));
                }
                catch (Exception ex)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        $"Error creating AutoBook (AutoBookID={autoBook.Id})", ex));
                    autoBook.Status = AutoBookStatuses.Fatal_Error;
                    _autoBookRepository.Update(autoBook);
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
