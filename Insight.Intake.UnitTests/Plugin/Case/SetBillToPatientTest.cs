using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Payment
{
    public class SetBillToPatientTest : PluginTestsBase
    {
        [Fact]
        public void CheckBillToPatientTrue()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account()
            {
                Id = Guid.NewGuid(),
                ipg_StatementToPatient = true
            };

            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_CarrierId = carrier.ToEntityReference(),
            };

            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<SetBillToPatient>(pluginContext);
            Assert.True(incident.ipg_BillToPatient.Value == (int)ipg_TwoOptions.Yes);
        }

        [Fact]
        public void CheckBillToPatientFalse()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account carrier = new Intake.Account()
            {
                Id = Guid.NewGuid(),
                ipg_StatementToPatient = true
            };

            Entitlement fc = new Entitlement()
            {
                Id = Guid.NewGuid(),
                ipg_Billtopatient = false
            };
            Intake.Account facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility);

            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_CarrierId = carrier.ToEntityReference(),
                ipg_ActualDOS = new DateTime(2021, 05, 20),
                ipg_FacilityId = facility.ToEntityReference()
            };



            List<Entity> entityArray = new List<Entity>
            {
                fc
            };

            OrganizationServiceMock.WithRetrieveCrud(carrier);

            OrganizationServiceMock.WithRetrieveMultipleCrud(Entitlement.EntityLogicalName, entityArray);

            OrganizationService = OrganizationServiceMock.Object;
            #endregion


            #region Setup execution context
            var inputParameters = new ParameterCollection { { "Target", incident } };

            PluginExecutionContextMock.Setup(x => x.MessageName).Returns(MessageNames.Create);
            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(inputParameters);
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(PipelineStages.PreOperation);
            PluginExecutionContextMock.Setup(x => x.PrimaryEntityName).Returns(Incident.EntityLogicalName);
            #endregion

            #region Execute plugin
            var plugin = new SetBillToPatient();
            plugin.Execute(ServiceProvider);
            #endregion

            #region Assert

            Assert.True(incident.ipg_BillToPatient.Value == (int)ipg_TwoOptions.No); 
            #endregion
        }

    }
}
