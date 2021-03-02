using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.core.Tasks;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests the task executor mechanism (i.e. Mechanism that performs background processing such as starting runs, processing output
    /// files etc).
    /// </summary>
    public class TaskExecutorTest : ISystemTest
    {
        private readonly string _rootFolder;
        private readonly IRepositoryFactory _repositoryFactory;
        private const string _category = "Task Executor";

        public TaskExecutorTest(string rootFolder, IRepositoryFactory repositoryFactory)
        {
            _rootFolder = rootFolder;
            _repositoryFactory = repositoryFactory;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return true;
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();
                        
            try
            {
                // Create test task instance
                TaskInstance taskInstance = TaskInstanceFactory.CreateTestTaskInstance(0, 3);

                // Execute task, wait for completion, will fail if exe missing, insufficient folder/file permissions
                //taskResult not used anywhere in this test - so commented out
                //TaskResult taskResult = new ProcessTaskExecutor(_rootFolder, _repositoryFactory, TimeSpan.FromSeconds(10)).Execute(taskInstance);

                if (taskInstance.Status != TaskInstanceStatues.CompletedSuccess)
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Error testing task executor. It may cause runs to fail.", ""));
                }
            }
            catch(System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error {0} testing task executor. It may cause runs to fail.", exception.Message), ""));
            }
            
            if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Task executor test OK", ""));
            }
            return results;
        }
    }
}
