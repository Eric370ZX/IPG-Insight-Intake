/**
 * @namespace Intake.CasePartDetail.Manage
 */
namespace Intake.CasePartDetail.Manage {
  class OptionSetOption {
    value: number;
    text: string;
  }

  const _formattedPropertySuffix: string = '@OData.Community.Display.V1.FormattedValue';
  const _poTypes: OptionSetOption[] = [
    { value: 923720000, text: 'TPO' },
    { value: 923720001, text: 'ZPO' },
    { value: 923720002, text: 'CPA' },
    { value: 923720004, text: 'MPO' }
  ];

  //controls
  const _partsFormSelector: string = '#partsForm';
  const _actualPartIdDataKey: string = 'actualPartId';
  const _productIdSelector: string = '[name="productId"]';
  const _partNumberSelector: string = '[name="partNumber"]';
  const _partDescriptionSelector: string = '[name="partDescription"]';
  const _mfgNameSelector: string = '[name="mfgName"]';
  const _boxQtySelector: string = '[name="boxQty"]';
  const _keywordSelector: string = '[name="keyword"]';
  const _priceBookSelector: string = '[name="priceBook"]';
  const _claimTierSelector: string = '[name="claimTier"]';
  const _qtyImplantedSelector: string = '[name="qtyImplanted"]';
  const _qtyWastedSelector: string = '[name="qtyWasted"]';
  const _serialNumberSelector: string = '[name="serialNumber"]';
  const _lotNumberSelector: string = '[name="lotNumber"]';
  const _poTypeSelector: string = '[name="poType"]';
  const _unitCostOverrideSelector: string = '[name="unitCostOverride"]';
  const _unitShippingSelector: string = '[name="unitShipping"]';
  const _unitTaxSelector: string = '[name="unitTax"]';
  const _msrpSelector: string = '[name="msrp"]';
  const _ipgDiscountPriceSelector: string = '[name="ipgDiscountPrice"]';
  const _aicSelector: string = '[name="aic"]';

  let _incidentId: string = null;

  export function init() {

    _incidentId = Intake.Utility.GetDataParam();
    if (!_incidentId) {
      parent.Xrm.Navigation.openErrorDialog({ message: 'Error! Could not find Case Primary Carrier and consequently check Unsupported parts' });
    }

    loadData();

    (<any>$("#searchInput")).select2({
      width: 300,
      minimumInputLength: 3,
      query: function (query) {

        //todo: request D365, migrate to select2 v4 (v4 was offline at the time of writing this code)

        var data = { results: [] }, i, j, s;
        for (i = 1; i < 5; i++) {
          s = "";
          for (j = 0; j < i; j++) { s = s + query.term; }
          data.results.push({ id: query.term + i, text: s });
        }
        query.callback(data);
      }
    });
  }

