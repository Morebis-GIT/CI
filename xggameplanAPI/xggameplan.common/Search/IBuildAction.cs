namespace xggameplan.common.Search
{
    public interface IBuildAction<out TResult>
    {
        TResult Build();
    }
}
