using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Insight.Intake.Helpers;
using Insight.Intake.Repositories;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;
using System.Text.RegularExpressions;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Managers
{
    public class TaskManager
    {
        private readonly SettingsHelper _settingsHelper;

        private readonly CalendarRepository _calendarRepository;
        private readonly TaskCategoryRepository _taskCategoryRepository;
        private readonly TaskTypeRepository _taskTypeRepository;
        private readonly TaskRepository _taskRepository;
        private readonly DocumentRepository _docRepository;

        private readonly IOrganizationService _crmService;
        private readonly ITracingService _tracingService;
        private readonly EntityReference _taskRef;
        private readonly Guid _initiatingUserId;


        public TaskManager(IOrganizationService organizationService, ITracingService tracingService, EntityReference taskRef, Guid initiatingUserId)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;
            this._taskRef = taskRef;
            this._initiatingUserId = initiatingUserId;

            _settingsHelper = new SettingsHelper(organizationService);
            _calendarRepository = new CalendarRepository(organizationService);
            _taskCategoryRepository = new TaskCategoryRepository(organizationService);
            _taskTypeRepository = new TaskTypeRepository(organizationService);
            _taskRepository = new TaskRepository(organizationService);
            _docRepository = new DocumentRepository(organizationService);
        }

        public TaskManager(IOrganizationService organizationService, ITracingService tracingService)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;
            _settingsHelper = new SettingsHelper(organizationService);
            _calendarRepository = new CalendarRepository(organizationService);
            _taskCategoryRepository = new TaskCategoryRepository(organizationService);
            _taskTypeRepository = new TaskTypeRepository(organizationService);
            _taskRepository = new TaskRepository(organizationService);
        }

        public Task GetRelatedTask(EntityReference entityReference, string taskTypeName, ColumnSet columnSet = null)
        {
            return _crmService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = columnSet ?? new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, entityReference.Id),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(Task.EntityLogicalName, ipg_tasktype.EntityLogicalName, Task.Fields.ipg_tasktypeid, Task.Fields.ipg_tasktypeid, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_tasktype.Fields.ipg_name, ConditionOperator.Equal, taskTypeName)
                            }
                        }
                    }
                }

            }).Entities.FirstOrDefault()?.ToEntity<Task>();
        }

        public Task GetOpenTask(EntityReference entityReference, EntityReference taskTypeId, ColumnSet columnSet = null)
        {
            var regardingCheck = entityReference != null ? new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, entityReference.Id)
                : new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Null);
            var taskType = _crmService.Retrieve(taskTypeId.LogicalName, taskTypeId.Id, new ColumnSet(ipg_tasktype.Fields.ipg_taskcategoryid))?.ToEntity<ipg_tasktype>();

            var query = new QueryExpression(Task.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = columnSet ?? new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        regardingCheck,
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                    }
                }
            };

            if(taskType.ipg_taskcategoryid?.Name == TaskCategoryNames.CarrierOutreach || taskType.ipg_taskcategoryid?.Name == TaskCategoryNames.PatientOutreach)
            {
                query.Criteria.AddCondition(new ConditionExpression(Task.Fields.ipg_taskcategoryid, ConditionOperator.Equal, taskType.ipg_taskcategoryid.Id));
            }
            else
            {
                query.Criteria.AddCondition(new ConditionExpression(Task.Fields.ipg_tasktypeid, ConditionOperator.Equal, taskType.Id));
            }

            return  _crmService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Task>();
        }

        public TaskManager(IOrganizationService organizationService, ITracingService tracingService, EntityReference taskRef = null)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;
            this._taskRef = taskRef;

            _settingsHelper = new SettingsHelper(organizationService);
            _calendarRepository = new CalendarRepository(organizationService);
            _taskCategoryRepository = new TaskCategoryRepository(organizationService);
            _taskTypeRepository = new TaskTypeRepository(organizationService);
            _taskRepository = new TaskRepository(organizationService);
        }

        internal void SyncStatuses(Task target)
        {
            if (target.ipg_reviewstatuscodeEnum == Task_ipg_reviewstatuscode.PendingReview)
            {
                target.StatusCodeEnum = Task_StatusCode.PendingApproval;
            }
        }

        internal void CreateCOTask(EntityReference ipg_CaseId, TaskTypeIds claimResponseFullDenial, string invoicenumber)
        {
            var taskType = GetTaskTypeById(claimResponseFullDenial);
            var description = taskType.ipg_description?.Replace("<Claim #>", invoicenumber);

            CreateTask(ipg_CaseId, taskType.ToEntityReference(), new Task() { Description = description });
        }

        public bool IsOpenTask(Task target)
        {
            Task existingTask;
            return IsOpenTask(target, out existingTask);
        }
        public bool IsOpenTask(Task target, out Task existingTask)
        {
            if (target.RegardingObjectId != null && (target.ipg_tasktypeid != null || target.ipg_tasktypecode != null))
            {
                var queryExpression = new QueryExpression(Task.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                    {
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, target.RegardingObjectId.Id),
                    }
                    }
                };

                if (target.ipg_carrierid != null)
                {
                    queryExpression.Criteria.AddCondition(Task.Fields.ipg_carrierid, ConditionOperator.Equal, target.ipg_carrierid.Id);
                }

                if (target.ipg_facilityid != null)
                {
                    queryExpression.Criteria.AddCondition(Task.Fields.ipg_facilityid, ConditionOperator.Equal, target.ipg_facilityid.Id);
                }
                string categoryName = "";
                if (target.ipg_taskcategoryid != null)
                {
                    categoryName = target.ipg_taskcategoryid.Name ??
                    _crmService.Retrieve(target.ipg_taskcategoryid.LogicalName, target.ipg_taskcategoryid.Id, new ColumnSet(ipg_taskcategory.Fields.ipg_name))
                    .ToEntity<ipg_taskcategory>().ipg_name;
                }

                if (target.ipg_tasktypeid != null)
                {
                    queryExpression.Criteria.AddCondition(new ConditionExpression(Task.Fields.ipg_tasktypeid, ConditionOperator.Equal, target.ipg_tasktypeid.Id));

                    if (target.ipg_tasksubcategoryid != null)
                    {
                        queryExpression.Criteria.AddCondition(new ConditionExpression(Task.Fields.ipg_tasksubcategoryid, ConditionOperator.Equal, target.ipg_tasksubcategoryid.Id));
                    }
                }
                else if(categoryName == TaskCategoryNames.CarrierOutreach || categoryName == TaskCategoryNames.PatientOutreach)
                {
                    queryExpression.Criteria.AddCondition(new ConditionExpression(Task.Fields.ipg_taskcategoryid, ConditionOperator.Equal, target.ipg_taskcategoryid.Id));
                }
                else if (target.ipg_tasktypecode != null)
                {
                    queryExpression.Criteria.AddCondition(new ConditionExpression(Task.Fields.ipg_tasktypecode, ConditionOperator.Equal, target.ipg_tasktypecode.Value));
                }

                var duplicates = _crmService.RetrieveMultiple(queryExpression);

                existingTask = duplicates.Entities.FirstOrDefault()?.ToEntity<Task>();

                return duplicates.Entities.Any();

            }
            else
            {
                throw new InvalidPluginExecutionException($"{nameof(Task.Fields.ipg_tasktypeid)} or {nameof(Task.Fields.ipg_tasktypecode)} is required");
            }
        }

        internal void CheckTaskForDuplicates(Task target)
        {
            Task existingTask;
            if (target.ipg_generatedbycodeEnum == ipg_GeneratedByCode.User && IsOpenTask(target, out existingTask))
            {
                throw new InvalidPluginExecutionException($"There is still an open task \"{existingTask.Subject}\" on the Case");
            }
        }

        public Task ConfigureTaskByTaskType(Task task)
        {
            if (task.ipg_tasktypeid != null)
            {
                ipg_tasktype taskType = _crmService.Retrieve(task.ipg_tasktypeid.LogicalName, task.ipg_tasktypeid.Id
                    , new ColumnSet(ipg_tasktype.Fields.ipg_name
                    , ipg_tasktype.Fields.ipg_tasktitle
                    , ipg_tasktype.Fields.ipg_startdate
                    , ipg_tasktype.Fields.ipg_duedate
                    , ipg_tasktype.Fields.ipg_assigntouserid
                    , ipg_tasktype.Fields.ipg_assigntoteam
                    , ipg_tasktype.Fields.ipg_documenttypeid
                    , ipg_tasktype.Fields.ipg_priority
                    , ipg_tasktype.Fields.ipg_isportal
                    , ipg_tasktype.Fields.ipg_subcategory
                    , ipg_tasktype.Fields.ipg_description
                    , ipg_tasktype.Fields.ipg_metatag
                    , ipg_tasktype.Fields.ipg_taskcategoryid
                    , ipg_tasktype.Fields.ipg_generatedbycode)).ToEntity<ipg_tasktype>();

                task.Subject = task.Subject ?? taskType.ipg_tasktitle ?? taskType.ipg_name;

                if (task.ScheduledStart == null || task.ScheduledEnd == null)
                {
                    Entity calendar = _calendarRepository.GetBusinessClosureCalendar();

                    task.ScheduledStart = task.ScheduledStart ??
                        (taskType.ipg_startdate.HasValue ? BusinessDayHelper.AddBusinessDays(DateTime.Now, taskType.ipg_startdate.Value, calendar) : DateTime.Now);
                    task.ScheduledEnd = task.ScheduledEnd ??
                        (taskType.ipg_duedate.HasValue ? BusinessDayHelper.AddBusinessDays(task.ScheduledStart.Value, taskType.ipg_duedate.Value, calendar) : task.ScheduledStart);
                }
                
                if (task.ipg_generatedbycodeEnum != ipg_GeneratedByCode.User && (taskType.ipg_assigntouserid != null || taskType.ipg_assigntoteam != null))
                {
                    task.OwnerId = taskType.ipg_assigntouserid ?? taskType.ipg_assigntoteam;
                }

                task.ipg_assignedtoteamid = task.ipg_assignedtoteamid ?? taskType.ipg_assigntoteam;



                task.Description = string.IsNullOrEmpty(task.Description) ? taskType.ipg_description : task.Description;
                task.ipg_priority = task.ipg_priority ?? taskType.ipg_priority;
                task.ipg_DocumentType = task.ipg_DocumentType ?? taskType.ipg_documenttypeid;
                task.Subcategory = task.Subcategory ?? taskType.ipg_subcategory;
                task.ipg_isvisibleonportal = task.ipg_isvisibleonportal ?? taskType.ipg_isportal;
                task.ipg_taskcategoryid = task.ipg_taskcategoryid ?? taskType.ipg_taskcategoryid;
                task.ipg_metatag = string.IsNullOrEmpty(task.ipg_metatag) ? taskType.ipg_metatag : task.ipg_metatag;
                task.ipg_generatedbycode = task.ipg_generatedbycode ?? taskType.ipg_generatedbycode;


                PopulateDocTypeBySubCategory(task);
                PopulatePlaceHolders(task);
            }

            return task;
        }

        public void PopulateDocTypeBySubCategory(Task task)
        {
            if (!string.IsNullOrWhiteSpace(task.Subcategory) && task.ipg_DocumentType == null)
            {
                task.ipg_DocumentType = _crmService.RetrieveMultiple(new QueryExpression(ipg_documenttype.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(ipg_documenttype.Fields.ipg_documenttypeId),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation, ConditionOperator.Equal, task.Subcategory)
                        }
                    }
                }).Entities.FirstOrDefault()?.ToEntityReference();
            }
        }


        public void PopulatePlaceHolders(Task task)
        {
            if (task.RegardingObjectId?.LogicalName == Intake.Contact.EntityLogicalName)
            {
                task.Description = task.Description?.Replace("<user name>", _crmService.Retrieve(task.RegardingObjectId.LogicalName, task.RegardingObjectId.Id, new ColumnSet(Intake.Contact.Fields.FullName)).ToEntity<Intake.Contact>().FullName);
            }

            if (task.Subject != null)
            {
                task.Subject = task.Subject.Replace("<Level>", $"{task.ipg_level ?? 1}");
            }

            if (task.RegardingObjectId?.LogicalName == ipg_casepartdetail.EntityLogicalName)
            {
                var actualPart = _crmService.Retrieve(task.RegardingObjectId.LogicalName, task.RegardingObjectId.Id, new ColumnSet(ipg_casepartdetail.Fields.ipg_caseid, ipg_casepartdetail.Fields.ipg_productid)).ToEntity<ipg_casepartdetail>();
                var product = actualPart.ipg_productid != null ? _crmService.Retrieve(actualPart.ipg_productid.LogicalName, actualPart.ipg_productid.Id, new ColumnSet(Intake.Product.Fields.Name, Intake.Product.Fields.ipg_manufacturerpartnumber, Intake.Product.Fields.ipg_manufacturerid)).ToEntity<Intake.Product>() : null;
                var incident = actualPart.ipg_caseid != null ? _crmService.Retrieve(actualPart.ipg_caseid.LogicalName, actualPart.ipg_caseid.Id, new ColumnSet(Incident.Fields.ipg_FacilityId)).ToEntity<Incident>() : null;

                if (task.ipg_caseid == null)
                {
                    task.ipg_caseid = incident.ToEntityReference();
                }

                task.Description = task.Description?
                    .Replace("<#, Name>", $"{product.ipg_manufacturerpartnumber}, {product.Name}")
                    .Replace("<Manufacturer Name>", product.ipg_manufacturerid?.Name).Replace("<Facility Name>", incident.ipg_FacilityId?.Name);
            }
        }
        public void CloseTask(string closureNote, Task_StatusCode closeReason, bool produceTaskNote = false, EntityReference taskReason = null)
        {
            if(_taskRef == null)
            {
                throw new ArgumentNullException(nameof(_taskRef));
            }

            CloseTask(_taskRef, closureNote, closeReason, produceTaskNote, taskReason);
        }

        public void CloseTask(EntityReference taskRef, string closureNote, Task_StatusCode closeReason, bool produceTaskNote = false, EntityReference taskReason = null)
        {
            var updTask = new Task();
            updTask.Id = taskRef.Id;
            if (_initiatingUserId != Guid.Empty)
            {
                updTask.ipg_closedbyid = new EntityReference(SystemUser.EntityLogicalName, _initiatingUserId);
            }

            if (taskReason != null)
            {
                updTask.ipg_taskreason = taskReason;
            }
            if (!string.IsNullOrEmpty(closureNote))
            {
                updTask.ipg_closurenote = closureNote;
            }

            if (closeReason == Task_StatusCode.Cancelled)
            {
                updTask.StateCode = TaskState.Completed;
                updTask.StatusCodeEnum = Task_StatusCode.Cancelled;
            }
            else if (closeReason == Task_StatusCode.Resolved)
            {
                updTask.StateCode = TaskState.Completed;
                updTask.StatusCodeEnum = Task_StatusCode.Resolved;
            }
            else if (closeReason == Task_StatusCode.ExceptionApproved)
            {
                updTask.StateCode = TaskState.Completed;
                updTask.StatusCodeEnum = Task_StatusCode.ExceptionApproved;
            }
            else
            {
                throw new InvalidPluginExecutionException("No relevant Task closeReason provided");
            }

            _crmService.Update(updTask);

            if (produceTaskNote)
            {
                CreateCloseNote(taskRef, closureNote, closeReason);
            }

            if (taskReason != null)
            {
                CreateNextCollectionTask(taskRef, taskReason);
            }
        }

        private void CreateNextCollectionTask(EntityReference taskRef, EntityReference taskReason)
        {
            var taskreasondetails = _crmService.RetrieveMultiple(new QueryExpression(ipg_taskreasondetails.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_taskreasondetails.Fields.ipg_tasktypeid
                , ipg_taskreasondetails.Fields.ipg_patientbalanceconditioncode
                , ipg_taskreasondetails.Fields.ipg_patientbalance
                , ipg_taskreasondetails.Fields.ipg_carrierbalance
                , ipg_taskreasondetails.Fields.ipg_carrierbalanceconditioncode
                , ipg_taskreasondetails.Fields.ipg_nopsgeneratedcode
                , ipg_taskreasondetails.Fields.ipg_taskstartdate
                , ipg_taskreasondetails.Fields.ipg_taskduedate
                ),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_taskreasondetails.Fields.ipg_tasktypeid, ConditionOperator.NotNull),
                        new ConditionExpression(ipg_taskreasondetails.Fields.ipg_onstatementeventid, ConditionOperator.Null),
                        new ConditionExpression(ipg_taskreasondetails.Fields.StateCode, ConditionOperator.Equal, (int)ipg_taskreasondetailsState.Active)
                    }
                },
                LinkEntities =
                {

                    new LinkEntity(ipg_taskreasondetails.EntityLogicalName
                    , ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName
                    , ipg_taskreasondetails.Fields.ipg_taskreasondetailsId
                    , ipg_ipg_taskreasondetails_ipg_statementgene.Fields.ipg_taskreasondetailsid, JoinOperator.LeftOuter)
                    {
                        EntityAlias = ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName,
                        Columns = new ColumnSet(ipg_ipg_taskreasondetails_ipg_statementgene.Fields.ipg_statementgenerationeventconfigurationid)
                    },
                    new LinkEntity(ipg_taskreasondetails.EntityLogicalName
                    , ipg_ipg_taskreasondetails_ipg_taskreason.EntityLogicalName
                    , ipg_taskreasondetails.Fields.ipg_taskreasondetailsId
                    , ipg_ipg_taskreasondetails_ipg_taskreason.Fields.ipg_taskreasondetailsid, JoinOperator.Inner)
                    {
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_ipg_taskreasondetails_ipg_taskreason.Fields.ipg_taskreasonid, ConditionOperator.Equal, taskReason.Id)
                            }
                        }
                    }
                }
            }).Entities.Select(x => x.ToEntity<ipg_taskreasondetails>());

            var oldTask = _crmService.Retrieve(taskRef.LogicalName, taskRef.Id, new ColumnSet(Task.Fields.RegardingObjectId, Task.Fields.ipg_level, Task.Fields.ipg_closurenote, Task.Fields.ipg_taskcategoryid, Task.Fields.ipg_caseid)).ToEntity<Task>();
           
            var incident = oldTask.ipg_caseid != null ? _crmService.Retrieve(oldTask.ipg_caseid.LogicalName, oldTask.ipg_caseid.Id
                , new ColumnSet(Incident.Fields.Id, Incident.Fields.ipg_RemainingPatientBalance
                , Incident.Fields.ipg_RemainingCarrierBalance, Incident.Fields.ipg_RemainingSecondaryCarrierBalance, Incident.Fields.ipg_LastStatementType)).ToEntity<Incident>() : null;
            
            var statementGeneratedEvents = _crmService.RetrieveMultiple(new QueryExpression(ipg_statementgenerationeventconfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_statementgenerationeventconfiguration.PrimaryNameAttribute, ConditionOperator.In
                        , new string[]{PSEvents.A2Generated, PSEvents.A3Generated, PSEvents.A5Generated, PSEvents.S1Generated, PSEvents.S3Generated })
                    }
                }
            }).Entities.Select(e => e.ToEntityReference()).ToList();

            decimal patientBalance = incident?.ipg_RemainingPatientBalance?.Value ?? 0;
            decimal carrierBalance = incident?.ipg_RemainingCarrierBalance?.Value ?? 0 + incident?.ipg_RemainingSecondaryCarrierBalance?.Value ?? 0;
            EntityReference lastStatementEvent = incident?.ipg_LastStatementType;
            var eventconfigattrname = $"{ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName}{ipg_ipg_taskreasondetails_ipg_statementgene.Fields.ipg_statementgenerationeventconfigurationid}";

            var taskReasonDetail = taskreasondetails.Where(trd => CheckTaskReasonDetail(trd, incident, statementGeneratedEvents)).FirstOrDefault();


            if (taskReasonDetail != null)
            {   
                if(string.IsNullOrEmpty(taskReason.Name))
                {
                    taskReason.Name = _crmService.Retrieve(taskReason.LogicalName, taskReason.Id, new ColumnSet(ipg_taskreason.PrimaryNameAttribute)).ToEntity<ipg_taskreason>().ipg_name;
                }
                var statementInfo = "";
                var subCategory = "";
                if (oldTask.ipg_taskcategoryid?.Name == TaskCategoryNames.PatientOutreach)
                {
                   subCategory = _docRepository.GetLastActivePSDoc(incident.ToEntityReference(), new ColumnSet(ipg_document.Fields.ipg_DocumentTypeId)).ipg_DocumentTypeId?.Name.Replace(DocumentCategioryNames.PatientStatement, "").Trim();
                   statementInfo = $"{Environment.NewLine}Current Patient Statement: " + subCategory;
                }

                var newdescription = $"Previous Action: {taskReason.Name} {statementInfo}";

                if (!string.IsNullOrEmpty(oldTask.ipg_closurenote))
                {
                    newdescription = newdescription + $"{Environment.NewLine}Closure Note: {oldTask.ipg_closurenote}";
                }

                var lastLevel = _crmService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(Task.Fields.ipg_level),
                    Orders = { new OrderExpression(Task.Fields.CreatedOn, OrderType.Descending) },
                    Criteria = new FilterExpression()
                    {
                        Conditions = {
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, oldTask.RegardingObjectId.Id)
                            , new ConditionExpression(Task.Fields.ipg_tasktypeid, ConditionOperator.Equal,  taskReasonDetail.ipg_tasktypeid.Id)
                        }
                    }

                }).Entities.FirstOrDefault()?.ToEntity<Task>().ipg_level ?? 0;

                var task = new Task()
                {
                    Description = newdescription,
                    Subcategory = subCategory,
                    RegardingObjectId = oldTask.RegardingObjectId,
                    ipg_level = lastLevel + 1,
                };

                task = GetTaskTemplateFromTaskReasonDetail(taskReasonDetail, task);

                if (incident != null)
                {
                    task.ipg_caseid = incident.ToEntityReference();
                }

                _crmService.Create(task);
            }
        }

        public ipg_tasktype GetTaskTypeByName(string name)
        {
            return _crmService.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_tasktype.Fields.ipg_taskcategoryid, ipg_tasktype.Fields.ipg_description),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_tasktype.Fields.ipg_name, ConditionOperator.Equal, name)
                    }
                }

            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();
        }

        public ipg_tasktype GetTaskTypeByTaskTypeId(TaskTypeIds tasktypeid, ColumnSet columns = null)
        {
            return GetTaskTypeByTaskTypeId(tasktypeid, _crmService, columns);
        }


        public static ipg_tasktype GetTaskTypeByTaskTypeId(TaskTypeIds tasktypeid, IOrganizationService service, ColumnSet columns = null)
        {
            return service.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = columns ?? new ColumnSet(true),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.Equal, (int)tasktypeid)
                    }
                }

            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();
        }

        public Guid CreateTask(EntityReference regardingRef, string subject, string description, ipg_TaskType1 taskType, EntityReference taskTypeId = null)
        {
            var task = new Task()
            {
                RegardingObjectId = regardingRef,
                Subject = subject,
                Description = description,
                ipg_tasktypecodeEnum = taskType,
                ipg_tasktypeid = taskTypeId
            };
            return _crmService.Create(task);
        }

        public Guid CreateTask(EntityReference regardingRef, string taskTypeName, Task taskTemplate = null)
        {
            var tasktypeRef = GetTaskTypeByName(taskTypeName)?.ToEntityReference();

            if (tasktypeRef == null)
            {
                throw new InvalidPluginExecutionException($"Unable to find a task type with name {taskTypeName}");
            }
            else
            {
                return CreateTask(regardingRef, tasktypeRef, taskTemplate);
            }
        }

        public Guid CreateTask(EntityReference regardingRef, EntityReference tasktypeRef, Task taskTemplate = null)
        {
            var task = new Task()
            {
                RegardingObjectId = regardingRef,
                ipg_tasktypeid = tasktypeRef
            };

            if (taskTemplate != null)
            {
                foreach (var attribute in taskTemplate.Attributes.Where(a => a.Key != Task.Fields.ActivityId))
                {
                    task[attribute.Key] = attribute.Value;
                }
            }

            var openTask = GetOpenTask(regardingRef, tasktypeRef);

            if (openTask != null)
            {
                _tracingService.Trace($"There is already open task for ${regardingRef?.LogicalName} ({regardingRef?.Id}) with type {tasktypeRef.Id}");
                return openTask.Id;
            }
            else
            {
                return _crmService.Create(task);
            }
        }

        public EntityReference CreateTaskByTaskTypeID(EntityReference regardingRef, string taskTypeId)
        {
            var tasktype = GetTaskTypeById(taskTypeId, _crmService, _tracingService);

            if (tasktype != null)
            {
                var id = CreateTask(regardingRef, tasktype.ToEntityReference());
                return new EntityReference(Task.EntityLogicalName, id);
            }
            else
            {
                return null;
            }
        }

        public Guid CreateTask(EntityReference regardingRef, Task taskTemplate)
        {
            taskTemplate.RegardingObjectId = regardingRef;

            if (taskTemplate.ipg_tasktypeid == null)
            {
                throw new ArgumentNullException(nameof(taskTemplate.ipg_tasktypeid));
            }

            var openTask = GetOpenTask(regardingRef, taskTemplate.ipg_tasktypeid);

            if (openTask != null)
            {
                _tracingService.Trace($"There is already open task for ${regardingRef?.LogicalName} ({regardingRef?.Id}) with type {taskTemplate.ipg_tasktypeid.Id}");
                return openTask.Id;
            }
            else
            {
                return _crmService.Create(taskTemplate);
            }
        }

        public Guid CreateTask(EntityReference regardingRef, TaskTypeIds tasktypeid, Task taskTemplate = null)
        {
            var tasktypeRef = GetTaskTypeById(tasktypeid, _crmService, _tracingService).ToEntityReference();

            if (tasktypeRef == null)
            {
                throw new InvalidPluginExecutionException($"Unable to find a task type with id {tasktypeid}");
            }
            else
            {
                var task = new Task()
                {
                    RegardingObjectId = regardingRef,
                    ipg_tasktypeid = tasktypeRef
                };

                if (taskTemplate != null)
                {
                    foreach (var attribute in taskTemplate.Attributes.Where(a => a.Key != Task.Fields.ActivityId))
                    {
                        task[attribute.Key] = attribute.Value;
                    }
                }

                var openTask = GetOpenTask(regardingRef, tasktypeRef);

                if (openTask != null)
                {
                    _tracingService.Trace($"There is already open task for ${regardingRef?.LogicalName} ({regardingRef?.Id}) with type {tasktypeRef.Id}");
                    return openTask.Id;
                }
                else
                {
                    _tracingService.Trace($"Task have been created for ${regardingRef?.LogicalName} ({regardingRef?.Id}) with type id {tasktypeid}");
                    return _crmService.Create(task);
                }
            }
        }

        public Guid CreateIfNotExistWithTaskCategoryTask(EntityReference regardingRef, string taskTypeName, Task taskTemplate = null)
        {
            var tasktypeRef = GetTaskTypeByName(taskTypeName);

            if (tasktypeRef != null && tasktypeRef.ipg_taskcategoryid != null)
            {
                var existedTask = _crmService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, regardingRef.Id),
                            new ConditionExpression(Task.Fields.ipg_taskcategoryid, ConditionOperator.Equal,tasktypeRef.ipg_taskcategoryid.Id)
                        }
                    }
                }).Entities.FirstOrDefault();

                if (existedTask == null)
                {
                    var task = new Task()
                    {
                        RegardingObjectId = regardingRef,
                        ipg_tasktypeid = tasktypeRef.ToEntityReference()
                    };

                    if (taskTemplate != null)
                    {
                        foreach (var attribute in taskTemplate.Attributes.Where(a => a.Key != Task.Fields.ActivityId))
                        {
                            task[attribute.Key] = attribute.Value;
                        }
                    }

                    return _crmService.Create(task);
                }
                else
                {
                    return existedTask.Id;
                }
            }

            return Guid.Empty;
        }

        internal IEnumerable<Task> GetTasks(EntityReference regardingObjectRef)
        {
            if (regardingObjectRef == null)
            {
                return null;
            }
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='task'>
                            <all-attributes/>
                            <order attribute='subject' descending='false' />
                            <filter type='and'>
                              <condition attribute='regardingobjectid' operator='eq' value='{regardingObjectRef.Id}' />
                            </filter>
                          </entity>
                        </fetch>";
            var result = _crmService.RetrieveMultiple(new FetchExpression(fetch))
                .Entities
                .Select(p => p.ToEntity<Task>());
            return result;
        }

        public void RescheduleTask(DateTime startDate, string rescheduleNote, bool produceTaskNote, string additionalTasks)
        {
            RescheduleTask(_taskRef.Id, startDate, rescheduleNote, produceTaskNote);
            if (!string.IsNullOrEmpty(additionalTasks))
            {
                var taskIds = additionalTasks.Split(',').Select(x => Guid.Parse(x));
                foreach (var taskId in taskIds)
                {
                    RescheduleTask(taskId, startDate, rescheduleNote, produceTaskNote);
                }
            }
        }

        private void RescheduleTask(Guid taskId, DateTime startDate, string rescheduleNote, bool produceTaskNote)
        {
            var taskEntity = _crmService.Retrieve(Insight.Intake.Task.EntityLogicalName, taskId,
                new ColumnSet(nameof(Task.Description).ToLower(), nameof(Task.Subject).ToLower(), nameof(Task.RegardingObjectId).ToLower())).ToEntity<Task>();
            var userName = _crmService.Retrieve(SystemUser.EntityLogicalName, _initiatingUserId, new ColumnSet(nameof(SystemUser.FullName).ToLower())).ToEntity<SystemUser>().FullName;
            var startDatestring = startDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
            var updTask = new Task();
            updTask.Id = taskId;
            updTask.Description = $"{taskEntity.Description} \r\n Claim Hold released by {userName} on {startDatestring}";
            updTask.ScheduledStart = startDate;
            if (!string.IsNullOrEmpty(rescheduleNote))
            {
                updTask.Description = $"{updTask.Description} due to: \r\n {rescheduleNote}";
            }
            _crmService.Update(updTask);
            if (produceTaskNote)
            {
                CreateRescheduleNote(updTask.Description, startDate, taskEntity.Subject, taskEntity.RegardingObjectId);
            }
        }

        internal void CreateRescheduleNote(string description, DateTime startDate, string subject, EntityReference regardingobjectid)
        {
            var note = new Annotation()
            {
                ObjectId = regardingobjectid,
                Subject = $"Task '{subject}' rescheduled",
                NoteText = description
            };
            _crmService.Create(note);
        }

        internal void CreateCloseNote(EntityReference tasktype, string closureNote, Task_StatusCode closeReason)
        {
            var taskEntity = _crmService.Retrieve(tasktype.LogicalName, tasktype.Id, new ColumnSet(Task.Fields.Subject, Task.Fields.RegardingObjectId)).ToEntity<Task>();
            if (taskEntity != null)
            {
                var reasonstring = closeReason.ToString();
                var caseNote = new Annotation()
                {
                    ObjectId = taskEntity.RegardingObjectId,
                    Subject = $"Task {reasonstring}: {taskEntity.Subject}",
                    NoteText = closureNote
                };
                var taskNote = new Annotation()
                {
                    ObjectId = taskEntity.ToEntityReference(),
                    Subject = $"Task {reasonstring}: {taskEntity.Subject}",
                    NoteText = closureNote
                };

                _crmService.Create(caseNote);
                _crmService.Create(taskNote);
            }
        }

        internal DateTime GetDueDateForSla(DateTime startDate, int slaTaskType, int? referralType = null)
        {
            DateTime result;
            var settings = _settingsHelper.GetSlaTaskSettings();

            int hours = 24;

            switch (slaTaskType)
            {
                case (int)ipg_SLATaskType.CreateReferral:
                    hours = settings.CreateRefferalDueDateOffset;
                    break;
                case (int)ipg_SLATaskType.GeneratePO:
                    hours = settings.GeneratePoDueDateOffset;
                    break;
                case (int)ipg_SLATaskType.PayProvider:
                    hours = settings.PayProviderDueDateOffset;
                    break;
                case (int)ipg_SLATaskType.DecisionCase:
                    if (referralType != null && referralType.HasValue)
                    {
                        switch (referralType.Value)
                        {
                            case (int)ipg_ReferralType.Stat:
                                hours = settings.DecisionStatCaseDueDateOffset;
                                break;
                            case (int)ipg_ReferralType.Urgent:
                                hours = settings.DecisionUrgentCaseDueDateOffset;
                                break;
                            case (int)ipg_ReferralType.Standard:
                                hours = settings.DecisionStandardCaseDueDateOffset;
                                break;
                            case (int)ipg_ReferralType.Retro:
                                hours = settings.DecisionRetroCaseDueDateOffset;
                                break;
                        }
                    }
                    break;
                default:
                    throw new InvalidPluginExecutionException("No due date settings for current SLA and Referral types");
            }

            if (slaTaskType == (int)ipg_SLATaskType.PayProvider)
            {
                result = startDate.AddHours(hours);
            }
            else
            {
                Entity calendar = _calendarRepository.GetBusinessClosureCalendar();

                result = BusinessDayHelper.AddBusinessDays(startDate, hours / 24, calendar);
                if (hours % 24 > 0)
                {
                    result.AddHours(hours % 24);
                }
            }

            return result;
        }

        internal IEnumerable<string> GetCaseFieldsBySoonestTasksMapping(Task task)
        {
            var overplappedMappings = CaseSoonestTasksHelper.FieldMappings
                .Where(mapping =>
                    task.Attributes.Any(attr =>
                        attr.Key == mapping.CheckField && mapping.IsContainsValue(attr.Value)))
                .ToArray();

            return overplappedMappings.Select(x => x.FillInField);
        }

        internal EntityReference GetSoonestDueTasksByCaseField(string caseField, Guid caseId)
        {
            var mapping = CaseSoonestTasksHelper.FieldMappings
                .FirstOrDefault(m => m.FillInField == caseField);

            if (mapping != null)
            {
                QueryExpression query = new QueryExpression(Task.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    TopCount = 1,
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(Task.Fields.ipg_caseid, ConditionOperator.Equal, caseId),
                            new ConditionExpression(mapping.CheckField, ConditionOperator.In, mapping.CheckFieldValues),
                            new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                        }
                    },
                    Orders =
                    {
                        new OrderExpression(Task.Fields.ScheduledEnd, OrderType.Ascending)
                    }
                };

                var task = _crmService.RetrieveMultiple(query).Entities.FirstOrDefault();
                if (task != null)
                    return task.ToEntityReference();
            }

            return null;
        }

        /// <summary>
        /// Generates and creates a Generate debit against a future CPA for a facility task
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Entity object with all fields and ID</returns>
        internal Task CreateGenerateDebitAgainstFutureCpaFacilityTask(Guid caseId)
        {
            Entity taskCategory = _taskCategoryRepository.GetByName(Constants.TaskCategoryNames.CaseProcessing, new ColumnSet(false));

            Task newTask = new Task()
            {
                Subject = TaskHelper.GenerateDebitAgainstCpaSubject,
                Description = TaskHelper.GenerateDebitAgainstCpaDescription,
                ipg_taskcategorycode = ipg_Taskcategory1.System.ToOptionSetValue(),
                ipg_taskcategoryid = taskCategory != null ? taskCategory.ToEntityReference() : null,
                RegardingObjectId = new EntityReference(Incident.EntityLogicalName, caseId),
                ipg_caseid = new EntityReference(Incident.EntityLogicalName, caseId)
            };

            newTask.Id = _crmService.Create(newTask);

            return newTask;
        }

        /// <summary>
        /// Checks for any open missing tissue request form task. If none exists, creates a new one.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Entity object with all fields and ID</returns>
        internal Task UpsertMissingTissueRequestFormTask(Guid caseId)
        {
            var tasks = _taskRepository.GetOpenMissingTissueRequestForm(caseId, new ColumnSet(false), 1);

            if (tasks.Any())
                return tasks.FirstOrDefault().ToEntity<Task>();

            return CreateMissingTissueRequestFormTask(caseId);
        }

        /// <summary>
        /// Cancels any open missing tissue request form tasks for a case.
        /// </summary>
        /// <param name="caseId"></param>
        internal void CancelOpenMissingTissueRequestFormTasks(Guid caseId)
        {
            var tasks = _taskRepository.GetOpenMissingTissueRequestForm(caseId, new ColumnSet(false));

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    _crmService.Execute(new SetStateRequest()
                    {
                        EntityMoniker = task.ToEntityReference(),
                        State = TaskState.Completed.ToOptionSetValue(),
                        Status = Task_StatusCode.Cancelled.ToOptionSetValue()
                    });
                }
            }
        }

        /// <summary>
        /// Generates a creates a new missing tissue request form task for a case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Entity object with all fields and ID</returns>
        internal Task CreateMissingTissueRequestFormTask(Guid caseId)
        {
            string subject = "Missing Information: Tissue Request Form";

            Entity taskCategory = _taskCategoryRepository.GetByName(Constants.TaskCategoryNames.MissingInformation, new ColumnSet(false));
            Entity taskType = _taskTypeRepository.GetByName("Missing Tissue Request Form", new ColumnSet(false)).FirstOrDefault();

            Task newTask = new Task()
            {
                Subject = subject,

                ipg_taskcategorycode = ipg_Taskcategory1.User.ToOptionSetValue(),
                ipg_systemtasktypecode = ipg_SystemTaskType.WorkflowTask_InfoorWarning.ToOptionSetValue(),
                ipg_tasktypecode = ipg_TaskType1.MissinginformationTissueRequestForm.ToOptionSetValue(),

                ipg_taskcategoryid = taskCategory != null ? taskCategory.ToEntityReference() : null,
                ipg_tasktypeid = taskType != null ? taskType.ToEntityReference() : null,

                RegardingObjectId = new EntityReference(Incident.EntityLogicalName, caseId),
                ipg_caseid = new EntityReference(Incident.EntityLogicalName, caseId),

                ipg_isvisibleonportal = true,
                ScheduledEnd = null
            };

            newTask.Id = _crmService.Create(newTask);

            return newTask;
        }

        /// <summary>
        /// Generates and creates a Generate & Submit Claim task
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="ScheduledStartDate"></param>
        /// <param name="owner"></param>
        /// <returns>Entity object with all fields and ID</returns>
        internal Task CreateGenerateSubmitClaimTask(Guid caseId, DateTime ScheduledStartDate, EntityReference owner, bool isSecondaryCarrier, string taskReasonName = null)
        {
            var taskType = GetTaskTypeById(TaskTypeIds.GENERATE_SUBMIT_CLAIM);

            Task newTask = new Task()
            {
                RegardingObjectId = new EntityReference(Incident.EntityLogicalName, caseId),
                ipg_taskcategorycode = ipg_Taskcategory1.System.ToOptionSetValue(),
                ipg_systemtasktypecode = ipg_SystemTaskType.WorkflowTask_InfoorWarning.ToOptionSetValue(),
                ScheduledStart = ScheduledStartDate,
                ipg_tasktypeid = taskType.ToEntityReference(),
                Description = string.Format(taskType.ipg_description, isSecondaryCarrier ? "Secondary" : "Primary")
            };

            if (owner != null)
            {
                newTask.OwnerId = owner;
            }
            if (!string.IsNullOrEmpty(taskReasonName))
            {
                var taskReason = GetTaskReasonByName(_crmService, taskReasonName);
                
                if(taskReason != null)
                {
                    newTask.ipg_taskreason = taskReason.ToEntityReference();
                }
            }

            newTask.Id = _crmService.Create(newTask);

            return newTask;
        }

        public bool CheckTaskByTaskType(Task task, string taskTypeName)
        {
            if (task.ipg_tasktypeid != null && !string.IsNullOrWhiteSpace(taskTypeName))
            {
                var taskType = _crmService.Retrieve(task.ipg_tasktypeid.LogicalName, task.ipg_tasktypeid.Id, new ColumnSet(ipg_tasktype.Fields.ipg_name)).ToEntity<ipg_tasktype>();
                return taskType.ipg_name?.ToLower().Trim() == taskTypeName.ToLower().Trim();
            }

            return false;
        }

        public void UpdateCaseHoldStatusByTaskk(EntityReference incident, Task task)
        {
            if (task.StateCode == TaskState.Open)
            {
                _crmService.Execute(new ipg_IPGCaseActionsHoldCaseRequest()
                {
                    Target = incident,
                    HoldNote = "Case put on Hold by task.",
                    IsOnHold = true,
                    HoldReason = new OptionSetValue((int)ipg_Caseholdreason.FacilityRecoveryResearchPending)
                });
            }
            else
            {
                _crmService.Execute(new ipg_IPGCaseActionsHoldCaseRequest()
                {
                    IsOnHold = false,
                    Target = incident,
                    HoldNote = "Case UnHold by task.",
                    HoldReason = new OptionSetValue((int)ipg_Caseholdreason.FacilityRecoveryResearchPending)
                });
            }
        }

        public ipg_tasktype RetrieveTaskTypeByName(string taskTypeName)
        {
            return _crmService.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(new string[] { ipg_tasktype.Fields.ipg_taskcategoryid }),
                Criteria = new FilterExpression()
                {
                    Conditions =
                        {
                            new ConditionExpression(ipg_tasktype.Fields.ipg_name, ConditionOperator.Equal, taskTypeName)
                        }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();
        }

        public enum TaskTypeIds
        {
            UNKNOWN_FACILITY_CARRIER_COMBINATION_FROM_EHR = 1011,
            MISSING_REQUIRED_FACILLITY_ACCOUNT_NUMBER = 1012,

            AUTORIZATION_REQUIRED = 1123,
            AUTORIZATION_REQUIRED_FOR_HPCS = 1124,

            SELECT_THE_CLAIMS_MAILING_ADDRESS = 1141,

            REQUEST_FOR_MANAGER_REVIEW = 1823,

            EBV_FAILED_TASK_TYPE_ID = 1202,
            MANUAL_BENEFIT_VERIFICATION_REQUIRED = 1216,
            INACTIVE_BENEFITS_TASK_TYPE_ID = 1218,

            NEW_PHYSICIAN_REQUEST = 1006,

            //TODO Deprecated needs to be refactored
            USE_DME_BENEFITS_IF_JQUPRFIX = 1213,

            MISSING_FACILITY_MFG = 2401,
            UNIT_COST_OVERRIDE_TRESHHOLD_EXCEEDED = 1103,

            //SLA Task Types
            SLA_Create_Referral = 2101,
            SLA_Decision_Stat_Case = 2104,
            SLA_Decision_Standard_Case = 2103,
            SLA_Decision_Urgent_Case = 2105,
            SLA_Decision_Retro_Case = 2102,

            SLA_Generate_PO = 2106,
            SLA_Pay_Provider = 2107,

            //Claim Management
            Secondary_Claim_Request = 1831,
            GENERATE_SUBMIT_CLAIM = 1706,
            Claim_Generation_Errors = 1704,

            //Duplicate Patient
            DUPLICATE_PATIENT = 1318,

            //Administrative
            NEW_PART_REQUEST = 1007,

            //Carrier Outreach
            CLAIM_RESPONSE_FULL_DENIAL = 2709,
            CLAIM_RESPONSE_PARTIAL_DENIAL = 2710,
            CHECK_CARRIER_BALANCE = 2711,

            //Carrier Service
            MISSING_ICN_VALUE = 1832,
            BILL_TO_PATIENT_NO = 1813,
            REVIEW_PROCESSED_AUTO_CLAIM = 1818,
            SECONDARY_CLAIM_REQUEST = 1831,

            //Patient Outreach
            CHECK_PATIENT_BALANCE = 2606,

            //Procedure Validation
            MISSING_OR_INVALID_PATIENT_ADDRESS = 1319,

            //Aministrative
            REQUEST_TO_UNLOCK_CASE = 1017,

            //Missing Information
            REQUEST_PATIENT_INFORMATION = 2334

        }
        public static class TaskReasonsNames
        {
            public const string ClaimHoldFacilityRequests = "Claim Hold - Facility Request";
            public const string ClaimHoldPatientDeductible = "Claim Hold - Patient Deductible";
            public const string NoClaimsHoldReason = "No Claim Holds";
            public const string ContactPatientIsSetToNo = "Contact Patient is Set to No";
            public const string NotNeeded = "Not Needed";
        }
        public static ipg_tasktype GetTaskTypeById(TaskTypeIds id, IOrganizationService crmService, ITracingService tracingService)
        {
            tracingService.Trace($"Looking for task type by string id {id}");

            var taskType = crmService.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions = { { new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.Equal, (int)id) } }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();

            if (taskType == null)
            {
                throw new Exception($"Task Type with {(int)id} does not exist, please contact System Administrator!");
            }

            tracingService.Trace($"task type with string id {(int)id} found crm id {taskType.Id}");

            return taskType;
        }

        public static ipg_tasktype GetTaskTypeById(string tasktypeid, IOrganizationService crmService, ITracingService tracingService)
        {
            int id = -1;
            int.TryParse(tasktypeid, out id);
            tracingService.Trace($"Looking for task type by string id {tasktypeid}");

            var taskType = crmService.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions = { { new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.Equal, id) } }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();

            if (taskType == null)
            {
                tracingService.Trace($"task type with string id {tasktypeid} not found crm id {taskType.Id}");
                return null;
            }

            tracingService.Trace($"task type with string id {tasktypeid} found crm id {taskType.Id}");

            return taskType;
        }

        public ipg_tasktype GetTaskTypeById(TaskTypeIds id)
        {
            _tracingService.Trace($"Looking for task type by string id {id}");
            var taskType = _crmService.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_tasktype.Fields.ipg_name, ipg_tasktype.Fields.ipg_tasktitle, ipg_tasktype.Fields.ipg_typeid,
                                          ipg_tasktype.Fields.ipg_taskcategoryid, ipg_tasktype.Fields.ipg_subcategory, ipg_tasktype.Fields.ipg_startdate,
                                          ipg_tasktype.Fields.ipg_duedate, ipg_tasktype.Fields.ipg_assigntoteam, ipg_tasktype.Fields.ipg_assigntouserid,
                                          ipg_tasktype.Fields.ipg_isportal, ipg_tasktype.Fields.ipg_priority, ipg_tasktype.Fields.ipg_isactive,
                                          ipg_tasktype.Fields.ipg_casestatecodes, ipg_tasktype.Fields.ipg_description),
                Criteria = new FilterExpression()
                {
                    Conditions = { { new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.Equal, (int)id) } }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();
            if (taskType == null)
            {
                throw new Exception($"Task Type with {(int)id} does not exist, please contact System Administrator!");
            }
            _tracingService.Trace($"task type with string id {(int)id} found crm id {taskType.Id}");
            return taskType;
        }

        public static IReadOnlyList<Task> GetOpenDecisionSLATasks(EntityReference related, IOrganizationService crmService, ITracingService tracingService)
        {
            if(related == null)
            {
                throw new ArgumentNullException($"{nameof(related)} is null in { nameof(GetOpenDecisionSLATasks)}");
            }

            tracingService.Trace($"{nameof(GetOpenDecisionSLATasks)} for {related.LogicalName} r{related.Id}");

            return crmService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Task.Fields.ScheduledEnd, Task.Fields.StateCode),
                Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal,related.Id),
                            new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                        }
                    },
                LinkEntities =
                    {
                        new LinkEntity(Task.EntityLogicalName, ipg_tasktype.EntityLogicalName, Task.Fields.ipg_tasktypeid, ipg_tasktype.PrimaryIdAttribute, JoinOperator.Inner)
                        {
                            LinkCriteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.In, new int[]
                                    {
                                        (int)TaskTypeIds.SLA_Decision_Retro_Case, 
                                        (int)TaskTypeIds.SLA_Decision_Standard_Case,
                                        (int)TaskTypeIds.SLA_Decision_Stat_Case,
                                        (int)TaskTypeIds.SLA_Decision_Urgent_Case
                                    })
                                }
                            }
                        }
                    }

            }).Entities.Select(t => t.ToEntity<Task>()).ToList();
        }

        public EntityReference GetSlaTaskTypeRefByReferralType(ipg_ReferralType? referralType)
        {
            return GetSlaTaskTypeRefByReferralType(referralType, _crmService, _tracingService);
        }

        public static  EntityReference GetSlaTaskTypeRefByReferralType(ipg_ReferralType? referralType, IOrganizationService crmService, ITracingService tracingService)
        {
            if(referralType == null)
            {
                throw new ArgumentNullException($"{nameof(referralType)} in {nameof(GetSlaTaskTypeRefByReferralType)}");
            }

            tracingService.Trace($"executes {nameof(GetSlaTaskTypeRefByReferralType)}");

            switch (referralType)
            {
                case ipg_ReferralType.Stat:
                    return GetTaskTypeByTaskTypeId(TaskTypeIds.SLA_Decision_Stat_Case, crmService ,  new ColumnSet(false))?.ToEntityReference();
                case ipg_ReferralType.Urgent:
                    return GetTaskTypeByTaskTypeId(TaskTypeIds.SLA_Decision_Urgent_Case, crmService,  new ColumnSet(false))?.ToEntityReference();
                case ipg_ReferralType.Standard:
                    return GetTaskTypeByTaskTypeId(TaskTypeIds.SLA_Decision_Standard_Case, crmService, new ColumnSet(false))?.ToEntityReference();
                case ipg_ReferralType.Retro:
                    return GetTaskTypeByTaskTypeId(TaskTypeIds.SLA_Decision_Retro_Case, crmService,  new ColumnSet(false))?.ToEntityReference();
                default:
                    throw new ArgumentException($"Cannot find referral type with OptionSet value: {referralType.ToString()}.");
            }
        }

        public static ipg_taskreason GetTaskReasonByName(IOrganizationService service, string glsName, ColumnSet columnset = null)
        {
            return service.RetrieveMultiple(new QueryExpression(ipg_taskreason.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = columnset ?? new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                        {
                            new ConditionExpression(ipg_taskreason.Fields.ipg_name, ConditionOperator.Equal, glsName)
                        }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_taskreason>();
        }

        public ipg_taskreasondetails GetTaskReasonDetailOnStatementEventRef(EntityReference statementEventRef)
        {
            _tracingService.Trace($"Looking for task reason details with on statement event ${statementEventRef.Id}");

            return _crmService.RetrieveMultiple(new QueryExpression(ipg_taskreasondetails.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_taskreasondetails.Fields.ipg_tasktypeid, ipg_taskreasondetails.Fields.ipg_taskstartdate, ipg_taskreasondetails.Fields.ipg_taskduedate),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_taskreasondetails.Fields.StateCode, ConditionOperator.Equal, (int)ipg_taskreasondetailsState.Active),
                        new ConditionExpression(ipg_taskreasondetails.Fields.ipg_onstatementeventid, ConditionOperator.Equal, statementEventRef.Id),
                        new ConditionExpression(ipg_taskreasondetails.Fields.ipg_tasktypeid, ConditionOperator.NotNull)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_taskreasondetails>();
        }

        public ipg_taskreasondetails GetTaskReasonDetailOnStatementEventName(string statementEventName)
        {
            _tracingService.Trace($"Looking for task reason details with on statement event {statementEventName}");

            return _crmService.RetrieveMultiple(new QueryExpression(ipg_taskreasondetails.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_taskreasondetails.Fields.ipg_tasktypeid, ipg_taskreasondetails.Fields.ipg_taskstartdate, ipg_taskreasondetails.Fields.ipg_taskduedate),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_taskreasondetails.Fields.StateCode, ConditionOperator.Equal, (int)ipg_taskreasondetailsState.Active),
                        new ConditionExpression(ipg_taskreasondetails.Fields.ipg_tasktypeid, ConditionOperator.NotNull)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_taskreasondetails.EntityLogicalName, ipg_statementgenerationeventconfiguration.EntityLogicalName, ipg_taskreasondetails.Fields.ipg_onstatementeventid, ipg_statementgenerationeventconfiguration.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_statementgenerationeventconfiguration.PrimaryNameAttribute, ConditionOperator.Equal, statementEventName)
                            }
                        }
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_taskreasondetails>();
        }

        public List<Task> GetRelatedTasksByCategory(EntityReference entityReference, string taskCategoryName, ColumnSet columnSet = null)
        {
            var regardingCheck = entityReference != null ? new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, entityReference.Id)
                : new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Null);

            return _crmService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = columnSet ?? new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        regardingCheck,
  
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                    }
                },
                LinkEntities =
                    {
                        new LinkEntity(Task.EntityLogicalName, ipg_taskcategory.EntityLogicalName, Task.Fields.ipg_taskcategoryid, ipg_taskcategory.PrimaryIdAttribute, JoinOperator.Inner)
                        {
                            LinkCriteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression(ipg_taskcategory.Fields.ipg_name, ConditionOperator.Equal, taskCategoryName)
                                }
                            }
                        }
                    }
            }).Entities.Select(t => t.ToEntity<Task>()).ToList();
        }

        public Task GetTaskTemplateFromTaskReasonDetail(ipg_taskreasondetails taskReasonDetail, Task task = null)
        {
            task = task ?? new Task() {ipg_level = 1 };

            task.ipg_tasktypeid = taskReasonDetail.ipg_tasktypeid;

            if (taskReasonDetail.ipg_taskduedate.HasValue || taskReasonDetail.ipg_taskstartdate.HasValue)
            {
                Entity calendar = _calendarRepository.GetBusinessClosureCalendar();

                task.ScheduledStart = (taskReasonDetail.ipg_taskstartdate.HasValue ? BusinessDayHelper.AddBusinessDays(DateTime.Now, taskReasonDetail.ipg_taskstartdate.Value, calendar) : DateTime.Now);
                task.ScheduledEnd = (taskReasonDetail.ipg_taskduedate.HasValue ? BusinessDayHelper.AddBusinessDays(task.ScheduledStart.Value, taskReasonDetail.ipg_taskduedate.Value, calendar) : task.ScheduledStart);
            }

            return task;
        }
        public bool CheckTaskReasonDetail(ipg_taskreasondetails trd, Incident incident, List<EntityReference> statementGeneratedEvents = null)
        {
            statementGeneratedEvents = statementGeneratedEvents ?? _crmService.RetrieveMultiple(new QueryExpression(ipg_statementgenerationeventconfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_statementgenerationeventconfiguration.PrimaryNameAttribute, ConditionOperator.In
                        , new string[]{PSEvents.A2Generated, PSEvents.A3Generated, PSEvents.A5Generated, PSEvents.S1Generated, PSEvents.S3Generated })
                    }
                }
            }).Entities.Select(e => e.ToEntityReference()).ToList();

            decimal patientBalance = incident?.ipg_RemainingPatientBalance?.Value ?? 0;
            decimal carrierBalance = incident?.ipg_RemainingCarrierBalance?.Value ?? 0 + incident?.ipg_RemainingSecondaryCarrierBalance?.Value ?? 0;
            EntityReference lastStatementEvent = incident?.ipg_LastStatementType;
            var eventconfigattrname = $"{ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName}{ipg_ipg_taskreasondetails_ipg_statementgene.Fields.ipg_statementgenerationeventconfigurationid}";


            return trd.ipg_carrierbalanceconditioncodeEnum.TestCondition(carrierBalance, trd.ipg_carrierbalance)
                            && trd.ipg_patientbalanceconditioncodeEnum.TestCondition(patientBalance, trd.ipg_patientbalance)
                            && ((trd.ipg_nopsgeneratedcodeEnum == ipg_TwoOptions.Yes && (lastStatementEvent == null || !statementGeneratedEvents.Contains(lastStatementEvent)))
                                || trd.ipg_nopsgeneratedcodeEnum != ipg_TwoOptions.Yes && ((trd.GetAttributeValue<AliasedValue>(eventconfigattrname)?.Value as Guid?) ?? lastStatementEvent?.Id) == lastStatementEvent?.Id);
        }

        public void CloseCategoryTasks(Guid incidentId, string category)
        {
            CloseTasks(new EntityReference(Incident.EntityLogicalName, incidentId), Task_StatusCode.Cancelled, null, null, new string[]{ category });
        }

        public void CancelCategoryTasks(EntityReference target, string closeReason, params string[] categories)
        {
            CloseTasks(target, Task_StatusCode.Cancelled, closeReason, null, categories);
        }

        public void CompleteTasks(EntityReference target, params TaskTypeIds[] taskTypeIds)
        {
            CloseTasks(target, Task_StatusCode.Resolved, null, taskTypeIds, null);
        }

        public void CancelTasks(EntityReference target, string closeReason, params TaskTypeIds[] taskTypeIds)
        {
            CloseTasks(target, Task_StatusCode.Cancelled, closeReason, taskTypeIds, null);
        }

        private void CloseTasks(EntityReference target, Task_StatusCode status, string closeReason, TaskTypeIds[] taskTypeIds, string[] categories)
        {
            var tasksQuery = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                        {
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, target.Id),
                            new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                        }
                }
            };

            if(taskTypeIds?.Any() == true)
            {
                var taskTypeLink = tasksQuery.AddLink(ipg_tasktype.EntityLogicalName, Task.Fields.ipg_tasktypeid, ipg_tasktype.PrimaryIdAttribute, JoinOperator.Inner);
                taskTypeLink.LinkCriteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.In, taskTypeIds.Select(v=>(int)v).ToArray())
                    }
                };
            }

            if (categories?.Any() == true)
            {
                var categoryLink = tasksQuery.AddLink(ipg_taskcategory.EntityLogicalName, Task.Fields.ipg_taskcategoryid, ipg_taskcategory.PrimaryIdAttribute, JoinOperator.Inner);
                categoryLink.LinkCriteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_taskcategory.Fields.ipg_name, ConditionOperator.In, categories)
                    }
                };
            }

            var tasks = _crmService.RetrieveMultiple(tasksQuery);

            var taskReason = string.IsNullOrEmpty(closeReason) ? null : GetTaskReasonByName(_crmService, closeReason)?.ToEntityReference();

            foreach (var task in tasks.Entities)
            {
                _crmService.Update(new Task()
                {
                    Id = task.Id,
                    ipg_taskreason = taskReason,
                    StateCode = TaskState.Completed,
                    StatusCodeEnum = status
                });
            }
        }

        public void FillTaskByTaskType(Task task, ipg_tasktype taskType, string[] descriptionParams = null)
        {
            task.Subject = taskType?.ipg_tasktitle;
            task.ipg_taskcategoryid = taskType?.ipg_taskcategoryid;
            task.ipg_assignedtoteamid = taskType?.ipg_assigntoteam;
            task.ipg_priority = taskType?.ipg_priority;
            task.ipg_generatedbycode = taskType?.ipg_generatedbycode;
            task.ipg_tasktypeid = taskType?.ToEntityReference();
            task.Description = descriptionParams == null
                               ? taskType?.ipg_description
                               : SetDescriptionParameters(taskType?.ipg_description, descriptionParams);
            if (taskType.ipg_startdate != null && taskType.ipg_duedate != null)
            {
                task.ScheduledStart = DateTime.Now.AddDays(taskType.ipg_startdate.Value);
                task.ScheduledEnd = DateTime.Now.AddDays(taskType.ipg_duedate.Value);
            }
        }
        private string SetDescriptionParameters(string description, string[] parameters)
        {
            if (!string.IsNullOrEmpty(description) && parameters != null)
            {
                Regex pattern = new Regex("<.*?>");
                foreach (var param in parameters)
                {
                    if (!string.IsNullOrEmpty(param))
                        description = pattern.Replace(description, param, 1);
                }
            }
            return description;
        }
    }
}