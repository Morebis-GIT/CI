using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using xggameplan.common.Extensions;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Interception
{
    public class FtsInterceptionFactory : IFtsInterceptionProvider
    {
        private static readonly MethodInfo ContainsGenericMethodInfo = typeof(FtsInterceptionFactory)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == nameof(Contains) && m.IsGenericMethod)?.GetGenericMethodDefinition();

        private readonly ICollection<FtsInjectionDescriptor> _descriptors = new List<FtsInjectionDescriptor>();

        public FtsRegistrationBuilder<TEntity> Register<TEntity>(string propertyName)
            where TEntity : class
        {
            var builder = new FtsRegistrationBuilder<TEntity>(propertyName);
            _descriptors.Add(builder.Descriptor);
            return builder;
        }

        public FtsRegistrationBuilder<TEntity> Register<TEntity>(Expression<Func<TEntity, string>> propertyReference)
            where TEntity : class
        {
            var builder = new FtsRegistrationBuilder<TEntity>(propertyReference);
            _descriptors.Add(builder.Descriptor);
            return builder;
        }

        public bool Contains<TEntity>(TEntity entity, string propertyName, string searchCondition)
        {
            var descriptor =
                _descriptors.FirstOrDefault(x => x.PropertyName == propertyName && x.EntityType == typeof(TEntity));
            if (descriptor != null)
            {
                var value = descriptor.SearchValueExpression.Compile().DynamicInvoke(entity).ToString();
                var res = new Regex(searchCondition, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(value);
                return res;
            }

            return false;
        }

        public bool Contains(Type entityType, object entity, string propertyName, string searchCondition)
        {
            return (bool) ContainsGenericMethodInfo.MakeGenericMethod(entityType)
                .Invoke(this, new object[] {entity, propertyName, searchCondition});
        }
    }

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

    public class FtsInjectionDescriptor
    {
        public Type EntityType { get; set; }
        public string PropertyName { get; set; }
        public LambdaExpression SearchValueExpression { get; set; }
    }
}
