using System;
using System.Web.Http;
using System.Web.Http.Description;
using xggameplan.AuditEvents;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Updates;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for applying or rolling back updates
    /// </summary>
    [RoutePrefix("Updates")]
    public class UpdatesController : ApiController
    {
        private readonly IUpdateManager _updateManager;
        private IAuditEventRepository _auditEventRepository;

        public UpdatesController(IUpdateManager updateManager, IAuditEventRepository auditEventRepository)
        {
            _updateManager = updateManager;
            _auditEventRepository = auditEventRepository;
        }

        /// <summary>
        /// Applies update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/Apply")]
        [AuthorizeRequest("Updates")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult Apply([FromUri] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            // Check if update exists
            IUpdate update = _updateManager.GetUpdate(id);
            if (update == null)
            {
                return NotFound();
            }

            // Apply update
            _updateManager.Apply(id);

            return Ok();
        }

        /// <summary>
        /// Rolls back the update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/RollBack")]
        [AuthorizeRequest("Updates")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult RollBack([FromUri] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            // Check if update exists
            IUpdate update = _updateManager.GetUpdate(id);
            if (update == null)
            {
                return NotFound();
            }            

            // Check if update supports roll back
            if (!update.SupportsRollback)
            {
                return this.Error().InvalidParameters("Update does not support roll back");
            }

            // Update
            _updateManager.RollBack(id);

            return Ok();
        }
    }
}
