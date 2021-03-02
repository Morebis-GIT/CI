using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class AuthorModel : ICloneable
    {
        public int Id { get; set; }             // User.Id?

        public string Name { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
