using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SqlServerIdentityGenerator : ISqlServerIdentityGenerator
    {
        private readonly ISqlServerTenantIdentifierSequence _tenantIdentifierSequence;

        public SqlServerIdentityGenerator(ISqlServerTenantIdentifierSequence tenantIdentifierSequence)
        {
            _tenantIdentifierSequence = tenantIdentifierSequence;
        }

        /// <summary>
        /// Generates N new identities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<T> GetIdentities<T>(int number) where T : class, IIntIdentifier, new()
        {
            var sequenceName = typeof(T).Name;

            //TODO: use sp_sequence_get_range to improve current implementation
            var res = new List<T>();
            for (var i = 0; i < number; i++)
            {
                res.Add(new T
                {
                    Id = _tenantIdentifierSequence.GetNextValue<int>(sequenceName)
                });
            }

            return res;
        }
    }
}
