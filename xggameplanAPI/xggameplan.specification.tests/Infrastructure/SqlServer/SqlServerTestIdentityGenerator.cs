using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerTestIdentityGenerator : IIdentityGenerator
    {
        private static readonly Dictionary<string, int> _storage = new Dictionary<string, int>();

        List<T> IIdentityGenerator.GetIdentities<T>(int number)
        {
            var result = new List<T>();
            for (int i = 0; i < number; i++)
            {
                result.Add(GetSingleIdentity<T>());
            }
            return result;
        }

        private T GetSingleIdentity<T>() where T : class, IIntIdentifier, new()
        {
            var key = typeof(T).Name;
            var value = 1;
            if (_storage.ContainsKey(key))
            {
                _storage[key] += 1;
                value = _storage[key];
            }
            else
            {
                _storage.Add(key, value);
            }

            return new T
            {
                Id = value
            };
        }
    }
}
