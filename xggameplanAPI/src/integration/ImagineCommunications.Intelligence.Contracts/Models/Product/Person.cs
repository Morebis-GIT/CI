using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class Person
    {
        public string Name { get; }
        public int PersonIdentifier { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public Person(string name, int personIdentifier, DateTime startDate, DateTime endDate)
        {
            Name = name;
            PersonIdentifier = personIdentifier;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
