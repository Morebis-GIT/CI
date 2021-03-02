using System;
using System.Collections.Generic;

namespace xggameplan.common.Search
{
    public abstract class SearchConditionBuilderContext<TResult> : IBuildAction<TResult>
    {
        private readonly ICollection<SearchConditionItem<TResult>> _items = new List<SearchConditionItem<TResult>>();

        public SearchLogicalOperand? CurrentLogicalOperand { get; set; }

        public void Add(SearchConditionItem<TResult> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.LogicalOperand = CurrentLogicalOperand;
            _items.Add(item);
        }

        public IEnumerable<SearchConditionItem<TResult>> Items => _items;
        public abstract TResult Build();
    }
}
