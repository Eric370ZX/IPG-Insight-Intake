using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using System.Collections.Generic;
using IntakeNS = Insight.Intake;

namespace Insight.Intake.Plugin.Case
{
    /// <summary>
    /// Purpose of this plugin is to validate pre and post surgery required documents on a case. And create tasks for user to 
    /// upload those documents.
    /// </summary>
    public class ValidateRequiredDocuments : IPlugin
    {
        #region Readonly Members

        private readonly string[] ColumnSetIncident = {
                            "ipg_carrierid",
                            "ipg_facilityid",
                            "ipg_cptcodeid1",
                            "ipg_cptcodeid2",
                            "ipg_cptcodeid3",
                            "ipg_cptcodeid4",
                            "ipg_cptcodeid5",
                            "ipg_cptcodeid6"
        };


        #endregion

        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{this.GetType()} Execute method started");
            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                tracingService.Trace("Validating Target input parameter");
                if (context.InputParameters.Contains("Target") == false
                    || !(context.InputParameters["Target"] is EntityReference targetEntityReference)
                    || targetEntityReference.LogicalName != Incident.EntityLogicalName)
                {
                    throw new Exception("Target entity Case is required");
                }

                tracingService.Trace("Getting Organization service");
                var organizationService = serviceProvider.GetOrganizationService(context.UserId);
                var helper = new ValidateRequiredDocumentPluginHelpers(organizationService, tracingService, context);

                try
                {
                    //Retrieve case information. (Additional information like facility, carrier and cptcode).
                    var caseReference = organizationService.Retrieve(targetEntityReference.LogicalName,
                                                                     targetEntityReference.Id,
                                                                     new ColumnSet(ColumnSetIncident)).ToEntity<Incident>();

                    //Retrieve required document types by facility.
                    var requiredDocumentFacility = helper.GetRequiredDocumentsForAccount(caseReference.ipg_FacilityId);
                    //Required document types by carrier
                    var requiredDocumentCarrier = helper.GetRequiredDocumentsForAccount(caseReference.ipg_CarrierId);

                    //Get documents already attached on case.
                    var documents = helper.GetDocumentsOnCase(targetEntityReference);

                    //Remove existing required information for document type on case.
                    helper.DeleteAllRequiredInformationOnCase(targetEntityReference);
                    //Remove all existing tasks for required documents on case.
                    helper.DeleteAllRequiredInformationTaskOnCase(targetEntityReference);

                    //For now only fetch post surgery required documents.
                    foreach (var reqDoc in requiredDocumentFacility
                                           .Union(requiredDocumentCarrier))
                    {

                        //If the required document does not exists on case.
                        if (documents.Any(x => x.ipg_DocumentTypeId.Id == reqDoc.Id) == false)
                        {
                            //Create required document information on case.
                            helper.CreateRequiredInformationOnCase(targetEntityReference, reqDoc);

                            //Create task for user to resolve.
                            helper.CreateTask(targetEntityReference, reqDoc);
                        }
                    }

                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in the RetrieveMultiple function", ex);
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace($"{this.GetType().Name}: {exception}");
                throw;
            }
        }

        #region Helper Class

        class ValidateRequiredDocumentPluginHelpers
        {
            private IOrganizationService organizationService;
            private ITracingService tracingService;
            private IPluginExecutionContext context;

            //Ctor
            internal ValidateRequiredDocumentPluginHelpers(IOrganizationService organizationService, ITracingService tracingService, IPluginExecutionContext context)
            {
                this.organizationService = organizationService;
                this.tracingService = tracingService;
                this.context = context;
            }

            private QueryExpression GetQueryExpressionForAccount(Guid accountId)
            {
                QueryExpression qx = new QueryExpression("ipg_documenttype");
                qx.ColumnSet.AddColumn("ipg_documenttypeid");
                qx.ColumnSet.AddColumn("ipg_name");

                LinkEntity link1 = qx.AddLink("ipg_informationtyperequiredinformationrule", "ipg_documenttypeid", "ipg_documenttypeid", JoinOperator.Inner);
                link1.EntityAlias = "be";

                //Pull document type from account.
                link1.LinkCriteria.AddCondition("ipg_accountid", ConditionOperator.Equal, accountId);

                return qx;
            }

            internal IEnumerable<ipg_documenttype> GetRequiredDocumentsForAccount(EntityReference accountId)
            {
                //To avoid null reference exception on the caller.
                if (accountId == null) return new List<ipg_documenttype>();

                var queryExpressionWithoutTemplate = GetQueryExpressionForAccount(accountId.Id);

                var docTypeWithoutTemplate = organizationService
                                            .RetrieveMultiple(queryExpressionWithoutTemplate)
                                            .Entities.Select(x => x.ToEntity<ipg_documenttype>());
                return docTypeWithoutTemplate;
            }

