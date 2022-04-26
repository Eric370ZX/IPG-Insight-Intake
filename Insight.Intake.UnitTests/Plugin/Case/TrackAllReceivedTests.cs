using FakeXrmEasy;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Common;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class TrackAllReceivedTests
    {
        [Fact]
        public void CheckAllReceived()
        {
            var fakedContext = new XrmFakedContext();

            DateTime allReceivedDate = new DateTime(2020, 09, 09, 12, 00, 00);
            EntityReference systemUserRef = new EntityReference("systemuser", Guid.NewGuid());

            Incident target = new Incident()
                .Fake()
                    .WithIsAllReceived(true)
                .Generate();

            Incident incident = new Incident()
                .Fake(target.Id)
                    .WithIsAllReceived(true)
                .Generate();

            var fakedEntities = new List<Entity>() { target, incident };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                InitiatingUserId = systemUserRef.Id
            };

            fakedContext.ExecutePluginWith<TrackAllReceived>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            Assert.NotNull(target.ipg_allreceivedby);
            Assert.NotNull(target.ipg_isallreceiveddate);
            Assert.Equal(systemUserRef.Id, target.ipg_allreceivedby.Id);
        }

        [Fact]
        public void CheckAllReceivedExisting()
        {
            var fakedContext = new XrmFakedContext();

            DateTime allReceivedDate = new DateTime(2020, 09, 09, 12, 00, 00);
            EntityReference allReceivedByUser = new EntityReference("systemuser", Guid.NewGuid());
            EntityReference updateUser = new EntityReference("systemuser", Guid.NewGuid());

            Incident target = new Incident()
                .Fake()
                    .WithIsAllReceived(true)
                .Generate();

            Incident incident = new Incident()
                .Fake(target.Id)
                    .WithIsAllReceived(true)
                    .WithIsAllReceivedDate(allReceivedDate)
                    .WithIsAllReceivedBy(allReceivedByUser)
                .Generate();

            var fakedEntities = new List<Entity>() { target, incident };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                InitiatingUserId = allReceivedByUser.Id
            };

            fakedContext.ExecutePluginWith<TrackAllReceived>(pluginContext);

            incident = fakedContext.GetOrganizationService().Retrieve<Incident>(incident.Id, new ColumnSet(true));

            Assert.NotNull(incident.ipg_isallreceiveddate);
            Assert.Equal(incident.ipg_isallreceiveddate.Value, allReceivedDate);
            Assert.NotEqual(incident.ipg_allreceivedby.Id, updateUser.Id);

            Assert.Null(target.ipg_isallreceiveddate);
            Assert.Null(target.ipg_allreceivedby);
        }
    }
}
