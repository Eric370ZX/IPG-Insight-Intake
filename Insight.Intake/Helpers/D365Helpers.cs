using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Helpers
{
    public static class D365Helpers
    {
        public static string GetOptionSetValueLabel(string entityName, string fieldName, int optionSetValue, IOrganizationService service)
        {
            var attReq = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = fieldName,
                RetrieveAsIfPublished = true
            };

            var attResponse = new RetrieveAttributeResponseWrapper(service.Execute(attReq));
            EnumAttributeMetadata attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

            OptionMetadata optionMetadata = attMetadata.OptionSet.Options.FirstOrDefault(x => x.Value == optionSetValue);

            return optionMetadata?.Label.UserLocalizedLabel.Label;
        }
        public static string GetGlobalSettingValueByKey(this IOrganizationService service, string key)
        {
            QueryByAttribute query = new QueryByAttribute(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_globalsetting.ipg_value).ToLower())
            };
            query.AddAttributeValue(nameof(ipg_globalsetting.ipg_name).ToLower(), key);

            EntityCollection coll = service.RetrieveMultiple(query);

            if (coll.Entities.Count == 1)
                return coll.Entities[0].GetAttributeValue<string>(nameof(ipg_globalsetting.ipg_value).ToLower());

            return string.Empty;
        }
        public static Team GetTeam(this IOrganizationService service, string teamName)
        {
            QueryByAttribute query = new QueryByAttribute(Team.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddAttributeValue(nameof(Team.Name).ToLower(), teamName);

            EntityCollection coll = service.RetrieveMultiple(query);

            return coll.Entities.FirstOrDefault()?.ToEntity<Team>();
        }
        public static string GetGlobalSettingValueByKey(this IOrganizationService service, string key, string defaultValue)
        {
            var value = string.Empty;

            QueryByAttribute query = new QueryByAttribute(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_globalsetting.ipg_value).ToLower())
            };
            query.AddAttributeValue(nameof(ipg_globalsetting.ipg_name).ToLower(), key);

            EntityCollection coll = service.RetrieveMultiple(query);

            if (coll.Entities.Count == 0)
            {
                service.Create(new ipg_globalsetting()
                {
                    ipg_name = key,
                    ipg_value = defaultValue
                });

                value = defaultValue;
            }
            else if (coll.Entities.Count == 1)
            {
                value = coll.Entities[0].GetAttributeValue<string>(nameof(ipg_globalsetting.ipg_value).ToLower());
                
                if (string.IsNullOrEmpty(value))
                {
                    service.Update(new ipg_globalsetting() { Id = coll.Entities[0].Id, ipg_value = defaultValue });

                    value = defaultValue;
                }
            }

            return value;
        }

        /// <summary>
        ///  Returns Address from Global Config Zip, State, City, Street
        /// </summary>
        /// <param name="crmContext"></param>
        /// <returns></returns>
        public static Tuple<string, string, string, string> GetIPGAddressFromConfiguration(IOrganizationService service)
        {
            var defaultValue = "ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta";
            
            var ipgConfig = service.GetGlobalSettingValueByKey("IPG Address", defaultValue);

            Func<List<string>, string, string> findValue = (source, key) =>
            {
                var temp = source.Where(s => !string.IsNullOrEmpty(s) && s.ToLower().Contains(key)).FirstOrDefault()?.Split(':');
                return temp != null && temp.Length > 1 ? temp[1] : null;
            };

            var addressArray = ipgConfig.Split(';').ToList();
            var zip = findValue(addressArray, "zip");
            var state = findValue(addressArray, "state");
            var city = findValue(addressArray, "city");
            var street = findValue(addressArray, "street");

            return new Tuple<string, string, string, string>(zip, state, city, street);
        }

        public static QueryExpression ConvertFetchXmlToQueryExpression(this IOrganizationService service, string fetchXml)
        {
            FetchXmlToQueryExpressionRequest conversionRequest = new FetchXmlToQueryExpressionRequest()
            {
                FetchXml = fetchXml
            };

            FetchXmlToQueryExpressionResponse conversionResponse =
                (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

            return conversionResponse.Query;
        }

        public static string GetIdAsString(Guid guid)
        {
            return guid.ToString().Replace("{", String.Empty).Replace("}", String.Empty);
        }

        public static string setStringValuesByTemplate(string template, Dictionary<string, string> templateData = null)
        {
            if (template != String.Empty && templateData != null)
            {
                foreach (var key in templateData.Keys)
                {
                    template = template.Replace(key, templateData[key]);
                }
            }
            return template;
        }
    }
}
