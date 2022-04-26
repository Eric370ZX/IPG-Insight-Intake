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
 * @namespace Intake.Document
 */
var Intake;
(function (Intake) {
    var EBVResponse;
    (function (EBVResponse) {
        var loadingMessage = 'Loading...';
        var formattedPropertySuffix = '@OData.Community.Display.V1.FormattedValue';
        var ActiveCoverageStatusCode = '1';
        var InactiveCoverageStatusCode = '6';
        var NonCoveredBenefitStatus = 'I';
        var displayableServiceTypes = [];
        var gwLookup = [];
        var Xrm = Xrm || parent.Xrm;
        function InitViewDetailsPage() {
            return __awaiter(this, void 0, void 0, function () {
                var ebvResponseId;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            ebvResponseId = Intake.Utility.GetDataParam();
                            if (!ebvResponseId) {
                                ebvResponseId = Xrm.Page.data.entity.getId();
                            }
                            if (!ebvResponseId) return [3 /*break*/, 2];
                            ebvResponseId = decodeURIComponent(ebvResponseId);
                            showProgressIndicator();
                            return [4 /*yield*/, Xrm.WebApi
                                    .retrieveRecord('ipg_ebvresponse', ebvResponseId)
                                    .then(function (ebvResponse) {
                                    return __awaiter(this, void 0, void 0, function () {
                                        var ebvTransactionPromise, subscriberPromise, dependentPromise, benefitsPromise;
                                        return __generator(this, function (_a) {
                                            switch (_a.label) {
                                                case 0:
                                                    closeProgressIndicator();
                                                    //get lookup values first
                                                    return [4 /*yield*/, getGWLookupValues()];
                                                case 1:
                                                    //get lookup values first
                                                    _a.sent();
                                                    showProgressIndicator();
                                                    ebvTransactionPromise = displayEBVTransactionInfo(ebvResponse);
                                                    subscriberPromise = displaySubscriberInfo(ebvResponse);
                                                    dependentPromise = displayDependentInfo(ebvResponse);
                                                    benefitsPromise = displayBenefits(ebvResponse);
                                                    Promise.all([ebvTransactionPromise, subscriberPromise, dependentPromise, benefitsPromise])
                                                        .then(closeProgressIndicator)
                                                        .catch(closeProgressIndicator);
                                                    return [2 /*return*/];
                                            }
                                        });
                                    });
                                }, function (error) {
                                    closeProgressIndicator();
                                    alert('Could not retrieve EBV Response: ' + error.message);
                                })];
                        case 1:
                            _a.sent();
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        EBVResponse.InitViewDetailsPage = InitViewDetailsPage;
        function displayEBVTransactionInfo(ebvResponse) {
            return __awaiter(this, void 0, void 0, function () {
                var inquiryIdElement, carrierNameElement;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            $('#responseDateTime').text(ebvResponse['ipg_createdat' + formattedPropertySuffix] || '');
                            $('#serviceDates').text(ebvResponse['ipg_servicedate' + formattedPropertySuffix] || '');
                            $('#responseMessage').text(ebvResponse.ipg_name || ''); //response message
                            inquiryIdElement = $('#inquiryId');
                            return [4 /*yield*/, Xrm.WebApi
                                    .retrieveMultipleRecords('ipg_ebvinquiry', "?$filter=ipg_discriminator eq 'Inquiry' and _ipg_responseid_value eq " + ebvResponse.ipg_ebvresponseid)
                                    .then(function success(ebvInquiriesResult) {
                                    if (ebvInquiriesResult && ebvInquiriesResult.entities.length) {
                                        inquiryIdElement.text(ebvInquiriesResult.entities[0].ipg_name || '');
                                    }
                                }, function (error) {
                                    alert('Could not retrieve EBV Inquiry: ' + error.message);
                                })];
                        case 1:
                            _a.sent();
                            carrierNameElement = $('#carrierName');
                            if (!ebvResponse._ipg_payerid_value) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi
                                    .retrieveRecord('ipg_ebventity', ebvResponse._ipg_payerid_value)
                                    .then(function success(ebvEntity) {
                                    carrierNameElement.text(ebvEntity.ipg_name || '');
                                }, function (error) {
                                    alert('Could not retrieve EBV Entity: ' + error.message);
                                })];
                        case 2:
                            _a.sent();
                            _a.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        }
        function displaySubscriberInfo(ebvResponse) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi
                                .retrieveRecord('ipg_ebvsubscriber', ebvResponse._ipg_subscriberid_value)
                                .then(function success(insured) {
                                renderSubscriberInfo('insured', insured, true);
                                if (!ebvResponse._ipg_dependentid_value) {
                                    renderSubscriberInfo('patient', insured, false);
                                }
                            }, function (error) {
                                alert('Could not retrieve EBV Subscriber: ' + error.message);
                            })];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        }
        function displayDependentInfo(ebvResponse) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!ebvResponse._ipg_dependentid_value) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi
                                    .retrieveRecord('ipg_ebvsubscriber', ebvResponse._ipg_dependentid_value)
                                    .then(function success(dependent) {
                                    renderSubscriberInfo('patient', dependent, false);
                                }, function (error) {
                                    alert('Could not retrieve Dependent person: ' + error.message);
                                })];
                        case 1:
                            _a.sent();
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        function renderSubscriberInfo(htmlIdPrefix, subscriber, renderCoverageInfo) {
            $("#" + htmlIdPrefix + "FirstName").text(subscriber.ipg_firstname || '');
            $("#" + htmlIdPrefix + "MiddleName").text(subscriber.ipg_middlename || '');
            $("#" + htmlIdPrefix + "LastName").text(subscriber.ipg_name || '');
            $("#" + htmlIdPrefix + "DateOfBirth").text(subscriber['ipg_dob' + formattedPropertySuffix] || '');
            $("#" + htmlIdPrefix + "Address1").text(subscriber.ipg_addressline1 || '');
            $("#" + htmlIdPrefix + "Address2").text(subscriber.ipg_addressline2 || '');
            $("#" + htmlIdPrefix + "City").text(subscriber.ipg_city || '');
            $("#" + htmlIdPrefix + "State").text(subscriber.ipg_state || '');
            $("#" + htmlIdPrefix + "Zip").text(subscriber.ipg_zip || '');
            $("#" + htmlIdPrefix + "Sex").text(subscriber.ipg_gender || '');
            $("#" + htmlIdPrefix + "MI").text(subscriber.ipg_memberid || '');
            if (renderCoverageInfo) {
                //coverage information
                var values = [];
                if (subscriber.ipg_refidentificationqualifier && subscriber.ipg_refidentification) {
                    var fieldName = getGWLookupValue('ReferenceCode', subscriber.ipg_refidentificationqualifier);
                    values.push({ fieldName: fieldName, fieldValue: subscriber.ipg_refidentification });
                }
                if (subscriber.ipg_planstartdate) {
                    values.push({ fieldName: 'Plan Date', fieldValue: (subscriber['ipg_planstartdate' + formattedPropertySuffix]) + ' - ' + (subscriber['ipg_planenddate' + formattedPropertySuffix] || '') });
                }
                if (subscriber.ipg_benefitplanstartdate) {
                    values.push({ fieldName: 'Plan Begin Date', fieldValue: subscriber['ipg_benefitplanstartdate' + formattedPropertySuffix] });
                }
                if (subscriber.ipg_benefitplanenddate) {
                    values.push({ fieldName: 'Plan End Date', fieldValue: subscriber['ipg_benefitplanenddate' + formattedPropertySuffix] });
                }
                if (subscriber.ipg_plansponsor) {
                    values.push({ fieldName: 'Plan Sponsor', fieldValue: subscriber.ipg_plansponsor });
                }
                //display coverage information
                var coverageInfoTemplateHtml = $('#coverageInformation-template').html();
                for (var _i = 0, values_1 = values; _i < values_1.length; _i++) {
                    var value = values_1[_i];
                    var coverageInfoTemplate = $(coverageInfoTemplateHtml).clone();
                    coverageInfoTemplate.find('[data-fieldName]').text(value.fieldName);
                    coverageInfoTemplate.find('[data-fieldValue]').text(value.fieldValue);
                    $("#" + htmlIdPrefix + "CoverageInfo").append(coverageInfoTemplate);
                }
            }
        }
        function displayBenefits(ebvResponse) {
            return __awaiter(this, void 0, void 0, function () {
                var benefits, benefitIds, benefitsIndustryCodes, responseServiceTypeCodes, serviceTypesToDisplay, _i, serviceTypesToDisplay_1, serviceType, inRecords, outRecords, unknownRecords, showBenefits, status_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, getBenefits(ebvResponse.ipg_ebvresponseid)];
                        case 1:
                            benefits = _a.sent();
                            benefitIds = benefits.map(function (benefit) { return benefit.ipg_ebvbenefitid; });
                            return [4 /*yield*/, getBenefitsIndustryCodes(benefitIds)];
                        case 2:
                            benefitsIndustryCodes = _a.sent();
                            responseServiceTypeCodes = getResponseServiceTypes(benefits);
                            return [4 /*yield*/, getDisplayableServiceTypes()];
                        case 3:
                            displayableServiceTypes = _a.sent();
                            serviceTypesToDisplay = filterServiceTypesToDisplay(responseServiceTypeCodes, displayableServiceTypes);
                            for (_i = 0, serviceTypesToDisplay_1 = serviceTypesToDisplay; _i < serviceTypesToDisplay_1.length; _i++) {
                                serviceType = serviceTypesToDisplay_1[_i];
                                inRecords = buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, 'Y');
                                outRecords = buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, 'N');
                                unknownRecords = buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, null);
                                showBenefits = inRecords.length > 0 || outRecords.length > 0 || unknownRecords.length > 0;
                                if (showBenefits) {
                                    status_1 = getBenefitStatusByServiceType(benefits, serviceType);
                                    renderServiceTypeBenefits(serviceType, status_1, inRecords, outRecords, unknownRecords);
                                }
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function getBenefits(responseId) {
            return __awaiter(this, void 0, void 0, function () {
                var requestOptions;
                return __generator(this, function (_a) {
                    requestOptions = "?$filter=ipg_discriminator eq 'EligibilityBenefit' and _ipg_responseid_value eq '" + responseId + "'&$orderby=ipg_servicetype";
                    return [2 /*return*/, Xrm.WebApi
                            .retrieveMultipleRecords('ipg_ebvbenefit', requestOptions)
                            .then(function success(benefitsResult) {
                            return benefitsResult.entities;
                        }, function (error) {
                            alert('Could not retrieve benefits: ' + error.message);
                        })];
                });
            });
        }
        function getBenefitsIndustryCodes(benefitIds) {
            return __awaiter(this, void 0, void 0, function () {
                var benefitIdsString, requestOptions;
                return __generator(this, function (_a) {
                    if (!benefitIds || benefitIds.length == 0) {
                        return [2 /*return*/, []];
                    }
                    benefitIdsString = benefitIds.map(function (val) { return "'" + val + "'"; }).join(', ');
                    requestOptions = "?$filter=ipg_discriminator eq 'BenefitIndustryCode' and Microsoft.Dynamics.CRM.In(PropertyName=@p1,PropertyValues=@p2)&@p1='ipg_ebvbenefitid'&@p2=[" + benefitIdsString + "]";
                    return [2 /*return*/, Xrm.WebApi
                            .retrieveMultipleRecords('ipg_ebvbenefitindustrycode', requestOptions)
                            .then(function success(codesResult) {
                            return codesResult.entities;
                        }, function (error) {
                            alert('Could not retrieve benefit industry codes: ' + error.message);
                        })];
                });
            });
        }
        function getResponseServiceTypes(benefits) {
            var responseServiceTypes = [];
            for (var _i = 0, benefits_1 = benefits; _i < benefits_1.length; _i++) {
                var benefit = benefits_1[_i];
                if (responseServiceTypes.indexOf(benefit.ipg_servicetype) == -1) {
                    responseServiceTypes.push(benefit.ipg_servicetype);
                }
            }
            return responseServiceTypes;
        }
        function getDisplayableServiceTypes() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, Xrm.WebApi
                            .retrieveMultipleRecords('ipg_gwservicetypecode', '?$filter=statecode eq 0' + //0 means active
                            ' and ipg_displayonebvrequest eq true')
                            .then(function success(serviceTypesResult) {
                            return serviceTypesResult.entities;
                        }, function (error) {
                            alert('Could not retrieve EBV service types: ' + error.message);
                        })];
                });
            });
        }
        function filterServiceTypesToDisplay(responseServiceTypeCodes, displayableServiceTypes) {
            var serviceTypesToDisplay = [];
            var _loop_1 = function (responseServiceTypeCode) {
                if (responseServiceTypeCode == null || displayableServiceTypes.find(function (st) { return (st.ipg_name || '').toUpperCase() == responseServiceTypeCode.toUpperCase(); })) {
                    serviceTypesToDisplay.push(responseServiceTypeCode);
                }
            };
            for (var _i = 0, responseServiceTypeCodes_1 = responseServiceTypeCodes; _i < responseServiceTypeCodes_1.length; _i++) {
                var responseServiceTypeCode = responseServiceTypeCodes_1[_i];
                _loop_1(responseServiceTypeCode);
            }
            return serviceTypesToDisplay;
        }
        function getBenefitStatusByServiceType(benefits, serviceType) {
            var isActive = false, isInactive = false;
            ;
            for (var _i = 0, benefits_2 = benefits; _i < benefits_2.length; _i++) {
                var benefit = benefits_2[_i];
                if (benefit.ipg_servicetype && serviceType
                    && benefit.ipg_servicetype.toUpperCase() == serviceType.toUpperCase()) {
                    if (benefit.ipg_status == ActiveCoverageStatusCode) {
                        isActive = true;
                    }
                    else if (benefit.ipg_status == InactiveCoverageStatusCode) {
                        isInactive = true;
                    }
                }
            }
            if (isActive) {
                return ActiveCoverageStatusCode;
            }
            if (isInactive) {
                return InactiveCoverageStatusCode;
            }
            return '';
        }
        function buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, nwFlag) {
            var sectionBenefits = benefits.filter(function (b) { return b.ipg_servicetype == serviceType
                && (b.ipg_inplannetwork == nwFlag || nwFlag == null && b.ipg_inplannetwork != 'Y' && b.ipg_inplannetwork != 'N'); });
            sectionBenefits = sectionBenefits.sort(function (obj1, obj2) {
                //order by CoverageLevel first
                if (obj1.ipg_coveragelevel > obj2.ipg_coveragelevel) {
                    return 1;
                }
                if (obj1.ipg_coveragelevel < obj2.ipg_coveragelevel) {
                    return -1;
                }
                //order by Status
                if (obj1.ipg_status > obj2.ipg_status) {
                    return 1;
                }
                if (obj1.ipg_status < obj2.ipg_status) {
                    return -1;
                }
                return 0;
            });
            var arrays = [];
            var _loop_2 = function (benefit) {
                var benefitIndustryCodes = benefitsIndustryCodes.filter(function (x) { return x._ipg_ebvbenefitid_value == benefit.ipg_ebvbenefitid; });
                var benefitValues = buildArrayOfValuesFromBenefit(benefit, benefitIndustryCodes);
                arrays.push(benefitValues);
            };
            for (var _i = 0, sectionBenefits_1 = sectionBenefits; _i < sectionBenefits_1.length; _i++) {
                var benefit = sectionBenefits_1[_i];
                _loop_2(benefit);
            }
            return arrays;
        }
        function buildArrayOfValuesFromBenefit(benefitRecord, benefitIndustryCodes) {
            var values = [];
            if (benefitIndustryCodes === null || benefitIndustryCodes === void 0 ? void 0 : benefitIndustryCodes.length) {
                for (var _i = 0, benefitIndustryCodes_1 = benefitIndustryCodes; _i < benefitIndustryCodes_1.length; _i++) {
                    var benefitIndustryCode = benefitIndustryCodes_1[_i];
                    var industryCodeString = benefitIndustryCode.ipg_industrycode + ' - ' + getGWLookupValue('IndustryCode', benefitIndustryCode.ipg_industrycode);
                    values.push([getGWLookupValue('industryCodeQualifiers', benefitIndustryCode.ipg_qualifiercode), industryCodeString]);
                }
            }
            if (benefitRecord.ipg_status == NonCoveredBenefitStatus) {
                values.push(['Non-Covered', '']);
            }
            else {
                if (benefitRecord.ipg_insurancetype) {
                    values.push([getGWLookupValue('InsuranceType', benefitRecord.ipg_insurancetype), '']);
                }
                if (benefitRecord.ipg_status != ActiveCoverageStatusCode
                    && benefitRecord.ipg_status != InactiveCoverageStatusCode) {
                    var statusValue = getStatusValue(benefitRecord);
                    values.push([getGWLookupValue('BenefitStatusCode', benefitRecord.ipg_status), statusValue]);
                }
                if (benefitRecord.ipg_coveragelevel) {
                    values.push(['coveragelevel', getGWLookupValue('CoverageLevel', benefitRecord.ipg_coveragelevel)]);
                }
                if (benefitRecord.ipg_coverageplandescription) {
                    values.push(['coverageplandescription', benefitRecord.ipg_coverageplandescription]);
                }
                if (benefitRecord.ipg_authcertrequired) {
                    switch (benefitRecord.ipg_authcertrequired) {
                        case 'Y':
                            values.push(['authcertrequired', 'Yes']);
                            break;
                        case 'N':
                            values.push(['authcertrequired', 'No']);
                            break;
                        case 'U':
                            values.push(['authcertrequired', 'Unknown']);
                            break;
                    }
                }
                if (benefitRecord.ipg_name) //payor note
                 {
                    values.push(['payornote', benefitRecord.ipg_name]);
                }
            }
            return values;
        }
        function getStatusValue(benefitRecord) {
            var statusValue = '';
            if (benefitRecord.ipg_percentage || benefitRecord.ipg_percentage == 0) {
                statusValue = (benefitRecord.ipg_percentage * 100).toString() + '%';
            }
            else if (benefitRecord.ipg_monetaryamount || benefitRecord.ipg_monetaryamount == 0) {
                statusValue = '$' + benefitRecord.ipg_monetaryamount.toFixed(2);
            }
            if (benefitRecord.ipg_quantity || benefitRecord.ipg_quantity == 0) {
                statusValue += ' ' + benefitRecord.ipg_quantity.toString() + ' ' + getGWLookupValue('QuantityQualifier', benefitRecord.ipg_quantityqualifier);
            }
            if (benefitRecord.ipg_timeperiodqualifier) {
                statusValue += ' ' + getGWLookupValue('TimePeriodQualifier', benefitRecord.ipg_timeperiodqualifier);
            }
            return statusValue;
        }
        function renderServiceTypeBenefits(serviceType, status, inRecords, outRecords, unknownRecords) {
            //prepare data
            var serviceTypeName;
            if (serviceType) {
                serviceTypeName = getServiceTypeNameById(serviceType);
            }
            else {
                serviceTypeName = 'General';
            }
            var statusName = getGWLookupValue('BenefitStatusCode', status);
            var statusColor = determineStatusColor(status);
            var benefitDetailsHtml = buildBenefitDetailsHtml(inRecords, outRecords, unknownRecords);
            //populate the template
            var serviceTypeTemplateHtml = $('#serviceType-template').html();
            var serviceTypeTemplate = $(serviceTypeTemplateHtml).clone();
            serviceTypeTemplate.find('[data-serviceTypeName]').text(serviceTypeName);
            var serviceTypeElement = serviceTypeTemplate.find('[data-status]');
            serviceTypeElement.text(statusName);
            serviceTypeElement.css('background-color', statusColor);
            serviceTypeTemplate.find('[data-benefits]').html(benefitDetailsHtml);
            $('#benefitsContainer').append(serviceTypeTemplate);
        }
        function determineStatusColor(status) {
            switch (status) {
                case ActiveCoverageStatusCode:
                    return 'green';
                case InactiveCoverageStatusCode:
                    return 'red';
            }
            return 'transparent';
        }
        function buildBenefitDetailsHtml(inRecords, outRecords, unknownRecords) {
            var html = '';
            if (inRecords.length > 0 || outRecords.length > 0) {
                html += buildHeaderHtml(inRecords.length > 0 ? 'In Network' : '', outRecords.length > 0 ? 'Out Network' : '');
                for (var benefitIndex = 0; benefitIndex < inRecords.length || benefitIndex < outRecords.length; benefitIndex++) {
                    var inBenefitValues = benefitIndex < inRecords.length ? inRecords[benefitIndex] : null;
                    var outBenefitValues = benefitIndex < outRecords.length ? outRecords[benefitIndex] : null;
                    html += buildBenefitDetailsRecordHtml(inBenefitValues, outBenefitValues, benefitIndex);
                }
            }
            if (unknownRecords.length > 0) {
                html += buildHeaderHtml('Unknown Network', '');
                for (var i = 0; i < unknownRecords.length; i++) {
                    html += buildBenefitDetailsRecordHtml(unknownRecords[i], null, i);
                }
            }
            return html;
        }
        function buildHeaderHtml(leftTitle, rightTitle) {
            var templateHtml = $('#benefitHeader-template').html();
            var template = $(templateHtml).clone();
            template.find('[data-leftHeader]').text(leftTitle);
            template.find('[data-rightHeader]').text(rightTitle);
            return template.prop('outerHTML');
        }
        function buildBenefitDetailsRecordHtml(leftValues, rightValues, benefitIndex) {
            var html = '';
            for (var i = 0; leftValues && i < leftValues.length || rightValues && i < rightValues.length; i++) {
                var leftKeyValue = leftValues && i < leftValues.length ? leftValues[i] : null;
                var rightKeyValue = rightValues && i < rightValues.length ? rightValues[i] : null;
                html += buildBenefitDetailsRowHtml(leftKeyValue, rightKeyValue, benefitIndex);
            }
            return html;
        }
        function buildBenefitDetailsRowHtml(leftKeyValue, rightKeyValue, benefitIndex) {
            var bgColor = benefitIndex % 2 == 0 ? '#F6F6F6' : '#FFF'; //todo: replace with styles
            var templateHtml = $('#benefitRow-template').html();
            var template = $(templateHtml).clone();
            //get cell elements
            var labelCell1 = template.find('[data-labelCell1]');
            var valueCell1 = template.find('[data-valueCell1]');
            var labelCell2 = template.find('[data-labelCell2]');
            var valueCell2 = template.find('[data-valueCell2]');
            //set properties
            if (leftKeyValue) {
                labelCell1.text(getLabelByKey(leftKeyValue[0]));
                valueCell1.text(leftKeyValue[1]);
            }
            labelCell1.css('background-color', bgColor); //todo: try tr color
            valueCell1.css('background-color', bgColor);
            if (rightKeyValue != null) {
                labelCell2.text(getLabelByKey(rightKeyValue[0]));
                labelCell2.css('background-color', bgColor);
                valueCell2.text(getLabelByKey(rightKeyValue[1]));
                valueCell2.css('background-color', bgColor);
            }
            else {
                labelCell2.remove();
                valueCell2.remove();
                valueCell1.prop('colspan', '3');
            }
            return template.prop('outerHTML');
        }
        function getLabelByKey(key) {
            switch (key) {
                case 'memberid':
                    return 'Member ID';
                case 'firstname':
                    return 'First Name';
                case 'lastname':
                    return 'Last Name';
                case 'dob':
                    return 'Date of Birth';
                case 'addressline1':
                    return 'Address 1';
                case 'addressline2':
                    return 'Address 2';
                case 'city':
                    return 'City';
                case 'state':
                    return 'State';
                case 'zip':
                    return 'Postal Code';
                case 'gender':
                    return 'Sex';
                case 'authcertrequired':
                    return 'Authorization or Cert Required';
                case 'payornote':
                    return 'Payer Note';
                case 'coverageplandescription':
                    return 'Coverage Plan Description';
                case 'coveragelevel':
                    return 'Coverage Level';
                default:
                    return key;
            }
        }
        //lookup key-name lookup requests
        function getServiceTypeNameById(serviceTypeId) {
            for (var _i = 0, displayableServiceTypes_1 = displayableServiceTypes; _i < displayableServiceTypes_1.length; _i++) {
                var serviceType = displayableServiceTypes_1[_i];
                if (serviceType.ipg_name == serviceTypeId) {
                    return serviceType.ipg_definition;
                }
            }
            return '';
        }
        function getGWLookupValues() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    showProgressIndicator();
                    return [2 /*return*/, Xrm.WebApi
                            .retrieveMultipleRecords('ipg_gwlookup')
                            .then(function success(lookupResult) {
                            closeProgressIndicator();
                            gwLookup = lookupResult.entities;
                        }, function (error) {
                            closeProgressIndicator();
                            alert('Could not retrieve GW lookup: ' + error.message);
                        })];
                });
            });
        }
        function getGWLookupValue(type, key) {
            for (var _i = 0, gwLookup_1 = gwLookup; _i < gwLookup_1.length; _i++) {
                var gwLookupRecord = gwLookup_1[_i];
                if (gwLookupRecord.ipg_datatype == type && gwLookupRecord.ipg_name == key) {
                    return gwLookupRecord.ipg_value;
                }
            }
            return key;
        }
        function showProgressIndicator() {
            Xrm.Utility.showProgressIndicator(loadingMessage);
        }
        function closeProgressIndicator() {
            Xrm.Utility.closeProgressIndicator();
        }
    })(EBVResponse = Intake.EBVResponse || (Intake.EBVResponse = {}));
})(Intake || (Intake = {}));
