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
namespace Intake.Utility {
  export const invalidDateNotificationKey = 'INVALID_DATE';
  export enum Operator {
    Equal = '==',
    LessThen = '<',
    GreaterThen = '>',
    LessThenEqual = '<=',
    GreaterThenEqual = '>=',
  }
  export enum OperandOperator {
    PlusSign = '+',
    MinusSign = '-',
  }
  export interface Condition {
    operator: Operator;
    operand: string | Date;
    message: string;
  }
  export interface DateConfiguration {
    dateFieldKey: string;
    conditions: Array<Condition>;
  }
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.ValidateDate
   * @returns {void}
   */
  export function ValidateDate(jsonString: string) {
    if (!Intake.Utility.isJson(jsonString)) {
      Xrm.Navigation.openErrorDialog({
        message: 'JSON parameter cannot be parsed',
        details: 'Intake.Utility.ValidateDate(jsonString: ?)',
      });
    }
    const configuration: DateConfiguration = JSON.parse(jsonString);
    const dateControl: Xrm.Controls.DateControl = configuration.dateFieldKey && Xrm.Page.getControl(configuration.dateFieldKey);
    const dateAttribute: Xrm.Attributes.DateAttribute = dateControl && dateControl.getAttribute();
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
    function parseOperand(originalOperand: string): Date {
      if (!originalOperand) {
        return;
      }
      const todayDate = new Date();
      // Clear hours
      todayDate.setHours(0, 0, 0, 0);
      originalOperand = originalOperand.toLowerCase();
      // Return today date if operand equals today
      if (originalOperand === 'today') {
        return todayDate;
      }
      // Prepare operands
      const operands: Array<string> = originalOperand.split(/\s+/g);
      for (let index = 0, length = operands.length; index < length; index++) {
        let operand = operands[index];
        // Prepare operator
        let operator: OperandOperator | string = OperandOperator.PlusSign;
        if (operand[0] === OperandOperator.PlusSign || operand[0] === OperandOperator.MinusSign) {
          operator = operand.substring(0, 1);
          operand = operand.substring(1);
        }
        // Prepare operator string
        const extractedNumberString: string = operand.replace(/[^\d]+/g, '');
        const extractedText: string = operand.replace(extractedNumberString, '');
        const extractedNumber: number = Number(extractedNumberString);
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
      const uniqueNotificationId: string = `${invalidDateNotificationKey}_${dateAttribute.getName()}`;
      const originalDate: Date = dateAttribute.getValue();
      dateControl.clearNotification(uniqueNotificationId);
      if (!originalDate) {
        return;
      }
      // Clear hours
      originalDate.setHours(0, 0, 0, 0);
      // Prepare conditions
      let conditions: Array<Condition> = configuration.conditions.map((condition: Condition): Condition => {
        return {
          operator: condition.operator,
          operand: parseOperand(condition.operand as string),
          message: condition.message,
        };
      });
      // Remove empty conditions
      conditions = conditions.filter((condition: Condition) => Boolean(condition.operand));
      // Start validation
      for (let index = 0, length = conditions.length; index < length; index++) {
        const condition: Condition = conditions[index];
        const operand: Date = condition.operand as Date;
        let message: string;
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
        const notificationConfiguration: Xrm.Controls.AddControlNotificationOptions = {
          notificationLevel: 'ERROR',
          messages: [ message ],
          uniqueId: uniqueNotificationId,
        };
        dateControl.addNotification(notificationConfiguration);
      }
    }
    dateAttribute.addOnChange(changeListener);
  }
}
