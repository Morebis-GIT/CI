using System;
using System.Collections.Generic;

namespace xggameplan.common.ActionProcessing
{
    public class ActionProcessorAggregateException : AggregateException
    {
        public ActionProcessorAggregateException()
        {
        }

        public ActionProcessorAggregateException(string message) : base(message)
        {
        }

        public ActionProcessorAggregateException(IEnumerable<Exception> innerExceptions) : base(innerExceptions)
        {
        }

        public ActionProcessorAggregateException(params Exception[] innerExceptions) : base(innerExceptions)
        {
        }
    }
}
