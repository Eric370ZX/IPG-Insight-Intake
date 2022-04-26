var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var Intake;
(function (Intake) {
    var Task;
    (function (Task) {
        var CloseTask;
        (function (CloseTask) {
            var CloseReason;
            (function (CloseReason) {
                CloseReason[CloseReason["Resolved"] = 5] = "Resolved";
                CloseReason[CloseReason["Cancelled"] = 923720001] = "Cancelled";
                CloseReason[CloseReason["ExceptionApproved"] = 923720000] = "ExceptionApproved";
            })(CloseReason || (CloseReason = {}));
            var Xrm = window.parent.Xrm;
            var data = JSON.parse(getUrlParameter("data"));
            var taskId = data.taskId;
            var multiple = false;
            var tasks = [];
            if (taskId !== null && taskId !== undefined) {
                tasks.push(taskId);
            }
            else if (data.length > 0) {
                tasks = data;
                multiple = true;
            }
            for (var i = 0; i < tasks.length; i++) {
                tasks[i] = tasks[i].replace('{', '').replace('}', '');
            }
            var isOutreachTaskClosure = false;
            var istaskReasoReq = false;
            function init() {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, initCloseReasons()];
                            case 1:
                                _a.sent();
                                if (!(taskId !== null && taskId !== undefined)) return [3 /*break*/, 3];
                                return [4 /*yield*/, initTaskReasons()];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 5];
                            case 3: return [4 /*yield*/, initAllTaskReasons()];
                            case 4:
                                _a.sent();
                                _a.label = 5;
                            case 5:
                                Intake.Task.CloseTask.CloseReasonChange();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            CloseTask.init = init;
            function initCloseReasons() {
                return __awaiter(this, void 0, void 0, function () {
                    var supervisorrole;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                supervisorrole = Xrm.Utility.getGlobalContext().userSettings.roles.get(function (r) { return r.name.trim().toLowerCase() == "supervisor"; });
                                return [4 /*yield*/, Xrm.Utility.getGlobalContext().getCurrentAppName()];
                            case 1:
                                if ((_a.sent()) != "Collections" || supervisorrole.length > 0) {
                                    $("#closeReason > option:nth-child(2)").css("display", "block");
                                    $("#closeReason > option:nth-child(3)").css("display", "block");
                                }
                                $("#closeReason").trigger("change");
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function initTaskReasons() {
                return __awaiter(this, void 0, void 0, function () {
                    var task, taskreasons, $selectWrapper;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord('task', taskId, '?$select=_ipg_taskcategoryid_value')];
                            case 1:
                                task = _a.sent();
                                if (!task['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue']) return [3 /*break*/, 3];
                                isOutreachTaskClosure = task['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue'].toLowerCase().indexOf("outreach") > -1;
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_taskreason', "?$select=ipg_name&$filter=ipg_usedbyactioncodes eq null and (_ipg_taskcategory_value eq " + task['_ipg_taskcategoryid_value'] + " or _ipg_taskcategory_value eq null)")];
                            case 2:
                                taskreasons = _a.sent();
                                if (taskreasons.entities.length > 0) {
                                    $selectWrapper = $('#taskreasonid');
                                    taskreasons.entities.map(function (taskReason) { return $selectWrapper.append($("<option/>", { value: taskReason.ipg_taskreasonid, text: taskReason.ipg_name })); });
                                }
                                _a.label = 3;
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            }
            function initAllTaskReasons() {
                return __awaiter(this, void 0, void 0, function () {
                    var tasksCondition, fetchXml, taskreasons, $selectWrapper;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                tasksCondition = tasks.map(function (t) { return "<value>" + t + "</value>"; });
                                fetchXml = [
                                    "<fetch distinct='true'>",
                                    "  <entity name='ipg_taskreason'>",
                                    "    <attribute name='ipg_name' />",
                                    "    <attribute name='ipg_taskreasonid' />",
                                    "    <filter>",
                                    "      <condition attribute='ipg_usedbyactioncodes' operator='null' />",
                                    "      <filter type='or'>",
                                    "        <condition attribute='ipg_taskcategory' operator='null' />",
                                    "        <condition entityname='task' attribute='activityid' operator='in'>", tasksCondition,
                                    "        </condition>",
                                    "      </filter>",
                                    "    </filter>",
                                    "    <link-entity name='task' from='ipg_taskcategoryid' to='ipg_taskcategory' link-type='outer' alias='task' />",
                                    "  </entity>",
                                    "</fetch>",
                                ].join("");
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_taskreason', "?fetchXml=" + encodeURIComponent(fetchXml))];
                            case 1:
                                taskreasons = _a.sent();
                                if (taskreasons.entities.length > 0) {
                                    $selectWrapper = $('#taskreasonid');
                                    taskreasons.entities.map(function (taskReason) { return $selectWrapper.append($("<option/>", { value: taskReason.ipg_taskreasonid, text: taskReason.ipg_name })); });
                                }
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function ProcessCloseTask() {
                var closeReason = $("#closeReason").val();
                var closeNote = $("#closeNote").val();
                var taskReason = $('#taskreasonid').val();
                if (!closeReason || closeNote == '') {
                    Xrm.Navigation.openAlertDialog({ text: "You need to populate mandatory fields", title: "Validation ERROR" });
                    return;
                }
                var req = {
                    taskid: null,
                    closeReason: closeReason,
                    closeNote: closeNote,
                    produceTaskNote: true
                };
                if ((!taskReason || taskReason == '0') && istaskReasoReq) {
                    Xrm.Navigation.openAlertDialog({ text: "You need to populate Task Reason", title: "Validation ERROR" });
                    return;
                }
                else {
                    req.taskReason = taskReason != '0' ? taskReason : null;
                }
                var ids = tasks.join(" or activityid eq ") + ')';
                Xrm.WebApi.retrieveMultipleRecords('task', '?$select=activityid, subject&$filter=statecode eq 0 and (activityid eq ' + ids).then(function (openTasks) {
                    var _a;
                    if (!(((_a = openTasks === null || openTasks === void 0 ? void 0 : openTasks.entities) === null || _a === void 0 ? void 0 : _a.length) > 0)) {
                        Xrm.Navigation.openConfirmDialog({ text: "All selected Tasks are already closed" }).then(function () {
                            window.close();
                        });
                        return;
                    }
                    var _loop_1 = function (i) {
                        req.taskid = openTasks.entities[i].activityid;
                        var onSuccess = function () {
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openAlertDialog({ text: "Task '" + openTasks.entities[i].subject + "' closed succesfully" }).then(function () {
                                window.close();
                            });
                        };
                        Xrm.Utility.showProgressIndicator("Closing...");
                        closeTaskRequest(req, onSuccess, function (message) {
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openErrorDialog({ message: "Error while closing the task:" + message });
                        });
                    };
                    for (var i = 0; i < openTasks.entities.length; i++) {
                        _loop_1(i);
                    }
                }, function (e) { console.log(e); });
            }
            CloseTask.ProcessCloseTask = ProcessCloseTask;
            function CloseTaskWindow() {
                window.close();
            }
            CloseTask.CloseTaskWindow = CloseTaskWindow;
            function CloseReasonChange() {
                var _a, _b;
                var $selectWrapper = $('#taskReason');
                var closeReason = $("#closeReason").val();
                var taskReason = (_a = $('#taskreasonid')) === null || _a === void 0 ? void 0 : _a[0];
                var taskReasonLbl = (_b = $('#lbltaskReason')) === null || _b === void 0 ? void 0 : _b[0];
                if (closeReason && closeReason == CloseReason.Cancelled || isOutreachTaskClosure || multiple) {
                    $selectWrapper.css("display", "flex");
                    istaskReasoReq = true;
                    if (closeReason == CloseReason.Cancelled) {
                        for (var i = 0; i < taskReason.options.length; i++) {
                            if (taskReason.options[i].text === "Not Needed") {
                                taskReason.value = taskReason.options[i].value;
                            }
                        }
                    }
                    return;
                }
                else {
                    $selectWrapper.css("display", "none");
                    $('#taskreasonid option[value=0]').prop('selected', 'selected').trigger("change");
                    istaskReasoReq = false;
                    taskReasonLbl.innerText = taskReasonLbl.innerText.replace('*', '');
                    return;
                }
            }
            CloseTask.CloseReasonChange = CloseReasonChange;
            function closeTaskRequest(req, onSuccess, onError) {
                var target = {
                    entityType: "task",
                    id: req.taskid
                };
                var parameters = {
                    Target: target,
                    CloseReason: req.closeReason,
                    CloseNote: req.closeNote,
                    ProduceTaskNote: req.produceTaskNote
                };
                parameters.TaskReason = req.taskReason ? { entityType: "ipg_taskreason", id: req.taskReason } : null;
                var ipg_IPGTaskActionsCloseTaskRequest = {
                    entity: parameters.Target,
                    CloseReason: parameters.CloseReason,
                    CloseNote: parameters.CloseNote,
                    ProduceTaskNote: parameters.ProduceTaskNote,
                    TaskReason: parameters.TaskReason,
                    getMetadata: function () {
                        return {
                            boundParameter: "entity",
                            parameterTypes: {
                                entity: {
                                    "typeName": "mscrm.task",
                                    "structuralProperty": 5
                                },
                                "CloseReason": {
                                    "typeName": "Edm.Int32",
                                    "structuralProperty": 1
                                },
                                "CloseNote": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                },
                                "ProduceTaskNote": {
                                    "typeName": "Edm.Boolean",
                                    "structuralProperty": 1
                                },
                                "TaskReason": {
                                    "typeName": "mscrm.ipg_taskreason",
                                    "structuralProperty": 5
                                }
                            },
                            operationType: 0,
                            operationName: "ipg_IPGTaskActionsCloseTask"
                        };
                    }
                };
                Xrm.WebApi.online.execute(ipg_IPGTaskActionsCloseTaskRequest).then(function success(result) {
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
        })(CloseTask = Task.CloseTask || (Task.CloseTask = {}));
    })(Task = Intake.Task || (Intake.Task = {}));
})(Intake || (Intake = {}));
$(function () {
    $("#okBtn").on('click', Intake.Task.CloseTask.ProcessCloseTask);
    $("#cancelBtn").on('click', Intake.Task.CloseTask.CloseTaskWindow);
    $("#closeReason").on('change', Intake.Task.CloseTask.CloseReasonChange);
    Intake.Task.CloseTask.init();
});
