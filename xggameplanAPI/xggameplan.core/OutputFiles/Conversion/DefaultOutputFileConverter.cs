using System;
using System.IO;

namespace xggameplan.OutputFiles.Conversion
{
    /// <summary>
    /// Default output file converter, does nothing.
    /// </summary>
    public class DefaultOutputFileConverter : IOutputFileConverter
    {       
        public void Convert(string autoBookFile, string localFile)
        {
            // Only copy file if different
            if (!autoBookFile.Equals(localFile, StringComparison.InvariantCultureIgnoreCase))
            {                
                File.Copy(autoBookFile, localFile, true);
            }                
        }
    }
}
