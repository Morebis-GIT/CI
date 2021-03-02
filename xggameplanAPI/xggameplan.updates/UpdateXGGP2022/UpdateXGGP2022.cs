using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP2022 : UpdateBase, IUpdate
    {
        public UpdateXGGP2022(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP2022_MapStartAndEnd(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id
        {
            get { return new Guid("bae01e2c-96b2-49a8-8699-c10a01ddf96d"); }
        }

        public override string Name
        {
            get { return "XGGP-2022"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; } 
        }
    }
}
