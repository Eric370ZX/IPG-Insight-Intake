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
/**
 * @namespace Intake.Task
 *
 */
var Intake;
(function (Intake) {
    var Task;
    (function (Task) {
        /**
         * the function to call custom CloseTaskDialog
         * @function Intake.Task.OpenCloseTaskDialog
         * @returns {void}
         */
        function OpenCloseTaskDialog(formContext) {
            formContext.data.refresh(true).then(function (p) {
                var data = { taskId: formContext.data.entity.getId() };
                var pageInput = {
                    pageType: "webresource",
                    webresourceName: "ipg_/intake/task/closeTask.html",
                    data: JSON.stringify(data)
                };
                var navigationOptions = {
                    target: 2,
                    width: 600,
                    height: 350,
                    position: 1,
                };
                var refreshForm = function () {
                    formContext.ui.refreshRibbon(true);
                    formContext.data.refresh(false);
                };
                Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshForm, refreshForm);
                //Xrm.Navigation.openWebResource(pageInput.webresourceName,  { width: 800, height: 670, openInNewWindow: true } , JSON.stringify(data));
            });
        }
        Task.OpenCloseTaskDialog = OpenCloseTaskDialog;
        function Assign(formContext, savedqueryId) {
            formContext.data.refresh(true).then(function (p) {
                var data = { taskId: formContext.data.entity.getId(), savedqueryId: savedqueryId };
                var pageInput = {
                    pageType: "webresource",
                    webresourceName: "ipg_/intake/task/assignTask.html",
                    data: JSON.stringify(data)
                };
                var navigationOptions = {
                    target: 2,
                    width: 500,
                    height: 220,
                    position: 1,
                };
                var refreshForm = function () { return formContext.data.refresh(false); };
                Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshForm, refreshForm);
            });
        }
        Task.Assign = Assign;
        function OpenRescheduleTaskDialog(formContext) {
            formContext.data.refresh(true).then(function (p) {
                var data = { taskId: formContext.data.entity.getId() };
                Xrm.Navigation.openWebResource("ipg_/intake/task/rescheduleTask.html", { width: 800, height: 670, openInNewWindow: true }, JSON.stringify(data));
            });
        }
        Task.OpenRescheduleTaskDialog = OpenRescheduleTaskDialog;
        /**
         * the function to call custom CloseTaskDialog
         * @function Intake.Task.RunClaimGeneration
         * @returns {void}
         */
        function RunClaimGeneration(formContext) {
            var reqObject = {
                getMetadata: function () {
                    return {
                        boundParameter: null,
                        operationType: 0,
                        operationName: "ipg_IPGIntakeActionsRunClaimGenerationJob",
                        parameterTypes: {}
                    };
                }
            };
            Xrm.Utility.showProgressIndicator("Running Claim Generation Job...");
            Xrm.WebApi.online.execute(reqObject)
                .then(function (response) {
                Xrm.Utility.closeProgressIndicator();
                if (response.ok) {
                    return response.json();
                }
                else {
                    Xrm.Navigation.openAlertDialog({ text: response.statusText });
                    return;
                }
            }, function (error) {
                Xrm.Utility.closeProgressIndicator();
                console.log(error.message);
                Xrm.Navigation.openAlertDialog({ text: error.message });
            })
                .then(function (result) {
                if (result.HasErrors == true) {
                    Xrm.Navigation.openAlertDialog({ text: result.Message });
                }
                else {
                    console.log("ok");
                    formContext.data.refresh(false);
                }
            });
        }
        Task.RunClaimGeneration = RunClaimGeneration;
        /**
         * the function to open a manual adjustment
         * @function Intake.Task.OpenAdjustmentForm
         * @returns {void}
         */
        function OpenAdjustmentForm(primaryControl) {
            var formContext = primaryControl;
            var caseId = formContext.getAttribute("regardingobjectid").getValue();
            if (caseId) {
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_adjustment";
                Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = Intake.Utility.removeCurlyBraces(caseId[0].id);
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = "incident";
                    formParameters["ipg_percent"] = 100;
                    formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
                    formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
                    formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
                    formParameters["ipg_casebalance"] = incident.ipg_casebalance;
                    Xrm.Navigation.openForm(entityFormOptions, formParameters);
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        Task.OpenAdjustmentForm = OpenAdjustmentForm;
        /**
         * return true if a task is a batch generation task
         * @function Intake.Task.IsBatchGenerationTask
         * @returns {boolean}
         */
        function IsBatchGenerationTask(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, taskType, CMS1500_TASK, result, UB04_TASK, result;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            taskType = formContext.getAttribute("ipg_tasktypeid").getValue();
                            if (!taskType) return [3 /*break*/, 3];
                            CMS1500_TASK = 'Task.InstitutionalClaimsReadyToPrint';
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_globalsetting', "?$select=ipg_value&$filter=ipg_name eq '" + CMS1500_TASK + "'")];
                        case 1:
                            result = _a.sent();
                            if (result.entities.length > 0 && result.entities[0].ipg_value) {
                                if (result.entities[0].ipg_value == taskType[0].name) {
                                    return [2 /*return*/, true];
                                }
                            }
                            UB04_TASK = 'Task.ProfessionalClaimsReadyToPrint';
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_globalsetting', "?$select=ipg_value&$filter=ipg_name eq '" + UB04_TASK + "'")];
                        case 2:
                            result = _a.sent();
                            if (result.entities.length > 0 && result.entities[0].ipg_value) {
                                if (result.entities[0].ipg_value == taskType[0].name) {
                                    return [2 /*return*/, true];
                                }
                            }
                            _a.label = 3;
                        case 3: return [2 /*return*/, false];
                    }
                });
            });
        }
        Task.IsBatchGenerationTask = IsBatchGenerationTask;
        /**
          * call Custom action
          * @function Intake.Task.callAction
          * @returns {void}
        */
        function callAction(actionName, parameters, async, successCallback) {
            var req = new XMLHttpRequest();
            req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/" + actionName, async);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        if (this.response == "")
                            successCallback();
                        else
                            successCallback(JSON.parse(this.response));
                    }
                    else {
                        Xrm.Utility.closeProgressIndicator();
                        alert(JSON.parse(this.response).error.message);
                    }
                }
            };
            req.send(JSON.stringify(parameters));
        }
        /**
         * Generates batch
         * @function Intake.Task.GenerateBatch
         * @returns {void}
         */
        function GenerateBatch(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, parameters;
                return __generator(this, function (_a) {
                    formContext = primaryControl;
                    parameters = {
                        Target: {
                            "activityid": formContext.data.entity.getId(),
                            "@odata.type": "Microsoft.Dynamics.CRM." + formContext.data.entity.getEntityName()
                        },
                        TaskType: formContext.getAttribute("ipg_tasktypeid").getValue()[0].name
                    };
                    Xrm.Utility.showProgressIndicator("Generating Batch...");
                    callAction("ipg_IPGIntakeActionsGenerateClaimsBatch", parameters, true, function (result) {
                        var _this = this;
                        formContext.data.refresh(true).then(function () { return __awaiter(_this, void 0, void 0, function () {
                            var blob, blobUrl;
                            return __generator(this, function (_a) {
                                Xrm.Utility.closeProgressIndicator();
                                formContext.ui.refreshRibbon();
                                blob = b64toBlob(result.PdfFileBase64, "application/pdf");
                                blobUrl = URL.createObjectURL(blob);
                                window.open(blobUrl, "_blank");
                                return [2 /*return*/];
                            });
                        }); });
                    });
                    return [2 /*return*/];
                });
            });
        }
        Task.GenerateBatch = GenerateBatch;
        function b64toBlob(b64Data, contentType, sliceSize) {
            if (sliceSize === void 0) { sliceSize = 512; }
            var byteCharacters = atob(b64Data);
            var byteArrays = [];
            for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                var slice = byteCharacters.slice(offset, offset + sliceSize);
                var byteNumbers = new Array(slice.length);
                for (var i = 0; i < slice.length; i++) {
                    byteNumbers[i] = slice.charCodeAt(i);
                }
                var byteArray = new Uint8Array(byteNumbers);
                byteArrays.push(byteArray);
            }
            var blob = new Blob(byteArrays, { type: contentType });
            return blob;
        }
        function OpenMultipleCloseTaskDialog(data, gridcontrol) {
            var pageInput = {
                pageType: "webresource",
                webresourceName: "ipg_/intake/task/closeTask.html",
                data: JSON.stringify(data)
            };
            var navigationOptions = {
                target: 2,
                width: 550,
                height: 350,
                position: 1,
            };
            var refreshGrid = function () { gridcontrol.refresh(); gridcontrol.refreshRibbon(); };
            Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshGrid, refreshGrid);
        }
        Task.OpenMultipleCloseTaskDialog = OpenMultipleCloseTaskDialog;
        function NewDocumentTask(primaryControl) {
            var formContext = primaryControl;
            var regardingRef = {
                entityType: formContext.data.entity.getEntityName(),
                id: formContext.data.entity.getId()
            };
            Xrm.WebApi.retrieveMultipleRecords("ipg_taskcategory", "?$filter=ipg_name eq 'Missing Information'").then(function (result) {
                if (result.entities.length > 0) {
                    var parameters = {};
                    parameters["ipg_taskcategoryid"] = result.entities[0]['ipg_taskcategoryid'];
                    parameters["ipg_taskcategoryidname"] = "Missing Information";
                    parameters["ipg_taskcategoryidtype"] = "ipg_taskcategory";
                    Xrm.Utility.openQuickCreate('task', regardingRef, parameters);
                }
            }, function (error) {
                console.log(error);
            });
        }
        Task.NewDocumentTask = NewDocumentTask;
        function OnlyOpenTasksSelected(selectedTaskIds) {
            return __awaiter(this, void 0, void 0, function () {
                var fetchXml, tasks;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!(selectedTaskIds && selectedTaskIds.length > 0)) return [3 /*break*/, 2];
                            fetchXml = generateFetchXmlToRetrieveTasksByIds(selectedTaskIds);
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("task", fetchXml)];
                        case 1:
                            tasks = _a.sent();
                            if (tasks && tasks.entities.length > 0) {
                                return [2 /*return*/, tasks.entities.every(function (d) {
                                        return (d["statecode"] === 0);
                                    })];
                            }
                            _a.label = 2;
                        case 2: return [2 /*return*/, false];
                    }
                });
            });
        }
        Task.OnlyOpenTasksSelected = OnlyOpenTasksSelected;
        function generateFetchXmlToRetrieveTasksByIds(taskIds) {
            var filterValues = "";
            taskIds.forEach(function (id) {
                filterValues += "\n<value>" + id + "</value>";
            });
            var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\n                  <entity name=\"task\">\n                    <attribute name=\"subject\" />\n                    <attribute name=\"statecode\" />\n                    <filter type=\"and\">\n                      <condition attribute=\"activityid\" operator=\"in\">" +
                filterValues +
                "\n                      </condition>\n                    </filter>\n                  </entity>\n                </fetch>";
            fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
            return fetchXml;
        }
    })(Task = Intake.Task || (Intake.Task = {}));
})(Intake || (Intake = {}));
