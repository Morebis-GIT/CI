using xggameplan.Model;

namespace xggameplan.core.Validators
{
    public interface IDataManipulator
    {
        CreatePassModel Manipulate(CreatePassModel command);
        CreateRunModel  Manipulate(CreateRunModel command);
    }
}
