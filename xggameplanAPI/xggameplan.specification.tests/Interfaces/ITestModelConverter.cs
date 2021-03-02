namespace xggameplan.specification.tests.Interfaces
{
    public interface ITestModelConverter<TRepositoryModel, TTestModel>
        where TRepositoryModel : class
        where TTestModel : class
    {
        TRepositoryModel ConvertToRepositoryModel(TTestModel testModel);

        TTestModel ConvertToTestModel(TRepositoryModel repositoryModel);
    }
}
