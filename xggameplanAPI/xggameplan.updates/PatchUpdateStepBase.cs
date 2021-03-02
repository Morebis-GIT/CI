using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client;

namespace xggameplan.Updates
{
    /// <summary>
    /// Base class for Raven patch update step
    /// </summary>
    internal abstract class PatchUpdateStepBase
    {
        /// <summary>
        /// Executes a patch script by index query
        /// </summary>
        /// <param name="session"></param>
        /// <param name="index"></param>
        /// <param name="query"></param>
        /// <param name="script"></param>
        protected void ExecutePatchScriptByIndexQuery(IDocumentSession session, string index, string query, string script)
        {
            _ = session.Advanced.DocumentStore.DatabaseCommands.UpdateByIndex(index,
                                new IndexQuery() { Query = query },
                                new ScriptedPatchRequest() { Script = script }).WaitForCompletion();
        }

        /// <summary>
        /// Executes a patch script by documentIds for a list of documents
        /// </summary>
        /// <param name="session"></param>
        /// <param name="documentIds"></param>
        /// <param name="script"></param>
        protected void ExecutePatchScriptByDocumentIds(IDocumentSession session, IEnumerable<string> documentIds, string script)
        {
            documentIds.ToList().ForEach(documentId => session.Advanced.DocumentStore.DatabaseCommands.Patch(documentId, new ScriptedPatchRequest() { Script = script }));
        }
    }
}
