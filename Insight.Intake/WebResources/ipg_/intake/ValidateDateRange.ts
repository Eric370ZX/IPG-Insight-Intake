/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  export const invalidDateRangeNotificationKey = 'INVALID_DATE_RANGE';
  export interface DateRangeConfiguration {
    fromDateFieldKey: string;
    toDateFieldKey: string;
    canBeEqual: boolean;
  }
  export enum ValidationContext {
    Unknown = 1 << 0,
    FromDateField = 1 << 1,
    ToDateField = 1 << 2,
  }
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.ValidateDateRange
   * @returns {void}
   */
  export function ValidateDateRange(jsonString: string) {
    if (!Intake.Utility.isJson(jsonString)) {
      Xrm.Navigation.openErrorDialog({
        message: 'JSON parameter cannot be parsed',
        details: 'Intake.Utility.ValidateDateRange(jsonString: ?)',
      });
    }
    const configuration: DateRangeConfiguration = JSON.parse(jsonString);
    const fromDateControl: Xrm.Controls.DateControl = configuration.fromDateFieldKey && Xrm.Page.getControl(configuration.fromDateFieldKey);
    const toDateControl: Xrm.Controls.DateControl = configuration.toDateFieldKey && Xrm.Page.getControl(configuration.toDateFieldKey);
    const fromDateAttribute: Xrm.Attributes.DateAttribute = fromDateControl && fromDateControl.getAttribute();
    const toDateAttribute: Xrm.Attributes.DateAttribute = toDateControl && toDateControl.getAttribute();
    // Listener.
    function changeListener(event: Xrm.Events.EventContext) {
      const uniqueNotificationId = `${invalidDateRangeNotificationKey}_${fromDateAttribute.getName()}_${toDateAttribute.getName()}`;
      const originalFromDateValue: Date = fromDateAttribute && fromDateAttribute.getValue();
      const originalToDateValue: Date = toDateAttribute && toDateAttribute.getValue();
      const fromDateValue = originalFromDateValue && new Date(originalFromDateValue);
      const toDateValue = originalToDateValue && new Date(originalToDateValue);
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
      let validationContext: ValidationContext = ValidationContext.Unknown;
      const eventSource = event.getEventSource() as Xrm.Controls.DateControl;
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
      let message: string;
      if (fromDateValue.getTime() > toDateValue.getTime()) {
        switch (validationContext) {
        case ValidationContext.FromDateField:
          message = `${fromDateControl.getLabel()} cannot be greater than ${toDateControl.getLabel()}`;
          break;
        case ValidationContext.ToDateField:
          message = `${toDateControl.getLabel()} cannot be less than ${fromDateControl.getLabel()}`;
          break;
        }
      }
      // Check equals if necessary.
      if (!configuration.canBeEqual && fromDateValue.getTime() === toDateValue.getTime()) {
        switch (validationContext) {
        case ValidationContext.FromDateField:
          message = `${fromDateControl.getLabel()} cannot be equal ${toDateControl.getLabel()}`;
          break;
        case ValidationContext.ToDateField:
          message = `${toDateControl.getLabel()} cannot be equal ${fromDateControl.getLabel()}`;
          break;
        }
      }
      // Stop process if message is empty.
      if (!message) {
        return;
      }
      // Show error notification based on the validation context.
      const notificationConfiguration: Xrm.Controls.AddControlNotificationOptions = {
        notificationLevel: 'ERROR',
        messages: [ message ],
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
}
