using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckSupportedPartsTests : PluginTestsBase
    {
        [Fact]
        public void CheckSupportedPartsTests_AllPartsHaveOrder_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_casepartdetail casePart = new ipg_casepartdetail().Fake();
            casePart.ipg_caseid = caseEntity.ToEntityReference();
            casePart.ipg_PurchaseOrderId = new EntityReference();
            casePart.ipg_productid = new EntityReference("ipg_product", new Guid("{881F5BE0-81B1-462A-9767-71C0A6C56EC2}"));//just random guid
            var listForInit = new List<Entity>() { caseEntity, casePart };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckSupportedParts",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckSupportedParts>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckSupportedPartsTests_PartIsNotSupported_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            var productReference = new EntityReference("product", new Guid("{DB6FCF52-4D7E-4F4A-AA60-317E42688DB3}"));//random guid
            Incident caseEntity = new Incident().Fake();
            ipg_casepartdetail casePart = new ipg_casepartdetail().Fake();
            casePart.ipg_caseid = caseEntity.ToEntityReference();
            casePart.ipg_PurchaseOrderId = null;
            casePart.ipg_productid = productReference;

            ipg_unsupportedpart unsPart = new ipg_unsupportedpart().Fake();
            unsPart.ipg_ProductId = productReference;
            unsPart.StateCode = ipg_unsupportedpartState.Active;
            unsPart.ipg_EffectiveDate = DateTime.Now.AddDays(-1);

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCarrierBalance",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCarrierBalance>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckSupportedPartsTests_PartsWithoutPO_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_casepartdetail casePart = new ipg_casepartdetail().Fake();
            casePart.ipg_caseid = caseEntity.ToEntityReference();
            casePart.ipg_PurchaseOrderId = null;
            var listForInit = new List<Entity>() { caseEntity, casePart };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckSupportedParts",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckSupportedParts>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
        [Fact]
        public void CheckSupportedPartsTests_PartsWereDebited_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            var productId = new Guid("{881F5BE0-81B1-462A-9767-71C0A6C56EC2}");
            ipg_casepartdetail casePart = new ipg_casepartdetail().Fake();
            casePart.ipg_caseid = caseEntity.ToEntityReference();
            casePart.ipg_PurchaseOrderId = new EntityReference();
            casePart.ipg_productid = new EntityReference("ipg_product", productId);//just random guid
            casePart.ipg_quantity = 5;

            ipg_casepartdetail casePart2 = new ipg_casepartdetail().Fake();
            casePart2.ipg_caseid = caseEntity.ToEntityReference();
            casePart2.ipg_PurchaseOrderId = new EntityReference();
            casePart2.ipg_productid = new EntityReference("ipg_product", productId);//just random guid
            casePart2.ipg_quantity = -5;

            var listForInit = new List<Entity>() { caseEntity, casePart, casePart2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckSupportedParts",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckSupportedParts>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
