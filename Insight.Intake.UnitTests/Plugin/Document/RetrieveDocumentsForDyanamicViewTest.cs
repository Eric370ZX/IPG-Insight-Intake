using FakeXrmEasy;
using Insight.Intake;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class RetrieveDocumentsForDyanamicViewTest: PluginTestsBase
    {
        [Fact]
        public void RenameDocumentWhenAttachedToCaseTest()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1");
            Incident caseEntity = new Incident().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity)
                .WithRevision(3);

            document.ipg_name = "Document Test Name";

            var listForInit = new List<Entity> { documentType, document, caseEntity };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Query"
                    , new FetchExpression(@"<fetch count='10' distinct='true' no-lock='true' returntotalrecordcount='true' page='1' >
                                          <entity name='ipg_document' >
                                            <attribute name='ipg_name' />
                                            <attribute name='createdon' />
                                            <attribute name='ipg_documentid' />
                                            <order attribute='createdon' descending='true' />
                                            <filter type='and' >
                                              <condition attribute='statecode' operator='eq' value='0' />
                                              <condition attribute='createdon' operator='le' value='2021-8-25' />
                                            </filter>
                                            <link-entity name='incident' from='incidentid' to='ipg_caseid' link-type='inner' alias='as' >
                                              <attribute name='title' />
                                              <attribute name='incidentid' />
                                              <attribute name='ipg_procedureid' />
                                              <filter type='and' >
                                                <condition attribute='ipg_facilityid' operator='eq' value='C9C40F5A-CE77-2E72-546F-5555F10433B7' uiname='Precision Ambulatory Surgery Center, LLC' uitype='account' />
                                              </filter>
                                              <link-entity name='ipg_casepartdetail' from='ipg_caseid' to='incidentid' link-type='inner' alias='actualPart' >
                                                <attribute name='ipg_purchaseorderid' />
                                                <attribute name='ipg_costoverride' />
                                                <attribute name='ipg_quantity' />
                                                <attribute name='ipg_potypecode' />
                                                <filter type='and' >
                                                  <condition attribute='ipg_productid' operator='eq' value='160b72f9-4c05-4fea-8f50-4dd66b454406' uiname='# 10 BLADE' uitype='product' />
                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                </filter>
                                              </link-entity>
                                            </link-entity>
                                            <link-entity name='ipg_documenttype' from='ipg_documenttypeid' to='ipg_documenttypeid' link-type='inner' >
                                              <filter>
                                                <condition attribute='ipg_documenttypeabbreviation' operator='eq' value='MFG INV' />
                                              </filter>
                                            </link-entity>
                                          </entity>
                                        </fetch>") } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.RetrieveMultiple,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RetrieveDocumentsForDynamicView>(pluginContext);
        }
    }
}
