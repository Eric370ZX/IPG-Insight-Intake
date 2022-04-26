/**
 * @namespace Intake.Document
 */
namespace Intake.EBVResponse {

  const loadingMessage: string = 'Loading...';
  const formattedPropertySuffix: string = '@OData.Community.Display.V1.FormattedValue';

  const ActiveCoverageStatusCode: string = '1';
  const InactiveCoverageStatusCode: string = '6';
  const NonCoveredBenefitStatus: string = 'I';

  let displayableServiceTypes: any[] = [];
  let gwLookup: any[] = [];

  var Xrm = Xrm || parent.Xrm;

  export async function InitViewDetailsPage(): Promise<void> {

    var ebvResponseId = Intake.Utility.GetDataParam();
    if (!ebvResponseId) {
      ebvResponseId = Xrm.Page.data.entity.getId();
    }

    if (ebvResponseId) {

      ebvResponseId = decodeURIComponent(ebvResponseId);

      showProgressIndicator();
      await Xrm.WebApi
        .retrieveRecord('ipg_ebvresponse', ebvResponseId)
        .then(async function (ebvResponse: any) {
          closeProgressIndicator();

          //get lookup values first
          await getGWLookupValues();
          
          showProgressIndicator();

          let ebvTransactionPromise = displayEBVTransactionInfo(ebvResponse);
          let subscriberPromise = displaySubscriberInfo(ebvResponse);
          let dependentPromise = displayDependentInfo(ebvResponse);
          let benefitsPromise = displayBenefits(ebvResponse);

          Promise.all([ebvTransactionPromise, subscriberPromise, dependentPromise, benefitsPromise])
            .then(closeProgressIndicator)
            .catch(closeProgressIndicator);
          
        },
          (error: any) => {
            closeProgressIndicator();
            alert('Could not retrieve EBV Response: ' + error.message);
          }
      );
    }
  }

  async function displayEBVTransactionInfo(ebvResponse: any): Promise<void> {
    $('#responseDateTime').text(ebvResponse['ipg_createdat' + formattedPropertySuffix] || '');
    $('#serviceDates').text(ebvResponse['ipg_servicedate' + formattedPropertySuffix] || '');
    $('#responseMessage').text(ebvResponse.ipg_name || ''); //response message

    let inquiryIdElement: JQuery<HTMLElement> = $('#inquiryId');
    
    await Xrm.WebApi
      .retrieveMultipleRecords('ipg_ebvinquiry', "?$filter=ipg_discriminator eq 'Inquiry' and _ipg_responseid_value eq " + ebvResponse.ipg_ebvresponseid)
      .then(
        function success(ebvInquiriesResult: Xrm.RetrieveMultipleResult) {
          if (ebvInquiriesResult && ebvInquiriesResult.entities.length) {
            inquiryIdElement.text(ebvInquiriesResult.entities[0].ipg_name || '');
          }
        },
        (error: any) => {
          alert('Could not retrieve EBV Inquiry: ' + error.message);
        }
      );

    let carrierNameElement: JQuery<HTMLElement> = $('#carrierName');
    
    if (ebvResponse._ipg_payerid_value) {
      await Xrm.WebApi
        .retrieveRecord('ipg_ebventity', ebvResponse._ipg_payerid_value)
        .then(
          function success(ebvEntity: any) {
            carrierNameElement.text(ebvEntity.ipg_name || '');
          },
          (error: any) => {
            alert('Could not retrieve EBV Entity: ' + error.message);
          }
        );
    }
  }

  async function displaySubscriberInfo(ebvResponse: any): Promise<void> {
    await Xrm.WebApi
      .retrieveRecord('ipg_ebvsubscriber', ebvResponse._ipg_subscriberid_value)
      .then(
        function success(insured: any) {
          renderSubscriberInfo('insured', insured, true);
          if (!ebvResponse._ipg_dependentid_value) {
            renderSubscriberInfo('patient', insured, false);
          }
        },
        (error: any) => {
          alert('Could not retrieve EBV Subscriber: ' + error.message);
        }
      );
  }

  async function displayDependentInfo(ebvResponse: any):  Promise<any> {
    if (ebvResponse._ipg_dependentid_value) {
      await Xrm.WebApi
        .retrieveRecord('ipg_ebvsubscriber', ebvResponse._ipg_dependentid_value)
        .then(
          function success(dependent: any) {
            renderSubscriberInfo('patient', dependent, false);
          },
          (error: any) => {
            alert('Could not retrieve Dependent person: ' + error.message);
          }
        );
    }
  }

