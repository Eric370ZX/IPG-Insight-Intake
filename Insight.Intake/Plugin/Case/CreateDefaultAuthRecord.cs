using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class CreateDefaultAuthRecord : PluginBase
    {
        public CreateDefaultAuthRecord() : base(typeof(CreateDefaultAuthRecord))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
        }

        void PreOperationCreateHandler(LocalPluginContext localContext)
        {
            var incident = localContext.Target<Incident>();
            if (string.IsNullOrEmpty(incident.ipg_facilityauthnumber))
            {
                return;
            }
            var authId= localContext.SystemOrganizationService.Create(new ipg_authorization()
            {
                ipg_incidentid = incident.ToEntityReference(),
                ipg_carrierid=incident.ipg_CarrierId,
                ipg_procedurenameid=incident.ipg_procedureid,
                ipg_facilityauthnumber=incident.ipg_facilityauthnumber,
                ipg_ipgauthnumber=incident.ipg_ipgauthnumber,
                ipg_AuthEffectiveDate=incident.ipg_autheffectivedate,
                ipg_AuthExpirationDate = incident.ipg_authexpirationdate,
                ipg_csrame = incident.ipg_csrname,
                ipg_csrphone = incident.ipg_csrphone,
                ipg_callreference = incident.ipg_callreference,
            });

            incident.ipg_AuthorizationId = new EntityReference(ipg_authorization.EntityLogicalName, authId);
        }
    }
}
