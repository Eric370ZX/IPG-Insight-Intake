using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Managers
{
    public class AccountManager
    {
        private IOrganizationService _crmService;
        private ITracingService _tracingService;

        public AccountManager(IOrganizationService organizationService, ITracingService tracingService)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;
        }
    }
}
