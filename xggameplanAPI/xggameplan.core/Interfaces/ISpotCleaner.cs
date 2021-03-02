using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xggameplan.core.Interfaces
{
    public interface ISpotCleaner
    {
        /// <summary>
        /// Exposes functionality to delete Spots.
        /// </summary>
        Task ExecuteAsync(IReadOnlyCollection<string> externalRefs, Action<string> logProgress, DateTime? thresholdDate = default, CancellationToken cancellationToken = default);
    }
}
