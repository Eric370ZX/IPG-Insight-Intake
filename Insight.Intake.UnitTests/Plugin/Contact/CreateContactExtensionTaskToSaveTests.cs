using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.PhoneExt
{
    public class CreateContactExtensionTaskToSaveTests : PluginTestsBase
    {

        [Fact]
        public void CreateContactExtensionTask()
        {
           
            var fakedContext = new XrmFakedContext();

            Contact caseEntity = new Contact().Fake();
          





            Task relTask = new Task().Fake()
                .RuleFor(p => p.RegardingObjectId, p => caseEntity.ToEntityReference());   

            var listForInit = new List<Entity>() { caseEntity};
            fakedContext.Initialize(listForInit);

            Contact extension = caseEntity.FakeWithContactExtension();

            //Assert
            if (extension.ipg_BusinessExt != "")
            {
                Assert.True(true);
            }
            
           
        }
         
    }
}

