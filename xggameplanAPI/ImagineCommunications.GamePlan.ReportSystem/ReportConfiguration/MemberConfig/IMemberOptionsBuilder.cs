using System;
using ImagineCommunications.GamePlan.ReportSystem.Formaters;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig
{
    public interface IMemberOptionsBuilder<out TBuilder, out TMemberOption>
        where TBuilder: IMemberOptionsBuilder<TBuilder, TMemberOption>
        where TMemberOption : class, IMemberOptions
    {
        TBuilder Ignore();
        TBuilder Ignore(bool ignore);
        TBuilder Formatter(IFormatter formatter);
        TBuilder Formatter(Func<object, string> formatExpression);
        TBuilder Header(string header);
        TBuilder Order(int order);
        TMemberOption Build();
    }
}
