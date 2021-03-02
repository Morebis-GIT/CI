using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using Microsoft.EntityFrameworkCore;
using MessageInfo = ImagineCommunications.Gameplan.Integration.Data.Entities.MessageInfo;
using MessageInfoDomain = ImagineCommunications.BusClient.Domain.Entities.MessageInfo;

namespace ImagineCommunications.Gameplan.Integration.Data.Repositories
{
    public class MessageInfoRepository : IMessageInfoRepository
    {
        private readonly IntelligenceDbContext _dbContext;
        private readonly IMapper _mapper;

        public MessageInfoRepository(IntelligenceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<MessageInfoDomain> GetByTransactionId(Guid transactionId)
        {
            var messageInfos = from msInfos in _dbContext.MessageInfos
                               join msType in _dbContext.MessageTypes on msInfos.Name equals msType.Id into msTypeJoined
                               from msType in msTypeJoined.DefaultIfEmpty()
                               where msInfos.GroupTransactionId == transactionId
                               select new MessageInfoDomain()
                               {
                                   Id = msInfos.Id,
                                   ReceivedDate = msInfos.ReceivedDate,
                                   ExecutedDate = msInfos.ExecutedDate,
                                   GroupTransactionId = msInfos.GroupTransactionId,
                                   Name = msInfos.Name,
                                   Priority = msInfos.Priority,
                                   ProcessedBatchCount = msInfos.ProcessedBatchCount,
                                   RetryCount = msInfos.RetryCount,
                                   State = msInfos.State,
                                   TotalBatchCount = msInfos.TotalBatchCount,
                                   Type = msInfos.Type,
                                   MessageType = msType != null ? new MessageType() { BatchSize = msType.BatchSize, Description = msType.Description, Id = msType.Id, MessageEntityTypeId = msType.MessageEntityTypeId, Name = msType.Name, Priority = msType.Priority } : null
                               };
            return messageInfos.ToList();
        }

        public IEnumerable<MessageInfoDomain> GetByTransactionIds(IEnumerable<Guid> transactionIds)
        {
            var messageInfos = from msInfos in _dbContext.MessageInfos
                               join msType in _dbContext.MessageTypes on msInfos.Name equals msType.Id
                               where transactionIds.Contains(msInfos.GroupTransactionId)
                               select new MessageInfoDomain()
                               {
                                   Id = msInfos.Id,
                                   ReceivedDate = msInfos.ReceivedDate,
                                   ExecutedDate = msInfos.ExecutedDate,
                                   GroupTransactionId = msInfos.GroupTransactionId,
                                   Name = msInfos.Name,
                                   Priority = msInfos.Priority,
                                   ProcessedBatchCount = msInfos.ProcessedBatchCount,
                                   RetryCount = msInfos.RetryCount,
                                   State = msInfos.State,
                                   TotalBatchCount = msInfos.TotalBatchCount,
                                   Type = msInfos.Type,
                                   MessageType = new MessageType() { BatchSize = msType.BatchSize, Description = msType.Description, Id = msType.Id, MessageEntityTypeId = msType.MessageEntityTypeId, Name = msType.Name, Priority = msType.Priority }
                               };
            return messageInfos.ToList();
        }

        public void Delete(Guid id)
        {
            var local = _dbContext.MessageInfos.Local.FirstOrDefault(i => i.Id == id)
                ?? _dbContext.Attach(new Entities.MessageInfo() { Id = id }).Entity;

            _dbContext.Remove(local);

            _dbContext.SaveChanges();
        }

        public void Add(MessageInfoDomain entity)
        {
            var messageInfo = _mapper.Map<MessageInfo>(entity);

            _dbContext.MessageInfos.Add(messageInfo);
            _dbContext.SaveChanges();
        }

        public void Update(MessageInfoDomain entity)
        {
            var local = _dbContext.MessageInfos.Local.FirstOrDefault(i => i.Id == entity.Id);

            if (local == null)
            {
                var entry = _dbContext.Attach(new Entities.MessageInfo() { Id = entity.Id });

                entry.State = EntityState.Modified;

                local = entry.Entity;
            }

            _mapper.Map(entity, local);

            _dbContext.SaveChanges();
        }
    }
}
