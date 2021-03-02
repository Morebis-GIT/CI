using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface IImportedModel
    {
        Type Type { get; }
        IEnumerable<object> CreateSet(Table table);
    }
}
