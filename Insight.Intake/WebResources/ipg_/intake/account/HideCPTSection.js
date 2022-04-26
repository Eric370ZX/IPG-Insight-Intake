var Account;
(function (Account) {
    function CPTCodeFormOnLoad(executionContext) {
        HideCPTSection(executionContext);
        UpdateCPTRequiredLevel(executionContext);
        SetDefaultExpirationDate(executionContext);
        SetHighDollarDefaultValue(executionContext);
    }
    Account.CPTCodeFormOnLoad = CPTCodeFormOnLoad;
    function HideCPTSection(executionContext) {
        var formContext = executionContext.getFormContext();
        var tabObj = formContext.ui.tabs.get("CPTCode");
        var sectionObj = tabObj.sections.get("BlockedByCarrier");
        sectionObj.setVisible(false);
    }
    function SetHighDollarDefaultValue(executionContext) {
        var formContext = executionContext.getFormContext();
        var highDollar = formContext.getAttribute("ipg_highdollar").getValue();
        if (highDollar == null) {
            formContext.getAttribute("ipg_highdollar").setValue(false);
        }
    }
    function UpdateCPTRequiredLevel(executionContext) {
        var formContext = executionContext.getFormContext();
        var implantUsed = formContext.getAttribute("ipg_implantused").getValue();
        if (implantUsed != 923720001) {
            formContext.getAttribute("ipg_procedurename").setRequiredLevel("required");
            formContext.getAttribute("ipg_cptroupreporting").setRequiredLevel("required");
            formContext.getAttribute("ipg_cptgroupgeneral").setRequiredLevel("required");
            formContext.getAttribute("ipg_cptgroupdetail").setRequiredLevel("required");
            formContext.getAttribute("ipg_apcweight").setRequiredLevel("required");
        }
        else {
            formContext.getAttribute("ipg_procedurename").setRequiredLevel("none");
            formContext.getAttribute("ipg_cptroupreporting").setRequiredLevel("none");
            formContext.getAttribute("ipg_cptgroupgeneral").setRequiredLevel("none");
            formContext.getAttribute("ipg_cptgroupdetail").setRequiredLevel("none");
            formContext.getAttribute("ipg_apcweight").setRequiredLevel("none");
        }
    }
    Account.UpdateCPTRequiredLevel = UpdateCPTRequiredLevel;
    function SetDefaultExpirationDate(executionContext) {
        var formContext = executionContext.getFormContext();
        var expDate = formContext.getAttribute("ipg_expirationdate").getValue();
        var epirationdate = new Date('12/31/9999');
        if (expDate == null) {
            formContext.getAttribute("ipg_expirationdate").setValue(epirationdate);
        }
    }
})(Account || (Account = {}));
