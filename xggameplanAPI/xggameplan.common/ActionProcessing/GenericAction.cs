using System;

namespace xggameplan.common.ActionProcessing
{
    public class GenericAction : IAction
    {
        private readonly Action _action;

        protected GenericAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Execute()
        {
            _action();
        }

        public static IAction Create(Action action)
        {
            return new GenericAction(action);
        }
    }
}
