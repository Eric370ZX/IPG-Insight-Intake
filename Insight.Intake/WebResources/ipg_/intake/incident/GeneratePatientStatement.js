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
 * @namespace Intake.Incident
 */
var Intake;
(function (Intake) {
    var Incident;
    (function (Incident) {
        var Xrm = window.Xrm || window.top.Xrm;
        var incidentId;
        var statementTypesSelectElementId = "statementTypesSelect";
        var deliveryMethodSelectElementId = "delivetyMethodSelect";
        var DelivaryMethod = {
            Paper: "Paper",
            Electronic: "Electronic"
        };
        var CaseStatesEnum;
        (function (CaseStatesEnum) {
            CaseStatesEnum[CaseStatesEnum["Intake"] = 923720000] = "Intake";
            CaseStatesEnum[CaseStatesEnum["Authorizations"] = 923720001] = "Authorizations";
            CaseStatesEnum[CaseStatesEnum["CaseManagement"] = 923720002] = "CaseManagement";
            CaseStatesEnum[CaseStatesEnum["Billing"] = 923720003] = "Billing";
            CaseStatesEnum[CaseStatesEnum["CarrierService"] = 923720004] = "CarrierService";
            CaseStatesEnum[CaseStatesEnum["PatientServices"] = 923720005] = "PatientServices";
            CaseStatesEnum[CaseStatesEnum["Finance"] = 923720006] = "Finance";
        })(CaseStatesEnum || (CaseStatesEnum = {}));
        ;
        var CaseStatusEnum;
        (function (CaseStatusEnum) {
            CaseStatusEnum[CaseStatusEnum["Open"] = 923720000] = "Open";
            CaseStatusEnum[CaseStatusEnum["Closed"] = 923720001] = "Closed";
        })(CaseStatusEnum || (CaseStatusEnum = {}));
        ;
        function InitGenerateStatementPage() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            incidentId = (new URLSearchParams(window.location.search)).get("data");
                            if (!incidentId) {
                                Xrm.Navigation.openErrorDialog({ message: 'Error. Incident ID parameter is required' });
                                return [2 /*return*/, null];
                            }
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, , 4, 5]);
                            Xrm.Utility.showProgressIndicator(null);
                            return [4 /*yield*/, populateDeliveryMethod()];
                        case 2:
                            _a.sent();
                            return [4 /*yield*/, populateStatementTypes()];
                        case 3:
                            _a.sent();
                            document.getElementById(deliveryMethodSelectElementId).addEventListener('change', populateStatementTypes);
                            return [3 /*break*/, 5];
                        case 4:
                            Xrm.Utility.closeProgressIndicator();
                            return [7 /*endfinally*/];
                        case 5: return [2 /*return*/];
                    }
                });
            });
        }
        Incident.InitGenerateStatementPage = InitGenerateStatementPage;
        function GeneratePatientStatement() {
            return __awaiter(this, void 0, void 0, function () {
                var statementTypesSelectElement, delivaryMethodSelectElement, finalConfirmtext, apiUrl, response, doc, createddoc, associateRequest, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            statementTypesSelectElement = document.getElementById(statementTypesSelectElementId);
                            delivaryMethodSelectElement = document.getElementById(deliveryMethodSelectElementId);
                            if (!statementTypesSelectElement.value || statementTypesSelectElement.value == "0") {
                                Xrm.Navigation.openAlertDialog({ text: 'Please select a Statement Type' });
                                return [2 /*return*/];
                            }
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 9, 10, 11]);
                            Xrm.Utility.showProgressIndicator(null);
                            finalConfirmtext = "Before generating this Statement, please ensure the Patient Address for this Case is correct.";
                            return [4 /*yield*/, checkPendingPatientStatement()];
                        case 2:
                            if (_a.sent()) {
                                finalConfirmtext = "A statement for this Patient is scheduled to generate within next 5 days. Do you wish to still generate this additional statement?";
                            }
                            else if (delivaryMethodSelectElement.value == DelivaryMethod.Paper) {
                                finalConfirmtext += " Please note that generating a paper Statement does not modify future Start Date or Due Date values, nor does it modify the overall Patient Statement sequence.";
                            }
                            else {
                                finalConfirmtext += " By selecting the 'Generate Statement' button, the Start Date and Due Date values for this and all future Statements will be reset and overall Patient Statements sequence will be modified.";
                            }
                            return [4 /*yield*/, Xrm.Navigation.openConfirmDialog({
                                    text: finalConfirmtext,
                                    confirmButtonLabel: "Generate Statement",
                                    cancelButtonLabel: "Cancel"
                                })];
                        case 3:
                            if (!(_a.sent()).confirmed) {
                                return [2 /*return*/];
                            }
                            return [4 /*yield*/, getUriFromGlobalSettings()];
                        case 4:
                            apiUrl = _a.sent();
                            return [4 /*yield*/, fetch(apiUrl, { method: 'POST',
                                    body: JSON.stringify({
                                        "CaseId": incidentId,
                                        "StatementType": statementTypesSelectElement.value
                                    }) })];
                        case 5:
                            response = _a.sent();
                            return [4 /*yield*/, response.json()];
                        case 6:
                            doc = _a.sent();
                            doc.ipg_deliverymethodcode = 427880000;
                            return [4 /*yield*/, Xrm.WebApi.createRecord("ipg_document", doc)];
                        case 7:
                            createddoc = _a.sent();
                            associateRequest = {
                                getMetadata: function () { return ({
                                    boundParameter: null,
                                    parameterTypes: {},
                                    operationType: 2,
                                    operationName: "Associate"
                                }); },
                                relationship: "ipg_incident_ipg_document",
                                target: {
                                    entityType: "incident",
                                    id: incidentId
                                },
                                relatedEntities: [
                                    {
                                        entityType: "ipg_document",
                                        id: createddoc.id
                                    }
                                ]
                            };
                            return [4 /*yield*/, Xrm.WebApi.online.execute(associateRequest)];
                        case 8:
                            _a.sent();
                            Xrm.Navigation.openAlertDialog({ text: "Patient Statement Generated!" }).then(function () { return window.close(); });
                            return [3 /*break*/, 11];
                        case 9:
                            e_1 = _a.sent();
                            Xrm.Navigation.openErrorDialog({ message: "Failed Generated PS. Try Later or Please Contact System Administrator!" });
                            return [3 /*break*/, 11];
                        case 10:
                            Xrm.Utility.closeProgressIndicator();
                            return [7 /*endfinally*/];
                        case 11: return [2 /*return*/];
                    }
                });
            });
        }
        Incident.GeneratePatientStatement = GeneratePatientStatement;
        function populateStatementTypes() {
            return __awaiter(this, void 0, void 0, function () {
                var deliveryMethodt, statementTypesSelectElement, i, L, availableTemplates, filter, template, result, _loop_1, i_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator(null);
                            deliveryMethodt = document.getElementById(deliveryMethodSelectElementId).value;
                            statementTypesSelectElement = document.getElementById(statementTypesSelectElementId);
                            //Clear options
                            statementTypesSelectElement.disabled = false;
                            L = statementTypesSelectElement.options.length - 1;
                            for (i = L; i >= 0; i--) {
                                statementTypesSelectElement.remove(i);
                            }
                            availableTemplates = ['D1', 'D2', 'A2', 'A3', 'A5', 'S1', 'S2', 'S3'];
                            if (deliveryMethodt == DelivaryMethod.Electronic) {
                                statementTypesSelectElement.disabled = true;
                                availableTemplates.unshift('P1');
                            }
                            filter = "";
                            for (template in availableTemplates) {
                                filter += (filter ? ' or ' : '') + ("endswith(ipg_DocumentTypeId%2fipg_documenttypeabbreviation, '" + template + "')");
                            }
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_document', "?$select=ipg_documentid&$orderby=createdon desc" + (deliveryMethodt == DelivaryMethod.Electronic ? "&$top=10" : "") + "&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)&$filter=(_ipg_caseid_value eq '" + incidentId + "' and ipg_isactivepatientstatement eq true) and (" + filter + ")")];
                        case 1:
                            result = _a.sent();
                            _loop_1 = function (i_1) {
                                var abr = availableTemplates[i_1];
                                var option = result.entities.find(function (x) { return abr == x.ipg_DocumentTypeId.ipg_documenttypeabbreviation.replace("PST_", ""); });
                                if (option) {
                                    addOption(statementTypesSelectElementId, abr, result.entities.indexOf(option) == 0);
                                }
                            };
                            for (i_1 = 0; i_1 < availableTemplates.length; i_1++) {
                                _loop_1(i_1);
                            }
                            Xrm.Utility.closeProgressIndicator();
                            return [2 /*return*/];
                    }
                });
            });
        }
        function populateDeliveryMethod() {
            return __awaiter(this, void 0, void 0, function () {
                var incident;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord('incident', incidentId, "?$select=ipg_billtopatient,ipg_remainingpatientbalance,ipg_statecode,ipg_casestatus")];
                        case 1:
                            incident = _a.sent();
                            if (incident.ipg_billtopatient
                                && incident.ipg_remainingpatientbalance > 0
                                && incident.ipg_statecode == CaseStatesEnum.PatientServices
                                && incident.ipg_casestatus == CaseStatusEnum.Open) {
                                addOption(deliveryMethodSelectElementId, DelivaryMethod.Electronic);
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function getUriFromGlobalSettings() {
            return __awaiter(this, void 0, void 0, function () {
                var setting;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_globalsetting', "?$top=1&$select=ipg_value&$filter=ipg_name eq 'GeneratePSURL'")];
                        case 1:
                            setting = _a.sent();
                            return [2 /*return*/, setting.entities[0].ipg_value];
                    }
                });
            });
        }
        function checkPendingPatientStatement() {
            return __awaiter(this, void 0, void 0, function () {
                var tasks;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_statementgenerationtask', "?$select=ipg_statementgenerationtaskid&$top=1&$filter=_ipg_caseid_value eq " + incidentId + " and ipg_startdate le " + addDays(new Date(), 5).toISOString() + " and statecode eq 0")];
                        case 1:
                            tasks = _a.sent();
                            return [2 /*return*/, tasks.entities.length > 0];
                    }
                });
            });
        }
        function addDays(date, days) {
            var result = new Date(date);
            result.setDate(result.getDate() + days);
            return result;
        }
        function addOption(elId, optVal, seleceted) {
            if (seleceted === void 0) { seleceted = null; }
            var deliveryMethodSelectElement = document.getElementById(elId);
            var newOption = document.createElement("option");
            newOption.value = optVal;
            newOption.text = optVal;
            if (seleceted) {
                newOption.selected = seleceted;
            }
            deliveryMethodSelectElement.add(newOption);
        }
    })(Incident = Intake.Incident || (Intake.Incident = {}));
})(Intake || (Intake = {}));
document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("generateBtn").addEventListener('click', Intake.Incident.GeneratePatientStatement, false);
    document.getElementById("cancelBtn").addEventListener('click', function () { return window.close(); }, false);
    Intake.Incident.InitGenerateStatementPage();
});
