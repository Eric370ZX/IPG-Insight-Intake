using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class GeneratePOTests : PluginTestsBase
    {
        [Fact]
        public void CheckPurchaseOrdersAreGenerated_CaseHasParts_returnSuccess()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            Intake.Product product = new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved);
            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(product).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(new Intake.Product().Fake()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithManufacturerReference(new Intake.Account().Fake()).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(new Intake.Product().Fake()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithManufacturerReference(new Intake.Account().Fake()).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, product, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "CPA" },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new CrmServiceContext(fakedService);

            var purchaseOrders = (from order in crmContext.CreateQuery<SalesOrder>()
                                  where order.ipg_CaseId.Id == caseEntity.Id
                                  select order).ToList();

            Assert.True(purchaseOrders.Count == 1);
        }

        [Fact]
        public void TestCPAGeneration()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "CPA" },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };
            var outputParameters = new ParameterCollection { { "Success", false } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.True(ThereIsPortalComment(crmContext, caseEntity.Id), "CPA Doesn't Have Portal Comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "CPA have Email");
        }

        [Fact]
        public void TestCPAGeneration_DoNotCommunicate()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "CPA" },
                { "EstimatedPO", false },
                { "CommunicatePo", false }
            };
            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.False(ThereIsPortalComment(crmContext, caseEntity.Id), "CPA shouldn't have a portal comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "CPA have Email");
        }

        [Fact]
        public void TestZPOGeneration()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "ZPO" },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);


            Assert.True(pluginContext.OutputParameters.Contains("Success")
                && pluginContext.OutputParameters["Success"] is bool
                && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.True(ThereIsPortalComment(crmContext, caseEntity.Id), "ZPO Doesn't Have Portal Comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "ZPO have Email");
        }

        [Fact]
        public void TestZPOGeneration_DoNotCommunicate()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "ZPO" },
                { "EstimatedPO", false },
                { "CommunicatePo", false }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                && pluginContext.OutputParameters["Success"] is bool
                && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.False(ThereIsPortalComment(crmContext, caseEntity.Id), "ZPO shouldn't has a comment");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "ZPO have Email");
        }

        [Fact]
        public void TestTPOGeneration()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_estimatedcasepartdetail> incidentParts = new List<ipg_estimatedcasepartdetail>() {
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active)
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "TPO" },
                { "EstimatedPO", true },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.True(ThereIsPortalComment(crmContext, caseEntity.Id), "TPO Doesn't Have Portal Comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "TPO have Email");
        }

        [Fact]
        public void TestTPOGeneration_DoNotCommunicate()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_estimatedcasepartdetail> incidentParts = new List<ipg_estimatedcasepartdetail>() {
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active)
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "TPO" },
                { "EstimatedPO", true },
                { "CommunicatePo", false }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.False(ThereIsPortalComment(crmContext, caseEntity.Id), "TPO shouldn't has a portal comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "TPO have Email");
        }

        //Faked Xrm doesn't contains mock for sendEmailRequest
        [Fact]
        public void TestMPOWithEmailGeneration()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active)
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "MPO" },
                { "CommunicateTo", "ahorodyskyi@ipg.com" },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            //fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            //Assert.True(pluginContext.OutputParameters.Contains("Success")
            //             && pluginContext.OutputParameters["Success"] is bool
            //             && pluginContext.OutputParameters.Contains("poId"));

            //Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
            //            && pluginContext.OutputParameters["poId"] as EntityReference != null);

            //var fakedService = fakedContext.GetOrganizationService();
            //var crmContext = new OrganizationServiceContext(fakedService);

            //var purchaseOrder = GetSalesOrder(crmContext, caseEntity.Id);

            //Assert.True(purchaseOrder != null, "There is no PO!");

            //Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            //Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            //Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            //Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            //Assert.False(ThereIsPortalComment(crmContext, caseEntity.Id), "MPO Have Portal Comment!");

            //Assert.True(ThereIsEmail(crmContext, purchaseOrder.Id), "PO doesn't have Email");
        }

        [Fact]
        public void TestMPOWithoutEmailGeneration()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "MPO" },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]},
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.False(ThereIsPortalComment(crmContext, caseEntity.Id), "MPO Have Portal Comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "PO have Email");
        }

        [Fact]
        public void TestMPOWithoutEmailGeneration_DoNotCommunicate()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "MPO" },
                { "EstimatedPO", false },
                { "CommunicatePo", false }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]},
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.False(ThereIsPortalComment(crmContext, caseEntity.Id), "MPO shouldn't has a portal comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "PO have Email");
        }

        [Fact]
        public void Test_TPOGenerationWithoutApprovedCasePartDetails()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_estimatedcasepartdetail> incidentParts = new List<ipg_estimatedcasepartdetail>() {
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active)
            };

            var listForInit = new List<Entity> { manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "TPO" },
                { "EstimatedPO", true },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]},
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrders = (from order in crmContext.CreateQuery<SalesOrder>()
                                  where order.ipg_CaseId.Id == caseEntity.Id
                                  select order).ToList();

            Assert.True(purchaseOrders.Count == 1);

            var purchaseOrderLines = (from line in crmContext.CreateQuery<SalesOrderDetail>()
                                      select line).ToList();

            Assert.True(purchaseOrderLines.Count == 2);
        }

        [Fact]
        public void Test_MFGAccountNumberFilled()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake()
                .RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => true);

            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            Intake.ipg_facilitymanufacturerrelationship relationship = new Intake.ipg_facilitymanufacturerrelationship().Fake()
             .WithFacilityReference(facility)
             .WithManufacturerReference(manufacturer)
             .WithManufacturerAccountNumber("54647");

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_estimatedcasepartdetail> incidentParts = new List<ipg_estimatedcasepartdetail>() {
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active)
            };

            var listForInit = new List<Entity> { relationship, manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "TPO" },
                { "EstimatedPO", true },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]},
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrders = (from order in crmContext.CreateQuery<SalesOrder>()
                                  where order.ipg_CaseId.Id == caseEntity.Id
                                  select order).ToList();

            Assert.True(purchaseOrders.Count == 1);

            var purchaseOrderLines = (from line in crmContext.CreateQuery<SalesOrderDetail>()
                                      select line).ToList();

            Assert.True(purchaseOrderLines.Count == 2);

            Assert.True(purchaseOrders.First().ipg_accountnumber == relationship.ipg_ManufacturerAccountNumber, "Account Number not Copied");
        }

        [Fact]
        public void Test_PostOperationHandlerManualCommunicatePO()
        {
            const string OrderId = "AD70E4CB-B52D-44F6-BACA-F0E7C728F947";
            var fakedContext = new XrmFakedContext();

            SystemUser systemUser = new SystemUser().Fake();
            SystemUser caseManager = new SystemUser().Fake();
            caseManager.InternalEMailAddress = "test02@mail.com";

            Incident caseEntity = new Incident().Fake();
            caseEntity.EmailAddress = "test@test.com";
            caseEntity.ipg_CaseManagerId = caseManager.ToEntityReference();
            SalesOrder salesOrder = new SalesOrder()
                .Fake()
                .WithPoTypeCode(ipg_PurchaseOrderTypes.CPA)
                .WithCaseReference(caseEntity)
                .WithStatusCode(SalesOrder_StatusCode.CommtoFacility);
            salesOrder.Id = new Guid(OrderId);

            var listForInit = new List<Entity> { systemUser, caseManager, salesOrder, caseEntity };


            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "OrderId", OrderId }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_ManualPOCommunication",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);
            Assert.Equal(true, (bool)pluginContext.OutputParameters["Success"]);
        }

        [Fact]
        public void Test_PostOperationHandlerManualCommunicatePO_WithoutCaseId()
        {
            const string OrderId = "AD70E4CB-B52D-44F6-BACA-F0E7C728F947";
            var fakedContext = new XrmFakedContext();

            SystemUser systemUser = new SystemUser().Fake();
            SalesOrder salesOrder = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.TPO);
            salesOrder.Id = new Guid(OrderId);

            var listForInit = new List<Entity> { systemUser, salesOrder };


            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "OrderId", OrderId }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_ManualPOCommunication",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);
            Assert.Equal("Purchase Order has no related case.", pluginContext.OutputParameters["Message"].ToString());
        }

        private bool ThereIsPortalComment(OrganizationServiceContext crmContext, Guid regardingObjectId)
        {
            return (from pc in crmContext.CreateQuery<adx_portalcomment>()
                    where pc.RegardingObjectId.Id == regardingObjectId
                    select pc).FirstOrDefault() != null;
        }

        private bool ThereIsPOLines(OrganizationServiceContext crmContext, Guid regardingObjectId, int number)
        {
            var lines = (from line in crmContext.CreateQuery<SalesOrderDetail>()
                         where line.SalesOrderId.Id == regardingObjectId
                         select line).ToList();

            return lines.Count == number;
        }

        private SalesOrder GetCreatedSalesOrder(OrganizationServiceContext crmContext, Guid caseId, SalesOrder existedPO = null)
        {
            var pos = (from order in crmContext.CreateQuery<SalesOrder>()
                       where order.ipg_CaseId.Id == caseId
                       orderby order.CreatedBy descending
                       select order).ToList();

            if(existedPO != null)
            {
                pos = pos.Where(po => po.Id != existedPO.Id).ToList();
            }

            return pos.FirstOrDefault();
        }

        private bool ThereIsNote(OrganizationServiceContext crmContext, Guid regardingObjectId)
        {
            return (from note in crmContext.CreateQuery<Annotation>()
                    where note.ObjectId.Id == regardingObjectId
                    select note).FirstOrDefault() != null;
        }

        private bool ThereIsEmail(OrganizationServiceContext crmContext, Guid regardingObjectId)
        {
            return (from email in crmContext.CreateQuery<Email>()
                    where email.RegardingObjectId.Id == regardingObjectId
                    select email).FirstOrDefault() != null;
        }

        [Fact]
        public void TestTPOWithDifferentMFGGenerated()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer2 = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake()
                .WithManufacturerReference(manufacturer),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake())

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            List<ipg_estimatedcasepartdetail> incidentParts = new List<ipg_estimatedcasepartdetail>() {
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active),
                new ipg_estimatedcasepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.TPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active)
            };

            SalesOrder existedTPO = new SalesOrder().FakeEstimatedTPO().WithMfgRef(manufacturer2).WithCaseReference(caseEntity);

            var listForInit = new List<Entity> {
                existedTPO,
                manufacturer
                , facility
                , caseEntity
                , systemUser
                , manufacturer2
                , new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "TPO" },
                { "EstimatedPO", true },
                { "CommunicatePo", true }
            };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            var output2Parameters = new ParameterCollection { { "Success", false } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeOrderActionsAfterGeneration",
                Stage = 40,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection()
                {
                    {"Target",outputParameters["poId"] },
                    {"CommunicatePo", inputParameters["CommunicatePo"]}
                },
                OutputParameters = output2Parameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var purchaseOrder = GetCreatedSalesOrder(crmContext, caseEntity.Id, existedTPO);

            Assert.True(purchaseOrder != null, "There is no PO!");

            Assert.True(ThereIsPOLines(crmContext, purchaseOrder.Id, 2), "No PO lines or not enough!");

            Assert.True(ThereIsNote(crmContext, purchaseOrder.Id), "PO doesn't Have Note!");
            Assert.True(ThereIsNote(crmContext, caseEntity.Id), "Case doesn't Have Note!");

            Assert.True(ThereIsPortalComment(crmContext, caseEntity.Id), "TPO Doesn't Have Portal Comment!");

            Assert.False(ThereIsEmail(crmContext, purchaseOrder.Id), "TPO have Email");
        }


        [Fact]
        public void TestRevisedPOGeneration_OldVoided_NewRevised_Generated()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            ipg_document poImage = new ipg_document().Fake();
            SalesOrder order = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.CPA).WithCaseReference(caseEntity).WithDocumentImage(poImage);
            List<SalesOrderDetail> orderDetails = new List<SalesOrderDetail>()
            {
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products.First()),
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products[1]),
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products[2]),
            };

            List <ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.ZPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[2]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { poImage, order, manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);
            listForInit.AddRange(orderDetails);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", "CPA" },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };
            var outputParameters = new ParameterCollection { { "Success", false } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.AddFakeMessageExecutor<FulfillSalesOrderRequest>(new FakeMessageExecutor<FulfillSalesOrderRequest, FulfillSalesOrderResponse>(
            (req, ctx) => 
            {
                var service = ctx.GetOrganizationService();
                var orderClose = req.OrderClose as OrderClose;
                service.Update(new SalesOrder() { Id = orderClose.SalesOrderId.Id, StateCode = SalesOrderState.Fulfilled, StatusCode = orderClose.StatusCode });
                return new FulfillSalesOrderResponse();
            })); 

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var generatedPO = GetCreatedSalesOrder(crmContext, caseEntity.Id, order);
            var oldPO = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(true)).ToEntity<SalesOrder>();
            var oldPOImage = fakedService.Retrieve(poImage.LogicalName, poImage.Id, new ColumnSet(true)).ToEntity<ipg_document>();




            Assert.NotNull(generatedPO);
            Assert.Equal(oldPO.ToEntityReference(), generatedPO.ipg_previousorderid);
            Assert.Equal(SalesOrder_StatusCode.Voided, oldPO.StatusCodeEnum);
            Assert.Equal(ipg_documentState.Inactive, oldPOImage.StateCode);
        }

        [Fact]
        public void TestRevisedPOGeneration_OldVoided_NewPOGenerated()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved)
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            ipg_document poImage = new ipg_document().Fake();
            SalesOrder order = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.CPA).WithCaseReference(caseEntity).WithDocumentImage(poImage);
            List<SalesOrderDetail> orderDetails = new List<SalesOrderDetail>()
            {
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products.First()),
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products[1]),
            };

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { poImage, order, manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);
            listForInit.AddRange(orderDetails);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", ipg_PurchaseOrderTypes.MPO.ToString() },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };
            var outputParameters = new ParameterCollection { { "Success", false } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.AddFakeMessageExecutor<FulfillSalesOrderRequest>(new FakeMessageExecutor<FulfillSalesOrderRequest, FulfillSalesOrderResponse>(
            (req, ctx) =>
            {
                var service = ctx.GetOrganizationService();
                var orderClose = req.OrderClose as OrderClose;
                service.Update(new SalesOrder() { Id = orderClose.SalesOrderId.Id, StateCode = SalesOrderState.Fulfilled, StatusCode = orderClose.StatusCode });
                return new FulfillSalesOrderResponse();
            }));

            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var generatedPO = GetCreatedSalesOrder(crmContext, caseEntity.Id, order);
            var oldPO = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(true)).ToEntity<SalesOrder>();
            var oldPOImage = fakedService.Retrieve(poImage.LogicalName, poImage.Id, new ColumnSet(true)).ToEntity<ipg_document>();




            Assert.NotNull(generatedPO);
            Assert.Null(generatedPO.ipg_previousorderid);
            Assert.Equal(SalesOrder_StatusCode.Voided, oldPO.StatusCodeEnum);
            Assert.Equal(ipg_documentState.Inactive, oldPOImage.StateCode);
        }

        [Fact]
        public void TestRevisedPOGeneration_OldVoided_NewPOGenerated_RevisedPOGenerated()
        {
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account facility = new Insight.Intake.Account().Fake();
            Insight.Intake.Account manufacturer = new Insight.Intake.Account().Fake();
            Insight.Intake.SystemUser systemUser = new Insight.Intake.SystemUser().Fake();

            List<Intake.Product> products = new List<Intake.Product>()
            {
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved),
                 new Intake.Product().Fake().WithManufacturerReference(manufacturer).WithStatus(Product_ipg_status.Approved)
                 ,new Intake.Product().Fake().WithManufacturerReference(new Insight.Intake.Account().Fake()).WithStatus(Product_ipg_status.Approved)

            };

            Incident caseEntity = new Incident().Fake()
                .WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = DateTime.Now.AddDays(-2);

            ipg_document poImage = new ipg_document().Fake();
            SalesOrder order = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.CPA).WithCaseReference(caseEntity).WithDocumentImage(poImage);
            List<SalesOrderDetail> orderDetails = new List<SalesOrderDetail>()
            {
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products.First()),
                new SalesOrderDetail().Fake().WithOrder(order).WithProduct(products[1]),
            };

            List<ipg_casepartdetail> incidentParts = new List<ipg_casepartdetail>() {
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products.First()).WithPOType((int)ipg_PurchaseOrderTypes.MPO).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag(),
                new ipg_casepartdetail().Fake().WithCaseReference(caseEntity).WithProductReference(products[1]).WithPOType((int)ipg_PurchaseOrderTypes.CPA).WithStatusCode((int)ipg_casepartdetail_StatusCode.Active).WithOrderRef(order).WithIsChangedFlag()
            };

            var listForInit = new List<Entity> { poImage, order, manufacturer, facility, caseEntity, systemUser, new ipg_globalsetting().Fake().WithName("IPG Address").WithValue("ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta") };
            listForInit.AddRange(incidentParts);
            listForInit.AddRange(products);
            listForInit.AddRange(orderDetails);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "manufacturer", manufacturer.ToEntityReference() },
                { "POType", ipg_PurchaseOrderTypes.MPO.ToString() },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };
            var outputParameters = new ParameterCollection { { "Success", false } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            var inputParameters2 = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "POType", ipg_PurchaseOrderTypes.CPA.ToString() },
                { "EstimatedPO", false },
                { "CommunicatePo", true }
            };
            var outputParameters2 = new ParameterCollection { { "Success", false } };


            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeCaseActionsGeneratePO",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters2,
                OutputParameters = outputParameters2,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = systemUser.Id
            };

            fakedContext.AddFakeMessageExecutor<FulfillSalesOrderRequest>(new FakeMessageExecutor<FulfillSalesOrderRequest, FulfillSalesOrderResponse>(
            (req, ctx) =>
            {
                var service = ctx.GetOrganizationService();
                var orderClose = req.OrderClose as OrderClose;
                service.Update(new SalesOrder() { Id = orderClose.SalesOrderId.Id, StateCode = SalesOrderState.Fulfilled, StatusCode = orderClose.StatusCode });
                return new FulfillSalesOrderResponse();
            }));


            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext);
            foreach (var part in incidentParts)
            {
                part.ipg_IsChanged = true;
                fakedContext.GetFakedOrganizationService().Update(part);
            }
            fakedContext.ExecutePluginWith<GeneratePO>(pluginContext2);

            Assert.True(pluginContext.OutputParameters.Contains("Success")
                         && pluginContext.OutputParameters["Success"] is bool
                         && pluginContext.OutputParameters.Contains("poId"));

            Assert.True(pluginContext.OutputParameters["Success"] as bool? == true
                        && pluginContext.OutputParameters["poId"] as EntityReference != null);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new CrmServiceContext(fakedService);

            var generatedNewPO = crmContext.SalesOrderSet.Where(o => o.SalesOrderId != order.Id && o.ipg_previousorderid == null).FirstOrDefault();
            var oldPO = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(true)).ToEntity<SalesOrder>();
            var oldPOImage = fakedService.Retrieve(poImage.LogicalName, poImage.Id, new ColumnSet(true)).ToEntity<ipg_document>();
            var revisedPO = crmContext.SalesOrderSet.Where(o => o.SalesOrderId != order.Id && o.ipg_previousorderid != null).FirstOrDefault();



            Assert.NotNull(generatedNewPO);
            Assert.NotNull(revisedPO);
            Assert.Equal(SalesOrder_StatusCode.Voided, oldPO.StatusCodeEnum);
            Assert.Equal(ipg_documentState.Inactive, oldPOImage.StateCode);
        }
    }
}