            internal IEnumerable<ipg_document> GetDocumentsOnCase(EntityReference incident)
            {

                QueryExpression qx = new QueryExpression("ipg_document");
                qx.ColumnSet.AddColumn("ipg_documentid");
                qx.ColumnSet.AddColumn("ipg_name");
                qx.ColumnSet.AddColumn("ipg_documenttypeid");
                qx.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                return organizationService.RetrieveMultiple(qx).
                       Entities.Select(x => x.ToEntity<ipg_document>());
            }


            internal void DeleteAllRequiredInformationOnCase(EntityReference incident)
            {
                var requiredInformationQuery = new QueryExpression
                {
                    EntityName = ipg_requiredinformation.EntityLogicalName,
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression(LogicalOperator.And),
                    PageInfo = new PagingInfo
                    {
                        ReturnTotalRecordCount = true
                    },
                };

                requiredInformationQuery.Criteria.AddCondition("ipg_documenttypeid", ConditionOperator.NotNull);
                requiredInformationQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                var currentRequiredDocuments = organizationService.RetrieveMultiple(requiredInformationQuery);
                foreach (var entity in currentRequiredDocuments.Entities)
                {
                    organizationService.Delete(ipg_requiredinformation.EntityLogicalName, entity.Id);
                }
            }

            internal void DeleteAllRequiredInformationTaskOnCase(EntityReference incident)
            {
                var requiredInformationTaskQuery = new QueryExpression
                {
                    EntityName = Task.EntityLogicalName,
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression(LogicalOperator.And),
                    PageInfo = new PagingInfo
                    {
                        ReturnTotalRecordCount = true
                    },
                };

                requiredInformationTaskQuery.Criteria.AddCondition("category", ConditionOperator.Equal, context.PrimaryEntityName);
                requiredInformationTaskQuery.Criteria.AddCondition("subject", ConditionOperator.Like, "Upload required document%");
                requiredInformationTaskQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, incident.Id);

                var currentRequiredInfomationTask = organizationService.RetrieveMultiple(requiredInformationTaskQuery);
                foreach (var entity in currentRequiredInfomationTask.Entities)
                {
                    organizationService.Delete(Task.EntityLogicalName, entity.Id);
                }

            }

            internal void CreateRequiredInformationOnCase(EntityReference incident, ipg_documenttype reqDoc)
            {

                var requiredInformationQuery = new QueryExpression
                {
                    EntityName = ipg_requiredinformation.EntityLogicalName,
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression(LogicalOperator.And),
                    PageInfo = new PagingInfo
                    {
                        ReturnTotalRecordCount = true
                    },
                };

                requiredInformationQuery.Criteria.AddCondition("ipg_documenttypeid", ConditionOperator.Equal, reqDoc.Id);
                requiredInformationQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                if (organizationService.RetrieveMultiple(requiredInformationQuery).TotalRecordCount > 0) return;

                // Create a task activity to follow up with the account customer in 7 days. 
                ipg_requiredinformation requiredinformation = new ipg_requiredinformation();
                requiredinformation.ipg_name = reqDoc.ipg_name;
                requiredinformation.ipg_CaseId = new EntityReference(Incident.EntityLogicalName, incident.Id);
                requiredinformation.ipg_DocumentTypeId = new EntityReference(ipg_documenttype.EntityLogicalName, reqDoc.Id);

                // Create the task in Microsoft Dynamics CRM.
                tracingService.Trace(string.Format("FollowupPlugin: Creating Required Information {0} on Case {1}.", reqDoc.ipg_name, incident.Name));
                organizationService.Create(requiredinformation);
            }

            internal void CreateTask(EntityReference incident, ipg_documenttype reqDoc)
            {
                var requiredInformationTaskQuery = new QueryExpression
                {
                    EntityName = Task.EntityLogicalName,
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression(LogicalOperator.And),
                    PageInfo = new PagingInfo
                    {
                        ReturnTotalRecordCount = true
                    },
                };

                requiredInformationTaskQuery.Criteria.AddCondition("category", ConditionOperator.Equal, context.PrimaryEntityName);
                requiredInformationTaskQuery.Criteria.AddCondition("subject", ConditionOperator.Like, "%" + reqDoc.ipg_name + "%");
                requiredInformationTaskQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, incident.Id);

                if (organizationService.RetrieveMultiple(requiredInformationTaskQuery).TotalRecordCount > 0) return;

                // Create a task activity to 
                Task followup = new Task();

                followup.Subject = string.Format("Upload required document {0}", reqDoc.ipg_name);
                followup.Description = "Please upload the required document.";
                followup.Category = context.PrimaryEntityName;
                followup.ipg_DocumentType = reqDoc.ToEntityReference();
                followup.Subcategory = reqDoc.ipg_DocumentTypeAbbreviation;


                // Refer to the case in the task activity.
                followup.RegardingObjectId = new EntityReference(Incident.EntityLogicalName, incident.Id);

                // Create the task in Microsoft Dynamics CRM.
                tracingService.Trace("FollowupPlugin: Creating the task activity.");
                organizationService.Create(followup);
            }
        }

        #endregion
    }
}