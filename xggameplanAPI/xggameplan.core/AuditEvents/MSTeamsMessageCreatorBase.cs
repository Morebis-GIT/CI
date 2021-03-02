using System;
using System.Collections.Generic;
using System.Data;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;

namespace xggameplan.AuditEvents
{
    public abstract class MSTeamsMessageCreatorBase
    {
        protected void AddValuesRow(DataTable dataTable, string item, string value)
        {
            DataRow row = dataTable.NewRow();
            row[0] = item;
            row[1] = value;
            dataTable.Rows.Add(row);
        }

        protected DataTable GetValuesDataTable(AuditEvent auditEvent, Run run, AuditEventType auditEventType, string subEvent, AuditEventValueTypeRepository auditEventValueTypeRepository,
                                            List<AuditEventValueConverter> valueConverters)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Item", typeof(String));
            dataTable.Columns.Add("Value", typeof(String));

            //string runUrl = string.Format(@"{0}/#/manage-runs", _frontendUrl);

            // Add standard header
            //AddValuesRow(dataTable, "Time", auditEvent.TimeCreated.ToString());
            //AddValuesRow(dataTable, "Event ID", auditEvent.ID);
            //AddValuesRow(dataTable, "Event", string.IsNullOrEmpty(subEvent) ? auditEventType.Description : string.Format("{0} - {1}", auditEventType.Description, subEvent));

            // Add values from Run ID
            if (run != null)
            {
                AddValuesRow(dataTable, "Run", string.Format("{0} ({1})", run.Description, run.Id.ToString()));
                //AddValuesRow(dataTable, "Description", run.Description);
                //AddValuesRow(dataTable, "Date Range", string.Format("{0} - {1}", run.StartDateTime.ToString(), run.EndDateTime.ToString()));
                //AddValuesRow(dataTable, "Created", run.CreatedDateTime.ToString());
                AddValuesRow(dataTable, "Executed", run.ExecuteStartedDateTime == null ? "n/a" : run.ExecuteStartedDateTime.Value.ToString());
                if (run.CompletedScenarios.Count == run.Scenarios.Count && run.LastScenarioCompletedDateTime != null)
                {
                    AddValuesRow(dataTable, "Completed", run.LastScenarioCompletedDateTime.Value.ToString());
                }
            }

            // Add other values
            HashSet<int> valueTypesDone = new HashSet<int>() { AuditEventValueTypes.GamePlanRunID, AuditEventValueTypes.GamePlanPipelineEventID, AuditEventValueTypes.GamePlanAutoBookEventID };
            foreach (AuditEventValue auditEventValue in auditEvent.Values)
            {
                if (!valueTypesDone.Contains(auditEventValue.TypeID))   // Ignore values done above
                {
                    AuditEventValueType auditEventValueType = auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                    if (!auditEventValueType.Internal)
                    {
                        AuditEventValueConverter auditEventValueConverter = valueConverters.Find(current => current.Handles(auditEventValue.TypeID));
                        if (auditEventValueConverter != null)
                        {
                            AddValuesRow(dataTable, auditEventValueType.Description, auditEventValueConverter.ValueConverter.Convert(auditEventValue.Value, auditEventValueType.Type, typeof(String)).ToString());
                        }
                        else
                        {
                            AddValuesRow(dataTable, auditEventValueType.Description, auditEventValue.Value.ToString());
                        }
                    }
                }
            }
            return dataTable;
        }

        protected string GetValueDescription(AuditEvent auditEvent, int valueTypeId, AuditEventValueTypeRepository auditEventValueTypeRepository, List<AuditEventValueConverter> valueConverters)
        {
            string eventDescription = "";
            AuditEventValue auditEventValue = auditEvent.GetValueByValueTypeId(valueTypeId);
            if (auditEventValue != null && auditEventValue.Value != null)
            {
                AuditEventValueType auditEventValueType = auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                AuditEventValueConverter auditEventValueConverter = valueConverters.Find(current => current.Handles(auditEventValue.TypeID));
                if (auditEventValueConverter != null)   // Sanity check
                {
                    eventDescription = auditEventValueConverter.ValueConverter.Convert(auditEventValue.Value, auditEventValueType.Type, typeof(String)).ToString();
                }
            }
            return eventDescription;
        }
    }
}

  
