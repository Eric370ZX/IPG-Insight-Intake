using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.ImportantEventsLog.Helpers
{
    public class QueryVisitor : QueryExpressionVisitorBase
    {
        public string CaseId { get; private set; }
        public string ReferralId { get; private set; }
        protected override ConditionExpression VisitConditionExpression(ConditionExpression condition)
        {
            if (condition.Values.Count > 0)
            {
                var value = condition.Values[0].ToString();
                switch (condition.AttributeName)
                {
                    case ipg_importanteventslog.Fields.ipg_caseid:
                        CaseId = value;
                        break;
                    case ipg_importanteventslog.Fields.ipg_referralid:
                        ReferralId = value;
                        break;
                    default:
                        break;
                }
            }

            return base.VisitConditionExpression(condition);
        }
    }
}
