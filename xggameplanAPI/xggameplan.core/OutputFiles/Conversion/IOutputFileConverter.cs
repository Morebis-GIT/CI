namespace xggameplan.OutputFiles.Conversion
{
    /// <summary>
    /// Interface for converting AutoBook file to local format.
    /// </summary>
    public interface IOutputFileConverter
    {        
        void Convert(string autoBookFile, string localFile);
    }
}
