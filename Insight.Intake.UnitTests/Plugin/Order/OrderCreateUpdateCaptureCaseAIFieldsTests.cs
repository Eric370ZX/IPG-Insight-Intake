using FakeXrmEasy;
using Insight.Intake.Plugin.Order;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Order
{
    public class OrderCreateUpdateCaptureCaseAIFieldsTests
    {
        [Fact]
        public void CreateOrder_AllFieldsOnCase_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Intake.Account facilityEntity = new Intake.Account().Fake();

            Incident caseEntity = new Incident().Fake().WithFacilityReference(facilityEntity);
            //"ipg_facilityid"
            caseEntity.Title = "123456789";
            caseEntity.ipg_FacilityMRN = "12";
            //caseEntity.ipg_PatientFullName = "Jane Doe";
            caseEntity.ipg_ActualDOS = new DateTime(2020,12,12);
            caseEntity.ipg_SurgeryDate = new DateTime(2020,11,12);
            SalesOrder salesOrder = new SalesOrder().Fake();
            salesOrder.ipg_CaseId = caseEntity.ToEntityReference();            
            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", salesOrder } };
          

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = inputParameters,              
                PostEntityImages = null,
                PreEntityImages = null                
            };
            //ACT
            fakedContext.ExecutePluginWith<OrderCreateUpdateCaptureCaseAIFields>(pluginContext);

            //Assert
            Assert.True(caseEntity.Title == salesOrder.ipg_caseticketnumber);
            Assert.True(caseEntity.ipg_FacilityMRN == salesOrder.ipg_casefacilitymrn);
            Assert.True(caseEntity.ipg_ActualDOS == salesOrder.ipg_casesurgerydate);           
            Assert.True(caseEntity.ipg_FacilityId.Name == salesOrder.ipg_casefacilityname);
        }
        [Fact]
        public void CreateOrder_NoActulaDOSOnCase_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Intake.Account facilityEntity = new Intake.Account().Fake();


            Incident caseEntity = new Incident().Fake().WithFacilityReference(facilityEntity);
            //"ipg_facilityid"
            caseEntity.Title = "123456789";
            caseEntity.ipg_FacilityMRN = "12";
            //caseEntity.ipg_PatientFullName = "Jane Doe";            
            caseEntity.ipg_SurgeryDate = new DateTime(2020,11,12);
            SalesOrder salesOrder = new SalesOrder().Fake();
            salesOrder.ipg_CaseId = caseEntity.ToEntityReference();            
            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", salesOrder } };
          

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = inputParameters,              
                PostEntityImages = null,
                PreEntityImages = null                
            };
            //ACT
            fakedContext.ExecutePluginWith<OrderCreateUpdateCaptureCaseAIFields>(pluginContext);

            //Assert
            Assert.True(caseEntity.Title == salesOrder.ipg_caseticketnumber);
            Assert.True(caseEntity.ipg_FacilityMRN == salesOrder.ipg_casefacilitymrn);
            Assert.True(caseEntity.ipg_SurgeryDate == salesOrder.ipg_casesurgerydate);           
            Assert.True(caseEntity.ipg_FacilityId.Name == salesOrder.ipg_casefacilityname);
        }
    }
}
