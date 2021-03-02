using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;
using Raven.Client.Linq;
using xggameplan.common.Utilities;
using xggameplan.core.Repository;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenUsersRepository : IUsersRepository, IUserSettingsService
    {
        private readonly IRavenMasterDbContext _dbContext;
        private readonly RavenPreviewFileStorage _previewFileStorage;

        public RavenUsersRepository(IRavenMasterDbContext dbContext, RavenPreviewFileStorage previewFileStorage)
        {
            _dbContext = dbContext;
            _previewFileStorage = previewFileStorage;
        }

        public User GetById(int id) => FindById(id);

        public IEnumerable<User> GetByIds(List<int> ids)
        {
            ids = ids.Distinct().ToList();
            var users = new List<User>();
            int page = 0;

            do
            {
                List<int> batch = ListUtilities.GetPageItems(ids, 1000, page++);

                if (batch.Count > 0)
                {
                    users.AddRange(_dbContext.Query<User>().Where(u => u.Id.In(batch)));
                }
                else
                {
                    return users;
                }
            } while (true);
        }

        public IQueryable<User> Query => _dbContext.Query<User>();

        public List<User> GetAll() => _dbContext.Query<User>().ToList();

        public User GetByEmail(string email) => _dbContext.Query<User>().Where(x => x.Email == email).SingleOrDefault();

        public void Insert(User user) => _dbContext.Add(user);

        public void Update(User user) => _dbContext.Update(user);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void SetPreviewFile(int userId, PreviewFile previewFile, Stream previewFileStream)
        {
            var user = FindById(userId);
            if (previewFileStream == null || user == null)
            {
                return;
            }
            var existingPreviewId = user.Preview?.Id;
            if (existingPreviewId != null)
            {
                _previewFileStorage.DeletePreviewFile(existingPreviewId);
            }
            user.Preview = previewFile;
            _previewFileStorage.UploadPreviewFile(previewFile, previewFileStream);
        }

        public PreviewFile GetPreviewFile(int entityId, out Stream previewFileStream)
        {
            var previewFile = FindById(entityId)?.Preview;
            previewFileStream = null;
            if (previewFile?.Id == null)
            {
                return null;
            }

            previewFileStream = _previewFileStorage.GetPreviewFileStream(previewFile.Id);
            return previewFile;
        }

        public void DeletePreviewFile(int entityId)
        {
            var entity = FindById(entityId);
            var previewFileId = entity.Preview?.Id;
            if (previewFileId != null)
            {
                entity.Preview = null;
                _dbContext.Update(entity);
                _previewFileStorage.DeletePreviewFile(previewFileId);
            }
        }

        public void Flush() =>
            _dbContext.SaveChanges(); /*?*/

        private User FindById(int entityId) =>
            _dbContext.Find<User>(entityId);

        public string GetUserSetting(int userId, string key)
        {
            var settings = FindById(userId)?.UserSettings;
            if (settings == null)
            {
                return null;
            }
            string sValue = null;
            settings.TryGetValue(key, out sValue);
            return sValue;
        }

        public void SetUserSetting(int userId, string key, string value)
        {
            var entity = FindById(userId);
            if (entity != null)
            {
                entity.UserSettings[key] = value;
                _dbContext.Update(entity);
            }
        }

        public void DeleteUserSetting(int userId, string key)
        {
            var entity = FindById(userId);
            if (entity != null && entity.UserSettings.ContainsKey(key))
            {
                entity.UserSettings.Remove(key);
                _dbContext.Update(entity);
            }
        }

        public byte[] GetContent(int entityId)
        {
            PreviewFile previewFile = FindById(entityId)?.Preview;
            if (previewFile is null)
            {
                return null;
            }
            else
            {
                return previewFile.GetContent();
            }
        }
    }
}
