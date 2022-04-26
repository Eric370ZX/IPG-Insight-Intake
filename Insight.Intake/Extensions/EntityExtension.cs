using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Extensions
{
    public static class EntityExtension
    {
        public static T MergeWithImage<T>(this Entity entity, T image) where T : Entity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var entityName = entity.LogicalName ?? image.LogicalName;

            var newEntity = new Entity(entityName);

            if (image.Attributes != null)
            {
                foreach (KeyValuePair<string, object> entry in image.Attributes)
                {
                    newEntity[entry.Key] = entity.Attributes.Contains(entry.Key) ? entity[entry.Key] : entry.Value;
                }
            }

            return newEntity.ToEntity<T>();
        }

        public static T DeepClone<T>(this T obj) where T : Entity
        {
            var ser = new DataContractSerializer(typeof(T));

            using (MemoryStream memory_stream = new MemoryStream())
            {
                ser.WriteObject(memory_stream, obj);
                memory_stream.Position = 0;
                return (T)ser.ReadObject(memory_stream);
            }
        }

        public static T Merge<T>(this T entity, T image) where T : Entity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (entity.LogicalName != image.LogicalName)
            {
                throw new ArgumentException(nameof(entity.LogicalName));
            }

            var newEntity = entity.DeepClone();

            if (image.Attributes != null)
            {
                foreach (KeyValuePair<string, object> entry in image.Attributes)
                {
                    if (!newEntity.Contains(entry.Key))
                    {
                        newEntity[entry.Key] = entry.Value;
                    }
                }
            }
            return newEntity.ToEntity<T>();
        }

        public static string GetAttributeLogicalName<T, P>(this T record,
                          Expression<Func<T, P>> attribute)
        where T : Entity
        {
            MemberExpression memberExpression =
                (MemberExpression)attribute.Body;
            var member = memberExpression.Member;

            var nameAttributes = member.GetCustomAttributes(
                typeof(AttributeLogicalNameAttribute), true);
            if (nameAttributes != null && nameAttributes.Length > 0)
            {
                var logicalName =
                    (nameAttributes[0] as AttributeLogicalNameAttribute)
                    .LogicalName;
                return logicalName;
            }
            throw new ArgumentException(string.Format(
                "{0} is not a CRM property of entity {1}",
                member.Name, typeof(T).Name));
        }
        public static string FormattedValue(this Entity source, string fieldName) => source.FormattedValues.Contains(fieldName) ? source.FormattedValues[fieldName] : "";

        /// <summary>
        /// Get Surgery Date from entity
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <returns>Datetime if it contains in source entity, or null</returns>
        public static DateTime? GetCaseDos(this Entity source)
        {
            if (source.LogicalName != Incident.EntityLogicalName && source.LogicalName != ipg_referral.EntityLogicalName)
            {
                return null;
            }
            return (source.Contains("ipg_actualdos") && source.GetAttributeValue<DateTime?>("ipg_actualdos") != null)
                ? source.GetAttributeValue<DateTime?>("ipg_actualdos")
                : source.GetAttributeValue<DateTime?>("ipg_surgerydate");
        }

        public static string GetStringFromField(this Entity entity, string fieldName, IOrganizationService crmService, string format = "")
        {
            var containerValue = entity.GetValueFromParentEntity(fieldName, crmService);
            
            if (containerValue is DateTime?)
            {
                var date = containerValue as DateTime?;
                
                return date?.ToString(format);
            }
            if (containerValue is string)
            {
                return containerValue as string;
            }
            else if (containerValue is bool)
            {
                return (containerValue as bool?) == true ? "Yes" : "No";
            }
            else if (containerValue is EntityReference)
            {
                var reference = containerValue as EntityReference;
                if (reference != null && reference.Name == null)
                {
                    var namefield = reference.LogicalName.StartsWith("ipg") ? "ipg_name" : "name";
                    reference.Name = crmService.Retrieve(reference.LogicalName, reference.Id, new ColumnSet(namefield)).GetAttributeValue<string>(namefield);
                }
                return reference?.Name;
            }
            else
            {
                return null;
            }
        }

        public static Dictionary<string, string> MergeWith(this Dictionary<string, string> main, Dictionary<string, string> additionals)
        {
            var newDictonary = new Dictionary<string, string>(main);
            foreach (var item in additionals)
            {
                if(newDictonary.ContainsKey(item.Key))
                {
                    newDictonary[item.Key] =  item.Value;
                }
                else
                {
                    newDictonary.Add(item.Key, item.Value);
                }
            }

            return newDictonary;
        }

        public static object GetValueFromParentEntity(this Entity entity, string fieldName, IOrganizationService crmService)
        {
            if(string.IsNullOrEmpty(fieldName))            
            {
                return null;
            }
            if (!fieldName.Contains("id."))
            {
                return entity[fieldName];
            }
            else
            {
                var dotIndex = fieldName.IndexOf(".");
                var reference = entity[fieldName.Substring(0, dotIndex)] as EntityReference;
                var parentEntityfieldName = fieldName.Substring(dotIndex + 1);
                
                if(parentEntityfieldName.Contains("id."))
                {
                    parentEntityfieldName = parentEntityfieldName.Substring(0, parentEntityfieldName.IndexOf("."));
                }

                return reference != null ? crmService.Retrieve(reference.LogicalName, reference.Id, new ColumnSet(parentEntityfieldName)).GetValueFromParentEntity(parentEntityfieldName, crmService) : null;
            }
        }
    }

    public class LogicalNameof<T>
    {
        public static string Property<TProp>(Expression<Func<T, TProp>> attribute)
        {
            MemberExpression memberExpression =
                (MemberExpression)attribute.Body;
            var member = memberExpression.Member;

            var nameAttributes = member.GetCustomAttributes(
                typeof(AttributeLogicalNameAttribute), true);
            if (nameAttributes != null && nameAttributes.Length > 0)
            {
                var logicalName =
                    (nameAttributes[0] as AttributeLogicalNameAttribute)
                    .LogicalName;
                return logicalName;
            }
            throw new ArgumentException(string.Format(
                "{0} is not a CRM property of entity {1}",
                member.Name, typeof(T).Name));
        }
    }
}
