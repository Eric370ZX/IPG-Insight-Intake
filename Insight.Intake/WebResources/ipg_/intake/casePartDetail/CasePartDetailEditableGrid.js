/**
 * @namespace Intake.CasePartDetail.EditableGrid
 */
var Intake;
(function (Intake) {
    var CasePartDetail;
    (function (CasePartDetail) {
        var EditableGrid;
        (function (EditableGrid) {
            /**
             * Called on salect row in grid
             * @function Intake.CasePartDetail.EditableGrid.OnSelectRow
             * @returns {void}
            */
            function OnSelectRow(executionContext) {
                var entityObject = executionContext.getFormContext().data.entity;
                var potype = entityObject.attributes.get("ipg_potypecode");
            }
            EditableGrid.OnSelectRow = OnSelectRow;
            function OnChange(executionContext) {
                var entityObject = executionContext.getFormContext().data.entity;
                var potype = entityObject.attributes.get("ipg_potypecode");
                var potypevalue = potype.getValue();
                var potypecontrol = potype.controls.getByIndex(0);
                if (potypevalue === 923720001) {
                    var acctionCollection = {
                        actions: null
                    };
                    acctionCollection.actions = [function () {
                            potypecontrol.clearNotification("1");
                        }];
                    potypecontrol.addNotification({
                        messages: ["Select another option!"],
                        notificationLevel: "ERROR",
                        uniqueId: "1",
                        actions: [acctionCollection]
                    });
                    return false;
                }
                else {
                    potypecontrol.clearNotification("1");
                }
            }
            EditableGrid.OnChange = OnChange;
            function OnSave(executionContext) {
            }
            EditableGrid.OnSave = OnSave;
            /**
            * set available PO Types by Surgery Date and DTM Member of Facility
            * @function Intake.CasePartDetail.SetAvailablePOTypes
            * @returns {void}
            */
            var defaultPOTypeOptions = null;
            function SetAvailablePOTypes(formContext) {
                var caseValue = formContext.getAttribute("ipg_caseid").getValue();
                var productValue = formContext.getAttribute("ipg_productid").getValue();
                var poTypepPickList = formContext.getControl("ipg_potypecode");
                if (poTypepPickList)
                    defaultPOTypeOptions = poTypepPickList.getOptions();
                if (!productValue && poTypepPickList) {
                    poTypepPickList.clearOptions();
                    return;
                }
                if (poTypepPickList) {
                    var currentPOType = Number(formContext.getAttribute("ipg_potypecode").getValue());
                    if (currentPOType != 923720003)
                        poTypepPickList.removeOption(923720003);
                }
                if (caseValue && caseValue.length && caseValue[0] && caseValue[0].id) {
                    Xrm.WebApi.retrieveRecord(caseValue[0].entityType, caseValue[0].id, "?$select=ipg_surgerydate&$expand=ipg_FacilityId($select=ipg_dtmmember)")
                        .then(function (caseResult) {
                        Xrm.WebApi.retrieveRecord(productValue[0].entityType, productValue[0].id, "?$select=ipg_boxquantity")
                            .then(function (productResult) {
                            var excludedOptions = null;
                            if (caseResult) {
                                var dateNow = new Date();
                                var currentDate = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate());
                                if (caseResult.ipg_surgerydate && new Date(caseResult.ipg_surgerydate) < currentDate) {
                                    if (productResult && productResult.ipg_boxquantity > 1) {
                                        excludedOptions = [923720000, 923720001, 923720004];
                                    }
                                    else {
                                        if (caseResult.ipg_FacilityId && caseResult.ipg_FacilityId.ipg_dtmmember)
                                            excludedOptions = [923720000];
                                        else
                                            excludedOptions = [923720000, 923720001, 923720004];
                                    }
                                }
                                else
                                    excludedOptions = [923720001, 923720002, 923720004];
                                if (poTypepPickList && excludedOptions) {
                                    excludedOptions.forEach(function (item) {
                                        poTypepPickList.removeOption(item);
                                    });
                                }
                            }
                        });
                    });
                }
            }
        })(EditableGrid = CasePartDetail.EditableGrid || (CasePartDetail.EditableGrid = {}));
    })(CasePartDetail = Intake.CasePartDetail || (Intake.CasePartDetail = {}));
})(Intake || (Intake = {}));
