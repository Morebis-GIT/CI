using System;

namespace xggameplan.OutputProcessors.Abstractions
{
    public interface IOutputFileProcessor
    {
        string FileName { get; }

        object ProcessFile(Guid scenarioId, string folder);
    }


    public interface IOutputFileProcessor<out TOutput> : IOutputFileProcessor
        where TOutput : class
    {
        TOutput ProcessFile(Guid scenarioId, string folder);
    }
}
