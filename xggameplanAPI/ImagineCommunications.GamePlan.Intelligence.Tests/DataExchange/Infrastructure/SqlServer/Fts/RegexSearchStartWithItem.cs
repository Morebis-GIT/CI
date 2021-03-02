using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Fts
{
    public class RegexSearchStartWithItem : RegexSearchItemBase
    {
        private readonly IReadOnlyCollection<string> _values;

        public RegexSearchStartWithItem(string value)
        {
            _values = (value ?? throw new ArgumentNullException(nameof(value))).Split(new[] {' '},
                StringSplitOptions.RemoveEmptyEntries).Select(Regex.Escape).ToList();
        }

        protected override string ConditionExpression =>
            _values.Any() ? @"\b" + string.Join(@"\w*\W", _values) : string.Empty;
    }
}
