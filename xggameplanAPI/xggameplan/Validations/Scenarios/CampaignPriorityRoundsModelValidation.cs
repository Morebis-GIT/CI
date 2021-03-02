using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CampaignPriorityRoundsModelValidation : AbstractValidator<CampaignPriorityRoundsModel>
    {
        public CampaignPriorityRoundsModelValidation()
        {
            When(campaignPriorityRounds => !IsNoPriorityRound(campaignPriorityRounds), () =>
            {
                Custom(campaignPriorityRounds =>
                {
                    const int HighestPriority = 1;

                    for (var i = 0; i < campaignPriorityRounds.Rounds.Count; i++)
                    {
                        var round = campaignPriorityRounds.Rounds[i];
                        var roundNumber = i + 1;

                        if(round.Number != roundNumber)
                        {
                            return new ValidationFailure(nameof(round.PriorityFrom), $"Priority Round {roundNumber} has invalid Number value.");
                        }

                        if (round.PriorityFrom > round.PriorityTo)
                        {
                            return new ValidationFailure(nameof(round.PriorityFrom), $"Round {roundNumber} has invalid priority range.");
                        }

                        if (round.PriorityFrom < (int)PassPriorityType.Exclude)
                        {
                            return new ValidationFailure(nameof(round.PriorityFrom), $"Priority Round {roundNumber} has invalid PriorityFrom value.");
                        }

                        if (round.PriorityTo > (int)PassPriorityType.Include)
                        {
                            return new ValidationFailure(nameof(round.PriorityTo), $"Priority Round {roundNumber} has invalid PriorityTo value.");
                        }

                        if (campaignPriorityRounds.Rounds.Any(r => (r.PriorityFrom <= round.PriorityFrom && r.PriorityTo > round.PriorityFrom ||
                        r.PriorityFrom > round.PriorityFrom && r.PriorityFrom <= round.PriorityTo) && !r.Equals(round)))
                        {
                            return new ValidationFailure(nameof(round), $"Priority Round {roundNumber} is overlapped by another round.");
                        }

                        if (round.PriorityFrom > HighestPriority && !campaignPriorityRounds.Rounds.Any(r => r.PriorityTo == round.PriorityFrom - 1))
                        {
                            return new ValidationFailure(nameof(round), $"There is a gap before Priority Round {roundNumber}.");
                        }

                        if (round.PriorityTo < (int)PassPriorityType.Include && !campaignPriorityRounds.Rounds.Any(r => r.PriorityFrom == round.PriorityTo + 1))
                        {
                            return new ValidationFailure(nameof(round), $"There is a gap after Priority Round {roundNumber}.");
                        }
                    }

                    return null;
                });
            });
        }

        private bool IsNoPriorityRound(CampaignPriorityRoundsModel campaignPriorityRounds)
        {
            return campaignPriorityRounds.Rounds.Count == 1 &&
                campaignPriorityRounds.Rounds[0].PriorityFrom == 0 &&
                campaignPriorityRounds.Rounds[0].PriorityTo == 0;
        }
    }
}
