using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using xggameplan.core.FeatureManagement.Exceptions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.FeatureManagement.Internal;

namespace xggameplan.core.FeatureManagement
{
    /// <inheritdoc/>
    public sealed class FeatureManager : IFeatureManager
    {
        private static readonly ConcurrentDictionary<int, FeatureFlagCollection> Cache =
            new ConcurrentDictionary<int, FeatureFlagCollection>();

        private readonly Lazy<FeatureFlagCollection> _factory;

        private Dictionary<string, IFeatureFlag> _featureDictionary;

        public FeatureManager(IEnumerable<IFeatureSetting> featureSettings)
        {
            var data = featureSettings.ToArray();
            _factory = new Lazy<FeatureFlagCollection>(() => InitializeFeatures(data));
        }

        public FeatureManager(TenantIdentifier tenant, IFeatureSettingsProvider dataProvider)
        {
            if (tenant is null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            if (dataProvider is null)
            {
                throw new ArgumentNullException(nameof(dataProvider));
            }

            _factory = new Lazy<FeatureFlagCollection>(() =>
                Cache.GetOrAdd(tenant.Id, (tenantId) => InitializeFeatures(dataProvider.GetForTenant(tenantId))));
        }

        /// <inheritdoc/>
        public bool IsEnabled(string featureName)
        {
            if (featureName is null)
            {
                throw new ArgumentNullException(nameof(featureName));
            }

            return FeatureDictionary.TryGetValue(featureName, out var featureFlag) && featureFlag.IsEnabled;
        }

        public void ClearCache() => Cache.Clear();

        public IReadOnlyCollection<IFeatureFlag> Features => _factory.Value;

        private IReadOnlyDictionary<string, IFeatureFlag> FeatureDictionary =>
            _featureDictionary ?? (_featureDictionary = Features.ToDictionary(k => k.Name));

        private FeatureFlagCollection InitializeFeatures(IEnumerable<IFeatureSetting> data)
        {
            var collection = new FeatureFlagCollection(data.Select(x => new FeatureFlag(x)));
            var collectionCount = collection.Select(x => x.Name).Distinct().Count();

            if (collectionCount != collection.Count)
            {
                throw new FeatureInitializeException($"Feature flags contain name duplicates.");
            }

            return collection;
        }
    }
}
