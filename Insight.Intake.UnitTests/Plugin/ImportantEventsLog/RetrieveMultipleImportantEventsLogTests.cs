using FakeXrmEasy;
using Insight.Intake.Plugin.ImportantEventsLog;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ImportantEventsLog
{
    public class RetrieveMultipleImportantEventsLogTests : PluginTestsBase
    {
        private XrmFakedContext fakedContext = new XrmFakedContext();
        private string fetchQuery;

        [Fact]
        public void RetrieveMultipleImportantEventsLog_ShouldReturnOnlyEventLogsWithVisibilityIsAll()
        {
            // Arrange
            fakedContext.AddExecutionMock<FetchXmlToQueryExpressionRequest>(RetrieveEntityMock);
            var fakedService = fakedContext.GetOrganizationService();

            var fakedCase = new Incident() { Id = Guid.NewGuid() };
            fakedService.Create(fakedCase);
            var fakedCaseId = fakedCase.Id.ToString().Replace("{", "").Replace("}", "");

            fetchQuery =
                $@"<fetch no-lock='false' count='5' page='1' returntotalrecordcount='true' distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                    <entity name = 'ipg_importanteventslog' >
                        <attribute name = 'ipg_name' />
                        <attribute name = 'statecode' />
                        <attribute name = 'ipg_datetimestamp' />
                        <attribute name = 'ipg_activity' />
                        <attribute name = 'ipg_activitydescription' />
                        <attribute name = 'ipg_performedby' />
                        <attribute name = 'ipg_casestatusdisplayed' />
                        <order descending = 'true' attribute = 'ipg_datetimestamp' />
                        <attribute name = 'ipg_importanteventslogid' />
                        <attribute name = 'ipg_configid' />
                            <filter type = 'and' >
                                <condition attribute = 'ipg_caseid' value = '{fakedCaseId}' operator= 'eq' />
                            </filter>
                    </entity>
                </fetch>";
            var fakedFetchQuery = new FetchExpression(fetchQuery);

            var listToInitialize = new List<Entity>()
            {
                new ipg_importanteventconfig() { Id = Guid.NewGuid(), ipg_name = "ET1", ipg_visibility = true },
                new ipg_importanteventconfig() { Id = Guid.NewGuid(), ipg_name = "ET2", ipg_visibility = false },
                new ipg_importanteventconfig() { Id = Guid.NewGuid(), ipg_name = "ET3", ipg_visibility = false },

                new ipg_importanteventslog(){ Id = Guid.NewGuid(), ipg_configId = "ET1", ipg_caseid = fakedCaseId },
                new ipg_importanteventslog(){ Id = Guid.NewGuid(), ipg_configId = "ET2", ipg_caseid = fakedCaseId },
                new ipg_importanteventslog(){ Id = Guid.NewGuid(), ipg_configId = "ET3", ipg_caseid = fakedCaseId }
            };
            fakedContext.Initialize(listToInitialize);

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.RetrieveMultiple,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_importanteventslog.EntityLogicalName,
                PreEntityImages = null,
                PostEntityImages = null,
                InputParameters = new ParameterCollection { { "Query", fakedFetchQuery } }
            };

            // Act
            fakedContext.ExecutePluginWith<RetrieveMultipleImportantEventsLogFilter>(fakedPluginContext);
            var results = fakedService.RetrieveMultiple(fakedFetchQuery).Entities; 

            // Assert
            Assert.NotNull(results);
            results.Select(x => x.ToEntity<ipg_importanteventslog>().ipg_configId).ToList().ForEach(configId => Assert.DoesNotContain("ET1", configId));
            Assert.True(results.Count == 2);
        }

        private OrganizationResponse RetrieveEntityMock(OrganizationRequest req)
        {
            var response = new FetchXmlToQueryExpressionResponse();
            response["Query"] = XrmFakedContext.TranslateFetchXmlToQueryExpression(fakedContext, fetchQuery);
            return response;
        }
    }
}