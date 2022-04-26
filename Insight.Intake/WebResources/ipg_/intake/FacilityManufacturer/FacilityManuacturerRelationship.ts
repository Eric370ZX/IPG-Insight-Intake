
namespace Intake.FacilityManufacturerRelationship {

  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    addFiltersToDistributorContactField(formContext);
    CreateTask(formContext);
    GetManufacturerName(formContext);
    RequestFacilityAccount(formContext);
  }

  /**
   * Called on change manufacturer
   * @function Intake.FacilityManufacturerRelationship.OnChangeManufacturer
   * @returns {void}
  */
  export function OnChangeManufacturer(executionContext) {
    const formContext = executionContext.getFormContext();
    
    changeFacilityManufacturerRelationshipName(formContext);
    addFiltersToDistributorContactField(formContext);
  }

  /**
   * Called on change facility
   * @function Intake.FacilityManufacturerRelationship.OnChangeFacility
   * @returns {void}
  */
   export function OnChangeFacility(executionContext) {
    const formContext = executionContext.getFormContext();

    changeFacilityManufacturerRelationshipName(formContext);
  }

  function changeFacilityManufacturerRelationshipName(formContext) {
    const facilityAttr = formContext.getAttribute("ipg_facilityid").getValue();
    const manufacturerAttr = formContext.getAttribute("ipg_manufacturerid").getValue();
    if(!facilityAttr || !manufacturerAttr)
      return;
    const facilityName = facilityAttr[0].name;
    const manufacturerName = manufacturerAttr[0].name;
    formContext.getAttribute("ipg_name").setValue(facilityName + " - " + manufacturerName);
  }

  export function CreateTask(formContext) {
    let currentDate = new Date();
    let day = currentDate.getDate()
    let month = currentDate.getMonth() + 1
    let year = currentDate.getFullYear()
    let currentMDYDate = month + "/" + day + "/" + year;
    let userSettings = Xrm.Utility.getGlobalContext().userSettings; // userSettings is an object with user information.
    let current_User_Id = userSettings.userId; // The user's unique id
    //let  current_User_Name = userSettings.userName;
    let current_User_Name = 'Vlad Vasiljevic';


    let current_User = userSettings.userName; // The user's unique id
    let manufacturerObject = Xrm.Page.getAttribute("ipg_manufacturerid").getValue();

    if (formContext.ui.getFormType() != 1) // 1 is for create
    {
      var Maanufacturerid = manufacturerObject[0].id;
      var manufacturerName = manufacturerObject[0].name;
      var object = new Array();
      object[0] = new Object();
      object[0].entityType = "account";
      object[0].name = manufacturerName;
      object[0].id = Maanufacturerid;
      formContext.getAttribute("ipg_manufacturerid").setValue(object);
    }

    if (manufacturerObject != null) {

      let facilityObject = Xrm.Page.getAttribute("ipg_facilityid").getValue();
      let facilityName = facilityObject[0].name;
      let facilityId = facilityObject[0].id;
      let id = manufacturerObject[0].id;
      let manufacturerName = manufacturerObject[0].name; 
      
      let description = "Part missing for Manufacturer " + manufacturerName + " does not have a relationship with Facility " + facilityName + ". Please create this association to resolve this task. ";
      let guid;
      guid = facilityId.replace("{", "").replace("}", "").toLowerCase();

      Xrm.WebApi.retrieveMultipleRecords("account", "?$select=ipg_manufactureraccountnumber,ipg_manufacturerisfacilityacctrequired,ipg_active,adx_createdbyusername &$filter=accountid  eq  ' " + guid + " '  and  name ne  ' " + current_User_Name + " '  and  ipg_active eq true  and  ipg_manufacturerisfacilityacctrequired eq true").then(function success(acctResult) {
        // if (acctResult.entities.length)
        if (acctResult.entities.length) {
          if (confirm("Manufacturer Account Number is required for this facility.  Do you want to save anyways?")) {
            Xrm.WebApi.online.retrieveMultipleRecords("task", "?$select=subject,activityid,activityid&$filter=subject eq 'Resolve Missing Facility Manufacturer Relationship'").then(function success(taskResults) {

              if (taskResults.entities.length == 0) {
                let entity = {};
                entity["subject"] = "Resolve Missing Facility Manufacturer Relationship";
                //  entity["description"] = "Part missing for Manufacturer " + manufacturerName + " does not have a relationship with Facility " + facilityName + ". Please create this association to resolve this task. ";
                entity["description"] = description;
                entity["scheduledstart"] = currentMDYDate;
                Xrm.WebApi.createRecord("task", entity).then(
                  function success(result) {
                    UpdateStatusFacilityManufacturer(formContext);
                  },
                  function (error) {
                    Xrm.Navigation.openErrorDialog(error);
                  }
                );
              }
            },
              function (error) {
                Xrm.Navigation.openErrorDialog(error);
              }
            );
          }
        }
      },
        function (error) {
          Xrm.Navigation.openErrorDialog(error);
        }
      );
    }
  }

  export function UpdateStatusFacilityManufacturer(formContext) {
    Xrm.WebApi.online.retrieveMultipleRecords("task", "?$select=subject&$filter=subject eq 'Resolve Missing Facility Manufacturer Relationship'").then(
      function success(taskResults) {
        if (taskResults.entities.length) {
          let updateStatus = taskResults.entities[0]["activityid"];
          let entity = {};
          entity["statecode"] = 1;

          Xrm.WebApi.updateRecord("task", updateStatus, entity).then(
            function success(result) {
            },
            function (error) {
              Xrm.Navigation.openErrorDialog(error);
            }
          );
        }
      },
      function (error) {
        Xrm.Navigation.openErrorDialog(error);
      }
    );
  }

  /**
   * add filters to distributor contact field
   * @function Intake.FacilityManufacturerRelationship.addFiltersToDistributorContactField
   * @returns {void}
  */
  export function addFiltersToDistributorContactField(formContext) {
    function addCustomLookupFilter(formContext) {
      let filters = [
        '<condition attribute="ipg_contacttypeidname" operator="like" value="%Distributor%" />',
      ];
      let filterXml = "<filter type=\"and\">" + filters.join('') + "</filter>";
      formContext.getControl("ipg_distributorcontact").addCustomFilter(filterXml, "contact");
    }
  }

  export function RequestFacilityAccount(formContext) {
    if (localStorage.accountId) {
      Xrm.WebApi.retrieveRecord("account", localStorage.accountId, "?$select=ipg_manufacturerisfacilityacctrequired").then(function success(results) {

        if (results.ipg_manufacturerisfacilityacctrequired == true) {
          formContext.getAttribute("ipg_manufactureraccountnumber")?.setRequiredLevel("required");
        }
      }, function (error) {
        console.log(error.message);
      });
    }
  
}




 export  function GetManufacturerName(formContext) {

   if (formContext.ui.getFormType() == 1) // 1 is for create
   {     
     formContext.getControl("ipg_active").setVisible(false);
     formContext.getAttribute("ipg_manufacturerrep")?.setRequiredLevel("required");
     formContext.getAttribute("ipg_manufacturerrepphone")?.setRequiredLevel("required");
     formContext.getAttribute("ipg_manufacturerrepemailmask")?.setRequiredLevel("required");

   }

    if (formContext.getAttribute("ipg_manufacturerid").getValue() == null && localStorage.accountId != null) {
      let accountIdNumber = localStorage.accountId;
      let accountName = localStorage.accountName;
      let object = new Array();
      object[0] = new Object();
      object[0].entityType = "account";
      object[0].name = accountName;
      object[0].id = accountIdNumber;


      if (localStorage.ManufacturerView === "true") {
        formContext.getAttribute("ipg_manufacturerid").setValue(null);
        formContext.getAttribute("ipg_manufacturerid").setValue(object);
        localStorage.removeItem("ManufacturerView");
      }


      if (localStorage.FacilityView === "true") {
        formContext.getAttribute("ipg_facilityid").setValue(null);
        formContext.getAttribute("ipg_facilityid").setValue(object);
        localStorage.removeItem("FacilityView");
      }

    }

    localStorage.removeItem("accountid");
    localStorage.removeItem("accountName");

  }

}
