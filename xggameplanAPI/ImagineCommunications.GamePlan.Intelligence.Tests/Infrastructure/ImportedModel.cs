using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure
{
    public class ImportedModel<T> : IImportedModel
        where T : class
    {
        public ImportedModel()
        {
            Type = typeof(T);
        }
        public Type Type { get; }

        public IEnumerable<object> CreateSet(Table table)
        {
            return table.CreateSet<T>();
        }
    }
}
