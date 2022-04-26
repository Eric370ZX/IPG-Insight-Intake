using FakeXrmEasy;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class PotentialDuplicatePatientTest : PluginTestsBase
    {
        const string TASK_NAME = "Potential Duplicate Patient";

        [Fact]
        public void TaskCreatedOnDuplicatePatientTest()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_contacttype contacttype = new ipg_contacttype().Fake("Patient");
            
            Contact fakeContact = new Contact().Fake((int)Contact_CustomerTypeCode.Patient);
            fakeContact.FirstName = "dylan";
            fakeContact.LastName = "cristall";
            fakeContact.BirthDate = new DateTime(2021, 1, 1);
            fakeContact.ipg_ContactTypeId = contacttype.ToEntityReference();
            fakeContact.ipg_ContactTypeId.Name = "Patient";

            Contact fakeContact2 = new Contact().Fake((int)Contact_CustomerTypeCode.Patient);
            fakeContact2.FirstName = "dilan";
            fakeContact2.LastName = "crystal";
            fakeContact2.BirthDate = new DateTime(2021, 1, 1);
            fakeContact2.ipg_ContactTypeId = contacttype.ToEntityReference();
            fakeContact2.ipg_ContactTypeId.Name = "Patient";

            OrganizationServiceMock.WithRetrieveCrud(fakeContact);

            OrganizationServiceMock.WithRetrieveCrud(contacttype);

            OrganizationServiceMock.WithRetrieveMultipleCrud(Contact.EntityLogicalName,
                new List<Entity>() { fakeContact2 });

            OrganizationServiceMock.WithRetrieveMultipleCrud(Task.EntityLogicalName,
              new List<Entity>() { new Task().Fake().WithDuplicatePatient(new Contact().Fake((int)Contact_CustomerTypeCode.Patient)) });

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_tasktype.EntityLogicalName,
                    new List<Entity>() { new ipg_tasktype().Fake().WithName("Potential Duplicate Patient") });

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context

            var inputParameters = new ParameterCollection
            {
                {"Target", fakeContact}
            };

            PluginExecutionContextMock.Setup(c => c.InputParameters).Returns(inputParameters);
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Update);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(Contact.EntityLogicalName);

            List<Task> tasksCreated = new List<Task>();

            OrganizationServiceMock.Setup(t =>
                t.Create(It.Is<Entity>(e => e.LogicalName.ToUpper() == Task.EntityLogicalName.ToUpper()))) //only match an entity with a logical name of "task"
                .Returns(Guid.NewGuid()) //can return any guid here
                .Callback<Task>(s => tasksCreated.Add(s)); //store the Create method invocation parameter for inspection later

            #endregion

            #region Execute Plugin

            var plugin = new PotentialDuplicatePatient();

            plugin.Execute(ServiceProvider);

            #endregion

            #region Assert

            Assert.All(tasksCreated, t => Assert.Equal(t.RegardingObjectId.Id, fakeContact.Id));
            #endregion
        }
    }
}
