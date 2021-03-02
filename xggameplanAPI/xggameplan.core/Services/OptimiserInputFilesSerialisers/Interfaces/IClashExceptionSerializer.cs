using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces
{
    /// <summary>
    /// Serializes clash exceptions into xml file.
    /// </summary>
    public interface IClashExceptionSerializer
    {
        /// <summary>Gets the filename.</summary>
        /// <value>The filename.</value>
        string Filename { get; }

        /// <summary>Serializes clash exceptions into the specified folder name.</summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="run">The run.</param>
        /// <param name="clashes">The clashes.</param>
        /// <param name="filteredProducts">List of products filtered by included campaign list.</param>
        void Serialize(string folderName, Run run, IReadOnlyCollection<Clash> clashes, IReadOnlyCollection<Product> filteredProducts);
    }
}
