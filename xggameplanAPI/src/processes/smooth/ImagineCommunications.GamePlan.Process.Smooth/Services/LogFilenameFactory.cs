using System;
using System.Globalization;
using System.IO;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class LogFilenameFactory
    {
        private readonly string _filenameRoot;

        public LogFilenameFactory(
            string salesAreaName,
            string rootFolder,
            DateTime processingWhen
            )
        {
            _filenameRoot = Factory(
                salesAreaName,
                rootFolder,
                processingWhen);
        }

        public LogFilenameFactory(
            Guid runId,
            string rootFolder,
            DateTime processingWhen
            )
        {
            _filenameRoot = Factory(
                runId.ToString(),
                rootFolder,
                processingWhen);
        }

        private string Factory(
            string salesAreaNameOrRunId,
            string rootFolder,
            DateTime processingWhen
            )
        {
            if (salesAreaNameOrRunId is null)
            {
                throw new ArgumentNullException(nameof(salesAreaNameOrRunId));
            }

            if (rootFolder is null)
            {
                throw new ArgumentNullException(nameof(rootFolder));
            }

            var _processorDateTimeToString = processingWhen.ToString(
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

            return Path.Combine(
                rootFolder,
                "Logs",
                $"{_processorDateTimeToString}.{salesAreaNameOrRunId}.");
        }

        public enum LogFileType
        {
            Unknown,
            Spots,
            SpotActions,
            BestBreak,
            Programmes,
            SmoothConfiguration
        }

        public string FilenameFor(LogFileType type)
        {
            switch (type)
            {
                case LogFileType.Unknown:
                    goto case default;

                case LogFileType.Spots:
                    return _filenameRoot + "smooth.txt";

                case LogFileType.SpotActions:
                    return _filenameRoot + "smooth_spot_actions.txt";

                case LogFileType.BestBreak:
                    return _filenameRoot + "smooth_best_break.txt";

                case LogFileType.Programmes:
                    return _filenameRoot + "smooth_programmes.txt";

                case LogFileType.SmoothConfiguration:
                    return _filenameRoot + "smooth_configuration.json";

                default:
                    throw new ArgumentException(nameof(type));
            }
        }
    }
}
