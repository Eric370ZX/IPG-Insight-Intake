using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Insight.Intake.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        public static int ComputeLevenshteinDistance(this string source, string target)
        {
            int sourceWordCount = (source == null) ? 0 : source.Length;
            int targetWordCount = (target == null) ? 0 :  target.Length;

            if (source == target) return 0;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        public static List<string> GetSubstringDevidedByComma(this string source)
        {
            var substr = new List<string>();

            if (!string.IsNullOrEmpty(source))
            {
                foreach (Match match in Regex.Matches(source, @"[^,\s+]*\([^)]+\)|[^,\s*]+", RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    substr.Add(match.Value);
                }
            }

            return substr;
        }

        public static List<string> GetSubstringDevidedByOrOperator(this string source)
        {
            var substr = new List<string>();

            foreach (Match match in Regex.Matches(source, @"[^|\s*]*", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                substr.Add(match.Value);
            }

            return substr;
        }

        public static List<string> ParseFunctionFieldNameAndParametersFromStr(this string source)
        {
            var substr = new List<string>();

            foreach (Match match in Regex.Matches(source, @"[^\s\(\),]+", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                substr.Add(match.Value);
            }

            return substr;
        }
    }
}
