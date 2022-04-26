using Insight.Intake.Plugin.Workflow;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Workflow
{
    public class ValidatePreCertRequirementsOnCaseTests: PluginTestsBase
    {
        [Fact]
        public void ValidateCPTPrecert()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account carrier = new Intake.Account().Fake();

            Incident caseEntity = new Incident().Fake()
                                      .WithActualDos(new DateTime(2018, 7, 22)) //Convert.ToDateTime("07/22/2018"))
                                      .FakeWithCptCode(cptCode)
                                      .WithPrimaryCarrierReference(carrier);
            OrganizationServiceMock.WithRetrieveCrud(caseEntity);

            ipg_cptcode fakeCptCode = new ipg_cptcode().Fake();
            var carrierprecertcpts = new ipg_carrierprecertcpt().Fake()
                                         .FakeWithRequirementType((int)ipg_CarrierPrecertCPTRequirementType.CPTLOMN)
                                         .FakeWithCptCode(cptCode)
                                         .Generate(1);

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_carrierprecertcpt.EntityLogicalName,
                                                             carrierprecertcpts.Cast<Entity>().ToList());

            OrganizationServiceMock.WithRetrieveMultipleCrud(Task.EntityLogicalName,
                                                             new EntityCollection());

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_carrierprecerthcpcs.EntityLogicalName,
                                                             new EntityCollection());

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsValidatePreCertRequirementsOnCaseRequest");

            var request = new ipg_IPGCaseActionsValidatePreCertRequirementsOnCaseRequest
            {
                Target = new EntityReference(caseEntity.LogicalName, caseEntity.Id)
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin


            IList<Task> tasksCreated = new List<Task>();
            OrganizationServiceMock.WithCreateCrud<Task>().Callback<Entity>(x => tasksCreated.Add(x.ToEntity<Task>()));

            var plugin = new ValidatePreCertRequirementsOnCase();
            plugin.Execute(ServiceProvider);

            Assert.Contains(tasksCreated, t => t.Category.Contains("CPTLOMN"));
            #endregion
        }

        [Fact]
        public void ValidateHighDollarPrecert()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Account carrier = new Intake.Account().Fake();

            Incident caseEntity = new Incident().Fake()
                                      .WithActualDos(new DateTime(2018, 7, 22))
                                      .FakeWithCptCode(cptCode)
                                      .WithPrimaryCarrierReference(carrier);
            OrganizationServiceMock.WithRetrieveCrud(caseEntity);

            ipg_cptcode fakeCptCode = new ipg_cptcode().Fake();
            var carrierprecertcpts = new ipg_carrierprecertcpt().Fake()
                                         .FakeWithRequirementType((int)ipg_CarrierPrecertCPTRequirementType.HighDollar)
                                         .FakeWithCptCode(cptCode)
                                         .Generate(1);

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_carrierprecertcpt.EntityLogicalName,
                                                             carrierprecertcpts.Cast<Entity>().ToList());

            OrganizationServiceMock.WithRetrieveMultipleCrud(Task.EntityLogicalName,
                                                             new EntityCollection());

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_carrierprecerthcpcs.EntityLogicalName,
                                                             new EntityCollection());

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsValidatePreCertRequirementsOnCaseRequest");

            var request = new ipg_IPGCaseActionsValidatePreCertRequirementsOnCaseRequest
            {
                Target = new EntityReference(caseEntity.LogicalName, caseEntity.Id)
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin


            IList<Task> tasksCreated = new List<Task>();
            OrganizationServiceMock.WithCreateCrud<Task>().Callback<Entity>(x => tasksCreated.Add(x.ToEntity<Task>()));

            var plugin = new ValidatePreCertRequirementsOnCase();
            plugin.Execute(ServiceProvider);

            Assert.Contains(tasksCreated, t => t.Category.Contains("HighDollar"));
            #endregion
        }
    }
}
