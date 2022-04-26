using FakeXrmEasy;
using Insight.Intake.Plugin.Account;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Account
{
    public class SetCarrierTimelyFilingRulePluginTests : PluginTestsBase
    {
        [Fact]
        public void Set_timely_filling_rule_to_180_if_carrier_not_wc()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account()
                .Fake((int)Account_CustomerTypeCode.Carrier)
                .WithCarrierType(ipg_CarrierType.IPG);

            var listForInit = new List<Entity> { carrier };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection(),
            };
            fakedContext.ExecutePluginWith<SetCarrierTimelyFilingRulePlugin>(pluginContext);

            Assert.Equal(180, carrier.ipg_timelyfilingrule.Value);
        }

        [Fact]
        public void Set_timely_filling_rule_to_90_if_carrier_is_wc()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account()
                .Fake((int)Account_CustomerTypeCode.Carrier)
                .WithCarrierType(ipg_CarrierType.WorkersComp);

            var listForInit = new List<Entity> { carrier };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection(),
            };
            fakedContext.ExecutePluginWith<SetCarrierTimelyFilingRulePlugin>(pluginContext);


            Assert.Equal(90, carrier.ipg_timelyfilingrule.Value);
        }

        [Fact]
        public void Set_timely_filling_rule_to_90_if_carrier_name_is_BCBSAnthemGA()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account()
                .Fake((int)Account_CustomerTypeCode.Carrier)
                .WithName("BCBS-Anthem-GA")
                .WithCarrierType(ipg_CarrierType.WorkersComp);

            var listForInit = new List<Entity> { carrier };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection(),
            };
            fakedContext.ExecutePluginWith<SetCarrierTimelyFilingRulePlugin>(pluginContext);

            Assert.Equal(90, carrier.ipg_timelyfilingrule.Value);
        }

        [Fact]
        public void Set_timely_filling_rule_to_90_if_carrier_name_is_BCBSAnthemCA()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account()
                .Fake((int)Account_CustomerTypeCode.Carrier)
                .WithName("BCBS-Anthem-CA")
                .WithCarrierType(ipg_CarrierType.WorkersComp);

            var listForInit = new List<Entity> { carrier };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection(),
            };
            fakedContext.ExecutePluginWith<SetCarrierTimelyFilingRulePlugin>(pluginContext);

            Assert.Equal(90, carrier.ipg_timelyfilingrule.Value);
        }

        [Fact]
        public void Not_set_timely_filling_rule_if_account_is_not_carrier()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account()
                .Fake();

            var listForInit = new List<Entity> { carrier };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection(),
            };
            fakedContext.ExecutePluginWith<SetCarrierTimelyFilingRulePlugin>(pluginContext);

            Assert.Null(carrier.ipg_timelyfilingrule);
        }
    }
}
