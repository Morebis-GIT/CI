using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Audit;
using xggameplan.core.AuditEvents.AuditEvents;

namespace xggameplan.AuditEvents
{
    public static class AuditEventFactory
    {
        public static AuditEvent CreateAuditEventForInformationMessage(int tenantId, int userId, string message)
        {
            var auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.InformationMessage,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });

            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForWarningMessage(int tenantId, int userId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.WarningMessage,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForGameplanPipelineStart(int tenantId, int userId, int pipelineEventId, Guid? runId, Guid? scenarioId, string autoBookId,
                                                                         string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRun,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanPipelineEventID, Value = pipelineEventId });
            if (runId != null && runId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            }
            if (scenarioId != null && scenarioId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanScenarioID, Value = scenarioId });
            }
            if (autoBookId != null && autoBookId != string.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanAutoBookID, Value = autoBookId });
            }
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForGameplanPipelineEnd(int tenantId, int userId, int pipelineEventId, Guid? runId, Guid? scenarioId, string autoBookId,
                                                                         string message, string pipelineErrorMessage, Exception exception = null)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRun,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanPipelineEventID, Value = pipelineEventId });
            if (runId != null && runId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            }
            if (scenarioId != null && scenarioId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanScenarioID, Value = scenarioId });
            }
            if (autoBookId != null && autoBookId != string.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanAutoBookID, Value = autoBookId });
            }
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            if (!String.IsNullOrEmpty(pipelineErrorMessage))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanPipelineEventErrorMessage, Value = pipelineErrorMessage });
            }
            if (exception != null)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Exception, Value = ExceptionModel.MapFrom(exception) });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForSmoothPipelineStart(
            int tenantId,
            int userId,
            int pipelineEventId,
            Guid? runId,
            Guid? scenarioId,
            Guid? autoBookId,
            string message
            )
        {
            var auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanSmoothRun,
                TenantID = tenantId,
                UserID = userId,
                Values = {
                    new AuditEventValue{
                        TypeID = AuditEventValueTypes.GamePlanPipelineEventID,
                        Value = pipelineEventId
                    }
                }
            };

            if (runId != null && runId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            }
            if (scenarioId != null && scenarioId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanScenarioID, Value = scenarioId });
            }
            if (autoBookId != null && autoBookId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanAutoBookID, Value = autoBookId });
            }
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }

            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForSmoothPipelineEnd(int tenantId, int userId, int pipelineEventId, Guid? runId, Guid? scenarioId, Guid? autoBookId,
                                                                      string message, string pipelineErrorMessage, Exception exception = null)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanSmoothRun,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanPipelineEventID, Value = pipelineEventId });
            if (runId != null && runId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            }
            if (scenarioId != null && scenarioId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanScenarioID, Value = scenarioId });
            }
            if (autoBookId != null && autoBookId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanAutoBookID, Value = autoBookId });
            }
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            if (!String.IsNullOrEmpty(pipelineErrorMessage))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanPipelineEventErrorMessage, Value = pipelineErrorMessage });
            }
            if (exception != null)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Exception, Value = ExceptionModel.MapFrom(exception) });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForException(int tenantId, int userId, string message, Exception exception)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.Exception,
                TenantID = tenantId,
                UserID = userId
            };
            //auditEvent.Values.Add(new AuditEventValue() { TypeID = ValueTypeException, Value = ExceptionModel.MapFrom(exception) });
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            //auditEvent.Values.Add(new AuditEventValue() { TypeID = ValueTypeException, Value = exception });
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Exception, Value = ExceptionModel.MapFrom(exception) });
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForAutoBookEvent(int tenantId, int userId, string autoBookId, int autoBookEventId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanAutoBookEvent,
                TenantID = tenantId,
                UserID = userId
            };
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanAutoBookID, Value = autoBookId });
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanAutoBookEventID, Value = autoBookEventId });
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunValidationFailure(int tenantId, int userId, Guid runId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRunValidationFailure,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunValidationWarning(int tenantId, int userId, Guid runId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.WarningMessage,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForScheduleDataUploadStarted(int tenantId, int userId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanScheduleDataUploadStarted,
                TenantID = tenantId,
                UserID = userId
            };
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunStarted(int tenantId, int userId, Guid runId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRunStarted,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunStartFailed(int tenantId, int userId, Guid runId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRunStartFailed,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunCompleted(int tenantId, int userId, Guid runId, string message, Guid? scenarioId = null)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRunCompleted,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            if (scenarioId != null && scenarioId != Guid.Empty)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanScenarioID, Value = scenarioId.Value });
            }
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunCreated(int tenantId, int userId, Guid runId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRunCreated,
                TenantID = tenantId,
                UserID = userId
            };
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanRunID, Value = runId });
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForSystemState(int tenantId, int userId, string message, SystemState systemState)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanSystemState,
                TenantID = tenantId,
                UserID = userId
            };
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.GamePlanSystemState, Value = systemState });
            return auditEvent;
        }

        public static AuditEvent CreateAuditEventForRunDeleted(int tenantId, int userId, Guid runId, string message)
        {
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = DateTime.UtcNow,
                EventTypeID = AuditEventTypes.GamePlanRunDeleted,
                TenantID = tenantId,
                UserID = userId
            };

            auditEvent.Values.Add(new AuditEventValue(AuditEventValueTypes.GamePlanRunID, runId));

            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }

            return auditEvent;
        }

        internal static AuditEvent CreateAuditEventForPreRunRecalculateBreakAvailabilityStart(
            int tenantId,
            int userId,
            Guid runId,
            Guid scenarioId
        ) => new PreRunRecalculateBreakAvailabilityStartAuditEvent(tenantId, userId)
        {
            Values = {
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanRunID,
                    runId
                ),
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanScenarioID,
                    scenarioId
                )
            }
        };

        internal static AuditEvent CreateAuditEventForPreRunRecalculateBreakAvailabilityEnd(
            int tenantId,
            int userId,
            Guid runId,
            Guid scenarioId
        ) => new PreRunRecalculateBreakAvailabilityEndAuditEvent(tenantId, userId)
        {
            Values = {
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanRunID,
                    runId
                ),
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanScenarioID,
                    scenarioId
                )
            }
        };

        internal static AuditEvent CreateAuditEventForPostSmoothRecalculateBreakAvailabilityStart(
            int tenantId,
            int userId,
            Guid runId,
            Guid scenarioId
        ) => new PostSmoothRecalculateBreakAvailabilityStartAuditEvent(tenantId, userId)
        {
            Values = {
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanRunID,
                    runId
                ),
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanScenarioID,
                    scenarioId
                )
            }
        };

        internal static AuditEvent CreateAuditEventForPostSmoothRecalculateBreakAvailabilityEnd(
            int tenantId,
            int userId,
            Guid runId,
            Guid scenarioId
        ) => new PostSmoothRecalculateBreakAvailabilityEndAuditEvent(tenantId, userId)
        {
            Values = {
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanRunID,
                    runId
                ),
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanScenarioID,
                    scenarioId
                )
            }
        };
    }
}
