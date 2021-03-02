using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class TaskInstanceRepository : ITaskInstanceRepository
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskInstanceRepository(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(TaskInstance taskInstance)
        {
            var taskInstanceId = (taskInstance ?? throw new ArgumentNullException(nameof(taskInstance))).Id;
            var entity = _dbContext.Query<Entities.Master.TaskInstance>()
                .Include(e => e.Parameters).FirstOrDefault(x => x.Id == taskInstanceId);
            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<Entities.Master.TaskInstance>(taskInstance),
                    post =>
                    {
                        post.MapTo(taskInstance);
                    }, _mapper);
            }
            else
            {
                _mapper.Map(taskInstance, entity);
                _dbContext.Update(_mapper.Map<Entities.Master.TaskInstance>(entity),
                    post => post.MapTo(taskInstance), _mapper);
            }
        }

        public TaskInstance Get(Guid id) =>
            _mapper.Map<TaskInstance>(
                _dbContext.Find<Entities.Master.TaskInstance>(new object[] { id }, find => find.IncludeCollection(e => e.Parameters))
            );


        public List<TaskInstance> GetAll() =>
            _dbContext.Query<Entities.Master.TaskInstance>()
                .Include(x => x.Parameters)
                .ProjectTo<TaskInstance>(_mapper.ConfigurationProvider)
                .ToList();

        public void Update(TaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                throw new ArgumentNullException(nameof(taskInstance));
            }

            var entity = _dbContext.Find<Entities.Master.TaskInstance>(new object[] { taskInstance.Id },
                post =>
                {
                    post.IncludeCollection(e => e.Parameters);
                });

            if (entity != null)
            {
                _mapper.Map(taskInstance, entity);
                _dbContext.Update(entity, post => post.MapTo(taskInstance), _mapper);
            }
        }

        public void Remove(Guid id)
        {
            var entity = _dbContext.Find<Entities.Master.TaskInstance>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
