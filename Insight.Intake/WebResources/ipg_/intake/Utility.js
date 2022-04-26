/**
 * @namespace Intake.Utility
 */
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
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
    var Utility;
    (function (Utility) {
        var Address = /** @class */ (function () {
            function Address() {
            }
            return Address;
        }());
        Utility.Address = Address;
        ;
        var RemittanceAddress = /** @class */ (function () {
            function RemittanceAddress() {
            }
            return RemittanceAddress;
        }());
        Utility.RemittanceAddress = RemittanceAddress;
        ;
        var Zip = /** @class */ (function () {
            function Zip() {
            }
            return Zip;
        }());
        Utility.Zip = Zip;
        ;
        var State = /** @class */ (function () {
            function State() {
            }
            return State;
        }());
        Utility.State = State;
        /**
         * @class EventEmitter
         */
        var EventEmitter = /** @class */ (function () {
            /**
             * @constructor
             */
            function EventEmitter() {
                this.listeners = [];
            }
            /**
             * Adds an event listener for the specified event.
             * @method EventEmitter#addEventListener
             * @template TPayload :: Type of payload.
             * @param {string} event :: Event name.
             * @param {Listener<TPayload>} listener :: Callback function.
             * @returns {void}
             * @throws {TypeError} If the listener is not a function, it throws a type error.
             */
            EventEmitter.prototype.addEventListener = function (event, listener) {
                if ('function' !== typeof listener) {
                    throw new TypeError('Listener should be function.');
                }
                this.listeners.push({ event: event, listener: listener });
            };
            /**
             * Removes a listener for the specified event.
             * @method EventEmitter#removeEventListener
             * @template TPayload :: Type of payload.
             * @param {string} event :: Event name.
             * @param {Listener<TPayload>} listener :: Callback function.
             * @returns {void}
             */
            EventEmitter.prototype.removeEventListener = function (event, listener) {
                for (var index = 0, length_1 = this.listeners.length; index < length_1; index++) {
                    var eventListener = this.listeners[index];
                    if (eventListener.event === event && eventListener.listener === listener) {
                        this.listeners.splice(index, 1);
                    }
                }
            };
            /**
             * Emits an event executing all appropriate listeners.
             * @method EventEmitter#emit
             * @template TPayload :: Type of payload.
             * @param {string} event :: Event name.
             * @param {TPayload} payload :: Payload.
             * @returns {void}
             */
            EventEmitter.prototype.emit = function (event, payload) {
                var _this = this;
                if (payload === void 0) { payload = {}; }
                var numberOfExistingListeners = 0;
                var numberOfAttempts = 0;
                var emitClosure = function () {
                    numberOfAttempts++;
                    for (var index = 0, length_2 = _this.listeners.length; index < length_2; index++) {
                        var eventListener = _this.listeners[index];
                        if (eventListener.event === event) {
                            eventListener.listener(payload);
                            numberOfExistingListeners++;
                        }
                    }
                    if (numberOfExistingListeners === 0 && (numberOfAttempts - 1) < EventEmitter.attemptsLimit) {
                        setTimeout(emitClosure, 100);
                    }
                };
                emitClosure();
            };
            /**
             * @property {number} EventEmitter.attemptsLimit
             * @static
             */
            EventEmitter.attemptsLimit = 50;
            return EventEmitter;
        }());
        Utility.EventEmitter = EventEmitter;
        /**
         * Return the search parameters of the current page.
         * @function Intake.Utility.getPageParams
         * @template TParams :: Search parameters.
         * @returns {TParams}
         */
        function getPageParams() {
            var params = [];
            var result = {};
            if (location.search) {
                params = location.search.substr(1).split('&');
                for (var index = 0, length_3 = params.length; index < length_3; index++) {
                    var keyValuePair = params[index].replace(/\+/g, ' ').split('=');
                    result[keyValuePair[0]] = keyValuePair[1];
                }
            }
            return result;
        }
        Utility.getPageParams = getPageParams;
        var curlyBracesRegExp = (/[{|}]/g);
        /**
         * Remove curly braces from provided string.
         * @function Intake.Utility.removeCurlyBraces
         * @param {string} str :: Any string.
         * @returns {string}
         */
        function removeCurlyBraces(str) {
            return curlyBracesRegExp.test(str) ? str.replace(curlyBracesRegExp, '') : str;
        }
        Utility.removeCurlyBraces = removeCurlyBraces;
        /**
         * Check to is it JSON string or not.
         * @param {string} jsonString :: Expected JSON string
         * @returns {boolean}
         */
        function isJson(jsonString) {
            try {
                JSON.parse(jsonString);
                return true;
            }
            catch (error) {
                return false;
            }
        }
        Utility.isJson = isJson;
        /**
         * @enum HttpRequestMethod
         */
        var HttpRequestMethod;
        (function (HttpRequestMethod) {
            HttpRequestMethod["Options"] = "OPTIONS";
            HttpRequestMethod["Head"] = "HEAD";
            HttpRequestMethod["Get"] = "GET";
            HttpRequestMethod["Post"] = "POST";
            HttpRequestMethod["Put"] = "PUT";
            HttpRequestMethod["Patch"] = "PATCH";
            HttpRequestMethod["Delete"] = "DELETE";
        })(HttpRequestMethod = Utility.HttpRequestMethod || (Utility.HttpRequestMethod = {}));
        /**
         * @enum HttpRequestContentType
         */
        var HttpRequestContentType;
        (function (HttpRequestContentType) {
            HttpRequestContentType["Json"] = "application/json";
        })(HttpRequestContentType = Utility.HttpRequestContentType || (Utility.HttpRequestContentType = {}));
        /**
         * @class HttpRequest
         * @template TRequest :: Request type
         * @template TResponse :: Response type
         */
        var HttpRequest = /** @class */ (function () {
            /**
             * @constructor
             * @param {HttpRequestOptions} options :: Request options
             */
            function HttpRequest(options) {
                var _this = this;
                this.options = options;
                this.promise = new Promise(function (resolve, reject) {
                    _this.requestContext = _this.getRequestContext();
                    _this.requestContext.onreadystatechange = _this.onReadyStateChangeCallback(resolve, reject).bind(_this);
                });
            }
            /**
             * @method HttpRequest.get
             * @template TRequest :: Request type
             * @template TResponse :: Response type
             * @param {Intake.Utility.HttpRequestOptions} options :: Request options
             * @returns {Promise}
             * @static
             */
            HttpRequest.get = function (options) {
                var httpRequest = new HttpRequest(__assign(__assign({}, options), { method: HttpRequestMethod.Get }));
                return httpRequest.promise;
            };
            /**
             * @method HttpRequest.delete
             * @template TRequest :: Request type
             * @template TResponse :: Response type
             * @param {Intake.Utility.HttpRequestOptions} options :: Request options
             * @returns {Promise}
             * @static
             */
            HttpRequest.delete = function (options) {
                var httpRequest = new HttpRequest(__assign(__assign({}, options), { method: HttpRequestMethod.Delete }));
                return httpRequest.promise;
            };
            /**
             * @method HttpRequest.post
             * @template TRequest :: Request type
             * @template TResponse :: Response type
             * @param {Intake.Utility.HttpRequestOptions} options :: Request options
             * @returns {Promise}
             * @static
             */
            HttpRequest.post = function (options) {
                var httpRequest = new HttpRequest(__assign(__assign({}, options), { method: HttpRequestMethod.Post }));
                return httpRequest.promise;
            };
            /**
             * @method HttpRequest#getRequestContext
             * @returns {XMLHttpRequest}
             * @private
             */
            HttpRequest.prototype.getRequestContext = function () {
                var request = new XMLHttpRequest();
                var path = this.options.path; // Mutable, see below
                var body;
                // Prepare a list of parameters for the request
                if (this.options.parameters) {
                    var parameters = [];
                    var parametersKeys = Object.keys(this.options.parameters);
                    for (var index = 0, length_4 = parametersKeys.length; index < length_4; index++) {
                        var parameterKey = parametersKeys[index];
                        var encodedKey = encodeURIComponent(parameterKey);
                        var encodedValue = encodeURIComponent(this.options.parameters[parameterKey]);
                        parameters.push(encodedKey + "=" + encodedValue);
                    }
                    path += "?" + parameters.join('&');
                }
                // Prepare body for the request
                if (this.options.method !== HttpRequestMethod.Get) {
                    body = JSON.stringify(this.options.body);
                }
                // Open request
                request.open(this.options.method, path, true);
                // Prepare a list of headers for the request
                if (this.options.headers) {
                    var headersKeys = Object.keys(this.options.headers);
                    for (var index = 0, length_5 = headersKeys.length; index < length_5; index++) {
                        var headerKey = headersKeys[index];
                        var headerValue = this.options.headers[headerKey];
                        request.setRequestHeader(headerKey, headerValue);
                    }
                }
                // Send prepared body
                request.send(body);
                return request;
            };
            /**
             * @method HttpRequest#onReadyStateChangeCallback
             * @private
             */
            HttpRequest.prototype.onReadyStateChangeCallback = function (resolve, reject) {
                var _this = this;
                return function () {
                    if (_this.requestContext.readyState !== 4) {
                        return;
                    }
                    var response = _this.requestContext.responseText;
                    if (isJson(_this.requestContext.responseText)) {
                        response = JSON.parse(_this.requestContext.responseText);
                    }
                    if (_this.requestContext.status === 200 || _this.requestContext.status === 204) {
                        resolve(response);
                    }
                    else {
                        reject(response);
                    }
                };
            };
            return HttpRequest;
        }());
        Utility.HttpRequest = HttpRequest;
        /**
       * Called on Form Load event
       * @function Intake.Utility.OnLoadForm
       * @returns {void}
       */
        function OnLoadForm(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            SetEmptyCityState(formContext);
            (_a = formContext.getAttribute('ipg_zipcodeid')) === null || _a === void 0 ? void 0 : _a.addOnChange(function () {
                SetEmptyCityState(formContext);
            });
        }
        Utility.OnLoadForm = OnLoadForm;
        /**
         * Refresh Ribbon After Saving. Can be used as a Function in the Form OnSave event.
         * @function Intake.Utility.RefreshRibbonAfterSaving
         * @retuns {void}
         */
        function RefreshRibbonAfterSaving() {
            Xrm.Page.ui.refreshRibbon();
        }
        Utility.RefreshRibbonAfterSaving = RefreshRibbonAfterSaving;
        /*
         * Determines and returns environment short name (for example, dev, qa or prd)
         * @returns {string}
         */
        function GetEnvironment() {
            if (location.host.indexOf('-dev') >= 0) {
                return 'dev';
            }
            else if (location.host.indexOf('-qa') >= 0) {
                return 'qa';
            }
            else if (location.host.indexOf('-prd') >= 0) {
                return 'prd';
            }
            else {
                return '';
            }
        }
        Utility.GetEnvironment = GetEnvironment;
        /**
          * Formats OData date
          * @function getTodayDateFormatted
          * @returns {string}
        */
        function FormatODataDate(date) {
            var dd = String(date.getDate());
            var mm = String(date.getMonth() + 1); //January is 0!
            var yyyy = date.getFullYear();
            return yyyy + '-' + mm + '-' + dd;
        }
        Utility.FormatODataDate = FormatODataDate;
        /**
         * Formats OData "In" operator (does value contains in arrey)
         * Use it for example for retrieving some records by some ids
         * @function FormatODataLogicIn
         * @returns {string}
       */
        function FormatODataLogicIn(attributeName, values) {
            return "(" + values.map(function (value) { return attributeName + " eq " + value; }).join(" or ") + ")";
        }
        Utility.FormatODataLogicIn = FormatODataLogicIn;
        /*
         * Parses HTTP search parameters (the string after ? symbol) and returns as string[][].
         * string[i][0] - parameter name,
         * string[i][1] - parameter value.
         * returns {string[][]}
        */
        function ParseSearchParameters(searchParameters) {
            var paramArray = searchParameters.split("&");
            var paramKeyValueArray = [];
            for (var i in paramArray) {
                paramKeyValueArray[i] = paramArray[i].replace(/\+/g, " ").split("=");
            }
            return paramKeyValueArray;
        }
        Utility.ParseSearchParameters = ParseSearchParameters;
        function GetFormattedId(id) {
            if (id) {
                return id.replace("{", "").replace("}", "").toLowerCase();
            }
        }
        Utility.GetFormattedId = GetFormattedId;
        /*
         * Finds the parameter value by its name
         * returns {string}
        */
        function GetParamValueByName(vals, paramName) {
            for (var i in vals) {
                if (vals[i][0].toLowerCase() == paramName.toLowerCase()) {
                    return vals[i][1];
                }
            }
            return null;
        }
        Utility.GetParamValueByName = GetParamValueByName;
        /*
         * Gets data search parameter.
         * returns {string}
        */
        function GetDataParam() {
            //Get the query string parameters and load them
            //into the vals array
            var windowWithParam = (window.parent || window);
            if (windowWithParam.location.search != '') {
                var vals = ParseSearchParameters(windowWithParam.location.search.substr(1));
                //look for the parameter named 'data'
                return GetParamValueByName(vals, 'data');
            }
            return null;
        }
        Utility.GetDataParam = GetDataParam;
        function GetWindowJsonParameters() {
            var params = new URLSearchParams(window.location.search);
            var recordId = params.get("id");
            var args = JSON.parse(params.get("data") || "{}");
            args.recordId = recordId;
            return args;
        }
        Utility.GetWindowJsonParameters = GetWindowJsonParameters;
        /*
         * Check crm field on update.
         * returns void
         * Handler Properties Configuration
         * Pass context as first parameter
         * Passed parameters: "^[\\d]{9}$", "The value must be 9 digits!"
        */
        function CheckFieldOnChangeByRegEx(executionContext, regExp, error) {
            if (executionContext && regExp && error) {
                var regexp = new RegExp(regExp);
                var fieldCodeAttribute = executionContext.getEventSource();
                var value = fieldCodeAttribute.getValue();
                var name_1 = fieldCodeAttribute.getName();
                var AttributeControl = executionContext.getFormContext().getControl(name_1);
                if (fieldCodeAttribute && value) {
                    if (AttributeControl) {
                        if (!regexp.test(value)) {
                            AttributeControl.setNotification(error, name_1);
                            return;
                        }
                    }
                }
                AttributeControl.clearNotification(name_1);
            }
        }
        Utility.CheckFieldOnChangeByRegEx = CheckFieldOnChangeByRegEx;
        function GetIPGAddress(WebApi) {
            return __awaiter(this, void 0, void 0, function () {
                var defaultAddress, key, result, findValue, value, addressArray, zip, state, city, street;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            defaultAddress = "ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta";
                            key = "IPG Address";
                            return [4 /*yield*/, WebApi.retrieveMultipleRecords('ipg_globalsetting', '?$top=1&$select=ipg_value'
                                    + ("&$filter=ipg_name eq '" + key + "'"))];
                        case 1:
                            result = _a.sent();
                            findValue = function (source, key) {
                                var temp = source.find(function (s) { return s && s.toLowerCase().indexOf(key) > -1; });
                                return temp && temp.split(':')[1];
                            };
                            if (!(result.entities.length === 0 || !result.entities[0].ipg_value)) return [3 /*break*/, 5];
                            if (!(result.entities.length > 0)) return [3 /*break*/, 3];
                            return [4 /*yield*/, WebApi.updateRecord('ipg_globalsetting', result.entities[0].ipg_globalsettingid, { ipg_value: defaultAddress })];
                        case 2:
                            _a.sent();
                            return [3 /*break*/, 5];
                        case 3: return [4 /*yield*/, WebApi.createRecord('ipg_globalsetting', { ipg_value: defaultAddress, ipg_name: key })];
                        case 4:
                            _a.sent();
                            _a.label = 5;
                        case 5:
                            value = result.entities.length > 0 && result.entities[0].ipg_value || defaultAddress || defaultAddress;
                            addressArray = value.split(';');
                            zip = findValue(addressArray, 'zip');
                            state = findValue(addressArray, 'state');
                            city = findValue(addressArray, 'city');
                            street = findValue(addressArray, 'street');
                            return [2 /*return*/, { zip: zip, state: state, city: city, street: street }];
                    }
                });
            });
        }
        Utility.GetIPGAddress = GetIPGAddress;
        function GetRemittanceAddress(WebApi) {
            return __awaiter(this, void 0, void 0, function () {
                var defaultAddress, key, result, value, findValue, addressArray, name, address, city, state, zip, phone;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            defaultAddress = "Name:IPG - Payer Account;Address:Dept. 2399, P.O. Box 122399;State:TX;City:Dallas;Zip:75312-2399;Phone:8667530046";
                            key = "Remittance Address";
                            return [4 /*yield*/, WebApi.retrieveMultipleRecords('ipg_globalsetting', '?$top=1&$select=ipg_value'
                                    + ("&$filter=ipg_name eq '" + key + "'"))];
                        case 1:
                            result = _a.sent();
                            value = defaultAddress;
                            findValue = function (source, key) {
                                var temp = source.find(function (s) { return s && s.toLowerCase().indexOf(key) > -1; });
                                return temp && temp.split(':')[1];
                            };
                            if (!(result.entities.length === 0 || !result.entities[0].ipg_value)) return [3 /*break*/, 6];
                            if (!(result.entities.length > 0)) return [3 /*break*/, 3];
                            return [4 /*yield*/, WebApi.updateRecord('ipg_globalsetting', result.entities[0].ipg_globalsettingid, { ipg_value: defaultAddress })];
                        case 2:
                            _a.sent();
                            return [3 /*break*/, 5];
                        case 3: return [4 /*yield*/, WebApi.createRecord('ipg_globalsetting', { ipg_value: defaultAddress, ipg_name: key })];
                        case 4:
                            _a.sent();
                            _a.label = 5;
                        case 5: return [3 /*break*/, 7];
                        case 6:
                            if (result.entities.length > 0 && result.entities[0].ipg_value) {
                                value = result.entities[0].ipg_value;
                            }
                            _a.label = 7;
                        case 7:
                            addressArray = value.split(';');
                            name = findValue(addressArray, 'name');
                            address = findValue(addressArray, 'address');
                            city = findValue(addressArray, 'city');
                            state = findValue(addressArray, 'state');
                            zip = findValue(addressArray, 'zip');
                            phone = findValue(addressArray, 'phone');
                            return [2 /*return*/, { name: name, address: address, city: city, state: state, zip: zip, phone: phone }];
                    }
                });
            });
        }
        Utility.GetRemittanceAddress = GetRemittanceAddress;
        function GetCalcRevWindowSizeAndPosition() {
            var w = window.innerWidth;
            if (!w && window.parent) {
                w = window.parent.innerWidth;
            }
            w -= 100;
            var h = window.innerHeight;
            if (!h && window.parent) {
                h = window.parent.innerHeight;
            }
            h -= 100;
            var left = 50;
            var top = 50;
            var screenX = window.screenX + left;
            return 'location=0, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left + ', screenX=' + screenX + ', screenY=' + top;
        }
        Utility.GetCalcRevWindowSizeAndPosition = GetCalcRevWindowSizeAndPosition;
        /// <summary>
        /// Returns true of the current user has the given security role
        /// </summary>
        function isUserInRole(roleName) {
            if (!roleName) {
                var entityName = Xrm.Page && Xrm.Page.data && Xrm.Page.data.entity && Xrm.Page.data.entity.getEntityName();
                console.error("roleName cannot be empty in isUserInRole! Please check the Ribbon for " + entityName);
            }
            var userRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
            return !!userRoles.get(function (lk) { return lk.name && lk.name.toLowerCase().trim() == roleName.toLowerCase().trim(); })[0];
        }
        Utility.isUserInRole = isUserInRole;
        function getAppName() {
            return Xrm.Utility.getGlobalContext().getCurrentAppName();
        }
        Utility.getAppName = getAppName;
        function isUserRoleInApp(roleName, appName) {
            return new Promise(function (resolve, reject) {
                getAppName()
                    .then(function (name) { return resolve(!name || name.toLowerCase().trim() != appName.toLowerCase().trim() || isUserInRole(roleName)); }, function (error) { return reject(error); });
            });
        }
        Utility.isUserRoleInApp = isUserRoleInApp;
        function isAppWithName(appName) {
            return new Promise(function (resolve, reject) {
                getAppName()
                    .then(function (name) { return resolve(name.toLowerCase().trim() == appName.toLowerCase().trim()); }, function (error) { return reject(error); });
            });
        }
        Utility.isAppWithName = isAppWithName;
        function GetZip(WebApi, key) {
            return __awaiter(this, void 0, void 0, function () {
                var result, id, zipName, city;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, WebApi.retrieveMultipleRecords('ipg_zipcode', '?$top=1&$select=ipg_zipcodeid,ipg_name,ipg_city'
                                + ("&$filter=ipg_name eq '" + key + "'"))];
                        case 1:
                            result = _a.sent();
                            if (result.entities.length > 0 && result.entities[0].ipg_zipcodeid && result.entities[0].ipg_name) {
                                id = result.entities[0].ipg_zipcodeid;
                                zipName = result.entities[0].ipg_name;
                                city = result.entities[0].ipg_city;
                                return [2 /*return*/, { id: id, zipName: zipName, city: city }];
                            }
                            else {
                                return [2 /*return*/, null];
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        Utility.GetZip = GetZip;
        function GetState(WebApi, key) {
            return __awaiter(this, void 0, void 0, function () {
                var result, id, stateName;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, WebApi.retrieveMultipleRecords('ipg_state', '?$top=1&$select=ipg_stateid,ipg_name'
                                + ("&$filter=ipg_name eq '" + key + "'"))];
                        case 1:
                            result = _a.sent();
                            if (result.entities.length > 0 && result.entities[0].ipg_stateid && result.entities[0].ipg_name) {
                                id = result.entities[0].ipg_stateid;
                                stateName = result.entities[0].ipg_name;
                                return [2 /*return*/, { id: id, stateName: stateName }];
                            }
                            else {
                                return [2 /*return*/, null];
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        Utility.GetState = GetState;
        // Export EventEmitter instance to global scope.
        var globalScope = window.parent || window;
        globalScope.__EventEmitterInstance = globalScope.__EventEmitterInstance || new EventEmitter();
        Utility.Events = globalScope.__EventEmitterInstance;
        /**
       * Set empty City State
       * @function Intake.Utility.SetEmptyCityState
       * @returns {void}
       */
        function SetEmptyCityState(formContext) {
            var zipcode = formContext.getAttribute("ipg_zipcodeid");
            var city = formContext.getAttribute("address1_city");
            var state = formContext.getAttribute("ipg_stateid");
            if ((zipcode === null || zipcode === void 0 ? void 0 : zipcode.getValue()) == null || (zipcode === null || zipcode === void 0 ? void 0 : zipcode.getValue()) === "") {
                city === null || city === void 0 ? void 0 : city.setValue(null);
                state === null || state === void 0 ? void 0 : state.setValue(null);
            }
        }
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
