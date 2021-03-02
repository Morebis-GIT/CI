using System;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.ReportSystem.Formaters;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig
{
    public abstract class MemberOptionsBuilder<TBuilder, TMemberOption> : IMemberOptionsBuilder<TBuilder, TMemberOption>
        where TBuilder : MemberOptionsBuilder<TBuilder, TMemberOption>
        where TMemberOption : class, IMemberOptions
    {
        protected TMemberOption Options { get; set; }

        protected MemberOptionsBuilder(TMemberOption options)
        {
            Options = options;
        }

        public virtual TBuilder Ignore()
        {
            Options.Ignore = true;

            return (TBuilder)this;
        }

        public virtual TBuilder Ignore(bool ignore)
        {
            Options.Ignore = ignore;

            return (TBuilder)this;
        }

        public virtual TBuilder Formatter(IFormatter formatter)
        {
            Options.Formatter = formatter;

            return (TBuilder)this;
        }

        public virtual TBuilder Formatter(Func<object, string> formatExpression)
        {
            Options.FormatterExpression = (Expression<Func<object, string>>)(x => formatExpression(x));

            return (TBuilder)this;
        }

        public virtual TBuilder Header(string header)
        {
            Options.Header = header;
            Options.IsHeader = true;

            return (TBuilder)this;
        }

        public TBuilder Order(int order)
        {
            Options.Order = order;

            return (TBuilder)this;
        }

        public TMemberOption Build()
        {
            return Options;
        }
    }
}
