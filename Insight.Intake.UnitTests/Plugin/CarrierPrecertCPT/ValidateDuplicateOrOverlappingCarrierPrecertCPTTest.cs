using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Insight.Intake.UnitTests.Fakes;
using Insight.Intake.Plugin.CarrierPrecertCPT;

namespace Insight.Intake.UnitTests.Plugin.CarrierPrecertCPT
{
    public class ValidateDuplicateOrOverlappingCarrierPrecertCPTTest : PluginTestsBase
    {
        [Fact]
        public void CheckForOverlappingDates()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;
            ipg_cptcode cpt = new ipg_cptcode().Fake();
            Intake.Account carrier = new Intake.Account().Fake();

            ipg_carrierprecertcpt carrierprecertcpt = new ipg_carrierprecertcpt().Fake()
                .FakeWithCptCode(cpt)
                .FakeWithCarrier(carrier)
                .FakeWithEffectiveStartAndEndDate(new DateTime(2019, 01, 01), new DateTime(2019, 11, 27));


            ipg_carrierprecertcpt duplicateCarrierPrecertCPT = new ipg_carrierprecertcpt().Fake()
                                                                .FakeWithCptCode(cpt)
                                                                .FakeWithCarrier(carrier)
                                                                .FakeWithEffectiveStartAndEndDate(new DateTime(2018, 08, 01), new DateTime(2019, 07, 31));

            List<Entity> entityArray = new List<Entity>
            {
                duplicateCarrierPrecertCPT
            };

            OrganizationServiceMock.WithRetrieveCrud(carrierprecertcpt);

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_carrierprecertcpt.EntityLogicalName, entityArray);

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns(MessageNames.Create);

            var request = new CreateRequest
            {
                Target = carrierprecertcpt.ToEntity<Entity>()
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(40);
            PluginExecutionContextMock.Setup(x => x.PrimaryEntityName).Returns(ipg_carrierprecertcpt.EntityLogicalName);

            var outputParameters = new ParameterCollection
            {
                { "id", carrierprecertcpt.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new ValidateDuplicateOrOverlappingCarrierPrecertCPT();
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

            #endregion
        }

    }
}
