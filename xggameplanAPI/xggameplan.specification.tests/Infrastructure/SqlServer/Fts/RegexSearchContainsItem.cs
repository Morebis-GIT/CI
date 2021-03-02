using System;
using System.Text.RegularExpressions;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Fts
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
