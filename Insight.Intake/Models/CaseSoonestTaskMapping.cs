using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Models
{
    public class CaseSoonestTaskMapping
    {
        public string CheckField { get; set; }

        public object[] CheckFieldValues { get; set; }

        public string FillInField { get; set; }

        public CaseSoonestTaskMapping() { }

        public CaseSoonestTaskMapping(string checkField, object checkFieldValue, string fillInField)
        {
            CheckField = checkField;
            CheckFieldValues = new[] { checkFieldValue };
            FillInField = fillInField;
        }

        public bool IsContainsValue(object value)
        {
            switch (value)
            {
                case EntityReference recordRef when recordRef != null:
                    return CheckFieldValues.Any(v => v is Guid id && recordRef.Id == id);
                case string text:
                    return CheckFieldValues.Any(v => v is string textValue && string.Equals(text, textValue));
                case int number:
                    return CheckFieldValues.Any(v => v is int intValue && number == intValue);
                case decimal number:
                    return CheckFieldValues.Any(v => v is decimal decimalValue && number == decimalValue);
            }

            return false;
        }
    }
}
