/**
 * @namespace Intake.Account
 */
namespace Intake.Account {

  const workersCompPlanTypeValue: number = 923720004;
  const dirtyFields: string[] = ['ipg_statementtopatient', 'address1_country', 'address2_line3', 'ipg_claimname', 'ipg_pricelistdate']; //these fields are set business rules
  const facilityFormId: string = "550FB40D-0D41-4728-989B-121EACCA2BE6";
  const carrierFormId: string = "BF7A66A7-F2ED-417D-821F-9F064E348EAD";
  const FormIds = {
    facility : "550FB40D-0D41-4728-989B-121EACCA2BE6",
    carrier: "BF7A66A7-F2ED-417D-821F-9F064E348EAD"
  };
  enum AccountStatusCodes {
    Active = 1,
    Inactive = 2
  };
  enum AccountStatuses {
    Active = 427880000,
    Inactive = 427880001,
  };
  /**
   * Called on load form
   * @function Intake.Account.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    formContext.getControl("customertypecode").setVisible(false);
    HideRelationTypeField(formContext);
    ChooseForm(formContext);
    hideWorkersCompOrOtherPlanTypes(formContext);
    //updateBillPatient(formContext);   
    setCarrierAccepted(formContext);
    let formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();
    sessionStorage.setItem('passVariables', formName);
    //setDefaultManufacturerPaymentThreshold(formContext);
    if (setDefaultManufacturerPaymentThreshold.length != 0) {
      setDefaultManufacturerPaymentThreshold(formContext);
    }
    OnChangeType();
    setSubmitModeForDirtyFields(formContext);
    SetRemittanceAddress(formContext);
    disableFormSelector(formContext);
    DirectBill(formContext);
  }

  /**
  * Called on save form
  * @function Intake.Account.OnSaveForm
  * @returns {void}
 */
  export function OnSaveForm(executionContext) {
    let formContext = executionContext.getFormContext();
    let currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
    if (currentFormId.toLocaleLowerCase() === facilityFormId.toLocaleLowerCase()) {
      let statusCode = formContext.getAttribute("statuscode")?.getValue();
      let status = formContext.getAttribute("ipg_status")?.getValue();
      if (statusCode != null && statusCode === AccountStatusCodes.Active && status != null && status === AccountStatuses.Active) {
        formContext.getAttribute("ipg_active").setValue(true);
      }
      else if (statusCode != null && statusCode === AccountStatusCodes.Inactive && status != null && status === AccountStatuses.Inactive) {
        formContext.getAttribute("ipg_active").setValue(false);
      }
    }
    if (formContext.ui.getFormType() == 1){ // Create form
      setRelationshipType(formContext);
    }
  }

  export function SetBSAFile(executionContext) {
    let formContext = executionContext.getFormContext();
    if (formContext.getAttribute("ipg_facilitysignedbsa").getValue() == 923720000) {   
      formContext.getAttribute("ipg_bsanotrequiredreason")?.setRequiredLevel("none");
    }
   else if (formContext.getAttribute("ipg_facilitysignedbsa").getValue() == 923720001) {
      formContext.getAttribute("ipg_bsanotrequiredreason")?.setRequiredLevel("required");
    }
    else {
      formContext.getAttribute("ipg_bsanotrequiredreason")?.setRequiredLevel("none");

    }
  }

  /**
  * Hides 'Relation Type' field if it's filled
  * @function Intake.Account.HideRelationTypeField
  * @returns {void}
  */
  function HideRelationTypeField(formContext) {
    var customertypecode = formContext.getAttribute("customertypecode");
    if (customertypecode) {
      if (customertypecode.getValue() != null)
        formContext.getControl("customertypecode").setVisible(false);
    }
  }
  
/**
* Displays the selected  Set CPT Description
* @function Intake.Account.SetCPTDescription
* @returns {void}
*/
  function DisplayCarrierCPTName(formContext) {   
   
   // ipg_carriercptname
   var carrierName = formContext.getAttribute("ipg_facilitycptname").getValue();
    if (carrierName == null) {
      formContext.getAttribute("ipg_carriercptname").setVisible(true);
    }
    else {
      formContext.getAttribute("ipg_carriercptname").setVisible(false);
    }
  }




