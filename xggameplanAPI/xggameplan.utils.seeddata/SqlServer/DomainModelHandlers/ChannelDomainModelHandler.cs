using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ChannelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Channel;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ChannelDomainModelHandler : IDomainModelHandler<Channel>
    {
        private readonly IChannelsRepository _channelsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public ChannelDomainModelHandler(IChannelsRepository channelsRepository, ISqlServerDbContext dbContext)
        {
            _channelsRepository = channelsRepository ?? throw new ArgumentNullException(nameof(channelsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Channel Add(Channel model)
        {
            _channelsRepository.Add(model);
            return model;
        }

        public void AddRange(params Channel[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() =>
            _dbContext.Query<ChannelEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<ChannelEntity>();

        public IEnumerable<Channel> GetAll() => _channelsRepository.GetAll();
    }
}
