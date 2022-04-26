/**
 * @namespace Intake.ClaimGenerationParameters
 */
var Intake;
(function (Intake) {
    var ClaimGenerationParameters;
    (function (ClaimGenerationParameters) {
        function initiallizeQuickCreateForm() {
            Xrm.Utility.showProgressIndicator("Loading...");
            var primaryCarrierId = Xrm.Page.context.getQueryStringParameters().ipg_primarycarrierid;
            var secondaryCarrierId = Xrm.Page.context.getQueryStringParameters().ipg_secondarycarrierid;
            var useSecondaryCarrier = Xrm.Page.context.getQueryStringParameters().ipg_usesecondarycarrier;
            var caseId = Xrm.Page.context.getQueryStringParameters().ipg_caseid;
            var primaryClaimType;
            var secondaryClaimType;
            var primaryIcn;
            var secondaryIcn;
            //window.parent.document.getElementById("quickCreateSaveAndCloseBtn").firstChild.textContent = "Ok";
            if (primaryCarrierId != null) {
                Xrm.WebApi.retrieveRecord("account", primaryCarrierId, "?$select=ipg_claimtype")
                    .then(function (response) {
                    primaryClaimType = response.ipg_claimtype;
                    Xrm.Page.getAttribute("ipg_primaryclaimtype").setValue(primaryClaimType);
                })
                    .then(function () {
                    var fetchXml = "<fetch top='1'>\n              <entity name='invoice'>\n                <attribute name='ipg_icn' />\n                <filter type='and'>\n                  <condition attribute='ipg_claimtypecode' operator='eq' value='" + primaryClaimType + "'/>\n                  <condition attribute='ipg_ismanuallysubmitted' operator='eq' value='1'/>\n                  <condition attribute='ipg_caseid' operator='eq' value='" + caseId + "'/>\n                </filter>\n                <order attribute='createdon' descending='true' />\n              </entity>\n            </fetch>";
                    fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
                    Xrm.WebApi.retrieveMultipleRecords("invoice", fetchXml)
                        .then(function (response) {
                        if (response && response.entities.length > 0) {
                            primaryIcn = response.entities[0].ipg_icn;
                            Xrm.Page.getAttribute("ipg_primaryicn").setValue(primaryIcn);
                        }
                        Xrm.Page.getAttribute("ipg_claimtype").setValue(primaryClaimType);
                        Xrm.Page.getAttribute("ipg_icn").setValue(primaryIcn);
                    });
                })
                    .then(function () {
                    if (useSecondaryCarrier == true) {
                        Xrm.WebApi.retrieveRecord("account", secondaryCarrierId, "?$select=ipg_claimtype")
                            .then(function (response) {
                            secondaryClaimType = response.ipg_claimtype;
                            Xrm.Page.getAttribute("ipg_secondaryclaimtype").setValue(secondaryClaimType);
                            Xrm.Page.getControl("ipg_carrier").setVisible(true);
                            Xrm.Page.getControl("ipg_primarycarrierid").setVisible(true);
                            Xrm.Page.getControl("ipg_secondarycarrierid").setVisible(true);
                        })
                            .then(function () {
                            var fetchXml = "<fetch top='1'>\n                      <entity name='invoice'>\n                        <attribute name='ipg_icn' />\n                        <filter type='and'>\n                          <condition attribute='ipg_claimtypecode' operator='eq' value='" + secondaryClaimType + "'/>\n                          <condition attribute='ipg_ismanuallysubmitted' operator='eq' value='1'/>\n                          <condition attribute='ipg_caseid' operator='eq' value='" + caseId + "'/>\n                        </filter>\n                        <order attribute='createdon' descending='true' />\n                      </entity>\n                    </fetch>";
                            fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
                            Xrm.WebApi.retrieveMultipleRecords("invoice", fetchXml)
                                .then(function (response) {
                                if (response && response.entities.length > 0) {
                                    secondaryIcn = response.entities[0].ipg_icn;
                                    Xrm.Page.getAttribute("ipg_secondaryicn").setValue(secondaryIcn);
                                }
                            });
                        });
                    }
                });
            }
            Xrm.Utility.closeProgressIndicator();
        }
        ClaimGenerationParameters.initiallizeQuickCreateForm = initiallizeQuickCreateForm;
    })(ClaimGenerationParameters = Intake.ClaimGenerationParameters || (Intake.ClaimGenerationParameters = {}));
})(Intake || (Intake = {}));
