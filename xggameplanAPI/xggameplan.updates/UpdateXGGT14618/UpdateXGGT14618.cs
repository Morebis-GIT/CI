using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT14618
{
    public class UpdateXGGT14618 : UpdateBase, IUpdate
    {
        public UpdateXGGT14618(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14618_AgHfssDemos(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("211D5553-F01A-4A0C-BC00-A26745868709");

        public override string Name => "XGGT-14618";

        public override string DatabaseVersion { get; } = "";
    }
}
