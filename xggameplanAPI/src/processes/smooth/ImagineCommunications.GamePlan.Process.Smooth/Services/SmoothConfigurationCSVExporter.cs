using System.IO;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Exports Smooth configuration to CSV
    /// </summary>
    public class SmoothConfigurationCSVExporter
    {
        private readonly SmoothConfiguration _smoothConfiguration;
        private readonly char _delimiter;

        public SmoothConfigurationCSVExporter(SmoothConfiguration smoothConfiguration, char delimiter)
        {
            _smoothConfiguration = smoothConfiguration;
            _delimiter = delimiter;
        }

        /// <summary>
        /// Exports passes and iterations
        /// </summary>
        public void ExportPasses(string file)
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (var writer = new StreamWriter(file, true))
            {
                // Write headers
                writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}",
                    _delimiter, "ID", "PassSequence", "PassType",
                    "Pass-BreakRequests", "Pass-CanSplitMultipartSpots", "Pass-ClientPicked", "Pass-HasMultipartSpots",
                    "Pass-HasProductClashCode", "Pass-HasSpotEndTime", "Pass-Preemptable", "Pass-Sponsored",
                    "Iteration-Sequence",
                    "Iteration-SpotFilter-HasSponsoredSpots", "Iteration-SpotFilter-HasBreakRequest", "Iteration-SpotFilter-HasFIBOrLIBRequest",
                    "Iteration-RequestedBreakPositionRules", "Iteration-RequestedPositionInBreakRules",
                    "Iteration-ProductClashRules",
                    "Iteration-RespectCampaignClash", "Iteration-RespectClashExceptions",
                    "Iteration-RespectRestrictions", "Iteration-RespectSpotTime"));

                int totalItemCount = 0;

                foreach (var pass in _smoothConfiguration.Passes.OrderBy(p => p.Sequence))
                {
                    if (pass is SmoothPassDefault smoothPassDefault)
                    {
                        foreach (var iterationRecord in _smoothConfiguration.IterationRecords
                            .Where(ir =>
                                ir.PassSequences.Contains(smoothPassDefault.Sequence)
                                && ir.PassDefaultIteration != null)
                            )
                        {
                            SmoothPassDefaultIteration passDefaultIteration = iterationRecord.PassDefaultIteration;

                            var breakRequestsString = new StringBuilder();

                            if (smoothPassDefault.BreakRequests == null)
                            {
                                _ = breakRequestsString.Append("Any");
                            }
                            else
                            {
                                foreach (var breakRequest in smoothPassDefault.BreakRequests)
                                {
                                    AddToBreakRequestsString(breakRequestsString, breakRequest);
                                }
                            }

                            totalItemCount++;
                            writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}",
                                _delimiter, totalItemCount, smoothPassDefault.Sequence, "Default",
                                breakRequestsString.ToString(), smoothPassDefault.CanSplitMultipartSpots.ToString(),
                                "True", // ClientPicked
                                smoothPassDefault.HasMultipartSpots == null ? "Any" : smoothPassDefault.HasMultipartSpots.ToString(),
                                smoothPassDefault.HasProductClashCode == null ? "Any" : smoothPassDefault.HasProductClashCode.ToString(),
                                smoothPassDefault.HasSpotEndTime == null ? "Any" : smoothPassDefault.HasSpotEndTime.ToString(),
                                smoothPassDefault.Preemptable == null ? "Any" : smoothPassDefault.Preemptable.ToString(),
                                smoothPassDefault.Sponsored == null ? "Any" : smoothPassDefault.Sponsored.ToString(),
                                passDefaultIteration.Sequence,
                                iterationRecord.SpotsCriteria.HasSponsoredSpots == null ? "Any" : iterationRecord.SpotsCriteria.HasSponsoredSpots.ToString(),
                                iterationRecord.SpotsCriteria.HasBreakRequest == null ? "Any" : iterationRecord.SpotsCriteria.HasBreakRequest.ToString(),
                                iterationRecord.SpotsCriteria.HasFIBORLIBRequests == null ? "Any" : iterationRecord.SpotsCriteria.HasFIBORLIBRequests.ToString(),
                                passDefaultIteration.BreakPositionRules, passDefaultIteration.RequestedPositionInBreakRules,
                                passDefaultIteration.ProductClashRules, passDefaultIteration.RespectCampaignClash,
                                passDefaultIteration.RespectClashExceptions, passDefaultIteration.RespectRestrictions, passDefaultIteration.RespectSpotTime));
                        }
                    }
                    else if (pass is SmoothPassUnplaced smoothPassUnplaced)
                    {
                        foreach (var iterationRecord in _smoothConfiguration.IterationRecords
                            .Where(ir =>
                                ir.PassSequences.Contains(smoothPassUnplaced.Sequence)
                                && ir.PassUnplacedIteration != null)
                            )
                        {
                            SmoothPassUnplacedIteration passUnplacedIteration = iterationRecord.PassUnplacedIteration;

                            totalItemCount++;
                            writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}",
                                _delimiter, totalItemCount, smoothPassUnplaced.Sequence, "Unplaced",
                                "Any", "Any", "Any", "Any", "Any", "Any", "Any", "Any",
                                passUnplacedIteration.Sequence,
                                iterationRecord.SpotsCriteria.HasSponsoredSpots == null ? "Any" : iterationRecord.SpotsCriteria.HasSponsoredSpots.ToString(),
                                iterationRecord.SpotsCriteria.HasBreakRequest == null ? "Any" : iterationRecord.SpotsCriteria.HasBreakRequest.ToString(),
                                iterationRecord.SpotsCriteria.HasFIBORLIBRequests == null ? "Any" : iterationRecord.SpotsCriteria.HasFIBORLIBRequests.ToString(),
                                "", "",
                                passUnplacedIteration.ProductClashRule, passUnplacedIteration.RespectCampaignClash,
                                passUnplacedIteration.RespectClashExceptions, passUnplacedIteration.RespectRestrictions, passUnplacedIteration.RespectSpotTime));
                        }
                    }
                }

                writer.Flush();
            }
        }

        private static void AddToBreakRequestsString(
            StringBuilder breakRequestsString,
            string breakRequest)
        {
            if (breakRequestsString.Length > 0)
            {
                _ = breakRequestsString.Append("; ");
            }

            _ = breakRequestsString.Append(breakRequest ?? "None");
        }

        /// <summary>
        /// Exports best break factor groups
        /// </summary>
        public void ExportBestBreakFactorGroups(string file)
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (var writer = new StreamWriter(file, true))
            {
                // Write headers
                writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}{0}{35}{0}{36}{0}{37}{0}{38}{0}{39}",
                                            _delimiter, "ID", "PassSeq", "GroupID", "GroupName", "GroupSequence", "GroupItem-Sequence",
                                            "Group-SameBreakGroupScoreFactorPriority", "Group-SameBreakGroupScoreFactor", "Group-SameBreakGroupScoreAction",
                                            "SpotFilter-HasSponsoredSpots", "SpotFilter-HasBreakRequest", "SpotFilter-HasFIBOrLIBRequest",
                                            "GroupItem-AllFilterFactorsMustBeZero", "GroupItem-UseZeroScoresInEvaluation", "GroupItem-GroupItemEvaluation",
                                            "FilterFactor-Priority1", "FilterFactor-Factor1",
                                            "FilterFactor-Priority2", "FilterFactor-Factor2",
                                            "FilterFactor-Priority3", "FilterFactor-Factor3",
                                            "FilterFactor-Priority4", "FilterFactor-Factor4",
                                            "FilterFactor-Priority5", "FilterFactor-Factor5",
                                            "FilterFactor-Priority6", "FilterFactor-Factor6",
                                            "DefaultFactor-Priority1", "DefaultFactor-Factor1",
                                            "DefaultFactor-Priority2", "DefaultFactor-Factor2",
                                            "DefaultFactor-Priority3", "DefaultFactor-Factor3",
                                            "DefaultFactor-Priority4", "DefaultFactor-Factor4",
                                            "DefaultFactor-Priority5", "DefaultFactor-Factor5",
                                            "DefaultFactor-Priority6", "DefaultFactor-Factor6"));

                int totalItemCount = 0;
                foreach (var pass in _smoothConfiguration.Passes.OrderBy(p => p.Sequence))
                {
                    foreach (var bestBreakGroupRecord in _smoothConfiguration.BestBreakFactorGroupRecords.Where(r => r.PassSequences.Contains(pass.Sequence) || r.PassSequences.Count == 0))
                    {
                        int groupItemCount = 0;
                        foreach (var item in bestBreakGroupRecord.BestBreakFactorGroup.Items)
                        {
                            totalItemCount++;
                            groupItemCount++;
                            int groupId = _smoothConfiguration.BestBreakFactorGroupRecords.IndexOf(bestBreakGroupRecord) + 1;

                            // Get filter factors
                            string[] filterFactorTypes = new string[10];
                            int[] filterFactorPriorities = new int[10];
                            for (int index = 0; item.FilterFactors != null && index < item.FilterFactors.Count; index++)
                            {
                                filterFactorTypes[index] = item.FilterFactors[index].Factor.ToString();
                                filterFactorPriorities[index] = item.FilterFactors[index].Priority;
                            }

                            // Get default factors
                            string[] defaultFactorTypes = new string[10];
                            int[] defaultFactorPriorities = new int[10];
                            for (int index = 0; item.DefaultFactors != null && index < item.DefaultFactors.Count; index++)
                            {
                                defaultFactorTypes[index] = item.DefaultFactors[index].Factor.ToString();
                                defaultFactorPriorities[index] = item.DefaultFactors[index].Priority;
                            }

                            writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}{0}{35}{0}{36}{0}{37}{0}{38}{0}{39}",
                                _delimiter, totalItemCount, pass.Sequence, groupId, bestBreakGroupRecord.BestBreakFactorGroup.Name, bestBreakGroupRecord.BestBreakFactorGroup.Sequence, groupItemCount,
                                bestBreakGroupRecord.BestBreakFactorGroup.SameBreakGroupScoreFactor == null ? "" : bestBreakGroupRecord.BestBreakFactorGroup.SameBreakGroupScoreFactor.Priority.ToString(),
                                bestBreakGroupRecord.BestBreakFactorGroup.SameBreakGroupScoreFactor == null ? "" : bestBreakGroupRecord.BestBreakFactorGroup.SameBreakGroupScoreFactor.Factor.ToString(),
                                bestBreakGroupRecord.BestBreakFactorGroup.SameBreakGroupScoreAction,
                                bestBreakGroupRecord.SpotsCriteria.HasSponsoredSpots == null ? "Any" : bestBreakGroupRecord.SpotsCriteria.HasSponsoredSpots.ToString(),
                                bestBreakGroupRecord.SpotsCriteria.HasBreakRequest == null ? "Any" : bestBreakGroupRecord.SpotsCriteria.HasBreakRequest.ToString(),
                                bestBreakGroupRecord.SpotsCriteria.HasFIBORLIBRequests == null ? "Any" : bestBreakGroupRecord.SpotsCriteria.HasFIBORLIBRequests.ToString(),
                                item.AllFilterFactorsMustBeNonZero, item.UseZeroScoresInEvaluation, item.Evaluation.ToString(),
                                filterFactorPriorities[0] == 0 ? "" : filterFactorPriorities[0].ToString(), filterFactorTypes[0],
                                filterFactorPriorities[1] == 0 ? "" : filterFactorPriorities[1].ToString(), filterFactorTypes[1],
                                filterFactorPriorities[2] == 0 ? "" : filterFactorPriorities[2].ToString(), filterFactorTypes[2],
                                filterFactorPriorities[3] == 0 ? "" : filterFactorPriorities[3].ToString(), filterFactorTypes[3],
                                filterFactorPriorities[4] == 0 ? "" : filterFactorPriorities[4].ToString(), filterFactorTypes[4],
                                filterFactorPriorities[5] == 0 ? "" : filterFactorPriorities[5].ToString(), filterFactorTypes[5],
                                defaultFactorPriorities[0] == 0 ? "" : defaultFactorPriorities[0].ToString(), defaultFactorTypes[0],
                                defaultFactorPriorities[1] == 0 ? "" : defaultFactorPriorities[1].ToString(), defaultFactorTypes[1],
                                defaultFactorPriorities[2] == 0 ? "" : defaultFactorPriorities[2].ToString(), defaultFactorTypes[2],
                                defaultFactorPriorities[3] == 0 ? "" : defaultFactorPriorities[3].ToString(), defaultFactorTypes[3],
                                defaultFactorPriorities[4] == 0 ? "" : defaultFactorPriorities[4].ToString(), defaultFactorTypes[4],
                                defaultFactorPriorities[5] == 0 ? "" : defaultFactorPriorities[5].ToString(), defaultFactorTypes[5]));
                        }
                    }
                }

                writer.Flush();
                writer.Close();
            }
        }
    }
}
