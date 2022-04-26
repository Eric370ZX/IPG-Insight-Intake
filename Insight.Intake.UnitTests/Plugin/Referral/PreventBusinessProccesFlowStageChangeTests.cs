using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class PreventBusinessProccesFlowStageChangeTests : PluginTestsBase
    {
        [Fact]
        public void Throw_exception_when_user_change_stage()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create a fake Referral BPF and PreImage
            ipg_ipgreferralbpfmainflow bpf = new ipg_ipgreferralbpfmainflow()
                .Fake(new EntityReference("activestage", Guid.NewGuid()));
            ipg_ipgreferralbpfmainflow preImage = new ipg_ipgreferralbpfmainflow()
                .Fake(new EntityReference("activestage", Guid.NewGuid()));

            var listForInit = new List<Entity> { bpf, preImage };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = ipg_ipgreferralbpfmainflow.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", bpf } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<PreventBusinessProccesFlowStageChange>(pluginContext));

            #endregion

            #region Asserts

            Assert.Equal("Error: You are not able to change stage.", ex.Message);

            #endregion
        }
    }
}
