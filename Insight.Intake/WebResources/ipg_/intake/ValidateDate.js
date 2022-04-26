/**
 * @namespace Intake.Utility
 * @example
 * // Configuration example:
 * '{
 *    "dateFieldKey": "field_key",
 *    "conditions": [
 *      {
 *        "operator": "==" | "<" | ">" | "<=" | ">=",
 *        "operand": "%nd" | "%nm" | "%ny" | "-%nd" | "-%nm" | "-%ny" | "+%nd" | "+%nm" | "+%ny",
 *        "message": "ERROR_MESSAGE"
 *      }
 *    ]
 * }'
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        Utility.invalidDateNotificationKey = 'INVALID_DATE';
        var Operator;
        (function (Operator) {
            Operator["Equal"] = "==";
            Operator["LessThen"] = "<";
            Operator["GreaterThen"] = ">";
            Operator["LessThenEqual"] = "<=";
            Operator["GreaterThenEqual"] = ">=";
        })(Operator = Utility.Operator || (Utility.Operator = {}));
        var OperandOperator;
        (function (OperandOperator) {
            OperandOperator["PlusSign"] = "+";
            OperandOperator["MinusSign"] = "-";
        })(OperandOperator = Utility.OperandOperator || (Utility.OperandOperator = {}));
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.ValidateDate
         * @returns {void}
         */
        function ValidateDate(jsonString) {
            if (!Intake.Utility.isJson(jsonString)) {
                Xrm.Navigation.openErrorDialog({
                    message: 'JSON parameter cannot be parsed',
                    details: 'Intake.Utility.ValidateDate(jsonString: ?)',
                });
            }
            var configuration = JSON.parse(jsonString);
            var dateControl = configuration.dateFieldKey && Xrm.Page.getControl(configuration.dateFieldKey);
            var dateAttribute = dateControl && dateControl.getAttribute();
            if (!dateAttribute) {
                return;
            }
            // parseOperand parses and returns a new instance of Date based on the passed operand.
            // If operand equals today, then returns today date.
            // If operand equals -%nd, then returns today Date - %n days.
            // If operand equals +%nd or %nd, then returns today Date + %n days.
            // If operand equals -%nm, then returns today Date - %n months.
            // If operand equals +%nm or %nm, then returns today Date + %n months.
            // If operand equals -%ny, then returns today Date - %n years.
            // If operand equals +%ny or %ny, then returns today Date + %n years.
            function parseOperand(originalOperand) {
                if (!originalOperand) {
                    return;
                }
                var todayDate = new Date();
                // Clear hours
                todayDate.setHours(0, 0, 0, 0);
                originalOperand = originalOperand.toLowerCase();
                // Return today date if operand equals today
                if (originalOperand === 'today') {
                    return todayDate;
                }
                // Prepare operands
                var operands = originalOperand.split(/\s+/g);
                for (var index = 0, length_1 = operands.length; index < length_1; index++) {
                    var operand = operands[index];
                    // Prepare operator
                    var operator = OperandOperator.PlusSign;
                    if (operand[0] === OperandOperator.PlusSign || operand[0] === OperandOperator.MinusSign) {
                        operator = operand.substring(0, 1);
                        operand = operand.substring(1);
                    }
                    // Prepare operator string
                    var extractedNumberString = operand.replace(/[^\d]+/g, '');
                    var extractedText = operand.replace(extractedNumberString, '');
                    var extractedNumber = Number(extractedNumberString);
                    // Update date based on operand criteria
                    switch (extractedText) {
                        case 'd':
                            switch (operator) {
                                case OperandOperator.PlusSign:
                                    todayDate.setDate(todayDate.getDate() + extractedNumber);
                                    break;
                                case OperandOperator.MinusSign:
                                    todayDate.setDate(todayDate.getDate() - extractedNumber);
                                    break;
                            }
                            break;
                        case 'm':
                            switch (operator) {
                                case OperandOperator.PlusSign:
                                    todayDate.setMonth(todayDate.getMonth() + extractedNumber);
                                    break;
                                case OperandOperator.MinusSign:
                                    todayDate.setMonth(todayDate.getMonth() - extractedNumber);
                                    break;
                            }
                            break;
                        case 'y':
                            switch (operator) {
                                case OperandOperator.PlusSign:
                                    todayDate.setFullYear(todayDate.getFullYear() + extractedNumber);
                                    break;
                                case OperandOperator.MinusSign:
                                    todayDate.setFullYear(todayDate.getFullYear() - extractedNumber);
                                    break;
                            }
                            break;
                    }
                }
                return todayDate;
            }
            // Listener
            function changeListener() {
                var uniqueNotificationId = Utility.invalidDateNotificationKey + "_" + dateAttribute.getName();
                var originalDate = dateAttribute.getValue();
                dateControl.clearNotification(uniqueNotificationId);
                if (!originalDate) {
                    return;
                }
                // Clear hours
                originalDate.setHours(0, 0, 0, 0);
                // Prepare conditions
                var conditions = configuration.conditions.map(function (condition) {
                    return {
                        operator: condition.operator,
                        operand: parseOperand(condition.operand),
                        message: condition.message,
                    };
                });
                // Remove empty conditions
                conditions = conditions.filter(function (condition) { return Boolean(condition.operand); });
                // Start validation
                for (var index = 0, length_2 = conditions.length; index < length_2; index++) {
                    var condition = conditions[index];
                    var operand = condition.operand;
                    var message = void 0;
                    switch (condition.operator) {
                        case Operator.Equal:
                            if (originalDate.getTime() === operand.getTime()) {
                                message = condition.message;
                            }
                            break;
                        case Operator.GreaterThen:
                            if (originalDate.getTime() > operand.getTime()) {
                                message = condition.message;
                            }
                            break;
                        case Operator.GreaterThenEqual:
                            if (originalDate.getTime() >= operand.getTime()) {
                                message = condition.message;
                            }
                            break;
                        case Operator.LessThen:
                            if (originalDate.getTime() < operand.getTime()) {
                                message = condition.message;
                            }
                            break;
                        case Operator.LessThenEqual:
                            if (originalDate.getTime() <= operand.getTime()) {
                                message = condition.message;
                            }
                            break;
                        default:
                            continue;
                    }
                    if (!message) {
                        continue;
                    }
                    var notificationConfiguration = {
                        notificationLevel: 'ERROR',
                        messages: [message],
                        uniqueId: uniqueNotificationId,
                    };
                    dateControl.addNotification(notificationConfiguration);
                }
            }
            dateAttribute.addOnChange(changeListener);
        }
        Utility.ValidateDate = ValidateDate;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
