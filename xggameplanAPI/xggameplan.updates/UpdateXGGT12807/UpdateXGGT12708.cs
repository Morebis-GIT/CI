using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT12708
{
    public class UpdateXGGT12708 : UpdateBase, IUpdate
    {
        public UpdateXGGT12708(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12708_RatingsPredictionsLogicChanges(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("A7EC0556-7F57-47C9-AE0F-7755617BF151");

        public override string Name => "XGGT-12708";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
