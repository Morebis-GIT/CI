using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class RepositoryDictionary : Dictionary<Type, object>
    {
        public RepositoryDictionary()
        {
        }

        public RepositoryDictionary(IDictionary<Type, object> dictionary) : base(dictionary)
        {
        }
    }
}
