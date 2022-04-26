namespace Intake.CaseAuthorization {
  enum CarrierType {
    Carrier1 = 1,
    Carrier2 = 2,
    //Tertiary = 2 not needed anymore
  }

  const IncidentCarrierTypeMapping = {
    [CarrierType.Carrier1]: "ipg_carrierid",
    [CarrierType.Carrier2]: "ipg_secondarycarrierid",
    //[CarrierType.Tertiary]: "ipg_carrierid" not needed anymore
  };

  const _incidentAuthFields = [
    "title", "_ipg_carrierid_value", "_ipg_procedureid_value",
    "_ipg_homeplancarrierid_value", "_ipg_secondarycarrierid_value",
    "_ipg_autocarrierid_value", "ipg_is_authorization_required"
  ];

  const _authorizationRecentFields = [
    "ipg_facilityauthnumber", "_createdby_value",
    "ipg_ipgauthnumber"
  ];
  
  var Xrm: Xrm.XrmStatic = Xrm || parent.Xrm;

  export async function Init() {
    const isLocked: boolean = Xrm.Page.getAttribute("ipg_islocked")?.getValue();

    //@ts-ignore
    document.getElementById('obtainAuthorization').disabled = isLocked;
  }
  
  export async function ObtainCarrierAuthDefault(): Promise<any> {
    const params = Intake.Utility.GetWindowJsonParameters();

    ObtainCarrierAuth(params.recordId, params.carrierType ?? 1);
  }

  export async function ObtainCarrierAuth(incidentId: string, carrierType: CarrierType): Promise<void> {
    var Xrm = Xrm || parent.Xrm;
    Xrm.Utility.showProgressIndicator("Preparing the form");

    const incident = await GetCaseDataForAuth(incidentId);

    const incidentRef = {
      entityType: "incident",
      id: incidentId,
      name: incident.title
    };

    const carrierId = incident[`_${IncidentCarrierTypeMapping[carrierType]}_value`];
    const carrierName = incident[`_${IncidentCarrierTypeMapping[carrierType]}_value@OData.Community.Display.V1.FormattedValue`];

    const carrierRef = {
      entityType: "account",
      id: carrierId,
      name: carrierName
    };

    const procedureNameRef = {
      entityType: "ipg_procedurename",
      id: incident["_ipg_procedureid_value"],
      name: incident["_ipg_procedureid_value@OData.Community.Display.V1.FormattedValue"]
    };

    let formParameters = {
      ["ipg_carrierid"]: carrierRef,
      ["ipg_procedurenameid"]: procedureNameRef,
      ["ipg_isauthrequired"]: incident["ipg_is_authorization_required"]
    };

    const recentAuth = await GetRecentAuthRecord(incidentId, carrierId);

   
    if (recentAuth) {
      formParameters["ipg_facilityauthnumber"] = recentAuth.ipg_facilityauthnumber;

      if ((recentAuth["_createdby_value@OData.Community.Display.V1.FormattedValue"] as string).toLowerCase() != 'system') {
        formParameters["ipg_facilityauthnumber"] =null;
       // formParameters["ipg_facilityauthnumber"] = recentAuth.ipg_facilityauthnumber;
       
      }
      
    }
    else {
      if (incident.ipg_incident_ipg_referral_AssociatedCaseId &&
        incident.ipg_incident_ipg_referral_AssociatedCaseId.length > 0) {
        formParameters["ipg_facilityauthnumber"] = incident.ipg_incident_ipg_referral_AssociatedCaseId[0].ipg_facility_auth;
      }
    }

    Xrm.Navigation.navigateTo({
      pageType: "entityrecord",
      entityName: "ipg_authorization",
      formId: "42b37e40-4ed6-402c-a87f-737cd05825ba",
      createFromEntity: incidentRef,
      data: formParameters
    }, {
      width: window.screen.width * 0.75,
      height: window.screen.height * 0.75,
      position: 1,
      target: 2 //dialog
    }).then(function (res: any) {
      setTimeout(_ => {
        //debugger;
        if (res.savedEntityReference && res.savedEntityReference.length) {
          
          //new authorization has have been associated in CaseRecentAuthorization plugin and will be displayed after saving the case
          Xrm.Page.data.refresh(true);
        }

      }, 250);
    }, error => {
      console.log(error);
    });

    Xrm.Utility.closeProgressIndicator();
  }

  async function GetCaseDataForAuth(incidentId: string): Promise<any> {
    incidentId = incidentId.replace("{", "").replace("}", "");

    const select = `$select=${_incidentAuthFields.join(",")}`;
    const expand = `$expand=ipg_incident_ipg_referral_AssociatedCaseId($select=ipg_facility_auth)`;
    const url = `/api/data/v9.1/incidents(${incidentId})?${select}&${expand}`;

    const response = await fetch(url, {
      method: "GET",
      headers: {
        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
      }
    });

    return await response.json();
  }

  async function GetRecentAuthRecord(incidentId: string, carrierId: string): Promise<any> {
    incidentId = incidentId.replace("{", "").replace("}", "");
    carrierId = carrierId.replace("{", "").replace("}", "");

    const filter = `$filter=_ipg_incidentid_value eq '${incidentId}' and _ipg_carrierid_value eq '${carrierId}' and statecode eq 0`;
    const orderby = `$orderby=createdon desc`;
    const top = `$top=1`;
    const select = `$select=${_authorizationRecentFields.join(",")}`;

    const url = `/api/data/v9.1/ipg_authorizations?${filter}&${orderby}&${top}&${select}`;

    const response = await fetch(url, {
      method: "GET",
      headers: {
        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
      }
    });

    const records = await response.json();
    if (records && records.value && records.value.length > 0) {
      return records.value[0];
    }

    return null;
  }
}
