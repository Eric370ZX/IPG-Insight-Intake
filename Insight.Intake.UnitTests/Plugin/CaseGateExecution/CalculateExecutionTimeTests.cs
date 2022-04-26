using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.CaseGateExecution;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.CaseGateExecution
{
    public class CalculatePreExecutionTimeTests : PluginTestsBase
    {
        [Fact]
        public void CheckPreExecutionTime()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            var now = DateTime.Now;
            int delta = 5;
            ipg_casegateexecution cge = new ipg_casegateexecution()
            {
                Id = Guid.NewGuid(),
                ipg_EndDatetimePreExecution = now
            };
            ipg_casegateexecution preImage = new ipg_casegateexecution()
            {
                Id = cge.Id,
                ipg_StartDatetimePreExecution = now.AddMilliseconds(-delta)
            };

            var listForInit = new List<Entity> { cge, preImage };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_casegateexecution.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", cge } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<CalculateExecutionTime>(pluginContext);
            #endregion

            #region Asserts
            Assert.Equal(cge.ipg_PreGateExecution, delta);
            #endregion
        }

        [Fact]
        public void CheckPostExecutionTime()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            var now = DateTime.Now;
            int delta = 5;
            ipg_casegateexecution cge = new ipg_casegateexecution()
            {
                Id = Guid.NewGuid(),
                ipg_EndDatetimePostExecution = now
            };
            ipg_casegateexecution preImage = new ipg_casegateexecution()
            {
                Id = cge.Id,
                ipg_StartDatetimePostExecution = now.AddMilliseconds(-delta)
            };

            var listForInit = new List<Entity> { cge, preImage };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_casegateexecution.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", cge } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<CalculateExecutionTime>(pluginContext);
            #endregion

            #region Asserts
            Assert.Equal(cge.ipg_PostGateExecution, delta);
            #endregion
        }

        [Fact]
        public void CheckWTExecutionTime()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            var now = DateTime.Now;
            int delta = 5;
            ipg_casegateexecution cge = new ipg_casegateexecution()
            {
                Id = Guid.NewGuid(),
                ipg_EndDatetimeWTExecution = now
            };
            ipg_casegateexecution preImage = new ipg_casegateexecution()
            {
                Id = cge.Id,
                ipg_StartDatetimeWTExecution = now.AddMilliseconds(-delta)
            };

            var listForInit = new List<Entity> { cge, preImage };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_casegateexecution.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", cge } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<CalculateExecutionTime>(pluginContext);
            #endregion

            #region Asserts
            Assert.Equal(cge.ipg_WTExecution, delta);
            #endregion
        }

        [Fact]
        public void CheckGateExecutionTime()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            var now = DateTime.Now;
            int delta = 5;
            ipg_casegateexecution cge = new ipg_casegateexecution()
            {
                Id = Guid.NewGuid(),
                ipg_EndDatetime = now
            };
            ipg_casegateexecution preImage = new ipg_casegateexecution()
            {
                Id = cge.Id,
                ipg_StartDatetime = now.AddMilliseconds(-delta)
            };

            var listForInit = new List<Entity> { cge, preImage };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_casegateexecution.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", cge } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<CalculateExecutionTime>(pluginContext);
            #endregion

            #region Asserts
            Assert.Equal(cge.ipg_GateExecution, delta);
            #endregion
        }
    }
}
