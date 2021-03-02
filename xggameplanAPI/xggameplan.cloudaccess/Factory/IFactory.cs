using xggameplan.cloudaccess.Business;

namespace xggameplan.cloudaccess.Factory
{
    public interface IFactory
    {
        ICloudStorage GetEngine();
    }
}