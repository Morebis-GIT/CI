using Autofac;
using xggameplan.core.Tasks;
using xggameplan.core.Tasks.Executors;

namespace xggameplan.core.DependencyInjection
{
    public class TaskExecutorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TaskExecutorFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TaskExecutorResolver>().As<ITaskExecutorResolver>().InstancePerLifetimeScope();
            builder.RegisterType<StartRunTaskExecutor>().Named<ITaskExecutor>(TaskIds.StartRun);
            builder.RegisterType<StartNextScenarioTaskExecutor>().Named<ITaskExecutor>(TaskIds.StartNextScenario);
            builder.RegisterType<ScenarioCompletedTaskExecutor>().Named<ITaskExecutor>(TaskIds.ScenarioCompleted);
            builder.RegisterType<TestTaskExecutor>().Named<ITaskExecutor>(TaskIds.Test);
        }
    }
}
