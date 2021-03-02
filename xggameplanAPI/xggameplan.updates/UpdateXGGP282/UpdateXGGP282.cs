using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP282 : UpdateBase, IUpdate
    {
        public UpdateXGGP282(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP282_ExistingTokensUpdate(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id
        {
            get { return new Guid("de2e002d-de41-4748-812e-b62147742720"); }
        }

        public override string Name
        {
            get { return "XGGP-282"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; } 
        }
    }
}
