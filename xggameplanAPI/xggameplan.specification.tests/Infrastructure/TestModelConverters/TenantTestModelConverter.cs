using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.specification.tests.Infrastructure.TestModels;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.TestModelConverters
{
    public class TenantTestModelConverter : ITestModelConverter<Tenant, TenantTestModel>
    {
        public Tenant ConvertToRepositoryModel(TenantTestModel testModel)
        {
            if (testModel == null)
            {
                return null;
            }

            return Tenant.Create(
                testModel.Id,
                testModel.Name,
                testModel.DefaultTheme,
                testModel.TenantDb
            );
        }

        public TenantTestModel ConvertToTestModel(Tenant repositoryModel)
        {
            if (repositoryModel == null)
            {
                return null;
            }

            return new TenantTestModel
            {
                Id = repositoryModel.Id,
                Name = repositoryModel.Name,
                DefaultTheme = repositoryModel.DefaultTheme,
                TenantDb = repositoryModel.TenantDb
            };
        }
    }
}
