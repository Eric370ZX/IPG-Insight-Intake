using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Helpers
{
    public class AttributeValuesBeforeAndAfter
    {
        public object Before { get; set; }
        public object After { get; set; }
    }
    public class PreImagePostImageHelper
    {
        /// <summary>
        /// This method takes Pre Image and Post Image of an entity 
        /// detects if there is a change in values
        /// </summary>
        /// <param name="preImage">The preimage of an entity</param>
        /// <param name="postImage">The post image of an entity</param>
        /// <returns>Returns collection of Entity attributes as keys and 
        /// Collection representing Before and After values. 
        /// After value will be null if key is found in pre image but not on post image</returns>
        public static Dictionary<string,AttributeValuesBeforeAndAfter> EntityPreAndPostImageChangesHelper(Entity preImage, Entity postImage)
        {
            var dictionaryOfAttributeValues = new Dictionary<string, AttributeValuesBeforeAndAfter>();
            if (preImage == null || postImage == null)
            {
                throw new ArgumentException("One of the arguments passed is null");
            }

            if (preImage.LogicalName != postImage.LogicalName)
            {
                throw new Exception("The arguments passed are not of the same Entity");
            }
            if(preImage.Attributes != null)
            {
                foreach(KeyValuePair<string,object> attribute in preImage.Attributes)
                {
                    if (postImage.Attributes.ContainsKey(attribute.Key))
                    {
                        dictionaryOfAttributeValues.Add(attribute.Key,
                            new AttributeValuesBeforeAndAfter
                            {
                                Before = preImage.Attributes[attribute.Key],
                                After = postImage.Attributes[attribute.Key]
                            });
                    }
                    else
                    {
                        dictionaryOfAttributeValues.Add(attribute.Key,
                            new AttributeValuesBeforeAndAfter
                            {
                                Before = preImage.Attributes[attribute.Key],
                                After = null
                            });
                    }
                }

                //add attributes that are not present in pre image
                foreach (KeyValuePair<string, object> attribute in postImage.Attributes)
                {
                    if (!preImage.Attributes.ContainsKey(attribute.Key))
                    {
                        dictionaryOfAttributeValues.Add(attribute.Key,
                            new AttributeValuesBeforeAndAfter
                            {
                                Before = null,
                                After = postImage.Attributes[attribute.Key]
                            });
                    }
                }
            }
            return dictionaryOfAttributeValues;
        }
    }
}
