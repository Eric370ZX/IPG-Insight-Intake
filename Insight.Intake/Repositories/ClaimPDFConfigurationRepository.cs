using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Insight.Intake.Repositories
{
    class ClaimPDFConfigurationRepository
    {
        private readonly IOrganizationService _crmService;
        private readonly ITracingService _tracingService;

        public ClaimPDFConfigurationRepository(IOrganizationService crmService, ITracingService tracing)
        {
            _crmService = crmService;
            _tracingService = tracing;
        }

        public Dictionary<string, string> GetStaticValues()
        {
            var mappings = new Dictionary<string, string>();

            QueryExpression query = new QueryExpression(ipg_claimpdffieldsmapping.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_claimpdffieldsmapping.Fields.StateCode, ConditionOperator.Equal, (int)ipg_claimpdffieldsmappingState.Active),
                        new ConditionExpression(ipg_claimpdffieldsmapping.Fields.ipg_value, ConditionOperator.NotNull)
                    }
                }
            };

            _crmService.RetrieveMultiple(query).Entities.Select(m => m.ToEntity<ipg_claimpdffieldsmapping>()).ToList().ForEach(map =>
            {
                mappings.AddIfNotNull(map.ipg_fieldname ?? map.ipg_name, map.ipg_value);
            });


            return mappings;
        }

        public Dictionary<string, string> GetConfigurations(ipg_claim_type claimType, Incident incident, Invoice claim)
        {
            var mappings = new Dictionary<string, string>();

            QueryExpression query = new QueryExpression(ipg_claimpdffieldsmapping.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression()
                        {
                            FilterOperator = LogicalOperator.Or,
                            Conditions =
                            {
                                 new ConditionExpression(ipg_claimpdffieldsmapping.Fields.ipg_claimtypecode, ConditionOperator.Equal, (int)claimType),
                                 new ConditionExpression(ipg_claimpdffieldsmapping.Fields.ipg_claimtypecode, ConditionOperator.Null)
                            }
                        }
                    },
                    Conditions = 
                    { 
                        new ConditionExpression(ipg_claimpdffieldsmapping.Fields.StateCode, ConditionOperator.Equal, (int)ipg_claimpdffieldsmappingState.Active)
                    }
                }
            };

            _crmService.RetrieveMultiple(query).Entities.Select(m => m.ToEntity<ipg_claimpdffieldsmapping>()).ToList().ForEach(map =>
            {
                var value = ProcessClaimPDFFieldMapping(incident, claim, map);
                if (!string.IsNullOrEmpty(map.ipg_fieldname))
                {
                    mappings.AddIfNotNull(map.ipg_fieldname, value);
                }
                mappings.AddIfNotNull(map.ipg_name, value);
            });

            return mappings;
        }

        public string GetStringValueFromSource(Entity entity, string fieldNames)
        {
            var returnValue = "";
            fieldNames.GetSubstringDevidedByComma()
                .Select(f =>
                {
                    var formatfunction = f.ParseFunctionFieldNameAndParametersFromStr();
                    if (formatfunction.Count() < 2)
                    {
                        return entity.GetStringFromField(f.Trim(), _crmService);
                    }
                    else
                    {
                        var functionName = formatfunction.First();
                        var value = entity.GetStringFromField(formatfunction.Skip(1).First(), _crmService, functionName == "format" ? formatfunction.Skip(2).First() : "");
                        if (functionName == "substr")
                        {
                            value = formatfunction.Count == 4 ? value.Substring(int.Parse(formatfunction.Skip(2).First()), int.Parse(formatfunction.Skip(3).First())) : value.Substring(int.Parse(formatfunction.Skip(2).First()));
                        }

                        return value;
                    }
                }).Where(v => !string.IsNullOrEmpty(v))
                .ToList().ForEach(f => returnValue += string.IsNullOrEmpty(returnValue) ? f : $", {f}");

            return returnValue;
        }

        public bool CheckConditions(Entity target, string conditions)
        {
            var fetchExpression = $@"<fetch top='1'>
                            <entity name='{target.LogicalName}'>
                            <attribute name='{target.LogicalName}id'/>
                            <filter type='and'>
                                <condition attribute='{target.LogicalName}id' operator='eq' value='{target.Id}'/>
                                {conditions}
                            </filter>
                            </entity>
                        </fetch>";

            try
            {
                return _crmService.RetrieveMultiple(new FetchExpression(fetchExpression)).Entities.Any();
            }
            catch (Exception e)
            {
                _tracingService.Trace($@"Could not check conditon with by fetchexpression {fetchExpression}");
            }

            return false;
        }

        private string ProcessClaimPDFFieldMapping(Incident incident, Invoice claim, ipg_claimpdffieldsmapping mapping)
        {
            var result = string.Empty;
            if(!string.IsNullOrWhiteSpace(mapping.ipg_conditions))
            {
                result = CheckConditions(incident, mapping.ipg_conditions) ? "Yes" : "No";
            }
            else
            {
                Regex rgx = new Regex(@"\[\w+\.\w+\]");
                var mc = rgx.Matches(mapping.ipg_value.Trim());
                if(mc.Count > 0)
                {
                    var arr = mc[0].Value.Substring(1, mc[0].Value.Length - 2).Split('.');
                    if (arr.Length == 2)
                    {
                        Entity ent = null;
                        if (arr[0].ToLower() == "incident")
                        {
                            ent = incident;
                        }
                        else if (arr[0].ToLower() == "invoice")
                        {
                            ent = claim;
                        }
                        if (ent != null)
                        {
                            var value = ent[arr[1].ToLower()];
                            if (value != null)
                            {
                                if (value.GetType() == typeof(EntityReference))
                                {
                                    var entityRef = (EntityReference)value;
                                    var entityName = entityRef.LogicalName.StartsWith("ipg") ? entityRef.LogicalName : entityRef.LogicalName.First().ToString().ToUpper() + entityRef.LogicalName.Substring(1);
                                    var primaryNameAttribute = Type.GetType("Insight.Intake." + entityName).GetField("PrimaryNameAttribute").GetValue(new Entity(entityRef.LogicalName, (entityRef.Id)));
                                    var entity = _crmService.Retrieve(entityRef.LogicalName, entityRef.Id, new ColumnSet((string)primaryNameAttribute));
                                    if (entity.Contains((string)primaryNameAttribute) && entity[(string)primaryNameAttribute] != null)
                                    {
                                        result = (string)entity[(string)primaryNameAttribute];
                                    }
                                }
                                else if (value.GetType() == typeof(OptionSetValue))
                                {
                                    result = ((OptionSetValue)value).Value.ToString();
                                }
                                else if (value.GetType() == typeof(bool))
                                {
                                    result = (bool)value ? "Yes" : "No";
                                }
                                else if (value.GetType() == typeof(DateTime))
                                {
                                    result = ((DateTime)value).ToString("MM/dd/yy");
                                }
                                else if (value.GetType() == typeof(Money))
                                {
                                    result = ((Money)value).Value.ToString();
                                }
                                else
                                {
                                    result = value.ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    result = mapping.ipg_value;
                }
            }
            return result;
        }

    }
}