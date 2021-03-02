using System;
using ImagineCommunications.GamePlan.Utils.DataPurging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using xggameplan.common.Types;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Infrastructure
{
    /// <summary>
    /// Exposes a functionality to resolve <see cref="IDataPurgingHandler"/> instance.
    /// </summary>
    public class DataPurgingHandlerProvider<THandler> : IDataPurgingHandlerProvider
        where THandler : class, IDataPurgingHandler
    {
        public DataPurgingHandlerProvider(string entityName)
        {
            EntityName = entityName;
        }

        public string EntityName { get; }
        public Priority Priority { get; set; } = Priority.Lowest;

        public IDataPurgingHandler Get(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<THandler>();
    }
}
