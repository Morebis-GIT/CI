using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Exceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions
{
    public static class ModelExtensions
    {
        public static IEntityType GetEntityType<TEntity>(this IModel model) where TEntity : class
        {
            return model.FindEntityType(typeof(TEntity)) ??
                   throw new EntityNotFoundException($"{typeof(TEntity).Name} is not an entity type.");
        }

        public static string GetFullTableName<TEntity>(this IModel model) where TEntity : class
        {
            return EntityTypeHelper.GetFullTableName(model?.GetEntityType<TEntity>());
        }
    }
}
