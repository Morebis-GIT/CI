using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes
{
    /// <summary>
    /// Extends domain <see cref="Clash"/> model functionality.
    /// </summary>
    public static class ClashExtensions
    {
        /// <summary>
        /// Indexes list by ExternalRef
        /// </summary>
        /// <param name="clashes"></param>
        /// <returns></returns>
        public static IImmutableDictionary<string, Clash> IndexListByExternalRef(this IEnumerable<Clash> clashes)
        {
            if (clashes is null)
            {
                throw new ArgumentNullException(nameof(clashes));
            }

            var clashesByExternalRef = new Dictionary<string, Clash>();

            foreach (var clash in clashes)
            {
                if (!clashesByExternalRef.ContainsKey(clash.Externalref))
                {
                    clashesByExternalRef.Add(clash.Externalref, clash);
                }
            }

            return clashesByExternalRef.ToImmutableDictionary();
        }
    }
}
