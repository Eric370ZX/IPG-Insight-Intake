/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        Utility.invalidDateRangeNotificationKey = 'INVALID_DATE_RANGE';
        var ValidationContext;
        (function (ValidationContext) {
            ValidationContext[ValidationContext["Unknown"] = 1] = "Unknown";
            ValidationContext[ValidationContext["FromDateField"] = 2] = "FromDateField";
            ValidationContext[ValidationContext["ToDateField"] = 4] = "ToDateField";
        })(ValidationContext = Utility.ValidationContext || (Utility.ValidationContext = {}));
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.ValidateDateRange
         * @returns {void}
         */
        function ValidateDateRange(jsonString) {
            if (!Intake.Utility.isJson(jsonString)) {
                Xrm.Navigation.openErrorDialog({
                    message: 'JSON parameter cannot be parsed',
                    details: 'Intake.Utility.ValidateDateRange(jsonString: ?)',
                });
            }
            var configuration = JSON.parse(jsonString);
            var fromDateControl = configuration.fromDateFieldKey && Xrm.Page.getControl(configuration.fromDateFieldKey);
            var toDateControl = configuration.toDateFieldKey && Xrm.Page.getControl(configuration.toDateFieldKey);
            var fromDateAttribute = fromDateControl && fromDateControl.getAttribute();
            var toDateAttribute = toDateControl && toDateControl.getAttribute();
            // Listener.
            function changeListener(event) {
                var uniqueNotificationId = Utility.invalidDateRangeNotificationKey + "_" + fromDateAttribute.getName() + "_" + toDateAttribute.getName();
                var originalFromDateValue = fromDateAttribute && fromDateAttribute.getValue();
                var originalToDateValue = toDateAttribute && toDateAttribute.getValue();
                var fromDateValue = originalFromDateValue && new Date(originalFromDateValue);
                var toDateValue = originalToDateValue && new Date(originalToDateValue);
                // Clear notifications.
                fromDateControl.clearNotification(uniqueNotificationId);
                toDateControl.clearNotification(uniqueNotificationId);
                // Drop process if at least one date is empty.
                if (!fromDateValue || !toDateValue) {
                    return;
                }
                // Clear hours.
                fromDateValue.setHours(0, 0, 0, 0);
                toDateValue.setHours(0, 0, 0, 0);
                // Prepare validation context.
                var validationContext = ValidationContext.Unknown;
                var eventSource = event.getEventSource();
                switch (eventSource.getName()) {
                    case fromDateAttribute.getName():
                        validationContext = ValidationContext.FromDateField;
                        break;
                    case toDateAttribute.getName():
                        validationContext = ValidationContext.ToDateField;
                        break;
                }
                // Stop process if context is Unknown.
                if (validationContext == ValidationContext.Unknown) {
                    return;
                }
                // Prepare error message.
                var message;
                if (fromDateValue.getTime() > toDateValue.getTime()) {
                    switch (validationContext) {
                        case ValidationContext.FromDateField:
                            message = fromDateControl.getLabel() + " cannot be greater than " + toDateControl.getLabel();
                            break;
                        case ValidationContext.ToDateField:
                            message = toDateControl.getLabel() + " cannot be less than " + fromDateControl.getLabel();
                            break;
                    }
                }
                // Check equals if necessary.
                if (!configuration.canBeEqual && fromDateValue.getTime() === toDateValue.getTime()) {
                    switch (validationContext) {
                        case ValidationContext.FromDateField:
                            message = fromDateControl.getLabel() + " cannot be equal " + toDateControl.getLabel();
                            break;
                        case ValidationContext.ToDateField:
                            message = toDateControl.getLabel() + " cannot be equal " + fromDateControl.getLabel();
                            break;
                    }
                }
                // Stop process if message is empty.
                if (!message) {
                    return;
                }
                // Show error notification based on the validation context.
                var notificationConfiguration = {
                    notificationLevel: 'ERROR',
                    messages: [message],
                    uniqueId: uniqueNotificationId,
                };
                switch (validationContext) {
                    case ValidationContext.FromDateField:
                        fromDateControl.addNotification(notificationConfiguration);
                        break;
                    case ValidationContext.ToDateField:
                        toDateControl.addNotification(notificationConfiguration);
                        break;
                }
            }
            fromDateAttribute.addOnChange(changeListener);
            toDateAttribute.addOnChange(changeListener);
        }
        Utility.ValidateDateRange = ValidateDateRange;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
