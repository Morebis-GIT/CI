using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;

namespace ImagineCommunications.GamePlan.Domain.OutputFiles
{
    public interface IOutputFileRepository
    {
        OutputFile Find(string id);

        List<OutputFile> GetAll();

        void Insert(OutputFile outputFile);
    }
}
