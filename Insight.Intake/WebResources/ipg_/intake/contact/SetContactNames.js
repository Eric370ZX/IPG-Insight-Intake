var Intake;
(function (Intake) {
    var Contact;
    (function (Contact) {
        function ContactType(executionContext) {
            var formContext = executionContext.getFormContext();
            var object = new Array();
            object[0] = new Object();
            object[0].id = "8B383208-32B6-42B6-81BC-A0CBFED34FAE";
            object[0].name = "Manufacturer";
            object[0].entityType = "ipg_contacttype";
            formContext.getAttribute("ipg_contacttypeid").setValue(null);
            formContext.getAttribute("ipg_contacttypeid").setValue(object);
            formContext.getAttribute("ipg_manufacturername").setValue(localStorage.manufacturername);
            localStorage.removeItem("manufacturername");
        }
        Contact.ContactType = ContactType;
    })(Contact = Intake.Contact || (Intake.Contact = {}));
})(Intake || (Intake = {}));
