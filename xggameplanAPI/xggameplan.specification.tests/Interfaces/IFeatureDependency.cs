using BoDi;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IFeatureDependency
    {
        void Register(IObjectContainer objectContainer);
    }
}
