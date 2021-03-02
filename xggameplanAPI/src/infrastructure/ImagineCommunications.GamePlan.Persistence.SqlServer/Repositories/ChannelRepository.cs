using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ChannelRepository : IChannelsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ChannelRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Channel channel)
        {
            var entity = _dbContext.Find<Entities.Tenant.Channel>(channel.Id);
            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<Entities.Tenant.Channel>(channel), post => post.MapTo(channel), _mapper);
            }
            else
            {
                _mapper.Map(channel, entity);
                _dbContext.Update(entity, post => post.MapTo(channel), _mapper);
            }
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.Channel>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public IEnumerable<Channel> GetAll()
        {
            return _dbContext.Query<Entities.Tenant.Channel>().ProjectTo<Channel>(_mapper.ConfigurationProvider).ToList();
        }

        public Channel GetById(int id)
        {
            return _mapper.Map<Channel>(_dbContext.Find<Entities.Tenant.Channel>(id));
        }
    }
}
