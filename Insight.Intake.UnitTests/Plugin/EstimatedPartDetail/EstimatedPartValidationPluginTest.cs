using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Insight.Intake.Plugin.EstimatedCasePartDetail;

namespace Insight.Intake.UnitTests.Plugin.EstimatedPartDetail
{
    public class EstimatedPartValidationPluginTest : PluginTestsBase
    {
        const string ERROR = "Error: ZPO can't have MultiPack Part!";

        [Fact]
        public void CheckCreateZPO_MPPart()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account fakeCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier);
            Incident fakeCase = new Incident().Fake()
                .WithCarrierReference(fakeCarrier);
            fakeCase.ipg_HomePlanCarrierId = fakeCarrier.ToEntityReference();
            fakeCase.ipg_ActualDOS = new DateTime(2021, 1, 1);
            fakeCase.ipg_SurgeryDate = new DateTime(2021, 1, 1);

            Intake.Product product = new Intake.Product().Fake().RuleFor(p => p.ipg_boxquantity, p => 2);

            OrganizationServiceMock.WithRetrieveCrud(fakeCase);
            OrganizationServiceMock.WithRetrieveCrud(product);

            OrganizationService = OrganizationServiceMock.Object;

            ipg_estimatedcasepartdetail part = new ipg_estimatedcasepartdetail().Fake().WithProductReference(product)
                                                              .RuleFor(p => p.ipg_potypecodeEnum, p => ipg_PurchaseOrderTypes.ZPO)
                                                              .RuleFor(p => p.ipg_caseid, p => fakeCase.ToEntityReference());

            #endregion

            #region Setup execution context

            var inputParameters = new ParameterCollection
            {
                {"Target", part}
            };

            PluginExecutionContextMock.Setup(c => c.InputParameters).Returns(inputParameters);
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PreValidation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Create);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_estimatedcasepartdetail.EntityLogicalName);
            PluginExecutionContextMock.Setup(c => c.PreEntityImages).Returns(new EntityImageCollection());


            EstimatedCasePartValidationPlugin estimatedCasePartValidationPlugin = new EstimatedCasePartValidationPlugin();
            InvalidPluginExecutionException exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => estimatedCasePartValidationPlugin.Execute(ServiceProvider));
            Assert.IsTrue(exception.Message == ERROR);

            #endregion
        }

        [Fact]
        public void CheckUpdateZPO_MPPart()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account fakeCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier);
            Incident fakeCase = new Incident().Fake()
                .WithCarrierReference(fakeCarrier);
            fakeCase.ipg_HomePlanCarrierId = fakeCarrier.ToEntityReference();
            fakeCase.ipg_ActualDOS = new DateTime(2021, 1, 1);
            fakeCase.ipg_SurgeryDate = new DateTime(2021, 1, 1);

            Intake.Product product = new Intake.Product().Fake().RuleFor(p => p.ipg_boxquantity, p => 2);

            OrganizationServiceMock.WithRetrieveCrud(fakeCase);
            OrganizationServiceMock.WithRetrieveCrud(product);

            OrganizationService = OrganizationServiceMock.Object;

            ipg_estimatedcasepartdetail part = new ipg_estimatedcasepartdetail().Fake().WithProductReference(product)
                                                              .RuleFor(p => p.ipg_potypecodeEnum, p => ipg_PurchaseOrderTypes.ZPO)
                                                              .RuleFor(p => p.ipg_caseid, p => fakeCase.ToEntityReference());

            #endregion

            #region Setup execution context

            var inputParameters = new ParameterCollection
            {
                {"Target", part}
            };

            PluginExecutionContextMock.Setup(c => c.InputParameters).Returns(inputParameters);
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PreValidation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Update);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_estimatedcasepartdetail.EntityLogicalName);
            PluginExecutionContextMock.Setup(c => c.PreEntityImages).Returns(new EntityImageCollection());


            EstimatedCasePartValidationPlugin estimatedCasePartValidationPlugin = new EstimatedCasePartValidationPlugin();
            InvalidPluginExecutionException exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => estimatedCasePartValidationPlugin.Execute(ServiceProvider));
            Assert.IsTrue(exception.Message == ERROR);

            #endregion
        }
    }
}
