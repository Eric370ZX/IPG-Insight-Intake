using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Plugin.Workflow;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Workflow
{
    public class CopyDocumentsPluginTests : PluginTestsBase
    {
        [Fact]
        public void CheckWhatDocumentsCopiedCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_referral referral = new ipg_referral().Fake();

            Incident incident = new Incident().Fake();

            var numberOfDocuments = new Random().Next(5, 10);
            
            IList<ipg_document> documents = new ipg_document().Fake()
                .WithReferralReference(referral)
                .Generate(numberOfDocuments);

            IList<ipg_document> updatedDocuments = new List<ipg_document>();

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_document.EntityLogicalName, documents.Cast<Entity>().ToList());

            OrganizationServiceMock.WithUpdateCrud<ipg_document>().Callback<Entity>(x => updatedDocuments.Add(x.ToEntity<ipg_document>()));
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGIntakeActionsCopyDocuments");

            var request = new ipg_IPGIntakeActionsCopyDocumentsRequest
            {
                SourceReference = new EntityReference(ipg_referral.EntityLogicalName, referral.Id),
                TargetReference = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new CopyDocumentsPlugin();

            plugin.Execute(ServiceProvider);
            
            Assert.Equal(documents.Count, updatedDocuments.Count);

            foreach (var document in documents)
            {
                var updatedDocument = updatedDocuments.FirstOrDefault(x => document.Id == x.Id);
                
                Assert.NotNull(updatedDocument);
                
                Assert.Equal(incident.LogicalName, updatedDocument.ipg_CaseId.LogicalName);
                
                Assert.Equal(incident.Id, updatedDocument.ipg_CaseId.Id);
            }
            #endregion
        }
    }
}