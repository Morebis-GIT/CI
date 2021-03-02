using System.Linq;
using System.Text;
using Raven.Abstractions.Extensions;
using xggameplan.Model;

namespace xggameplan.Validations.Scenarios
{
    public static class CampaignPriorityRoundsModelValidations
    {
        private static IModelDataValidator<CampaignPriorityRoundsModel> _campaignPriorityRoundsModelValidator =
            new CampaignPriorityRoundsModelValidator(new CampaignPriorityRoundsModelValidation());

        public static string ValidateCampaignPriorityRounds(CampaignPriorityRoundsModel campaignPriorityRounds)
        {
            var cprErrors = new StringBuilder();

            if (campaignPriorityRounds?.Rounds?.Any() == true)
            {
                if (!_campaignPriorityRoundsModelValidator.IsValid(campaignPriorityRounds))
                {
                    _campaignPriorityRoundsModelValidator.Errors.ForEach(e => cprErrors.AppendLine(e.ErrorMessage));
                }
            }

            return cprErrors.ToString();
        }
    }
}
