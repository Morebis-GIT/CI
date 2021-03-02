using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers
{
    public static class EntityTypeHelper
    {
        public static string GetFullTableName(IEntityType entityType)
        {
            var entityTypeInfo = entityType?.Relational();
            return entityTypeInfo is null ? null :
                (!string.IsNullOrEmpty(entityTypeInfo.Schema) ? $"{entityTypeInfo.Schema}." : string.Empty) +
                $"{entityTypeInfo.TableName}";
        }
    }
}
