namespace xggameplan.OutputFiles.Filtering
{
    /// <summary>
    /// Interface for filtering output file
    /// </summary>
    internal interface IOutputFileFilter<T>
    {
        void Filter(T filterSettings);
    }
}
