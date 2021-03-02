using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipDomain = ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;

namespace ImagineCommunications.GamePlan.Domain.Sponsorships
{
    public interface ISponsorshipRepository
    {
        SponsorshipDomain.Sponsorship Get(string externalReferenceId);

        IEnumerable<SponsorshipDomain.Sponsorship> GetAll();

        void Add(SponsorshipDomain.Sponsorship sponsorship);

        void Update(SponsorshipDomain.Sponsorship sponsorship);

        void Delete(string externalReferenceId);

        void SaveChanges();

        bool Exists(string externalReferenceId);

        Task TruncateAsync();
    }
}
