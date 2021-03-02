using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.cloudaccess.Business;
using xggameplan.AutoBooks.AWS;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests cloud
    /// </summary>
    internal class CloudTest : ISystemTest
    {
        private ICloudStorage _cloudStorage;
        private AWSSettings _awsSettings;
        private const string _category = "Cloud";

        public CloudTest(ICloudStorage cloudStorage, AWSSettings awsSettings)
        {
            _cloudStorage = cloudStorage;
            _awsSettings = awsSettings;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return true;
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();
            Model.S3UploadComment upload = null;
            Model.S3DownloadComment download = null;
            bool success = false;

            // Upload test file
            Guid id = Guid.NewGuid();
            string localTempFolder = System.Web.Hosting.HostingEnvironment.MapPath("/Temp");
            string localOutputFolder = System.Web.Hosting.HostingEnvironment.MapPath("/Output");
            string localTempFile = Path.Combine(localTempFolder, string.Format("{0}.tmp", id));
            int action = 0;         // 1=Upload, 2=Download

            try
            {
                // Create local file
                Directory.CreateDirectory(localTempFolder);
                File.WriteAllText(localTempFile, new String('X', 1024 * 10));

                // Upload to cloud
                action = 1;
                upload = new Model.S3UploadComment()
                {
                    BucketName = _awsSettings.S3Bucket,
                    DestinationFilePath = string.Format("input/Test-{0}.tmp", id.ToString()),
                    SourceFilePath = localTempFile
                };
                _cloudStorage.Upload(upload);

                // Download from cloud
                action = 2;
                download = new Model.S3DownloadComment()
                {
                    BucketName = _awsSettings.S3Bucket,
                    FileName = string.Format("input/Test-{0}.tmp", id.ToString()),
                    LocalFileFolder = localOutputFolder
                };
                _cloudStorage.Download(download);
                success = true;
            }
            catch(System.Exception exception)
            {
                switch (action)
                {
                    case 0:
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error creating file for testing the cloud storage upload/download: {0}. It will cause runs to fail.", exception.Message), ""));
                        break;
                    case 1:
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error uploading file to cloud storage: {0}. It will cause runs to fail.", exception.Message), ""));
                        break;
                    case 2:
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error downloading file from cloud storage: {0}. It will cause runs to fail.", exception.Message), ""));
                        break;
                }
            }
            finally
            {
                // Delete local file
                try
                {
                    if (File.Exists(localTempFile))
                    {
                        File.Delete(localTempFile);
                    }
                }
                catch { };  // Ignore

                // Delete cloud file
                try
                {
                    if (upload != null)
                    {
                        Model.S3FileComment delete = new Model.S3FileComment()
                        {
                            BucketName = _awsSettings.S3Bucket,
                            FileNameWithPath = upload.DestinationFilePath
                        };
                        _cloudStorage.DeleteFile(delete);
                    }
                }
                catch { };      // Ignore

                if (success)
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Cloud test OK", ""));
                }
            }
            return results;
        }
    }
}
