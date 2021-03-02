using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Validates the Smooth configuration
    /// </summary>
    public class SmoothConfigurationValidator
    {
        /// <summary>
        /// Validates the Smooth configuration
        /// </summary>
        /// <param name="smoothConfiguration"></param>
        public List<string> Validate(SmoothConfiguration smoothConfiguration)
        {
            var messages = new List<string>();

            if (String.IsNullOrEmpty(smoothConfiguration.Version))
            {
                messages.Add("Version is not set");
            }

            if (smoothConfiguration.Passes == null
                || smoothConfiguration.Passes.Count == 0)
            {
                messages.Add("No passes defined");
            }

            if (smoothConfiguration.IterationRecords == null
                || smoothConfiguration.IterationRecords.Count == 0)
            {
                messages.Add("No pass iterations defined");
            }

            if (smoothConfiguration.BestBreakFactorGroupRecords == null
                || smoothConfiguration.BestBreakFactorGroupRecords.Count == 0)
            {
                messages.Add("No best break factor groups defined");
            }

            // Check passes
            var passSequences = new List<int>();

            foreach (var pass in smoothConfiguration.Passes)
            {
                if (passSequences.Contains(pass.Sequence))
                {
                    messages.Add($"Pass {pass.Sequence} has been duplicated");
                }

                passSequences.Add(pass.Sequence);

                // Check for pass iterations
                var iterationRecords = smoothConfiguration.IterationRecords
                    .Where(ir => ir.PassSequences.Contains(pass.Sequence));

                if (!iterationRecords.Any())
                {
                    messages.Add($"Pass {pass.Sequence} has no iterations defined");
                }

                // Check best break factor groups
                if (pass.GetType() == typeof(SmoothPassDefault))
                {
                    var bestBreakFactorGroups = smoothConfiguration.BestBreakFactorGroupRecords
                        .Where(bb => bb.PassSequences.Contains(pass.Sequence));

                    if (!bestBreakFactorGroups.Any())
                    {
                        messages.Add($"Pass {pass.Sequence} has no best break factor groups defined");
                    }
                }
            }

            // Check pass iterations
            var passIterationSequences = new List<int>();

            foreach (var iterationRecord in smoothConfiguration.IterationRecords)
            {
                foreach (var passSequence in iterationRecord.PassSequences)
                {
                    var pass = smoothConfiguration.Passes.Find(p => p.Sequence == passSequence);

                    if (pass is null)
                    {
                        messages.Add($"Pass iteration {passSequence} has no pass defined");
                    }
                }

                if (iterationRecord.PassDefaultIteration != null)
                {
                    if (passIterationSequences.Contains(iterationRecord.PassDefaultIteration.Sequence))
                    {
                        messages.Add($"Pass iteration {iterationRecord.PassDefaultIteration.Sequence} must be unique");
                    }

                    passIterationSequences.Add(iterationRecord.PassDefaultIteration.Sequence);
                }
                else if (iterationRecord.PassUnplacedIteration != null)
                {
                    if (passIterationSequences.Contains(iterationRecord.PassUnplacedIteration.Sequence))
                    {
                        messages.Add($"Pass iteration {iterationRecord.PassUnplacedIteration.Sequence} has no pass defined");
                    }

                    passIterationSequences.Add(iterationRecord.PassUnplacedIteration.Sequence);
                }
                else
                {
                    messages.Add("Pass iteration must be either a default or unplaced pass iteration");
                }
            }

            // Check best break factor groups
            foreach (var bestBreakFactorGroup in smoothConfiguration.BestBreakFactorGroupRecords)
            {
                if (bestBreakFactorGroup.BestBreakFactorGroup?.Items.Count > 0 != true)
                {
                    messages.Add("Best break factor group has no items defined");
                }

                if (bestBreakFactorGroup.PassSequences == null || bestBreakFactorGroup.PassSequences.Count == 0)
                {
                    messages.Add("Best break factor group has no pass sequences defined");
                }

                foreach (var passSequence in bestBreakFactorGroup.PassSequences)
                {
                    var pass = smoothConfiguration.Passes.Find(p => p.Sequence == passSequence);

                    if (pass is null)
                    {
                        messages.Add(
                            $"Best break factor group {bestBreakFactorGroup.BestBreakFactorGroup.Name} for pass {passSequence} has no pass defined"
                            );
                    }
                }

                foreach (var item in bestBreakFactorGroup.BestBreakFactorGroup.Items)
                {
                    if (item.DefaultFactors.Count == 0)
                    {
                        messages.Add(
                            $"Best break factor group {bestBreakFactorGroup.BestBreakFactorGroup.Name} has no default factors"
                            );
                    }

                    if (item.FilterFactors != null)
                    {
                        foreach (var filterFactor in item.FilterFactors)
                        {
                            // Bit of a hack until we've split BestBreakFactors
                            // enum for filters & defaults
                            if (!filterFactor.Factor.ToString().StartsWith("Is"))
                            {
                                messages.Add(string.Format("Best break factor group {0} has a filter factor {1} but is is invalid because it uses a default factor {2}", bestBreakFactorGroup.BestBreakFactorGroup.Name, filterFactor.Factor.ToString(), filterFactor.Priority, filterFactor.Factor.ToString()));
                            }

                            if (filterFactor.Priority < 1 || filterFactor.Priority > BestBreakFactorGroupItem.MaxBreakFactorPriority)
                            {
                                messages.Add(string.Format("Best break factor group {0} has a filter factor {1} but the priority {2} is invalid", bestBreakFactorGroup.BestBreakFactorGroup.Name, filterFactor.Factor.ToString(), filterFactor.Priority));
                            }
                        }
                    }
                    if (item.DefaultFactors == null || item.DefaultFactors.Count == 0)
                    {
                        messages.Add(string.Format("Best break factor group {0} has no default factors", bestBreakFactorGroup.BestBreakFactorGroup.Name));
                    }
                    else
                    {
                        foreach (var defaultFactor in item.DefaultFactors)
                        {
                            if (defaultFactor.Factor.ToString().StartsWith("Is"))   // Bit of a hack until we've split BestBreakFactors enum for filters & defaults
                            {
                                messages.Add(string.Format("Best break factor group {0} has a default factor {1} that is invalid because it uses a filter factor {2}", bestBreakFactorGroup.BestBreakFactorGroup.Name, defaultFactor.Factor.ToString(), defaultFactor.Priority, defaultFactor.Factor.ToString()));
                            }

                            if (defaultFactor.Priority < 1 || defaultFactor.Priority > BestBreakFactorGroupItem.MaxBreakFactorPriority)
                            {
                                messages.Add(string.Format("Best break factor group {0} has a default factor {1} but the priority {2} is invalid", bestBreakFactorGroup.BestBreakFactorGroup.Name, defaultFactor.Factor.ToString(), defaultFactor.Priority));
                            }
                        }
                    }
                }
            }

            return messages;
        }
    }
}
