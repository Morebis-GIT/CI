using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CampaignPriorityRoundsModelValidator : ModelDataValidatorBase<CampaignPriorityRoundsModel>, IModelDataValidator<CampaignPriorityRoundsModel>
    {
        public CampaignPriorityRoundsModelValidator(IValidator<CampaignPriorityRoundsModel> modelValidator)
            : base(modelValidator)
        {
        }
    }
}
