using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT11538
{
    public class UpdateXGGT11538 : UpdateBase, IUpdate
    {
        public UpdateXGGT11538(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT11538_FeatureFlag_StrikeWeightDayPartsMerge(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("92B77E22-B36E-480D-B398-0C42BA14E422");

        public override string Name => "XGGT-11538";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
