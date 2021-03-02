using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class ProductCreatedOrUpdated : IProductCreatedOrUpdated
    {
        public string Externalidentifier { get; }
        public string ParentExternalidentifier { get; }
        public string Name { get; }
        public DateTime EffectiveStartDate { get; }
        public DateTime EffectiveEndDate { get; }
        public string ClashCode { get; }
        public string ReportingCategory { get; }
        public List<Agency> Agencies { get; }
        public List<Advertiser> Advertisers { get; }
        public List<Person> Persons { get; }

        public ProductCreatedOrUpdated(
            string externalidentifier,
            string parentExternalidentifier,
            string name,
            DateTime effectiveStartDate,
            DateTime effectiveEndDate,
            string clashCode, 
            string reportingCategory,
            List<Agency> agencies, 
            List<Advertiser> advertisers, 
            List<Person> persons)
        {
            Externalidentifier = externalidentifier;
            ParentExternalidentifier = parentExternalidentifier;
            Name = name;
            EffectiveStartDate = effectiveStartDate;
            EffectiveEndDate = effectiveEndDate;
            ClashCode = clashCode;
            ReportingCategory = reportingCategory;
            Agencies = agencies;
            Advertisers = advertisers;
            Persons = persons;
        }
    }
}
