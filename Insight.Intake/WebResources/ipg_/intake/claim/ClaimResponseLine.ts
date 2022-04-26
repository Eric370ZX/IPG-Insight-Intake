function SetRemarkCode(executionContext) {
   
  let  formContext = executionContext.getFormContext();
  let  remarkId = formContext.getAttribute("ipg_remarkcodestring").getValue();

  if (remarkId != null) {
    Xrm.WebApi.retrieveMultipleRecords("ipg_claimresponseremarkcode", "?$select=ipg_name, ipg_description").then(function success(results) {
      if (results.entities.length != 0) {
        for (let  i = 0; i < results.entities.length; i++) {
          if (remarkId.includes(results.entities[i]["ipg_name"])) {

            formContext.getAttribute("ipg_remarkdescription").setValue(results.entities[i]["ipg_description"]);
          }
        }
      }
    }, function (error) {
    });
  }
}


function SetDenialCode(executionContext) {
   
  let  formContext = executionContext.getFormContext();
  let  denialId = formContext.getAttribute("ipg_denialcategorycode").getValue();
  let  optionsetAttribute = "ipg_denialcategorycode";
  let  options = Xrm.Page.getAttribute(optionsetAttribute).getOptions();
  if (denialId != null) {
    Xrm.WebApi.retrieveMultipleRecords("ipg_claimdenialcodecategory", "?$select=ipg_name,ipg_description").then(function success(results) {
      if (results.entities.length != 0) {
        for (let  i = 0; i < options.length; i++) {
          if (options[i].value == denialId) {
            // optName = options[i].text.toString();
            for (let  ii = 0; ii < results.entities.length; ii++) {
              if (options[i].text == results.entities[ii]["ipg_name"]) {
                formContext.getAttribute("ipg_denialdescription").setValue(results.entities[ii]["ipg_description"]);

              }
            }
          }
        }

      }
    }, function (error) {
    });
  }
}
