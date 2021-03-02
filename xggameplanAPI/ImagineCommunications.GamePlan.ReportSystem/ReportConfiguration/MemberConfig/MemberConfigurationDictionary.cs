using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig
{
    public class MemberConfigurationDictionary<T>: Dictionary<MemberInfo, T> where T : IMemberOptions
    {
        public IEnumerable<T> GetActiveOrderlyOptions()
        {
            foreach (var memberConfig in Values.OrderBy(r => r.Order))
            {
                if (!memberConfig.Ignore)
                {
                    yield return memberConfig;
                }
            }
        }
    }
}
