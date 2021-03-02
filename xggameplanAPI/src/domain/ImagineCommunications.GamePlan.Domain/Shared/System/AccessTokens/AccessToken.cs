using System;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens
{
    public class AccessToken
    {
        public string Token { get; set; }

        public int UserId { get; set; }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public Instant ValidUntil
        {
            get => Instant.FromDateTimeOffset(ValidUntilValue);
            set => ValidUntilValue = value.ToDateTimeOffset();
        }

        public DateTimeOffset ValidUntilValue { get; set; }
    }
}
