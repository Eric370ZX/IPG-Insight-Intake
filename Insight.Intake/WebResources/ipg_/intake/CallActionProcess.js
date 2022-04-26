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
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.CallActionProcess
         * @returns {void}
         */
        function CallActionProcess(entityName, entityId, actionName, actionArguments) {
            if (entityId === void 0) { entityId = null; }
            if (actionArguments === void 0) { actionArguments = null; }
            var globalContext = Xrm.Utility.getGlobalContext();
            var clientUrl = globalContext.getClientUrl();
            if (!entityId) {
                var currentEntityId = Xrm.Page.data.entity.getId();
                entityId = Intake.Utility.removeCurlyBraces(currentEntityId);
            }
            var requestOptions = {
                path: clientUrl + "/api/data/v9.0/" + entityName + "(" + entityId + ")/Microsoft.Dynamics.CRM." + actionName,
                body: actionArguments || {},
                headers: {
                    'OData-MaxVersion': '4.0',
                    'OData-Version': '4.0',
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
            };
            return Intake.Utility.HttpRequest
                .post(requestOptions)
                .then(function (response) { return response; })
                .catch(function (response) {
                return Xrm.Navigation
                    .openErrorDialog({ message: response.error.message })
                    .then(function () { return null; });
            });
        }
        Utility.CallActionProcess = CallActionProcess;
        function CallActionProcessAsync(entityName, entityId, actionName, actionArguments) {
            if (entityId === void 0) { entityId = null; }
            if (actionArguments === void 0) { actionArguments = null; }
            return __awaiter(this, void 0, void 0, function () {
                var currentEntityId, path, json, resp, _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            if (!entityId) {
                                currentEntityId = Xrm.Page.data.entity.getId();
                                entityId = Intake.Utility.removeCurlyBraces(currentEntityId);
                            }
                            path = "/api/data/v9.0/" + entityName + "(" + entityId + ")/Microsoft.Dynamics.CRM." + actionName;
                            json = {};
                            _b.label = 1;
                        case 1:
                            _b.trys.push([1, 4, , 5]);
                            return [4 /*yield*/, fetch(path, {
                                    method: 'POST',
                                    headers: {
                                        'OData-MaxVersion': '4.0',
                                        'OData-Version': '4.0',
                                        'Accept': 'application/json',
                                        'Content-Type': 'application/json',
                                    },
                                    body: JSON.stringify(actionArguments)
                                })];
                        case 2:
                            resp = _b.sent();
                            return [4 /*yield*/, resp.json];
                        case 3:
                            json = _b.sent();
                            return [3 /*break*/, 5];
                        case 4:
                            _a = _b.sent();
                            return [3 /*break*/, 5];
                        case 5: return [2 /*return*/, json];
                    }
                });
            });
        }
        Utility.CallActionProcessAsync = CallActionProcessAsync;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
