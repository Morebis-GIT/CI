using System;
using System.IO;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.common.Types;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.Model;

namespace xggameplan.core.Landmark.PayloadProviders
{
    public class CloudLandmarkAutoBookPayloadProvider : LandmarkAutoBookPayloadProviderBase
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly AWSSettings _awsSettings;

        public CloudLandmarkAutoBookPayloadProvider(ICloudStorage cloudStorage, AWSSettings awsSettings, RootFolder baseLocalFolder) : base(baseLocalFolder)
        {
            _cloudStorage = cloudStorage;
            _awsSettings = awsSettings;
        }

        protected override string DownloadScenarioInputFiles(Guid scenarioId, string localFolder, string localInputFolder)
        {
            return DownloadFile(scenarioId, localFolder, localInputFolder);
        }

        protected override string DownloadRunInputFiles(Guid runId, string localFolder, string localInputFolder)
        {
            return DownloadFile(runId, localFolder, localInputFolder);
        }

        private string DownloadFile(Guid fileId, string localFolder, string localInputFolder)
        {
            var zipFileName = $"{fileId.ToString()}.zip";
            var localZipFile = Path.Combine(localInputFolder, zipFileName);
            var zipFileKey = $@"input/{zipFileName}";

            _cloudStorage.Download(new S3DownloadComment
            {
                BucketName = _awsSettings.S3Bucket,
                FileName = zipFileKey,
                LocalFileFolder = localFolder
            });

            return localZipFile;
        }
    }
}
