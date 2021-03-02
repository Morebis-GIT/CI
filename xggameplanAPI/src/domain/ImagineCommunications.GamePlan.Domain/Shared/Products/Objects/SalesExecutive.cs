using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.Products.Objects
{
    public class SalesExecutive
    {
        public string Name { get; set; }
        public int Identifier { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
