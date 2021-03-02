using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Newtonsoft.Json;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGP282_ExistingTokensUpdate : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGP282_ExistingTokensUpdate(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("b9dd9ccf-30d1-4f4b-9b8d-8280991e26d8");

        public void Apply()
        {
            var maximumValidUntilDate = new DateTimeOffset(DateTime.Now.AddYears(1));
            var accessTokensToRemove = new List<AccessToken>(default(int));

            using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString))
            using (IDocumentSession session = documentStore.OpenSession())
            {
                var existingTokens = session.GetAll<AccessToken>(accessToken => accessToken.ValidUntilValue <= maximumValidUntilDate);

                accessTokensToRemove.AddRange(existingTokens);
                BackupAccessTokens(accessTokensToRemove);

                accessTokensToRemove.ForEach(token => session.Delete(token));

                session.SaveChanges();
            }
        }

        private void BackupAccessTokens(IEnumerable<AccessToken> tokensToBackup)
        {
            foreach (var token in tokensToBackup)
            {
                string accessTokenFile = Path.Combine(_rollBackFolder, string.Format("AccessToken.{0}.{1}.json", token.UserId, token.GetHashCode()));
                var serialized = JsonConvert.SerializeObject(token);
                if (File.Exists(accessTokenFile))
                {
                    File.Delete(accessTokenFile);
                }
                File.WriteAllText(accessTokenFile, serialized);
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-282 : Reduce the lifetime of an access token";

        public bool SupportsRollback => false;
    }
}
