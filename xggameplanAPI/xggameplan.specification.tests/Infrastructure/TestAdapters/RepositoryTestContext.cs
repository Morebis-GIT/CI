using System;
using System.Collections.Generic;

namespace xggameplan.specification.tests.Infrastructure.TestAdapters
{
    public class RepositoryTestContext
    {
        public Type RepositoryModelType { get; set; }

        public Type TestModelType { get; set; }

        public int? LastOperationCount { get; set; }

        public object LastSingleResult { get; set; }

        public IEnumerable<object> LastCollectionResult { get; set; }

        public Exception LastException { get; set; }
    }
}
