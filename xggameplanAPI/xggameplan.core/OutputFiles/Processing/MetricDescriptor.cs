using System;

namespace xggameplan.OutputFiles.Processing
{
    internal sealed class MetricDescriptor<TImport> where TImport : class
    {
        public string Name { get; }

        public string DisplayFormat { get; }

        public Func<TImport, double> Value { get; }

        public MetricDescriptor(string name, string displayFormat, Func<TImport, double> calculator)
        {
            Name = name;
            DisplayFormat = displayFormat;
            Value = calculator;
        }
    }
}
