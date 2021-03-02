using System.Collections.Generic;
using BoDi;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Extensions
{
    public static class ObjectContainerExtensions
    {
        private static string FeatureDependenciesName = "featureDependencies";
        private static string ScenarioDependenciesName = "scenarioDependencies";

        private static ICollection<IFeatureDependency> GetFeatureDependencies(IObjectContainer objectContainer)
        {
            if (!objectContainer.IsRegistered<ICollection<IFeatureDependency>>(FeatureDependenciesName))
            {
                objectContainer.RegisterInstanceAs<ICollection<IFeatureDependency>>(new List<IFeatureDependency>(), FeatureDependenciesName);
            }

            return objectContainer.Resolve<ICollection<IFeatureDependency>>(FeatureDependenciesName);
        }

        private static ICollection<IScenarioDependency> GetScenarioDependencies(IObjectContainer objectContainer)
        {
            if (!objectContainer.IsRegistered<ICollection<IScenarioDependency>>(ScenarioDependenciesName))
            {
                objectContainer.RegisterInstanceAs<ICollection<IScenarioDependency>>(new List<IScenarioDependency>(), ScenarioDependenciesName);
            }
            return objectContainer.Resolve<ICollection<IScenarioDependency>>(ScenarioDependenciesName);
        }

        public static void RegisterFeatureDependency<TType>(this IObjectContainer objectContainer)
            where TType : IFeatureDependency
        {
            var featureDependencies = GetFeatureDependencies(objectContainer);
            featureDependencies.Add(objectContainer.Resolve<TType>());
        }

        public static IEnumerable<IFeatureDependency> ResolveFeatureDependencies(this IObjectContainer objectContainer)
        {
            return GetFeatureDependencies(objectContainer);
        }

        public static void RegisterScenarioDependency<TType>(this IObjectContainer objectContainer)
            where TType : IScenarioDependency
        {
            var scenarioDependencies = GetScenarioDependencies(objectContainer);
            scenarioDependencies.Add(objectContainer.Resolve<TType>());
        }

        public static IEnumerable<IScenarioDependency> ResolveScenarioDependencies(this IObjectContainer objectContainer)
        {
            return GetScenarioDependencies(objectContainer);
        }
    }
}
