using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;

namespace Insight.Intake
{
    public class PluginBase : IPlugin
    {
        static PluginBase()
        {
            // Hook into ResolveAssembly event for external dependencies
            //AssemblyLoader.RegisterAssemblyLoader();
        }

        public class LocalPluginContext
        {
            internal IServiceProvider ServiceProvider { get; private set; }

            public T GetInput<T>(string parameterName)
            {
                var context = this.PluginExecutionContext;
                if (!this.PluginExecutionContext.InputParameters.Contains(parameterName))
                {
                    throw new InvalidPluginExecutionException($"{parameterName} is empty");
                }

                var param = this.PluginExecutionContext.InputParameters[parameterName];
                if (param == null)
                {
                    throw new InvalidPluginExecutionException($"{parameterName} is null");

                }
                if (!(param is T))
                {
                    throw new InvalidPluginExecutionException($"{parameterName} is not of type {typeof(T).Name}. Real type is {param?.GetType().Name}");

                }
                return (T)param;
            }

            public T GetNullAbleInput<T>(string parameterName)
            {
                if (!this.PluginExecutionContext.InputParameters.Contains(parameterName))
                {
                    return default(T);
                }

                var param = this.PluginExecutionContext.InputParameters[parameterName];

                if (param == null)
                {
                    return default(T);
                }
                if (!(param is T))
                {
                    throw new InvalidPluginExecutionException($"{parameterName} is not of type {typeof(T).Name}. Real type is {param?.GetType().Name}");

                }

                return (T)param;
            }

            internal IOrganizationService OrganizationService { get; private set; }

            internal IOrganizationService SystemOrganizationService { get; private set; }
            internal IPluginExecutionContext PluginExecutionContext { get; private set; }
            internal ITracingService TracingService { get; private set; }

            public EntityReference TargetRef(string parameterName = "Target") =>
             GetInput<EntityReference>(parameterName);

            public T Target<T>() where T : Entity => GetInput<Entity>("Target").ToEntity<T>();
            public T PostImage<T>(string imageName = "PostImage") where T : Entity => this.PluginExecutionContext.PostEntityImages[imageName].ToEntity<T>();
            public T PreImage<T>(string imageName = "PreImage") where T : Entity => this.PluginExecutionContext.PreEntityImages.Contains(imageName) ? this.PluginExecutionContext.PreEntityImages[imageName].ToEntity<T>() : null;

            private LocalPluginContext()
            {
            }

            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                // Obtain the execution context service from the service provider.
                this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the tracing service from the service provider.
                this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the Organization Service factory service from the service provider
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                this.OrganizationService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);
                this.SystemOrganizationService = factory.CreateOrganizationService(null);
            }

            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                if (this.PluginExecutionContext == null)
                {
                    this.TracingService.Trace(message);
                }
                else
                {
                    this.TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this.PluginExecutionContext.CorrelationId,
                        this.PluginExecutionContext.InitiatingUserId);
                }
            }
        }

        private Collection<Tuple<int, string, string, Action<LocalPluginContext>>> registeredEvents;

        /// <summary>
        /// Gets the List of events that the plug-in should fire for. Each List
        /// Item is a <see cref="System.Tuple"/> containing the Pipeline Stage, Message and (optionally) the Primary Entity. 
        /// In addition, the fourth parameter provide the delegate to invoke on a matching registration.
        /// </summary>
        protected Collection<Tuple<int, string, string, Action<LocalPluginContext>>> RegisteredEvents
        {
            get
            {
                if (this.registeredEvents == null)
                {
                    this.registeredEvents = new Collection<Tuple<int, string, string, Action<LocalPluginContext>>>();
                }

                return this.registeredEvents;
            }
        }

        protected void RegisterEvent(int pipelineStage, string messageName, string entityLogicalName, Action<LocalPluginContext> action)
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(pipelineStage, messageName, entityLogicalName, action));
        }

        /// <summary>
        /// Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        protected string ChildClassName
        {
            get;

            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="childClassName">The <see cref=" cred="Type"/> of the derived class.</param>
        internal PluginBase(Type childClassName)
        {
            this.ChildClassName = childClassName.ToString();
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);

            localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName));
            localcontext.Trace("Depth = " + localcontext.PluginExecutionContext.Depth);
            localcontext.Trace("PrimaryEntityName = " + localcontext.PluginExecutionContext.PrimaryEntityName);
            localcontext.Trace("MessageName = " + localcontext.PluginExecutionContext.MessageName);
            localcontext.Trace("Mode = " + localcontext.PluginExecutionContext.Mode);
            localcontext.Trace("Stage = " + localcontext.PluginExecutionContext.Stage);

            try
            {
                // Iterate over all of the expected registered events to ensure that the plugin
                // has been invoked by an expected event
                // For any given plug-in event at an instance in time, we would expect at most 1 result to match.
                Action<LocalPluginContext> entityAction =
                    (from a in this.RegisteredEvents
                     where (
                         a.Item1 == localcontext.PluginExecutionContext.Stage &&
                         a.Item2 == localcontext.PluginExecutionContext.MessageName &&
                         (string.IsNullOrWhiteSpace(a.Item3)
                             ? true
                             : MessageNames.Associate == localcontext.PluginExecutionContext.MessageName ?
                                a.Item3 == localcontext.GetNullAbleInput<Relationship>("Relationship")?.SchemaName
                                : a.Item3 == localcontext.PluginExecutionContext.PrimaryEntityName)
                         )
                     select a.Item4).FirstOrDefault();

                if (entityAction != null)
                {
                    localcontext.Trace(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} is firing for Entity: {1}, Message: {2}",
                        this.ChildClassName,
                        localcontext.PluginExecutionContext.PrimaryEntityName,
                        localcontext.PluginExecutionContext.MessageName));

                    entityAction.Invoke(localcontext);

                    // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                    // guard against multiple executions.
                    return;
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e));
                throw;
            }
            catch (Exception ex)
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", ex));
                throw new InvalidPluginExecutionException("Error: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message), ex);
            }
            finally
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName));
            }
        }
    }

    public static class PipelineStages
    {
        public const int PreValidation = 10;
        public const int PreOperation = 20;
        public const int PostOperation = 40;
    }

    public static class MessageNames
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string SetState = "SetState";
        public const string SetStateDynamicEntity = "SetStateDynamicEntity";
        public const string Assign = "Assign";
        public const string Publish = "Publish";
        public const string PublishAll = "PublishAll";
        public const string Retrieve = "Retrieve";
        public const string RetrieveMultiple = "RetrieveMultiple";
        public const string Associate = "Associate";
    }
}
