using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class UpdatePatientInformationPluginTests: PluginTestsBase
    {
        [Fact]
        public void MostRecentReferralUpdateTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            ipg_referral updatedReferral = new ipg_referral().Fake().WithPatientReference(patient);
            ipg_document document = new ipg_document().Fake().WithReferralReference(updatedReferral);

            updatedReferral.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { updatedReferral, patient, document};

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", updatedReferral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", updatedReferral } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);


            //documents
            //documents
            Assert.All((from ent in crmContext.CreateQuery<ipg_document>()
                        where ent.ipg_ReferralId.Id == updatedReferral.Id
                        select ent),
                        item => (from ent in crmContext.CreateQuery<ipg_document>()
                                 where ent.ipg_patientid.Id == patient.Id
                                 select ent).ToList().Contains(item));
            #endregion
        }

        [Fact]
        public void OnDocumentCreatedWithMostRecentReferral()
        {
                #region Setup services
                var fakedContext = new XrmFakedContext();
                //Create a fake patient contact and incident. Bind the contact with the incident.

                Contact patient = new Contact().Fake();
                ipg_referral updatedReferral = new ipg_referral().Fake().WithPatientReference(patient);
                ipg_document document = new ipg_document().Fake().WithReferralReference(updatedReferral);

                updatedReferral.ipg_SurgeryDate = DateTime.Today;


                var listForInit = new List<Entity> { updatedReferral, patient, document };

                fakedContext.Initialize(listForInit);

                #endregion

                #region Setup execution context

                var pluginContext = new XrmFakedPluginExecutionContext()
                {
                    MessageName = MessageNames.Create,
                    Stage = PipelineStages.PostOperation,
                    PrimaryEntityName = ipg_document.EntityLogicalName,
                    InputParameters = new ParameterCollection() { { "Target", document } },
                    OutputParameters = new ParameterCollection(),
                    PostEntityImages = new EntityImageCollection() { { "PostImage", document } },
                    PreEntityImages = new EntityImageCollection(),
                    InitiatingUserId = Guid.NewGuid()
                };
                #endregion

                #region Execute plugin

                fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

                #endregion

                #region Asserts
                var fakedService = fakedContext.GetOrganizationService();
                var crmContext = new OrganizationServiceContext(fakedService);


                //documents
                Assert.All((from ent in crmContext.CreateQuery<ipg_document>()
                            where ent.ipg_ReferralId.Id == updatedReferral.Id
                            select ent),
                            item => (from ent in crmContext.CreateQuery<ipg_document>()
                                     where ent.ipg_patientid.Id == patient.Id
                                     select ent).ToList().Contains(item));
                #endregion
            }
    }
}
