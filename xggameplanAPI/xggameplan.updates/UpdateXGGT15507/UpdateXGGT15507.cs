using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT15507
{
    public class UpdateXGGT15507 : UpdateBase, IUpdate
    {
        public UpdateXGGT15507(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT15507_AddBreakPosInProgFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("F6356803-681F-49BC-8BA8-E7AE751F87F4");

        public override string Name => "XGGT-15507";

        public override string DatabaseVersion => "";
    }
}
