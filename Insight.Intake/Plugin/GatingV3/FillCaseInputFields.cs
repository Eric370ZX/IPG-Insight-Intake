using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Insight.Intake.Plugin.Gating;

namespace Insight.Intake.Plugin.GatingV3
{
    public class FillCaseInputFields : PluginBase
    {

        IOrganizationService service;
        OrganizationServiceContext crmContext;

        public FillCaseInputFields() : base(typeof(FillCaseInputFields))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                service = localPluginContext.OrganizationService;
                crmContext = new OrganizationServiceContext(service);
                var tracingService = localPluginContext.TracingService;
                var entity = (Entity)context.InputParameters["Target"];
                var gateManager = new GateManager(service, tracingService, entity.ToEntityReference());

                var caseNumber = gateManager.GetCaseNumber(entity, context.MessageName);
                if (string.IsNullOrEmpty(caseNumber))
                {
                    return;
                }

                var updatedCaseInputFields = new List<ipg_caseinputfield>();
                var fields = (from fieldSource in crmContext.CreateQuery<ipg_workflowtaskinputfieldsource>()
                                              where fieldSource.ipg_Entity == entity.LogicalName
                                              select new { fieldName = fieldSource.ipg_Field, fieldId = fieldSource.ipg_WorkflowTaskInputFieldId });
                var fieldsID = fields.Select(f => f.fieldId.Id);
                var caseInputFields = (from caseInputField in crmContext.CreateQuery<ipg_caseinputfield>()
                                       where caseInputField.ipg_CaseNumber == caseNumber
                                       && fieldsID.Contains(caseInputField.ipg_InputFieldId.Id) == true
                                       select caseInputField);
                foreach (var field in fields)
                {
                    if(entity.Contains(field.fieldName))
                    {
                        var record = new ipg_caseinputfield()
                        {
                            ipg_CaseInputFieldValue = gateManager.ObjectPresentation(entity[field.fieldName]),
                            ipg_CaseInputFieldValueId = gateManager.ObjectToString(entity[field.fieldName]),
                            ipg_CaseInputFieldChanged = true
                        };
                        FillCaseId(entity, record);
                        var caseInputField = caseInputFields.Where(f => f.ipg_InputFieldId.Id == field.fieldId.Id).FirstOrDefault();
                        if (caseInputField == null)
                        {
                            record.ipg_CaseNumber = caseNumber;
                            record.ipg_InputFieldId = field.fieldId;
                            service.Create(record);
                            updatedCaseInputFields.Add(record);
                        }
                        else
                        {
                            record.ipg_caseinputfieldId = caseInputField.ipg_caseinputfieldId;
                            service.Update(record);
                            updatedCaseInputFields.Add(caseInputField);
                        }
                    }
                    else if(context.MessageName == MessageNames.Create)
                    {
                        var record = new ipg_caseinputfield()
                        {
                            ipg_CaseNumber = caseNumber,
                            ipg_InputFieldId = field.fieldId,
                            ipg_CaseInputFieldChanged = true
                        };
                        FillCaseId(entity, record);
                        service.Create(record);
                        updatedCaseInputFields.Add(record);
                    }
                }

                ResetCaseWorkflowTasks(entity, context.MessageName, updatedCaseInputFields);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void FillCaseId(Entity entity, ipg_caseinputfield record)
        {
            if (string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName))
            {
                record.ipg_ReferralId = entity.ToEntityReference();
            }
            else if (string.Equals(entity.LogicalName, Incident.EntityLogicalName))
            {
                record.ipg_CaseId = entity.ToEntityReference();
            }
        }

        private void ResetCaseWorkflowTasks(Entity entity, string messageName, List<ipg_caseinputfield> updatedCaseInputFields)
        {
            if (string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName) && messageName == MessageNames.Create || updatedCaseInputFields.Count() == 0)
            {
                return;
            }

            var inputFields = updatedCaseInputFields.Select(e => e.ipg_InputFieldId.Id).Distinct();
            var workflowTasks = (from relationship in crmContext.CreateQuery<ipg_ipg_workflowtask_ipg_workflowtaskinput>()
                                 join workflowTask in crmContext.CreateQuery<ipg_workflowtask>() on relationship.ipg_workflowtaskid equals workflowTask.ipg_workflowtaskId
                                 join workflowTaskInputField in crmContext.CreateQuery<ipg_workflowtaskinputfield>() on relationship.ipg_workflowtaskinputfieldid equals workflowTaskInputField.ipg_workflowtaskinputfieldId
                                 where inputFields.Contains(workflowTaskInputField.Id) == true
                                 select workflowTask.Id);
            var caseWorkflowTasks = (from cwt in crmContext.CreateQuery<ipg_caseworkflowtask>()
                                     where workflowTasks.Contains(cwt.ipg_WorkflowTaskId.Id) == true
                                     && cwt.ipg_Passed == true
                                     select cwt);

            foreach (var caseWorkflowTask in caseWorkflowTasks)
            {
                var record = new ipg_caseworkflowtask()
                {
                    Id = caseWorkflowTask.Id,
                    ipg_Passed = false
                };
                service.Update(record);
            }
        }
    }
}