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
        var AssignTask;
        (function (AssignTask) {
            ;
            var Xrm = window.parent.Xrm;
            var data = JSON.parse(getUrlParameter("data"));
            function init() {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                Xrm.Utility.showProgressIndicator("");
                                return [4 /*yield*/, initUsers()];
                            case 1:
                                _a.sent();
                                return [4 /*yield*/, initTaskReasons()];
                            case 2:
                                _a.sent();
                                Xrm.Utility.closeProgressIndicator();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            AssignTask.init = init;
            function initUsers() {
                return __awaiter(this, void 0, void 0, function () {
                    var users, $selectWrapper, $selecet;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('systemuser', "?savedQuery=" + data.savedqueryId)];
                            case 1:
                                users = _a.sent();
                                $selectWrapper = $('#assignTo');
                                $selecet = $('select', $selectWrapper);
                                $selecet.append($("<option/>", { value: 0, text: '' }));
                                users.entities.map(function (user) { return $selecet.append($("<option/>", { value: user.systemuserid, text: user.fullname })); });
                                $selectWrapper.css('display', 'flex');
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function initTaskReasons() {
                return __awaiter(this, void 0, void 0, function () {
                    var task, assigningCode, taskreasons, $selectWrapper, $selecet;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord('task', data.taskId, '?$select=_ipg_taskcategoryid_value')];
                            case 1:
                                task = _a.sent();
                                if (!task['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue']) return [3 /*break*/, 3];
                                assigningCode = 427880000;
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_taskreason', "?$select=ipg_name&$filter=_ipg_taskcategory_value eq " + task['_ipg_taskcategoryid_value'] + " and Microsoft.Dynamics.CRM.ContainValues(PropertyName=@p1,PropertyValues=@p2)&@p1='ipg_usedbyactioncodes'&@p2='" + assigningCode + "'")];
                            case 2:
                                taskreasons = _a.sent();
                                if (taskreasons.entities.length > 0) {
                                    $selectWrapper = $('#taskreasonid');
                                    $selecet = $('select', $selectWrapper);
                                    $selecet.append($("<option/>", { value: 0, text: '' }));
                                    taskreasons.entities.map(function (taskReason) { return $selecet.append($("<option/>", { value: taskReason.ipg_taskreasonid, text: taskReason.ipg_name })); });
                                    $selectWrapper.css('display', 'flex');
                                }
                                _a.label = 3;
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            }
            function ProcessAssignTask() {
                var user = $('#assignTo>div>select').val();
                if (!user || user == '0') {
                    Xrm.Navigation.openAlertDialog({ text: "You need to select User", title: "Validation ERROR" });
                    return;
                }
                var taskreason = $('#taskreasonid>div>select').val();
                var onSuccess = function () {
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: "Task Assigned successfully" }).then(function () {
                        window.close();
                    });
                };
                var task = { taskid: data.taskId, ownerid: user, taskreasonid: taskreason };
                Xrm.Utility.showProgressIndicator("");
                assignTaskRequest(task, onSuccess, function (message) {
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: "Error while Assigning the task:" + message });
                });
            }
            AssignTask.ProcessAssignTask = ProcessAssignTask;
            function CloseTaskWindow() {
                window.close();
            }
            AssignTask.CloseTaskWindow = CloseTaskWindow;
            function assignTaskRequest(task, onSuccess, onError) {
                var taskupdateObj = { "ownerid@odata.bind": "/systemusers(" + task.ownerid + ")" };
                if (task.taskreasonid && task.taskreasonid != '0') {
                    taskupdateObj["ipg_taskreason_Task@odata.bind"] = "/ipg_taskreasons(" + task.taskreasonid + ")";
                }
                Xrm.WebApi.online.updateRecord("task", task.taskid, taskupdateObj).then(function success(result) {
                    if (result.ok || result.id) {
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
        })(AssignTask = Task.AssignTask || (Task.AssignTask = {}));
    })(Task = Intake.Task || (Intake.Task = {}));
})(Intake || (Intake = {}));
$(function () {
    $("#okBtn").on('click', Intake.Task.AssignTask.ProcessAssignTask);
    $("#cancelBtn").on('click', Intake.Task.AssignTask.CloseTaskWindow);
    Intake.Task.AssignTask.init();
});
