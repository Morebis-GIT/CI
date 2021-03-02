using System.IO;
using System.Linq;
using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using Serilog;
using xggameplan.common.Extensions;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class PreviewableEntityMigrationDocumentHandler<TPreviewableDomainModel, TIdentityEntityModel, TPreviewRepository> :
        RavenToSqlIdentityMigrationDocumentHandler<TPreviewableDomainModel, TIdentityEntityModel>
            where TPreviewableDomainModel : class
            where TIdentityEntityModel : class
            where TPreviewRepository : class, IPreviewFileStorage
    {

        public PreviewableEntityMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex,
            ILogger logger) : base(containerIndex, logger)
        {
        }


        private int GetDestinationModelId(TPreviewableDomainModel destinationModel) =>
           (int)typeof(TPreviewableDomainModel).GetProperty("Id").GetValue(destinationModel);

        private void SetDestinationModelPreview(TPreviewableDomainModel destinationModel, object value) =>
           typeof(TPreviewableDomainModel).GetProperty("Preview").SetValue(destinationModel, value);


        protected override void AdjustModel(TPreviewableDomainModel model) {
            base.AdjustModel(model);
            var ravenEntityPreviewStorage = SourceContainer.Resolve<TPreviewRepository>();
            var destinationModelId = GetDestinationModelId(model);
            Stream stream;
            var previewFile = ravenEntityPreviewStorage.GetPreviewFile(destinationModelId, out stream);
            if (previewFile != null && stream != null)
            {
                var location = previewFile.Id.Split('/').FirstOrDefault() ?? "unidentified";
                previewFile = PreviewFile.Create(previewFile.FileName, previewFile.ContentType, previewFile.ContentLength, location);
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    _ = ms.Seek(0, SeekOrigin.Begin);
                    previewFile.SetContent(ms.ToByteArray());
                    stream.Dispose();
                }
                SetDestinationModelPreview(model, previewFile);
            }

            // Fix situation when referenced file can not be retriewed
            if (previewFile == null)
            {
                SetDestinationModelPreview(model, null);
            }
        }
    }
}
