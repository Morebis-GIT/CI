using System;
using xggameplan.cloudaccess.Business;
using xggameplan.cloudaccess.Business.S3;
using xggameplan.Model;

namespace xggameplan.cloudaccess.Factory
{
    public class CloudStorageFactory : IFactory
    {
        private readonly CloudConfig _config;

        public CloudStorageFactory(CloudConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// get the engine type s3/azure
        /// </summary>
        /// <returns></returns>
        public ICloudStorage GetEngine()
        {
            switch (_config.StorageType)
            {
                case CloudStorageType.S3:
                    return new S3Business(_config.AwsConfig);     // Re-do. have to resolve using DI
                default:
                    throw new NotSupportedException($"'{_config.StorageType}' storage type is not supported.");
            }
        }
    }
}
