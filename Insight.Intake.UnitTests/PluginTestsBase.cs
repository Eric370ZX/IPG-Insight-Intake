using System;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Insight.Intake.UnitTests
{
    public class PluginTestsBase
    {
        internal readonly Mock<IPluginExecutionContext> PluginExecutionContextMock;
        internal readonly Mock<IServiceProvider> ServiceProviderMock;
        internal readonly Mock<IOrganizationService> OrganizationServiceMock;

        internal IServiceProvider ServiceProvider;
        internal IOrganizationService OrganizationService;

        public PluginTestsBase()
        {
            OrganizationServiceMock = new Mock<IOrganizationService>();

            var tracingServiceMock = new Mock<ITracingService>();
            tracingServiceMock.Setup(t => t.Trace(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((t1, t2) => Console.WriteLine(t1, t2));

            PluginExecutionContextMock = new Mock<IPluginExecutionContext>().WithUserId(Guid.NewGuid())
                .WithEntityLogicalName(ipg_referral.EntityLogicalName);

            ServiceProviderMock = new Mock<IServiceProvider>()
                .WithServiceEndpointNotificationService(new Mock<IServiceEndpointNotificationService>().Object)
                .WithTracingService(tracingServiceMock.Object)
                .WithOrganizationServiceFactory(new Mock<IOrganizationServiceFactory>().WithOrganizationService(OrganizationServiceMock.Object).Object)
                .WithPluginExecutionContext(PluginExecutionContextMock.Object);
        }
    }
}
