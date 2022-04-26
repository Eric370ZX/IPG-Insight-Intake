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
 * @namespace Intake.PortalComment
 */
var Intake;
(function (Intake) {
    var PortalComment;
    (function (PortalComment) {
        var _entityName = "adx_portalcomment";
        function isRegardingReferralClosedAndHasAssociatedCase(selectedControlIds) {
            return __awaiter(this, void 0, void 0, function () {
                var results, _i, selectedControlIds_1, portalCommentId, isReferralClosedAndHasAssociatedCase;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            results = [];
                            _i = 0, selectedControlIds_1 = selectedControlIds;
                            _a.label = 1;
                        case 1:
                            if (!(_i < selectedControlIds_1.length)) return [3 /*break*/, 4];
                            portalCommentId = selectedControlIds_1[_i];
                            return [4 /*yield*/, CheckIfRegardingReferralClosedAndHasAssociatedCase(portalCommentId)["response"]];
                        case 2:
                            isReferralClosedAndHasAssociatedCase = _a.sent();
                            results.push(!isReferralClosedAndHasAssociatedCase);
                            _a.label = 3;
                        case 3:
                            _i++;
                            return [3 /*break*/, 1];
                        case 4: return [2 /*return*/, results.indexOf(false) <= 0];
                    }
                });
            });
        }
        PortalComment.isRegardingReferralClosedAndHasAssociatedCase = isRegardingReferralClosedAndHasAssociatedCase;
        function CheckIfRegardingReferralClosedAndHasAssociatedCase(portalCommentId) {
            return __awaiter(this, void 0, void 0, function () {
                var fetch, response;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            fetch = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\n                     <entity name=\"adx_portalcomment\">\n                       <attribute name=\"activityid\" />\n                       <attribute name=\"subject\" />\n                       <attribute name=\"createdon\" />\n                       <order attribute=\"subject\" descending=\"false\" />\n                       <filter type=\"and\">\n                         <condition attribute=\"activityid\" operator=\"eq\" uiname=\"reply\" uitype=\"adx_portalcomment\" value=\"" + portalCommentId + "\" />\n                       </filter>\n                       <link-entity name=\"ipg_referral\" from=\"ipg_referralid\" to=\"regardingobjectid\" link-type=\"inner\" alias=\"ac\">\n                         <filter type=\"and\">\n                           <condition attribute=\"ipg_casestatus\" operator=\"eq\" value=\"923720001\" />\n                           <condition attribute=\"ipg_associatedcaseid\" operator=\"not-null\" />\n                         </filter>\n                       </link-entity>\n                     </entity>\n                   </fetch>";
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('adx_portalcomment', "?fetchXml=" + fetch)];
                        case 1:
                            response = _a.sent();
                            return [2 /*return*/, {
                                    response: response && response.entities.length > 0 ? true : false
                                }];
                    }
                });
            });
        }
        /**
        * Open new PortalComment form from existing PortalComment
        * @function Intake.PortalComment.ReplyToPortalComment
        * @returns {void}
        */
        function ReplyToPortalComment(primaryControl) {
            var formContext = primaryControl;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "adx_portalcomment";
            entityFormOptions["useQuickCreateForm"] = false;
            var formParameters = {};
            if (formContext !== null) {
                formParameters["regardingobjectid"] = formContext.getAttribute("regardingobjectid").getValue();
                formParameters["to"] = formContext.getAttribute("from").getValue().filter(function (x) { return x.entityType === "contact"; });
                formParameters["description"] = "Regarding your note: \r\n" + formContext.getAttribute("description").getValue() + "\r\n";
                //From will default to current user
                //formParameters["from"] = formContext.getAttribute("to").getValue();
            }
            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                //formContext.data.refresh(true);
                console.log(success);
            }, function (error) {
                console.log(error);
            });
        }
        PortalComment.ReplyToPortalComment = ReplyToPortalComment;
        function ReplyToFromHomeGrid(selectedControl, selectedRecordId) {
            return __awaiter(this, void 0, void 0, function () {
                var $select, $expand, params, response, portalComment, regardingObject, to, description;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!selectedRecordId) return [3 /*break*/, 4];
                            selectedRecordId = selectedRecordId.replace("}", "").replace("{", "");
                            $select = "$select=_regardingobjectid_value,description";
                            $expand = "$expand=adx_portalcomment_activity_parties($filter=participationtypemask eq 1)";
                            params = "?" + $select + "&" + $expand;
                            return [4 /*yield*/, fetch("/api/data/v9.1/" + _entityName + "s(" + selectedRecordId + ")" + params, {
                                    method: "GET",
                                    headers: {
                                        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
                                    }
                                })];
                        case 1:
                            response = _a.sent();
                            return [4 /*yield*/, response.json()];
                        case 2:
                            portalComment = _a.sent();
                            return [4 /*yield*/, GetIncidentOrReferral(portalComment._regardingobjectid_value)];
                        case 3:
                            regardingObject = _a.sent();
                            regardingObject["name"] = portalComment["_regardingobjectid_value@OData.Community.Display.V1.FormattedValue"];
                            to = {
                                entityType: "contact",
                                id: portalComment.adx_portalcomment_activity_parties[0]["_partyid_value"],
                                name: portalComment.adx_portalcomment_activity_parties[0]["_partyid_value@OData.Community.Display.V1.FormattedValue"]
                            };
                            description = "Regarding your note: \r\n" + portalComment.description + "\r\n";
                            OpenPrefilledFromForReply([regardingObject], [to], description);
                            _a.label = 4;
                        case 4: return [2 /*return*/];
                    }
                });
            });
        }
        PortalComment.ReplyToFromHomeGrid = ReplyToFromHomeGrid;
        function RetriveSaleOrdersByIds(selectedRecordIds) {
            return __awaiter(this, void 0, void 0, function () {
                var idsQueryString;
                return __generator(this, function (_a) {
                    idsQueryString = "&$filter=";
                    selectedRecordIds.forEach(function (id) {
                        idsQueryString += "activityid eq " + id;
                        if (selectedRecordIds.indexOf(id) != selectedRecordIds.length - 1)
                            idsQueryString += " or ";
                    });
                    return [2 /*return*/, Xrm.WebApi.retrieveMultipleRecords(_entityName, "?$select=statuscode" + idsQueryString)];
                });
            });
        }
        function MarkAsUnreadEnableRule(selectedRecordIds) {
            return __awaiter(this, void 0, void 0, function () {
                var retrivedRecords;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, RetriveSaleOrdersByIds(selectedRecordIds)];
                        case 1:
                            retrivedRecords = _a.sent();
                            if (retrivedRecords.entities.length > 0) {
                                return [2 /*return*/, !retrivedRecords.entities.some(function (comment) { return comment["statuscode"] != 427880000; } /* Viewed */)];
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        PortalComment.MarkAsUnreadEnableRule = MarkAsUnreadEnableRule;
        function MarkAsReadEnableRule(selectedRecordIds) {
            return __awaiter(this, void 0, void 0, function () {
                var retrivedRecords;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, RetriveSaleOrdersByIds(selectedRecordIds)];
                        case 1:
                            retrivedRecords = _a.sent();
                            if (retrivedRecords.entities.length > 0) {
                                return [2 /*return*/, !retrivedRecords.entities.some(function (comment) { return comment["statuscode"] == 427880000; } /* Viewed */)];
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        PortalComment.MarkAsReadEnableRule = MarkAsReadEnableRule;
        function MarkAsViewed(selectedControl, selectedRecordIds) {
            return __awaiter(this, void 0, void 0, function () {
                var _i, selectedRecordIds_1, recordId;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator("Updating the records");
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, , 6, 7]);
                            _i = 0, selectedRecordIds_1 = selectedRecordIds;
                            _a.label = 2;
                        case 2:
                            if (!(_i < selectedRecordIds_1.length)) return [3 /*break*/, 5];
                            recordId = selectedRecordIds_1[_i];
                            return [4 /*yield*/, Xrm.WebApi.updateRecord(_entityName, recordId, {
                                    statuscode: 427880000,
                                    statecode: 1,
                                    ipg_markedread: true
                                })];
                        case 3:
                            _a.sent();
                            _a.label = 4;
                        case 4:
                            _i++;
                            return [3 /*break*/, 2];
                        case 5: return [3 /*break*/, 7];
                        case 6:
                            Xrm.Utility.closeProgressIndicator();
                            (selectedControl.data && selectedControl.data.refresh()) || selectedControl.refresh();
                            return [7 /*endfinally*/];
                        case 7: return [2 /*return*/];
                    }
                });
            });
        }
        PortalComment.MarkAsViewed = MarkAsViewed;
        function MarkAsUnread(selectedControl, selectedRecordIds) {
            return __awaiter(this, void 0, void 0, function () {
                var _i, selectedRecordIds_2, recordId;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator("Updating the records");
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, , 6, 7]);
                            _i = 0, selectedRecordIds_2 = selectedRecordIds;
                            _a.label = 2;
                        case 2:
                            if (!(_i < selectedRecordIds_2.length)) return [3 /*break*/, 5];
                            recordId = selectedRecordIds_2[_i];
                            return [4 /*yield*/, Xrm.WebApi.updateRecord(_entityName, recordId, {
                                    statuscode: 923720000,
                                    statecode: 0,
                                    ipg_markedread: false
                                })];
                        case 3:
                            _a.sent();
                            _a.label = 4;
                        case 4:
                            _i++;
                            return [3 /*break*/, 2];
                        case 5: return [3 /*break*/, 7];
                        case 6:
                            Xrm.Utility.closeProgressIndicator();
                            (selectedControl.data && selectedControl.data.refresh()) || selectedControl.refresh();
                            return [7 /*endfinally*/];
                        case 7: return [2 /*return*/];
                    }
                });
            });
        }
        PortalComment.MarkAsUnread = MarkAsUnread;
        function OpenPrefilledFromForReply(regardingObject, to, description) {
            var entityFormOptions = {
                entityName: "adx_portalcomment",
                useQuickCreateForm: false
            };
            var formParameters = {};
            formParameters["regardingobjectid"] = regardingObject;
            formParameters["to"] = to;
            formParameters["description"] = description;
            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                console.log(success);
            }, function (error) {
                console.log(error);
            });
        }
        function GetIncidentOrReferral(recordId) {
            return __awaiter(this, void 0, void 0, function () {
                var exc_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            _a.trys.push([0, 2, , 4]);
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", recordId, "?$select=title")];
                        case 1:
                            _a.sent();
                            return [2 /*return*/, {
                                    id: recordId,
                                    entityType: "incident"
                                }];
                        case 2:
                            exc_1 = _a.sent();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_referral", recordId, "?$select=ipg_name")];
                        case 3:
                            _a.sent();
                            return [2 /*return*/, {
                                    id: recordId,
                                    entityType: "ipg_referral"
                                }];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        }
        function enableViewedButtonOnForm(primaryControl) {
            var formContext = primaryControl;
            var statusAttr = formContext.getAttribute("statuscode");
            if (statusAttr && statusAttr.getValue() && statusAttr.getValue() !== 427880000) {
                return true;
            }
            return false;
        }
        PortalComment.enableViewedButtonOnForm = enableViewedButtonOnForm;
        function enableNewCommentButtonOnRibbon(primaryControl) {
            if (primaryControl) {
                if (primaryControl == "ipg_referral") {
                    return false;
                }
            }
            return true;
        }
        PortalComment.enableNewCommentButtonOnRibbon = enableNewCommentButtonOnRibbon;
    })(PortalComment = Intake.PortalComment || (Intake.PortalComment = {}));
})(Intake || (Intake = {}));
