using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using Serilog;
using xggameplan.common.Helpers;
using xggameplan.utils.seeddata.Infrastructure;

namespace xggameplan.utils.seeddata.Migration
{
    public abstract class MigrationDocumentHandler<TDomainModel> : IMigrationDocumentHandler<TDomainModel>
        where TDomainModel : class
    {
        protected ILifetimeScope SourceContainer { get; }
        protected ILifetimeScope DestinationContainer { get; set; }
        protected ILogger Logger { get; }

        protected MigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex, ILogger logger)
        {
            if (containerIndex == null)
            {
                throw new ArgumentNullException(nameof(containerIndex));
            }

            SourceContainer = containerIndex[MigrationSource.From];
            DestinationContainer = containerIndex[MigrationSource.To];
            Logger = logger;
        }

        protected virtual IEnumerable<TDomainModel> GetSourceDocuments()
        {
            if (PageSize > 0)
            {
                var pageReadingOptions = SourceContainer.Resolve<PageReadingOptions>();
                pageReadingOptions.PageSize = PageSize;
            }

            return SourceContainer.Resolve<IDomainModelContext>().GetAll<TDomainModel>();
        }

        protected virtual void AddDestinationModel(TDomainModel destinationModel) =>
            DestinationContainer.Resolve<IDomainModelContext>().Add(destinationModel);

        protected virtual void AddDestinationModelRange(IEnumerable<TDomainModel> destinationModels) =>
            DestinationContainer.Resolve<IDomainModelContext>().AddRange(destinationModels.Cast<object>());

        protected virtual void AdjustModel(TDomainModel model)
        {
        }

        protected virtual void SourceModelListPageAction(List<TDomainModel> modelList)
        {
            if (ProcessDestinationAsABatch)
            {
                AddDestinationModelRange(modelList);
            }
            else
            {
                modelList?.ForEach(AddDestinationModel);
            }
        }

        protected virtual int ProcessSourceModelByPages()
        {
            var total = 0;
            if (PageSize == 0)
            {
                Logger.Debug($"All {typeof(TDomainModel).Name} documents are requested.");
                var allModels = StopwatchHelper.StopwatchFunc(() => GetSourceDocuments().ToList(), out var watch);
                total = allModels.Count;
                Logger.Debug($"{total} {typeof(TDomainModel).Name} documents have been received in {watch.Elapsed}.");
                watch = StopwatchHelper.StopwatchAction(() => SourceModelListPageAction(allModels));
                Logger.Debug($"{total} {typeof(TDomainModel).Name} documents have been processed in {watch.Elapsed}.");
            }
            else
            {
                Logger.Debug($"{typeof(TDomainModel).Name} documents requested by pages with {PageSize} docs for each page.");
                var idx = 0;
                var page = 1;
                var modelList = new List<TDomainModel>((int) PageSize);
                foreach (var doc in GetSourceDocuments())
                {
                    if (idx >= PageSize)
                    {
                        CallPageAction();
                        modelList.Clear();
                        idx = 0;
                        page++;
                    }

                    AdjustModel(doc);
                    modelList.Add(doc);
                    idx++;
                    total++;
                }

                if (idx > 0)
                {
                    CallPageAction();
                }

                #region LocalFunc
                void CallPageAction()
                {
                    var destContainer = DestinationContainer;
                    try
                    {
                        using (DestinationContainer = DestinationContainer.BeginLifetimeScope())
                        {
                            var watch = StopwatchHelper.StopwatchAction(() => SourceModelListPageAction(modelList));
                            Logger.Debug(
                                $"Page #{page} of {idx} {typeof(TDomainModel).Name} documents has been processed in {watch.Elapsed}.");
                        }
                    }
                    finally
                    {
                        DestinationContainer = destContainer;
                    }
                }
                #endregion
            }

            return total;
        }

        public virtual bool Validate()
        {
            return true;
        }

        public virtual int Execute()
        {
            return ProcessSourceModelByPages();
        }

        public virtual bool ProcessDestinationAsABatch { get; set; } = true;

        public virtual int PageSize { get; } = 1024;
    }
}
