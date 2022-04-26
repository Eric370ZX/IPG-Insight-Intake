/**
 * @namespace Intake.Incident
 */
namespace Intake.Incident {
  let Xrm = window.Xrm || window.top.Xrm;
  let incidentId: string;
  let statementTypesSelectElementId = "statementTypesSelect";
  let deliveryMethodSelectElementId = "delivetyMethodSelect";
  let DelivaryMethod =
  {
    Paper:"Paper",
    Electronic:"Electronic"
  } 

  enum CaseStatesEnum {
    Intake = 923720000,
    Authorizations = 923720001,
    CaseManagement = 923720002,
    Billing = 923720003,
    CarrierService = 923720004,
    PatientServices = 923720005,
    Finance = 923720006
  };
  enum CaseStatusEnum{
    Open = 923720000,
    Closed = 923720001
  };

  export async function InitGenerateStatementPage() {
    incidentId = (new URLSearchParams(window.location.search)).get("data");
    if (!incidentId) {
      Xrm.Navigation.openErrorDialog({ message: 'Error. Incident ID parameter is required' });
      return null;
    }
    try {
      Xrm.Utility.showProgressIndicator(null);
      await populateDeliveryMethod();
      await populateStatementTypes();
      document.getElementById(deliveryMethodSelectElementId).addEventListener('change', populateStatementTypes);
    } 
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function GeneratePatientStatement() {
    let statementTypesSelectElement = <HTMLSelectElement>document.getElementById(statementTypesSelectElementId);
    let delivaryMethodSelectElement = <HTMLSelectElement>document.getElementById(deliveryMethodSelectElementId);

    if (!statementTypesSelectElement.value || statementTypesSelectElement.value == "0") {
      Xrm.Navigation.openAlertDialog({text:'Please select a Statement Type'});
      return;
    }

    try
    {
      Xrm.Utility.showProgressIndicator(null);

      var finalConfirmtext = "Before generating this Statement, please ensure the Patient Address for this Case is correct.";

      if (await checkPendingPatientStatement()) 
      {
        finalConfirmtext = "A statement for this Patient is scheduled to generate within next 5 days. Do you wish to still generate this additional statement?";
      }
      else if(delivaryMethodSelectElement.value == DelivaryMethod.Paper)
      {
        finalConfirmtext += " Please note that generating a paper Statement does not modify future Start Date or Due Date values, nor does it modify the overall Patient Statement sequence."; 
      }
      else
      {
        finalConfirmtext += " By selecting the 'Generate Statement' button, the Start Date and Due Date values for this and all future Statements will be reset and overall Patient Statements sequence will be modified.";
      }

      if (!(await Xrm.Navigation.openConfirmDialog(
        {
          text:finalConfirmtext
          , confirmButtonLabel: "Generate Statement"
          , cancelButtonLabel: "Cancel"
        })).confirmed)
        {
          return;
        }


      var apiUrl = await getUriFromGlobalSettings();
      var response = await fetch(apiUrl, {method: 'POST'
      , body: JSON.stringify({
        "CaseId":incidentId,
        "StatementType":statementTypesSelectElement.value})});

      var doc = await response.json();
      doc.ipg_deliverymethodcode = 427880000;
      var createddoc = await Xrm.WebApi.createRecord("ipg_document", doc);

      var associateRequest = {
        getMetadata: () => ({
            boundParameter: null,
            parameterTypes: {},
            operationType: 2,
            operationName: "Associate"
        }),
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
    } 
      await Xrm.WebApi.online.execute(associateRequest);
      Xrm.Navigation.openAlertDialog({text:"Patient Statement Generated!"}).then(()=>window.close());
    }
    catch(e)
    {
      Xrm.Navigation.openErrorDialog({message:"Failed Generated PS. Try Later or Please Contact System Administrator!"});
    }
    finally
    {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  async function populateStatementTypes(): Promise<void> {
    Xrm.Utility.showProgressIndicator(null);
    var deliveryMethodt = (<HTMLSelectElement>document.getElementById(deliveryMethodSelectElementId)).value;
    var statementTypesSelectElement = (<HTMLSelectElement>document.getElementById(statementTypesSelectElementId));
    //Clear options
    statementTypesSelectElement.disabled = false;
    var i, L = statementTypesSelectElement.options.length - 1;
    for(i = L; i >= 0; i--) {
      statementTypesSelectElement.remove(i);
    }

    let availableTemplates = ['D1', 'D2', 'A2', 'A3', 'A5', 'S1', 'S2', 'S3'];
    if(deliveryMethodt == DelivaryMethod.Electronic)
    {
      statementTypesSelectElement.disabled = true; 
      availableTemplates.unshift('P1');
    }
    
    var filter = "";
    for(var template in availableTemplates)
    {
      filter += (filter ? ' or ' : '') + `endswith(ipg_DocumentTypeId%2fipg_documenttypeabbreviation, '${template}')`;
    }
      var result = await Xrm.WebApi.retrieveMultipleRecords('ipg_document'
      ,`?$select=ipg_documentid&$orderby=createdon desc${deliveryMethodt == DelivaryMethod.Electronic ? "&$top=10" : ""}&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)&$filter=(_ipg_caseid_value eq '${incidentId}' and ipg_isactivepatientstatement eq true) and (${filter})`);
      
      for (let i = 0; i < availableTemplates.length; i++) {
        let abr = availableTemplates[i];
        let option = result.entities.find(x=> abr == x.ipg_DocumentTypeId.ipg_documenttypeabbreviation.replace("PST_",""));

        if(option)
        {
          addOption(statementTypesSelectElementId, abr, result.entities.indexOf(option) == 0);
        }
      }
      Xrm.Utility.closeProgressIndicator();
}

  async function populateDeliveryMethod(): Promise<void> {
    var incident = await Xrm.WebApi.retrieveRecord('incident', incidentId
    , "?$select=ipg_billtopatient,ipg_remainingpatientbalance,ipg_statecode,ipg_casestatus");
    if(incident.ipg_billtopatient 
      && incident.ipg_remainingpatientbalance > 0 
      && incident.ipg_statecode == CaseStatesEnum.PatientServices
      && incident.ipg_casestatus == CaseStatusEnum.Open)
      {
        addOption(deliveryMethodSelectElementId, DelivaryMethod.Electronic);
      }
  }

  async function getUriFromGlobalSettings():Promise<string> {
    var setting = await Xrm.WebApi.retrieveMultipleRecords('ipg_globalsetting'
    , `?$top=1&$select=ipg_value&$filter=ipg_name eq 'GeneratePSURL'`);

    return setting.entities[0].ipg_value;
  }

  async function checkPendingPatientStatement(): Promise<boolean> {
    let tasks = await Xrm.WebApi.retrieveMultipleRecords('ipg_statementgenerationtask'
    , `?$select=ipg_statementgenerationtaskid&$top=1&$filter=_ipg_caseid_value eq ${incidentId} and ipg_startdate le ${addDays(new Date(), 5).toISOString()} and statecode eq 0`);
    return tasks.entities.length > 0;
  }

  function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
  }

  function addOption(elId:string, optVal:string, seleceted:boolean=null)
  {
    var deliveryMethodSelectElement = <HTMLSelectElement>document.getElementById(elId);
    let newOption: HTMLOptionElement = document.createElement("option");
    newOption.value = optVal;
    newOption.text = optVal;
    if(seleceted)
    {
      newOption.selected = seleceted;
    }
    deliveryMethodSelectElement.add(newOption);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  document.getElementById("generateBtn").addEventListener('click',Intake.Incident.GeneratePatientStatement, false);
  document.getElementById("cancelBtn").addEventListener('click', () => window.close(), false);
  Intake.Incident.InitGenerateStatementPage();
});