  function renderSubscriberInfo(htmlIdPrefix: string, subscriber: any, renderCoverageInfo: boolean): void {
    $(`#${htmlIdPrefix}FirstName`).text(subscriber.ipg_firstname || '');
    $(`#${htmlIdPrefix}MiddleName`).text(subscriber.ipg_middlename || '');
    $(`#${htmlIdPrefix}LastName`).text(subscriber.ipg_name || '');
    $(`#${htmlIdPrefix}DateOfBirth`).text(subscriber['ipg_dob' + formattedPropertySuffix] || '');
    $(`#${htmlIdPrefix}Address1`).text(subscriber.ipg_addressline1 || '');
    $(`#${htmlIdPrefix}Address2`).text(subscriber.ipg_addressline2 || '');
    $(`#${htmlIdPrefix}City`).text(subscriber.ipg_city || '');
    $(`#${htmlIdPrefix}State`).text(subscriber.ipg_state || '');
    $(`#${htmlIdPrefix}Zip`).text(subscriber.ipg_zip || '');
    $(`#${htmlIdPrefix}Sex`).text(subscriber.ipg_gender || '');
    $(`#${htmlIdPrefix}MI`).text(subscriber.ipg_memberid || '');

    if (renderCoverageInfo) {
      //coverage information
      let values: { fieldName: string, fieldValue: string }[] = [];
      if (subscriber.ipg_refidentificationqualifier && subscriber.ipg_refidentification) {
        let fieldName = getGWLookupValue('ReferenceCode', subscriber.ipg_refidentificationqualifier);
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
      let coverageInfoTemplateHtml: string = $('#coverageInformation-template').html();
      for (let value of values) {
        let coverageInfoTemplate: JQuery<HTMLElement> = $(coverageInfoTemplateHtml).clone();
        coverageInfoTemplate.find('[data-fieldName]').text(value.fieldName);
        coverageInfoTemplate.find('[data-fieldValue]').text(value.fieldValue);
        $(`#${htmlIdPrefix}CoverageInfo`).append(coverageInfoTemplate);
      }
    }
  }

  async function displayBenefits(ebvResponse: any): Promise<void> {
    
    let benefits = await getBenefits(ebvResponse.ipg_ebvresponseid);
    let benefitIds = <string[]>benefits.map((benefit) => benefit.ipg_ebvbenefitid);
    let benefitsIndustryCodes = await getBenefitsIndustryCodes(benefitIds);
    let responseServiceTypeCodes: string[] = getResponseServiceTypes(benefits);

    displayableServiceTypes = await getDisplayableServiceTypes();

    let serviceTypesToDisplay: string[] = filterServiceTypesToDisplay(responseServiceTypeCodes, displayableServiceTypes);
    for (let serviceType of serviceTypesToDisplay) {

      let inRecords: [string, string][][] = buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, 'Y');
      let outRecords: [string, string][][] = buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, 'N');
      let unknownRecords: [string, string][][] = buildArraysOfBenefitValues(benefits, benefitsIndustryCodes, serviceType, null);

      let showBenefits: boolean = inRecords.length > 0 || outRecords.length > 0 || unknownRecords.length > 0;
      if (showBenefits) {
        let status: string = getBenefitStatusByServiceType(benefits, serviceType);
        renderServiceTypeBenefits(serviceType, status, inRecords, outRecords, unknownRecords);
      }
    }
  }

  async function getBenefits(responseId: string): Promise<any[]> {
    let requestOptions = "?$filter=ipg_discriminator eq 'EligibilityBenefit' and _ipg_responseid_value eq '" + responseId + "'&$orderby=ipg_servicetype";

    return Xrm.WebApi
      .retrieveMultipleRecords('ipg_ebvbenefit', requestOptions)
      .then(function success(benefitsResult: Xrm.RetrieveMultipleResult) {

        return benefitsResult.entities;
      },
        (error: any) => {
          alert('Could not retrieve benefits: ' + error.message);
        }
      );
  }

  async function getBenefitsIndustryCodes(benefitIds: string[]): Promise<any[]> {
    if (! benefitIds || benefitIds.length == 0) {
      return [];
    }

    let benefitIdsString: string = benefitIds.map((val) => "'" + val + "'").join(', ');
    let requestOptions = `?$filter=ipg_discriminator eq 'BenefitIndustryCode' and Microsoft.Dynamics.CRM.In(PropertyName=@p1,PropertyValues=@p2)&@p1='ipg_ebvbenefitid'&@p2=[${benefitIdsString}]`;

    return Xrm.WebApi
      .retrieveMultipleRecords('ipg_ebvbenefitindustrycode', requestOptions)
      .then(function success(codesResult: Xrm.RetrieveMultipleResult) {

        return codesResult.entities;
      },
        (error: any) => {
          alert('Could not retrieve benefit industry codes: ' + error.message);
        }
      );
  }

  function getResponseServiceTypes(benefits: any[]): string[] {
    let responseServiceTypes: string[] = [];
    for (let benefit of benefits) {
      if (responseServiceTypes.indexOf(benefit.ipg_servicetype) == -1) {
        responseServiceTypes.push(benefit.ipg_servicetype);
      }
    }
    return responseServiceTypes;
  }

  async function getDisplayableServiceTypes() : Promise<any[]> {
    return Xrm.WebApi
      .retrieveMultipleRecords('ipg_gwservicetypecode',
        '?$filter=statecode eq 0' + //0 means active
        ' and ipg_displayonebvrequest eq true')
      .then(function success(serviceTypesResult: Xrm.RetrieveMultipleResult) {
        return serviceTypesResult.entities;
      },
        (error: any) => {
          alert('Could not retrieve EBV service types: ' + error.message);
        }
      );
  }

  function filterServiceTypesToDisplay(responseServiceTypeCodes: string[], displayableServiceTypes: any[]): string[] {
    let serviceTypesToDisplay: string[] = [];

    for (let responseServiceTypeCode of responseServiceTypeCodes) {
      if (responseServiceTypeCode == null || displayableServiceTypes.find(st => (st.ipg_name || '').toUpperCase() == responseServiceTypeCode.toUpperCase())) {
        serviceTypesToDisplay.push(responseServiceTypeCode);
      }
    }

    return serviceTypesToDisplay;
  }

  function getBenefitStatusByServiceType(benefits: any[], serviceType: string): string {
    
    let isActive: boolean = false, isInactive: boolean = false;;
    for (let benefit of benefits) {
      if (benefit.ipg_servicetype && serviceType
        && (<string>benefit.ipg_servicetype).toUpperCase() == serviceType.toUpperCase()) {

        if (benefit.ipg_status == ActiveCoverageStatusCode) {
          isActive = true;
        }
        else if (benefit.ipg_status == InactiveCoverageStatusCode){
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

  function buildArraysOfBenefitValues(benefits: any[], benefitsIndustryCodes: any[], serviceType: string, nwFlag: string) : [string, string][][] {

    let sectionBenefits = benefits.filter(b => b.ipg_servicetype == serviceType
      && (b.ipg_inplannetwork == nwFlag || nwFlag == null && b.ipg_inplannetwork != 'Y' && b.ipg_inplannetwork != 'N'));

    sectionBenefits = sectionBenefits.sort((obj1, obj2) => {
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

    let arrays: [string, string][][] = [];
    for (let benefit of sectionBenefits) {
      let benefitIndustryCodes: any[] = benefitsIndustryCodes.filter((x) => x._ipg_ebvbenefitid_value == benefit.ipg_ebvbenefitid);
      let benefitValues = buildArrayOfValuesFromBenefit(benefit, benefitIndustryCodes);
      arrays.push(benefitValues);
    }

    return arrays;

  }

  function buildArrayOfValuesFromBenefit(benefitRecord: any, benefitIndustryCodes: any[]): [string, string][] {
    let values: [string, string][] = [];

    if (benefitIndustryCodes?.length) {
      for (let benefitIndustryCode of benefitIndustryCodes) {
        let industryCodeString: string = benefitIndustryCode.ipg_industrycode + ' - ' + getGWLookupValue('IndustryCode', benefitIndustryCode.ipg_industrycode);
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

        let statusValue: string = getStatusValue(benefitRecord);
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
        values.push(['payornote', (<string>benefitRecord.ipg_name)]);
      }
    }

    return values;
  }

  function getStatusValue(benefitRecord): string {
    let statusValue: string = '';

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

  function renderServiceTypeBenefits(serviceType: string, status: string,
    inRecords: [string, string][][], outRecords: [string, string][][], unknownRecords: [string, string][][]): void {

    //prepare data

    let serviceTypeName: string;
    if (serviceType) {
      serviceTypeName = getServiceTypeNameById(serviceType);
    }
    else {
      serviceTypeName = 'General';
    }

    let statusName: string = getGWLookupValue('BenefitStatusCode', status);
    let statusColor: string = determineStatusColor(status);

    let benefitDetailsHtml: string = buildBenefitDetailsHtml(inRecords, outRecords, unknownRecords);


    //populate the template

    let serviceTypeTemplateHtml: string = $('#serviceType-template').html();
    let serviceTypeTemplate: JQuery<HTMLElement> = $(serviceTypeTemplateHtml).clone();
    serviceTypeTemplate.find('[data-serviceTypeName]').text(serviceTypeName);
    let serviceTypeElement = serviceTypeTemplate.find('[data-status]');
    serviceTypeElement.text(statusName);
    serviceTypeElement.css('background-color', statusColor)
    serviceTypeTemplate.find('[data-benefits]').html(benefitDetailsHtml);

    $('#benefitsContainer').append(serviceTypeTemplate);
  }

  function determineStatusColor(status: string): string {
    switch (status) {
      case ActiveCoverageStatusCode:
        return 'green';

      case InactiveCoverageStatusCode:
        return 'red';
    }

    return 'transparent';
  }

  function buildBenefitDetailsHtml(inRecords: [string, string][][], outRecords: [string, string][][], unknownRecords: [string, string][][]): string {
    let html: string = '';

    if (inRecords.length > 0 || outRecords.length > 0) {
      html += buildHeaderHtml(inRecords.length > 0 ? 'In Network' : '', outRecords.length > 0 ? 'Out Network' : '');

      for (var benefitIndex = 0; benefitIndex < inRecords.length || benefitIndex < outRecords.length; benefitIndex++) {

        let inBenefitValues: [string, string][] = benefitIndex < inRecords.length ? inRecords[benefitIndex] : null;
        let outBenefitValues: [string, string][] = benefitIndex < outRecords.length ? outRecords[benefitIndex] : null;

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

  function buildHeaderHtml(leftTitle: string, rightTitle: string): string {
    let templateHtml: string = $('#benefitHeader-template').html();
    let template: JQuery<HTMLElement> = $(templateHtml).clone();

    template.find('[data-leftHeader]').text(leftTitle);
    template.find('[data-rightHeader]').text(rightTitle);

    return template.prop('outerHTML');
  }

  function buildBenefitDetailsRecordHtml(leftValues: [string, string][], rightValues: [string, string][], benefitIndex: number) :string {
    let html: string = '';

    for (var i = 0; leftValues && i < leftValues.length || rightValues && i < rightValues.length; i++) {

      let leftKeyValue: [string, string] = leftValues && i < leftValues.length ? leftValues[i] : null;
      let rightKeyValue: [string, string] = rightValues && i < rightValues.length ? rightValues[i] : null;

      html += buildBenefitDetailsRowHtml(leftKeyValue, rightKeyValue, benefitIndex);
    }


    return html;
  }

  function buildBenefitDetailsRowHtml(leftKeyValue: [string, string], rightKeyValue: [string, string], benefitIndex: number): string {
    let bgColor: string = benefitIndex % 2 == 0 ? '#F6F6F6' : '#FFF'; //todo: replace with styles

    let templateHtml: string = $('#benefitRow-template').html();
    let template: JQuery<HTMLElement> = $(templateHtml).clone();

    //get cell elements

    let labelCell1: JQuery<HTMLElement> = template.find('[data-labelCell1]');
    let valueCell1: JQuery<HTMLElement> = template.find('[data-valueCell1]');
    let labelCell2: JQuery<HTMLElement> = template.find('[data-labelCell2]');
    let valueCell2: JQuery<HTMLElement> = template.find('[data-valueCell2]');


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

  function getLabelByKey(key: string): string {
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

  function getServiceTypeNameById(serviceTypeId: string): string {
    for (let serviceType of displayableServiceTypes) {
      if (serviceType.ipg_name == serviceTypeId) {
        return serviceType.ipg_definition;
      }
    }

    return '';
  }

  async function getGWLookupValues(): Promise<void> {
    showProgressIndicator();
    return Xrm.WebApi
      .retrieveMultipleRecords('ipg_gwlookup')
      .then(function success(lookupResult: Xrm.RetrieveMultipleResult) {
        closeProgressIndicator();
        gwLookup = lookupResult.entities;
      },
        (error: any) => {
          closeProgressIndicator();
          alert('Could not retrieve GW lookup: ' + error.message);
        }
      );
  }

  function getGWLookupValue(type: string, key: string): string {
    for (let gwLookupRecord of gwLookup) {
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
}
