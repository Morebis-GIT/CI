using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions
{
    public class MasterDocumentSessionWrapper
    {
        public IDocumentSession DocumentSession;

        public MasterDocumentSessionWrapper(IDocumentSession session)
        {

            DocumentSession = session;
        }
    }

    public class MasterAsyncDocumentSessionWrapper
    {
        public IAsyncDocumentSession DocumentSession;

        public MasterAsyncDocumentSessionWrapper(IAsyncDocumentSession session)
        {

            DocumentSession = session;
        }
    }
}
