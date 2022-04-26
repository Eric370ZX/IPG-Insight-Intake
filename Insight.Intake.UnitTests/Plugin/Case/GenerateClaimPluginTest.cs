using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;
using static Insight.Intake.Helpers.Constants;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class GenerateClaimPluginTest : PluginTestsBase
    {
        const string sourcepath = "../../PDF_Files/";
        const string cmspdfTemplatePath = sourcepath + "cms1500.pdf";
        const string ub04pdftemplatepath = sourcepath + "UB-04.pdf";
        [Fact]
        public void CheckValidationPoints()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;
            Incident incident = new Incident().Fake(IncidentState.Active)
                .WithActualDos(new DateTime(2000, 10, 10))
                .FakeWithClaimDetails(false, true, false);

            ipg_taskcategory taskcartegory = new ipg_taskcategory().Fake().WithName(TaskCategoryNames.PatientOutreach);
            var tasktype = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.Claim_Generation_Errors).WithCategory(taskcartegory);
            var fakedContext = new XrmFakedContext();

            fakedContext.Initialize(new List<Entity> { incident, taskcartegory, tasktype });
            #endregion

            #region Setup execution context

            var request = new ipg_IPGCaseActionsCreateClaimRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
                IsPrimaryOrSecondaryClaim = true,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                ClaimType = (int)ipg_claim_type.CMS1500,
                Icn = "",
                Box32 = "",
                Reason = ""
            };

            var output = new ipg_IPGCaseActionsCreateClaimResponse();


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
                OutputParameters = output.Results,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };
            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeExecutOrganizationRequest());

            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);

            Assert.True(output.HasErrors);

            var errorList = output.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var expectedErrors = new List<string>
            {
                "- Relation to Insurred must be selected",
            };

            foreach (var expectedError in expectedErrors)
            {
                Assert.Contains(expectedError, errorList);
            }
            #endregion
        }

        [Fact]
        public void ClaimCMS1500GenerationWithMultiplePages()
        {
            ipg_zipcode zip = new ipg_zipcode() { Id = Guid.NewGuid() };
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.Claim_Generation_Errors);
            ipg_carrierclaimsmailingaddress claimmalingaddress = new ipg_carrierclaimsmailingaddress().Fake(zip);
            ipg_globalsetting cmssetting = new ipg_globalsetting().Fake().WithName("CMS1500_PDF_TEMPLATE");
            ipg_globalsetting ub04setting = new ipg_globalsetting().Fake().WithName("UB04_PDF_TEMPLATE");

            ipg_globalsetting taskCMS1500Settings = new ipg_globalsetting().Fake("Task.InstitutionalClaimsReadyToPrint", "Task.InstitutionalClaimsReadyToPrint");
            ipg_globalsetting taskUB04Settings = new ipg_globalsetting().Fake("Task.ProfessionalClaimsReadyToPrint", "Task.ProfessionalClaimsReadyToPrint");

            ipg_tasktype tasktypeProfessionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.ProfessionalClaimsReadyToPrint");
            ipg_tasktype tasktypeInstitutionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.InstitutionalClaimsReadyToPrint");
            ipg_melissazipcode melisazipcode = new ipg_melissazipcode().Fake();

            Annotation cmsNote = new Annotation().Fake().WithObjectReference(cmssetting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(cmspdfTemplatePath)));
            Annotation ub04Note = new Annotation().Fake().WithObjectReference(ub04setting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(ub04pdftemplatepath)));


            ipg_documenttype doctypeCMS1500 = new ipg_documenttype().Fake("Claim CMS1500");
            ipg_documenttype doctypeUB04 = new ipg_documenttype().Fake("Claim UB04");
            Contact patient = new Contact().Fake();
            ipg_claimstatuscode newstatuscode = new ipg_claimstatuscode().FakeNewClaimStatusCode();
            ipg_claimstatuscode closedcliamstatuscode = new ipg_claimstatuscode().FakeClosedClaimStatusCode();
            ipg_state facilityState = new ipg_state().Fake();
            ipg_ipg_carriernetwork_ipg_state carriernetworkstate = new ipg_ipg_carriernetwork_ipg_state().Fake().WithState(facilityState);
            Intake.Account mfg = new Intake.Account().Fake(Account_CustomerTypeCode.Manufacturer);
            Intake.Account carrier = new Intake.Account().Fake().FakeCarrierForClaimGeneration(true, ipg_claim_type.CMS1500);
            Intake.Account facility = new Intake.Account().Fake().FakeFacilityForClaimGeneration(facilityState);
            Contact phisician = new Contact().FakeWithPhysicianDetails("33","588");
            Incident incident = new Incident().Fake().WithPrimaryCarrierReference(carrier).WithFacilityReference(facility)
                .WithInsuredPatient(new_RelationtoInsured.Self).WithPhysician(phisician).WithActualDos(DateTime.Now).WithPatientReference(patient)
                .WithPatientZipCode(melisazipcode)
                .WithPrimaryClaimAddress(claimmalingaddress)
                .WithSecondaryClaimAddress(claimmalingaddress);

            Intake.Product product = new Intake.Product().Fake().WithManufacturerReference(mfg);
            ipg_masterhcpcs hcpc = new ipg_masterhcpcs().Fake();

            List<ipg_casepartdetail> parts = new List<ipg_casepartdetail>();

            for (int i = 0; i <= 8; i++)
            {
                parts.Add(new ipg_casepartdetail().Fake().WithCaseReference(incident).WithHCPCS(hcpc).WithManufacturerReference(mfg).WithProductReference(product).WithBillableInfo(1, i, i));
            }

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize((new List<Entity> { cmssetting, ub04setting, cmsNote, ub04Note
                , incident, carrier,facilityState, carriernetworkstate, facility, phisician, mfg
                , newstatuscode, closedcliamstatuscode
                , product, hcpc
                , doctypeCMS1500, doctypeUB04
                , taskCMS1500Settings, taskUB04Settings, tasktypeProfessionalClaimsReadyToPrint, tasktypeInstitutionalClaimsReadyToPrint
                , patient
                , claimmalingaddress
                , tasktype
                , zip
                , melisazipcode
            }).Union(parts));
            var request = new ipg_IPGCaseActionsCreateClaimRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
                IsPrimaryOrSecondaryClaim = true,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                ClaimType = (int)ipg_claim_type.CMS1500,
                Icn = "",
                Box32 = "",
                Reason = ""
            };

            var output = new ipg_IPGCaseActionsCreateClaimResponse();


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
                OutputParameters = output.Results,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeExecutOrganizationRequest());

            //ACT
            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);

            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            Invoice GenerateClaim = (from inv in context.InvoiceSet
                                     where inv.ipg_caseid.Id == incident.Id
                                     select inv).FirstOrDefault();

            Annotation note = (from an in context.AnnotationSet
                                                 join doc in context.ipg_documentSet on an.ObjectId.Id equals doc.Id
                                                 where doc.ipg_DocumentTypeId.Id == doctypeCMS1500.Id
                                                 select an).FirstOrDefault();

            Assert.NotNull(GenerateClaim);
            Assert.NotNull(note);

            byte[] bytes = Convert.FromBase64String(note.DocumentBody);

            System.IO.FileStream stream =
            new FileStream(sourcepath + "CMS1500CreatedFromTest.pdf", FileMode.Create);
            System.IO.BinaryWriter writer =
                new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
        }


        [Fact]
        public void ClaimUB04GenerationWithMultiplePages()
        {
            ipg_zipcode zip = new ipg_zipcode() { Id = Guid.NewGuid() };
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.Claim_Generation_Errors);
            ipg_carrierclaimsmailingaddress claimmalingaddress = new ipg_carrierclaimsmailingaddress().Fake(zip);
            ipg_globalsetting cmssetting = new ipg_globalsetting().Fake().WithName("CMS1500_PDF_TEMPLATE");
            ipg_globalsetting ub04setting = new ipg_globalsetting().Fake().WithName("UB04_PDF_TEMPLATE");
            ipg_melissazipcode melisazipcode = new ipg_melissazipcode().Fake();

            ipg_globalsetting taskCMS1500Settings = new ipg_globalsetting().Fake("Task.InstitutionalClaimsReadyToPrint", "Task.InstitutionalClaimsReadyToPrint");
            ipg_globalsetting taskUB04Settings = new ipg_globalsetting().Fake("Task.ProfessionalClaimsReadyToPrint", "Task.ProfessionalClaimsReadyToPrint");

            ipg_tasktype tasktypeProfessionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.ProfessionalClaimsReadyToPrint");
            ipg_tasktype tasktypeInstitutionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.InstitutionalClaimsReadyToPrint");

            Annotation cmsNote = new Annotation().Fake().WithObjectReference(cmssetting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(cmspdfTemplatePath)));
            Annotation ub04Note = new Annotation().Fake().WithObjectReference(ub04setting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(ub04pdftemplatepath)));


            ipg_documenttype doctypeCMS1500 = new ipg_documenttype().Fake("Claim CMS1500");
            ipg_documenttype doctypeUB04 = new ipg_documenttype().Fake("Claim UB04");

            Contact patient = new Contact().Fake();
            ipg_claimstatuscode newstatuscode = new ipg_claimstatuscode().FakeNewClaimStatusCode();
            ipg_claimstatuscode closedcliamstatuscode = new ipg_claimstatuscode().FakeClosedClaimStatusCode();
            ipg_state facilityState = new ipg_state().Fake();
            ipg_ipg_carriernetwork_ipg_state carriernetworkstate = new ipg_ipg_carriernetwork_ipg_state().Fake().WithState(facilityState);
            Intake.Account mfg = new Intake.Account().Fake(Account_CustomerTypeCode.Manufacturer);
            Intake.Account carrier = new Intake.Account().Fake().FakeCarrierForClaimGeneration(true, ipg_claim_type.UB04);
            Intake.Account facility = new Intake.Account().Fake().FakeFacilityForClaimGeneration(facilityState);
            Contact phisician = new Contact().FakeWithPhysicianDetails("33", "588");
            Incident incident = new Incident().Fake().WithPrimaryCarrierReference(carrier)
                .WithFacilityReference(facility).WithInsuredPatient(new_RelationtoInsured.Self)
                .WithPhysician(phisician).WithActualDos(DateTime.Now).WithPatientReference(patient)
                .WithPrimaryClaimAddress(claimmalingaddress)
                .WithPatientZipCode(melisazipcode)
                .WithSecondaryClaimAddress(claimmalingaddress);

            Intake.Product product = new Intake.Product().Fake().WithManufacturerReference(mfg);
            ipg_masterhcpcs hcpc = new ipg_masterhcpcs().Fake();

            List<ipg_casepartdetail> parts = new List<ipg_casepartdetail>();

            for (int i = 0; i < 24; i++)
            {
                parts.Add(new ipg_casepartdetail().Fake().WithCaseReference(incident).WithHCPCS(hcpc).WithManufacturerReference(mfg).WithProductReference(product).WithBillableInfo(1,i,i));
            }

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize((new List<Entity> { cmssetting, ub04setting, cmsNote, ub04Note
                , incident, carrier,facilityState, carriernetworkstate, facility, phisician, mfg
                , newstatuscode, closedcliamstatuscode
                , product, hcpc
                , doctypeCMS1500, doctypeUB04
                , taskCMS1500Settings, taskUB04Settings, tasktypeProfessionalClaimsReadyToPrint, tasktypeInstitutionalClaimsReadyToPrint
                , patient
                , claimmalingaddress
                , tasktype
                , zip
                , melisazipcode
            }).Union(parts));

            var request = new ipg_IPGCaseActionsCreateClaimRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
                IsPrimaryOrSecondaryClaim = true,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                ClaimType = (int)ipg_claim_type.CMS1500,
                Icn = "",
                Box32 = "",
                Reason = ""
            };

            var output = new ipg_IPGCaseActionsCreateClaimResponse();


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
                OutputParameters = output.Results,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeExecutOrganizationRequest());

            //ACT
            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);

            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            Invoice GenerateClaim = (from inv in context.InvoiceSet
                                     where inv.ipg_caseid.Id == incident.Id
                                     select inv).FirstOrDefault();

            Annotation note = (from an in context.AnnotationSet
                               join doc in context.ipg_documentSet on an.ObjectId.Id equals doc.Id
                               where doc.ipg_DocumentTypeId.Id == doctypeUB04.Id
                               select an).FirstOrDefault();

            Assert.NotNull(GenerateClaim);
            Assert.NotNull(note);

            byte[] bytes = Convert.FromBase64String(note.DocumentBody);

            System.IO.FileStream stream =
            new FileStream(sourcepath + "UB04CreatedFromTest.pdf", FileMode.Create);
            System.IO.BinaryWriter writer =
                new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
        }


        [Fact]
        public void ClaimAutoCarriernerationWithMultiplePages()
        {
            ipg_zipcode zip = new ipg_zipcode() { Id = Guid.NewGuid()};
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.Claim_Generation_Errors);
            ipg_carrierclaimsmailingaddress claimmalingaddress = new ipg_carrierclaimsmailingaddress().Fake(zip);
            ipg_globalsetting cmssetting = new ipg_globalsetting().Fake().WithName("CMS1500_PDF_TEMPLATE");
            ipg_globalsetting ub04setting = new ipg_globalsetting().Fake().WithName("UB04_PDF_TEMPLATE");
            ipg_melissazipcode melisazipcode = new ipg_melissazipcode().Fake();

            ipg_globalsetting taskCMS1500Settings = new ipg_globalsetting().Fake("Task.InstitutionalClaimsReadyToPrint", "Task.InstitutionalClaimsReadyToPrint");
            ipg_globalsetting taskUB04Settings = new ipg_globalsetting().Fake("Task.ProfessionalClaimsReadyToPrint", "Task.ProfessionalClaimsReadyToPrint");

            ipg_tasktype tasktypeProfessionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.ProfessionalClaimsReadyToPrint");
            ipg_tasktype tasktypeInstitutionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.InstitutionalClaimsReadyToPrint");

            Annotation cmsNote = new Annotation().Fake().WithObjectReference(cmssetting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(cmspdfTemplatePath)));
            Annotation ub04Note = new Annotation().Fake().WithObjectReference(ub04setting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(ub04pdftemplatepath)));


            ipg_documenttype doctypeCMS1500 = new ipg_documenttype().Fake("Claim CMS1500");
            ipg_documenttype doctypeUB04 = new ipg_documenttype().Fake("Claim UB04");
            Contact patient = new Contact().Fake();
            ipg_claimstatuscode newstatuscode = new ipg_claimstatuscode().FakeNewClaimStatusCode();
            ipg_claimstatuscode closedcliamstatuscode = new ipg_claimstatuscode().FakeClosedClaimStatusCode();
            ipg_state facilityState = new ipg_state().Fake();
            ipg_ipg_carriernetwork_ipg_state carriernetworkstate = new ipg_ipg_carriernetwork_ipg_state().Fake().WithState(facilityState);
            Intake.Account mfg = new Intake.Account().Fake(Account_CustomerTypeCode.Manufacturer);
            Intake.Account carrier = new Intake.Account().Fake().FakeCarrierForClaimGeneration(true, ipg_claim_type.CMS1500);
            Intake.Account secCarrier = new Intake.Account().Fake().FakeCarrierForClaimGeneration(true, ipg_claim_type.CMS1500, ipg_CarrierType.Auto);
            Intake.Account facility = new Intake.Account().Fake().FakeFacilityForClaimGeneration(facilityState);
            Contact phisician = new Contact().FakeWithPhysicianDetails("33", "588");
            Incident incident = new Incident().Fake().WithPrimaryCarrierReference(carrier).WithFacilityReference(facility)
                .WithInsuredPatient(new_RelationtoInsured.Self).WithPhysician(phisician).WithActualDos(DateTime.Now).WithPatientReference(patient)
                .WithSecondaryCarrierReference(secCarrier)
                .WithPrimaryClaimAddress(claimmalingaddress)
                .WithPatientZipCode(melisazipcode)
                .WithSecondaryClaimAddress(claimmalingaddress);

            Intake.Product product = new Intake.Product().Fake().WithManufacturerReference(mfg);
            ipg_masterhcpcs hcpc = new ipg_masterhcpcs().Fake();

            List<ipg_casepartdetail> parts = new List<ipg_casepartdetail>();

            for (int i = 0; i <= 8; i++)
            {
                parts.Add(new ipg_casepartdetail().Fake().WithCaseReference(incident).WithHCPCS(hcpc).WithManufacturerReference(mfg).WithProductReference(product).WithBillableInfo(1, i, i, 60));
            }

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize((new List<Entity> { cmssetting, ub04setting, cmsNote, ub04Note
                , incident, carrier,facilityState, carriernetworkstate, facility, phisician, mfg, secCarrier
                , newstatuscode, closedcliamstatuscode
                , product, hcpc
                , doctypeCMS1500, doctypeUB04
                , taskCMS1500Settings, taskUB04Settings, tasktypeProfessionalClaimsReadyToPrint, tasktypeInstitutionalClaimsReadyToPrint
                , patient
                , claimmalingaddress
                , tasktype
                , zip
                , melisazipcode
            }).Union(parts));
            var request = new ipg_IPGCaseActionsCreateClaimRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
                IsPrimaryOrSecondaryClaim = true,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                ClaimType = (int)ipg_claim_type.CMS1500,
                Icn = "",
                Box32 = "",
                Reason = ""
            };

            var output = new ipg_IPGCaseActionsCreateClaimResponse();


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
                OutputParameters = output.Results,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeExecutOrganizationRequest());

            //ACT
            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);

            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            Invoice GenerateClaim = (from inv in context.InvoiceSet
                                     where inv.ipg_caseid.Id == incident.Id
                                     select inv).FirstOrDefault();

            Annotation note = (from an in context.AnnotationSet
                               join doc in context.ipg_documentSet on an.ObjectId.Id equals doc.Id
                               where doc.ipg_DocumentTypeId.Id == doctypeCMS1500.Id
                               select an).FirstOrDefault();

            Assert.NotNull(GenerateClaim);
            Assert.Equal(true, GenerateClaim.ipg_isprimaryorsecondaryclaim);
            Assert.Equal(GenerateClaim.CustomerId.Id, secCarrier.Id);
            Assert.NotNull(note);
        }



        [Fact]
        public void SkipGenerationIfPendingFacilityCarrierContract()
        {
            //ARRANGE

            var tasktype = new ipg_tasktype().Fake().WithTypeId(TaskManager.TaskTypeIds.Claim_Generation_Errors);
            var carrier = new Intake.Account().Fake().FakeCarrierForClaimGeneration(true, ipg_claim_type.CMS1500).Generate();
            var facility = new Intake.Account().Fake().Generate();
            var incident = new Incident().Fake()
                .WithPrimaryCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithActualDos(DateTime.Now)
                .Generate();

            var facilityCarrierEntitlement = new Entitlement().Fake()
                .WithEntitlementType(new OptionSetValue((int)ipg_EntitlementTypes.FacilityCarrier))
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10))
                .WithContractStatus(new OptionSetValue((int)Entitlement_ipg_contract_status.Pending))
                .Generate();

            var globalSetting = new ipg_globalsetting().Fake("TASK_REASON_ID_PENDING_FACILITY_CARRIER_CONTRACT", "98959d49-5530-ec11-b6e5-000d3a5b4345").Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                incident, carrier, facility, facilityCarrierEntitlement, globalSetting, tasktype
            });
            var request = new ipg_IPGCaseActionsCreateClaimRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
                IsPrimaryOrSecondaryClaim = true,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                ClaimType = (int)ipg_claim_type.CMS1500,
                Icn = "",
                Box32 = "",
                Reason = "",
                ManualClaim = false
            };

            var output = new ipg_IPGCaseActionsCreateClaimResponse();

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
                OutputParameters = output.Results,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.AddFakeMessageExecutor<OrganizationRequest>(new FakeExecutOrganizationRequest());

            //ACT

            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);

            //ASSERT

            Assert.True((bool)pluginContext.OutputParameters["SkipClaimGeneration"]);
            Assert.Equal(Guid.Parse(globalSetting.ipg_value), pluginContext.OutputParameters["SkipTaskReasonId"]);
        }

        [Fact]
        public void ClaimCMS1500GenerationWithGatingFail()
        {
            ipg_zipcode zip = new ipg_zipcode() { Id = Guid.NewGuid() };
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.Claim_Generation_Errors);
            ipg_carrierclaimsmailingaddress claimmalingaddress = new ipg_carrierclaimsmailingaddress().Fake(zip);
            ipg_globalsetting cmssetting = new ipg_globalsetting().Fake().WithName("CMS1500_PDF_TEMPLATE");
            ipg_globalsetting ub04setting = new ipg_globalsetting().Fake().WithName("UB04_PDF_TEMPLATE");
            ipg_melissazipcode melisazipcode = new ipg_melissazipcode().Fake();

            ipg_globalsetting taskCMS1500Settings = new ipg_globalsetting().Fake("Task.InstitutionalClaimsReadyToPrint", "Task.InstitutionalClaimsReadyToPrint");
            ipg_globalsetting taskUB04Settings = new ipg_globalsetting().Fake("Task.ProfessionalClaimsReadyToPrint", "Task.ProfessionalClaimsReadyToPrint");

            ipg_tasktype tasktypeProfessionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.ProfessionalClaimsReadyToPrint");
            ipg_tasktype tasktypeInstitutionalClaimsReadyToPrint = new ipg_tasktype().Fake().WithName("Task.InstitutionalClaimsReadyToPrint");

            Annotation cmsNote = new Annotation().Fake().WithObjectReference(cmssetting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(cmspdfTemplatePath)));
            Annotation ub04Note = new Annotation().Fake().WithObjectReference(ub04setting).WithDocument(cmspdfTemplatePath, Convert.ToBase64String(File.ReadAllBytes(ub04pdftemplatepath)));


            ipg_documenttype doctypeCMS1500 = new ipg_documenttype().Fake("Claim CMS1500");
            ipg_documenttype doctypeUB04 = new ipg_documenttype().Fake("Claim UB04");
            Contact patient = new Contact().Fake();
            ipg_claimstatuscode newstatuscode = new ipg_claimstatuscode().FakeNewClaimStatusCode();
            ipg_claimstatuscode closedcliamstatuscode = new ipg_claimstatuscode().FakeClosedClaimStatusCode();
            ipg_state facilityState = new ipg_state().Fake();
            ipg_ipg_carriernetwork_ipg_state carriernetworkstate = new ipg_ipg_carriernetwork_ipg_state().Fake().WithState(facilityState);
            Intake.Account mfg = new Intake.Account().Fake(Account_CustomerTypeCode.Manufacturer);
            Intake.Account carrier = new Intake.Account().Fake().FakeCarrierForClaimGeneration(true, ipg_claim_type.CMS1500);
            Intake.Account facility = new Intake.Account().Fake().FakeFacilityForClaimGeneration(facilityState);
            Contact phisician = new Contact().FakeWithPhysicianDetails("33", "588");
            Incident incident = new Incident().Fake().WithPrimaryCarrierReference(carrier).WithFacilityReference(facility)
                .WithInsuredPatient(new_RelationtoInsured.Self).WithPhysician(phisician).WithActualDos(DateTime.Now).WithPatientReference(patient)
                .WithPrimaryClaimAddress(claimmalingaddress)
                .WithPatientZipCode(melisazipcode)
                .WithSecondaryClaimAddress(claimmalingaddress);

            Intake.Product product = new Intake.Product().Fake().WithManufacturerReference(mfg);
            ipg_masterhcpcs hcpc = new ipg_masterhcpcs().Fake();

            List<ipg_casepartdetail> parts = new List<ipg_casepartdetail>();

            for (int i = 0; i <= 8; i++)
            {
                parts.Add(new ipg_casepartdetail().Fake().WithCaseReference(incident).WithHCPCS(hcpc).WithManufacturerReference(mfg).WithProductReference(product).WithBillableInfo(1, i, i));
            }

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize((new List<Entity> { cmssetting, ub04setting, cmsNote, ub04Note
                , incident, carrier,facilityState, carriernetworkstate, facility, phisician, mfg
                , newstatuscode, closedcliamstatuscode
                , product, hcpc
                , doctypeCMS1500, doctypeUB04
                , taskCMS1500Settings, taskUB04Settings, tasktypeProfessionalClaimsReadyToPrint, tasktypeInstitutionalClaimsReadyToPrint
                , patient
                , claimmalingaddress
                , tasktype
                , zip
                , melisazipcode
            }).Union(parts));

            var request = new ipg_IPGCaseActionsCreateClaimRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
                IsPrimaryOrSecondaryClaim = true,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                ClaimType = (int)ipg_claim_type.CMS1500,
                Icn = "",
                Box32 = "",
                Reason = ""
            };

            var output = new ipg_IPGCaseActionsCreateClaimResponse();


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
                OutputParameters = output.Results,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.AddExecutionMock<ipg_IPGGatingStartGateProcessingRequest>((req) => { return new ipg_IPGGatingStartGateProcessingResponse() { Succeeded = false, Output = "Gate Configuration field is empty. Please contact administrator to resolve this issue" }; });

            //ACT
            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);

            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            Invoice GenerateClaim = (from inv in context.InvoiceSet
                                     where inv.ipg_caseid.Id == incident.Id
                                     select inv).FirstOrDefault();

            Annotation note = (from an in context.AnnotationSet
                               join doc in context.ipg_documentSet on an.ObjectId.Id equals doc.Id
                               where doc.ipg_DocumentTypeId.Id == doctypeCMS1500.Id
                               select an).FirstOrDefault();

            Assert.NotNull(GenerateClaim);
            Assert.NotNull(note);
            Assert.True(output.HasErrors);
            Assert.Contains("Gate Configuration field is empty. Please contact administrator to resolve this issue", output.Message);
        }

        [Fact(Skip = "Relevant for integation tests only")]
        //[Fact]
        public void ClaimGenerationOnRealDataTest()
        {
            //Use to catch specific error

            var fakedContext = new XrmRealContext();
            var caseRef = new EntityReference(Incident.EntityLogicalName, new Guid("{620236b8-87b9-ec11-983f-000d3a5b6a73}"));
            var service = fakedContext.GetOrganizationService();

            var claimRequest = new ipg_IPGCaseActionsCreateClaimRequest()
            {
                Target = caseRef,
                IsPrimaryOrSecondaryClaim = false,
                GenerateClaimFlag = true,
                GeneratePdfFlag = true,
                IsReplacementClaim = false,
                Icn = "",
                Box32 = "",
                Reason = "",
                ManualClaim = true
            };

            var outputParams = new ParameterCollection();


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = claimRequest.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = claimRequest.Parameters,
                OutputParameters = outputParams,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<GenerateClaimPlugin>(pluginContext);
        }


        public class FakeExecutOrganizationRequest : IFakeMessageExecutor
        {
            public bool isExecuted = false;
            public bool CanExecute(OrganizationRequest request)
            {
                return  true;
            }

            public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
            {
                isExecuted = true;
                return new OrganizationResponse();
            }

            public Type GetResponsibleRequestType()
            {
                return typeof(ExecuteWorkflowRequest);
            }
        }

        private List<ipg_dxcode> GetDxCodesForCase(IOrganizationService _crmService, List<Guid> dxCodeIds)
        {
            var query = new QueryExpression(ipg_dxcode.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_dxcode.Fields.ipg_ICDVersion, ipg_dxcode.PrimaryNameAttribute),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_dxcode.PrimaryIdAttribute, ConditionOperator.In, dxCodeIds.ToArray())
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities.Cast<ipg_dxcode>().ToList();
        }
    }
}   