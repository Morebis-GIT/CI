using System.Collections.Generic;
using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    /// <summary>
    /// Currently a dumping ground for model loaders until I move them somewhere better.
    /// </summary>
    public interface IModelLoaders
    {
        // Methods to get all models
        IImmutableList<Campaign> GetAllCampaigns();
        IImmutableList<Clash> GetAllClashes();
        IImmutableList<ClashException> GetAllClashException();
        IImmutableList<IndexType> GetAllIndexTypes();
        IImmutableList<Product> GetAllProducts();
        IImmutableList<Restriction> GetAllRestrictions();
        IImmutableList<SmoothFailureMessage> GetAllSmoothFailureMessages();
        IImmutableList<Sponsorship> GetAllSponsorshipRestrictions();
        IImmutableList<Universe> GetAllUniverses();

        // Methods to get date and sales area specific models
        IImmutableList<Break> GetAllBreaksForSalesAreaForSmoothPeriod(
            DateTimeRange smoothPeriod,
            IReadOnlyCollection<string> salesAreaNames
            );

        // Methods to get global models
        SmoothConfiguration GetSmoothConfiguration();
    }
}
