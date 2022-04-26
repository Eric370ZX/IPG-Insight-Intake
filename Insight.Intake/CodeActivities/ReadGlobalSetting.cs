using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.CodeActivities
{
    public class ReadGlobalSetting: CodeActivity
    {
        [Input("Name")]
        [RequiredArgument]
        public InArgument<string> Name { get; set; }
        [Output("Value")]
        public OutArgument<string> Value { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService crmService = serviceFactory.CreateOrganizationService(Guid.Empty);
            var settingName = Name.Get(context);
            var value= crmService.GetGlobalSettingValueByKey(settingName);
            Value.Set(context, value);
        }
    }
}
