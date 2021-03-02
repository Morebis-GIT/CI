using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbSequence;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SqlServerTenantIdentifierSequence : SqlServerIdentifierSequenceBase, ISqlServerTenantIdentifierSequence
    {
        public SqlServerTenantIdentifierSequence(ISqlServerTenantDbContext dbContext) : base(dbContext)
        {
        }
    }
}
