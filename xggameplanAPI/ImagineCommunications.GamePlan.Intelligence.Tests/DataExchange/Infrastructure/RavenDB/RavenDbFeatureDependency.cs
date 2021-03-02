using BoDi;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    public class RavenDbFeatureDependency : IFeatureDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
#pragma warning disable DF0010 // Marks undisposed local variables.
            // Register this so it's also disposed.
            var ravenDb = new RavenDbDataStore();
            objectContainer.RegisterInstanceAs(ravenDb, null, true);
#pragma warning restore DF0010 // Marks undisposed local variables.

            // Stores and sessions should be registered as instances in order to
            // be disposed after scenario execution.
            objectContainer.RegisterInstanceAs(ravenDb.DocumentStore, null, true);
            objectContainer.RegisterInstanceAs(ravenDb.FilesStore, null, true);
        }
    }
}
