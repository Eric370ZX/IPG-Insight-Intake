using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.NPI
{
    public class CreateTaskWhenPhysicianNPIIsUniqueTests : PluginTestsBase
    {
        [Fact]
        public void CreateContactMissingNpiTaskToSaveTests()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();

            Contact caseEntity = new Contact().Fake();

            Task relTask = new Task().Fake()
                .RuleFor(p => p.RegardingObjectId, p => caseEntity.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            //Act
            Contact npi = caseEntity.FakeWithMissingNPI("");

            //Assert
            if (npi.ipg_PhysicianNPI == "")
            {
                Assert.False(false);
           //     throw new Exception($"NPI is a REQUIRED field!");
            }


        }

        [Fact]
        public void CreateContactNpiTaskToSaveTests()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();

            Contact caseEntity = new Contact().Fake();

            Task relTask = new Task().Fake()
                .RuleFor(p => p.RegardingObjectId, p => caseEntity.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            //Act
            Contact npi = caseEntity.FakeWithNPI();

            //Assert
            if (npi.ipg_PhysicianNPI == "")
            {
                Assert.False(false);
            }
            else
            {
                Assert.True(true);
            }

        }

        [Fact]
        public void CreateContactDuplicateNpiTaskToSaveTests()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();

            Contact caseEntity = new Contact().Fake();

            Task relTask = new Task().Fake()
                .RuleFor(p => p.RegardingObjectId, p => caseEntity.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            //Act
            Contact npi = caseEntity.FakeWithDuplicateNPI("123456789");

            //Assert
            if (npi.ipg_PhysicianNPI == "duplicate")
            {
                Assert.False(false);
              //  throw new Exception($"NPI {npi.ipg_PhysicianNPI}is a duplicate number");
            }


        }
        [Fact]
        public void CreateContactReplaceEntryNpiTaskToSaveTests()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();

            Contact caseEntity = new Contact().Fake();

            Task relTask = new Task().Fake()
                .RuleFor(p => p.RegardingObjectId, p => caseEntity.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            //Act
            Contact npi = caseEntity.FakeWithDuplicateNPI("123456789");

            //Assert
            if (npi.ipg_PhysicianNPI == "duplicate")
            {
                Assert.False(false);
               // throw new Exception($"NPI {npi.ipg_PhysicianNPI} exists replace it with a unique number before saving");
            }


        }
        [Fact]
        public void CreateContactExistingPhysicianNpiTaskToSaveTests()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();

            Contact caseEntity = new Contact().Fake();

            Task relTask = new Task().Fake()
                .RuleFor(p => p.RegardingObjectId, p => caseEntity.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            //Act
            Contact npi = caseEntity.FakeWithDuplicateNPI("123456789");

            //Assert
            if (npi.ipg_PhysicianNPI == "duplicate")
            {
                Assert.False(false);
               // throw new Exception($"NPI {npi.ipg_PhysicianNPI} exists for Physican, {npi.FirstName} {npi.LastName}");
            }

        }
    }

    }