using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RepositoryMethod
{
    public class GenericTypeRepositoryMethodResolveStrategy<TInstance> : IRepositoryMethodResolveStrategy
        where TInstance : class
    {
        protected TInstance Instance { get; }

        protected virtual BindingFlags MethodSearchBindingFlags =>
            BindingFlags.Instance | BindingFlags.Public;

        protected virtual Type TypeOfInstance => typeof(TInstance);

        protected virtual IEnumerable<MethodInfo> FilterByMethodName(IEnumerable<MethodInfo> methods, string methodName)
        {
            return methods.Where(m => m.Name == methodName);
        }

        protected virtual IEnumerable<MethodInfo> FilterByParameters(IEnumerable<MethodInfo> methods, IRepositoryMethodParameters parameters)
        {
            return methods.Where(m =>
            {
                var p = m.GetParameters();
                return !p.Any() && !parameters.Any() ||
                       p.Any() && !p.Select(x => x.Name).Except(parameters.Select(x => x.Key)).Any();
            });
        }

        protected virtual object ResolveParameterValue(ParameterInfo parameterInfo, IRepositoryMethodParameters parameters, out Exception exception)
        {
            exception = null;
            try
            {
                if (!parameters.ContainsKey(parameterInfo.Name))
                {
                    if (parameterInfo.HasDefaultValue)
                    {
                        return parameterInfo.DefaultValue;
                    }

                    throw new Exception($"'{parameterInfo.Name}' parameter has not been defined.");
                }

                return parameters[parameterInfo.Name].SpecflowConvert(parameterInfo.ParameterType);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return null;
        }

        public GenericTypeRepositoryMethodResolveStrategy(TInstance instance)
        {
            Instance = instance;
        }


        public RepositoryMethodResolveInfo Resolve(string methodName, IRepositoryMethodParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new RepositoryMethodParameters();
            }

            var methods = TypeOfInstance.GetMethods(MethodSearchBindingFlags);
            if (TypeOfInstance.IsInterface)
            {
                methods = methods
                    .Union(TypeOfInstance.GetInterfaces().SelectMany(x => x.GetMethods(MethodSearchBindingFlags)))
                    .ToArray();
            }

            methods = FilterByParameters(FilterByMethodName(methods, methodName), parameters).ToArray();

            if (!methods.Any())
            {
                return null;
            }

            if (methods.Distinct(new MethodParameterNamesEqualityComparer()).Count() != methods.Length)
            {
                throw new Exception(
                    $"'{TypeOfInstance.Name}' type has ambiguous definitions of '{methodName}' method with the same names of parameters.");
            }

            var exceptions = new List<Exception>();
            foreach (var method in methods)
            {
                var methodParams = method.GetParameters();
                var paramValues = new List<object>();
                var paramsPopulated = true;
                foreach (var methodParam in methodParams)
                {
                    var value = ResolveParameterValue(methodParam, parameters, out var ex);
                    if (ex != null)
                    {
                        exceptions.Add(new Exception(
                            $"Resolving of '{method.Name}' method with {methodParams.Length} parameters has been failed.",
                            ex));
                        paramsPopulated = false;
                        break;
                    }

                    paramValues.Add(value);
                }

                if (paramsPopulated)
                {
                    return new RepositoryMethodResolveInfo
                    {
                        Method = method,
                        Instance = Instance,
                        Parameters = paramValues.ToArray()
                    };
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Resolving of '{methodName}' method of '{typeof(TInstance).Name}' type has thrown errors. See inner exceptions for more details.",
                    exceptions);
            }

            return null;
        }
    }
}
