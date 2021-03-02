using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.TestModelConverters
{
    public class ReferenceTestModelConverter<TRepositoryModel> :
        ITestModelConverter<TRepositoryModel, TRepositoryModel>
        where TRepositoryModel : class
    {
        public TRepositoryModel ConvertToRepositoryModel(TRepositoryModel modelDto)
        {
            return modelDto;
        }

        public TRepositoryModel ConvertToTestModel(TRepositoryModel model)
        {
            return model;
        }
    }
}
