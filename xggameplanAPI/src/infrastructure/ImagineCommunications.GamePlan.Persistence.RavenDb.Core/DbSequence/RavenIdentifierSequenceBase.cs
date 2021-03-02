using System;
using System.Globalization;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbSequence
{
    public class RavenIdentifierSequenceBase : IRavenIdentifierSequence
    {
        private readonly IDocumentSession _documentSession;

        protected RavenIdentifierSequenceBase(IDocumentSession documentSession)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public T GetNextValue<T>(string sequenceName) where T : IConvertible
        {
            var identifier = _documentSession.Advanced.DocumentStore.DatabaseCommands.NextIdentityFor(sequenceName);
            return (T)Convert.ChangeType(identifier, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
