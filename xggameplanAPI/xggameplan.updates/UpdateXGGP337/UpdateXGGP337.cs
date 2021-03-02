using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update for XGGP-337
    /// </summary>
    public class UpdateXGGP337 : UpdateBase, IUpdate
    {
        private List<string> _tenantConnectionStrings;
       
        public UpdateXGGP337(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings.ToList();            
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP337_XGGP357(_tenantConnectionStrings, _updatesFolder)
            };                        
        }

        public override Guid Id
        {
            get { return new Guid("98ef8be0-495e-4397-97e3-6372d28dd4e0"); }
        }

        public override string Name
        {
            get { return "XGGP-337"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; }    // TODO: Set this
        }

        public List<Guid> DependsOnUpdates
        {
            get { return new List<Guid>(); }
        }
    }
}
