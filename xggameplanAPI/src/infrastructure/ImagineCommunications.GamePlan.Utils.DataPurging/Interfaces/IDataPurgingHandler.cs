using System.Threading;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Interfaces
{
    /// <summary>Exposes the functionality to execute purging task</summary>
    public interface IDataPurgingHandler
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
