using Insight.Intake.UnitTests.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Insight.Intake.Plugin.Gating;
using FakeXrmEasy;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using FakeXrmEasy.FakeMessageExecutors;
using Insight.Intake.Plugin.Gating.DetermineFacilityCarrierStatus;
using Microsoft.Xrm.Sdk.Metadata;
using FakeXrmEasy.Extensions;
using static FakeXrmEasy.XrmFakedContext;
using Microsoft.Xrm.Sdk.Messages;
using Insight.Intake.Plugin.Gating.PostProcess;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class GateConfig
    {
        public GateConfig() { }
        public GateConfig(Type wtPlugin, string requestName, ipg_SeverityLevel severityLevel, string passMessage, string failMessage, ipg_gateconfiguration gate)
        {
            WTPlugin = wtPlugin;
            SevirityLevel = severityLevel;
            PassMessage = passMessage;
            FailMessage = failMessage;
            Gate = gate;
            RequestName = requestName;
        }

        public string RequestName { get; set; }
        public Type WTPlugin { get; set; }
        public ipg_SeverityLevel SevirityLevel { get; set; }
        public string PassMessage { get; set; }
        public string FailMessage { get; set; }
        public ipg_gateconfiguration Gate {get; set;}
    }
    public class ReferralOn2GateTests
    {
        private static bool acceptReferralExecuted = false;
        private static bool bvfRunned = false;

        [Fact]
        public void ReferralPassGate2AndCaseCreated()
        {
            var fakedContext = new XrmFakedContext();
            var fakedData = new List<Entity>();
           
            var referralFieldsInMetadata = new List<string>
            { ipg_referral.Fields.ipg_PatientLastName,
            ipg_referral.Fields.ipg_PatientFirstName,
            ipg_referral.Fields.ipg_PatientDateofBirth,
            ipg_referral.Fields.ipg_PatientCity,
            ipg_referral.Fields.ipg_PatientAddress,
            ipg_referral.Fields.ipg_PatientState,
            ipg_referral.Fields.ipg_PatientZipCodeId,
            ipg_referral.Fields.ipg_cellphone,
            ipg_referral.Fields.ipg_memberidnumber,
            ipg_referral.Fields.ipg_CarrierId,
            ipg_referral.Fields.ipg_autoclaimnumber,
            ipg_referral.Fields.ipg_autoadjustername,
            ipg_referral.Fields.ipg_autodateofincident};

            SystemUser user = new SystemUser().Fake();
            ipg_gateconfiguration gate1config = new ipg_gateconfiguration().Fake("Gate 1");
            ipg_gateconfiguration gate2config = new ipg_gateconfiguration().Fake("Gate 2");
            ipg_gateconfiguration gate3config = new ipg_gateconfiguration().Fake("Gate 3");

            ipg_lifecyclestep intake1 = new ipg_lifecyclestep().Fake(gate1config);
            ipg_lifecyclestep intake2 = new ipg_lifecyclestep().Fake(gate2config);
            ipg_lifecyclestep intake3 = new ipg_lifecyclestep().Fake(gate3config);

            ipg_gateprocessingrule gateprocessingrule = new ipg_gateprocessingrule().Fake(intake2, intake3, "", ipg_SeverityLevel.Warning);

            var stateCode = ipg_CaseStateCodes.Authorization;
            ipg_gatedetermination gateDetermination1 = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(intake1.ToEntityReference())
                .WithGateConfigurationReference(gate1config.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);
            ipg_gatedetermination gateDetermination2 = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(intake2.ToEntityReference())
                .WithGateConfigurationReference(gate2config.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);
            ipg_gatedetermination gateDetermination3 = new ipg_gatedetermination().Fake()
                .WithLifecycleStepReference(intake3.ToEntityReference())
                .WithGateConfigurationReference(gate3config.ToEntityReference())
                .WithCaseState(stateCode)
                .WithTriggeredBy(false);

            ipg_procedurename procedureName = new ipg_procedurename().Fake().WithActiveValue(true);
            EntityReference initiatingUser = user.ToEntityReference();

            Intake.Account carrier = new Intake.Account().Fake().WithCarrierAccepted();

            Intake.Account facility = new Intake.Account().Fake();

            var dos = DateTime.Now.AddDays(30);
            ipg_cptcode cpt1 = new ipg_cptcode().Fake()
             .WithEffectiveDate(dos.AddDays(-30))
            .WithExpirationDate(dos.AddDays(60));
            ipg_cptcode cpt2 = new ipg_cptcode().Fake();

            Entitlement entitlement = new Entitlement().Fake()
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(dos,dos.AddDays(5));

            ipg_facilitycarriercptrulecontract facilitycarriercptrulecontract =
                           new ipg_facilitycarriercptrulecontract().Fake()
                           .WithCPTInclusionType(ipg_cptinclusiontyperule.Included)
                           .FakeWithEntitlement(entitlement)
                           .WithEffectivesDates(dos, dos.AddDays(5));

            ipg_facilitycarriercptrule facilitycarriercptrule = new ipg_facilitycarriercptrule().Fake()
                .FakeWithCptCode(cpt1)
                .WithEffectivesDates(dos, dos.AddDays(5));


            ipg_referral referral = new ipg_referral().Fake()
                .WithLFstep(intake2)
                .WithOwner(user)
                .WithCarrierReference(carrier)
                .WithSurgeryDate(dos)
                .WithCptCodeReferences(new List<ipg_cptcode>() { cpt1, cpt2 })
                .WithProcedureName(procedureName)
                .WithFacilityReference(facility)
                .WithCaseStatus(ipg_CaseStatus.Open)
                .WithStateCode(stateCode);


            Intake.Workflow post2GateAction = new Intake.Workflow().Fake("IPGGatingPostProcessGate2");
            gate2config.ipg_successpostprocessid = post2GateAction.ToEntityReference();

            fakedData.AddRange(new List<Entity>() {
                post2GateAction,
                gate1config, 
                gate2config, 
                intake1, 
                intake2,
                intake3,
                referral, 
                user, 
                carrier, 
                entitlement,
                facilitycarriercptrulecontract,
                cpt1, 
                cpt2, 
                facilitycarriercptrule, 
                facility, 
                procedureName, 
                gate3config,
                gateprocessingrule,
                gateDetermination1,
                gateDetermination2,
                gateDetermination3});

            var Wts = new List<GateConfig>()
            {
                new GateConfig(typeof(CPTSupportedByIPG), "ipg_IPGGatingCPTSupportedByIPG", ipg_SeverityLevel.Info, "Passed", "Failed", gate2config),
                new GateConfig(typeof(ValidateDxCode), "ipg_IPGGatingValidateDxCode", ipg_SeverityLevel.Info, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CPTSupportedByFacility), "ipg_IPGGatingCPTSupportedByFacility", ipg_SeverityLevel.Critical, "Passed", "Failed", gate2config),
                new GateConfig(typeof(PatientDemographicsNotProvided), "ipg_IPGGatingPatientDemographicsNotProvided", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckRequiredAuthForHMO), "ipg_IPGGatingCheckRequiredAuthForHMO", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckHighDollarAuth), "ipg_IPGGatingCheckHighDollarAuth", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckPreAuthMemberId), "ipg_IPGGatingCheckPreAuthMemberId", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckDxGender), "ipg_IPGGatingCheckDxGender", ipg_SeverityLevel.Info, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckPhysicianValue), "ipg_IPGGatingCheckPhysicianValue", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckAutoCarrier), "ipg_IPGGatingCheckAutoCarrier", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(NoContract), "ipg_IPGGatingDetermineFacilityCarrierStatusNoContract", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(DOSIsOutOfContract), "ipg_IPGGatingDetermineFacilityCarrierStatusDOSIsOutOfContract", ipg_SeverityLevel.Warning, "Passed", "Failed", gate2config),
                new GateConfig(typeof(CheckCarrierIsAccepted), "ipg_IPGGatingCarrierIsAccepted", ipg_SeverityLevel.Error, "Passed", "Failed", gate2config),
            };

            for (var i = 0; i < Wts.Count; i++)
            {
                var wt = Wts[i];
                Intake.Workflow workflow = new Intake.Workflow().Fake(wt.RequestName.Replace("ipg_", ""));
                ipg_gateconfigurationdetail gateconfigdetails = new ipg_gateconfigurationdetail().Fake(wt.Gate, workflow, (i + 1) * 100, wt.RequestName, ipg_SeverityLevel.Warning/*wt.SevirityLevel*/, wt.PassMessage, wt.FailMessage);

                fakedData.Add(workflow);
                fakedData.Add(gateconfigdetails);
            }

            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeMessageExecutor(Wts));
            fakedContext.AddExecutionMock<ipg_IPGGatingVerifyBenefitsRequest>(ExecuteBVFMock);
            fakedContext.AddExecutionMock<ExecuteWorkflowRequest>(ExecuteBVFMock);

            fakedContext.Initialize(fakedData);

            var metadata = new EntityMetadata()
            {
                LogicalName = ipg_referral.EntityLogicalName
            };

            metadata.SetAttributeCollection(referralFieldsInMetadata.Select(field => new AttributeMetadata() {LogicalName = field, DisplayName = new Label() {UserLocalizedLabel = new LocalizedLabel() { Label = field} }  }));
            fakedContext.InitializeMetadata(metadata);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() }, { "InitiatingUser", initiatingUser } };
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

            Assert.True(acceptReferralExecuted);
            Assert.True(bvfRunned);

            acceptReferralExecuted = false;
            bvfRunned = false;
        }

        [Fact]
        public void ReferralPassGate2AndCaseNotCreated()
        {
            var fakedContext = new XrmFakedContext();
            var fakedData = new List<Entity>();

            var referralFieldsInMetadata = new List<string>
            { ipg_referral.Fields.ipg_PatientLastName,
            ipg_referral.Fields.ipg_PatientFirstName,
            ipg_referral.Fields.ipg_PatientDateofBirth,
            ipg_referral.Fields.ipg_PatientCity,
            ipg_referral.Fields.ipg_PatientAddress,
            ipg_referral.Fields.ipg_PatientState,
            ipg_referral.Fields.ipg_PatientZipCodeId,
            ipg_referral.Fields.ipg_cellphone,
            ipg_referral.Fields.ipg_memberidnumber,
            ipg_referral.Fields.ipg_CarrierId,
            ipg_referral.Fields.ipg_autoclaimnumber,
            ipg_referral.Fields.ipg_autoadjustername,
            ipg_referral.Fields.ipg_autodateofincident};

            SystemUser user = new SystemUser().Fake();
            ipg_gateconfiguration gate1config = new ipg_gateconfiguration().Fake("Gate 1");
            ipg_gateconfiguration gate2config = new ipg_gateconfiguration().Fake("Gate 2");
            ipg_gateconfiguration gate3config = new ipg_gateconfiguration().Fake("Gate 3");

            ipg_lifecyclestep intake1 = new ipg_lifecyclestep().Fake(gate1config);
            ipg_lifecyclestep intake2 = new ipg_lifecyclestep().Fake(gate2config);
            ipg_lifecyclestep intake3 = new ipg_lifecyclestep().Fake(gate3config);

            ipg_gateprocessingrule gateprocessingrule = new ipg_gateprocessingrule().Fake(intake1, intake2, "", ipg_SeverityLevel.Warning);

            ipg_procedurename procedureName = new ipg_procedurename().Fake().WithActiveValue(true);
            EntityReference initiatingUser = user.ToEntityReference();

            Intake.Account carrier = new Intake.Account().Fake().WithCarrierAccepted();

            Intake.Account facility = new Intake.Account().Fake();

            var dos = DateTime.Now.AddDays(30);
            ipg_cptcode cpt1 = new ipg_cptcode().Fake()
             .WithEffectiveDate(dos.AddDays(-30))
            .WithExpirationDate(dos.AddDays(60));
            ipg_cptcode cpt2 = new ipg_cptcode().Fake();

            Entitlement entitlement = new Entitlement().Fake()
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(dos, dos.AddDays(5));

            ipg_facilitycarriercptrulecontract facilitycarriercptrulecontract =
               new ipg_facilitycarriercptrulecontract().Fake()
               .WithCPTInclusionType(ipg_cptinclusiontyperule.Included)
               .FakeWithEntitlement(entitlement)
               .WithEffectivesDates(dos, dos.AddDays(5));

            ipg_facilitycarriercptrule facilitycarriercptrule = new ipg_facilitycarriercptrule().Fake()
                .FakeWithCptCode(cpt1)
                .FakeWithContract(facilitycarriercptrulecontract)
                .WithEffectivesDates(dos, dos.AddDays(5));


            ipg_referral referral = new ipg_referral().Fake()
                .WithLFstep(intake1)
                .WithOwner(user)
                .WithCarrierReference(carrier)
                .WithSurgeryDate(dos)
                .WithCptCodeReferences(new List<ipg_cptcode>() { cpt1, cpt2 })
                .WithProcedureName(procedureName)
                .WithFacilityReference(facility)
                .WithCaseStatus(ipg_CaseStatus.Open);

            fakedData.AddRange(new List<Entity>() {
                gate1config,
                gate2config,
                intake1,
                intake2,
                intake3,
                referral,
                user,
                carrier,
                entitlement,
                cpt1,
                cpt2,
                facilitycarriercptrule,
                facility,
                procedureName,
                gate3config,
                gateprocessingrule}); 

            var Wts= new List<GateConfig>()
            { 
                new GateConfig(typeof(CPTSupportedByIPG), "ipg_IPGGatingCPTSupportedByIPG", ipg_SeverityLevel.Critical, "Passed", "Failed", gate1config),
                new GateConfig(typeof(CPTSupportedByFacility), "ipg_IPGGatingCPTSupportedByFacility", ipg_SeverityLevel.Critical, "Passed","Unable to Authorize - CPT is not supported by the Facility", gate1config),
                new GateConfig(typeof(CheckDuplicateCase), "ipg_IPGGatingCheckDuplicateCase", ipg_SeverityLevel.Warning, "No potential duplicate cases.", "There are potential duplicate cases. Please check notes on the case for detail.", gate1config)
            };

            for (var i = 0; i < Wts.Count; i++ )
            {
                var wt = Wts[i];
                Intake.Workflow workflow = new Intake.Workflow().Fake(wt.RequestName.Replace("ipg_", ""));
                ipg_gateconfigurationdetail gateconfigdetails = new ipg_gateconfigurationdetail().Fake(wt.Gate, workflow, (i + 1) *100, wt.RequestName, wt.SevirityLevel, wt.PassMessage, wt.FailMessage);
                
                fakedData.Add(workflow);
                fakedData.Add(gateconfigdetails);
            }

            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeMessageExecutor(Wts));
            fakedContext.AddExecutionMock<ipg_IPGGatingVerifyBenefitsRequest>(ExecuteBVFMock);
            fakedContext.AddExecutionMock<ExecuteWorkflowRequest>(ExecuteBVFMock);

            fakedContext.Initialize(fakedData);

            var metadata = new EntityMetadata()
            {
                LogicalName = ipg_referral.EntityLogicalName
            };

            metadata.SetAttributeCollection(referralFieldsInMetadata.Select(field => new AttributeMetadata() { LogicalName = field, DisplayName = new Label() { UserLocalizedLabel = new LocalizedLabel() { Label = field } } }));
            fakedContext.InitializeMetadata(metadata);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() }, { "InitiatingUser", initiatingUser } };
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

            Assert.False(acceptReferralExecuted);
            Assert.False(bvfRunned);
        }

        public OrganizationResponse ExecuteBVFMock(OrganizationRequest req)
        {
            bvfRunned = true;
            return new ExecuteWorkflowResponse {};
        }
        class FakeMessageExecutor : IFakeMessageExecutor
        {
            private List<GateConfig> _gateConfigs;
            public FakeMessageExecutor(List<GateConfig> gateConfigs)
            {
                _gateConfigs = gateConfigs;
            }
            public bool CanExecute(OrganizationRequest request)
            {
                return _gateConfigs.Any(g => g.RequestName == request.RequestName)
                    || request.RequestName == "ipg_IPGIntakeActionsAcceptReferral"
                    || request.RequestName == "ipg_IPGGatingPostProcessGate2";
            }

            public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
            {
                ParameterCollection outputParameters = new ParameterCollection();
                if(request.RequestName == "ipg_IPGGatingPostProcessGate2")
                {
                    var pluginContext = new XrmFakedPluginExecutionContext()
                    {
                        MessageName = request.RequestName,
                        Stage = 40, //post
                        PrimaryEntityName = Incident.EntityLogicalName,
                        InputParameters = request.Parameters,
                        OutputParameters = outputParameters,
                        PostEntityImages = null,
                        PreEntityImages = null
                    };
                    MethodInfo method = typeof(XrmFakedContext).GetMethods().Where(x => x.Name == nameof(ctx.ExecutePluginWith))
                        .FirstOrDefault(x => x.IsGenericMethod);

                    MethodInfo generic = method.MakeGenericMethod(typeof(Gate2));
                    generic.Invoke(ctx, new List<Object>() { pluginContext }.ToArray());
                }
                else if (request.RequestName == "ipg_IPGIntakeActionsAcceptReferral")
                {
                    acceptReferralExecuted = true;
                    outputParameters["Succeeded"] = true;
                    var svc = ctx.GetOrganizationService();
                    var refRef = request.Parameters["Target"] as EntityReference;
                    var caseRef = new EntityReference(Incident.EntityLogicalName, svc.Create(new Incident()));
                    svc.Update(new ipg_referral() { Id = refRef.Id, ipg_AssociatedCaseId = caseRef });
                }
                else
                {

                    var _pluginType = _gateConfigs.First(g => g.RequestName == request.RequestName).WTPlugin;

                    var pluginContext = new XrmFakedPluginExecutionContext()
                    {
                        MessageName = request.RequestName,
                        Stage = 40, //post
                        PrimaryEntityName = ipg_referral.EntityLogicalName,
                        InputParameters = request.Parameters,
                        OutputParameters = outputParameters,
                        PostEntityImages = null,
                        PreEntityImages = null
                    };
                    MethodInfo method = typeof(XrmFakedContext).GetMethods().Where(x => x.Name == nameof(ctx.ExecutePluginWith))
                        .FirstOrDefault(x => x.IsGenericMethod);

                    MethodInfo generic = method.MakeGenericMethod(_pluginType);
                    generic.Invoke(ctx, new List<Object>() { pluginContext }.ToArray());
                }

                OrganizationResponse response = new OrganizationResponse();
                response.Results = outputParameters;

                return response;
            }

            public Type GetResponsibleRequestType()
            {
                throw new NotImplementedException();
            }
        }
    }
}
