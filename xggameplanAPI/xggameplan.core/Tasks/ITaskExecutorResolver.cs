namespace xggameplan.core.Tasks
{
    public interface ITaskExecutorResolver
    {
        ITaskExecutor Resolve(string taskId);
    }
}
