using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    internal class FtsPropertyReferenceVisitor : ExpressionVisitor
    {
        private readonly FtsContainsMethodDescriptor _descriptor;

        public FtsPropertyReferenceVisitor(FtsContainsMethodDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (_descriptor.EntityReference == null)
            {
                _descriptor.EntityReference = node.Expression;
                _descriptor.PropertyReference = Expression.Constant(node.Member.Name);
                _ = new FtsEntityReferenceVisitor(_descriptor).Visit(node.Expression);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_descriptor.EntityReference == null)
            {
                if (node.Method.Name == nameof(EF.Property) &&
                    node.Method.DeclaringType == typeof(EF) &&
                    node.Arguments?.Count > 1)
                {
                    _descriptor.EntityReference = node.Arguments[0];
                    _descriptor.PropertyReference = node.Arguments[1];
                    _ = new FtsEntityReferenceVisitor(_descriptor).Visit(node.Arguments[0]);
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
