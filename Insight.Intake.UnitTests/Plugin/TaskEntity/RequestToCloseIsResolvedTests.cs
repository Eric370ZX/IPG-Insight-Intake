﻿using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class RequestToCloseIsResolvedTests
    {
        [Fact]
        public void RequestToCloseIsResolvedTests_EmptyTaskType()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = Guid.NewGuid();
            caseEntity.ipg_casehold = true;

            Task task1Entity = new Task().Fake();
            task1Entity.Id = Guid.NewGuid();
            task1Entity.RegardingObjectId = caseEntity.ToEntityReference();
            task1Entity.StatusCodeEnum = Task_StatusCode.Resolved;
            task1Entity.ipg_tasktypeid = null;

            var listForInit = new List<Entity>() { task1Entity, caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", task1Entity } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var postImage = new EntityImageCollection();
            postImage.Add("PostImage", task1Entity);
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = postImage,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<RequestToCloseIsResolved>(pluginContext);
            var incidents = fakedContext.CreateQuery<Incident>()
                .Where(inc => inc.ipg_casehold == true)
                .ToList();

            /// Assert
            Assert.True(incidents.Any());
        }

        [Fact]
        public void RequestToCloseIsResolvedTests_RequestToCloseCaseResovled_Success()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = Guid.NewGuid();
            caseEntity.ipg_casehold = true;

            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .RuleFor(p=>p.ipg_typeid,p=> 99022);//Request to Close Case

            Task task1Entity = new Task().Fake();
            task1Entity.Id = Guid.NewGuid();
            task1Entity.RegardingObjectId = caseEntity.ToEntityReference();
            task1Entity.StatusCodeEnum = Task_StatusCode.Resolved;
            task1Entity.ipg_tasktypeid = taskType.ToEntityReference();

            Team carrierteam = new Team().Fake("Carrier Services");

            var listForInit = new List<Entity>() { task1Entity, caseEntity, taskType, carrierteam };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", task1Entity } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var postImage = new EntityImageCollection();
            postImage.Add("PostImage", task1Entity);
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = postImage,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<RequestToCloseIsResolved>(pluginContext);
            var incidents = fakedContext.CreateQuery<Incident>()
                .Where(inc => inc.ipg_casehold == true)
                .ToList();

            /// Assert
            Assert.True(!incidents.Any());
        }
    }
}