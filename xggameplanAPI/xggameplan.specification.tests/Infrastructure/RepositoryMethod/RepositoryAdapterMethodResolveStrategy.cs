using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RepositoryMethod
{
    public class RepositoryAdapterMethodResolveStrategy<TInstance> : GenericTypeRepositoryMethodResolveStrategy<TInstance>
        where TInstance : class
    {
        private const string LegacyParameterNameStartWith = "param";

        protected override BindingFlags MethodSearchBindingFlags =>
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected override Type TypeOfInstance => Instance.GetType();

        protected override IEnumerable<MethodInfo> FilterByParameters(IEnumerable<MethodInfo> methods, IRepositoryMethodParameters parameters)
        {
            var callResultMethods = methods.Where(m => typeof(CallMethodResult).IsAssignableFrom(m.ReturnType)).ToArray();
            var genericMethods = callResultMethods.Where(m =>
            {
                var p = m.GetParameters();
                return p.Length == 1 && (typeof(IRepositoryMethodParameters).IsAssignableFrom(p[0].ParameterType) ||
                                         //legacy repository method definition
                                         typeof(IDictionary<string, string>).IsAssignableFrom(p[0].ParameterType) &&
                                         p[0].Name.StartsWith(LegacyParameterNameStartWith, StringComparison.Ordinal));
            }).ToArray();
            return genericMethods.Union(base.FilterByParameters(callResultMethods.Except(genericMethods), parameters));
        }

        protected override IEnumerable<MethodInfo> FilterByMethodName(IEnumerable<MethodInfo> methods, string methodName)
        {
            return methods.Where(m => m.GetCustomAttributes<RepositoryMethodAttribute>().Any() &&
                                      (m.GetCustomAttribute<RepositoryMethodAttribute>().MethodName ?? m.Name) ==
                                      methodName);
        }


        protected override object ResolveParameterValue(ParameterInfo parameterInfo, IRepositoryMethodParameters parameters, out Exception exception)
        {
            if (parameterInfo.ParameterType.IsAssignableFrom(typeof(IRepositoryMethodParameters)) ||
                //legacy repository method definition
                parameterInfo.ParameterType.IsAssignableFrom(typeof(IDictionary<string, string>)) &&
                parameterInfo.Name.StartsWith(LegacyParameterNameStartWith, StringComparison.Ordinal))
            {
                exception = null;
                return parameters;
            }

            return base.ResolveParameterValue(parameterInfo, parameters, out exception);
        }

        public RepositoryAdapterMethodResolveStrategy(TInstance instance) : base(instance)
        {
        }
    }
}
