using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Extensions
{
    public static class OptionSetValueExtensions
    {
        public static TEnum? ToEnum<TEnum>(this OptionSetValue optionSet) where TEnum : struct
        {
            TEnum? result = null;

            if (optionSet != null)
            {
                result = (TEnum?)Enum.ToObject(typeof(TEnum), optionSet.Value);
            }

            return result;
        }

        public static object ToEnum(this OptionSetValue optionSet, Type enumType)
        {
            object result = null;

            if (optionSet != null)
            {
                result = Enum.ToObject(enumType, optionSet.Value);
            }

            return result;
        }

        public static OptionSetValue ToOptionSetValue<T>(this T? @enum) where T : struct
        {
            return @enum.HasValue ? ToOptionSetValue(@enum.Value) : null;
        }

        public static OptionSetValue ToOptionSetValue<T>(this T @enum) where T : struct
        {
            return new OptionSetValue((int)(ValueType)@enum);
        }

        public static bool TestCondition(this ipg_Conditions? condition, decimal? targetValue, decimal? conditionValue, decimal? conditionValue2 = null)
        {
            if (!condition.HasValue)
            {
                return true;
            }

            switch (condition)
            {
                case ipg_Conditions.Equal:
                    return targetValue == conditionValue;

                case ipg_Conditions.GreaterThan:
                    return targetValue > conditionValue;

                case ipg_Conditions.LessThan:
                    return targetValue < conditionValue;

                case ipg_Conditions.GreaterEqual:
                    return targetValue >= conditionValue;

                case ipg_Conditions.LessEqual:
                    return targetValue <= conditionValue;
                case ipg_Conditions.Between:
                    return conditionValue2.HasValue ? targetValue > conditionValue && targetValue < conditionValue2 : throw new NotSupportedException("Unexpected border with condition : " + condition);
                default:
                    throw new NotSupportedException("Unexpected condition: " + condition);
            }
        }
    }
}
