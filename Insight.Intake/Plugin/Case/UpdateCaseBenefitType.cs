using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.Plugin.Case
{
    public class UpdateCaseBenefitType : PluginBase
    {
        public static string DmeMemberIdPrefix = "JQU";
        

        public UpdateCaseBenefitType() : base(typeof(UpdateCaseBenefitType))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationCreateHandler);
        }


        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var organizationService = localPluginContext.OrganizationService;
            var traceService = localPluginContext.TracingService;
            
            Incident targetIncident = localPluginContext.Target<Incident>();
            if (targetIncident == null)
            {
                traceService.Trace("Target is empty. Exit");
                return;
            }

            if(targetIncident.ipg_CarrierId != null)
            {
                ipg_BenefitType? newBenefitType = null;

                traceService.Trace("Retrieving Carrier 1");
                var carrier1 = organizationService.Retrieve(Intake.Account.EntityLogicalName, targetIncident.ipg_CarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>();
                if (carrier1.ipg_CarrierType != null)
                {
                    switch (carrier1.ipg_CarrierTypeEnum.Value)
                    {
                        case ipg_CarrierType.Auto:
                            newBenefitType = ipg_BenefitType.Auto;
                            break;
                        case ipg_CarrierType.WorkersComp:
                            newBenefitType = ipg_BenefitType.WorkersComp;
                            break;
                    }

                    //Update default benefit plan types
                    switch (carrier1.ipg_CarrierTypeEnum)
                    {
                        case ipg_CarrierType.Government:
                            targetIncident.ipg_benefitplantypecode = new OptionSetValue((int)ipg_BenefitPlanType.Government);
                            break;
                        default:
                            targetIncident.ipg_benefitplantypecode = new OptionSetValue((int)ipg_BenefitPlanType.EPO);
                            break;
                    }
                }
                if(newBenefitType.HasValue == false && (targetIncident.ipg_MemberIdNumber ?? "").ToUpper().StartsWith(DmeMemberIdPrefix))
                {
                    newBenefitType = ipg_BenefitType.DurableMedicalEquipment_DME;
                }

                if(newBenefitType.HasValue)
                {
                    traceService.Trace("Setting Benefit Type for Carrier 1");
                    targetIncident.ipg_BenefitTypeCodeEnum = newBenefitType;
                }
            }

            if(targetIncident.ipg_SecondaryCarrierId != null)
            {
                if ((targetIncident.ipg_SecondaryMemberIdNumber ?? "").ToUpper().StartsWith(DmeMemberIdPrefix))
                {
                    traceService.Trace("Setting Benefit Type for Carrier 2");
                    targetIncident.ipg_Carrier2BenefitTypeCodeEnum = ipg_BenefitType.DurableMedicalEquipment_DME;
                }
            }

            traceService.Trace("Updating Target input parameter");
            localPluginContext.PluginExecutionContext.InputParameters["Target"] = targetIncident.ToEntity<Entity>();
        }

        void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            Incident target = localPluginContext.Target<Incident>();
            if (target == null)
            {
                localPluginContext.TracingService.Trace("Target is empty. Exit");
                return;
            }

            if ((target.ipg_MemberIdNumber ?? "").ToUpper().StartsWith(DmeMemberIdPrefix) == false)
            {
                localPluginContext.TracingService.Trace($"Not {DmeMemberIdPrefix} member id. Exit");
                return;
            }

            CreateDmeUserTaskIfDoesNotExist(service, localPluginContext.TracingService, target);
        }

        private void CreateDmeUserTaskIfDoesNotExist(IOrganizationService service, ITracingService tracingService, Incident target)
        {
            using (var context = new CrmServiceContext(service))
            {
                tracingService.Trace("Requesting an existing task");
                var existingTask = (from t in context.TaskSet
                                    join tt in context.ipg_tasktypeSet on t.ipg_tasktypeid.Id equals tt.Id
                                    where t.RegardingObjectId != null && t.RegardingObjectId.Id == target.Id
                                     && tt.ipg_typeid == (int)TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX
                                     && t.StateCode == TaskState.Open
                                    select t).FirstOrDefault();

                if (existingTask != null)
                {
                    tracingService.Trace("The task already exists");
                    return;
                }

                //todo: this may not be needed if ConfigureTask sets Description field
                var existingDescription = (from t in context.ipg_tasktypeSet
                                           where t.ipg_typeid == (int)TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX
                                           select t).FirstOrDefault();


                if (existingTask != null)
                {
                    tracingService.Trace("The task already exists");
                    return;
                }


                var taskType = context.ipg_tasktypeSet.FirstOrDefault(tt => tt.ipg_typeid == (int)TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX && tt.StateCode == ipg_tasktypeState.Active);
                if (taskType == null)
                {
                    throw new InvalidPluginExecutionException("Could not find the task type with id = " + (int)TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX);
                }

                tracingService.Trace("Creating a new task");

                var newTask = new Task
                {
                    RegardingObjectId = target.ToEntityReference(),
                    ipg_tasktypeid = taskType.ToEntityReference(),
                    Description = existingDescription.ipg_description
                };
                context.AddObject(newTask);
                context.SaveChanges();
            }
        }
    }
}
