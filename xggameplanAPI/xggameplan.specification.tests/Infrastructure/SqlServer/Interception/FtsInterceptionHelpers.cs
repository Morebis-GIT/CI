﻿using System.Linq;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Interception
{
    public static class FtsInterceptionHelpers
    {
        public static string ComputedField(params string[] values)
        {
            return values != null ? string.Join(" ", values.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty;
        }
    }
}
