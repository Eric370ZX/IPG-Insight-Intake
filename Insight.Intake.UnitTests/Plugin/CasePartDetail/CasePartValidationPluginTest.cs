using FakeXrmEasy;
using Insight.Intake.Plugin.CasePartDetail;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using ParameterCollection = Microsoft.Xrm.Sdk.ParameterCollection;

namespace Insight.Intake.UnitTests.Plugin.CasePartDetail
{
    public class CasePartValidationPluginTest: PluginTestsBase
    {
        const string NotValidPO_ERROR = "Error: PO Type not Valid!";

        [Fact]
        public void CheckCreateZPO_MPPart_Fails()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();

            Incident incident = new Incident().Fake()
                .WithScheduledDos(new DateTime())
                .WithCarrierReference(carrier);

            Intake.Product product = new Intake.Product().Fake().WithBoxQuantity(2);


            ipg_casepartdetail actualPart = new ipg_casepartdetail().Fake()
                .WithPOType((int)ipg_PurchaseOrderTypes.ZPO)
                .WithCaseReference(incident)
                .WithProductReference(product);

            #endregion

            #region Setup execution context

            fakedContext.Initialize(new List<Entity>() {incident, product });
            var inputParameters = new ParameterCollection() { { "Target", actualPart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = actualPart.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection()
            };

            InvalidPluginExecutionException exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CasePartValidationPlugin>(pluginContext));
            Assert.IsTrue(exception.Message == NotValidPO_ERROR);

            #endregion
        }

        [Fact]
        public void CheckUpdateZPO_MPPart_Fails()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();

            Incident incident = new Incident().Fake()
                .WithScheduledDos(new DateTime())
                .WithCarrierReference(carrier);
            
            Intake.Product product = new Intake.Product().Fake().WithBoxQuantity(2);


            ipg_casepartdetail actualPart = new ipg_casepartdetail().Fake()
                .WithPOType((int)ipg_PurchaseOrderTypes.CPA)
                .WithCaseReference(incident)
                .WithProductReference(product);

            #endregion

            #region Setup execution context

            fakedContext.Initialize(new List<Entity>() {actualPart, incident, product });

            var inputParameters = new ParameterCollection() { { "Target", new ipg_casepartdetail() { Id = actualPart.Id, ipg_potypecodeEnum = ipg_PurchaseOrderTypes.ZPO } } };
            var preImages = new EntityImageCollection() { { "PreImage", actualPart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = actualPart.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = preImages
            };


            InvalidPluginExecutionException exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CasePartValidationPlugin>(pluginContext));
            Assert.IsTrue(exception.Message == NotValidPO_ERROR);

            #endregion
        }

        [Fact]
        public void CheckUpdatePOTypeWhenPOOpen()
        {
            #region Setup execution context
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();
            SalesOrder po = new SalesOrder().Fake();
            Incident incident = new Incident().Fake()
                .WithScheduledDos(new DateTime())
                .WithCarrierReference(carrier);

            Intake.Product product = new Intake.Product().Fake();

            ipg_casepartdetail actualPart = new ipg_casepartdetail().Fake()
                .WithPO(po)
                .WithPOType((int)ipg_PurchaseOrderTypes.MPO)
                .WithCaseReference(incident)
                .WithProductReference(product);

            fakedContext.Initialize(new List<Entity>() { po, actualPart, incident, product });

            var inputParameters = new ParameterCollection() { { "Target", new ipg_casepartdetail() {Id = actualPart.Id, ipg_potypecodeEnum = ipg_PurchaseOrderTypes.CPA } } };
            var preImages = new EntityImageCollection() { {"PreImage", actualPart }};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = actualPart.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = preImages
            };

            #endregion

            fakedContext.ExecutePluginWith<CasePartValidationPlugin>(pluginContext);
        }

        [Fact]
        public void CheckUpdatePOTypeWhenPOClosed()
        {
            #region Setup execution context
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();
            SalesOrder po = new SalesOrder().Fake().WithStateCode(SalesOrderState.Fulfilled);
            Incident incident = new Incident().Fake()
                .WithScheduledDos(new DateTime())
                .WithCarrierReference(carrier);

            Intake.Product product = new Intake.Product().Fake();

            ipg_casepartdetail actualPart = new ipg_casepartdetail().Fake()
                .WithPO(po)
                .WithPOType((int)ipg_PurchaseOrderTypes.MPO)
                .WithCaseReference(incident)
                .WithProductReference(product);

            fakedContext.Initialize(new List<Entity>() { po, actualPart, incident, product });

            var inputParameters = new ParameterCollection() { { "Target", new ipg_casepartdetail() { Id = actualPart.Id, ipg_potypecodeEnum = ipg_PurchaseOrderTypes.CPA } } };
            var preImages = new EntityImageCollection() { { "PreImage", actualPart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = actualPart.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = preImages
            };

            #endregion

            Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CasePartValidationPlugin>(pluginContext), "Error: You cannot Modify PO type as Parent PO is not Open!");
        }
        [Fact]
        public void TestThatTPOCreated()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();

            Incident incident = new Incident().Fake()
                .WithScheduledDos(DateTime.Now.AddDays(5))
                .WithCarrierReference(carrier);

            Intake.Product product = new Intake.Product().Fake().WithBoxQuantity(1);


            ipg_casepartdetail actualPart = new ipg_casepartdetail().Fake()
                .WithPOType((int)ipg_PurchaseOrderTypes.TPO)
                .WithCaseReference(incident)
                .WithProductReference(product);

            #endregion

            #region Setup execution context

            fakedContext.Initialize(new List<Entity>() { incident, product });

            var inputParameters = new ParameterCollection() { { "Target", actualPart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = actualPart.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
            };


            fakedContext.ExecutePluginWith<CasePartValidationPlugin>(pluginContext);

            #endregion
        }

        [Fact]
        public void TestThatTPONotCreated()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();

            Incident incident = new Incident().Fake()
                .WithScheduledDos(DateTime.Now.AddDays(-5))
                .WithCarrierReference(carrier);

            Intake.Product product = new Intake.Product().Fake().WithBoxQuantity(1);


            ipg_casepartdetail actualPart = new ipg_casepartdetail().Fake()
                .WithPOType((int)ipg_PurchaseOrderTypes.TPO)
                .WithCaseReference(incident)
                .WithProductReference(product);

            #endregion

            #region Setup execution context

            fakedContext.Initialize(new List<Entity>() {incident, product });

            var inputParameters = new ParameterCollection() { { "Target", actualPart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = actualPart.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
            };


            InvalidPluginExecutionException exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CasePartValidationPlugin>(pluginContext));
            Assert.IsTrue(exception.Message == NotValidPO_ERROR);

            #endregion
        }
    }
}
