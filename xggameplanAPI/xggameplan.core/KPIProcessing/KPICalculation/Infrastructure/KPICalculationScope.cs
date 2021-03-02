using System;
using Autofac;
using xggameplan.KPIProcessing.Abstractions;

namespace xggameplan.KPIProcessing.KPICalculation.Infrastructure
{
    /// <inheritdoc />
    public class KPICalculationScope : IKPICalculationScope
    {
        private readonly ILifetimeScope _lifetimeScope;

        public KPICalculationScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        /// <inheritdoc />
        public T Resolve<T>() => _lifetimeScope.Resolve<T>();

        /// <inheritdoc />
        public bool TryResolve<T>(out T service) where T : class
        {
            service = _lifetimeScope.ResolveOptional<T>();
            return service != null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _lifetimeScope.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
