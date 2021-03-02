using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class UserRepository : IUsersRepository
    {
        private const int MaxClauseCount = 2000;

        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly SqlServerPreviewFileStorage<Entities.Master.User> _previewFileStorage;
        private readonly IMapper _mapper;

        public IQueryable<User> Query =>
            _dbContext.Query<Entities.Master.User>().ProjectTo<User>(_mapper.ConfigurationProvider);

        public UserRepository(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _previewFileStorage = new SqlServerPreviewFileStorage<Entities.Master.User>(dbContext, mapper);
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public User GetById(int id)
        {
            var entity = _dbContext.Find<Entities.Master.User>(new object[] { id },
                pa => pa.IncludeReference(x => x.Preview));
            return _mapper.Map<User>(entity);
        }

        public IEnumerable<User> GetByIds(List<int> ids)
        {
            ids = ids.Distinct().ToList();
            var result = new List<User>();
            for (int i = 0, page = 0; i < ids.Count; i += MaxClauseCount, page++)
            {
                var batch = ids.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Master.User>()
                    .Where(u => batch.Contains(u.Id)).ProjectTo<User>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public User GetByEmail(string email) =>
            _dbContext.Query<Entities.Master.User>()
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .SingleOrDefault(e => e.Email == email);

        public void Update(User user)
        {
            var entity = _dbContext.Find<Entities.Master.User>(
                new object[] { (user ?? throw new ArgumentNullException(nameof(user))).Id },
                find => find.IncludeCollection(x => x.UserSettings));
            if (entity != null)
            {
                _mapper.Map(user, entity);
                _dbContext.Update(entity, post => post.MapTo(user), _mapper);
            }
        }

        public void Insert(User user) => _dbContext.Add(_mapper.Map<Entities.Master.User>(user),
                post =>
                {
                    post.MapTo(user);
                }, _mapper);

        public List<User> GetAll() =>
            _dbContext.Query<Entities.Master.User>()
            .ProjectTo<User>(_mapper.ConfigurationProvider)
            .ToList();

        public PreviewFile GetPreviewFile(int entityId, out Stream previewFileStream) =>
           _previewFileStorage.GetPreviewFile(entityId, out previewFileStream);

        public void SetPreviewFile(int entityId, PreviewFile previewFile, Stream previewFileStream) =>
            _previewFileStorage.SetPreviewFile(entityId, previewFile, previewFileStream);

        public void DeletePreviewFile(int entityId) => _previewFileStorage.DeletePreviewFile(entityId);

        public void Flush() => _previewFileStorage.Flush();

        public byte[] GetContent(int entityId)
        {
            return _previewFileStorage.GetContent(entityId);
        }
    }
}
