/**
        * Open Primary and Secondary Functions
        * @function Intake.Account.D
        * @returns {void}
        */
function DisplayPrimaryContactForm(primaryControl) {
  var formContext = primaryControl;
  var entityFormOptions = {};
  entityFormOptions["entityName"] = "account";
  entityFormOptions["useQuickCreateForm"] = true;
  var formParameters = {};
  formParameters["ipg_accountid"] = formContext.data.entity.getId();
  formParameters["ipg_accountidname"] = formContext.getAttribute("name").getValue();
  localStorage.manufacturername = formContext.getAttribute("name").getValue();
  Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
   
    console.log(success);
  }, function (error) {
    console.log(error);
  });
}
 



/**
* Open new account form
* @function Intake.Account.displayFacilityCPTForm
* @returns {void}
*/


function DisplayCPTForm(primaryControl) {

  const formid = "06dc498c-ef54-eb11-a812-00224808592a";

  var formContext = primaryControl;
  var parameters = { formid: formid };
  var entityFormOptions = {};
  entityFormOptions["entityName"] = "ipg_facilitycpt";

  var recordId = formContext.data.entity.getId();
  //    var recordName   = formContext.getAttribute("new_name").getValue();    

  //    if (recordId != null)   { parameters["new_targetentityfieldid"] = recordId; }
  //   if (recordName != null) { parameters["new_targetentityfieldname"] = recordName; }

  Xrm.Navigation.openForm(entityFormOptions, parameters);
}
