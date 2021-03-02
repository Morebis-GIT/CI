using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.AWS;
using xggameplan.AutoBooks.Abstractions;
using AutoBookDomainObject = ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects.AutoBook;

namespace xggameplan.TestEnv.AutoBook
{
    public class TestEnvironmentAWSAutoBooks : AWSAutoBooks
    {
        private readonly IAutoBooksTestHandler _autoBookTestHandler;

        public TestEnvironmentAWSAutoBooks(IRepositoryFactory repositoryFactory, IAutoBookRepository autoBookRepository,
            IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository,
            IAWSInstanceConfigurationRepository awsInstanceConfigurationRepository,
            IAuditEventRepository auditEventRepository, AutoBookSettings autoBooksSettings,
            IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook> autoBooksApi, IAutoBooksTestHandler autoBookTestHandler) : base(
            repositoryFactory, autoBookRepository, autoBookInstanceConfigurationRepository,
            awsInstanceConfigurationRepository, auditEventRepository, autoBooksSettings, autoBooksApi, null)
        {
            _autoBookTestHandler = autoBookTestHandler;
        }

        protected override IAutoBookAPI CreateAutoBookApi(AutoBookDomainObject autoBook)
        {
            return new AWSAutoBookAPIStub(autoBook, _autoBookTestHandler);
        }
    }
}
