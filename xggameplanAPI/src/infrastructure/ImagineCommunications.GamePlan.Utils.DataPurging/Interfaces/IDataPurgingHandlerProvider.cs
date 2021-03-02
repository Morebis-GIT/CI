using System;
using xggameplan.common.Types;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Interfaces
{
    /// <summary>
    /// Exposes a functionality to resolve <see cref="IDataPurgingHandler"/> instance.
    /// </summary>
    public interface IDataPurgingHandlerProvider
    {
        /// <summary>Gets the name of the purging entity.</summary>
        string EntityName { get; }

        /// <summary>Gets the priority.</summary>
        Priority Priority { get; }

        /// <summary>Resolves the <see cref="IDataPurgingHandler"/> instance using the specified service provider.</summary>
        /// <param name="serviceProvider"></param>
        IDataPurgingHandler Get(IServiceProvider serviceProvider);
    }
}
