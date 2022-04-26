/**
 * @namespace Intake.Account
 */
namespace Intake.Account {


  function EnterEmail(executionContext) {
    var formContext = executionContext.getFormContext();
    {
      let communicatePo = formContext.getAttribute("ipg_ccmmunicateposto").getValue();
      if (ValidateEmail(communicatePo) != false) {
        SendEmail(communicatePo);
      }
    }
  }


  function SendEmail(communicatePo) {
    const entry = communicatePo.split(";");
    let i
    for (i = 0; i < entry.length; i++) {
      if (entry[i] != "") {
        alert(entry[i] + " can be sent to Manufacturer with PO");
      }
    }
  }


  function ValidateEmail(communicatePo) {
    if (communicatePo.includes('@') == false) {
      alert("Enter ampersand before domain name");
      return false;
    }
    if (communicatePo.endsWith(';') == false) {
      {
        alert("Enter semicolon after each email addresses");
        return false;
      }
    }
  }
}
