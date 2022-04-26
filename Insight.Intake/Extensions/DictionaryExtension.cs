using System.Collections.Generic;

namespace Insight.Intake.Extensions
{
    public static class DictionaryExtension
    {
        public static void AddIfNotNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) 
        {
            if(!string.IsNullOrEmpty(value as string))
            {
                if(dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }
        }

        public static TValue GetValueORDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
        }
    }
}
