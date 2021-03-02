using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using MessagePayloadDomain = ImagineCommunications.BusClient.Domain.Entities.MessagePayload;

namespace ImagineCommunications.Gameplan.Integration.Data.Repositories
{
    public class MessagePayloadRepository : IMessagePayloadRepository
    {
        private readonly IntelligenceDbContext _dbContext;
        private readonly IMapper _mapper;

        public MessagePayloadRepository(IntelligenceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public MessagePayloadDomain GetById(Guid id)
        {
            return _dbContext.MessagePayloads
                .ProjectTo<MessagePayloadDomain>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id);
        }

        public void Add(MessagePayloadDomain entity)
        {
            _dbContext.MessagePayloads.Add(_mapper.Map<MessagePayload>(entity));
            _dbContext.SaveChanges();
        }

        public void DeleteById(Guid messageId)
        {
            var data = _dbContext.MessagePayloads.Local.FirstOrDefault(t => t.Id == messageId) ??
                _dbContext.MessagePayloads.Attach(new MessagePayload() { Id = messageId }).Entity;

            _dbContext.MessagePayloads.Remove(data);
        }
    }
}
