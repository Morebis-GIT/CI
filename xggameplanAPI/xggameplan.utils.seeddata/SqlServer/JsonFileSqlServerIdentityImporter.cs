using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.utils.seeddata.Seeding;

namespace xggameplan.utils.seeddata.SqlServer
{
    public class JsonFileSqlServerIdentityImporter<TDomainModel, TIdentityEntity> : JsonFileImporter<TDomainModel>
        where TDomainModel : class
        where TIdentityEntity : class
    {
        private readonly ISqlServerDbContext _dbContext;

        public JsonFileSqlServerIdentityImporter(IDomainModelContext domainModelContext, ISqlServerDbContext dbContext) : base(
            domainModelContext, dbContext)
        {
            _dbContext = dbContext;
        }

        protected override void SaveImportedData()
        {
            // TODO: Aurora-tbd require recheck
            /*
            var tableName = _dbContext.Specific.Model.GetFullTableName<TIdentityEntity>();
            var connection = _dbContext.Specific.Database.GetDbConnection();
            _dbContext.Specific.Database.OpenConnection();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SET IDENTITY_INSERT {tableName} ON;";
                _ = command.ExecuteNonQuery();
            }
            */
            try
            {
                base.SaveImportedData();
            }
            finally
            {
                // TODO: Aurora-tbd require recheck
                /*
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SET IDENTITY_INSERT {tableName} OFF;";
                    _ = command.ExecuteNonQuery();
                }
                */
            }
        }
    }
}
