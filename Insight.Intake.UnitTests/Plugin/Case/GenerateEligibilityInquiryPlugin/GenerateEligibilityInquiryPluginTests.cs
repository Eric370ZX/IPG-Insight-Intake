using System;
using System.IO;
using System.Text.RegularExpressions;
using Insight.Intake.Models;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;
using Enumerable = System.Linq.Enumerable;
using Match = System.Text.RegularExpressions.Match;

namespace Insight.Intake.UnitTests.Plugin.Case.GenerateEligibilityInquiryPlugin
{
    //this unit test in commented out because GenerateEligibilityInquiryPlugin has been refactored 
    //as a part of BenefitVerifier
    //public class GenerateEligibilityInquiryPluginTests : PluginTestsBase
    //{
    //    private static readonly string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Plugin\Case\GenerateEligibilityInquiryPlugin";
    //    private readonly Intake.Plugin.Case.GenerateEligibilityInquiryPlugin _plugin = new Intake.Plugin.Case.GenerateEligibilityInquiryPlugin(
    //        File.ReadAllText(CurrentPath + @"\UnsecureConfig.xml"),
    //        File.ReadAllText(CurrentPath + @"\SecureConfig.xml"));

    //    [Fact]
    //    public void Generates_XML_when_Patient_is_Insured()
    //    {
    //        //ARRANGE

    //        var inputTarget = new EntityReference(Incident.EntityLogicalName, Guid.Parse("fb97b9f9-47f1-45dc-9da7-ec2173a0d193"));
    //        var inputParameters = new ParameterCollection
    //        {
    //            {"Target", inputTarget},
    //            {Intake.Plugin.Case.GenerateEligibilityInquiryPlugin.IsUserGeneratedInputParameterName, true}
    //        };
    //        PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

    //        var fakeCarrier = new Intake.Account().FakeCarrierForEBV();
    //        var fakeIncident = new Incident().FakeInsuredPatient(fakeCarrier);
    //        OrganizationServiceMock
    //            .Setup(x => x.Retrieve(Incident.EntityLogicalName, inputTarget.Id, It.Is<ColumnSet>(cs => 
    //                ContainsColumns(cs, nameof(Incident.ipg_CarrierId).ToLower(),
    //                    nameof(Incident.ipg_MemberIdNumber).ToLower(),
    //                    nameof(Incident.ipg_RelationToInsured).ToLower(),
    //                    nameof(Incident.ipg_PatientFirstName).ToLower(),
    //                    nameof(Incident.ipg_PatientLastName).ToLower(),
    //                    nameof(Incident.ipg_PatientMiddleName).ToLower(),
    //                    nameof(Incident.ipg_PatientDateofBirth).ToLower(),
    //                    nameof(Incident.ipg_PatientGender).ToLower(),
    //                    nameof(Incident.ipg_SurgeryDate).ToLower()))))
    //            .Returns(fakeIncident);

    //        OrganizationServiceMock.Setup(os => os.Retrieve(Intake.Account.EntityLogicalName, fakeIncident.ipg_CarrierId.Id,
    //            It.Is<ColumnSet>(cs => ContainsColumns(cs, nameof(Intake.Account.Name).ToLower(),
    //                nameof(Intake.Account.ipg_ZirMedID).ToLower())))).Returns(fakeCarrier);

    //        var outputParameters = new ParameterCollection();
    //        PluginExecutionContextMock.Setup(pec => pec.OutputParameters).Returns(outputParameters);

    //        ServiceProvider = ServiceProviderMock.Object;


    //        //ACT

    //        _plugin.Execute(ServiceProvider);


    //        //ASSERT

    //        var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
    //        string outputXml = pluginExecutionContext
    //            .OutputParameters[Intake.Plugin.Case.GenerateEligibilityInquiryPlugin.OutputParameterName].ToString();
    //        string expectedXml = File.ReadAllText(CurrentPath + @"\EligibilityInquirySubscriber.xml");
    //        expectedXml = CopyXmlElement(outputXml, expectedXml, nameof(EligibilityInquiry.id));
    //        expectedXml = CopyXmlElement(outputXml, expectedXml, nameof(EligibilityInquiry.CreatedAt));

    //        Assert.Equal(expectedXml, outputXml);
    //    }

    //    [Fact]
    //    public void Generates_XML_when_Patient_is_Dependent()
    //    {
    //        //ARRANGE

