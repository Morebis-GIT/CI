using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.common.ActionProcessing
{
    public class ActionProcessor : ActionProcessor<ICollection<IAction>>
    {
        private ICollection<IAction> _actionCollection;

        protected override ICollection<IAction> ActionCollection =>
            _actionCollection ?? (_actionCollection = new List<IAction>());

        protected override void InternalAdd(IAction action)
        {
            ActionCollection.Add(action);
        }

        protected override void InternalClear()
        {
            ActionCollection.Clear();
        }

        public override int Count => ActionCollection.Count;
    }

    public abstract class ActionProcessor<TCollection> : IActionProcessor
        where TCollection : IEnumerable<IAction>
    {
        protected abstract TCollection ActionCollection { get; }

        protected abstract void InternalAdd(IAction action);

        protected abstract void InternalClear();

        public void Add(IAction action)
        {
            InternalAdd(action ?? throw new ArgumentNullException(nameof(action)));
        }

        public void Clear()
        {
            InternalClear();
        }

        public virtual int Count => ActionCollection.Count();

        public void Execute()
        {
            var actionExceptions = new List<Exception>();
            foreach (var action in ActionCollection)
            {
                try
                {
                    action.Execute();
                }
                catch (Exception ex)
                {
                    actionExceptions.Add(ex);
                }
            }

            if (actionExceptions.Count > 0)
            {
                throw new ActionProcessorAggregateException(actionExceptions);
            }
        }
    }
}
