/**
 * @namespace Intake.ChargeCenter
 */
var Intake;
(function (Intake) {
    var ChargeCenter;
    (function (ChargeCenter) {
        /**
          * Called on FormLoad event
          * @function Intake.ChargeCenter.OnFormLoad
          * @returns {void}
        */
        function OnFormLoad(executionContext) {
            disableOrEnableKeywordExceptionsField(executionContext);
        }
        ChargeCenter.OnFormLoad = OnFormLoad;
        /**
          * Called on Supported field Change event
          * @function Intake.ChargeCenter.OnSupportedChange
          * @returns {void}
        */
        function OnSupportedChange(executionContext) {
            disableOrEnableKeywordExceptionsField(executionContext);
        }
        ChargeCenter.OnSupportedChange = OnSupportedChange;
        /**
          * Called on Form Save event
          * @function Intake.ChargeCenter.OnSaveForm
          * @returns {void}
        */
        function OnSaveForm(executionContext) {
            validateRequiredFields(executionContext);
        }
        ChargeCenter.OnSaveForm = OnSaveForm;
        /**
          * Enables or disables KeywordExceptions field based on the value of Supported field
          * @returns {void}
        */
        function disableOrEnableKeywordExceptionsField(executionContext) {
            //I had to use this script instead of a business rule because on 4/5/2019 D365 does not support multi-select option set fields in business rules
            var formContext = executionContext.getFormContext();
            var isSupported = formContext.getAttribute("ipg_supported").getValue();
            if (isSupported) {
                formContext.getControl("ipg_keywordexceptions").setDisabled(true);
                formContext.getAttribute("ipg_keywordexceptions").setValue(null);
            }
            else {
                formContext.getControl("ipg_keywordexceptions").setDisabled(false);
            }
        }
        /**
          * At least one of these fields is required: Part Category, Part Name or HCPCS Code
          * @returns {boolean}
        */
        function validateRequiredFields(executionContext) {
            var formContext = executionContext.getFormContext();
            var partCategoryAttributeValue = formContext.getAttribute("ipg_partcategoryid").getValue();
            var partAttributeValue = formContext.getAttribute("ipg_productid").getValue();
            var hcpcsCodeAttributeValue = formContext.getAttribute("ipg_hcpcscodeid").getValue();
            var notificationId = 'requiredFieldValidation';
            if (!partCategoryAttributeValue && !partAttributeValue && !hcpcsCodeAttributeValue) {
                formContext.ui.setFormNotification('At least one of these fields is required: Part Category, Part Name or HCPCS Code', 'ERROR', notificationId);
                var saveEventArgs = executionContext.getEventArgs();
                saveEventArgs.preventDefault();
                return false;
            }
            formContext.ui.clearFormNotification(notificationId);
            return true;
        }
    })(ChargeCenter = Intake.ChargeCenter || (Intake.ChargeCenter = {}));
})(Intake || (Intake = {}));
