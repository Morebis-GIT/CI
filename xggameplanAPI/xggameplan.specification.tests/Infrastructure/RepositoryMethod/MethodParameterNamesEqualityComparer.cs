using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace xggameplan.specification.tests.Infrastructure.RepositoryMethod
{
    public class MethodParameterNamesEqualityComparer : IEqualityComparer<MethodInfo>
    {
        public bool Equals(MethodInfo x, MethodInfo y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var p1 = x.GetParameters();
            var p2 = y.GetParameters();

            if (p1.Length != p2.Length)
            {
                return false;
            }

            return !p1.Select(p => p.Name).Except(p2.Select(p => p.Name)).Any();
        }

        public int GetHashCode(MethodInfo obj)
        {
            return 0;
        }
    }
}
