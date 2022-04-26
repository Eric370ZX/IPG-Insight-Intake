
export function SetUpdatedVendorID() {
 
  let manufacturer = Xrm.Page.getAttribute("name").getValue().trim();
  let state = Xrm.Page.getAttribute("ipg_stateid").getValue();


  if (state == null) {
    return;
  }

  let stateObject = state[0];
  let stateName = stateObject.name;
  let currVendorID = Xrm.Page.getAttribute("ipg_vendoridnumber").getValue();

  if (manufacturer.length == 0 || currVendorID)
    return;

  let vendorID = "F" + manufacturer.charAt(0).toUpperCase();
  let index = 0;
  let pos = manufacturer.indexOf(" ");
  let maxLen = manufacturer.split(' ').length;
  while (pos != -1 && index < maxLen) {
    manufacturer = manufacturer.substr(pos).trim();
    if (manufacturer.length > 0)
      vendorID = vendorID + manufacturer.charAt(0).toUpperCase();
    pos = manufacturer.indexOf(" ");
    index++;
  }

  Xrm.WebApi.retrieveMultipleRecords("account", "?$select=ipg_vendoridnumber&$filter=ipg_vendoridnumber eq '" + vendorID + "'").then(function success(result) {
    if (result.entities.length == 0) {

      Xrm.Page.getAttribute("ipg_vendoridnumber").setValue(vendorID + "" + localStorage.Abv);
      Xrm.Page.getControl("ipg_vendoridnumber").setDisabled(true);
    }
    else {
      Xrm.Page.getAttribute("ipg_vendoridnumber").setValue(result.entities[0]["ipg_vendoridnumber"]);
      Xrm.Page.getControl("ipg_manufacturergreatplainsvendorid").setDisabled(true);
    }
    localStorage.removeItem("Abv");
  },
    function (error) {
    });
  GetStateAbbrevivation(stateName);
}



export function GetStateAbbrevivation(stateName) {

  Xrm.WebApi.retrieveMultipleRecords("ipg_state", "?$select=ipg_abbreviation&$filter=ipg_name eq '" + stateName + "'").then(function success(results) {
    if (results.entities.length == 0) {
      return "--";
    }
    else {

      localStorage.Abv = results.entities[0]["ipg_abbreviation"];
    }
  },
    function (error) {
    });

}


