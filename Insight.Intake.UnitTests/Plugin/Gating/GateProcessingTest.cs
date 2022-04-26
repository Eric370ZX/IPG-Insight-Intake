using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class GateProcessingTest : PluginTestsBase
    {
        [Fact]
        public void CheckGateProcessing_GateConfigurationIsNull()
        {
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };

            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open);
            referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { referral, lifecyclestepid };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);
            Assert.Equal("Gate Configuration field is empty. Please contact administrator to resolve this issue.", pluginContext.OutputParameters["Output"]);
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckGateProcessing_LifecycleStepIsNull()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referral = new ipg_referral().Fake();

            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { referral };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);
            Assert.Equal("Lifecycle step field is empty. Please contact administrator to resolve this issue.", pluginContext.OutputParameters["Output"]);
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckGateProcessing_GateConfigurationStateIsInactive()
        {
            var fakedContext = new XrmFakedContext();

            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(20)
                .WithStateCode(1);

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            var stateCode = ipg_CaseStateCodes.Authorization;
            ipg_gatedetermination gateDetermination = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(lifecyclestepid.ToEntityReference())
                .WithGateConfigurationReference(gateConfiguration.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);

            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open).WithStateCode(stateCode);
            referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();

            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { gateConfiguration, referral, lifecyclestepid, gateDetermination };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);
            Assert.Equal("Gate Configuration is Inactive. Please contact administrator to resolve this issue.", pluginContext.OutputParameters["Output"]);
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckGateProcessing_NoActiveGateConfigurationDetails()
        {
            var fakedContext = new XrmFakedContext();

            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(20);

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            var stateCode = ipg_CaseStateCodes.Authorization;
            ipg_gatedetermination gateDetermination = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(lifecyclestepid.ToEntityReference())
                .WithGateConfigurationReference(gateConfiguration.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);

            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open).WithStateCode(stateCode); ;
            referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();

            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .WithStateCode(ipg_gateconfigurationdetailState.Inactive);

            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { gateConfiguration, referral, gateConfigurationDetail, lifecyclestepid, gateDetermination };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);
            Assert.Equal("There are no Active Gate Configuration Detail records. Please contact administrator to resolve this issue.", pluginContext.OutputParameters["Output"]);
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckGateProcessing_TaskAlreadyCreatedForCase_NewTaskIsNotCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(20);

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open);
            referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();

            Incident caseEntity = new Incident().Fake();
            referral.ipg_AssociatedCaseId = caseEntity.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration);
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical);

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.ApproveCPTCode);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            Task task = new Task().Fake();
            task.RegardingObjectId = caseEntity.ToEntityReference();
            task.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.ApproveCPTCode);
            task.StateCode = TaskState.Open;

            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { gateConfiguration, referral, gateConfigurationDetail, taskConfig, task, lifecyclestepid };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.ipg_tasktypecode.Value == taskConfig.ipg_tasktypecode.Value
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();

            Assert.True(tasks.Count == 1);
        }

        [Fact]
        public void CheckGateProcessing_TaskAlreadyCreatedAndCompletedForCase_NewTaskIsNotCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(20);

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open);
            referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();

            Incident caseEntity = new Incident().Fake();
            referral.ipg_AssociatedCaseId = caseEntity.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration);
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical);

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.ApproveCPTCode);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            Task task = new Task().Fake();
            task.RegardingObjectId = caseEntity.ToEntityReference();
            task.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.ApproveCPTCode);
            task.StateCode = TaskState.Completed;

            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { gateConfiguration, referral, gateConfigurationDetail, taskConfig, task, lifecyclestepid };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.ipg_tasktypecode.Value == taskConfig.ipg_tasktypecode.Value
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();

            Assert.True(tasks.Count == 1);
        }

        [Fact]
        public void CheckGateProcessing_TaskIsNotCreatedForCase_NewTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);
            ipg_gatedetermination gDetermination = new ipg_gatedetermination().Fake()
                .RuleFor(p => p.ipg_CaseStateEnum, p => ipg_CaseStateCodes.Authorization)
                .RuleFor(p => p.ipg_LifecycleStepId, p => lifecyclestepid.ToEntityReference())
                .RuleFor(p => p.ipg_GateConfigurationId, p => gateConfiguration.ToEntityReference())
                .RuleFor(p => p.ipg_TriggeredBy, p => false);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();




            ipg_lifecyclestep nextLifecycleStep = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };

            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
                //ipg_nextlifecyclestepid = nextLifecycleStep.ToEntityReference()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            //ipg_referral referral = new ipg_referral().Fake();
            //referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();

            Incident caseEntity = new Incident().Fake().WithActualDos(DateTime.Now.AddDays(3))
                .RuleFor(p => p.ipg_StateCodeEnum, p => ipg_CaseStateCodes.Authorization);
            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            //referral.ipg_AssociatedCaseId = caseEntity.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());
            gateConfigurationDetail.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical);

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.ApproveCPTCode);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
                .Fake()
                .RuleFor(p => p.ipg_OutcomeType, p => false)
                .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());

            EntityReference target = caseEntity.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { caseEntity, gateConfiguration, gateConfigurationDetail, taskConfig, lifecyclestepid, gateProcessingRule, wfTask, outputConfig, gDetermination };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.ipg_tasktypecode.Value == taskConfig.ipg_tasktypecode.Value
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();
            Assert.True(tasks.Count != 0);
        }

        [Fact]
        public void CheckGateProcessing_GatingIsRunForReferralAndTaskCreatedForCase_NewTaskIsCreatedForCase()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_StateCode = new OptionSetValue((int)ipg_CaseStateCodes.Authorization);

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);
            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
            };

            ipg_gatedetermination gDetermination = new ipg_gatedetermination().Fake()
                .RuleFor(p => p.ipg_CaseStateEnum, p => ipg_CaseStateCodes.Authorization)
                .RuleFor(p => p.ipg_LifecycleStepId, p => lifecyclestepid.ToEntityReference())
                .RuleFor(p => p.ipg_GateConfigurationId, p => gateConfiguration.ToEntityReference())
                .RuleFor(p => p.ipg_TriggeredBy, p => false);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();

            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open);
            referral.ipg_statecode = new OptionSetValue((int)ipg_CaseStateCodes.Authorization);
            referral.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            referral.ipg_AssociatedCaseId = caseEntity.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration);
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical);
            gateConfigurationDetail.ipg_WorkflowTaskId = wfTask.ToEntityReference();

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.ApproveCPTCode);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);
            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
   .Fake()
   .RuleFor(p => p.ipg_OutcomeType, p => false)
   .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
   .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());

            Task task = new Task().Fake();
            task.RegardingObjectId = referral.ToEntityReference();
            //task.ipg_tasktypeid = taskType.ToEntityReference();
            task.StateCode = TaskState.Open;

            EntityReference target = referral.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { gateConfiguration, referral, gateConfigurationDetail, taskConfig, task, lifecyclestepid, outputConfig, gDetermination,wfTask, gateProcessingRule };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser }, { "IsManual", false } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.ipg_tasktypecode.Value == taskConfig.ipg_tasktypecode.Value
                         && taskRecord.RegardingObjectId.Id == referral.ipg_AssociatedCaseId.Id
                         select taskRecord).ToList();
            Assert.True(tasks.Count != 0);
        }

        [Fact]
        public void CheckGateProcessing_GatingIsRunForCase_NewPortalCommentIsCreatedForCase()
        {
            var fakedContext = new XrmFakedContext();

            var facility = new Intake.Account() { Id = Guid.NewGuid() };
            var contactAccount = new ipg_contactsaccounts()
            {
                Id = Guid.NewGuid(),
                ipg_accountid = facility.ToEntityReference(),
                ipg_contactid = new EntityReference(Contact.EntityLogicalName, Guid.NewGuid())
            };

            var stateCode = ipg_CaseStateCodes.Authorization;
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(20);
            Insight.Intake.Contact contact = new Contact().Fake();
            Incident caseEntity = new Incident().Fake().WithActualDos(DateTime.Now.AddDays(3))
                .WithState((int)stateCode);
            caseEntity.ipg_FacilityId = facility.ToEntityReference();
            /*caseEntity.ipg_origin = new OptionSetValue((int)Incident_CaseOriginCode.Portal);
            caseEntity.CustomerId = contact.ToEntityReference();*/
            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            ipg_gatedetermination gateDetermination = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(lifecyclestepid.ToEntityReference())
                .WithGateConfigurationReference(gateConfiguration.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);

            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();

            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
                //ipg_nextlifecyclestepid = nextLifecycleStep.ToEntityReference()
            };

            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration);
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Critical);

            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { gateConfiguration, caseEntity, gateConfigurationDetail, facility, contactAccount, lifecyclestepid, gateProcessingRule, gateDetermination };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var portalComments = (from portalComment in fakedCrmContext.CreateQuery<adx_portalcomment>()
                                  where portalComment.RegardingObjectId.Id == caseEntity.Id
                                  select portalComment).ToList();

            Assert.True(portalComments.Count == 1);
        }

        [Fact]
        public void CheckGateProcessing_TaskIsNotCreatedForCase_NewInactiveHCPCSTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);
            ipg_gatedetermination gDetermination = new ipg_gatedetermination().Fake()
                .RuleFor(p => p.ipg_CaseStateEnum, p => ipg_CaseStateCodes.Authorization)
                .RuleFor(p => p.ipg_LifecycleStepId, p => lifecyclestepid.ToEntityReference())
                .RuleFor(p => p.ipg_GateConfigurationId, p => gateConfiguration.ToEntityReference())
                .RuleFor(p => p.ipg_TriggeredBy, p => false);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();




            ipg_lifecyclestep nextLifecycleStep = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            Incident caseEntity = new Incident().Fake().WithActualDos(DateTime.Now.AddDays(3))
                .RuleFor(p => p.StateCode, p => IncidentState.Active);
            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            caseEntity.ipg_StateCode = new OptionSetValue((int)ipg_CaseStateCodes.Authorization);
            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory().Fake().WithName("Case Processing");

            ipg_tasktype inactiveHcpcsType = new ipg_tasktype()
               .Fake()
               .WithName("Inactive HCPCS on Case")
               .WithCategory(caseProcessingCategory)
               .WithDescription("HCPCS {0} is inactive for Procedure Date {1} for this Case. Please review HCPCS table and associated Parts to resolve this issue.");

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            //taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypeid = inactiveHcpcsType.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.HCPCSisexpiredforDOSontheCase);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
                .Fake()
                .RuleFor(p => p.ipg_OutcomeType, p => false)
                .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());


            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());
            gateConfigurationDetail.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error);


            ipg_masterhcpcs inactiveHCPCS = new ipg_masterhcpcs()
                .Fake("Inactive Test", DateTime.Now.AddDays(4), DateTime.Now.AddDays(2));
            ipg_casepartdetail actualpart = new ipg_casepartdetail()
                .Fake()
                .WithCaseReference(caseEntity)
                .WithHCPCS(inactiveHCPCS);



            EntityReference target = caseEntity.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { caseEntity, gateConfiguration, gateConfigurationDetail,
                taskConfig, lifecyclestepid, gateProcessingRule, caseProcessingCategory, inactiveHcpcsType,
                inactiveHCPCS, actualpart , wfTask,outputConfig, gDetermination};

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.Description.Contains(string.Format("HCPCS {0} is inactive for Procedure Date {1} for this Case.", inactiveHCPCS.ipg_name, caseEntity.ipg_ActualDOS))
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();

            //Assert.True(tasks.Count == 0);
            Assert.True(tasks.Count != 0);
        }

        [Fact]
        public void CheckGateProcessing_TaskIsNotCreatedForCase_NewClaimGenerationErrorsTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);
            ipg_gatedetermination gDetermination = new ipg_gatedetermination().Fake()
                .RuleFor(p => p.ipg_CaseStateEnum, p => ipg_CaseStateCodes.Authorization)
                .RuleFor(p => p.ipg_LifecycleStepId, p => lifecyclestepid.ToEntityReference())
                .RuleFor(p => p.ipg_GateConfigurationId, p => gateConfiguration.ToEntityReference())
                .RuleFor(p => p.ipg_TriggeredBy, p => false);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();

            ipg_lifecyclestep nextLifecycleStep = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            Intake.Account carrier = new Intake.Account().Fake().WithName("Test");
            Incident caseEntity = new Incident().Fake().WithActualDos(DateTime.Now.AddDays(3)).WithCarrierReference(carrier)
                .RuleFor(p => p.ipg_StateCodeEnum, p => ipg_CaseStateCodes.Authorization);
            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());
            gateConfigurationDetail.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error);

            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory().Fake().WithName("Case Processing");
            ipg_tasktype inactiveHcpcsType = new ipg_tasktype()
                .Fake()
                .WithName("Claim Generation Errors")
                .WithCategory(caseProcessingCategory)
                .WithDescription("Claim for Carrier {0} could not be generated. Please correct the underlying cause and resubmit Case for processing.");

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypeid = inactiveHcpcsType.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.HCPCSisexpiredforDOSontheCase);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
                .Fake()
                .RuleFor(p => p.ipg_OutcomeType, p => false)
                .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());


            EntityReference target = caseEntity.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { caseEntity, gateConfiguration, gateConfigurationDetail,
                taskConfig, lifecyclestepid, gateProcessingRule, caseProcessingCategory, inactiveHcpcsType,
                carrier, wfTask,outputConfig, gDetermination };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.Description.Contains(string.Format("Claim for Carrier {0} could not be generated. Please correct the underlying cause and resubmit Case for processing.", carrier.Name))
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();
            Assert.True(tasks.Count != 0);
        }

        [Fact]
        public void CheckGateProcessing_TaskIsNotCreatedForCase_NewPhysicianRequestTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();

            ipg_lifecyclestep nextLifecycleStep = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            var stateCode = ipg_CaseStateCodes.Authorization;
            ipg_gatedetermination gateDetermination = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(lifecyclestepid.ToEntityReference())
                .WithGateConfigurationReference(gateConfiguration.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);

            Intake.Account facility = new Intake.Account().Fake().WithName("Test");
            Contact physician = new Contact().Fake();
            physician.FirstName = "Test";
            physician.LastName = "Test";
            Incident caseEntity = new Incident().Fake().WithActualDos(DateTime.Now.AddDays(3)).WithFacilityReference(facility).WithState((int)stateCode);
            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());
            gateConfigurationDetail.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error);

            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory().Fake().WithName("Case Processing");
            ipg_tasktype inactiveHcpcsType = new ipg_tasktype()
                .Fake()
                .WithName("New Physician Request")
                .WithCategory(caseProcessingCategory)
                .WithDescription("Review New Physician Request for {0} for Facility {1}.");

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypeid = inactiveHcpcsType.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.HCPCSisexpiredforDOSontheCase);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
    .Fake()
    .RuleFor(p => p.ipg_OutcomeType, p => false)
    .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
    .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());

            EntityReference target = caseEntity.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { caseEntity, gateConfiguration, gateConfigurationDetail,
                taskConfig, lifecyclestepid, gateProcessingRule, caseProcessingCategory, inactiveHcpcsType,
                facility, physician, outputConfig, gateDetermination, wfTask };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.Description.Contains(string.Format("Review New Physician Request for {0} for Facility {1}.", physician.FullName, facility.Name))
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();
            Assert.True(tasks.Count != 0);
        }

        [Fact]
        public void CheckGateProcessing_TaskIsNotCreatedForCase_NewFacilityBSANotonFileandFacilityNotExemptTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);
            ipg_gatedetermination gDetermination = new ipg_gatedetermination().Fake()
                .RuleFor(p => p.ipg_CaseStateEnum, p => ipg_CaseStateCodes.Authorization)
                .RuleFor(p => p.ipg_LifecycleStepId, p => lifecyclestepid.ToEntityReference())
                .RuleFor(p => p.ipg_GateConfigurationId, p => gateConfiguration.ToEntityReference())
                .RuleFor(p => p.ipg_TriggeredBy, p => false);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();



            ipg_lifecyclestep nextLifecycleStep = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            Intake.Account facility = new Intake.Account().Fake().WithName("Test");
            Contact physician = new Contact().Fake();
            physician.FirstName = "Test";
            physician.LastName = "Test";
            Incident caseEntity = new Incident().Fake().WithActualDos(DateTime.Now.AddDays(3)).WithFacilityReference(facility)
                .RuleFor(p => p.ipg_StateCodeEnum, p => ipg_CaseStateCodes.Authorization);

            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());
            gateConfigurationDetail.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error);

            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory().Fake().WithName("Case Processing");
            ipg_tasktype taskType = new ipg_tasktype()
                .Fake()
                .WithName("Facility BSA Not on File and Facility Not Exempt")
                .WithCategory(caseProcessingCategory)
                .WithDescription("BSA for {0} is not on file and Facility is not exempt. Update Facility BSA requirement or obtain BSA document and attach to Facility record.");

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypeid = taskType.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.HCPCSisexpiredforDOSontheCase);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
    .Fake()
    .RuleFor(p => p.ipg_OutcomeType, p => false)
    .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
    .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());

            EntityReference target = caseEntity.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { caseEntity, gateConfiguration, gateConfigurationDetail,
                taskConfig, lifecyclestepid, gateProcessingRule, caseProcessingCategory, taskType,
                facility, physician, outputConfig, wfTask,gDetermination};

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.Description.Contains(string.Format("BSA for {0} is not on file and Facility is not exempt. Update Facility BSA requirement or obtain BSA document and attach to Facility record.", facility.Name))
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();
            Assert.True(tasks.Count != 0);
        }

        [Fact]
        public void CheckGateProcessing_TaskIsNotCreatedForCase_NewMissingFacilityCarrierContractTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();
            
            ipg_lifecyclestep lifecyclestepid = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake(60);
            ipg_gatedetermination gDetermination = new ipg_gatedetermination().Fake()
                .RuleFor(p => p.ipg_CaseStateEnum, p => ipg_CaseStateCodes.Authorization)
                .RuleFor(p => p.ipg_LifecycleStepId, p => lifecyclestepid.ToEntityReference())
                .RuleFor(p => p.ipg_GateConfigurationId, p => gateConfiguration.ToEntityReference())
                .RuleFor(p => p.ipg_TriggeredBy, p => false);

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake();

            ipg_lifecyclestep nextLifecycleStep = new ipg_lifecyclestep()
            {
                Id = Guid.NewGuid()
            };
            ipg_gateprocessingrule gateProcessingRule = new ipg_gateprocessingrule()
            {
                Id = Guid.NewGuid(),
                ipg_lifecyclestepid = lifecyclestepid.ToEntityReference(),
                ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error),
                ipg_gateconfigurationid = gateConfiguration.ToEntityReference()
            };
            lifecyclestepid.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();

            Intake.Account facility = new Intake.Account().Fake().WithName("Test Facility");
            Intake.Account carrier = new Intake.Account().Fake().WithName("Test Carrier");
            var dos = DateTime.Now.AddDays(3);
            Incident caseEntity = new Incident().Fake()
                .WithActualDos(dos)
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .RuleFor(p => p.ipg_StateCodeEnum, p => ipg_CaseStateCodes.Authorization);

            caseEntity.ipg_lifecyclestepid = lifecyclestepid.ToEntityReference();
            ipg_gateconfigurationdetail gateConfigurationDetail = new ipg_gateconfigurationdetail().Fake(gateConfiguration)
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());
            gateConfigurationDetail.ipg_gateconfigurationid = gateConfiguration.ToEntityReference();
            gateConfigurationDetail.ipg_severitylevel = new OptionSetValue((int)ipg_SeverityLevel.Error);

            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory().Fake().WithName("Case Processing");
            ipg_tasktype taskType = new ipg_tasktype()
                .Fake()
                .WithName("Missing Facility Carrier Contract")
                .WithCategory(caseProcessingCategory)
                .WithDescription("Contract for {0} and {1} does not exist for Procedure Date {2}.");

            ipg_taskconfiguration taskConfig = new ipg_taskconfiguration().Fake();
            taskConfig.ipg_gateconfigurationdetailid = gateConfigurationDetail.ToEntityReference();
            taskConfig.ipg_tasktypeid = taskType.ToEntityReference();
            taskConfig.ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.HCPCSisexpiredforDOSontheCase);
            taskConfig.ipg_createif = new OptionSetValue((int)ipg_CreateIf.Failed);

            ipg_workflowtaskoutputconfig outputConfig = new ipg_workflowtaskoutputconfig()
                .Fake()
                .RuleFor(p => p.ipg_OutcomeType, p => false)
                .RuleFor(p => p.ipg_TaskConfigurationId, p => taskConfig.ToEntityReference())
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference());

            EntityReference target = caseEntity.ToEntityReference();
            EntityReference initiatingUser = new EntityReference();

            var listForInit = new List<Entity> { caseEntity, gateConfiguration, gateConfigurationDetail,
                taskConfig, lifecyclestepid, gateProcessingRule, caseProcessingCategory, taskType,
                facility, carrier, wfTask,outputConfig, gDetermination };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", target }, { "InitiatingUser", initiatingUser } };
            var otputParameters = new ParameterCollection { { "Output", "" }, { "Succeeded", false } };
            ;
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = otputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var fakedCrmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in fakedCrmContext.CreateQuery<Task>()
                         where taskRecord.StateCode == TaskState.Open
                         && taskRecord.Description.Contains(string.Format("Contract for {0} and {1} does not exist for Procedure Date {2}.", facility.Name, carrier.Name, dos))
                         && taskRecord.RegardingObjectId.Id == caseEntity.Id
                         select taskRecord).ToList();
            Assert.True(tasks.Count != 0);
        }
    }
}