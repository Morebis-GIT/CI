using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Moq;
using NUnit.Framework;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.core.tests.FeatureManagement
{
    [TestFixture]
    public class FeatureManagerTests
    {
        private Mock<IFeatureSettingsProvider> _mockFeatureSettingsProvider;
        private IFeatureManager _featureManager;

        [SetUp]
        public void Init()
        {
            _mockFeatureSettingsProvider = new Mock<IFeatureSettingsProvider>();
            var tenantIdentifier = new TenantIdentifier(1, string.Empty);
            _featureManager = new FeatureManager(tenantIdentifier, _mockFeatureSettingsProvider.Object);
            _featureManager.ClearCache();
        }

        [Test, TestCaseSource(nameof(_testFeatureNames))]
        [Description("Given valid feature name when checking feature state then feature state must be returned")]
        public void IsEnabledWhenCalledWithValidFeatureNameThenShouldReturnFeatureState(FeatureStateTestData testData)
        {
            _ = _mockFeatureSettingsProvider.Setup(p => p.GetForTenant(It.IsAny<int>())).Returns(new[] { testData });

            _ = _featureManager.IsEnabled(testData.Name).Should().Be(testData.Enabled);
        }

        [Test, TestCaseSource(nameof(_testFeatureNames))]
        [Description("Given nonexistent feature name when checking feature state then disabled state should be returned")]
        public void IsEnabledWhenCalledWithNonExistentFeatureNameThenShouldReturnFalse(FeatureStateTestData testData)
        {
            _ = _mockFeatureSettingsProvider.Setup(p => p.GetForTenant(It.IsAny<int>())).Returns(new[]
            {
                new FeatureStateTestData { Name = nameof(FeatureName.Default) }
            });

            _ = _featureManager.IsEnabled(testData.Name).Should().Be(false);
        }

        [Test, TestCaseSource(nameof(_testFeatureNames))]
        [Description("Given valid feature name when getting features state then feature state must be returned")]
        public void GetFeaturesStateWhenCalledWithValidFeatureNameThenShouldReturnFeatureState(FeatureStateTestData testData)
        {
            _ = _mockFeatureSettingsProvider.Setup(p => p.GetForTenant(It.IsAny<int>())).Returns(new[] { testData });

            _ = _featureManager.Features.Count.Should().Be(1);

            var firstFeature = _featureManager.Features.First();
            _ = firstFeature.Name.Should().BeEquivalentTo(testData.Name);
            _ = firstFeature.IsEnabled.Should().Be(testData.Enabled);
            _ = firstFeature.IsShared.Should().Be(testData.IsShared);
        }

        [Test]
        [Description("Given feature with parent when checking feature state then correct feature state must be returned")]
        [TestCase(nameof(FeatureName.First), false, false)]
        [TestCase(nameof(FeatureName.Second), true, true)]
        [TestCase(nameof(FeatureName.Fifth), false, true)]
        [TestCase(nameof(FeatureName.Sixth), true, false)]
        public void IsEnabledWhenCalledWithChildFeatureThenShouldThrowException(string featureName, bool childEnabled, bool parentEnabled)
        {
            var feature = new FeatureStateTestData { Id = 2, Name = featureName, Enabled = childEnabled };
            var parentFeature = new FeatureStateTestData { Id = 1, Name = Guid.NewGuid().ToString("N"), Enabled = parentEnabled };

            feature.ParentIds = new List<int> { parentFeature.Id };

            _ = _mockFeatureSettingsProvider.Setup(p => p.GetForTenant(It.IsAny<int>()))
                .Returns(new[] { parentFeature, feature });

            _ = _featureManager.IsEnabled(featureName).Should().Be(childEnabled && parentEnabled);
        }

        [Test]
        public void DependsOnWhenCalledThenShouldReturnParentFeatureFlags()
        {
            var featureOne = new FeatureStateTestData { Id = 1, Name = nameof(FeatureName.First), Enabled = false };
            var featureTwo = new FeatureStateTestData
            {
                Id = 2,
                Name = nameof(FeatureName.Second),
                Enabled = true,
                ParentIds = new List<int> { 1 }
            };
            var featureThree = new FeatureStateTestData
            {
                Id = 3,
                Name = nameof(FeatureName.Third),
                Enabled = true,
                ParentIds = new List<int> { 1, 2 }
            };

            _ = _mockFeatureSettingsProvider.Setup(p => p.GetForTenant(It.IsAny<int>()))
                .Returns(new[] { featureOne, featureTwo, featureThree });

            _ = _featureManager.Features.Count.Should().Be(3);
            var featureFlagOne =
                _ = _featureManager.Features.FirstOrDefault(x => x.Name.Equals(nameof(FeatureName.First)));
            _ = featureFlagOne.Should().NotBeNull();
            _ = featureFlagOne.DependsOn.Should().BeEmpty();
            var featureFlagTwo =
                _ = _featureManager.Features.FirstOrDefault(x => x.Name.Equals(nameof(FeatureName.Second)));
            _ = featureFlagTwo.Should().NotBeNull();
            _ = featureFlagTwo.DependsOn.Should().ContainSingle(ff => ff.Name.Equals(nameof(FeatureName.First)));
            var featureFlagThree =
                _ = _featureManager.Features.FirstOrDefault(x => x.Name.Equals(nameof(FeatureName.Third)));
            _ = featureFlagThree.Should().NotBeNull();
            _ = featureFlagThree.DependsOn.Should().HaveCount(2);
            _ = featureFlagThree.DependsOn.Should().Contain(ff =>
                  ff.Name.Equals(nameof(FeatureName.First)) || ff.Name.Equals(nameof(FeatureName.Second)));
        }

        private static readonly FeatureStateTestData[] _testFeatureNames =
        {
            new FeatureStateTestData {Id = 1, Name = nameof(FeatureName.First), Enabled = true, IsShared = true},
            new FeatureStateTestData {Id = 2, Name = nameof(FeatureName.Second), Enabled = true, IsShared = false},
            new FeatureStateTestData {Id = 3, Name = nameof(FeatureName.Third), Enabled = true, IsShared = false},
            new FeatureStateTestData {Id = 4, Name = nameof(FeatureName.Fourth), Enabled = false, IsShared = true},
            new FeatureStateTestData {Id = 5, Name = nameof(FeatureName.Fifth), Enabled = false, IsShared = false},
            new FeatureStateTestData {Id = 6, Name = nameof(FeatureName.Sixth), Enabled = false, IsShared = true}
        };

        [TearDown]
        public void Cleanup()
        {
            _featureManager = null;
        }
    }

    public class FeatureStateTestData : IFeatureSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<int> ParentIds { get; set; }
        public bool IsShared { get; set; }

        IReadOnlyCollection<int> IFeatureSetting.ParentIds => ParentIds;
    }

    public enum FeatureName
    {
        Default = 0,
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth
    }
}
