namespace ImagineCommunications.GamePlan.ReportSystem.Formaters
{
    public interface IFormatter
    {
        string Format(object value);
    }

    public interface IFormatter<in T>: IFormatter
    {
        string Format(T value);
    }
}