  function SetCPTDescription(formContext) {

    var cptDescription;
    var cptCodeObject = Xrm.Page.getAttribute("ipg_cptcode").getValue();

    if (cptCodeObject != null) {
      var cptCodeId;
      cptCodeId= cptCodeObject[0];
      var cptId;

      //cptId= Object.fromEntries(
      //  Object.entries(cptCodeId).map(([key, value]) => [key, value])
      //);
      var epirationdate = new Date('12/31/9999');
      var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
      var guid = cptId.id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("ipg_cptcode", guid, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {
        Xrm.Page.getAttribute("ipg_facilitycptname").setValue(results["ipg_cptname"] + " - " + results["ipg_cptcode"]);
        Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
        Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate);
        Xrm.Page.getAttribute("ipg_expirationdate").setValue(epirationdate);

      }, function (error) {

      });

    }

  }
 // Account.SetCPTDescription = SetCPTDescription;





  /**
  * Choose form depending on 'Relation Type' field value
  * @function Intake.Account.ChooseForm
  * @returns {void}
  */
  function ChooseForm(formContext) {
    var customertypecode = formContext.getAttribute("customertypecode");
    if (customertypecode) {
      var relationType = customertypecode.getValue();
      if ((formContext.ui.getFormType() == 2) && (relationType != null)) {
        var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();

        interface accountTypesValues {
          name: string;
          value: number;
          formId: string;
        }
        var accountTypes: accountTypesValues[] = [
          { name: 'carrier', value: 923720001, formId: 'BF7A66A7-F2ED-417D-821F-9F064E348EAD'.toLocaleLowerCase() },
          { name: 'manufacturer', value: 923720002, formId: '42DEB6BE-3ADB-4BBE-BCD2-90B46ED81AC8'.toLocaleLowerCase() },
          { name: 'facility', value: 923720000, formId: 'e3ed4465-f4d0-4aa7-8f16-07bf6dd60d13'.toLocaleLowerCase() },
          { name: 'distributor', value: 923720003, formId: '25B4ED80-3C95-4D29-8120-6384A813BA12'.toLocaleLowerCase() }
        ];

        var currentAccountType = accountTypes.find(x => x.value == relationType);
        if (currentAccountType != undefined) {
          if (currentFormId != currentAccountType.formId) {
            var items = formContext.ui.formSelector.items.get();
            for (var i in items) {
              if (items[i].getId() == currentAccountType.formId) {
                //prevent the dialog about unsaved changes
                DoNotSubmitChangedAttributes();
                items[i].navigate();
              }
            }
          }
        }
      }
    }
  }

  /*
   * Changes Submit Mode for all dirty attributes. It can be used to prevent the dialog about unsaved changes
   */
  function DoNotSubmitChangedAttributes(): void {
    let attributes: Xrm.Attributes.Attribute[] = Xrm.Page.data.entity.attributes.get();
    for (let attr of attributes) {
      if (attr.getIsDirty()) {
        attr.setSubmitMode("never");
      }
    }
  }

  /**
   * Called on change 'Pay to address' field
   * @function Intake.Account.OnChangePayToAddress
   * @returns {void}
  */
  export function OnChangePayToAddress(executionContext) {
    let formContext = executionContext.getFormContext();
    var claimspaytoaddress = formContext.getAttribute("ipg_claimspaytoaddress").getValue();
    if (claimspaytoaddress == 923720001) {
      formContext.getAttribute("address2_line3").setValue(null);
      formContext.getAttribute("ipg_claimspaytopostalcode").setValue(null);
      formContext.getAttribute("ipg_claimspaytopostalcode").fireOnChange();
    }
    else {
      Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", "?$select=ipg_value&$filter=ipg_name eq 'IPG Address'").then(
        function success(result){
          if(result.entities.length > 0){
            var ipgAddress: string = result.entities[0].ipg_value;
            var addressArray = ipgAddress.split(';');
            var zip;
            var street;
            addressArray.forEach(function(item){
              if(item.startsWith("ZIP")){
                zip = item.substring(4);
              }
              else if(item.startsWith("Street")){
                street = item.substring(8);
              }
            });
            if(street){
              formContext.getAttribute("address2_line3").setValue(street);
            }
            if (zip){
              Xrm.WebApi.retrieveMultipleRecords("ipg_zipcode", `?$select=ipg_zipcodeid&$filter=ipg_name eq '${zip}'`).then(
                function success(results) {
                  if(results.entities.length > 0) {
                    formContext.getAttribute("ipg_claimspaytopostalcode").setValue([{ entityType: "ipg_zipcode", id: results.entities[0]["ipg_zipcodeid"], name: zip }]);
                    formContext.getAttribute("ipg_claimspaytopostalcode").fireOnChange();
                  }
                },
                function (error) {
                  console.log(error.message);
                }
              );
            }
          }
        },
        function(error){
          console.log(error.message);
        }
      );
    }
  }

  export function OnChangeType(executionContext: Xrm.Events.EventContext = null) {
    let fieldCodeAttribute: Xrm.Attributes.Attribute;
    let formContext: Xrm.FormContext;
    if (executionContext != null) {
      fieldCodeAttribute = executionContext.getEventSource() as Xrm.Attributes.Attribute;
      formContext = executionContext.getFormContext();
    }
    else {
      formContext = Xrm.Page;
      fieldCodeAttribute = formContext.getAttribute("ipg_type");
    }
  }

  /*
   * Called on change of account.ipg_carriersupportedplantypes
   * @function Intake.Account.OnCarrierSupportedPlanTypesChange
   * @returns {void}
   */
  export function OnCarrierSupportedPlanTypesChange(executionContext: Xrm.Events.SaveEventContext) {
 

    let formContext: Xrm.FormContext = executionContext.getFormContext();

    hideWorkersCompOrOtherPlanTypes(formContext);

    let selectedPlanTypes: number[] = formContext.getAttribute("ipg_carriersupportedplantypes").getValue();
    if (selectedPlanTypes == null || selectedPlanTypes.length == 0) {
      resetPlanTypeOptions(formContext);
    }

    updateBillPatient(formContext);
  }

  function hideWorkersCompOrOtherPlanTypes(formContext: Xrm.FormContext) {


    let planTypesControl: Xrm.Controls.Control = formContext.getControl("ipg_carriersupportedplantypes");
    if (planTypesControl) {
      let controlPlanTypeOptions: Xrm.OptionSetValue[] = planTypesControl.getOptions();
      let selectedPlanTypes: number[] = formContext.getAttribute("ipg_carriersupportedplantypes").getValue();

      if (selectedPlanTypes && selectedPlanTypes.length > 0) {
        if (selectedPlanTypes.indexOf(workersCompPlanTypeValue) == -1) {
          //remove Workers Comp option

          if (optionsContainValue(controlPlanTypeOptions, workersCompPlanTypeValue)) {
            planTypesControl.removeOption(workersCompPlanTypeValue);
          }
        }
        else {
          //remove all non Workers Comp options

          for (let i: number = 0; i < controlPlanTypeOptions.length; i++) {
            if (controlPlanTypeOptions[i].value && controlPlanTypeOptions[i].value != workersCompPlanTypeValue) {
              planTypesControl.removeOption(controlPlanTypeOptions[i].value);
            }
          }
        }
      }
    }
  }

  function updateBillPatient(formContext: Xrm.FormContext) {

    if (formContext.getAttribute("ipg_carriersupportedplantypes") && formContext.getAttribute("ipg_statementtopatient")) {
      let selectedPlanTypes: number[] = formContext.getAttribute("ipg_carriersupportedplantypes").getValue();
      formContext.getAttribute("ipg_statementtopatient").setValue(
        selectedPlanTypes == null || selectedPlanTypes.indexOf(workersCompPlanTypeValue) == -1);
    }


  }

  function optionsContainValue(array: Xrm.OptionSetValue[], optionValue: number): boolean {
    for (let i = 0; i < array.length; i++) {
      if (array[i].value == optionValue) {
        return true;
      }
    }

    return false;
  }

  function resetPlanTypeOptions(formContext: Xrm.FormContext) {
    //get all options
    let allPlanTypeOptions: Xrm.OptionSetValue[] = (<any>formContext.getAttribute("ipg_carriersupportedplantypes")).getOptions();

    //get disabled options and add only enabled
    var query = "?$select=ipg_disabledplantypes&$filter=ipg_name eq 'Disabled plan types'";
    Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", query)
      .then((result: Xrm.RetrieveMultipleResult) => {
        let disabledPlanTypes: string[] = [];

        if (result.entities.length > 0) {
          let disabledPlanTypesString: string = result.entities[0].ipg_disabledplantypes;
          if (disabledPlanTypesString) {
            disabledPlanTypes = disabledPlanTypesString.split(",");
          }
        }

        let planTypesControl: Xrm.Controls.Control = formContext.getControl("ipg_carriersupportedplantypes");
        planTypesControl.clearOptions();
        for (let i: number = 0; i < allPlanTypeOptions.length; i++) {
          if (disabledPlanTypes.indexOf(allPlanTypeOptions[i].value.toString()) == -1) {
            planTypesControl.addOption(allPlanTypeOptions[i]);
          }
        }
      },
        function (error) {
          console.log(error.message);
        }
      );
  }

  export function ShowLegacyCarrierAuditInformation(primaryControl) {
    let env = Intake.Utility.GetEnvironment();
    let envSuffix: string;
    if (env) {
      envSuffix = '-' + env;
    }
    else {
      envSuffix = '';
    }
    let formContext: Xrm.FormContext = primaryControl;
    var incidentId = formContext.data.entity.getId();
    incidentId = incidentId.replace(/[{}]/g, "");

    Xrm.Navigation.openUrl("https://insight-auditinfo" + envSuffix + ".azurewebsites.net/carrierauditinfo/index/" + incidentId);
  }

  async function getDocuments(facilityId: string, type: string) {
    const fetch = `<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='ipg_document'>
                    <all-attributes/>
                    <order attribute='ipg_name' descending='false' />
                    <filter type='and'>
                      <condition attribute='ipg_facilityid' operator='eq' value='${facilityId}' />
                      <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                    <link-entity name='ipg_documenttype' from='ipg_documenttypeid' to='ipg_documenttypeid' link-type='inner' alias='aa'>
                      <filter type='and'>
                        <condition attribute='ipg_documenttypeabbreviation' operator='eq' value='${type}' />
                      </filter>
                    </link-entity>
                  </entity>
                </fetch>`;
    return Xrm.WebApi.retrieveMultipleRecords("ipg_document", `?fetchXml=${fetch}`);
  }

  async function setCarrierAccepted(form: Xrm.FormContext) {
    const customertypecode = form.getAttribute<Xrm.Attributes.OptionSetAttribute>("customertypecode");
    if (customertypecode && customertypecode.getValue() == 923720001) {//Carrier
      const userTeams = await GetuserTeams();
	  
      const isCarrierAdminTeam = userTeams.filter(p => p == "Carrier Services").length > 0;
      const hasAdminRole = await checkUserRole("System Administrator");

      const carrierAccepted = form.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_carrieraccepted");
      carrierAccepted.setRequiredLevel(isCarrierAdminTeam || hasAdminRole ? "required" : "none");
      carrierAccepted.controls.forEach(ctrl => ctrl.setDisabled(!(isCarrierAdminTeam || hasAdminRole)));
    }
  }
  async function GetuserTeams(): Promise<string[]> {
    const fetchXml = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
              <entity name="team">
                <attribute name="name" />
                <order attribute="name" descending="false" />
                <link-entity name="teammembership" from="teamid" to="teamid" visible="false" intersect="true">
                  <link-entity name="systemuser" from="systemuserid" to="systemuserid" alias="ai">
                    <filter type="and">
                      <condition attribute="systemuserid" operator="eq-userid" />
                    </filter>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>`;
    const teams = await Xrm.WebApi.online.retrieveMultipleRecords("team", `?fetchXml=${fetchXml}`);
    return teams.entities.map(p => p.name);
  }
  async function checkUserRole(roleName: string): Promise<boolean> {
    const personalRoles = Xrm.Utility.getGlobalContext().getUserRoles();
    let result = false;
    for (var i = 0; i < personalRoles.length; i++) {
      const targetUserRole = await GetRoleName(personalRoles[i]);
      if (roleName.toLowerCase() == targetUserRole.toLowerCase()) {
        result = true;
        break;
      }
    }
    return result;
  }
  async function GetRoleName(roleId: string): Promise<string> {
    const roleNames = await Xrm.WebApi.retrieveMultipleRecords("role", `?$select=name&$filter=roleid eq ${roleId}`);
    if (roleNames.entities.length == 0) {
      return "";
    }
    return roleNames.entities[0].name as string;
  }

  function setDefaultManufacturerPaymentThreshold(form) {
    if (form.getAttribute("customertypecode").getValue() != null) {
    //  if (form.getAttribute("customertypecode").setValue() != null) {
      var customertypecode = form.getAttribute("customertypecode");
      if (((form.ui.getFormType() == 1)) && customertypecode && customertypecode.getValue() == 923720002) { //Manufacturer

        if (form.getAttribute("ipg_manufacturerpaymentthreshold") != null) {
          form.getAttribute("ipg_manufacturerpaymentthreshold").setValue(0);
        }
      }
    }
  }
  //function setDefaultManufacturerPaymentThreshold(form: Xrm.FormContext) {

  //  const customertypecode = form.getAttribute<Xrm.Attributes.OptionSetAttribute>("customertypecode");
  //  if (((form.ui.getFormType() == 1)) && customertypecode && customertypecode.getValue() == 923720002) {//Manufacturer
  //    form.getAttribute("ipg_manufacturerpaymentthreshold").setValue(0);
  //  }
  //}

  function setSubmitModeForDirtyFields(formContext) {
    if (formContext.ui.getFormType() != 1) {
      dirtyFields.forEach(field => {
        let attr = formContext.getAttribute(field);
        if (attr && attr.getIsDirty()) {
          attr.setSubmitMode('never');
        }
      });
    }
  }

  /**
   * Called on dirty field change
   * @function Intake.Account.onDirtyFieldChange
   * @returns {void}
  */
  export function onDirtyFieldChange(executionContext) {
    let formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() != 1) {
      let field = executionContext.getEventSource()._attributeName;
      let attr = formContext.getAttribute(field);
      if (!attr.getIsDirty()) {
        attr.setSubmitMode('dirty');
      }
    }
  }

/**
 * Called on on Sub Grid FacilityPhysician Record Selected
 * @function Intake.Account.onSubGridFacilityPhysicianRecordSelect
 * @returns {void}
*/
  export function onSubGridFacilityPhysicianRecordSelect(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    var currentEntity = formContext.data.entity;
    currentEntity.attributes.forEach(function (attribute, i) {
      if (attribute.getName() !== "ipg_status") {
        attribute.controls.get(0).setDisabled(true);
      }
    });
  }

  function setRelationshipType(formContext: Xrm.FormContext){
    var formId = formContext.ui.formSelector.getCurrentItem().getId();
    if (formId === FormIds.carrier){
      formContext.getAttribute("customertypecode")?.setValue(923720001); // Carrier
    }
  }

  export async function SetRemittanceAddress(primaryControl) {
    let address = await Intake.Utility.GetRemittanceAddress(Xrm.WebApi);
    let formContext: Xrm.FormContext = primaryControl;
    let payToName = formContext.getAttribute("ipg_paytoname");
    let payToAddress = formContext.getAttribute("ipg_paytoaddress");
    let payToCity = formContext.getAttribute("ipg_paytocity");
    let payToStateId = formContext.getAttribute("ipg_paytostateid");
    let payToZipId = formContext.getAttribute("ipg_paytozipid");
    let payToPhone = formContext.getAttribute("ipg_paytophone");

    if (payToName && address.name) {
      payToName.setValue(address.name);
    }
    if (payToAddress && address.address) {
      payToAddress.setValue(address.address);
    }
    if (payToCity && address.city) {
        payToCity.setValue(address.city);
    }
    
    if (payToStateId && address.state) {
        let state = await Intake.Utility.GetState(Xrm.WebApi, address.state);
        if(state !== null && state.id && state.stateName) {
            payToStateId.setValue([{
            id: state.id.replace("{", "").replace("}", ""),
            entityType: "ipg_state",
            name: state.stateName
          }]);
        }
      }

    if (payToZipId && address.zip) {
      let zip = await Intake.Utility.GetZip(Xrm.WebApi, address.zip);
      if(zip !== null && zip.id && zip.zipName) {
        payToZipId.setValue([{
          id: zip.id.replace("{", "").replace("}", ""),
          entityType: "ipg_zipcode",
          name: zip.zipName
        }]);
      }
    }
    if (payToPhone && address.phone) {
      payToPhone.setValue(address.phone);
    }
  }
  
  function disableFormSelector(formContext){
    formContext.ui.formSelector.items.forEach(formItem => formItem.setVisible(false));
  }

  export function DirectBill(formContext)
  {
    if (formContext.getAttribute("ipg_manufacturerisparticipating")?.getValue() == true) {
      formContext.getAttribute("address1_line2")?.setRequiredLevel("required");
      formContext.getAttribute("address2_city")?.setRequiredLevel("required");
      formContext.getAttribute("address2_stateorprovince")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_melissaaccountzipcodeid")?.setRequiredLevel("required");
    }
    else {
      formContext.getAttribute("address1_line2")?.setRequiredLevel("none");
      formContext.getAttribute("address2_city")?.setRequiredLevel("none");
      formContext.getAttribute("address2_stateorprovince")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_melissaaccountzipcodeid")?.setRequiredLevel("none");
    }
  }

}
