using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT16422
{
    public class UpdateXGGT16422 : UpdateBase, IUpdate
    {
        public UpdateXGGT16422(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT16422_AddMapBreakWithProgrammesByExternalRefFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("DD4ED315-0FF0-43D3-B2F6-7289754487FE");

        public override string Name => "XGGT-16422";

        public override string DatabaseVersion => "";
    }
}
