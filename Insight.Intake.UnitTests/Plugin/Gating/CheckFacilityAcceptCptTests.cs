using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckFacilityAcceptCptTests : PluginTestsBase
    {
        [Fact]
        public void CheckFacilityAcceptCptTests_EntitlementWithCptExist_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            var dos = DateTime.Now.AddDays(-30);
            ipg_cptcode cpt1 = new ipg_cptcode().Fake()
             .WithEffectiveDate(dos.AddDays(-30))
            .WithExpirationDate(dos.AddDays(30));
            ipg_cptcode cpt2 = new ipg_cptcode().Fake();

            Entitlement entitlement = new Entitlement().Fake()
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility);

            ipg_facilitycarriercptrulecontract contract = new ipg_facilitycarriercptrulecontract().Fake()
                                                              .FakeWithEntitlement(entitlement)
                                                              .WithCPTInclusionType(ipg_cptinclusiontyperule.Included);

            ipg_facilitycarriercptrule facilitycarriercptrule = new ipg_facilitycarriercptrule().Fake()
                .FakeWithCptCode(cpt1)
                .FakeWithContract(contract);


            ipg_referral refEntity = new ipg_referral().Fake()
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithSurgeryDate(dos)
                .WithCptCodeReferences(new List<ipg_cptcode>() { cpt1, cpt2 });

            var listForInit = new List<Entity>() { refEntity, carrier, facility, cpt1, cpt2, facilitycarriercptrule, contract, entitlement };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckFacilityAcceptCpt",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckFacilityAcceptCpt>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckFacilityAcceptCptTests_NoEntitlement_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            var dos = DateTime.Now.AddDays(-30);
            ipg_cptcode cpt1 = new ipg_cptcode().Fake()
             .WithEffectiveDate(dos.AddDays(-30))
            .WithExpirationDate(dos.AddDays(30));
            ipg_cptcode cpt2 = new ipg_cptcode().Fake();

            ipg_referral refEntity = new ipg_referral().Fake()
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithSurgeryDate(dos)
                .WithCptCodeReferences(new List<ipg_cptcode>() { cpt1, cpt2 });

            var listForInit = new List<Entity>() { refEntity, carrier, facility, cpt1, cpt2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckFacilityAcceptCpt",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckFacilityAcceptCpt>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
