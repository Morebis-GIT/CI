using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Represent a grouping of breaks by their containers. Each container
    /// can be retrieved by a container reference, and each container holds
    /// references to the breaks within that container.
    /// </summary>
    /// <seealso cref="Dictionary{ContainerReference, List{Break}}" />
    public sealed class BreakContainers
        : Dictionary<ContainerReference, List<Break>>
    {
        private BreakContainers()
        { }

        /// <summary>Adds a new container with the specified container reference.</summary>
        /// <param name="containerReference">The new container reference.</param>
        /// <param name="item">The break to add to the container.</param>
        public void Add(
            in ContainerReference containerReference,
            Break item)
        {
            var newContainer = new List<Break> { item };
            Add(containerReference, newContainer);
        }

        /// <summary>
        /// Groups the breaks by container. All of the breaks must be in containers.
        /// If any break is found to not be in a container an empty grouping is
        /// returned.
        /// </summary>
        /// <param name="breaks">The breaks to group by container.</param>
        public static BreakContainers GroupBreaks(
            IEnumerable<Break> breaks
            )
        {
            if (!breaks.Any())
            {
                return new BreakContainers();
            }

            var buckets = new BreakContainers();

            foreach (var item in breaks)
            {
                if (!ContainerReference.TryParse(item.ExternalBreakRef, out var containerReference))
                {
                    return new BreakContainers();
                }

                if (buckets.TryGetValue(containerReference, out var container))
                {
                    container.Add(item);
                }
                else
                {
                    buckets.Add(containerReference, item);
                }
            }

            return buckets;
        }
    }
}
