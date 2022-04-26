using System;
using System.Xml;

namespace Insight.Intake.Helpers
{
    class PluginConfiguration
    {
        public static Guid GetConfigDataGuid(XmlDocument doc, string label)
        {
            string tempString = GetValueNode(doc, label);

            if (tempString != string.Empty)
            {
                return new Guid(tempString);
            }

            return Guid.Empty;
        }

        public static bool GetConfigDataBool(XmlDocument doc, string label, bool defaultValue)
        {
            if (bool.TryParse(GetValueNode(doc, label), out var retVar))
            {
                return retVar;
            }

            return defaultValue;
        }

        public static int GetConfigDataInt(XmlDocument doc, string label)
        {
            if (int.TryParse(GetValueNode(doc, label), out var retVar))
            {
                return retVar;
            }

            return -1;
        }

        public static string GetConfigDataString(XmlDocument doc, string label)
        {
            return GetValueNode(doc, label);
        }


        private static string GetValueNode(XmlDocument doc, string key)
        {
            XmlNode node = doc.SelectSingleNode($"Settings/setting[@name='{key}']");

            if (node != null)
            {
                return node.SelectSingleNode("value")?.InnerText;
            }

            return string.Empty;
        }
    }
}