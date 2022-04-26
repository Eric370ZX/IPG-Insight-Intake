using System;
using System.ServiceModel;
using Insight.Intake.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Account
{
    public class CheckFacilityBSAEntitlementsPlugin : IPlugin
    {
        //todo: try to replace this plugin with a universal FetchXML plugin


        public static readonly string TargetInputParameterName = "Target";
        public static readonly string CheckOnDateInputParameterName = "Date";
        public static readonly string ResultOutputParameterName = "Result";


        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracingService.Trace($"{this.GetType()} Execute method started");

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                tracingService.Trace($"Validating {TargetInputParameterName} input parameter");
                if (context.InputParameters.Contains(TargetInputParameterName) == false
                    || !(context.InputParameters[TargetInputParameterName] is EntityReference targetEntityReference)
                    || targetEntityReference.LogicalName != Intake.Account.EntityLogicalName)
                {
                    throw new Exception($"{TargetInputParameterName} entity Account is required");
                }

                var checkOnDate = (DateTime?)context.InputParameters[CheckOnDateInputParameterName];
                if (checkOnDate.HasValue == false)
                {
                    throw new Exception($"{CheckOnDateInputParameterName} input parameter is required");
                }


                tracingService.Trace("Getting Organization service");
                var organizationService = serviceProvider.GetOrganizationService(context.UserId);

                tracingService.Trace("Retrieving account BSA entitlements");
                var queryExpression = new QueryExpression(Entitlement.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(Entitlement.ipg_EntitlementType).ToLower(), ConditionOperator.Equal, (int)ipg_EntitlementTypes.BSA),
                            new ConditionExpression(nameof(Entitlement.CustomerId).ToLower(), ConditionOperator.Equal, targetEntityReference.Id),
                            new ConditionExpression(nameof(Entitlement.StartDate).ToLower(), ConditionOperator.LessEqual, checkOnDate),
                            new ConditionExpression(nameof(Entitlement.EndDate).ToLower(), ConditionOperator.GreaterEqual, checkOnDate)
                        }
                    }
                };
                EntityCollection entitlements = organizationService.RetrieveMultiple(queryExpression);

                tracingService.Trace("Adding output parameter");
                bool result = entitlements.Entities.Count > 0;
                if (context.OutputParameters.Contains(ResultOutputParameterName))
                {
                    context.OutputParameters[ResultOutputParameterName] = result;
                }
                else
                {
                    context.OutputParameters.Add(ResultOutputParameterName, result);
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace($"{this.GetType().Name}: {exception}");
                throw;
            }
        }
    }
}