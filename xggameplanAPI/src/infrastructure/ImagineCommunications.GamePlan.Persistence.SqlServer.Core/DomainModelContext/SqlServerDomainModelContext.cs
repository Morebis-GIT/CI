using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext
{
    public class SqlServerDomainModelContext : IDomainModelContext
    {
        private static readonly MethodInfo AddMethodInfo = typeof(IDomainModelContext)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x =>
                x.Name == nameof(IDomainModelContext.Add) && x.IsGenericMethod)?.GetGenericMethodDefinition();

        private static readonly MethodInfo AddRangeMethodInfo = typeof(IDomainModelContext)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x =>
                x.Name == nameof(IDomainModelContext.AddRange) && x.IsGenericMethod)?.GetGenericMethodDefinition();

        private readonly IDomainModelHandlerResolver _domainModelHandlerResolver;

        public SqlServerDomainModelContext(IDomainModelHandlerResolver domainModelHandlerResolver)
        {
            _domainModelHandlerResolver = domainModelHandlerResolver;
        }

        public TModel Add<TModel>(TModel model) where TModel : class
        {
            return _domainModelHandlerResolver.Resolve<TModel>().Add(model);
        }

        public object Add(object model)
        {
            var modelType = (model ?? throw new ArgumentNullException(nameof(model))).GetType();
            return AddMethodInfo.MakeGenericMethod(modelType).Invoke(this, new object[] {model});
        }

        public void AddRange<TModel>(params TModel[] models) where TModel : class
        {
            _domainModelHandlerResolver.Resolve<TModel>().AddRange(models);
        }

        public void AddRange(params object[] models)
        {
            AddRange(models.AsEnumerable());
        }

        public void AddRange(IEnumerable<object> models)
        {
            foreach (var group in (models ?? throw new ArgumentNullException(nameof(models))).Where(m => m != null)
                .GroupBy(k => k.GetType()))
            {
                AddRangeMethodInfo.MakeGenericMethod(group.Key).Invoke(this,
                    new object[] {EnumerableCastHelper.CastToArray(group.AsEnumerable(), group.Key)});
            }
        }

        public int Count<TModel>() where TModel : class => _domainModelHandlerResolver.Resolve<TModel>().Count();

        public void DeleteAll<TModel>() where TModel : class =>
            _domainModelHandlerResolver.Resolve<TModel>().DeleteAll();

        public IEnumerable<TModel> GetAll<TModel>() where TModel : class =>
            _domainModelHandlerResolver.Resolve<TModel>().GetAll();
    }
}
