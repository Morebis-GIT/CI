using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using xggameplan.core.FeatureManagement.Exceptions;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.core.FeatureManagement.Internal
{
    internal sealed class FeatureFlag : IFeatureFlag
    {
        private readonly int _id;
        private readonly bool _featureSettingEnabled;
        private readonly IReadOnlyCollection<int> _parentIds;

        private IDictionary<int, FeatureFlag> _collection;
        private IReadOnlyCollection<FeatureFlag> _dependsOn;
        private bool? _isEnabled;
        private bool _isEnabledChecking;

        internal FeatureFlag(IFeatureSetting featureSetting)
        {
            if (featureSetting is null)
            {
                throw new ArgumentNullException(nameof(featureSetting));
            }

            _id = featureSetting.Id;
            Name = AdjustName(featureSetting.Name) ?? $"NoName {_id}";
            IsShared = featureSetting.IsShared;
            _featureSettingEnabled = featureSetting.Enabled;
            _parentIds = featureSetting.ParentIds ?? Array.Empty<int>();
        }

        public string Name { get; }

        public bool IsShared { get; }

        public bool IsEnabled
        {
            get
            {
                if (!_isEnabled.HasValue && !(Collection is null))
                {
                    lock (this)
                    {
                        if (!_isEnabled.HasValue && !(Collection is null))
                        {
                            if (_isEnabledChecking)
                            {
                                throw new FeatureManagementException($"'{Name}' feature has recursive dependency.");
                            }

                            _isEnabledChecking = true;

                            try
                            {
                                _isEnabled = DependsOn.Aggregate(_featureSettingEnabled,
                                    (current, next) => current && next.IsEnabled);
                            }
                            finally
                            {
                                _isEnabledChecking = false;
                            }
                        }
                    }
                }

                return _isEnabled ?? _featureSettingEnabled;
            }
        }

        public IReadOnlyCollection<IFeatureFlag> DependsOn
        {
            get
            {
                if (_dependsOn is null && !(Collection is null))
                {
                    lock (this)
                    {
                        if (_dependsOn is null && !(Collection is null))
                        {
                            _dependsOn = _parentIds
                                .Where(x => x != _id)
                                .Select(x =>
                                {
                                    Collection.TryGetValue(x, out var ff);

                                    return ff;
                                })
                                .Where(x => !(x is null))
                                .ToArray();
                        }
                    }
                }

                return _dependsOn;
            }
        }

        internal IDictionary<int, FeatureFlag> Collection
        {
            get => _collection;
            set
            {
                if (value != _collection)
                {
                    lock (this)
                    {
                        _collection?.Remove(_id);
                        Reset();
                        _collection = value;
                        _collection?.Add(_id, this);
                    }
                }
            }
        }

        private static string AdjustName(string name) => name?.Trim();

        private void Reset()
        {
            _isEnabled = null;
            _dependsOn = null;
        }
    }
}
