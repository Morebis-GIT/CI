using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.Model;

namespace xggameplan.Services
{
    /// <summary>
    /// Enables setting of IDs
    /// </summary>
    public static class IdUpdater
    {
        public static void SetIds(Run run, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IRunRepository>();
            run.Id = (run.Id == Guid.Empty) ? Guid.NewGuid() : run.Id;
            run.CustomId = (run.CustomId == 0) ? identityGenerator.GetIdentities<RunNoIdentity>(1)[0].Id : run.CustomId;
            run.AnalysisGroupTargets.ForEach(SetIds);
            run.Scenarios.ForEach(SetIds);
        }

        public static void SetIds(Scenario scenario, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IScenarioRepository>();
            scenario.Id = (scenario.Id == Guid.Empty) ? Guid.NewGuid() : scenario.Id;
            scenario.CustomId = (scenario.CustomId == 0) ? identityGenerator.GetIdentities<ScenarioNoIdentity>(1)[0].Id : scenario.CustomId;
            scenario.Passes.ForEach(pass => SetIds(pass, identityGeneratorResolver));
        }

        private static void SetIds(RunScenario scenario)
        {
            scenario.Id = (scenario.Id == Guid.Empty) ? Guid.NewGuid() : scenario.Id;
        }

        private static void SetIds(AnalysisGroupTarget item) =>
            item.Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;

        public static void SetIds(PassReference pass, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IPassRepository>();
            pass.Id = (pass.Id == 0) ? identityGenerator.GetIdentities<PassIdIdentity>(1)[0].Id : pass.Id;
        }

        public static void SetIds(Pass pass, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IPassRepository>();
            pass.Id = (pass.Id == 0) ? identityGenerator.GetIdentities<PassIdIdentity>(1)[0].Id : pass.Id;
        }

        public static void SetCustomId(Scenario scenario, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IScenarioRepository>();
            scenario.CustomId = (scenario.CustomId == 0) ? identityGenerator.GetIdentities<ScenarioNoIdentity>(1)[0].Id : scenario.CustomId;
        }

        public static void SetIds(PassModel passModel, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IPassRepository>();
            passModel.Id = (passModel.Id == 0) ? identityGenerator.GetIdentities<PassIdIdentity>(1)[0].Id : passModel.Id;
        }

        public static void SetIdForScenario(Scenario scenario, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IScenarioRepository>();
            scenario.Id = (scenario.Id == Guid.Empty) ? Guid.NewGuid() : scenario.Id;
            scenario.CustomId = (scenario.CustomId == 0) ? identityGenerator.GetIdentities<ScenarioNoIdentity>(1)[0].Id : scenario.CustomId;
        }

        public static void SetIdForPasssReference(PassReference pass, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            var identityGenerator =
                (identityGeneratorResolver ?? throw new ArgumentNullException(nameof(identityGeneratorResolver)))
                .Resolve<IPassRepository>();
            pass.Id = (pass.Id == 0) ? identityGenerator.GetIdentities<PassIdIdentity>(1)[0].Id : pass.Id;
        }

        public static void SetIds(CreateRunScenarioModel createRunScenarioModel, IIdentityGeneratorResolver identityGeneratorResolver)
        {
            createRunScenarioModel.Id = createRunScenarioModel.Id == Guid.Empty ? new Guid() : createRunScenarioModel.Id;
            createRunScenarioModel.Passes.ForEach(pass => SetIds(pass, identityGeneratorResolver));

            if (createRunScenarioModel.CampaignPassPriorities != null)
            {
                createRunScenarioModel.CampaignPassPriorities.ForEach(c =>
                {
                    c.PassPriorities.ForEach(p =>
                    {
                        p.PassId = createRunScenarioModel.Passes.Where(a => a.Name.Trim().ToUpperInvariant() == p.PassName.Trim().ToUpperInvariant())
                                                                .Select(a => a.Id).FirstOrDefault();
                    });
                });
            }
        }
    }
}
