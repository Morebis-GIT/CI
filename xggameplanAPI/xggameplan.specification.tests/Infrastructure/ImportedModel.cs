using System;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure
{
    public class ImportedModel<T> : IImportedModel
        where T : class
    {
        public ImportedModel()
        {
            Type = typeof(T);
        }
        public Type Type { get; }
    }
}
