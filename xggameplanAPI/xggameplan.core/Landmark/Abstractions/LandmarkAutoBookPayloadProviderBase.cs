using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using xggameplan.common.Types;
using xggameplan.Model;

namespace xggameplan.core.Landmark.Abstractions
{
    /// <inheritdoc />
    public abstract class LandmarkAutoBookPayloadProviderBase : ILandmarkAutoBookPayloadProvider
    {
        /// <summary>
        /// The names and mandatory flags for files that should be added to payload.
        /// </summary>
        public static readonly Dictionary<string, bool> RunLevelFilesConfig = new Dictionary<string, bool>
            {
                { "v_rs_list.xml", false },
                { "v_isr_list.xml", false },
                { "v_param_date_list.xml", false },
                { "v_fail_type_list.xml", false },
                { "v_camp_incl_list.xml", false},
                { "v_abdg_list.xml", false },
                { "v_camp_abdg_list.xml", false },
                { "v_invs_excl_list.xml", false }
            };

        private readonly RootFolder _baseLocalFolder;

        protected LandmarkAutoBookPayloadProviderBase(RootFolder baseLocalFolder) => _baseLocalFolder = baseLocalFolder;

        /// <inheritdoc />
        public IEnumerable<LandmarkInputFilePayload> GetFiles(Guid runId, Guid scenarioId)
        {
            var localFolder = Path.Combine(_baseLocalFolder, "Output", @"landmark", scenarioId.ToString());
            var localInputFolder = Path.Combine(localFolder, "input");
            var localOutputFolder = Path.Combine(localFolder, "output");

            if (Directory.Exists(localFolder))
            {
                Directory.Delete(localFolder, true);
            }

            Directory.CreateDirectory(localInputFolder);
            Directory.CreateDirectory(localOutputFolder);

            try
            {
                var localScenarioZipFile = DownloadScenarioInputFiles(scenarioId, localFolder, localInputFolder);
                var localRunZipFile = DownloadRunInputFiles(runId, localFolder, localInputFolder);
                var result = ExtractFiles(localRunZipFile, localScenarioZipFile, localOutputFolder).Select(file =>
                    new LandmarkInputFilePayload
                    {
                        FileName = file,
                        Payload = File.ReadAllText(Path.Combine(localOutputFolder, file))
                    })
                    .ToList();

                return result;
            }
            finally
            {
                Directory.Delete(localFolder, true);
            }
        }

        /// <summary>
        /// Downloads input files to the local folder for further processing
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="localFolder"></param>
        /// <param name="localInputFolder"></param>
        /// <returns>path to scenario input zip file</returns>
        protected abstract string DownloadScenarioInputFiles(Guid scenarioId, string localFolder, string localInputFolder);

        protected abstract string DownloadRunInputFiles(Guid runId, string localFolder, string localInputFolder);

        /// <summary>
        /// Reads Run and Scenario zip files and extracts files
        /// </summary>
        /// <param name="localRunZipFile"></param>
        /// <param name="localScenarioZipFile"></param>
        /// <param name="localOutputFolder"></param>
        /// <returns></returns>
        private static IEnumerable<string> ExtractFiles(string localRunZipFile, string localScenarioZipFile, string localOutputFolder)
        {
            var scenarioLevelExtractedFiles = ExtractAllEntities(localScenarioZipFile, localOutputFolder);
            var runLevelExtractedFiles = FindEntitiesToExtract(RunLevelFilesConfig, localRunZipFile, localOutputFolder);

            return scenarioLevelExtractedFiles.Concat(runLevelExtractedFiles).Select(e => e.Name).ToList();
        }

        /// <summary>
        /// Finds entities to extract according to passed files configuration.
        /// </summary>
        /// <param name="filesConfig">The files configuration.</param>
        /// <param name="localZipFile">The local zip file.</param>
        /// <param name="localOutputFolder">The local output folder.</param>
        /// <returns>The list of extracted entities</returns>
        /// <exception cref="InvalidOperationException">Mandatory file {file.Key} not found</exception>
        private static List<ZipArchiveEntry> FindEntitiesToExtract(Dictionary<string, bool> filesConfig, string localZipFile, string localOutputFolder)
        {
            var entriesToExtract = new List<ZipArchiveEntry>();
            using (var zipArchive = ZipFile.OpenRead(localZipFile))
            {
                var entries = zipArchive.Entries.ToDictionary(e => e.Name, e => e);
                foreach (var file in filesConfig)
                {
                    if (entries.TryGetValue(file.Key, out var zipEntry))
                    {
                        entriesToExtract.Add(zipEntry);
                    }
                    else if (file.Value)
                    {
                        throw new InvalidOperationException($"Mandatory file {file.Key} not found");
                    }
                }

                entriesToExtract.ForEach(e => e.ExtractToFile(Path.Combine(localOutputFolder, e.Name)));
                return entriesToExtract;
            }
        }

        /// <summary>
        /// Extracts all entities from zip archive to output folder.
        /// </summary>
        /// <param name="localZipFile">The local zip file.</param>
        /// <param name="localOutputFolder">The local output folder.</param>
        /// <returns></returns>
        private static List<ZipArchiveEntry> ExtractAllEntities(string localZipFile, string localOutputFolder)
        {
            using (var zipArchive = ZipFile.OpenRead(localZipFile))
            {
                var entries = zipArchive.Entries.ToList();

                entries.ForEach(e => e.ExtractToFile(Path.Combine(localOutputFolder, e.Name)));

                return entries;
            }
        }
    }
}
