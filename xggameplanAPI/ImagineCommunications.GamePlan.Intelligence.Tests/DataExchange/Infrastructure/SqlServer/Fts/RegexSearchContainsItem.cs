using System;
using System.Text.RegularExpressions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Fts
{
    public class RegexSearchContainsItem : RegexSearchItemBase
    {
        private readonly string _value;

        public RegexSearchContainsItem(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected override string ConditionExpression => $@"\b{Regex.Escape(_value)}\b";
    }
}
