using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.Database;

namespace xggameplan.core.Export
{
    /// <summary>
    /// Settings for master database
    /// </summary>
    public class MasterDatabaseProcessingSettings : DatabaseMassProcessingSettings
    {
        public MasterDatabaseProcessingSettings(string dataFolder) =>
            Init(dataFolder);

        public MasterDatabaseProcessingSettings() => Init();
        

        private void Init(string dataFolder = null)
        {
            DataFolder = dataFolder;
            SetDocumentTypesToProcess(_exportDataTypes);
            SetDocumentTypeFilterFunctions(GetDocumentExportFilters());
        }
        private Dictionary<Type, Func<object, bool>> GetDocumentExportFilters()
        {
            return new Dictionary<Type, Func<object, bool>>()
            {
                { typeof(AccessToken), IsExportAccessToken },
                { typeof(User), IsExportUser }
            };
        }

        /// <summary>
        /// Whether to export access token, only non-expiring for internal users
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static bool IsExportAccessToken(object accessToken)
        {
            var accessTokenObject = (AccessToken)accessToken;
            var nonExpiringAccessTokens = new List<Guid>()
            {
                new Guid("74fe2f3c-2d2c-4fa4-879a-4984f18fe979"),   // Tasks
                new Guid("8df22a0d-bbc3-4ab5-9359-b6f235e873cd"),   // Frontend
                new Guid("77648dbd-e6de-49df-9d75-c4513ab41029")    // AutoBook
            }.Select(at => at.ToString().ToUpper());

            return accessTokenObject.ValidUntilValue > DateTime.UtcNow.AddYears(1)
                   && nonExpiringAccessTokens.Contains(accessTokenObject.Token.ToUpper());
        }

        /// <summary>
        /// Whether to export user, only internal users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool IsExportUser(object user)
        {
            var userObject = (User)user;
            string[] names = new string[] { "internal", "autobook", "frontend", "tasks" };
            return Array.IndexOf(names, userObject.Name.ToLower()) != -1;
        }

        private readonly List<Type> _exportDataTypes = new List<Type>()
        {
                    typeof(AccessToken),
                    typeof(User),
                    typeof(Tenant),
                    typeof(TaskInstance),
                    typeof(UpdateDetails),
                    typeof(PreviewFile),
                    typeof(ProductSettings),
        };
    }
}
