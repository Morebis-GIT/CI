using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts
{
    public class FullTextSearchContainsItem : FullTextSearchItemBase
    {
        private readonly string _value;

        public FullTextSearchContainsItem(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected override string ConditionTerm => $"\"{_value}\"";
    }
}
