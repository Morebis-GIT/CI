using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;

namespace xggameplan.OutputFiles
{
    /// <summary>
    /// Handles local storage of output files obtained from repository
    /// </summary>
    public class LocalOutputFiles
    {
        private IResultsFileRepository _resultsFileRepository;
        private string _folder;

        public LocalOutputFiles(IResultsFileRepository resultsFileRepository, string folder)
        {
            _resultsFileRepository = resultsFileRepository;
            _folder = folder;
        }

        /// <summary>
        /// Gets output files for scenario, stores them locally
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="compressed"></param>
        /// <param name="outputFiles"></param>        
        public void GetOutputFiles(Guid scenarioId, bool compressed, List<OutputFile> outputFiles)
        {
            string scenarioDataFolder = GetScenarioDataFolder(scenarioId);
            Directory.CreateDirectory(scenarioDataFolder);

            // Get all output files, use local cache if possible
            foreach(OutputFile outputFile in outputFiles)
            {
                string localFile = Path.Combine(scenarioDataFolder, GetOutputFileName(outputFile, compressed));
                if (!System.IO.File.Exists(localFile))
                {
                    _resultsFileRepository.Get(scenarioId, outputFile.FileId, compressed, scenarioDataFolder);
                }                
            }
        }    

        public string GetOutputFilePath(Guid scenarioId, bool compressed, OutputFile outputFile)
        {
            string scenarioDataFolder = GetScenarioDataFolder(scenarioId);
            return Path.Combine(scenarioDataFolder, GetOutputFileName(outputFile, compressed));
        }  
        
        private string GetOutputFileName(OutputFile outputFile, bool compressed)
        {
            return compressed ? string.Format("{0}{1}", outputFile.FileId, ".zip") : outputFile.FileId;
        }

        /// <summary>
        /// Deletes all output files for scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        public void DeleteOutputFiles(Guid scenarioId)
        {
            string scenarioRootFolder = GetScenarioRootFolder(scenarioId);
            if (Directory.Exists(scenarioRootFolder))
            {
                Directory.Delete(scenarioRootFolder, true);
            }
        }

        private string GetScenarioRootFolder(Guid scenarioId)
        {
            return string.Format(@"{0}\{1}", _folder, scenarioId);
        }

        private string GetScenarioDataFolder(Guid scenarioId)
        {
            return string.Format(@"{0}\Data", GetScenarioRootFolder(scenarioId));
        }
    }
}
