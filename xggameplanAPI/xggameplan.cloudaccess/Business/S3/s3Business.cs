using System;
using System.Collections.Generic;
using xggameplan.cloudaccess.Engine.S3;
using xggameplan.Model;

namespace xggameplan.cloudaccess.Business.S3
{
    public class S3Business : ICloudStorage
    {
        private static volatile IS3 _s3Obj;
        private readonly AwsConfiguration _config;

        public S3Business(AwsConfiguration config)
        {
            _config = config;
        }

        protected IS3 S3Obj
        {
            get
            {
                _s3Obj = _s3Obj ?? new Engine.S3.S3(_config);// Re-do. have to resolve using DI
                return _s3Obj;
            }
        }

        /// <summary>
        /// download file from S3
        /// </summary>
        /// <param name="s3Input"></param>
        /// <returns></returns>
        public string Download(S3DownloadComment s3Input)
        {
            try
            {
                if (s3Input == null ||
                     string.IsNullOrWhiteSpace(s3Input.BucketName) ||
                    string.IsNullOrWhiteSpace(s3Input.FileName) ||
                    string.IsNullOrWhiteSpace(s3Input.LocalFileFolder))
                {
                    throw new ArgumentNullException(nameof(s3Input));
                }

                return S3Obj.DownloadFile(s3Input.BucketName, s3Input.FileName, s3Input.LocalFileFolder);
            }
            catch (Exception ex)
            {
                var errormsg = $"BucketName = {s3Input.BucketName} FileName = { s3Input.FileName } DestinationFolder = {s3Input.LocalFileFolder}";
                throw new Exception("Exception during s3 download " + errormsg + ex);
            }
        }

        /// <summary>
        /// Upload file to S3
        /// </summary>
        /// <param name="s3Input"></param>
        /// <returns></returns>
        public bool Upload(S3UploadComment s3Input)
        {
            try
            {
                if (s3Input == null
                    || string.IsNullOrWhiteSpace(s3Input.BucketName)
                    || string.IsNullOrWhiteSpace(s3Input.DestinationFilePath))

                {
                    throw new ArgumentNullException();
                }

                //Check if bucket available or not
                if (!S3Obj.IsBucketExist(s3Input.BucketName))
                {
                    _ = S3Obj.CreateBucket(s3Input.BucketName);
                }
                // if bucket available then file exist or not
                if (S3Obj.IsFileExist(s3Input.BucketName, s3Input.DestinationFilePath))
                {
                    throw new Exception("Already file exist");
                }
                if (!string.IsNullOrWhiteSpace(s3Input.SourceFilePath) &&
                    System.IO.File.Exists(s3Input.SourceFilePath))
                {
                    return S3Obj.CreateFile(s3Input.SourceFilePath, s3Input.BucketName, s3Input.DestinationFilePath); // Upload file from local disk to S3
                }
                if (s3Input.FileStream != null)
                {
                    return S3Obj.CreateFile(s3Input.FileStream, s3Input.BucketName, s3Input.DestinationFilePath); // Upload file from memory stream to S3
                }

                throw new ArgumentNullException();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Determines if file exists
        /// </summary>
        /// <param name="s3Input"></param>
        /// <returns></returns>
        public bool FileExists(S3FileComment s3File)
        {
            bool exists = false;
            try
            {
                if (s3File == null
                    || string.IsNullOrWhiteSpace(s3File.BucketName)
                    || string.IsNullOrWhiteSpace(s3File.FileNameWithPath))
                {
                    throw new ArgumentNullException();
                }

                //Check if bucket available or not
                if (S3Obj.IsBucketExist(s3File.BucketName))
                {
                    exists = S3Obj.IsFileExist(s3File.BucketName, s3File.FileNameWithPath);
                }
            }
            catch
            {
                throw;
            }
            return exists;
        }

        /// <summary>
        /// Deletes file if exists
        /// </summary>
        /// <param name="s3Input"></param>
        /// <returns></returns>
        public void DeleteFile(S3FileComment s3File)
        {
            try
            {
                if (s3File == null
                    || string.IsNullOrWhiteSpace(s3File.BucketName)
                    || string.IsNullOrWhiteSpace(s3File.FileNameWithPath))
                {
                    throw new ArgumentNullException();
                }

                //Check if bucket available or not
                if (S3Obj.IsBucketExist(s3File.BucketName))
                {
                    if (S3Obj.IsFileExist(s3File.BucketName, s3File.FileNameWithPath))
                    {
                        _ = S3Obj.DeleteFile(s3File.BucketName, s3File.FileNameWithPath);
                    }
                    else
                    {
                        throw new System.IO.FileNotFoundException(string.Format("File {0} does not exist", s3File.FileNameWithPath));
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteFilesByPrefix(S3DeleteByPrefixComment s3Prefix)
        {
            if (s3Prefix is null)
            {
                throw new ArgumentNullException(nameof(s3Prefix));
            }

            if (string.IsNullOrWhiteSpace(s3Prefix.BucketName))
            {
                throw new ArgumentNullException(
                        nameof(s3Prefix),
                        $"{nameof(s3Prefix.BucketName)} cannot be null"
                    );
            }

            if (string.IsNullOrWhiteSpace(s3Prefix.FilePrefix))
            {
                throw new ArgumentNullException(
                        nameof(s3Prefix),
                        $"{nameof(s3Prefix.FilePrefix)} cannot be null"
                    );
            }

            var result = S3Obj.GetFiles(s3Prefix.BucketName, s3Prefix.FilePrefix) ?? new List<string>();

            foreach (var fileKey in result)
            {
                DeleteFile(
                    new S3FileComment()
                    {
                        BucketName = s3Prefix.BucketName,
                        FileNameWithPath = fileKey
                    }
                );
            }
        }

        public string GetPreSignedUrl(S3FileComment s3File) =>
            _s3Obj.GetPreSignedUrl(s3File.BucketName, s3File.FileNameWithPath);
    }
}
