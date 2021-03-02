using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.BusClient.Domain;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Dto;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using Microsoft.EntityFrameworkCore;
using GroupTransactionInfoDomain = ImagineCommunications.BusClient.Domain.Entities.GroupTransactionInfo;

namespace ImagineCommunications.Gameplan.Integration.Data.Repositories
{
    public class GroupTransactionInfoRepository : IGroupTransactionInfoRepository
    {
        private readonly IntelligenceDbContext _dbContext;
        private readonly IMapper _mapper;

        public GroupTransactionInfoRepository(IntelligenceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public GroupTransactionInfoDomain GetLatestExecutedGroupTransaction()
        {
            return _dbContext.GroupTransactionInfos
            .ProjectTo<GroupTransactionInfoDomain>(_mapper.ConfigurationProvider)
            .OrderByDescending(c => c.ExecutedDate)
            .FirstOrDefault(c =>
                    c.State == MessageState.Completed ||
                    c.State == MessageState.Failed ||
                    c.State == MessageState.InProgress
                );
        }

        public GroupTransactionInfoDomain GetById(Guid id)
        {
            return _dbContext.GroupTransactionInfos
                .ProjectTo<GroupTransactionInfoDomain>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id);
        }

        public void Add(GroupTransactionInfoDomain entity)
        {
            var messageInfo = _mapper.Map<GroupTransactionInfo>(entity);

            var entry = _dbContext.GroupTransactionInfos.Add(messageInfo);

            _dbContext.SaveChanges();
        }

        public void Update(GroupTransactionInfoDomain entity)
        {
            var local = _dbContext.GroupTransactionInfos.Local.FirstOrDefault(i => i.Id == entity.Id);

            if (local == null)
            {
                var entry = _dbContext.Attach(new GroupTransactionInfo() { Id = entity.Id });

                entry.State = EntityState.Modified;

                local = entry.Entity;
            }

            _mapper.Map(entity, local);

            _dbContext.SaveChanges();
        }


        public List<GroupTransactionInfoDomain> GetTransactionsToExecute(int transactionsLimit = 10)
        {
            return _mapper.Map<List<GroupTransactionInfoDomain>>(_dbContext.GroupTransactionInfos
                .Where(t => t.State != MessageState.Completed && t.State != MessageState.InProgress)
                .Select(t => new GroupTransactionInfoDto
                {
                    Id = t.Id,
                    EventCount = t.EventCount,
                    State = t.State,
                    CreatedDate = t.CreatedDate,
                    ReceivedDate = t.ReceivedDate,
                    ExecutedDate = t.ExecutedDate,
                    CompletedDate = t.CompletedDate,
                    MessageCount = t.Messages.Count
                })
                .OrderBy(t => t.CreatedDate)
                .Take(transactionsLimit)
                .AsEnumerable()
                .TakeWhile(t =>
                    t.State != MessageState.Failed &&
                    !(t.State == MessageState.Pending && t.EventCount != t.MessageCount)));
        }
    }
}
