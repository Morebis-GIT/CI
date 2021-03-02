using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Domain.Generic.Helpers
{
    public static class TaskReflectionHelper
    {
        private static readonly MethodInfo WaitForTaskResultMethod =
            typeof(TaskReflectionHelper).GetMethod(nameof(WaitForTaskResult),
                BindingFlags.Static | BindingFlags.NonPublic);

        private static T WaitForTaskResult<T>(Task<T> task)
        {
            return task.Result;
        }

        public static object WaitForTask(Task task)
        {
            var taskType = task.GetType();
            if (taskType.IsGenericType)
            {
                var resultType = taskType.GetGenericArguments().Single();
                return WaitForTaskResultMethod.MakeGenericMethod(resultType).Invoke(null, new object[] { task });
            }

            task.Wait();
            return null;
        }

        public static Task<object> WaitForTaskAsync(Task task)
        {
            return Task.Run(() => WaitForTask(task));
        }
    }
}
