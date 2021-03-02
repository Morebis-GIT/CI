using BoDi;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    public class RavenDbFeatureDependency : IFeatureDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            //stores and sessions should be registered as instances in order to be disposed after scenario execution
            var ravenDb = new RavenDbDataStore();
            objectContainer.RegisterInstanceAs(ravenDb.DocumentStore, null, true);
            objectContainer.RegisterInstanceAs(ravenDb.FilesStore, null, true);
        }
    }
}