    //        var inputTarget = new EntityReference(Incident.EntityLogicalName, Guid.Parse("fb97b9f9-47f1-45dc-9da7-ec2173a0d193"));
    //        var inputParameters = new ParameterCollection
    //        {
    //            {"Target", inputTarget},
    //            {Intake.Plugin.Case.GenerateEligibilityInquiryPlugin.IsUserGeneratedInputParameterName, true}
    //        };
    //        PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

    //        var fakeCarrier = new Intake.Account().FakeCarrierForEBV();
    //        var fakeIncident = new Incident().FakeDependentPatient(fakeCarrier);
    //        OrganizationServiceMock
    //            .Setup(x => x.Retrieve(Incident.EntityLogicalName, inputTarget.Id, It.Is<ColumnSet>(cs =>
    //                ContainsColumns(cs, nameof(Incident.ipg_CarrierId).ToLower(),
    //                    nameof(Incident.ipg_MemberIdNumber).ToLower(),
    //                    nameof(Incident.ipg_RelationToInsured).ToLower(),
    //                    nameof(Incident.ipg_InsuredFirstName).ToLower(),
    //                    nameof(Incident.ipg_InsuredLastName).ToLower(),
    //                    nameof(Incident.ipg_InsuredMiddleName).ToLower(),
    //                    nameof(Incident.ipg_InsuredDateOfBirth).ToLower(),
    //                    nameof(Incident.ipg_InsuredGender).ToLower(),
    //                    nameof(Incident.ipg_PatientFirstName).ToLower(),
    //                    nameof(Incident.ipg_PatientLastName).ToLower(),
    //                    nameof(Incident.ipg_PatientMiddleName).ToLower(),
    //                    nameof(Incident.ipg_PatientDateofBirth).ToLower(),
    //                    nameof(Incident.ipg_PatientGender).ToLower(),
    //                    nameof(Incident.ipg_SurgeryDate).ToLower()))))
    //            .Returns(fakeIncident);

    //        OrganizationServiceMock.Setup(os => os.Retrieve(Intake.Account.EntityLogicalName, fakeIncident.ipg_CarrierId.Id,
    //            It.Is<ColumnSet>(cs => ContainsColumns(cs, nameof(Intake.Account.Name).ToLower(),
    //                nameof(Intake.Account.ipg_ZirMedID).ToLower())))).Returns(fakeCarrier);

    //        var outputParameters = new ParameterCollection();
    //        PluginExecutionContextMock.Setup(pec => pec.OutputParameters).Returns(outputParameters);

    //        ServiceProvider = ServiceProviderMock.Object;


    //        //ACT

    //        _plugin.Execute(ServiceProvider);


    //        //ASSERT

    //        var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
    //        string outputXml = pluginExecutionContext
    //            .OutputParameters[Intake.Plugin.Case.GenerateEligibilityInquiryPlugin.OutputParameterName].ToString();
    //        string expectedXml = File.ReadAllText(CurrentPath + @"\EligibilityInquiryDependent.xml");
    //        expectedXml = CopyXmlElement(outputXml, expectedXml, nameof(EligibilityInquiry.id));
    //        expectedXml = CopyXmlElement(outputXml, expectedXml, nameof(EligibilityInquiry.CreatedAt));

    //        Assert.Equal(expectedXml, outputXml);
    //    }


    //    #region helpers

    //    private bool ContainsColumns(ColumnSet columnSet, params string[] columns)
    //    {
    //        return Enumerable.Any(columns, c => columnSet.Columns.Contains(c));
    //    }

    //    private string CopyXmlElement(string xmlFrom, string xmlTo, string elementName)
    //    {
    //        string xmlElementPattern = $"<{elementName}>" + "{0}" + $"</{elementName}>";
    //        string findPattern = string.Format(xmlElementPattern, @"([^<>]+)");
    //        Match guidMatch = Regex.Match(xmlFrom, findPattern);
    //        if (guidMatch.Success == false)
    //        {
    //            throw new Exception("Could not find the requested element");
    //        }
    //        string guid = guidMatch.Groups[1].Value;
    //        string newXmlElement = string.Format(xmlElementPattern, guid);
    //        return Regex.Replace(xmlTo, findPattern, newXmlElement);
    //    }

    //    #endregion


    //}
}
