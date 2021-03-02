using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    public static class FullTextSearchConditionBuilderExtensions
    {
        public static IFullTextSearchConditionBuilder StartAllWith(this IFullTextSearchConditionBuilder builder, params string[] values)
        {
            if (values?.Any() ?? false)
            {
                var b = builder.StartWith(values.First());
                foreach (var value in values.Skip(1))
                {
                    builder = b.And();
                    b = builder.StartWith(value);
                }
            }

            return builder;
        }

        public static IFullTextSearchConditionBuilder StartAnyWith(this IFullTextSearchConditionBuilder builder, params string[] values)
        {
            if (values?.Any() ?? false)
            {
                var b = builder.StartWith(values.First());
                foreach (var value in values.Skip(1))
                {
                    builder = b.Or();
                    b = builder.StartWith(value);
                }
            }

            return builder;
        }
    }
}
