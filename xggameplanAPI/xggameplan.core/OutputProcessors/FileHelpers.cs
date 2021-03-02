using System;
using System.IO;
using System.Linq;

namespace xggameplan.core.OutputProcessors
{
    public static class FileHelpers
    {
        public static string GetPathToFileIfExists(string baseDirectory, string fileNameWithoutExtention) =>
            Directory.GetFiles(baseDirectory, "*.*", SearchOption.AllDirectories)
                .FirstOrDefault(p => 
                        String.Equals(
                                Path.GetFileNameWithoutExtension(p), 
                                fileNameWithoutExtention, 
                                StringComparison.OrdinalIgnoreCase
                            )
                    );
    }
}
