/**
 * @namespace Intake.ClaimGenerationParameters
 */

namespace Intake.ClaimGenerationParameters {
  export function initiallizeQuickCreateForm() {
    Xrm.Utility.showProgressIndicator("Loading...");
    const primaryCarrierId = Xrm.Page.context.getQueryStringParameters().ipg_primarycarrierid;
    const secondaryCarrierId = Xrm.Page.context.getQueryStringParameters().ipg_secondarycarrierid;
    const useSecondaryCarrier = Xrm.Page.context.getQueryStringParameters().ipg_usesecondarycarrier;
    const caseId = Xrm.Page.context.getQueryStringParameters().ipg_caseid;
    let primaryClaimType;
    let secondaryClaimType;
    let primaryIcn;
    let secondaryIcn;
   
    //window.parent.document.getElementById("quickCreateSaveAndCloseBtn").firstChild.textContent = "Ok";

    if (primaryCarrierId != null) {
      Xrm.WebApi.retrieveRecord("account", primaryCarrierId, "?$select=ipg_claimtype")
        .then((response): void => {
          primaryClaimType = response.ipg_claimtype;
          Xrm.Page.getAttribute("ipg_primaryclaimtype").setValue(primaryClaimType);
        })
        .then((): void => {
          let fetchXml = `<fetch top='1'>
              <entity name='invoice'>
                <attribute name='ipg_icn' />
                <filter type='and'>
                  <condition attribute='ipg_claimtypecode' operator='eq' value='${primaryClaimType}'/>
                  <condition attribute='ipg_ismanuallysubmitted' operator='eq' value='1'/>
                  <condition attribute='ipg_caseid' operator='eq' value='${caseId}'/>
                </filter>
                <order attribute='createdon' descending='true' />
              </entity>
            </fetch>`;
          fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
          Xrm.WebApi.retrieveMultipleRecords("invoice", fetchXml)
            .then((response): void => {
              if (response && response.entities.length > 0) {
                primaryIcn = response.entities[0].ipg_icn;
                Xrm.Page.getAttribute("ipg_primaryicn").setValue(primaryIcn);
              }
              Xrm.Page.getAttribute("ipg_claimtype").setValue(primaryClaimType);
              Xrm.Page.getAttribute("ipg_icn").setValue(primaryIcn);
            });
        })
        .then((): void => {
          if (useSecondaryCarrier == true) {
            Xrm.WebApi.retrieveRecord("account", secondaryCarrierId, "?$select=ipg_claimtype")
            .then((response): void => {
              secondaryClaimType = response.ipg_claimtype;
              Xrm.Page.getAttribute("ipg_secondaryclaimtype").setValue(secondaryClaimType);
              Xrm.Page.getControl("ipg_carrier").setVisible(true);
              Xrm.Page.getControl("ipg_primarycarrierid").setVisible(true);
              Xrm.Page.getControl("ipg_secondarycarrierid").setVisible(true);
            })
            .then((): void => {
              let fetchXml = `<fetch top='1'>
                      <entity name='invoice'>
                        <attribute name='ipg_icn' />
                        <filter type='and'>
                          <condition attribute='ipg_claimtypecode' operator='eq' value='${secondaryClaimType}'/>
                          <condition attribute='ipg_ismanuallysubmitted' operator='eq' value='1'/>
                          <condition attribute='ipg_caseid' operator='eq' value='${caseId}'/>
                        </filter>
                        <order attribute='createdon' descending='true' />
                      </entity>
                    </fetch>`;
              fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
              Xrm.WebApi.retrieveMultipleRecords("invoice", fetchXml)
                .then((response): void => {
                  if (response && response.entities.length > 0) {
                    secondaryIcn = response.entities[0].ipg_icn;
                    Xrm.Page.getAttribute("ipg_secondaryicn").setValue(secondaryIcn);
                  }
                })
            });
          }
        });
    }
    Xrm.Utility.closeProgressIndicator();
  }
}
