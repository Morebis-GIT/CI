using System;
using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    public class FtsInjectionDescriptor
    {
        public Type EntityType { get; set; }
        public string PropertyName { get; set; }
        public LambdaExpression SearchValueExpression { get; set; }
    }
}