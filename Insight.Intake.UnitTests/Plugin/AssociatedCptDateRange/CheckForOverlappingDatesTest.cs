using Insight.Intake.Plugin.AssociatedCPTDateRange;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.AssociatedCptDateRange
{
    public class CheckForOverlappingDatesTest : PluginTestsBase
    {
        [Fact]
        public void CheckForOverlappingDates()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;
            //ipg_cptcode cptCode = new ipg_cptcode().Fake();
            //Entitlement carrierFacilityRelationship = new Entitlement().Fake();
            //carrierFacilityRelationship.StartDate = DateTime.Today;
            //carrierFacilityRelationship.EndDate = DateTime.Today;
            //ipg_facilitycarriercptrule facilityCarrierCPTRule = new ipg_facilitycarriercptrule().Fake()
            //                                                             .FakeWithCptCode(cptCode)
            //                                                             .FakeWithEntitlement(carrierFacilityRelationship);

            ipg_associatedcpt  associatedcpt= new ipg_associatedcpt().Fake();

            ipg_associatedcptdaterange fakeDateRangeOnCreate = new ipg_associatedcptdaterange().Fake()
                .FakeWithFacilityReference(associatedcpt, new DateTime(2019, 01, 01), new DateTime(2019, 11, 27));

            List<Entity> dateRangeArray = new List<Entity>
            {
                new ipg_associatedcptdaterange().Fake()
                       .FakeWithFacilityReference(associatedcpt, new DateTime(2018, 08, 01), new DateTime(2019, 07 , 31))
            };

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_associatedcptdaterange.EntityLogicalName, dateRangeArray);

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Create");

            var request = new CreateRequest
            {
                Target = fakeDateRangeOnCreate.ToEntity<Entity>()
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(40);
            PluginExecutionContextMock.Setup(x => x.PrimaryEntityName).Returns(ipg_associatedcptdaterange.EntityLogicalName);

            var outputParameters = new ParameterCollection
            {
                { "id", fakeDateRangeOnCreate.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new CheckForOverlappingDates();
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

            #endregion
        }
    }
}

