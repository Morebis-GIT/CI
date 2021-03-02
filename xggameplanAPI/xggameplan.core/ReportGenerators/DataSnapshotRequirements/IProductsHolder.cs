using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface IProductsHolder
    {
        IEnumerable<Product> Products { get; }
    }
}
