using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ImagineCommunications.GamePlan.ReportSystem.TypeAccessor
{
    public class TypeAccessor
    {
        private IDictionary<CacheKey, MemberAccessor> GetterCache { get; set; } = new ConcurrentDictionary<CacheKey,MemberAccessor>();

        public static IEnumerable<MemberInfo> GetAllMembers(Type type, bool includeNonPublic = false)
        {
            var nonPublicFlag = includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default;
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | nonPublicFlag);
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | nonPublicFlag);

            return properties.Concat<MemberInfo>(fields);
        }

        public static MemberInfo GetMember(Type type, string memberName, bool includeNonPublic = false)
        {
            return GetAllMembers(type, includeNonPublic).FirstOrDefault(m => m.Name == memberName) ??
                   throw new ArgumentOutOfRangeException(nameof(memberName),
                       $"Cannot find member {memberName} of type {type}.");
        }

        public static IEnumerable<Type> GetGenericIEnumerable(object o)
        {
            return o.GetType()
                .GetInterfaces()
                .Where(t => t.IsGenericType
                            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments()[0]);
        }

        public virtual object GetValue(object instance, MemberInfo member)
        {
            return GetValue(instance, member.Name);
        }

        public virtual object GetValue(object instance, string memberName)
        {
            if (instance is null)
            {
                return null;
            }

            Type type = instance.GetType();
            var key = new CacheKey(type, memberName);
            if (!GetterCache.TryGetValue(key, out MemberAccessor accessor))
            {
                var member = GetMember(type, memberName);
                Func<object, object> getAccessor = GetGetAccessor(type, memberName);

                if (getAccessor == null)
                {
                    throw new InvalidOperationException($"Member '{memberName}' does not exists in {type.FullName}");
                }

                accessor = new MemberAccessor
                {
                    MemberInfo = member,
                    GetAccessor = getAccessor
                };

                GetterCache[key] = accessor;
            }

            return accessor.GetAccessor(instance);
        }

        public static MemberInfo GetMember(Expression expression)
        {
            if(expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var lambda = (LambdaExpression)expression;
            MemberExpression memberExpr = null;
            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr =
                        ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;
            }

            if (memberExpr is null)
            {
                throw new Exception($"{nameof(expression)} parameter is invalid");
            }

            return memberExpr.Member;
        }

        private static Func<object, object> GetGetAccessor(Type type, string name)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var property = Expression.PropertyOrField(Expression.Convert(instance, type), name);

            return (Expression.Lambda<Func<object, object>>(Expression.Convert(property, typeof(object)), instance)).Compile();
        }

        private struct CacheKey
        {
            public Type Type { get; }
            public string Member { get; }

            internal CacheKey(Type type, string member)
            {
                Type = type;
                Member = member;
            }

            public override int GetHashCode()
            {
                return Type.GetHashCode() ^ Member.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                {
                    return false;
                }

                if ((ValueType)this == obj)
                {
                    return true;
                }

                var cacheKey = (CacheKey)obj;

                if (Type == cacheKey.Type)
                {
                    return Member == cacheKey.Member;
                }

                return false;
            }
        }

        protected class MemberAccessor
        {
            public string Name => MemberInfo.Name;
            public MemberInfo MemberInfo { get; set; }
            public Func<object, object> GetAccessor { get; set; }
        }
    }

    public class TypeAccessor<TSource> : TypeAccessor
    {
        protected IReadOnlyDictionary<string, MemberAccessor<TSource>> GetterCache { get; set; }

        public TypeAccessor(bool includeNonPublic = false)
        {
            InitGetterCache(includeNonPublic);
        }

        protected void InitGetterCache(bool includeNonPublic)
        {
            var nonPublicFlag = includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default;
            var properties = typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public | nonPublicFlag);
            var fields = typeof(TSource).GetFields(BindingFlags.Instance | BindingFlags.Public | nonPublicFlag);

            var propertiesAccessor = properties
                .Select(propertyInfo => new MemberAccessor<TSource>
                {
                    MemberInfo = propertyInfo,
                    ResultType = propertyInfo.PropertyType,
                    GetAccessor = propertyInfo.GetGetAccessor<TSource>(includeNonPublic)
                })
                .Where(a => a.GetAccessor != null);

            var fieldAccessor = fields.Select(fieldInfo => new MemberAccessor<TSource>
            {
                MemberInfo = fieldInfo,
                ResultType = fieldInfo.FieldType,
                GetAccessor = fieldInfo.GetGetAccessor<TSource>(includeNonPublic)
            });

            GetterCache = new ReadOnlyDictionary<string, MemberAccessor<TSource>>(propertiesAccessor
                .Union(fieldAccessor)
                .ToDictionary(a => a.Name, a => a));
        }

        public Dictionary<string, object> GetValues(TSource instance,
            IEnumerable<Expression<Func<TSource, object>>> properties)
            => properties?.ToDictionary(property => GetMember(property).Name,
                   property => GetValue(instance, property))
               ?? new Dictionary<string, object>();

        public Dictionary<string, object> GetValues(TSource instance)
            => GetterCache.Keys.ToDictionary(key => key, key => GetValue(instance, key));

        public object GetValue(TSource instance, string propertyName)
            => GetterCache[propertyName].GetAccessor.Invoke(instance);

        public TResult GetValue<TResult>(TSource instance, string propertyName)
        {
            var property = GetterCache[propertyName];
            return (TResult) property.GetAccessor.Invoke(instance);
        }

        public override object GetValue(object instance, string memberName)
        {
            var memberAccessor = GetterCache[memberName];
            return memberAccessor.GetAccessor.Invoke((TSource)instance);
        }

        public override object GetValue(object instance, MemberInfo member)
        {
            return GetValue(instance, member.Name);
        }

        public TValue GetValue<TValue>(TSource instance, Expression<Func<TSource, TValue>> property)
            => (TValue)GetterCache[GetMember(property).Name].GetAccessor(instance);

        private Dictionary<string, object> GetValues(TSource instance, IEnumerable<string> properties)
            => properties?.ToDictionary(propertyName => propertyName,
                   propertyName => GetValue(instance, propertyName)) ?? new Dictionary<string, object>();

        protected class MemberAccessor<TSource> : MemberAccessor
        {
            public Type ResultType { get; set; }
            public new Func<TSource, object> GetAccessor { get; set; }
        }
    }
}
