/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        var accountId;
        var titleId;
        function ClaimOnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            CaseIncident(formContext);
        }
        Case.ClaimOnLoad = ClaimOnLoad;
        function CaseIncident(formContext) {
            var carrierObject = Xrm.Page.getAttribute("ipg_caseid").getValue();
            if (carrierObject != null) {
                var carrierId = carrierObject[0];
                var guid = carrierId.id.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("incident", guid, "?$select=title,incidentid&$expand=ipg_CarrierId($select=accountid)").then(function success(results) {
                    accountId = results.ipg_CarrierId.accountid.replace("{", "").replace("}", "");
                    titleId = results.title;
                    CarrierAccount(formContext);
                }, function (error) {
                });
            }
        }
        Case.CaseIncident = CaseIncident;
        function CarrierAccount(formContext) {
            Xrm.WebApi.retrieveRecord("account", accountId, "?$select=name,ipg_claimtype").then(function success(result) {
                formContext.getAttribute("ipg_claimformtype").setValue(result["ipg_claimtype"]);
                formContext.getAttribute("ipg_id").setValue(titleId);
                var object = new Array();
                object[0] = new Object();
                object[0].id = accountId;
                object[0].name = result.name;
                object[0].entityType = "account";
                formContext.getAttribute("ipg_carrierid").setValue(object);
            }, function (error) {
            });
        }
        Case.CarrierAccount = CarrierAccount;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
