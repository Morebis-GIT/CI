namespace xgCore.xgGamePlan.ApiEndPoints.Models.Users
{
    public class UpdatePasswordModel
    {
        public int Id { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
