using System;

namespace xggameplan.Model
{
    public class SalesAreaReference : ICloneable
    {
        public int Id { get; set; }     // Guid?

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
