using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.AuditEvents;
using xggameplan.core.RunManagement.Notifications;
using xggameplan.RunManagement.Notifications;
using xggameplan.core.Helpers;

namespace xggameplan.core.RunManagement
{
    public class RunCompletionNotifier
    {
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly INotificationCollection _notifications;
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;

        public RunCompletionNotifier(
            ITenantSettingsRepository tenantSettingsRepository,
            IAuditEventRepository auditEventRepository,
            INotificationCollection notifications,
            IPipelineAuditEventRepository pipelineAuditEventRepository)
        {
            _tenantSettingsRepository = tenantSettingsRepository;
            _auditEventRepository = auditEventRepository;
            _notifications = notifications;
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
        }

        /// <summary>
        /// Handles run completed, all scenarios
        /// </summary>
        /// <param name="run"></param>
        /// <param name="success"></param>
        public void Notify(Run run, bool success)
        {
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            bool notificationsConfigured = false;

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunCompleted(0, 0, run.Id, success ? "Completed successfully" : "Completed with errors"));

            // Get run event settings
            if (tenantSettings.RunEventSettings != null)
            {
                RunEventSettings runEventSettings = tenantSettings.RunEventSettings.Where(item => item.EventType == RunEvents.RunCompleted).FirstOrDefault();
                if (runEventSettings != null)
                {
                    var exceptions = new List<Exception>();

                    try
                    {
                        // Check HTTP notification
                        INotification<HTTPNotificationSettings> httpNotification = _notifications?.GetNotification<HTTPNotificationSettings>();
                        if (runEventSettings.HTTP != null && httpNotification != null && runEventSettings.HTTP.Enabled)
                        {
                            notificationsConfigured = true;
                            try
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0, 
                                    PipelineEventIDs.STARTED_NOTIFYING_MULE_SOFT_API, run.Id, null, null, string.Format("Generating HTTP notification for run completed (RunID={0}, Success={1})", run.Id, success)));
                                
                                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                    PipelineEventIDs.STARTED_NOTIFYING_MULE_SOFT_API, run.Id, Guid.Empty, null));

                                httpNotification.RunCompleted(run, success, runEventSettings.HTTP);
                                
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, 
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, null, null, string.Format("Generated HTTP notification for run completed (RunID={0}, Success={1})", run.Id, success), null, null));

                                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, Guid.Empty, null));
                            }
                            catch (System.Exception exception)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, 
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, null, null, string.Format("Failed to generate HTTP notification for run completed (RunID={0}, Success={1})", run.Id, success), exception.Message, exception));

                                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, Guid.Empty, exception.Message));
                                throw;
                            }
                            finally
                            {
                                _pipelineAuditEventRepository.SaveChanges();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        exceptions.Add(exception);
                    }

                    if (exceptions.Count == 1)
                    {
                        throw exceptions[0];
                    }
                    else if (exceptions.Count > 0)
                    {
                        throw new AggregateException(exceptions);
                    }
                }
            }

            // Log warning if notifications not enabled so that there's no dispute over why they weren't generated
            if (!notificationsConfigured)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, string.Format("Not generating notification for run completed because notifications are not configured or enabled (RunID={0}, Success={1})", run.Id, success)));
            }
        }

        /// <summary>
        /// Handles run scenario completed
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenario"></param>
        /// <param name="success"></param>
        public void Notify(Run run, RunScenario scenario, bool success)
        {
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            bool notificationsConfigured = false;

            // Get run scenario event settings
            if (tenantSettings.RunEventSettings != null)
            {
                RunEventSettings runEventSettings = tenantSettings.RunEventSettings.Where(item => item.EventType == RunEvents.RunScenarioCompleted).FirstOrDefault();
                if (runEventSettings != null)
                {
                    var exceptions = new List<Exception>();

                    try
                    {
                        // Check HTTP notification
                        INotification<HTTPNotificationSettings> httpNotification = _notifications?.GetNotification<HTTPNotificationSettings>();
                        if (runEventSettings.HTTP != null && httpNotification != null && runEventSettings.HTTP.Enabled)
                        {
                            notificationsConfigured = true;
                            try
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0, 
                                    PipelineEventIDs.STARTED_NOTIFYING_MULE_SOFT_API, run.Id, scenario.Id, null, string.Format("Generating HTTP notification for run completed (RunID={0}, ScenarioID={1}, Success={2})", run.Id, scenario.Id, success)));

                                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                    PipelineEventIDs.STARTED_NOTIFYING_MULE_SOFT_API, run.Id, scenario.Id, null));

                                httpNotification.RunCompleted(run, scenario, success, runEventSettings.HTTP);

                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, 
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, scenario.Id, null, string.Format("Generated HTTP notification for run completed (RunID={0}, ScenarioID={1}, Success={2})", run.Id, scenario.Id, success), null, null));

                                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun, 
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, scenario.Id, null));
                            }
                            catch (System.Exception exception)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, 
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, scenario.Id, null, string.Format("Failed to generate HTTP notification for run completed (RunID={0}, ScenarioID={1}, Success={2})", run.Id, scenario.Id, success), exception.Message, exception));

                                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                    PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, run.Id, scenario.Id, exception.Message));

                                throw;
                            }
                            finally
                            {
                                _pipelineAuditEventRepository.SaveChanges();
                            }
                        }
                    }
                    catch (System.Exception exception)
                    {
                        exceptions.Add(exception);
                    }

                    if (exceptions.Count == 1)
                    {
                        throw exceptions[0];
                    }
                    else if (exceptions.Count > 0)
                    {
                        throw new AggregateException(exceptions);
                    }
                }
            }

            // Log warning if notifications not enabled so that there's no dispute over why they weren't generated
            if (!notificationsConfigured)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                    $"Not generating notification for run completed because notifications are not configured or enabled (RunID={run.Id}, ScenarioID={scenario.Id}, Success={success})"));
            }
        }
    }
}
