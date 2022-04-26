using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case.GenerateEligibilityInquiryPlugin
{
    public class OpenCaseTests
    {
        [Fact]
        public void CheckCaseOpenedONOpenCaseAction()
        {
            var fakedContext = new XrmFakedContext();

            ipg_gateconfiguration gateconfig = new ipg_gateconfiguration().Fake("Gate 3");
            ipg_lifecyclestep lf = new ipg_lifecyclestep().Fake(gateconfig, "Authtorization");
            Incident inciddent = new Incident().Fake().WithLFStep(lf).WithCaseStatus((int)ipg_CaseStatus.Closed);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName("Carrier Collections");
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithName("Outgoing Call Required. Level 1").WithCategory(taskCategory);


            var listForInit = new List<Entity> {inciddent, tasktype, taskCategory };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", inciddent.ToEntityReference() }};

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseOpen",
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
            };

            fakedContext.ExecutePluginWith<OpenCaseAction>(pluginContext);

            using (CrmServiceContext context = new CrmServiceContext(fakedContext.GetOrganizationService()))
            {
                var updIncident = context.IncidentSet.FirstOrDefault();

                Assert.Equal(ipg_CaseStatus.Open, updIncident.ipg_CaseStatusEnum);
            }
        }
    }
}
