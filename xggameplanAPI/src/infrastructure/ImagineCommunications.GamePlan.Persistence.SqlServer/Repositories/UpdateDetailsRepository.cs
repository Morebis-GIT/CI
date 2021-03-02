using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class UpdateDetailsRepository : IUpdateDetailsRepository
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateDetailsRepository(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public UpdateDetails Find(Guid id) =>
            _mapper.Map<UpdateDetails>(_dbContext.Find<Entities.Master.UpdateDetails>(id));

        public void Update(UpdateDetails updateDetails) {
            var entityToUpdate = _dbContext.Find<Entities.Master.UpdateDetails>(updateDetails.Id);
            if (entityToUpdate != null)
            {
                _mapper.Map(updateDetails, entityToUpdate);
                _dbContext.Update(_mapper.Map<Entities.Master.UpdateDetails>(entityToUpdate),
                    post => post.MapTo(updateDetails), _mapper);
                }
        } 

        public void Add(UpdateDetails updateDetails) =>
            _dbContext.Add(_mapper.Map<Entities.Master.UpdateDetails>(updateDetails),
                post => post.MapTo(updateDetails), _mapper);

        public List<UpdateDetails> GetAll() =>
            _dbContext.Query<Entities.Master.UpdateDetails>()
            .ProjectTo<UpdateDetails>(_mapper.ConfigurationProvider)
            .ToList();

        public void Remove(Guid id)
        {
            var entityToRemove = _dbContext.Find<Entities.Master.UpdateDetails>(id);
            if (entityToRemove != null)
            {
                _dbContext.Remove(entityToRemove);
            }
        }
    }
}
