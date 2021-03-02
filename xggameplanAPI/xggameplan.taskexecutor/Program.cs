using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Autofac;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Mappings;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Microsoft.Extensions.Configuration;
using NodaTime;
using xggameplan.common.Caching;
using xggameplan.core.DependencyInjection;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Hubs;
using xggameplan.core.Tasks;
using xggameplan.taskexecutor.DependencyInjection;
using AutoMapperConfig = xggameplan.Configuration.AutoMapperConfig;

namespace xggameplan.taskexecutor
{
    internal class Program
        : IDisposable
    {
        private const int ResultOk = 0;
        private const int ResultTaskIdMissing = -1;
        private const int ResultRootFolderMissing = -2;
        private const int ResultRootFolderDoesNotExist = -3;
        private const int ResultFailure = -4;
        private const int ResultExceptionCaught = 666;

        /// <summary>
        /// If running the API in Visual Studio this is the only way to debug
        /// the program.
        /// </summary>
        [Conditional("DEBUG")]
        private static void LaunchDebugger() => Debugger.Launch();

        // Where Gameplan is located, contains "App_Data" (so that we can read
        // config.json), "bin", "Logs" (for audit events) etc
        private static string _rootFolder = string.Empty;

        private static string _logFolder = Environment.CurrentDirectory;

        private static Mutex _syncMutex;

        private static void Main(string[] args)
        {
            int exitCode;

            Guid taskInstanceId = Guid.Empty;

            try
            {
                LaunchDebugger();

                Console.WriteLine($"Current directory = {Environment.CurrentDirectory}");

                // Get arguments from command line
                foreach (string arg in args)
                {
                    Console.WriteLine($"Checking argument {arg}");

                    string[] argValues = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    // Strip start/end quotes
                    if (argValues.Length > 1)
                    {
                        argValues[1] = StripStartEndChars(argValues[1], new[] { '\'', '"' });
                    }

                    switch (argValues[0].ToUpperInvariant())
                    {
                        case "/TASKINSTANCEID":
                            taskInstanceId = new Guid(argValues[1]);
                            break;

                        case "/ROOTFOLDER":
                            _rootFolder = argValues[1];
                            break;
                    }

                    Console.WriteLine($"Checked argument {arg}");
                }

                // Check arguments OK
                if (taskInstanceId == Guid.Empty)
                {
                    Console.WriteLine("TaskInstanceID is missing");
                    Environment.Exit(ResultTaskIdMissing);
                }

                if (String.IsNullOrEmpty(_rootFolder))
                {
                    Console.WriteLine("RootFolder is missing");
                    Environment.Exit(ResultRootFolderMissing);
                }

                if (!Directory.Exists(_rootFolder))
                {
                    Console.WriteLine($"RootFolder {_rootFolder} does not exist");
                    Environment.Exit(ResultRootFolderDoesNotExist);
                }

                using (_syncMutex = new Mutex(false, "TaskExecutorLogMutex"))
                {
                    AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                    WriteLog(
                        taskInstanceId,
                        "",
                        $"Unhandled exception (ProcessId={Process.GetCurrentProcess().Id}, RootFolder={_rootFolder})" +
                        Environment.NewLine + eventArgs.ExceptionObject.ToString());

                    _logFolder = Path.Combine(_rootFolder, "Logs");
                    WriteLog(
                        taskInstanceId,
                        "",
                        $"Process has started (ProcessId = {Process.GetCurrentProcess().Id}, RootFolder = {_rootFolder})"
                        );

                    var applicationConfiguration = new ConfigurationBuilder()
                        .SetBasePath(Path.Combine(_rootFolder, "App_Data"))
                        .AddJsonFile("config.json").Build();

                    using (var scope = SetupAutofac(applicationConfiguration, taskInstanceId))
                    {
                        exitCode = ExecuteTask(scope);
                    }
                }
            }
            catch (Exception e)
            {
                exitCode = ResultExceptionCaught;

                try
                {
                    using (_syncMutex = new Mutex(false, "TaskExecutorLogMutex"))
                    {
                        WriteLog(taskInstanceId, "", $"Catch Error = {e.Message}: {Process.GetCurrentProcess().Id}, RootFolder = {_rootFolder}, StackTrace = {e.StackTrace}");
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to write to log - tasks.txt");
                }
            }
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Executes task
        /// </summary>
        private static int ExecuteTask(ILifetimeScope scope)
        {
            var taskInstance = scope.Resolve<TaskInstance>();
            var taskResult = new TaskResult();
            int result = ResultFailure;

            try
            {
                if (taskInstance.Status != TaskInstanceStatues.Starting)
                {
                    throw new Exception(
                        $"Task Instance status is {taskInstance.Status} but it should be {TaskInstanceStatues.Starting}"
                        );
                }

                var taskExecutorFactory = scope.Resolve<TaskExecutorFactory>();

                WriteLog(taskInstance.Id, taskInstance.TaskId, "Getting task executor");

                var taskExecutor = taskExecutorFactory.GetTaskProcessor(taskInstance);

                WriteLog(taskInstance.Id, taskInstance.TaskId, "Got task executor");
                WriteLog(taskInstance.Id, taskInstance.TaskId, "Executing task");

                taskResult = taskExecutor.Execute(taskInstance);

                WriteLog(taskInstance.Id, taskInstance.TaskId, "Executed task");

                if (IsTaskSuccessful())
                {
                    result = ResultOk;
                }
                else
                {
                    if (taskResult?.Exception is null)
                    {
                        WriteLog(taskInstance.Id, taskInstance.TaskId, "Unknown error");
                    }
                    else if (taskResult.Exception != null)
                    {
                        WriteLog(taskInstance.Id, taskInstance.TaskId, $"Error: {taskResult.Exception.Message}, Stack: {taskResult.Exception.StackTrace}");
                    }
                }
            }
            catch (AggregateException exception)
            {
                var message = ComposeLogMessage(exception);
                WriteLog(taskInstance.Id, taskInstance == null ? "" : taskInstance.TaskId, message);
            }
            catch (Exception exception)
            {
                WriteLog(taskInstance.Id, taskInstance == null ? "" : taskInstance.TaskId, $"Error: {exception.Message}, Stack: {exception.StackTrace}");
            }
            finally
            {
                WriteLog(taskInstance.Id, taskInstance == null ? "" : taskInstance.TaskId, $"Process is terminating (Status={(taskResult == null ? "Unknown" : taskResult.Status.ToString())}, Result={result})");
            }

            return result;

            //----------------
            // Local functions
            bool IsTaskSuccessful() =>
                taskResult != null &&
                taskResult.Status == TaskInstanceStatues.CompletedSuccess;
        }

        private static void WriteLog(Guid taskInstanceId, string taskId, string message)
        {
            const char delimiter = (char)9;

            _ = Directory.CreateDirectory(_logFolder);

            var logTimeStamp = DateTime.UtcNow;

            if (_syncMutex.WaitOne())
            {
                try
                {
                    using (var writer = new StreamWriter($@"{_logFolder}\{logTimeStamp:dd-MM-yyyy}.tasks.txt", true))
                    {
                        writer.WriteLine($"{Log(logTimeStamp)}{delimiter}{taskInstanceId}{delimiter}{taskId}{delimiter}{message}");
                    }
                }
                catch (Exception e)
                {
                    using (var writer = new StreamWriter($@"{_logFolder}\{logTimeStamp:dd-MM-yyyy}.err.tasks.txt", true))
                    {
                        writer.WriteLine($"{Log(logTimeStamp)}{delimiter}{taskInstanceId}{delimiter}{taskId}{delimiter}{message}");
                        writer.WriteLine($"{Log(logTimeStamp)}{delimiter}{taskInstanceId}{delimiter}{taskId}{delimiter}{e.Message}");
                    }
                }
                finally
                {
                    _syncMutex.ReleaseMutex();
                }
            }

            //----------------
            // Local functions
            string Log(DateTime value) => value
                .ToUniversalTime()
                .ToString("O", CultureInfo.InvariantCulture);
        }

        private static string StripStartEndChars(string input, char[] chars)
        {
            if (!string.IsNullOrEmpty(input))
            {
                foreach (char currentChar in chars)
                {
                    bool found = false;

                    if (input[0] == currentChar)
                    {
                        input = input.Substring(1);
                        found = true;
                    }

                    if (input[input.Length - 1] == currentChar)
                    {
                        input = input.Substring(0, input.Length - 1);
                        found = true;
                    }

                    if (found)
                    {
                        break;
                    }
                }
            }
            return input;
        }

        private static ILifetimeScope SetupAutofac(IConfiguration applicationConfiguration, Guid taskInstanceId)
        {
            var builder = new ContainerBuilder();

            _ = builder.AddAutoMapper(
                typeof(AutoMapperConfig).Assembly,
                typeof(AccessTokenProfile).Assembly,
                typeof(SynchronizationProfile).Assembly);

            _ = builder.RegisterInstance(applicationConfiguration).As<IConfiguration>();
            _ = builder.RegisterInstance(SystemClock.Instance).As<IClock>();
            _ = builder.RegisterType<InMemoryCache>().As<ICache>().InstancePerLifetimeScope();

            _ = builder.RegisterModule(new CloudModule(applicationConfiguration));
            _ = builder.RegisterModule<TaskExecutorModule>();
            _ = builder.RegisterModule(new MasterModule(applicationConfiguration));

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var taskInstanceRepository = scope.Resolve<ITaskInstanceRepository>();
                var tenantRepository = scope.Resolve<ITenantsRepository>();
                var featureSettingsProvider = scope.Resolve<IFeatureSettingsProvider>();

                WriteLog(taskInstanceId, "", "Loading task instance");
                var taskInstance = taskInstanceRepository.Get(taskInstanceId);
                if (taskInstance is null)
                {
                    throw new InvalidOperationException($"Task instance '{taskInstanceId}' does not exist.");
                }

                WriteLog(taskInstanceId, taskInstance.TaskId, "Loaded task instance");
                WriteLog(taskInstanceId, taskInstance.TaskId, "Loading task instance tenant");

                var tenant = tenantRepository.GetById(taskInstance.TenantId);
                if (tenant is null)
                {
                    throw new InvalidOperationException($"Tenant '{taskInstance.TenantId}' does not exist. Task instance: '{taskInstanceId}'.");
                }

                WriteLog(taskInstanceId, taskInstance.TaskId, $"Loaded task instance tenant: id '{tenant.Id}'");

                WriteLog(taskInstanceId, taskInstance.TaskId, "Loading tenant features");
                var featureManager = new FeatureManager(featureSettingsProvider.GetForTenant(tenant.Id));
                WriteLog(taskInstanceId, taskInstance.TaskId,
                    $"Loaded tenant features: {featureManager.Features.Count}");
                var mainScope = container.BeginLifetimeScope(b =>
                {
                    _ = b.Register(context => new HubNotificationStub<RunNotification>())
                        .As<IHubNotification<RunNotification>>().InstancePerLifetimeScope();
                    _ = b.Register(context => new HubNotificationStub<ScenarioNotificationModel>())
                        .As<IHubNotification<ScenarioNotificationModel>>()
                        .InstancePerLifetimeScope();
                    _ = b.Register(context => new HubNotificationStub<LandmarkRunStatusNotification>())
                        .As<IHubNotification<LandmarkRunStatusNotification>>()
                        .InstancePerLifetimeScope();
                    _ = b.RegisterType<HubNotificationStub<InfoMessageNotification>>()
                        .As<IHubNotification<InfoMessageNotification>>()
                        .InstancePerLifetimeScope();

                    _ = b.RegisterModule(new CoreAutofacModule(_rootFolder));
                    _ = b.RegisterModule(new LandmarkRunServicesModule(applicationConfiguration, featureManager));
                    _ = b.RegisterModule(new DefaultAuditEventModule(_logFolder));
                    _ = b.RegisterModule(new OutputFilesProcessingModule());
                    _ = b.RegisterModule(new TenantModule(applicationConfiguration, featureManager, taskInstance, tenant));
                });
                mainScope.CurrentScopeEnding += (sender, args) => container.Dispose();
                return mainScope;
            }
        }

        private static string ComposeLogMessage(AggregateException aggregate)
        {
            var messageBuilder = new StringBuilder().AppendLine("Error: aggregate exception occurred, Stack traces:");

            int count = 1;
            foreach (var exception in aggregate.InnerExceptions)
            {
                _ = messageBuilder.AppendLine($"\t{count++})\t{exception}");
            }

            return messageBuilder.ToString();
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _syncMutex?.Dispose();
                    _syncMutex = null;
                }

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Program() { // Do not change this code. Put cleanup code in
        // Dispose(bool disposing) above. Dispose(false); }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool
            // disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above. GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
