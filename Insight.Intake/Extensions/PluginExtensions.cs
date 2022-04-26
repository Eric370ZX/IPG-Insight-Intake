using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Extensions
{
    public static class PluginExtensions
    {
        public static T deserializeObject<T>(string str)
        {
            using (var ms = str.ToStream())
            {
                try
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    var deserializedObject = (T)ser.ReadObject(ms);
                    ms.Close();
                    return deserializedObject;
                }
                catch {
                    return default(T);
                }
            }
        }

        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