  function loadData(): void {

    parent.Xrm.WebApi.retrieveRecord('incident', _incidentId, '?$select=title,ipg_patientfullname').then(function (incidentResult) {
      $('#caseTitleHeader').text(incidentResult.title + ' - ' + incidentResult.ipg_patientfullname);
    },
      function (error) {
        parent.Xrm.Navigation.openErrorDialog({ message: error.message });
      });

    parent.Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail',
      '?$expand=ipg_productid(' +
      '$select=ipg_manufacturerpartnumber,name,_ipg_manufacturerid_value,ipg_boxquantity,ipg_ipgpartnumber,ipg_claimtier,ipg_msrp)' +
      `&$filter=ipg_caseid/incidentid eq '${_incidentId}'`).then(function (actualPartsResult) {

        let actualPartTemplateHtml: string = $('#actualPart-template').html();

        for (var i = 0; i < actualPartsResult.entities.length; i++) {
          let actualPartTemplate: JQuery<HTMLElement> = $(actualPartTemplateHtml).clone();
          let actualPart: any = actualPartsResult.entities[i];

          actualPartTemplate.data(_actualPartIdDataKey, actualPart.ipg_casepartdetailid);

          //column1
          actualPartTemplate.find(_partNumberSelector).val(actualPart.ipg_productid.ipg_manufacturerpartnumber); //TODO: probably we can use productnumber instead of ipg_manufacturerpartnumber
          actualPartTemplate.find(_partDescriptionSelector).val(actualPart.ipg_productid.name);
          actualPartTemplate.find(_mfgNameSelector).val(actualPart.ipg_productid['_ipg_manufacturerid_value' + _formattedPropertySuffix]);
          actualPartTemplate.find(_boxQtySelector).val(actualPart.ipg_productid.ipg_boxquantity);
          actualPartTemplate.find(_keywordSelector).val(actualPart.ipg_productid.ipg_ipgpartnumber);
          actualPartTemplate.find(_priceBookSelector).val('TODO');
          actualPartTemplate.find(_claimTierSelector).val(actualPart.ipg_productid['ipg_claimtier' + _formattedPropertySuffix]);

          //column2
          actualPartTemplate.find(_qtyImplantedSelector).val(actualPart.ipg_quantity);
          actualPartTemplate.find(_qtyWastedSelector).val(actualPart.ipg_quantitywasted);
          actualPartTemplate.find(_serialNumberSelector).val(actualPart.ipg_serialnumber);
          actualPartTemplate.find(_lotNumberSelector).val(actualPart.ipg_lotnumber);

          //column3
          let poTypeSelect: JQuery<HTMLElement> = actualPartTemplate.find(_poTypeSelector);
          populateOptionSetSelect(<HTMLSelectElement>poTypeSelect[0], _poTypes);
          poTypeSelect.val(actualPart.ipg_potypecode);
          actualPartTemplate.find(_unitCostOverrideSelector).val(actualPart.ipg_enteredunitcost);
          actualPartTemplate.find(_unitShippingSelector).val(actualPart.ipg_enteredshipping);
          actualPartTemplate.find(_unitTaxSelector).val(actualPart.ipg_enteredtax);
          actualPartTemplate.find(_msrpSelector).val(actualPart.ipg_productid.ipg_msrp);
          actualPartTemplate.find(_ipgDiscountPriceSelector).val(actualPart.ipg_costprice);
          actualPartTemplate.find(_aicSelector).val(actualPart.ipg_mac);

          $(_partsFormSelector).append(actualPartTemplate);
        }
      },
        function (error) {
          parent.Xrm.Navigation.openErrorDialog({ message: error.message });
        });
  }

  function populateOptionSetSelect(selectElement: HTMLSelectElement, options: OptionSetOption[]): void {
    for (let option of options) {
      let newOption: HTMLOptionElement = <HTMLOptionElement>document.createElement('option');
      newOption.value = option.value.toString();
      newOption.text = option.text;
      selectElement.add(newOption);
    }
  }

  export async function saveData(): Promise<void> {

    let errorMessage: string = validateForm();
    if (errorMessage) {
      parent.Xrm.Navigation.openErrorDialog({ message: errorMessage });
      return;
    }

    await deleteParts();
    await addOrUpdateParts();
  }

  async function deleteParts(): Promise<void> {
    let actualParts: any[] = await parent.Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail',
      '?$select=ipg_casepartdetailid' +
      `&$filter=ipg_caseid/incidentid eq '${_incidentId}'`).then(function (actualPartsResult): any[] {

        return actualPartsResult.entities;
      });

    //choose actual parts to delete
    let actualPartsToDelete: any[] = [];
    let uiParts: JQuery<HTMLElement> = $(`[data-${_actualPartIdDataKey}]`);
    for (let actualPart of actualParts) {
      let uiPart: JQuery<HTMLElement> = uiParts.filter((i, el) => $(el).data(_actualPartIdDataKey) == actualPart.ipg_casepartdetailid);
      if (uiPart.length == 0) {
        actualPartsToDelete.push(actualPart);
      }
    }

    //delete
    for (let actualPartToDelete of actualPartsToDelete) {
      await parent.Xrm.WebApi.deleteRecord('ipg_casepartdetail', actualPartToDelete.ipg_casepartdetailid).then(function success() { },
        function (error) {
          parent.Xrm.Navigation.openErrorDialog({ message: error.message });
        });
    }
  }

  async function addOrUpdateParts(): Promise<void> {
    let uiParts: JQuery<HTMLElement> = $(_partsFormSelector).find(`[data-${_actualPartIdDataKey}]`);
    for (let i = 0; i < uiParts.length; i++) {
      let uiActualPart: JQuery<HTMLElement> = uiParts.eq(i);
      let uiActualPartId: string = uiActualPart.data(_actualPartIdDataKey);
      if ((uiActualPartId || '').trim() == '') {
        let createModel: any = buildCreateModel(uiActualPart);
        await parent.Xrm.WebApi.createRecord('ipg_casepartdetail', createModel).then(function success(creationResult: any) {
          uiActualPart.data(_actualPartIdDataKey, creationResult.id);
        },
          function (error) {
            parent.Xrm.Navigation.openErrorDialog({ message: error.message });
          }
        );
      }
      else {
        let updateModel: any = buildUpdateModel(uiActualPart);
        await parent.Xrm.WebApi.updateRecord('ipg_casepartdetail', uiActualPartId, updateModel).then(function success() { },
          function (error) {
            parent.Xrm.Navigation.openErrorDialog({ message: error.message });
          }
        );
      }
    }

  }

  function buildCreateModel(uiPart: JQuery<HTMLElement>): any {
    let model: any = buildUpdateModel(uiPart);
    model['ipg_caseid@odata.bind'] = `/incidents(${_incidentId})`;
    model['ipg_productid@odata.bind'] = `/products(${uiPart.find(_productIdSelector).val()})`;

    return model;
  }

  function buildUpdateModel(uiPart: JQuery<HTMLElement>): any {
    let quantityString: string = uiPart.find(_qtyImplantedSelector).val().toString().trim();
    let quantityWastedString: string = uiPart.find(_qtyWastedSelector).val().toString().trim();
    let poTypeCodeString: string = uiPart.find(_poTypeSelector).val().toString().trim();
    let unitCostOverrideString: string = uiPart.find(_unitCostOverrideSelector).val().toString().trim();
    let enteredShippingString = uiPart.find(_unitShippingSelector).val().toString().trim();
    let enteredTaxString = uiPart.find(_unitTaxSelector).val().toString().trim();

    let model: object = {

      //column2
      'ipg_quantity': quantityString != '' ? Number(quantityString) : null,
      'ipg_quantitywasted': quantityWastedString != '' ? Number(quantityWastedString) : null,
      'ipg_serialnumber': uiPart.find(_serialNumberSelector).val(),
      'ipg_lotnumber': uiPart.find(_serialNumberSelector).val(),

      //column3
      'ipg_potypecode': poTypeCodeString != '' ? Number(poTypeCodeString) : null,
      'ipg_enteredunitcost': unitCostOverrideString != '' ? Number(unitCostOverrideString) : null,
      'ipg_enteredshipping': enteredShippingString != '' ? Number(enteredShippingString) : null,
      'ipg_enteredtax': enteredTaxString != '' ? Number(enteredTaxString) : null

    }

    return model;
  }

  export function remove(button: HTMLElement) {
    if (confirm('Remove this part?')) {
      button.closest('[data-actualPartId]').remove();
    }
  }

  function validateForm(): string {



    return '';
  }

  export function advancedSearch(button: HTMLElement) {
    let filterString: string = '';
    let partNumber: string = $('#advSearchPartNumber').val().toString().trim();
    if (partNumber) {
      filterString += `ipg_manufacturerproductnumber eq '${encodeURIComponent(partNumber)}'`;
    }



    let optionsString: string = '';
    if (filterString) {

    }

      parent.Xrm.WebApi.retrieveMultipleRecords('product', optionsString).then(function (productsResult) {
          //todo: advanced search results
      },
        function (error) {
          parent.Xrm.Navigation.openErrorDialog({ message: error.message });
        });
  }
}
