using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.RunCleaning
{
    internal delegate Task<IReadOnlyCollection<object>> RemovableEntitiesCollectHandler(ISqlServerTenantDbContext dbContext);

    /// <summary>
    /// Represents information about removed entity and its delegate
    /// to collect removing data.
    /// </summary>
    internal class RemovableEntitiesCollectHandlerInfo
    {
        public string EntityName { get; private set; }
        public RemovableEntitiesCollectHandler CollectFunc { get; private set; }

        public static RemovableEntitiesCollectHandlerInfo Create(string name, RemovableEntitiesCollectHandler func)
        {
            return new RemovableEntitiesCollectHandlerInfo { EntityName = name, CollectFunc = func };
        }
    }
}
