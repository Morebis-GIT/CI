using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Utils.DataPurging.Attributes;
using ImagineCommunications.GamePlan.Utils.DataPurging.Interfaces;
using Microsoft.Extensions.Options;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Handlers
{
    /// <summary>
    /// Exposes purging task functionality with configuration options.
    /// Descendant classes should contain <see cref="PurgingOptionsAttribute"/>
    /// which contains information about configuration section.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options.</typeparam>
    public abstract class DataPurgingHandlerBase<TOptions> : IDataPurgingHandler
        where TOptions : class, new()
    {
        private readonly IOptionsSnapshot<TOptions> _options;
        private TOptions _cachedOptions;

        protected DataPurgingHandlerBase(IOptionsSnapshot<TOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected virtual TOptions Options
        {
            get
            {
                if (_cachedOptions is null)
                {
                    var attr = GetType().GetCustomAttribute<PurgingOptionsAttribute>(true);
                    _cachedOptions = string.IsNullOrEmpty(attr?.Name) ? _options.Value : _options.Get(attr.Name);
                }

                return _cachedOptions;
            }
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
