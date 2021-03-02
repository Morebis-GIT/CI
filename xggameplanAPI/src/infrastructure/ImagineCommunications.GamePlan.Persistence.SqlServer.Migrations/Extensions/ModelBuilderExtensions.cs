using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions
{
    /// <summary>
    /// Extends <see cref="ModelBuilder"/> functionality.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Ignores entity registrations in <see cref="DbContext"/> which are registered as stored procedures.
        /// It might be useful for migration.
        /// </summary>
        public static void RemoveStoredProcEntities(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                if (!(entityType.FindAnnotation(SqlServerSpecificDbAdapter.StoredProcAnnotationName) is null))
                {
                    modelBuilder.Ignore(entityType.ClrType);
                }
            }
        }
    }
}
