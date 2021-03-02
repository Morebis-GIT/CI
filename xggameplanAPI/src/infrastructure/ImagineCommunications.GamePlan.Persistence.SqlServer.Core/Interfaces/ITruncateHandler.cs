using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ITruncateHandler
    {
        Task TruncateAsync(IEntityType entityType, DeleteFromOptions options = DeleteFromOptions.None,
            CancellationToken cancellationToken = default);
    }
}
