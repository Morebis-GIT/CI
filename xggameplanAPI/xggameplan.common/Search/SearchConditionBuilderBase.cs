using System;
using System.Collections.Generic;

namespace xggameplan.common.Search
{
    public abstract class SearchConditionBuilderBase<TBuilder, TResult> : ISearchConditionBuilder<TBuilder, TResult>, IFollowingActionSearchConditionBuilder<TBuilder, TResult>
        where TBuilder : SearchConditionBuilderBase<TBuilder, TResult>
    {
        private readonly Stack<SearchConditionBuilderContext<TResult>> _builderContexts =
            new Stack<SearchConditionBuilderContext<TResult>>();

        private SearchConditionBuilderContext<TResult> _currentBuilderContext;

        protected SearchConditionBuilderContext<TResult> CurrentBuilderContext =>
            _currentBuilderContext ?? (_currentBuilderContext = CreateBuilderContext());

        protected abstract SearchConditionBuilderContext<TResult> CreateBuilderContext();

        protected abstract SearchConditionBlockItem<TResult> CreateBlockItem(SearchConditionBuilderContext<TResult> blockBuilderContext);

        public virtual TResult Build()
        {
            return CurrentBuilderContext.Build();
        }

        protected virtual IFollowingActionSearchConditionBuilder<TBuilder, TResult> Block(Action<TBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            SearchConditionBlockItem<TResult> item = null;
            var exceptionIsThrown = false;

            _builderContexts.Push(_currentBuilderContext);
            try
            {
                _currentBuilderContext = null;
                builder(this as TBuilder);
                item = _currentBuilderContext != null ? CreateBlockItem(_currentBuilderContext) : null;
            }
            catch
            {
                exceptionIsThrown = true;
                throw;
            }
            finally
            {
                _currentBuilderContext = _builderContexts.Pop();
                if (!exceptionIsThrown && item != null)
                {
                    CurrentBuilderContext.Add(item);
                }
            }

            return (TBuilder) this;
        }

        IFollowingActionSearchConditionBuilder<TBuilder, TResult> ISearchConditionBuilder<TBuilder, TResult>.Block(Action<TBuilder> builder)
        {
            return Block(builder);
        }

        TBuilder IFollowingActionSearchConditionBuilder<TBuilder, TResult>.And()
        {
            CurrentBuilderContext.CurrentLogicalOperand = SearchLogicalOperand.And;
            return (TBuilder) this;
        }

        TBuilder IFollowingActionSearchConditionBuilder<TBuilder, TResult>.AndNot()
        {
            CurrentBuilderContext.CurrentLogicalOperand = SearchLogicalOperand.AndNot;
            return (TBuilder)this;
        }

        TBuilder IFollowingActionSearchConditionBuilder<TBuilder, TResult>.Or()
        {
            CurrentBuilderContext.CurrentLogicalOperand = SearchLogicalOperand.Or;
            return (TBuilder)this;
        }
    }
}
