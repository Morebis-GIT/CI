using System.Web.Hosting;
using System.Web.Http;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using xggameplan.core.Database;
using xggameplan.core.Export;

namespace xggameplan.Areas.System.Database
{
    /// <summary>
    /// Provides API for interacting with master database for maintenance functions.
    /// </summary>
    [RoutePrefix("databases/master")]
    public class MasterDatabaseController : ApiController
    {
        private readonly DatabaseJsonExporter _databaseExporter;

        public MasterDatabaseController(IMasterDbContext dbContext)
        {
            _databaseExporter = new DatabaseJsonExporter(dbContext);
        }

        /// <summary>
        /// For Imagine use only - Exports the master database, currently just the data.
        /// </summary>        
        [Route("export")]
        public IHttpActionResult PostExport()
        {
            _databaseExporter.Export(
                new MasterDatabaseProcessingSettings(
                    dataFolder: HostingEnvironment.MapPath("/Temp/Export/Master/SeedData")));
            return Ok();
        }
    }
}
