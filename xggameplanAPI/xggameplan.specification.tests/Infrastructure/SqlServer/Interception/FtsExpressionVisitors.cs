using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Interception
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
                    node.Method.DeclaringType == typeof(EF))
                {
                    _descriptor.EntityReference = node.Arguments[0];
                    _descriptor.PropertyReference = node.Arguments[1];
                    _ = new FtsEntityReferenceVisitor(_descriptor).Visit(node.Arguments[0]);
                }
            }

            return base.VisitMethodCall(node);
        }
    }

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

    internal class FtsContainsMethodDescriptor
    {
        public Expression EntityReference { get; set; }

        public Expression PropertyReference { get; set; }

        public Type EntityType { get; set; }
    }

}
