namespace Intake.Task.RescheduleTask {
    export function init() {
        //@ts-ignore
        $("#startdate").datepicker();
    }
    export function ProcessRescheduleTask() {
        const reason = $("#reason").val();
        //@ts-ignore
        var startDate = JSON.stringify($("#startdate").datepicker("getDate").toISOString());
        if (!reason) {
            alertDialog("You need to populate mandatory fields", "ERROR", null, null);
            return;
        }
        var data = JSON.parse(getUrlParameter("data"));
        const taskId = data.taskId as string;
        const additionalTasks = data.additionalTasks as string;
        const onSuccess = () => {
            window.parent.Xrm.Utility.closeProgressIndicator();
            alertDialog("Task rescheduled succesfully", "SUCCESS",
                () => { window.parent.close(); }, null);
        };
        window.parent.Xrm.Utility.showProgressIndicator("Rescheduling the task...");
        rescheduleTaskRequest(taskId as string, additionalTasks as string, startDate as string, reason as string, true, onSuccess, (message) => {
            window.parent.Xrm.Utility.closeProgressIndicator();
            alertDialog(`Error while Rescheduling the task:${message}`, "ERROR", null, null);
        });
    }
    export function CloseTaskWindow() {
        window.parent.close();
    }

    function rescheduleTaskRequest(taskid: string, additionalTasks: string, startDate: string, reason: string, produceTaskNote: boolean, onSuccess: () => void, onError: (message: string) => void) {
        const target: { entityType: string, id: string } = {
            entityType: "task",
            id: taskid
        };
        const parameters: { Target: any, AdditionalTasks: string, NewStartDate: string, RescheduleNote: string, ProduceTaskNote: boolean } = {
            Target: target,
            AdditionalTasks: additionalTasks,
            NewStartDate: startDate,
            RescheduleNote: reason,
            ProduceTaskNote: produceTaskNote
        };

        var ipg_IPGTaskActionsRescheduleTaskRequest = {
            entity: parameters.Target,
            AdditionalTasks: parameters.AdditionalTasks,
            NewStartDate: parameters.NewStartDate,
            RescheduleNote: parameters.RescheduleNote,
            ProduceTaskNote: parameters.ProduceTaskNote,
            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        entity: {
                            "typeName": "mscrm.task",
                            "structuralProperty": 5
                        },
                        "AdditionalTasks": {
                            "typeName": "Edm.String",
                            "structuralProperty": 1
                        },
                        "NewStartDate": {
                            "typeName": "Edm.DateTimeOffset",
                            "structuralProperty": 1
                        },
                        "RescheduleNote": {
                            "typeName": "Edm.String",
                            "structuralProperty": 1
                        },
                        "ProduceTaskNote": {
                            "typeName": "Edm.Boolean",
                            "structuralProperty": 1
                        }
                    },
                    operationType: 0,
                    operationName: "ipg_IPGTaskActionsRescheduleTask"
                };
            }
        };

        window.parent.Xrm.WebApi.online.execute(ipg_IPGTaskActionsRescheduleTaskRequest).then(
            function success(result) {
                if (result.ok) {
                    onSuccess();
                }
            },
            function (error) {
                onError(error.message);
            }
        );
    }
    function getUrlParameter(sParam) {
        let sPageURL = window.location.search.substring(1),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? "" : decodeURIComponent(sParameterName[1]);
            }
        }
    };

    function alertDialog(alertText: string, alertTitle: string, onSuccess: () => void, onError: () => void) {
        let alertStrings = { confirmButtonLabel: "OK", text: alertText, title: alertTitle };
        let alertOptions = { height: 120, width: 260 };
        window.parent.Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
            function (success) {
                onSuccess();
            },
            function (error) {
                onError();
            }
        );
    }
}
$(document).ready(() => {
    Intake.Task.RescheduleTask.init();
    $("#okBtn").click(Intake.Task.RescheduleTask.ProcessRescheduleTask);
    $("#cancelBtn").click(Intake.Task.RescheduleTask.CloseTaskWindow);
});
