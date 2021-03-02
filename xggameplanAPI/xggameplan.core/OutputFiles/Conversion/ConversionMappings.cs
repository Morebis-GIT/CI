using System;
using System.Collections.Generic;

namespace xggameplan.OutputFiles.Conversion
{
    /// <summary>
    /// Defines conversion mappings when converting output file data. This is typically only necessary to convert static test output
    /// files to refer to local data. E.g. Local Campaign IDs etc.
    /// </summary>
    public class ConversionMappings
    {
        public List<ConversionMapping<Int32>> Campaigns = new List<ConversionMapping<int>>();
        public List<ConversionMapping<Int32>> SalesAreas = new List<ConversionMapping<int>>();
        public List<ConversionMapping<Int64>> SpotExternalRefs = new List<ConversionMapping<long>>();
    }

    /// <summary>
    /// Individual conversion mapping
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConversionMapping<T>
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
    }
}