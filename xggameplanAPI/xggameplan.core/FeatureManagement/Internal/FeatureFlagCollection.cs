using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.core.FeatureManagement.Internal
{
    internal sealed class FeatureFlagCollection : ICollection<FeatureFlag>, IReadOnlyCollection<FeatureFlag>
    {
        private readonly IDictionary<int, FeatureFlag> _storage = new Dictionary<int, FeatureFlag>();

        public FeatureFlagCollection()
        {
        }

        public FeatureFlagCollection(IEnumerable<FeatureFlag> featureFlags)
        {
            foreach (var featureFlag in featureFlags)
            {
                Add(featureFlag);
            }
        }

        public void Add(FeatureFlag item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Collection = _storage;
        }

        public void Clear()
        {
            foreach (var item in _storage.Values.ToArray())
            {
                item.Collection = null;
            }
        }

        public bool Contains(FeatureFlag item) => _storage.Values.Contains(item);

        public void CopyTo(FeatureFlag[] array, int arrayIndex) =>
            _storage.Values.CopyTo(array, arrayIndex);

        public bool Remove(FeatureFlag item)
        {
            if (!(item is null) && item.Collection == _storage)
            {
                item.Collection = null;
                return true;
            }

            return false;
        }

        public IEnumerator<FeatureFlag> GetEnumerator() => _storage.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _storage.Count;

        public bool IsReadOnly => _storage.IsReadOnly;
    }
}
