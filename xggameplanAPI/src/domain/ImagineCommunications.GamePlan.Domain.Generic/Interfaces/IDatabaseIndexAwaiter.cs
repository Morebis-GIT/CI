using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    public interface IDatabaseIndexAwaiter
    {
        Task WaitForIndexAsync(string indexName);
        Task WaitForIndexesAsync();
    }
}
