namespace xgCore.xgGamePlan.ApiEndPoints.Models.Users
{
    public class CreateUserModel
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string ThemeName { get; set; }

        public string Location { get; set; }

        public string Role { get; set; }

        public int TenantId { get; set; }

        public string Region { get; set; }
    }
}
