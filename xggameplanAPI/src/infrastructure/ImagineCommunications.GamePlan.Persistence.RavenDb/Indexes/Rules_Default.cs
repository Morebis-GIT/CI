using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    /// <summary>
    /// Define a RavenDB index for Rule documents.
    /// </summary>
    public class Rules_Default
        : AbstractIndexCreationTask<Rule>
    {
        /// <summary>
        /// The name of the default Rules documents index.
        /// </summary>
        public static string DefaultIndexName => "Rules/Default";

        public Rules_Default()
        {
            Map = rules => from rule in rules
                            select new
                            {
                                rule.Id,
                                rule.RuleId,
                                rule.RuleTypeId,
                                rule.Description
                            };

            Index(r => r.Description, FieldIndexing.Analyzed);
        }
    }
}
