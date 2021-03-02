using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.cloudaccess.Business.S3;
using xggameplan.cloudaccess.Factory;
using xggameplan.cloudaccess.Interfaces;
using xggameplan.Model;

namespace xggameplan.core.DependencyInjection
{
    /// <summary>
    /// Autofac module for cloud (AWS/Azure etc)
    /// </summary>
    public class CloudModule : Module
    {
        private readonly IConfiguration _configuration;

        public CloudModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (!Enum.TryParse<CloudStorageType>(_configuration["CloudAccessType:Type"] ?? "S3", out var storageType))
            {
                storageType = CloudStorageType.S3;
            }

            switch (storageType)
            {
                case CloudStorageType.S3:
                    _ = builder.RegisterInstance(_configuration.GetSection("AWS").Get<AwsConfiguration>());
                    _ = builder.RegisterInstance(_configuration.GetSection("AutoBooks:AWSSettings").Get<AWSSettings>())
                        .AsSelf().As<IS3BucketSettings>();
                    _ = builder.RegisterType<S3BucketStorage>().As<ICloudStorageV2>().SingleInstance();
                    break;
            }

            _ = builder.Register(context => new CloudConfig
            {
                StorageType = storageType,
                AwsConfig = storageType == CloudStorageType.S3 ? context.Resolve<AwsConfiguration>() : null
            }).SingleInstance().AsSelf();

            _ = builder.RegisterType<CloudStorageFactory>().As<IFactory>().SingleInstance();
            _ = builder.Register(context => context.Resolve<IFactory>().GetEngine()).As<ICloudStorage>().InstancePerDependency();
        }
    }
}
