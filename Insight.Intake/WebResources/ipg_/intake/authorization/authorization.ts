namespace Intake.Authorization {
  export async function OnLoad(executionContext): Promise<void> {
    var formContext: Xrm.FormContext = executionContext.getFormContext();
    addCarrierLookupCustomView(formContext);
    IsAuthRequiredChanged(executionContext);
    disableAllFields(executionContext);
    checkIfDefault(formContext);
    setDefaultValuesByCarrierPlanType(formContext);
   
  }
  async function checkIfDefault(form: Xrm.FormContext) {
    if (!form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_incidentid").getValue()) {
      return;
    }
    const incidentId = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_incidentid").getValue()[0].id;

    const existingAuthRecords = await Xrm.WebApi.retrieveMultipleRecords("ipg_authorization", `?$select=ipg_facilityauthnumber,_createdby_value&$filter=_ipg_incidentid_value eq ${incidentId}`);
    if (existingAuthRecords.entities.length > 0) {
      const systemCreatedRecords = existingAuthRecords
        .entities
        .filter(p => p["_createdby_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "system");

     // if (systemCreatedRecords.length > 0) {
       // const authNumber = systemCreatedRecords[0]["ipg_facilityauthnumber"];
      //  form.getAttribute("ipg_facilityauthnumber")?.setValue(authNumber);
     // }
    }
  }
  export function IsAuthRequiredChanged(executionContext): void {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    const isAuthRquired = formContext.getAttribute("ipg_isauthrequired")?.getValue();

    if (isAuthRquired) {
      formContext.getAttribute("ipg_callreference")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_csrame")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_csrphone")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_ipgauthnumber")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_autheffectivedate")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_authexpirationdate")?.setRequiredLevel("required");
    }
    else {
      formContext.getAttribute("ipg_callreference")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_csrame")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_csrphone")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_ipgauthnumber")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_autheffectivedate")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_authexpirationdate")?.setRequiredLevel("none");
    }
  }

  export function IsSameAsFacilityChanged(executionContext): void {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    const isAuthSameAsFacility = formContext.getAttribute("ipg_isauthsameasfacility")?.getValue();

    if (isAuthSameAsFacility) {
      formContext.getControl("ipg_ipgauthnumber")?.setDisabled(true);
      formContext.getAttribute("ipg_ipgauthnumber")?.setValue(formContext.getAttribute("ipg_facilityauthnumber")?.getValue());
    }
    else {
      formContext.getControl("ipg_ipgauthnumber")?.setDisabled(false);
    }
  }

  export function FacilityAuthChanged(executionContext): void {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    const isAuthSameAsFacility = formContext.getAttribute("ipg_isauthsameasfacility")?.getValue();

    if (isAuthSameAsFacility) {
      formContext.getAttribute("ipg_ipgauthnumber")?.setValue(formContext.getAttribute("ipg_facilityauthnumber")?.getValue());
    }
  }


  export function disableAllFields(executionContext): void {
    const formContext: Xrm.FormContext = executionContext.getFormContext();

    if (formContext.ui.getFormType() == XrmEnum.FormType.Update) {
      formContext.ui.controls.forEach((control: Xrm.Controls.StandardControl) => {
        if (control && control.getDisabled && !control.getDisabled()) {
          control.setDisabled(true);
        }
      });
    }
  }



  async function addCarrierLookupCustomView(formContext: Xrm.FormContext) {
    var carrierControl = <Xrm.Controls.LookupControl>formContext.getControl("ipg_carrierid");
    if (carrierControl) {
      var incidentRef = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_incidentid")?.getValue();
      if (incidentRef && incidentRef.length > 0) {
        var incident = await Xrm.WebApi.retrieveRecord("incident",
          incidentRef[0].id.replace('{', '').replace('}', ''),
          "?$select=_ipg_carrierid_value,_ipg_secondarycarrierid_value");
        var viewId = "00000000-0000-0000-00AA-000010001121";
        var fetchXml = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
  <entity name="account">
    <attribute name="name" />
    <attribute name="accountid" />
    <order attribute="name" descending="false" />
    <filter type="and">
      <filter type="or">`
        if (incident["_ipg_carrierid_value"]) {
          fetchXml += `\n<condition attribute="accountid" operator="eq" value="${incident["_ipg_carrierid_value"]}" />`;
        }
        if (incident["_ipg_secondarycarrierid_value"]) {
          fetchXml += `\n<condition attribute="accountid" operator="eq" uiname="Test" uitype="account" value="${incident["_ipg_secondarycarrierid_value"]}" />`;
        }
        fetchXml += `\n</filter>
      </filter>
    </entity>
  </fetch>`;
        var viewDisplayName = "Carriers";
        var layoutXml = `<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>
      <row name='result' id='accountid'>
      <cell name='name' width='300' />
      </row>
      </grid>`;
        carrierControl.addCustomView(viewId, 'account', viewDisplayName, fetchXml, layoutXml, true);
      }
    }
  }

  async function setDefaultValuesByCarrierPlanType(formContext: Xrm.FormContext){
    var carrierRef = formContext.getAttribute("ipg_carrierid")?.getValue();
    var incidentRef = formContext.getAttribute("ipg_incidentid")?.getValue();
    var carrier = await Xrm.WebApi.retrieveRecord("account", carrierRef[0].id, "?$select=ipg_carriertype");
    var incident = await Xrm.WebApi.retrieveRecord("incident", incidentRef[0].id, "?$select=ipg_surgerydate,ipg_actualdos");
    var dosString = incident["ipg_actualdos"] ?? incident["ipg_surgerydate"];
    var dos = new Date(dosString);
    if (carrier.ipg_carriertype == 427880001){
      formContext.getAttribute("ipg_autheffectivedate").setValue(new Date("1-1-" + dos.getFullYear()));
      formContext.getAttribute("ipg_authexpirationdate").setValue(new Date("12-31-9999"));
    }
  }
}


