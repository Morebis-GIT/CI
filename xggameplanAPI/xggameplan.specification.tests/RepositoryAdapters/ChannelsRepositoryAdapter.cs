using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ChannelsRepositoryAdapter : RepositoryTestAdapter<Channel, IChannelsRepository, int>
    {
        public ChannelsRepositoryAdapter(IScenarioDbContext dbContext, IChannelsRepository repository) :
            base(dbContext, repository)
        {
        }

        protected override Channel Add(Channel model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Channel> AddRange(params Channel[] models)
        {
            foreach (var channel in models)
            {
                Repository.Add(channel);
            }
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override IEnumerable<Channel> GetAll()
        {
            return Repository.GetAll();
        }

        protected override Channel GetById(int id)
        {
            return Repository.GetById(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override Channel Update(Channel model)
        {
            throw new NotImplementedException();
        }
    }
}
