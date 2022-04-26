/**
 * @namespace Intake.CasePartDetailRibbon
 */
var Intake;
(function (Intake) {
    var CasePartDetailRibbon;
    (function (CasePartDetailRibbon) {
        function OpenCalcRevHistory(primaryControl) {
            //debugger;
            //get CalcRev application environment suffix
            var env = Intake.Utility.GetEnvironment();
            var envSuffix;
            if (env) {
                envSuffix = '-' + env;
            }
            else {
                envSuffix = '';
            }
            //get incidentId
            var incidentId = primaryControl.data.entity.getId();
            incidentId = incidentId.replace(/[{}]/g, ""); //delete curly brackets
            var windowSizeAndPosition = Intake.Utility.GetCalcRevWindowSizeAndPosition();
            //finally open the window
            window.open("https://insight-calcrev" + envSuffix + ".azurewebsites.net/Case/History#/" + incidentId, 'calcRevWindow', windowSizeAndPosition);
        }
        CasePartDetailRibbon.OpenCalcRevHistory = OpenCalcRevHistory;
        function ShouldEnableCalcRevHistoryButton(primaryControl) {
            //debugger;
            var actualRevenueAttribute = primaryControl.getAttribute("ipg_actualrevenue");
            if (actualRevenueAttribute) {
                var actualRevenueAttributeValue = actualRevenueAttribute.getValue();
                if (typeof actualRevenueAttributeValue !== 'undefined' && actualRevenueAttributeValue != null) {
                    return true;
                }
            }
            return false;
        }
        CasePartDetailRibbon.ShouldEnableCalcRevHistoryButton = ShouldEnableCalcRevHistoryButton;
        function OpenManagePage(primaryControl, primaryRecordId, type) {
            var params = {
                caseId: Intake.Utility.removeCurlyBraces(primaryRecordId),
                isEstimatedParts: 0
            };
            if (type == "Estimated") {
                params.isEstimatedParts = 1;
            }
            if (params.isEstimatedParts == 0) {
                //Call EBV Before Adding/Manage Parts
                var arguments_1 = { IsUserGenerated: false, CarrierNumber: 1 };
                Intake.Utility.CallActionProcessAsync("incidents", Intake.Utility.removeCurlyBraces(primaryRecordId), "ipg_IPGCaseActionsVerifyBenefits", arguments_1);
            }
            var windowParams = {
                width: 1320,
                height: 1024,
                openInNewWindow: true,
            };
            Xrm.Navigation.openWebResource('ipg_/intake/react-pages/case-actual-parts.html', windowParams, JSON.stringify(params));
        }
        CasePartDetailRibbon.OpenManagePage = OpenManagePage;
        function DisplayIcon(rowData, userLCID) {
            var str = JSON.parse(rowData);
            var quantity = str.ipg_quantity_Value || 0;
            var icon = "ipg_/intake/img/warning-yellow-32px.png";
            if (quantity == 0) {
                return [icon];
            }
        }
        CasePartDetailRibbon.DisplayIcon = DisplayIcon;
        function IsCaseInCaseManagement(primaryControl) {
            var _a, _b;
            return ((_a = primaryControl === null || primaryControl === void 0 ? void 0 : primaryControl.data) === null || _a === void 0 ? void 0 : _a.entity.getEntityName()) === "incident" && ((_b = primaryControl.getAttribute("ipg_statecode")) === null || _b === void 0 ? void 0 : _b.getValue()) === 923720002; //Case Management
        }
        CasePartDetailRibbon.IsCaseInCaseManagement = IsCaseInCaseManagement;
    })(CasePartDetailRibbon = Intake.CasePartDetailRibbon || (Intake.CasePartDetailRibbon = {}));
})(Intake || (Intake = {}));
