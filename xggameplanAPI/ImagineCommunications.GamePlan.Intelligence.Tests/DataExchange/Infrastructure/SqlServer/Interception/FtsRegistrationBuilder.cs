using System;
using System.Linq.Expressions;
using System.Reflection;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Extensions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    public class FtsRegistrationBuilder<TEntity>
        where TEntity : class
    {
        public FtsRegistrationBuilder(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            Descriptor.PropertyName = propertyName;
        }

        public FtsRegistrationBuilder(Expression<Func<TEntity, string>> propertyReference)
        {
            if (!propertyReference.IsParameterMemberExpression(MemberTypes.Property | MemberTypes.Field))
            {
                throw new Exception($"{nameof(propertyReference)} argument should be a member expression.");
            }
            Descriptor.PropertyName = propertyReference.GetMemberInfo().Name;
            Descriptor.SearchValueExpression = propertyReference;
        }


        public FtsRegistrationBuilder<TEntity> SearchValue(Expression<Func<TEntity, string>> expression)
        {
            Descriptor.SearchValueExpression = expression ?? throw new ArgumentNullException(nameof(expression));
            return this;
        }

        public FtsInjectionDescriptor Descriptor { get; } = new FtsInjectionDescriptor {EntityType = typeof(TEntity)};
    }
}