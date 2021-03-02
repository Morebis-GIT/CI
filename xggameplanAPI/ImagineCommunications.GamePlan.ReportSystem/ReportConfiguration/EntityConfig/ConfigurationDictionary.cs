using System.Collections.Generic;
using System.Reflection;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig
{
    public class ConfigurationDictionary<TBuilder, TMemberOption> : Dictionary<MemberInfo, TBuilder>, IConfigurationDictionary<TBuilder, TMemberOption>
        where TBuilder : IMemberOptionsBuilder<TBuilder, TMemberOption>
        where TMemberOption : class, IMemberOptions
    {
    }

    public interface IConfigurationDictionary<TBuilder, TMemberOption> : IDictionary<MemberInfo, TBuilder>
        where TBuilder : IMemberOptionsBuilder<TBuilder, TMemberOption>
        where TMemberOption : class, IMemberOptions
    {

    }
}
