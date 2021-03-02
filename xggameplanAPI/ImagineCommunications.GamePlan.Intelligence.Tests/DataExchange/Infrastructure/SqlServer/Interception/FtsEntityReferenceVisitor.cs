using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    internal class FtsEntityReferenceVisitor : ExpressionVisitor
    {
        private readonly FtsContainsMethodDescriptor _descriptor;

        public FtsEntityReferenceVisitor(FtsContainsMethodDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (_descriptor.EntityType == null)
            {
                _descriptor.EntityType = node.Type;
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_descriptor.EntityType == null)
            {
                _descriptor.EntityType = node.Type;
            }
            return base.VisitParameter(node);
        }
    }
}