using System;
using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    internal class FtsContainsMethodDescriptor
    {
        public Expression EntityReference { get; set; }

        public Expression PropertyReference { get; set; }

        public Type EntityType { get; set; }
    }
}