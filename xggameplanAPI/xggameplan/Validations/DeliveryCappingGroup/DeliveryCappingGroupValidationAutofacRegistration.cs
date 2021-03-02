using Autofac;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;

namespace xggameplan.Validations.DeliveryCappingGroup
{
    public static class DeliveryCappingGroupValidationAutofacRegistration
    {
        public static DeliveryCappingGroupValidator GetValidator(IComponentContext context)
        {
            var repository = context.Resolve<IDeliveryCappingGroupRepository>();

            return new DeliveryCappingGroupValidator(GetValidation(repository));
        }

        public static DeliveryCappingGroupValidation GetValidation(IDeliveryCappingGroupRepository repository)
        {
            return new DeliveryCappingGroupValidation(x =>
            {
                var existingEntity = repository.GetByDescription(x.Description);
                return existingEntity == null || existingEntity.Id == x.Id;
            });
        }
    }
}