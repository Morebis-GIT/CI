using System;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Defines a boundary in which KPI calculation instances are shared and configured.
    /// Disposing an <see cref="IKPICalculationScope"/> will dispose the shared components.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IKPICalculationScope : IDisposable
    {
        /// <summary>
        /// Resolves KPI calculation related component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary>
        /// Tries the resolve KPI calculation related component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Resolved</returns>
        bool TryResolve<T>(out T service) where T : class;
    }
}
