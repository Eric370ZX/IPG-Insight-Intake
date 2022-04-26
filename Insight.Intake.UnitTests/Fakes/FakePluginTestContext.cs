using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Moq.Language.Flow;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakePluginTestContext
    {

        #region IPluginExecutionContext
        public static Mock<IPluginExecutionContext> WithUserId(this Mock<IPluginExecutionContext> self, Guid userId)
        {
            self.Setup(t => t.UserId).Returns(userId);
            return self;
        }

        public static Mock<IPluginExecutionContext> WithEntityLogicalName(this Mock<IPluginExecutionContext> self, string entityLogicalName)
        {
            self.Setup(t => t.PrimaryEntityName).Returns(entityLogicalName);
            return self;
        }
        #endregion

        #region IServiceProvider
        public static Mock<IServiceProvider> WithServiceEndpointNotificationService(this Mock<IServiceProvider> self,
            IServiceEndpointNotificationService notificationService)
        {
            self.Setup(t => t.GetService(It.Is<Type>(i => i == typeof(IServiceEndpointNotificationService))))
                .Returns(notificationService);
            return self;
        }

        public static Mock<IServiceProvider> WithTracingService(this Mock<IServiceProvider> self,
            ITracingService notificationService)
        {
            self.Setup(t => t.GetService(It.Is<Type>(i => i == typeof(ITracingService)))).Returns(notificationService);
            return self;
        }

        public static Mock<IServiceProvider> WithOrganizationServiceFactory(this Mock<IServiceProvider> self,
            IOrganizationServiceFactory factoryMock)
        {
            self.Setup(t => t.GetService(It.Is<Type>(i => i == typeof(IOrganizationServiceFactory)))).Returns(factoryMock);
            return self;
        }

        public static Mock<IServiceProvider> WithPluginExecutionContext(this Mock<IServiceProvider> self,
            IPluginExecutionContext pluginExecutionContext)
        {
            self.Setup(t => t.GetService(It.Is<Type>(i => i == typeof(IPluginExecutionContext)))).Returns(pluginExecutionContext);
            return self;
        }
        #endregion

        #region IOrganizationServiceFactory
        public static Mock<IOrganizationServiceFactory> WithOrganizationServiceFactory(this Mock<IOrganizationServiceFactory> self,
            IOrganizationService organizationService)
        {
            self.Setup(t => t.CreateOrganizationService(It.IsAny<Guid>())).Returns(organizationService);
            return self;
        }

        public static Mock<IOrganizationServiceFactory> WithOrganizationService(this Mock<IOrganizationServiceFactory> self,
            IOrganizationService organizationService)
        {
            self.Setup(t => t.CreateOrganizationService(It.IsAny<Guid>())).Returns(organizationService);
            return self;
        }
        #endregion

        #region IOrganizationService
        public static IReturnsResult<IOrganizationService> WithCreateContactCrud(this Mock<IOrganizationService> self, Guid newGuid)
        {
            return self.Setup(x => x.Create(It.IsAny<Contact>()))
                .Returns(newGuid);
        }

        public static ISetup<IOrganizationService, Guid> WithCreateCrud<T>(this Mock<IOrganizationService> self) where T : Entity
        {
            return self.Setup(x => x.Create(It.IsAny<T>()));
        }

        public static ISetup<IOrganizationService> WithUpdateCrud<T>(this Mock<IOrganizationService> self) where T : Entity
        {
            return self.Setup(x => x.Update(It.IsAny<T>()));
        }
        
        public static Mock<IOrganizationService> WithRetrieveCrud<T>(this Mock<IOrganizationService> self, T entity) where T : Entity
        {
            self.Setup(x => x
                    .Retrieve(
                        It.Is<string>(isString => isString == entity.LogicalName), 
                        It.Is<Guid>(isGuid => isGuid == entity.Id), 
                        It.IsAny<ColumnSet>()
                    )
                )
                .Returns(entity);

            return self;
        }
        
        public static Mock<IOrganizationService> WithRetrieveMultipleCrud(this Mock<IOrganizationService> self, string entityName, EntityCollection entityCollection)
        {
            self.Setup(x => x
                    .RetrieveMultiple(
                        It.Is<QueryExpression>(isQueryExpression => isQueryExpression.EntityName == entityName)
                    )
                )
                .Returns(entityCollection);
            
            return self;
        }

        public static Mock<IOrganizationService> WithRetrieveMultipleCrud<T>(this Mock<IOrganizationService> self, string entityName, T entities) where T : IList<Entity>
        {
            self.Setup(x => x
                    .RetrieveMultiple(
                        It.Is<QueryExpression>(isQueryExpression => isQueryExpression.EntityName == entityName)
                    )
                )
                .Returns(new EntityCollection(entities));
            
            return self;
        }

        public static Mock<IOrganizationService> WithRetrieveMultipleCrud<T>(this Mock<IOrganizationService> self, string entityName, params T[] entitiesList) where T : IList<Entity>
        {
            var setup = self.SetupSequence(x => x
                .RetrieveMultiple(
                    It.Is<QueryExpression>(isQueryExpression => isQueryExpression.EntityName == entityName)
                )
            );

            foreach (var entities in entitiesList)
            {
                setup.Returns(new EntityCollection(entities));
            }
            
            return self;
        }
        #endregion
    }
}
