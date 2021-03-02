using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.core.Extensions;
using xggameplan.Extensions;
using xggameplan.model.AutoGen.AgSponsorships;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    public class SponsorshipExclusivitySerialiser
    {
        public SponsorshipExclusivitySerialiser(string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            FolderName = folderName;
        }

        private string FolderName { get; }

        /// <summary>
        /// Sponsorship exclusivity filename.
        /// </summary>
        public static string Filename => "v_spon_excl_list.xml";

        public void Serialise(
            List<Sponsorship> allSponsorships,
            IReadOnlyCollection<Programme> programmes,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionary,
            IReadOnlyCollection<SalesArea> salesAreas,
            IMapper mapper
            )
        {
            ToAgSponsorships(
                allSponsorships,
                programmes,
                programmeDictionary,
                salesAreas.ToList(),
                mapper)
                .Serialize(Path.Combine(FolderName, Filename));
        }

        private static AgSponsorshipsSerialization ToAgSponsorships(
            List<Sponsorship> sponsorships,
            IReadOnlyCollection<Programme> programmes,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            List<SalesArea> salesAreas,
            IMapper mapper)
        {
            var serialiser = new AgSponsorshipsSerialization();

            if (sponsorships is null)
            {
                return serialiser.MapFrom(new List<AgSponsorship>());
            }

            var sponsorshipList = sponsorships
                .Where(sponsorship =>
                    sponsorship.SponsoredItems != null &&
                    sponsorship.SponsoredItems.Any(sponsoredItem =>
                        sponsoredItem.SponsorshipItems.Any(sponsorshipItem =>
                            sponsorship.RestrictionLevel == SponsorshipRestrictionLevel.TimeBand
                            || (sponsorship.RestrictionLevel == SponsorshipRestrictionLevel.Programme
                                && programmes.Any(p => p.ProgrammeName == sponsorshipItem.ProgrammeName)
                            )
                        )
                    )
                )
                .ToImmutableList();

            var agSponsorships = new ConcurrentBag<AgSponsorship>();

            Parallel.ForEach(sponsorshipList, sponsorship =>
            {
                var sponsorshipRestrictionLevel = sponsorship.RestrictionLevel;

                sponsorship.SponsoredItems.ForEach(sponsoredItem =>
                {
                    var agSponsorship = new AgSponsorship()
                    {
                        CalculationType = (int)sponsoredItem.CalculationType,
                        RestrictionType = sponsoredItem.RestrictionType.HasValue ? (int)sponsoredItem.RestrictionType.Value : -1,
                        RestrictionValue = sponsoredItem.RestrictionValue.HasValue ? sponsoredItem.RestrictionValue.Value : -1,
                        Applicability = sponsoredItem.Applicability.HasValue ? (int)sponsoredItem.Applicability.Value : -1,
                        SponsoredItems = new List<AgSponsoredItems>()
                    };

                    agSponsorship.AdvertiserExclusivities = mapper.Map<List<AgAdvertiserExclusivity>>(sponsoredItem.AdvertiserExclusivities ?? new List<AdvertiserExclusivity>());
                    agSponsorship.ClashExclusivities = mapper.Map<List<AgClashExclusivity>>(sponsoredItem.ClashExclusivities ?? new List<ClashExclusivity>());
                    agSponsorship.ProductsCode = mapper.Map<List<AgProductCode>>(sponsoredItem.Products ?? new List<string>());

                    if (sponsoredItem.SponsorshipItems != null)
                    {
                        sponsoredItem.SponsorshipItems
                            .Where(sponsorshipItem =>
                                sponsorshipRestrictionLevel == SponsorshipRestrictionLevel.TimeBand
                                || (sponsorshipRestrictionLevel == SponsorshipRestrictionLevel.Programme
                                    && programmes.Any(p => p.ProgrammeName == sponsorshipItem.ProgrammeName))
                            )
                            .ToList()
                            .ForEach(sponsorshipItem =>
                            {
                                var programmeExternalReference = programmes.Any(p => p.ProgrammeName == sponsorshipItem.ProgrammeName)
                                    ? programmes.First(p => p.ProgrammeName == sponsorshipItem.ProgrammeName).ExternalReference
                                    : "";

                                var programmeNo = programmeDictionaries.FirstOrDefault(p => p.ExternalReference == programmeExternalReference)?.Id ?? 0;

                                var agSponsoredItem = new AgSponsoredItems()
                                {
                                    SalesAreas = new List<AgSalesArea>(),
                                    ProgramNumber = programmeNo,
                                    StartDate = AgConversions.ToAgDateYYYYMMDDAsString(sponsorshipItem.StartDate),
                                    EndDate = AgConversions.ToAgDateYYYYMMDDAsString(sponsorshipItem.EndDate),
                                };

                                agSponsoredItem.DayParts = mapper.Map<List<AgSponsorshipDayPart>>(sponsorshipItem.DayParts ?? new List<SponsoredDayPart>());

                                if (sponsorshipItem.SalesAreas != null)
                                {
                                    sponsorshipItem.SalesAreas
                                    .ToList()
                                    .ForEach(salesAreaName =>
                                        agSponsoredItem.SalesAreas.Add(mapper.Map<AgSalesArea>(Tuple.Create(salesAreaName, salesAreas))));
                                }

                                agSponsorship.SponsoredItems.Add(agSponsoredItem);
                            });
                    }

                    agSponsorship.SponsoredItemsCount = agSponsorship.SponsoredItems.Count;
                    agSponsorships.Add(agSponsorship);
                });
            });

            return serialiser.MapFrom(agSponsorships.ToList());
        }
    }
}
