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
    var Case;
    (function (Case) {
        var Assign;
        (function (Assign) {
            var selectedRecordLabel = "You have selected <&N> record(-s). Please indicate the User and/or Team to which these records should be assigned.";
            var confirmtring = "System is about to reassign all related open User Tasks to the User you assigned this Case to. Do you wish to proceed?";
            var assignTypes = {
                me: "Me",
                userOrTeam: "User or team"
            };
            var selectedRecordsIds;
            var Xrm;
            function InitData() {
                return __awaiter(this, void 0, void 0, function () {
                    var error_1;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                _a.trys.push([0, 2, , 3]);
                                selectedRecordsIds = JSON.parse(GetUrlParameter("data"))
                                    .selectedRecordsIds;
                                if (!Array.isArray(selectedRecordsIds)) {
                                    selectedRecordsIds = new Array(selectedRecordsIds
                                        .replace("{", "")
                                        .replace("}", "")
                                        .toLocaleLowerCase());
                                }
                                Xrm = window.parent.Xrm;
                                SetSelectedRecordsLabel(selectedRecordsIds.length);
                                return [4 /*yield*/, FillTeams()];
                            case 1:
                                _a.sent();
                                return [3 /*break*/, 3];
                            case 2:
                                error_1 = _a.sent();
                                debugger;
                                console.log(error_1);
                                return [3 /*break*/, 3];
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            }
            Assign.InitData = InitData;
            function SetSelectedRecordsLabel(count) {
                document.getElementById("selected-records-label").innerText =
                    selectedRecordLabel.replace("<&N>", count.toString());
            }
            function RetrieveAllTeamRecords() {
                var teamFetchXML = "<?xml version='1.0'?>" +
                    "<fetch distinct='false' mapping='logical' outputformat='xmlplatform' version='1.0'>" +
                    "<entity name='team'>" +
                    "</entity>" +
                    "</fetch>";
                teamFetchXML = "?fetchXml=" + encodeURIComponent(teamFetchXML);
                return Xrm.WebApi.retrieveMultipleRecords("team", teamFetchXML);
            }
            function FillTeams() {
                return __awaiter(this, void 0, void 0, function () {
                    var teams, select;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, RetrieveAllTeamRecords()];
                            case 1:
                                teams = _a.sent();
                                select = document.getElementById("assign-to-team-select");
                                teams.entities.forEach(function (team) {
                                    select.options[select.options.length] = new Option(team.name, team.teamid);
                                });
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function IsMeAssignTypeSelected() {
                var select = document.getElementById("assign-to-type-select");
                var selected = select.options[select.selectedIndex].text;
                if (selected === assignTypes.userOrTeam) {
                    return false;
                }
                return true;
            }
            function GetTeamsByLoggedUser() {
                return __awaiter(this, void 0, void 0, function () {
                    var loggedUserId, teamFetchXML;
                    return __generator(this, function (_a) {
                        loggedUserId = Xrm.Utility.getGlobalContext()
                            .userSettings.userId.toLowerCase()
                            .replace("{", "")
                            .replace("}", "");
                        teamFetchXML = "<?xml version='1.0'?>\n     <fetch distinct='true' mapping='logical' outputformat='xmlplatform' version='1.0'>\n      <entity name='team'>\n      <attribute name='name'/>\n      <attribute name='teamid'/>\n        <link-entity name='teammembership' intersect='true' visible='false' to='teamid' from='teamid'>\n          <link-entity name='systemuser' to='systemuserid' from='systemuserid' alias='ab'>\n            <filter type='and'>\n              <condition attribute='systemuserid' value='" + loggedUserId + "' operator='eq'/>\n            </filter>\n          </link-entity>\n        </link-entity>\n      </entity>\n     </fetch>";
                        teamFetchXML = "?fetchXml=" + encodeURIComponent(teamFetchXML);
                        return [2 /*return*/, Xrm.WebApi.retrieveMultipleRecords("team", teamFetchXML)];
                    });
                });
            }
            function PopulateAssignedToTeamByDefault() {
                return __awaiter(this, void 0, void 0, function () {
                    var teams, select;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, GetTeamsByLoggedUser()];
                            case 1:
                                teams = (_a.sent()).entities;
                                if (teams.length == 1) {
                                    select = document.getElementById("assign-to-team-select");
                                    select.options[0] = new Option(teams[0].name, teams[0].teamid);
                                }
                                else
                                    return [2 /*return*/];
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function OnAssignToTypeChanged() {
                var isMeAssignTypeSelected = IsMeAssignTypeSelected();
                if (!isMeAssignTypeSelected) {
                    PopulateAssignedToTeamByDefault();
                }
                document.getElementById("assign-to-team-select").disabled =
                    isMeAssignTypeSelected;
                document.getElementById("assign-to-user-select").disabled =
                    isMeAssignTypeSelected;
                if (isMeAssignTypeSelected) {
                    ClearSelectById("assign-to-team-select");
                    ClearSelectById("assign-to-user-select");
                }
            }
            Assign.OnAssignToTypeChanged = OnAssignToTypeChanged;
            function GetUrlParameter(sParam) {
                var sPageURL = window.location.search.substring(1), sURLVariables = sPageURL.split("&"), sParameterName, i;
                for (i = 0; i < sURLVariables.length; i++) {
                    sParameterName = sURLVariables[i].split("=");
                    if (sParameterName[0] === sParam) {
                        return sParameterName[1] === undefined
                            ? ""
                            : decodeURIComponent(sParameterName[1]);
                    }
                }
            }
            function ProcessAssign() {
                if (IsInputDataValid())
                    AssignCasesRequest();
                else
                    OnError("Validation failed", "Please select any team ");
            }
            Assign.ProcessAssign = ProcessAssign;
            function CloseDialog() {
                window.close();
            }
            Assign.CloseDialog = CloseDialog;
            function GetUsersByTeam(teamId) {
                var usersByTeamFetch = "<?xml version='1.0'?>\n    <fetch distinct='true' mapping='logical' version='1.0'>\n      <entity name='systemuser'>\n        <attribute name='fullname'/>\n        <attribute name='systemuserid'/>\n          <filter type='and'>\n            <condition attribute='isdisabled' value='0' operator='eq'/>\n          </filter>\n        <link-entity name='teammembership' intersect='true' visible='false' to='systemuserid' from='systemuserid'>\n          <link-entity name='team' to='teamid' from='teamid'>\n            <filter type='and'>\n              <condition attribute='teamid' value='" + teamId + "' uitype='team' operator='eq'/>\n            </filter>\n          </link-entity>\n        </link-entity>\n      </entity>\n    </fetch>";
                usersByTeamFetch = "?fetchXml=" + encodeURIComponent(usersByTeamFetch);
                return Xrm.WebApi.retrieveMultipleRecords("systemuser", usersByTeamFetch);
            }
            function FillUsersBySelectedTeam() {
                return __awaiter(this, void 0, void 0, function () {
                    var users, select;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, GetUsersByTeam(document.getElementById("assign-to-team-select").value)];
                            case 1:
                                users = _a.sent();
                                select = document.getElementById("assign-to-user-select");
                                users.entities.forEach(function (user) {
                                    select.options[select.options.length] = new Option(user.fullname, user.systemuserid);
                                });
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Assign.FillUsersBySelectedTeam = FillUsersBySelectedTeam;
            function ClearSelectById(id) {
                $("#" + id)
                    .find("option")
                    .remove()
                    .end()
                    .append('<option selected value="0"></option>');
            }
            Assign.ClearSelectById = ClearSelectById;
            function DoAssignToUser() {
                return (document.getElementById("assign-to-user-select").value != "0");
            }
            function CanCaseBeUpdated(id) {
                return __awaiter(this, void 0, void 0, function () {
                    var caseRecord;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("incident", id, "?$select=title")];
                            case 1:
                                caseRecord = _a.sent();
                                return [2 /*return*/, caseRecord.title != null && caseRecord.title !== ""];
                        }
                    });
                });
            }
            function OnError(title, text) {
                var alertStrings = {
                    confirmButtonLabel: "Ok",
                    text: text,
                    title: title,
                };
                var alertOptions = { height: 120, width: 260 };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                    Xrm.Utility.closeProgressIndicator();
                    window.close();
                });
            }
            function IsInputDataValid() {
                return IsMeAssignTypeSelected()
                    ? true
                    : document.getElementById("assign-to-team-select").value != "0";
            }
            function AssignCasesRequest() {
                return __awaiter(this, void 0, void 0, function () {
                    var assignToTeam, assignToUser, isUserSelected, selectedUserOrTeamId, hasError, entity, result, _loop_1, i, state_1;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                Xrm.Utility.showProgressIndicator("Processing...");
                                assignToTeam = document.getElementById("assign-to-team-select").value;
                                assignToUser = document.getElementById("assign-to-user-select").value;
                                isUserSelected = DoAssignToUser() || IsMeAssignTypeSelected();
                                selectedUserOrTeamId = IsMeAssignTypeSelected()
                                    ? Xrm.Utility.getGlobalContext()
                                        .userSettings.userId.toLowerCase()
                                        .replace("{", "")
                                        .replace("}", "")
                                    : DoAssignToUser()
                                        ? assignToUser
                                        : assignToTeam;
                                entity = {};
                                if (assignToTeam) {
                                    entity["ipg_assignedtoteamid@odata.bind"] = "/teams(" + assignToTeam + ")";
                                }
                                if (!isUserSelected) return [3 /*break*/, 2];
                                return [4 /*yield*/, Xrm.Navigation.openConfirmDialog({ text: confirmtring })];
                            case 1:
                                result = _a.sent();
                                if (!result.confirmed) {
                                    Xrm.Utility.closeProgressIndicator();
                                    return [2 /*return*/];
                                }
                                entity["ownerid@odata.bind"] = "/systemusers(" + selectedUserOrTeamId + ")";
                                return [3 /*break*/, 3];
                            case 2:
                                entity["ownerid@odata.bind"] = "/teams(" + selectedUserOrTeamId + ")";
                                _a.label = 3;
                            case 3:
                                _loop_1 = function (i) {
                                    var isCasesValid;
                                    return __generator(this, function (_a) {
                                        switch (_a.label) {
                                            case 0: return [4 /*yield*/, CanCaseBeUpdated(selectedRecordsIds[i])];
                                            case 1:
                                                isCasesValid = _a.sent();
                                                if (hasError || !isCasesValid) {
                                                    OnError("Error", "Selected record(-s) can not be updated");
                                                    return [2 /*return*/, "break"];
                                                }
                                                Xrm.WebApi.online
                                                    .updateRecord("incident", selectedRecordsIds[i], entity)
                                                    .then(function success(result) {
                                                    if (i == selectedRecordsIds.length - 1) {
                                                        var alertStrings = {
                                                            confirmButtonLabel: "Ok",
                                                            text: "Records were seccessfuly updated",
                                                            title: "Success",
                                                        };
                                                        var alertOptions = { height: 120, width: 260 };
                                                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                                                            Xrm.Utility.closeProgressIndicator();
                                                            window.close();
                                                        });
                                                    }
                                                }, function (error) {
                                                    Xrm.Utility.closeProgressIndicator();
                                                    Xrm.Navigation.openAlertDialog(error.message);
                                                    hasError = true;
                                                });
                                                return [2 /*return*/];
                                        }
                                    });
                                };
                                i = 0;
                                _a.label = 4;
                            case 4:
                                if (!(i < selectedRecordsIds.length)) return [3 /*break*/, 7];
                                return [5 /*yield**/, _loop_1(i)];
                            case 5:
                                state_1 = _a.sent();
                                if (state_1 === "break")
                                    return [3 /*break*/, 7];
                                _a.label = 6;
                            case 6:
                                i++;
                                return [3 /*break*/, 4];
                            case 7: return [2 /*return*/];
                        }
                    });
                });
            }
        })(Assign = Case.Assign || (Case.Assign = {}));
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
$(function () {
    Intake.Case.Assign.InitData();
    $("#assign-to-type-select")
        .on("change", function () {
        Intake.Case.Assign.OnAssignToTypeChanged();
    })
        .trigger("change");
    $("#okBtn").on("click", function () {
        Intake.Case.Assign.ProcessAssign();
    });
    $("#cancelBtn").on("click", function () {
        Intake.Case.Assign.CloseDialog();
    });
    $("#assign-to-team-select").on("change", function () {
        Intake.Case.Assign.ClearSelectById("assign-to-user-select");
        Intake.Case.Assign.FillUsersBySelectedTeam();
    });
});
