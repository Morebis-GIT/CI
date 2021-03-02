using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static void ToStoredProcedure<T>(this EntityTypeBuilder<T> entityTypeBuilder, string name)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            entityTypeBuilder.HasAnnotation(SqlServerSpecificDbAdapter.StoredProcAnnotationName, name);
        }
    }
}
