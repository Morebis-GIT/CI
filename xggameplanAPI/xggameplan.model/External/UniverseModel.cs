using System;

namespace xggameplan.Model
{
    public class UniverseModel
    {
        public Guid Id { get; set; }
        public string SalesArea { get; set; }

        public string Demographic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UniverseValue { get; set; }
    }

    public class CreateUniverse
    {
        public Guid Id { get; set; }
        public string SalesArea { get; set; }

        public string Demographic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UniverseValue { get; set; }
    }

}
