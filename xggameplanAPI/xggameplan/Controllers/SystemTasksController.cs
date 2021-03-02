using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using xggameplan.common.Services;
using xggameplan.Filters;
using xggameplan.SystemTasks;

namespace xggameplan.Controllers
{
    /// <summary>
    /// System tasks API
    /// </summary>
    [RoutePrefix("SystemTasks")]
    public class SystemTasksController : ApiController
    {
        private readonly ISystemTasksManager _systemTasksManager;

        public SystemTasksController(ISystemTasksManager systemTasksManager)
        {
            _systemTasksManager = systemTasksManager;
        }

        /// <summary>
        /// Executes system task, returns results
        /// </summary>
        /// <returns></returns>
        [Route("{id}/execute")]
        [AuthorizeRequest("SystemTasks")]
        [ResponseType(typeof(List<SystemTaskResult>))]
        public IHttpActionResult PostExecuteTask([FromUri] string id)
        {
            if (!_systemTasksManager.TaskExists(id))
            {
                return NotFound();
            }

            // Prevent simultaneous execution of task
            using (MachineLock.Create(string.Format("xggameplan.systemtasks.{0}.execute", id), TimeSpan.FromSeconds(60)))
            {
                var systemTaskResults = _systemTasksManager.ExecuteTask(id);
                return Ok(systemTaskResults);
            }
        }
    }
}
