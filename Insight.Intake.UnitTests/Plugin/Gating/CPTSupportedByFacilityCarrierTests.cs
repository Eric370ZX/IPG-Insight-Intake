using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CPTSupportedByFacilityCarrierTests : PluginTestsBase
    {
        //should be reviewed

        /*[Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsNoContract_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();

            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility);

            var listForInit = new List<Entity> { referral, cptCode, facility, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractIncludedAll_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2));
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.IncludedAll));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);

            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractIncludedAndCPTCodeIsNotAssociated_returnSuccessFalse()
        {
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.Included));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);

            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractIncludedAndCPTCodeIsAssociated_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetFakedOrganizationService();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.Included));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);

            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            fakedContext.AddRelationship("ipg_entitlement_ipg_cptcode", new XrmFakedRelationship
            {
                IntersectEntity = "ipg_entitlement_ipg_cptcode",
                Entity1LogicalName = Entitlement.EntityLogicalName,
                Entity1Attribute = "entitlementid",
                Entity2LogicalName = ipg_cptcode.EntityLogicalName,
                Entity2Attribute = "ipg_cptcodeid"
            });

            var request = new AssociateRequest()
            {
                Target = contract.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference(ipg_cptcode.EntityLogicalName, cptCode.Id),
                },
                Relationship = new Relationship("ipg_entitlement_ipg_cptcode")
            };

            fakedService.Execute(request);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractIncludedAndOnlyOneCPTCodeIsAssociated_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetFakedOrganizationService();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            ipg_cptcode cptCode2 = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.Included));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);


            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode, cptCode2 })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            fakedContext.AddRelationship("ipg_entitlement_ipg_cptcode", new XrmFakedRelationship
            {
                IntersectEntity = "ipg_entitlement_ipg_cptcode",
                Entity1LogicalName = Entitlement.EntityLogicalName,
                Entity1Attribute = "entitlementid",
                Entity2LogicalName = ipg_cptcode.EntityLogicalName,
                Entity2Attribute = "ipg_cptcodeid"
            });

            var request = new AssociateRequest()
            {
                Target = contract.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference(ipg_cptcode.EntityLogicalName, cptCode.Id),
                },
                Relationship = new Relationship("ipg_entitlement_ipg_cptcode")
            };

            fakedService.Execute(request);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractExcludedAndOnlyOneCPTCodeIsAssociated_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetFakedOrganizationService();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            ipg_cptcode cptCode2 = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.Excluded));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);


            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode, cptCode2 })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            fakedContext.AddRelationship("ipg_entitlement_ipg_cptcode", new XrmFakedRelationship
            {
                IntersectEntity = "ipg_entitlement_ipg_cptcode",
                Entity1LogicalName = Entitlement.EntityLogicalName,
                Entity1Attribute = "entitlementid",
                Entity2LogicalName = ipg_cptcode.EntityLogicalName,
                Entity2Attribute = "ipg_cptcodeid"
            });

            var request = new AssociateRequest()
            {
                Target = contract.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference(ipg_cptcode.EntityLogicalName, cptCode.Id),
                },
                Relationship = new Relationship("ipg_entitlement_ipg_cptcode")
            };

            fakedService.Execute(request);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractExcludedAndCPTCodeIsAssociated_returnSuccessFalse()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetFakedOrganizationService();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.Excluded));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);

            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            fakedContext.AddRelationship("ipg_entitlement_ipg_cptcode", new XrmFakedRelationship
            {
                IntersectEntity = "ipg_entitlement_ipg_cptcode",
                Entity1LogicalName = Entitlement.EntityLogicalName,
                Entity1Attribute = "entitlementid",
                Entity2LogicalName = ipg_cptcode.EntityLogicalName,
                Entity2Attribute = "ipg_cptcodeid"
            });

            var request = new AssociateRequest()
            {
                Target = contract.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference(ipg_cptcode.EntityLogicalName, cptCode.Id),
                },
                Relationship = new Relationship("ipg_entitlement_ipg_cptcode")
            };

            fakedService.Execute(request);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckCPTSupportedByFacilityCarrier_ThereIsContractExcludedAndCPTCodeIsNotAssociated_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Entitlement contract = new Entitlement().Fake().WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                //.WithCPTInclusionType(new OptionSetValue((int)ipg_cptinclusiontyperule.Excluded));
            contract.StateCode = EntitlementState.Active;
            contract.StatusCode = new OptionSetValue((int)Entitlement_StatusCode.Active);

            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode })
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithSurgeryDate(DateTime.Now);

            var listForInit = new List<Entity> { referral, cptCode, contract, facility, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacilityCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacilityCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }*/
    }
}
