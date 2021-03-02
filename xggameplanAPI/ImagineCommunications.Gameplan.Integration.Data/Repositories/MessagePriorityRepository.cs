using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using System.Collections.Generic;
using System.Linq;
using MessagePriorityDomain = ImagineCommunications.BusClient.Domain.Entities.MessageType;

namespace ImagineCommunications.Gameplan.Integration.Data.Repositories
{
    public class MessagePriorityRepository : IMessagePriorityRepository
    {
        private readonly IntelligenceDbContext _dbContext;
        private readonly IMapper _mapper;

        public MessagePriorityRepository(IntelligenceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<MessagePriorityDomain> GetAll()
        {
            return _dbContext.MessageTypes
                .ProjectTo<MessagePriorityDomain>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public MessagePriorityDomain GetById(string id)
        {
            return _dbContext.MessageTypes
                .ProjectTo<MessagePriorityDomain>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
