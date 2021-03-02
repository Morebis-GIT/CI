using System;

namespace xggameplan.common.BackgroundJobs
{
    public interface IBackgroundJobParameter
    {
        string Name { get; }

        Type Type { get; }

        object Value { get; }
    }
}
