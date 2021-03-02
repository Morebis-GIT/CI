using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using Microsoft.Extensions.Configuration;
using NodaTime;

namespace xggameplan.Areas.System.Auth
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private const string ACCESS_TOKEN_DURATION_CONFIG_NAME = "Security:Tokens:DurationInMinutes";
        private const int ACCESS_TOKEN_DEFAULT_DURATION_IN_MINUTES = 10080;

        private readonly IUsersRepository _usersRepository;
        private readonly IAccessTokensRepository _accessTokenRepository;
        private readonly IClock _clock;
        private readonly IConfiguration _configuration;

        public AuthenticationManager(
            IUsersRepository usersRepository,
            IAccessTokensRepository accessTokenRepository,
            IClock clock) : this(usersRepository, accessTokenRepository, clock, null) { }

        public AuthenticationManager(
            IUsersRepository usersRepository,
            IAccessTokensRepository accessTokenRepository,
            IClock clock,
            IConfiguration configuration)
        {
            _usersRepository = usersRepository;
            _accessTokenRepository = accessTokenRepository;
            _clock = clock;
            _configuration = configuration;
        }

        
        public bool TrySignIn(
            string email,
            string password,
            out AccessToken token)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            User user = _usersRepository.GetByEmail(email);
            if ( user == null || !user.IsPasswordValid(password))
            {
                token = null;
                return false;
            }
            
            token = new AccessToken
            {
                ValidUntilValue = GetExprirationTime().UtcDateTime,
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
            };

            _accessTokenRepository.Insert(token);
            _accessTokenRepository.SaveChanges();
            return true;
        }

        public User GetAuthenticatedUser(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var accessToken = _accessTokenRepository.Find(token);

            if (accessToken == null)
            {
                return null;
            }

            if (IsExpired(accessToken.ValidUntilValue))
            {
                return null;
            }

            return _usersRepository.GetById(accessToken.UserId);
        }

        public void SignOut(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            _accessTokenRepository.Delete(token);
            _accessTokenRepository.SaveChanges();
        }

        private bool IsExpired(DateTimeOffset expirationTime)
        {
            return _clock.GetCurrentInstant().ToDateTimeOffset() >= expirationTime;
        }

        private DateTimeOffset GetExprirationTime()
        {
            return _clock.GetCurrentInstant().Plus(Duration.FromMinutes(
                   _configuration?.GetValue<int>(ACCESS_TOKEN_DURATION_CONFIG_NAME) ?? ACCESS_TOKEN_DEFAULT_DURATION_IN_MINUTES)).ToDateTimeOffset();
        }
    }
}
