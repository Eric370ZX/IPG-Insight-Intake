using FakeXrmEasy;
using Microsoft.Xrm.Client;
using Insight.Intake.Plugin.Product;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;
using Insight.Intake.Plugin.InformationTypeRequiredInformationRule;

namespace Insight.Intake.UnitTests.Plugin.Product
{
    public class InformationTypeRequiredInformationRuleValidateDuplicateTests : PluginTestsBase
    {
        [Fact]
        public void CheckWhenDuplicationExists()
        {
            var fakedContext = new XrmFakedContext();
            ipg_documenttype ipg_Documenttype = new ipg_documenttype().Fake();
            ipg_informationtyperequiredinformationrule rule = new ipg_informationtyperequiredinformationrule().WithDocumentType(ipg_Documenttype);

            var duplicate = rule.Clone().ToEntity<ipg_informationtyperequiredinformationrule>();
            duplicate.ipg_DocumentTypeId = ipg_Documenttype.ToEntityReference();
            duplicate.Id = Guid.NewGuid();

            fakedContext.Initialize(new List<Entity> { rule, duplicate });

            var inputParameters = new ParameterCollection { { "Target", rule } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_informationtyperequiredinformationrule.EntityLogicalName,
                InputParameters = inputParameters,
            };

            var error = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<InformationTypeRequiredInformationRuleValidateDuplicate>(pluginContext));
            Assert.Contains($"Rule with new \"{rule.ipg_name}\" already exist for \"{duplicate.ipg_DocumentTypeId.Name}\" document type.", error.Message);
        }

    }
}
