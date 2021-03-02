using System;
using System.IO;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.common.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SqlServerPreviewFileStorage<TPreviewParentEntity>
        where TPreviewParentEntity : class, IPreviewable
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public SqlServerPreviewFileStorage(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private TPreviewParentEntity GetEntityById(int entityId) =>
             _dbContext.Find<TPreviewParentEntity>(new object[] { entityId }, pa => pa.IncludeReference(x => x.Preview));

        public void SetPreviewFile(int entityId, PreviewFile previewFile, Stream previewFileStream)
        {
            var entity = GetEntityById(entityId);
            if (entity != null)
            {
                entity.Preview = null;
                entity.Preview = _mapper.Map<Entities.Master.PreviewFile>(previewFile);
                entity.Preview.SetContentBytes(previewFileStream.ToByteArray());
                _dbContext.Update(entity, post =>
                {
                    //post.MapTo( entity.Preview );
                }, _mapper);
            }
        }

        public PreviewFile GetPreviewFile(int entityId, out Stream previewFileStream)
        {
            var entity = GetEntityById(entityId);
            if (entity == null || entity.Preview == null)
            {
                previewFileStream = null;
                return null;
            }
            previewFileStream = new MemoryStream(entity.Preview.Content);
            return _mapper.Map<PreviewFile>(entity.Preview);
        }

        public void DeletePreviewFile(int entityId)
        {
            var entity = GetEntityById(entityId);
            if (entity == null)
            {
                throw new ArgumentException($"{entityId} preview is not found");
            }
            _dbContext.Remove(entity.Preview);
        }

        public void Flush() => _dbContext.SaveChanges();

        public byte[] GetContent(int entityId)
        {
            var entity = GetEntityById(entityId);
            if (entity is null || entity.Preview is null)
            {
                return null;
            }
            else
            {
                return entity.Preview.Content;
            }
        }
    }
}
