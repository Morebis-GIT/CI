using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT9221 : UpdateBase, IUpdate
    {
        public UpdateXGGT9221(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT9221_AddProgrammeRule(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("9EE839BA-5A27-4569-B544-25DB627C05EA");

        public override string Name => "XGGT-9221";

        public override string DatabaseVersion { get; } = "";
    }
}
