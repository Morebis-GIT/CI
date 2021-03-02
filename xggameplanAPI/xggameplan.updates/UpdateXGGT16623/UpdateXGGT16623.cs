using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT16623 : UpdateBase, IUpdate
    {
        public UpdateXGGT16623(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);

            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT16623_AddToleranceSpotsProgrammeRule(
                    configuration.TenantConnectionStrings)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("0cd45ca3-754b-4396-a349-4e0b219a990e");

        public override string Name => "XGGT-16623";

        public override string DatabaseVersion => "";
    }
}
