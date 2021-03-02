using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AccessTokens
{
    public class AccessTokenModel
    {
        public string Token { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}
