using System.Collections.Generic;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Users;

namespace xgCore.xgGamePlan.AutomationTests.Contexts
{
    [Binding]
    public class UsersContext
    {
        #pragma warning disable CA2227
        public List<UserModel> InitialUsers { get; set; }
        #pragma warning restore CA2227

        public UserModel GivenUser { get; set; }

        public UserModel ReturnedUser { get; set; }
    }
}
