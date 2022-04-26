/**
 * @namespace Intake.ClaimResponseBatch
 */
var Intake;
(function (Intake) {
    var ClaimResponseBatch;
    (function (ClaimResponseBatch) {
        var disabledFields = [];
        /**
        * Called on Form Load event
        * @function Intake.ClaimResponseBatch.OnLoadForm
        * @returns {void}
      */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var isManualBatchAttr = formContext.getAttribute("ipg_ismanualbatch");
            var controls = formContext.ui.controls.get();
            for (var i in controls) {
                var control = controls[i];
                if (control.getDisabled()) {
                    disabledFields.push(control._controlName);
                }
            }
            if (formContext.ui.getFormType() == 1) {
                setBatchType(formContext);
                if (isManualBatchAttr != null) {
                    isManualBatchAttr.setValue(true);
                }
                var recordCountAttr = formContext.getAttribute("ipg_recordcount");
                if (recordCountAttr != null) {
                    recordCountAttr.setValue(0);
                }
                var appliedAmountAttr = formContext.getAttribute("ipg_appliedamount_new");
                if (appliedAmountAttr != null) {
                    appliedAmountAttr.setValue(0);
                }
            }
            else {
                chooseForm(formContext);
                if (formContext.ui.getFormType() == 2) {
                    SetAvailability(formContext);
                }
            }
            var isManualBatch = isManualBatchAttr != null ? isManualBatchAttr.getValue() : false;
            var tabObj = formContext.ui.tabs.get("GeneralTab");
            if (tabObj != null) {
                var manualBatchHeaderSection = tabObj.sections.get("ManualBatchHeaderSection");
                if (manualBatchHeaderSection != null) {
                    manualBatchHeaderSection.setVisible(isManualBatch);
                }
                var manualBaatchPaymentsSection = tabObj.sections.get("ManualBatchPaymentsSection");
                if (manualBaatchPaymentsSection != null) {
                    manualBaatchPaymentsSection.setVisible(isManualBatch);
                }
                var automaticBaatchSubgridsSection = tabObj.sections.get("AutomaticBatchSubgridsSection");
                if (automaticBaatchSubgridsSection != null) {
                    automaticBaatchSubgridsSection.setVisible(!isManualBatch);
                }
            }
            var bankDateAttr = formContext.getAttribute("ipg_paymentdate");
            if (bankDateAttr != null && !bankDateAttr.getValue()) {
                bankDateAttr.setValue(new Date());
            }
        }
        ClaimResponseBatch.OnLoadForm = OnLoadForm;
        /**
        * Called on Bank Date Change event
        * @function Intake.ClaimResponseBatch.OnChangeBankDate
        * @returns {void}
      */
        function OnChangeBankDate(executionContext) {
            var formContext = executionContext.getFormContext();
            var bankDateAttr = formContext.getAttribute("ipg_paymentdate");
            if (bankDateAttr != null) {
                var bankDate = bankDateAttr.getValue();
                if (bankDate > Date.now()) {
                    Xrm.Utility.alertDialog("Bank date can't be set in the future", function () {
                        bankDateAttr.setValue(new Date());
                    });
                }
            }
        }
        ClaimResponseBatch.OnChangeBankDate = OnChangeBankDate;
        function SetAvailability(formContext) {
            var userId = Intake.Utility.removeCurlyBraces(Xrm.Utility.getGlobalContext().getUserId());
            var ownerId = formContext.getAttribute("ownerid").getValue();
            var controls = formContext.ui.controls.get();
            if (!ownerId) {
                for (var i in controls) {
                    var control = controls[i];
                    if (!control.getDisabled() && (control._controlName != "ownerid")) {
                        control.setDisabled(true);
                    }
                }
            }
            else if (Intake.Utility.removeCurlyBraces(ownerId[0].id) != userId) {
                for (var i in controls) {
                    var control = controls[i];
                    if (!control.getDisabled()) {
                        control.setDisabled(true);
                    }
                }
            }
            else {
                for (var i in controls) {
                    var control = controls[i];
                    if (control.getDisabled() && (!disabledFields.find(function (f) { return f == control._controlName; }))) {
                        control.setDisabled(false);
                    }
                }
            }
        }
        ClaimResponseBatch.SetAvailability = SetAvailability;
        function OnOwnerChange(executionContext) {
            var formContext = executionContext.getFormContext();
            SetAvailability(formContext);
        }
        ClaimResponseBatch.OnOwnerChange = OnOwnerChange;
        /**
        * Called on Claim Response Type change event
        * @function Intake.ClaimResponseBatch.onTypeChange
        * @returns {void}
      */
        function onTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            chooseForm(formContext);
        }
        ClaimResponseBatch.onTypeChange = onTypeChange;
        var batchTypes = [
            { name: 'payment', value: false, formId: 'F4BC4EC3-1EC2-4939-8019-0A0F6169C9A1'.toLocaleLowerCase() },
            { name: 'refund', value: true, formId: 'EC1CA242-DAE9-4EC8-BF96-D2C1449CD8D1'.toLocaleLowerCase() }
        ];
        function setBatchType(formContext) {
            var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
            formContext.getAttribute('ipg_type').setValue(batchTypes.find(function (x) { return x['formId'] == currentFormId; })['value']);
        }
        function chooseForm(formContext) {
            var batchType = formContext.getAttribute("ipg_type").getValue();
            var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
            var currentBatchType = batchTypes.find(function (x) { return x['value'] == batchType; });
            if (currentBatchType != undefined) {
                if (currentFormId != currentBatchType['formId']) {
                    var items = formContext.ui.formSelector.items.get();
                    for (var i in items) {
                        if (items[i].getId() == currentBatchType['formId']) {
                            doNotSubmitChangedAttributes();
                            items[i].navigate();
                        }
                    }
                }
            }
        }
        function doNotSubmitChangedAttributes() {
            var attributes = Xrm.Page.data.entity.attributes.get();
            for (var _i = 0, attributes_1 = attributes; _i < attributes_1.length; _i++) {
                var attr = attributes_1[_i];
                if (attr.getIsDirty()) {
                    attr.setSubmitMode("never");
                }
            }
        }
    })(ClaimResponseBatch = Intake.ClaimResponseBatch || (Intake.ClaimResponseBatch = {}));
})(Intake || (Intake = {}));
