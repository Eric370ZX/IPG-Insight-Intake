var Intake;
(function (Intake) {
    var Task;
    (function (Task) {
        var RescheduleTask;
        (function (RescheduleTask) {
            function init() {
                //@ts-ignore
                $("#startdate").datepicker();
            }
            RescheduleTask.init = init;
            function ProcessRescheduleTask() {
                var reason = $("#reason").val();
                //@ts-ignore
                var startDate = JSON.stringify($("#startdate").datepicker("getDate").toISOString());
                if (!reason) {
                    alertDialog("You need to populate mandatory fields", "ERROR", null, null);
                    return;
                }
                var data = JSON.parse(getUrlParameter("data"));
                var taskId = data.taskId;
                var additionalTasks = data.additionalTasks;
                var onSuccess = function () {
                    window.parent.Xrm.Utility.closeProgressIndicator();
                    alertDialog("Task rescheduled succesfully", "SUCCESS", function () { window.parent.close(); }, null);
                };
                window.parent.Xrm.Utility.showProgressIndicator("Rescheduling the task...");
                rescheduleTaskRequest(taskId, additionalTasks, startDate, reason, true, onSuccess, function (message) {
                    window.parent.Xrm.Utility.closeProgressIndicator();
                    alertDialog("Error while Rescheduling the task:" + message, "ERROR", null, null);
                });
            }
            RescheduleTask.ProcessRescheduleTask = ProcessRescheduleTask;
            function CloseTaskWindow() {
                window.parent.close();
            }
            RescheduleTask.CloseTaskWindow = CloseTaskWindow;
            function rescheduleTaskRequest(taskid, additionalTasks, startDate, reason, produceTaskNote, onSuccess, onError) {
                var target = {
                    entityType: "task",
                    id: taskid
                };
                var parameters = {
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
                window.parent.Xrm.WebApi.online.execute(ipg_IPGTaskActionsRescheduleTaskRequest).then(function success(result) {
                    if (result.ok) {
                        onSuccess();
                    }
                }, function (error) {
                    onError(error.message);
                });
            }
            function getUrlParameter(sParam) {
                var sPageURL = window.location.search.substring(1), sURLVariables = sPageURL.split('&'), sParameterName, i;
                for (i = 0; i < sURLVariables.length; i++) {
                    sParameterName = sURLVariables[i].split('=');
                    if (sParameterName[0] === sParam) {
                        return sParameterName[1] === undefined ? "" : decodeURIComponent(sParameterName[1]);
                    }
                }
            }
            ;
            function alertDialog(alertText, alertTitle, onSuccess, onError) {
                var alertStrings = { confirmButtonLabel: "OK", text: alertText, title: alertTitle };
                var alertOptions = { height: 120, width: 260 };
                window.parent.Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                    onSuccess();
                }, function (error) {
                    onError();
                });
            }
        })(RescheduleTask = Task.RescheduleTask || (Task.RescheduleTask = {}));
    })(Task = Intake.Task || (Intake.Task = {}));
})(Intake || (Intake = {}));
$(document).ready(function () {
    Intake.Task.RescheduleTask.init();
    $("#okBtn").click(Intake.Task.RescheduleTask.ProcessRescheduleTask);
    $("#cancelBtn").click(Intake.Task.RescheduleTask.CloseTaskWindow);
});
