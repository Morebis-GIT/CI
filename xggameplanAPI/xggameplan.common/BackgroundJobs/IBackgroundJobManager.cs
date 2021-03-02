namespace xggameplan.common.BackgroundJobs
{
    public interface IBackgroundJobManager
    {
        void StartJob<TBackgroundJob>(params IBackgroundJobParameter[] parameters)
            where TBackgroundJob : class, IBackgroundJob;
    }
}
