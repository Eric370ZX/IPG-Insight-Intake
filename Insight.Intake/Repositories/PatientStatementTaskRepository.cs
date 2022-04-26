using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Repositories
{
    public static class PSEvents
    {
        public const string CaseApproved = "Case Approved";
        public const string PromotedToCollection1 = "Promoted to Collections1";
        public const string PromotedToCollection2 = "Promoted to Collections2";
        public const string CarrierPayment = "Carrier Payment";
        public const string A2Generated = "A2 Generated";
        public const string A3Generated = "A3 Generated";
        public const string A5Generated = "A5 Generated";
        public const string S1Generated = "S1 Generated";
        public const string S3Generated = "S3 Generated";
    }

    public static class PSType
    {
        public const string P1 = "P1";
        public const string A2 = "A2";
        public const string A3 = "A3";
        public const string A5 = "A5";
        public const string D1 = "D1";
        public const string D2 = "D2";
        public const string S1 = "A5";
        public const string S3 = "S1";
    }

    public class PatientStatementTaskRepository
    {
        private readonly IOrganizationService _crmService;
        private readonly ITracingService _tracingService;
        private readonly DocumentRepository _docRepo;

        public PatientStatementTaskRepository(IOrganizationService crmService, ITracingService tracing)
        {
            _crmService = crmService;
            _tracingService = tracing;
            _docRepo = new DocumentRepository(crmService);
        }

        public List<ipg_statementgenerationtask> GetActiveStatements(EntityReference caseRef, ColumnSet columnSet, params string[] statementNames)
        {
            var statementEventFilters = new FilterExpression();

            if(statementNames.Any())
            {
                statementEventFilters.Conditions.Add(new ConditionExpression(ipg_statementgenerationeventconfiguration.PrimaryNameAttribute, ConditionOperator.In, statementNames));
            }

            return _crmService.RetrieveMultiple(new QueryExpression(ipg_statementgenerationtask.EntityLogicalName)
            {
                ColumnSet = columnSet,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_caseid, ConditionOperator.Equal, caseRef.Id),
                        new ConditionExpression(ipg_statementgenerationtask.Fields.StateCode, ConditionOperator.Equal, (int)ipg_statementgenerationtaskState.Active),
                        new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_IsSent, ConditionOperator.NotEqual, true)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_statementgenerationtask.EntityLogicalName, ipg_statementgenerationeventconfiguration.EntityLogicalName
                    , ipg_statementgenerationtask.Fields.ipg_eventid, ipg_statementgenerationeventconfiguration.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        LinkCriteria = statementEventFilters
                    }
                }
            }).Entities.Select(e => e.ToEntity<ipg_statementgenerationtask>()).ToList();
        }
        public List<ipg_statementgenerationtask> GetActiveStatements(EntityReference caseRef, ColumnSet columnSet = null)
        {
            return _crmService.RetrieveMultiple(new QueryExpression(ipg_statementgenerationtask.EntityLogicalName)
            {
                ColumnSet = columnSet ?? new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_caseid, ConditionOperator.Equal, caseRef.Id),
                        new ConditionExpression(ipg_statementgenerationtask.Fields.StateCode, ConditionOperator.Equal, (int)ipg_statementgenerationtaskState.Active),
                        new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_IsSent, ConditionOperator.NotEqual, true)
                    }
                }
            }).Entities.Select(e => e.ToEntity<ipg_statementgenerationtask>()).ToList();
        }

        public void CancelStatementTasks(EntityReference caseRef, params string[] statementNames)
        {
            var openTasks = GetActiveStatements(caseRef, new ColumnSet(false), statementNames);

            openTasks.ForEach(t => _crmService.Update(new ipg_statementgenerationtask() { Id = t.Id, StateCode = ipg_statementgenerationtaskState.Inactive, StatusCodeEnum = ipg_statementgenerationtask_StatusCode.Canceled }));
        }

        public void CancelAllStatementTasks(EntityReference caseRef)
        {
            var openTasks = _crmService.RetrieveMultiple(new QueryExpression(ipg_statementgenerationtask.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_caseid, ConditionOperator.Equal, caseRef.Id),
                        new ConditionExpression(ipg_statementgenerationtask.Fields.StateCode, ConditionOperator.Equal, (int)ipg_statementgenerationtaskState.Active),
                        new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_IsSent, ConditionOperator.NotEqual, true)
                    }
                },
            }).Entities.Select(e => e.ToEntity<ipg_statementgenerationtask>()).ToList();

            openTasks.ForEach(t => _crmService.Update(new ipg_statementgenerationtask() { Id = t.Id, StateCode = ipg_statementgenerationtaskState.Inactive, StatusCodeEnum = ipg_statementgenerationtask_StatusCode.Canceled }));
        }

        public void CreateTask(EntityReference caseRef, ipg_statementgenerationeventconfiguration config, DateTime date)
        {
            var statementTask = new ipg_statementgenerationtask()
            {
                ipg_eventid = new EntityReference(ipg_statementgenerationeventconfiguration.EntityLogicalName, config.Id),
                ipg_caseid = caseRef,
                ipg_StartDate = date.AddDays(config.ipg_EventDueDaysStart ?? 0),
                ipg_EndDate = config.ipg_EventDueDaysEnd.HasValue
                       ? date.AddDays(config.ipg_EventDueDaysEnd ?? 0)
                       : (DateTime?)null
            };

            _crmService.Create(statementTask);
        }

        public ipg_statementgenerationeventconfiguration FindStatementGenerationConfig(string statementConfigName)
        {
            var queryExpression = new QueryExpression(ipg_statementgenerationeventconfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                    ipg_statementgenerationeventconfiguration.PrimaryIdAttribute,
                    ipg_statementgenerationeventconfiguration.Fields.ipg_EventDueDaysStart,
                    ipg_statementgenerationeventconfiguration.Fields.ipg_EventDueDaysEnd
                    ),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(ipg_statementgenerationeventconfiguration.PrimaryNameAttribute, ConditionOperator.Equal, statementConfigName),
                            new ConditionExpression(ipg_statementgenerationeventconfiguration.Fields.StateCode, ConditionOperator.Equal, (int)ipg_statementgenerationeventconfigurationState.Active)
                        }
                },
                TopCount = 1
            };

            EntityCollection eventConfigs = _crmService.RetrieveMultiple(queryExpression);
            Entity eventConfig = eventConfigs.Entities.FirstOrDefault();
            if (eventConfig != null)
            {
                return eventConfig.ToEntity<ipg_statementgenerationeventconfiguration>();
            }

            return null;
        }

        public ipg_statementgenerationtask FindOpenStatementGenerationTask(EntityReference incidentReference, ipg_statementgenerationeventconfiguration statementConfig)
        {
            var queryExpression = new QueryExpression(ipg_statementgenerationtask.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                   nameof(ipg_statementgenerationtask.ipg_statementgenerationtaskId).ToLower()
                   ),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(ipg_statementgenerationtask.Fields.StateCode, ConditionOperator.Equal, (int)ipg_statementgenerationtaskState.Active),
                            new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_IsSent, ConditionOperator.NotEqual, true),
                            new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_eventid, ConditionOperator.Equal, statementConfig.Id),
                            new ConditionExpression(ipg_statementgenerationtask.Fields.ipg_caseid, ConditionOperator.Equal, incidentReference.Id),
                        }
                },
                TopCount = 1
            };

            EntityCollection entityCollection = _crmService.RetrieveMultiple(queryExpression);
            if (entityCollection.Entities.Any())
            {
                return entityCollection.Entities.FirstOrDefault().ToEntity<ipg_statementgenerationtask>();
            }

            return null;
        }

        public void CreateStatementGenerationTaskIfNotExists(EntityReference incidentReference, string eventName)
        {
            _tracingService.Trace("Searching for a statement generation config " + eventName);
            var statementGenerationConfig = FindStatementGenerationConfig(eventName);
            if (statementGenerationConfig == null)
            {
                throw new InvalidPluginExecutionException($"Could not find this statement generation config: {eventName}");
            }

            _tracingService.Trace("Searching for an open statement generation task");
            var existingOpenStatementGenerationTask = FindOpenStatementGenerationTask(incidentReference, statementGenerationConfig);
            if (existingOpenStatementGenerationTask == null)
            {
                _tracingService.Trace("Creating a new statement generation task");
                DateTime date = DateTime.Today;
                
                if (eventName == PSEvents.CarrierPayment)
                {
                    var openPS = _docRepo.GetActivePSDoc(incidentReference, PSType.A2);

                    if (openPS == null)
                    {
                        CancelAllStatementTasks(incidentReference);
                        _crmService.Update(new Incident() { Id = incidentReference.Id, ipg_LastStatementType = statementGenerationConfig.ToEntityReference(), ipg_LastStatementGeneratedDate = DateTime.Now });
                        CreateTask(incidentReference, statementGenerationConfig, date);
                    }
                }
                else
                {
                    CreateTask(incidentReference, statementGenerationConfig, date);
                }
            }
        }
    }
}
