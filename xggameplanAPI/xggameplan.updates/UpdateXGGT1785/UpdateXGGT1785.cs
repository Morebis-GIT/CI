using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    internal class UpdateXGGT1785 : UpdateBase, IUpdate
    {
        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("dbeeb60b-f107-4e3c-bb8e-0d4eff656f2c");

        public override string Name => "XGGT-1785";

        public override string DatabaseVersion { get; } = "";

        public UpdateXGGT1785(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT1785_AddFuncationalAreaCampaignFaultTypes(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }
    }
}