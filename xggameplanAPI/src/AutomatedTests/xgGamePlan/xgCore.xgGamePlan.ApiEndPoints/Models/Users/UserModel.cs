using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is UserModel user)
            {
                return Equals(user);
            }

            return false;
        }

        protected bool Equals(UserModel other)
        {
            return Id == other.Id && string.Equals(Name, other.Name, StringComparison.InvariantCulture) &&
                   string.Equals(Surname, other.Surname, StringComparison.InvariantCulture) &&
                   string.Equals(Email, other.Email, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
