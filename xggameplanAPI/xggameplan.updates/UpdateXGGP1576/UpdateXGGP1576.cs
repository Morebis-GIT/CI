using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP1576 : UpdateBase, IUpdate
    {
        public UpdateXGGP1576(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP1576_RunProcessRanges(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id
        {
            get { return new Guid("8027fa36-2877-4022-91de-a88b70e13e8a"); }    
        }

        public override string Name
        {
            get { return "XGGP-1576"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; } 
        }
    }
}
