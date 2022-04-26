using FakeXrmEasy;
using Insight.Intake.Plugin.Logging;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Logging
{
    public class CasePromotedToCollectionLogPluginTests : PluginTestsBase
    {
        [Theory]
        [ClassData(typeof(TestData))]
        public void MoveCaseBetweenGates(ipg_lifecyclestep prevLfStep, ipg_lifecyclestep lfStep,  List<ipg_gateprocessingrule> gateProcessingRules, List<ipg_gateconfiguration> gates, ipg_gateactivity log = null)
        {
            XrmFakedContext fakedContext = new XrmFakedContext();

            Incident preCase = new Incident().Fake().WithLFStep(prevLfStep);
            Incident postCase = new Incident().Fake(preCase.Id).WithLFStep(lfStep);
            Incident target = new Incident().Fake(preCase.Id).WithLFStep(lfStep);

            var inputParameters = new ParameterCollection { { "Target", target } };
            var preImage = new EntityImageCollection { { "PreImage", preCase } };
            var postImage = new EntityImageCollection { { "PostImage", postCase } };

            var intilizedList = new List<Entity>() { postCase, prevLfStep, lfStep };
            intilizedList.AddRange(gateProcessingRules);
            intilizedList.AddRange(gates);

            fakedContext.Initialize(intilizedList);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = postImage,
                PreEntityImages = preImage
            };
            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            fakedContext.ExecutePluginWith<CasePromotedToCollectionLogPlugin>(pluginContext);



            var createdLog = (from actlog in crmContext.CreateQuery<ipg_gateactivity>()
                              select actlog).ToList();

            if (log != null)
            {
                Assert.Single(createdLog);
                
                Assert.True(log.Subject == createdLog.First().Subject, "Subject not Equal");
               
                Assert.NotNull(createdLog.First().RegardingObjectId);
                Assert.Equal(preCase.LogicalName, createdLog.First().RegardingObjectId.LogicalName);
                Assert.Equal(preCase.Id, createdLog.First().RegardingObjectId.Id);
                Assert.Equal(postCase.ModifiedOn, createdLog.First().ActualStart);

            }
            else
            {
                Assert.Empty(createdLog);
            }
        }



        public class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return TestCasesGenerator.GetCaseMovedFromGate9ToNextTestData();
                yield return TestCasesGenerator.GetCaseMovedFromGate7ToNextTestData();
                yield return TestCasesGenerator.GetCaseMovedWithinGate9ToNextTestData();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static class TestCasesGenerator
        {
            public static void GetGatesLifecycleStepsProcessingRulesConfigs(out List<ipg_gateconfiguration> gates, out List<ipg_lifecyclestep> lfSteps, out List<ipg_gateprocessingrule> gateprocessingRules)
            {
                gates = new List<ipg_gateconfiguration>();
                lfSteps = new List<ipg_lifecyclestep>();
                gateprocessingRules = new List<ipg_gateprocessingrule>();

                ipg_gateconfiguration gate7 = new ipg_gateconfiguration().Fake("Gate 7");
                ipg_gateconfiguration gate8 = new ipg_gateconfiguration().Fake("Gate 8");
                ipg_gateconfiguration gate9 = new ipg_gateconfiguration().Fake("Gate 9");
                ipg_gateconfiguration gate10 = new ipg_gateconfiguration().Fake("Gate 10");

                gates.Add(gate7);
                gates.Add(gate8);
                gates.Add(gate9);
                gates.Add(gate10);

                ipg_lifecyclestep lfGate9step = new ipg_lifecyclestep().Fake(gate9, "lfGate9step");
                ipg_lifecyclestep lfGate9step2 = new ipg_lifecyclestep().Fake(gate9, "lfGate9step2");
                ipg_lifecyclestep lfGate10step = new ipg_lifecyclestep().Fake(gate10, "lfGate10step");
                ipg_lifecyclestep lfGate7step = new ipg_lifecyclestep().Fake(gate7, "lfGate7step");
                ipg_lifecyclestep lfGate8step = new ipg_lifecyclestep().Fake(gate8, "lfGate8step");

                lfSteps.Add(lfGate9step);
                lfSteps.Add(lfGate9step2);
                lfSteps.Add(lfGate10step);
                lfSteps.Add(lfGate7step);
                lfSteps.Add(lfGate8step);

                gateprocessingRules.Add((ipg_gateprocessingrule)new ipg_gateprocessingrule().Fake(lfGate9step, lfGate9step2, "processRule9"));
                gateprocessingRules.Add((ipg_gateprocessingrule)new ipg_gateprocessingrule().Fake(lfGate9step, lfGate10step, "processRule9_10"));
                gateprocessingRules.Add((ipg_gateprocessingrule)new ipg_gateprocessingrule().Fake(lfGate7step, lfGate8step, "processRule7_8"));
            }

            public static object[] GetCaseMovedFromGate9ToNextTestData()
            {
                List<ipg_gateprocessingrule> gateprocessingRules;
                List<ipg_gateconfiguration> gates;
                List<ipg_lifecyclestep> lfSteps;

                GetGatesLifecycleStepsProcessingRulesConfigs(out gates, out lfSteps, out gateprocessingRules);

                var testData = new object[5];
                testData[2] = gateprocessingRules;
                testData[3] = gates;

                testData[0] = lfSteps.Where(lfs => lfs.ipg_name == "lfGate9step").First();
                testData[1] = lfSteps.Where(lfs => lfs.ipg_name == "lfGate10step").First();

                testData[4] = new ipg_gateactivity() {Subject = "Case was promoted to Collections (passed Gate 9)"};

                return testData;
            }

            public static object[] GetCaseMovedFromGate7ToNextTestData()
            {
                List<ipg_gateprocessingrule> gateprocessingRules;
                List<ipg_gateconfiguration> gates;
                List<ipg_lifecyclestep> lfSteps;

                GetGatesLifecycleStepsProcessingRulesConfigs(out gates, out lfSteps, out gateprocessingRules);

                var testData = new object[5];
                testData[2] = gateprocessingRules;
                testData[3] = gates;

                testData[0] = lfSteps.Where(lfs => lfs.ipg_name == "lfGate7step").First();
                testData[1] = lfSteps.Where(lfs => lfs.ipg_name == "lfGate8step").First();

                return testData;
            }

            public static object[] GetCaseMovedWithinGate9ToNextTestData()
            {
                List<ipg_gateprocessingrule> gateprocessingRules;
                List<ipg_gateconfiguration> gates;
                List<ipg_lifecyclestep> lfSteps;

                GetGatesLifecycleStepsProcessingRulesConfigs(out gates, out lfSteps, out gateprocessingRules);

                var testData = new object[5];
                testData[2] = gateprocessingRules;
                testData[3] = gates;

                testData[0] = lfSteps.Where(lfs => lfs.ipg_name == "lfGate9step").First();
                testData[1] = lfSteps.Where(lfs => lfs.ipg_name == "lfGate9step2").First();

                return testData;
            }
        }
    }
}
