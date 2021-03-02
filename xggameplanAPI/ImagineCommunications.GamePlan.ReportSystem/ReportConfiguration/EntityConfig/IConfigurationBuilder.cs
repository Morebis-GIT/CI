using System;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig
{
    public interface IConfigurationBuilder<TEntity, out TMemberOptionsBuilder, out TBuilder, out TReportConfig>
        where TBuilder : IConfigurationBuilder<TEntity, TMemberOptionsBuilder, TBuilder, TReportConfig>
        where TEntity : class
        where TMemberOptionsBuilder: IMemberOptionsBuilder<TMemberOptionsBuilder, IMemberOptions>
        where TReportConfig: IReportConfigurations
    {
        TBuilder HideHeader();
        TBuilder HideHeader(bool isHideHeader);
        TBuilder UseHumanizeHeader(bool useHeaderHumanizer = false);
        TBuilder OrderMembersAsDescribed(bool orderMembersAsDescribed = true);
        TBuilder IgnoreNotDescribed();
        TBuilder ForMember(Expression<Func<TEntity, object>> memberExpression, Action<TMemberOptionsBuilder> memberOptions);
        TBuilder ForMember(string memberName, Action<TMemberOptionsBuilder> memberOptions);
        TReportConfig BuildConfiguration();
    }
}
