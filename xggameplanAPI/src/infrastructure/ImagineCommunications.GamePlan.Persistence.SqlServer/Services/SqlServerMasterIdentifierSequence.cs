using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbSequence;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SqlServerMasterIdentifierSequence : SqlServerIdentifierSequenceBase, ISqlServerMasterIdentifierSequence
    {
        public SqlServerMasterIdentifierSequence(ISqlServerMasterDbContext dbContext) : base(dbContext)
        {
        }
    }
}
