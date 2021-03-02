using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product
{
    public interface IProductCreatedOrUpdated : IEvent
    {
        string Externalidentifier { get; }
        string ParentExternalidentifier { get; }
        string Name { get; }
        DateTime EffectiveStartDate { get; }
        DateTime EffectiveEndDate { get; }
        string ClashCode { get; }
        string ReportingCategory { get; }
        List<Agency> Agencies { get; }
        List<Advertiser> Advertisers { get; }
        List<Person> Persons { get; }
    }
}
