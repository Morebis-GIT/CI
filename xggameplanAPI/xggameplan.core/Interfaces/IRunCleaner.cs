using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xggameplan.core.Interfaces
{
    /// <summary>
    /// Exposes functionality to delete run(s).
    /// </summary>
    public interface IRunCleaner
    {
        Task ExecuteAsync(Guid runId, CancellationToken cancellationToken = default);

        Task ExecuteAsync(IReadOnlyCollection<Guid> runIds, CancellationToken cancellationToken = default);
    }
}
