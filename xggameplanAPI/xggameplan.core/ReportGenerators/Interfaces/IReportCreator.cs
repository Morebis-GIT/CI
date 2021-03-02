using System.Collections.Generic;
using System.IO;

namespace xggameplan.core.ReportGenerators.Interfaces
{
    /// <summary>
    /// Exposes generic functionality of report creator.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    public interface IReportCreator<in TSource, out TDest>
        where TSource : class
        where TDest : class
    {
        /// <summary>Generates the report data.</summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        IReadOnlyCollection<TDest> GenerateReportData(IReadOnlyCollection<TSource> source);

        /// <summary>Generates the report.</summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        Stream GenerateReport(IReadOnlyCollection<TSource> source);
    }
}
