var Intake;
(function (Intake) {
    var PortalUpdate;
    (function (PortalUpdate) {
        var attributes = {
            startDate: "ipg_startdate",
            endDate: "ipg_dueto",
            sentTo: "ipg_facility_account",
            isAllFacilities: "ipg_isallfacilities",
            messageStartDateInThePast: "Start date cannot be in the past.",
            messageStartDateLaterEndDate: "Start date can't be later than 'Display End Date'.",
            messageEndDateEarlierStartDate: "End date can't be earlear than 'Display Start Date'"
        };
        function validateDisplayStartDateOnChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var today = new Date();
            var currentDate = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0);
            var startDate = setProperStartDateTime(formContext.getAttribute(attributes.startDate).getValue());
            var endDate = setProperEndDateTime(formContext.getAttribute(attributes.endDate).getValue());
            if (startDate) {
                formContext.getAttribute(attributes.startDate).setValue(startDate);
                if (startDate < currentDate) {
                    formContext.getControl(attributes.startDate).setNotification(attributes.messageStartDateInThePast, "displayStartDateBeforeToday");
                }
                else {
                    formContext.getControl(attributes.startDate).clearNotification("displayStartDateBeforeToday");
                }
            }
            if (startDate && endDate) {
                if (startDate > endDate) {
                    formContext.getControl(attributes.startDate).setNotification(attributes.messageStartDateLaterEndDate, "startDateLaterEndDate");
                }
                else {
                    formContext.getControl(attributes.startDate).clearNotification("startDateLaterEndDate");
                }
            }
        }
        PortalUpdate.validateDisplayStartDateOnChange = validateDisplayStartDateOnChange;
        function validateDisplayEndDateOnChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var startDate = setProperStartDateTime(formContext.getAttribute(attributes.startDate).getValue());
            var endDate = setProperEndDateTime(formContext.getAttribute(attributes.endDate).getValue());
            if (endDate) {
                formContext.getAttribute(attributes.endDate).setValue(endDate);
                if (startDate && endDate < startDate) {
                    formContext.getControl(attributes.endDate).setNotification(attributes.messageEndDateEarlierStartDate, "displayEndDateBeforeStart");
                }
                else {
                    formContext.getControl(attributes.endDate).clearNotification("displayEndDateBeforeStart");
                }
            }
        }
        PortalUpdate.validateDisplayEndDateOnChange = validateDisplayEndDateOnChange;
        function isAllFacilitiesOnChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var isAllFacilities = formContext.getAttribute(attributes.isAllFacilities).getValue();
            if (isAllFacilities) {
                formContext.getAttribute(attributes.sentTo).setRequiredLevel("none");
                formContext.getControl(attributes.sentTo).setVisible(false);
            }
            else {
                formContext.getAttribute(attributes.sentTo).setRequiredLevel("required");
                formContext.getControl(attributes.sentTo).setVisible(true);
            }
        }
        PortalUpdate.isAllFacilitiesOnChange = isAllFacilitiesOnChange;
        function OnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            formContext.getControl(attributes.startDate).setShowTime(false);
            // formContext.getControl(attributes.endDate).setShowTime(false);
            var isAllFacilitiesAttr = formContext.getAttribute(attributes.isAllFacilities);
            if (isAllFacilitiesAttr) {
                var isAllFacilities = isAllFacilitiesAttr.getValue();
                if (isAllFacilities) {
                    formContext.getAttribute(attributes.sentTo).setRequiredLevel("none");
                    formContext.getControl(attributes.sentTo).setVisible(false);
                }
            }
        }
        PortalUpdate.OnLoad = OnLoad;
        function setProperStartDateTime(dateTime) {
            if (dateTime)
                return new Date(dateTime.getFullYear(), dateTime.getMonth(), dateTime.getDate(), 0, 0, 0);
        }
        function setProperEndDateTime(dateTime) {
            if (dateTime)
                return new Date(dateTime.getFullYear(), dateTime.getMonth(), dateTime.getDate(), 11, 59, 59);
        }
    })(PortalUpdate = Intake.PortalUpdate || (Intake.PortalUpdate = {}));
})(Intake || (Intake = {}));
