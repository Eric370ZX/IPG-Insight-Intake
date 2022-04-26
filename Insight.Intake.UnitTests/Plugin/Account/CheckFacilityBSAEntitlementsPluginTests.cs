using System;
using System.Linq;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Account;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Account
{
    public class CheckFacilityBSAEntitlementsPluginTests : PluginTestsBase
    {
        [Fact]
        public void Fails_when_no_target_input_parameter_provided()
        {
            //ARRANGE

            var inputParameters = new ParameterCollection
            {
                { CheckFacilityBSAEntitlementsPlugin.CheckOnDateInputParameterName, DateTime.Today}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;


            //ACT, ASSERT

            var plugin = new CheckFacilityBSAEntitlementsPlugin();
            Assert.ThrowsAny<Exception>(() => plugin.Execute(ServiceProvider));
        }

        [Fact]
        public void Fails_when_no_date_input_parameter_provided()
        {
            //ARRANGE

            var inputParameters = new ParameterCollection
            {
                {"Target", new EntityReference(Intake.Account.EntityLogicalName, Guid.NewGuid())}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;


            //ACT, ASSERT

            var plugin = new CheckFacilityBSAEntitlementsPlugin();
            Assert.ThrowsAny<Exception>(() => plugin.Execute(ServiceProvider));
        }

        [Fact]
        public void Returns_true_when_entitlements_exist()
        {
            //ARRANGE

            var inputTarget = new EntityReference(Intake.Account.EntityLogicalName, Guid.NewGuid());
            DateTime date = DateTime.Today;
            var inputParameters = new ParameterCollection
            {
                { CheckFacilityBSAEntitlementsPlugin.TargetInputParameterName, inputTarget},
                { CheckFacilityBSAEntitlementsPlugin.CheckOnDateInputParameterName, date}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(q => q.EntityName == Entitlement.EntityLogicalName
                    && q.Criteria.FilterOperator == LogicalOperator.And
                    && q.Criteria.Conditions.Count == 4
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(Entitlement.ipg_EntitlementType).ToLower() && c.Operator == ConditionOperator.Equal && c.Values.Count == 1 && (int)c.Values[0] == (int)ipg_EntitlementTypes.BSA)
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(Entitlement.CustomerId).ToLower() && c.Operator == ConditionOperator.Equal && c.Values.Count == 1 && (Guid)c.Values[0] == inputTarget.Id)
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(Entitlement.StartDate).ToLower() && c.Operator == ConditionOperator.LessEqual && c.Values.Count == 1 && (DateTime)c.Values[0] == date)
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(Entitlement.EndDate).ToLower() && c.Operator == ConditionOperator.GreaterEqual && c.Values.Count == 1 && (DateTime)c.Values[0] == date))))
                .Returns(new EntityCollection(new Entity[]{new Entitlement()}));

            var outputParameters = new ParameterCollection();
            PluginExecutionContextMock.Setup(pec => pec.OutputParameters).Returns(outputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            
            //ACT

            var plugin = new CheckFacilityBSAEntitlementsPlugin();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            Assert.True((bool)pluginExecutionContext.OutputParameters[CheckFacilityBSAEntitlementsPlugin.ResultOutputParameterName]);
        }

        [Fact]
        public void Returns_false_when_no_entitlements()
        {
            //ARRANGE

            var inputTarget = new EntityReference(Intake.Account.EntityLogicalName, Guid.NewGuid());
            DateTime date = DateTime.Today;
            var inputParameters = new ParameterCollection
            {
                { CheckFacilityBSAEntitlementsPlugin.TargetInputParameterName, inputTarget},
                { CheckFacilityBSAEntitlementsPlugin.CheckOnDateInputParameterName, date}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.IsAny<QueryExpression>()))
                .Returns(new EntityCollection());

            var outputParameters = new ParameterCollection();
            PluginExecutionContextMock.Setup(pec => pec.OutputParameters).Returns(outputParameters);

            ServiceProvider = ServiceProviderMock.Object;


            //ACT

            var plugin = new CheckFacilityBSAEntitlementsPlugin();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            Assert.False((bool)pluginExecutionContext.OutputParameters[CheckFacilityBSAEntitlementsPlugin.ResultOutputParameterName]);
        }
    }
}
