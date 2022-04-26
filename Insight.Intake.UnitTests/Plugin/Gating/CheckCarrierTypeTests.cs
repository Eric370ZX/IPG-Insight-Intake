using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
	public class CheckCarrierTypeTests : PluginTestsBase
	{
		[Fact]
		public void CheckCarrierTypeTests_CarrierOfTypeWorkersComp()
		{
			var fakedContext = new XrmFakedContext();

			Intake.Account carrier = new Intake.Account().Fake();
			carrier.ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.WorkersComp);

			Incident incident = new Incident().Fake()
				.RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());

			var listForInit = new List<Entity>() { incident, carrier};
			fakedContext.Initialize(listForInit);

			var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
			var outputParameters = new ParameterCollection { { "Succeeded", false } };

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = "ipg_IPGGatingCheckCarrierType",
				Stage = 40,
				PrimaryEntityName = null,
				InputParameters = inputParameters,
				OutputParameters = outputParameters,
				PostEntityImages = null,
				PreEntityImages = null
			};
			//ACT
			fakedContext.ExecutePluginWith<CheckCarrierType>(pluginContext);

			//Assert
			Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
			Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);

		}

		[Fact]
		public void CheckCarrierTypeTests_CarrierOfTypeNonWorkersComp()
		{
			var fakedContext = new XrmFakedContext();

			Intake.Account carrier = new Intake.Account().Fake();
			carrier.ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Auto);

			Incident incident = new Incident().Fake()
				.RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());

			var listForInit = new List<Entity>() { incident, carrier };
			fakedContext.Initialize(listForInit);

			var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
			var outputParameters = new ParameterCollection { { "Succeeded", false } };

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = "ipg_IPGGatingCheckCarrierType",
				Stage = 40,
				PrimaryEntityName = null,
				InputParameters = inputParameters,
				OutputParameters = outputParameters,
				PostEntityImages = null,
				PreEntityImages = null
			};
			//ACT
			fakedContext.ExecutePluginWith<CheckCarrierType>(pluginContext);

			//Assert
			Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
			Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);

		}
	}
}
