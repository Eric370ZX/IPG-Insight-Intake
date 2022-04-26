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
        var taskSubjectOptions = {
            portalDocument: 427880000,
            anyDocument: 427880001,
            portalField: 427880002,
            anyField: 427880003
        };
        var taskGeneratedBy;
        (function (taskGeneratedBy) {
            taskGeneratedBy[taskGeneratedBy["user"] = 427880001] = "user";
            taskGeneratedBy[taskGeneratedBy["both"] = 427880002] = "both";
        })(taskGeneratedBy || (taskGeneratedBy = {}));
        var caseStatus;
        (function (caseStatus) {
            caseStatus[caseStatus["Open"] = 923720000] = "Open";
            caseStatus[caseStatus["Closed"] = 923720001] = "Closed";
        })(caseStatus || (caseStatus = {}));
        var tempTaskTypeFilterFetch;
        function setDataFromTaskType(form) {
            var _a, _b;
            return __awaiter(this, void 0, void 0, function () {
                var taskTypeField, taskTypeRef, taskType, assignToTeamAttr, ownerAttr, assignToTeamVal, assignToTeamAttrVal, regardingAttrVal, regarding, taskCategoryAttr, visibleOnPortalAttr, priorityAttr, descriptionAttr, subCategoryAttr;
                return __generator(this, function (_c) {
                    switch (_c.label) {
                        case 0:
                            if (!(form.ui.getFormType() === 1 /* Create */)) return [3 /*break*/, 5];
                            taskTypeField = form.getAttribute("ipg_tasktypeid");
                            taskTypeRef = taskTypeField.getValue() && taskTypeField.getValue().length > 0 && taskTypeField.getValue()[0];
                            if (!taskTypeRef) return [3 /*break*/, 5];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord(taskTypeRef.entityType, taskTypeRef.id, "?$select=_ipg_documenttypeid_value,ipg_startdate,ipg_priority,ipg_subcategory,_ipg_assigntouserid_value,ipg_isportal,_ipg_taskcategoryid_value,_ipg_assigntoteam_value,ipg_name,ipg_description,ipg_duedate")];
                        case 1:
                            taskType = _c.sent();
                            return [4 /*yield*/, setDates(form, taskType)];
                        case 2:
                            _c.sent();
                            assignToTeamAttr = form.getAttribute('ipg_assignedtoteamid');
                            ownerAttr = form.getAttribute('ownerid');
                            if (taskType._ipg_assigntouserid_value) {
                                ownerAttr === null || ownerAttr === void 0 ? void 0 : ownerAttr.setValue([
                                    {
                                        entityType: 'systemuser',
                                        name: taskType['_ipg_assigntouserid_value@OData.Community.Display.V1.FormattedValue'],
                                        id: taskType._ipg_assigntouserid_value
                                    }
                                ]);
                            }
                            if (taskType._ipg_assigntoteam_value) {
                                assignToTeamVal = [
                                    {
                                        entityType: 'team',
                                        name: taskType['_ipg_assigntoteam_value@OData.Community.Display.V1.FormattedValue'],
                                        id: taskType._ipg_assigntoteam_value
                                    }
                                ];
                                assignToTeamAttr === null || assignToTeamAttr === void 0 ? void 0 : assignToTeamAttr.setValue(assignToTeamVal);
                                if (!taskType._ipg_assigntouserid_value) {
                                    ownerAttr === null || ownerAttr === void 0 ? void 0 : ownerAttr.setValue(assignToTeamVal);
                                }
                                assignToTeamAttr.fireOnChange();
                            }
                            assignToTeamAttrVal = assignToTeamAttr === null || assignToTeamAttr === void 0 ? void 0 : assignToTeamAttr.getValue();
                            regardingAttrVal = (_a = form.getAttribute("regardingobjectid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            if (!((assignToTeamAttrVal === null || assignToTeamAttrVal === void 0 ? void 0 : assignToTeamAttrVal.length) > 0 && (regardingAttrVal === null || regardingAttrVal === void 0 ? void 0 : regardingAttrVal.length) > 0 && (regardingAttrVal[0].entityType == "ipg_referral" || regardingAttrVal[0].entityType == "incident"))) return [3 /*break*/, 4];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord(regardingAttrVal[0].entityType, regardingAttrVal[0].id, "?$select=_ipg_assignedtoteamid_value,_ownerid_value")];
                        case 3:
                            regarding = (_c.sent());
                            if (regarding._ipg_assignedtoteamid_value
                                && regarding._ipg_assignedtoteamid_value.replace("{", "").replace("}", "").toLowerCase() === assignToTeamAttrVal[0].id.replace("{", "").replace("}", "").toLowerCase()
                                && regarding["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"] === "systemuser") {
                                ownerAttr.setValue([{ entityType: "systemuser", id: regarding._ownerid_value, name: regarding["_ownerid_value@OData.Community.Display.V1.FormattedValue"] }]);
                            }
                            _c.label = 4;
                        case 4:
                            taskCategoryAttr = form.getAttribute('ipg_taskcategoryid');
                            if (taskType._ipg_taskcategoryid_value && taskCategoryAttr) {
                                if (!taskCategoryAttr.getValue() || taskCategoryAttr.getValue()[0].id != taskType._ipg_taskcategoryid_value) {
                                    taskCategoryAttr.setValue([{
                                            entityType: 'ipg_taskcategory',
                                            name: taskType['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue'],
                                            id: taskType._ipg_taskcategoryid_value
                                        }]);
                                }
                            }
                            visibleOnPortalAttr = form.getAttribute('ipg_isvisibleonportal');
                            visibleOnPortalAttr && visibleOnPortalAttr.setValue((_b = taskType.ipg_isportal) !== null && _b !== void 0 ? _b : false);
                            priorityAttr = form.getAttribute('ipg_priority');
                            if (priorityAttr && taskType.ipg_priority) {
                                priorityAttr.setValue(taskType.ipg_priority);
                            }
                            descriptionAttr = form.getAttribute('description');
                            if (descriptionAttr && taskType.ipg_description) {
                                descriptionAttr.setValue(taskType.ipg_description);
                            }
                            subCategoryAttr = form.getAttribute('subcategory');
                            if (subCategoryAttr && taskType.ipg_subcategory) {
                                subCategoryAttr.setValue(taskType.ipg_subcategory);
                            }
                            _c.label = 5;
                        case 5: return [2 /*return*/];
                    }
                });
            });
        }
        function setTitle(form) {
            var _a, _b, _c, _d, _e;
            return __awaiter(this, void 0, void 0, function () {
                var regarding, incident, _f, subj, taskTypeId, taskType;
                return __generator(this, function (_g) {
                    switch (_g.label) {
                        case 0:
                            if (!(form.ui.getFormType() === 1 /* Create */)) return [3 /*break*/, 5];
                            regarding = (_a = form.getAttribute("regardingobjectid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            if (!(regarding.length > 0 && regarding[0].entityType == 'incident')) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('incident', regarding[0].id, "?$select=ipg_statecode")];
                        case 1:
                            _f = (_g.sent());
                            return [3 /*break*/, 3];
                        case 2:
                            _f = null;
                            _g.label = 3;
                        case 3:
                            incident = _f;
                            subj = form.getAttribute("subject");
                            taskTypeId = (_c = (_b = form.getAttribute("ipg_tasktypeid")) === null || _b === void 0 ? void 0 : _b.getValue()[0]) === null || _c === void 0 ? void 0 : _c.id;
                            if (!(incident && subj && taskTypeId)) return [3 /*break*/, 5];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_tasktype", taskTypeId, "?$select=ipg_name,ipg_tasktitle")];
                        case 4:
                            taskType = _g.sent();
                            subj.setValue("".concat((_e = (_d = taskType.ipg_tasktitle) !== null && _d !== void 0 ? _d : taskType.ipg_name) !== null && _e !== void 0 ? _e : ''));
                            _g.label = 5;
                        case 5: return [2 /*return*/];
                    }
                });
            });
        }
        function ipg_tasktypeidOnChange(form) {
            setTitle(form);
            setDataFromTaskType(form);
        }
        function getRegardingCaseId(form) {
            var regObj = form.getAttribute("regardingobjectid");
            if (!regObj || !regObj.getValue() || regObj.getValue()[0].entityType != "incident") {
                return;
            }
            return regObj.getValue()[0].name;
        }
        function getDescriptionForNotEnyFieldSubject(subject, caseId) {
            return "Please provide missing value of ".concat(subject, " for Case ").concat(caseId);
        }
        function changeFieldPropertyByTaskSubject(form, taskSubjectValue, taskSubjectOptionName) {
            var _a, _b, _c, _d, _e;
            if (taskSubjectValue == taskSubjectOptions.anyField) {
                (_a = form.getAttribute("ipg_fieldname")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                (_b = form.getControl("ipg_fieldname")) === null || _b === void 0 ? void 0 : _b.setVisible(true);
                var fieldName = (_c = form.getAttribute("ipg_fieldname")) === null || _c === void 0 ? void 0 : _c.getValue();
            }
            else {
                (_d = form.getAttribute("ipg_fieldname")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("none");
                (_e = form.getControl("ipg_fieldname")) === null || _e === void 0 ? void 0 : _e.setVisible(false);
                var caseId = getRegardingCaseId(form);
                var description = getDescriptionForNotEnyFieldSubject(taskSubjectOptionName, caseId);
            }
        }
        function fieldNameOnChange(form) {
            var _a;
            var fieldName = (_a = form.getAttribute("ipg_fieldname")) === null || _a === void 0 ? void 0 : _a.getValue();
        }
        function taskSubjectOnChange(form) {
            var _a, _b;
            var taskSubjectValue = (_a = form.getAttribute("ipg_tasksubject")) === null || _a === void 0 ? void 0 : _a.getValue();
            var taskSubjectOptionName = (_b = form.getAttribute("ipg_tasksubject")) === null || _b === void 0 ? void 0 : _b.getSelectedOption().text;
            changeFieldPropertyByTaskSubject(form, taskSubjectValue, taskSubjectOptionName);
        }
        function setAssignedToTeam(form, teamName) {
            Xrm.WebApi.retrieveMultipleRecords('team', "?$select=name, teamid&$filter=contains(name,'".concat(teamName, "')")).then(function success(result) {
                var assignedToTeamAttr = form.getAttribute('ipg_assignedtoteamid');
                if (assignedToTeamAttr) {
                    assignedToTeamAttr.setValue([{
                            entityType: 'team',
                            name: result.entities[0].name,
                            id: result.entities[0].teamid
                        }]);
                }
            });
        }
        function setEndDate(endDateAttr, startDate, ipg_duedate) {
            if (endDateAttr && startDate) {
                if (ipg_duedate > 0) {
                    var parametersConfirm = {
                        "StartDate": startDate.toISOString(),
                        "BusinessDaysToAdd": ipg_duedate
                    };
                    callAction("ipg_IPGIntakeActionsAddBusinessDays", parametersConfirm, true, function (resultsConfirm) {
                        endDateAttr.setValue(new Date(resultsConfirm.ResultDate));
                        Xrm.Utility.closeProgressIndicator();
                    });
                }
                else {
                    endDateAttr.setValue(startDate);
                    Xrm.Utility.closeProgressIndicator();
                }
            }
        }
        function setDates(form, taskType) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var startDateAttr, endDateAttr, parametersConfirm;
                return __generator(this, function (_b) {
                    if (form.ui.getFormType() === 1 /* Create */
                        && form.getAttribute("scheduledstart")
                        && form.getAttribute("scheduledend")
                        && taskType) {
                        startDateAttr = form.getAttribute("scheduledstart");
                        endDateAttr = Xrm.Page.getAttribute("scheduledend");
                        if (taskType["ipg_startdate"] > 0) {
                            parametersConfirm = {
                                "StartDate": new Date().toISOString(),
                                "BusinessDaysToAdd": taskType["ipg_startdate"]
                            };
                            Xrm.Utility.showProgressIndicator("Processing...");
                            callAction("ipg_IPGIntakeActionsAddBusinessDays", parametersConfirm, true, function (resultsConfirm) {
                                var _a;
                                startDateAttr.setValue(new Date(resultsConfirm.ResultDate));
                                setEndDate(endDateAttr, startDateAttr.getValue(), (_a = taskType["ipg_duedate"]) !== null && _a !== void 0 ? _a : 0);
                            });
                        }
                        else {
                            startDateAttr.setValue(new Date());
                            setEndDate(endDateAttr, startDateAttr.getValue(), (_a = taskType["ipg_duedate"]) !== null && _a !== void 0 ? _a : 0);
                        }
                    }
                    return [2 /*return*/];
                });
            });
        }
        function ipg_taskblocksgatingOnChange(form) {
            var taskBlockGateAttr = form.getAttribute("ipg_taskblocksgating");
            var blockedgateAttr = form.getAttribute("ipg_blockedgateid");
            if (taskBlockGateAttr && blockedgateAttr) {
                var blockGateVisible = taskBlockGateAttr ? taskBlockGateAttr.getValue() : false;
                var blockedgateCtrl = form.getControl("ipg_blockedgateid");
                blockedgateCtrl.setVisible(blockGateVisible);
                blockedgateAttr.setRequiredLevel(blockGateVisible ? "required" : "none");
                if (!blockGateVisible) {
                    blockedgateAttr.setValue(null);
                    blockedgateAttr.setSubmitMode("always");
                }
            }
        }
        /**
         * Called on load a Task form
         * @function Intake.Task.OnLoadForm
         * @returns {void}
         */
        function OnLoadForm(context) {
            var _a, _b, _c, _d, _e;
            var form = context.getFormContext();
            makeSystemTaskReadOnly(form);
            var taskType = form.getAttribute("ipg_tasktypeid");
            var taskSubject = form.getAttribute("ipg_tasksubject");
            taskType.addOnChange(function () { return ipg_tasktypeidOnChange(form); });
            taskSubject.addOnChange(function () { return taskSubjectOnChange(form); });
            var taskBlockGateAttr = form.getAttribute("ipg_taskblocksgating");
            if (taskBlockGateAttr) {
                taskBlockGateAttr.addOnChange(function () { return ipg_taskblocksgatingOnChange(form); });
                taskBlockGateAttr.fireOnChange();
            }
            (_a = form.getAttribute("ipg_fieldname")) === null || _a === void 0 ? void 0 : _a.addOnChange(function () { fieldNameOnChange(form); });
            var ownerAttr = form.getAttribute("ownerid");
            var assignToTeamAttr = form.getAttribute("ipg_assignedtoteamid");
            ownerAttr === null || ownerAttr === void 0 ? void 0 : ownerAttr.addOnChange(OnOwnerChangeOrAssignedToteam);
            assignToTeamAttr === null || assignToTeamAttr === void 0 ? void 0 : assignToTeamAttr.addOnChange(OnOwnerChangeOrAssignedToteam);
            ownerAttr === null || ownerAttr === void 0 ? void 0 : ownerAttr.fireOnChange();
            if (form.ui.getFormType() === 1 /* Create */) {
                (_b = form.getAttribute("ipg_generatedbycode")) === null || _b === void 0 ? void 0 : _b.setValue(taskGeneratedBy.user); //set default as usertask
                (_c = form.getAttribute("ipg_generatedbycode")) === null || _c === void 0 ? void 0 : _c.setSubmitMode("always"); //set default as user
                var editablefields = ["subject", "ipg_taskcategoryid", "ipg_isvisibleonportal", "ipg_tasksubcategoryid", "description", "ipg_assignedtoteamid"];
                editablefields.forEach(function (f) { var _a; return (_a = form.getControl(f)) === null || _a === void 0 ? void 0 : _a.setDisabled(false); });
                var sStart = form.getAttribute("scheduledstart");
                if (sStart) {
                    sStart.setValue(new Date());
                }
                var taskAttr = form.getAttribute("ipg_taskcategoryid");
                taskAttr.addOnChange(OnTaskCategoryChange);
                taskAttr.fireOnChange();
            }
            AddCustomFilterForTaskCategory(form);
            AddCustomFilterForPortalUser(form);
            ConfigureEnableFields(form);
            OnTaskCategoryChange(context);
            (_d = form.getControl("statuscode")) === null || _d === void 0 ? void 0 : _d.removeOption(4); //Waiting on someone else
            (_e = form.getControl("statuscode")) === null || _e === void 0 ? void 0 : _e.removeOption(7); //Deferred
            filterOwnerLookupForPoolTask(form);
        }
        Task.OnLoadForm = OnLoadForm;
        /** Called on form save */
        // export function OnSave(context: Xrm.Events.SaveEventContext) {
        //   const formContext: Xrm.FormContext = context.getFormContext();
        //   const exceptionApprovedAttribute = formContext.getAttribute("ipg_is_exception_approved");
        //   if (exceptionApprovedAttribute?.getIsDirty() && exceptionApprovedAttribute?.getValue())
        //     Xrm.Navigation.navigateTo({
        //       pageType: "entityrecord",
        //       formId:"56EDCFE7-B4F3-4378-BAB5-FC8F40DB3AF3",
        //       entityName: "annotation",
        //       data: {
        //         "objectid": formContext.data.entity.getEntityReference()
        //       }
        //     }, {
        //       target: 2
        //     })
        //       .then(val => {}, error => console.error(error));
        // }
        function filterOwnerLookupForPoolTask(formContext) {
            var _a, _b;
            return __awaiter(this, void 0, void 0, function () {
                var taskType, assignedTeam, poolTeam;
                return __generator(this, function (_c) {
                    switch (_c.label) {
                        case 0:
                            taskType = (_a = formContext.getAttribute("ipg_tasktypeid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            assignedTeam = (_b = formContext.getAttribute("ipg_assignedtoteamid")) === null || _b === void 0 ? void 0 : _b.getValue();
                            if (!(taskType && taskType[0].name === "Request to Complete Case Mgmt. Work (Pool)")) return [3 /*break*/, 3];
                            if (!(assignedTeam && assignedTeam[0].name === "Pool")) return [3 /*break*/, 1];
                            setCustomOwnerLookupViewForPoolTask(formContext, assignedTeam[0].id);
                            return [3 /*break*/, 3];
                        case 1: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("team", "?$select=name,teamid&$filter=name eq 'Pool'")];
                        case 2:
                            poolTeam = _c.sent();
                            if (poolTeam.entities.length > 0) {
                                setCustomOwnerLookupViewForPoolTask(formContext, poolTeam.entities[0].id);
                            }
                            _c.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        }
        function setCustomOwnerLookupViewForPoolTask(formContext, poolTeamId) {
            var ownerControl = formContext.getControl("ownerid");
            if (ownerControl) {
                var viewId = "00000000-0000-0000-00AA-000013001121";
                var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">\n        <entity name=\"systemuser\">\n          <attribute name=\"fullname\" />\n          <attribute name=\"systemuserid\" />\n          <order attribute=\"fullname\" descending=\"false\" />\n          <link-entity name=\"teammembership\" from=\"systemuserid\" to=\"systemuserid\" visible=\"false\" intersect=\"true\">\n            <link-entity name=\"team\" from=\"teamid\" to=\"teamid\" alias=\"aa\">\n              <filter type=\"and\">\n                <condition attribute=\"teamid\" operator=\"eq\" value=\"".concat(poolTeamId, "\" />\n              </filter>\n            </link-entity>\n          </link-entity>\n        </entity>\n      </fetch>");
                var viewDisplayName = "Team Members";
                var layoutXml = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>\n      <row name='result' id='systemuserid'>\n      <cell name='fullname' width='300' />\n      </row>\n      </grid>";
                ownerControl.addCustomView(viewId, 'systemuser', viewDisplayName, fetchXml, layoutXml, true);
            }
        }
        function OnOwnerChangeOrAssignedToteam(event) {
            var _a, _b;
            return __awaiter(this, void 0, void 0, function () {
                var form, ownerAttr, assignToTeamAttr, ownerAttrVal, assignToTeamAttrVal;
                return __generator(this, function (_c) {
                    form = event.getFormContext();
                    ownerAttr = form.getAttribute("ownerid");
                    assignToTeamAttr = form.getAttribute("ipg_assignedtoteamid");
                    ownerAttrVal = ownerAttr === null || ownerAttr === void 0 ? void 0 : ownerAttr.getValue();
                    assignToTeamAttrVal = assignToTeamAttr === null || assignToTeamAttr === void 0 ? void 0 : assignToTeamAttr.getValue();
                    form.getControl("ipg_portaluser").setDisabled(!((ownerAttrVal === null || ownerAttrVal === void 0 ? void 0 : ownerAttrVal.length) > 0 && ((_a = ownerAttrVal[0].name) === null || _a === void 0 ? void 0 : _a.toLowerCase().indexOf("portal")) > -1
                        || (assignToTeamAttrVal === null || assignToTeamAttrVal === void 0 ? void 0 : assignToTeamAttrVal.length) > 0 && ((_b = assignToTeamAttrVal[0].name) === null || _b === void 0 ? void 0 : _b.toLowerCase().indexOf("portal")) > -1));
                    return [2 /*return*/];
                });
            });
        }
        function ConfigureEnableFields(form) {
            Xrm.Utility.getGlobalContext().getCurrentAppName().then(function (appname) {
                if (appname === "Collections") {
                    var subjVal = form.getAttribute("subject") && form.getAttribute("subject").getValue();
                    if (subjVal && subjVal.indexOf("Scheduled Payment Plan/Follow up Date") > -1) {
                        var startAttr = form.getAttribute("scheduledstart");
                        var endAttr = form.getAttribute("scheduledend");
                        startAttr.controls.forEach(function (control) {
                            control.setDisabled(false);
                        });
                        endAttr.controls.forEach(function (control) {
                            control.setDisabled(false);
                        });
                    }
                }
            });
        }
        Task.ConfigureEnableFields = ConfigureEnableFields;
        /**
         * Called on change a Task Category field
         * @function Intake.Task.OnChangeTaskCategory
         * @returns {void}
         */
        function OnChangeTaskCategory(executionContext) {
            var formContext = executionContext.getFormContext();
            makeSystemTaskReadOnly(formContext);
        }
        Task.OnChangeTaskCategory = OnChangeTaskCategory;
        /**
         * the function makes the form of system task read-only
         * @function Intake.Task.makeSystemTaskReadOnly
         * @returns {void}
         */
        function makeSystemTaskReadOnly(formContext) {
            var controls = formContext.getControl();
            if (!formContext.getAttribute("ipg_taskcategorycode"))
                return;
            var taskCategoryValue = formContext.getAttribute("ipg_taskcategorycode").getValue();
            if (controls) { //system task
                controls.forEach(function (control) {
                    if (taskCategoryValue === 427880001 && control.getVisible()) {
                        control.setDisabled(true);
                    }
                });
            }
        }
        /**
         * call Custom action
         * @function Intake.Incident.Ribbon.callAction
         * @returns {void}
        */
        function callAction(actionName, parameters, async, successCallback) {
            var req = new XMLHttpRequest();
            req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + actionName, async);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200) {
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
        function AddCustomFilterForPortalUser(form) {
            var portalUserCtr = form.getControl("ipg_portaluser");
            if (portalUserCtr) {
                portalUserCtr.addPreSearch(FilterPortalUser);
            }
        }
        function AddCustomFilterForTaskCategory(form) {
            return __awaiter(this, void 0, void 0, function () {
                var taskCategoryCtr, _a, _b;
                return __generator(this, function (_c) {
                    switch (_c.label) {
                        case 0:
                            taskCategoryCtr = form.getControl("ipg_taskcategoryid");
                            if (!taskCategoryCtr) return [3 /*break*/, 2];
                            _b = (_a = taskCategoryCtr).addPreSearch;
                            return [4 /*yield*/, filterTaskCategory(form)];
                        case 1:
                            _b.apply(_a, [_c.sent()]);
                            _c.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        function AddCustomFilterForTaskType(form) {
            var taskTypeCtr = form.getControl("ipg_tasktypeid");
            if (taskTypeCtr) {
                filterTaskTypes(form);
            }
        }
        function FilterPortalUser(executionContext) {
            var viewId = '00000000-0000-0000-0000-27f3954693a9';
            var formContext = executionContext.getFormContext();
            var taskTypeatr = formContext.getAttribute("ipg_tasktypecode");
            var taskTypeText = taskTypeatr && taskTypeatr.getText();
            var regardingatr = formContext.getAttribute("regardingobjectid");
            var regardingatrVal = regardingatr && regardingatr.getValue() && regardingatr.getValue().length > 0 && regardingatr.getValue()[0];
            if (regardingatrVal && regardingatrVal.entityType == 'incident'
                && taskTypeText && taskTypeText.toLowerCase().indexOf('missing information') > -1) {
                var ctr = formContext.getControl("ipg_portaluser");
                var layoutXml = "<grid name='contacts' object='2' jump='fullname' select='1' icon='1' preview='1'><row name='contact' id='contactid'><cell name='fullname'/></row></grid>";
                ctr.addCustomView(viewId, "contact", "Facility Contacts", BuildFetchForContactsByCaseFacility(regardingatrVal), layoutXml, true);
            }
        }
        function BuildFetchForContactsByCaseFacility(caseRef) {
            var fetchXml = [
                "<fetch>",
                "  <entity name='contact'>",
                "    <attribute name='fullname' />",
                "    <link-entity name='ipg_contactsaccounts' from='ipg_contactid' to='contactid' link-type='inner'>",
                "      <link-entity name='incident' from='ipg_facilityid' to='ipg_accountid' link-type='inner'>",
                "        <filter type='and'>",
                "          <condition attribute='incidentid' operator='eq' value='", caseRef.id, "'/>",
                "        </filter>",
                "      </link-entity>",
                "    </link-entity>",
                "  </entity>",
                "</fetch>",
            ].join("");
            return fetchXml;
        }
        function filterTaskCategory(form) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var regarding, entityType, regardingEntity, _b, validTaskCategories, _c;
                return __generator(this, function (_d) {
                    switch (_d.label) {
                        case 0:
                            regarding = (_a = form.getAttribute("regardingobjectid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            entityType = regarding.length > 0 ? regarding[0].entityType : null;
                            if (!(entityType && entityType == 'incident' || entityType == 'ipg_referral')) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord(entityType, regarding[0].id, "?$select=ipg_statecode")];
                        case 1:
                            _b = _d.sent();
                            return [3 /*break*/, 3];
                        case 2:
                            _b = null;
                            _d.label = 3;
                        case 3:
                            regardingEntity = _b;
                            if (!regardingEntity) return [3 /*break*/, 5];
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_tasktype", "?$select=_ipg_taskcategoryid_value&$filter=(ipg_generatedbycode eq ".concat(taskGeneratedBy.user, " or ipg_generatedbycode eq ").concat(taskGeneratedBy.both, ") and Microsoft.Dynamics.CRM.ContainValues(PropertyName=@p1,PropertyValues=@p2)&@p1='ipg_casestatecodes'&@p2=['").concat(regardingEntity.ipg_statecode, "']"))];
                        case 4:
                            _c = (_d.sent());
                            return [3 /*break*/, 6];
                        case 5:
                            _c = null;
                            _d.label = 6;
                        case 6:
                            validTaskCategories = _c;
                            return [2 /*return*/, function (executionContext) {
                                    var formContext = executionContext.getFormContext();
                                    var taskCategoryCtr = formContext.getControl("ipg_taskcategoryid");
                                    if (regardingEntity && validTaskCategories.entities.length > 0) {
                                        var fetchTaskCategoryValues = validTaskCategories.entities
                                            .map(function (x) { return x["_ipg_taskcategoryid_value"] ? "<value>" + x["_ipg_taskcategoryid_value"] + "</value>" : ""; }).join("");
                                        var filterXml = [
                                            "    <filter type='and'>",
                                            "      <condition attribute='ipg_taskcategoryid' operator='in' >",
                                            fetchTaskCategoryValues,
                                            "      </condition>",
                                            "    </filter>"
                                        ].join("");
                                        taskCategoryCtr.addCustomFilter(filterXml);
                                    }
                                    else {
                                        taskCategoryCtr.setDisabled(true);
                                        taskCategoryCtr.setNotification("Task for this case can not be created.", "taskCantBeCreated");
                                    }
                                }];
                    }
                });
            });
        }
        function filterTaskTypes(form) {
            var _a;
            var taskTypeCtr = form.getControl("ipg_tasktypeid");
            var regarding = (_a = form.getAttribute("regardingobjectid")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (regarding) {
                if (regarding.length > 0 && regarding[0].entityType == 'incident') {
                    Xrm.WebApi.retrieveRecord('incident', regarding[0].id, "?$select=ipg_statecode,ipg_casestatus").then(function success(incident) {
                        var taskCategory = form.getAttribute("ipg_taskcategoryid");
                        addTaskTypeCustomFilter(taskCategory, taskTypeCtr, incident.ipg_statecode, incident.ipg_casestatus);
                    });
                }
                else if (regarding.length > 0 && regarding[0].entityType == 'ipg_referral') {
                    Xrm.WebApi.retrieveRecord('ipg_referral', regarding[0].id, "?$select=ipg_statecode,ipg_casestatus").then(function success(ipg_referral) {
                        var taskCategory = form.getAttribute("ipg_taskcategoryid");
                        addTaskTypeCustomFilter(taskCategory, taskTypeCtr, ipg_referral.ipg_statecode, ipg_referral.ipg_casestatus);
                    });
                }
            }
        }
        function addTaskTypeCustomFilter(taskCategory, taskTypeCtr, statecode, casestatus) {
            if (taskCategory === null || taskCategory === void 0 ? void 0 : taskCategory.getValue()) {
                tempTaskTypeFilterFetch = [
                    "    <filter type='and'>",
                    "      <condition attribute='ipg_isactive' operator='eq' value='1'/>",
                    "      <condition attribute='statecode' operator='eq' value='0'/>",
                    "      <condition attribute='ipg_casestatecodes' operator='contain-values'>",
                    "       <value>", statecode, "</value>",
                    "      </condition>",
                    "      <condition attribute='ipg_taskcategoryid' operator = 'eq' value='", taskCategory.getValue()[0].id, "'/>",
                    "    <filter type='", casestatus != caseStatus.Closed ? 'or' : 'and', "'>",
                    "      <condition attribute='ipg_generatedbycode' operator = 'eq' value='", taskGeneratedBy.user, "'/>",
                    "      <condition attribute='ipg_generatedbycode' operator = 'eq' value='", taskGeneratedBy.both, "'/>",
                    "    </filter>",
                    "    </filter>"
                ].join("");
                var addFilter = function (executionContext) { return taskTypeCtr.addCustomFilter(tempTaskTypeFilterFetch); };
                taskTypeCtr.addPreSearch(addFilter);
            }
        }
        /**
       * add custom views to document type field
       * @function Intake.Task.addFiltersToDocumentType
       * @returns {void}
      */
        function OnTaskCategoryChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var entityLabel;
            var taskCategory = formContext.getAttribute("ipg_taskcategoryid");
            AddCustomFilterForTaskType(formContext);
            var tasktypeattr = formContext.getAttribute("ipg_tasktypeid");
            if (formContext.ui.getFormType() === 1) {
                if (taskCategory.getValue() != null) {
                    entityLabel = taskCategory.getValue()[0].name;
                    tasktypeattr.controls.forEach(function (c) { return c.setDisabled(false); });
                }
                else {
                    tasktypeattr.setValue(null);
                    tasktypeattr.controls.forEach(function (c) { return c.setDisabled(true); });
                }
            }
        }
        Task.OnTaskCategoryChange = OnTaskCategoryChange;
    })(Task = Intake.Task || (Intake.Task = {}));
})(Intake || (Intake = {}));
