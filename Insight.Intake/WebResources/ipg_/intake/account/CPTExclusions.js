var intake;
(function (intake) {
    var Account;
    (function (Account) {
        function SetCarrierCPTDescription(formContext) {
            var cptCodeObject = Xrm.Page.getAttribute("ipg_cptcodeid").getValue();
            if (cptCodeObject != null) {
                var cptCodeId = cptCodeObject[0];
                var cptId = void 0;
                var epirationdate_1 = new Date('12/31/9999');
                var todayDate_1 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
                var guid = cptId.id.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("ipg_cptcode", guid, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {
                    Xrm.Page.getAttribute("ipg_carriercptname").setValue(results["ipg_cptname"] + " - " + results["ipg_cptcode"]);
                    Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
                    Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate_1);
                    Xrm.Page.getAttribute("ipg_expirationdate").setValue(epirationdate_1);
                }, function (error) {
                });
            }
        }
        Account.SetCarrierCPTDescription = SetCarrierCPTDescription;
        function DisplayCarrierCPTNewForm(primaryControl) {
            var formContext = primaryControl;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_associatedcpt";
            entityFormOptions["useQuickCreateForm"] = true;
            var formParameters = {};
            formParameters["ipg_accountid"] = formContext.data.entity.getId();
            formParameters["ipg_accountidname"] = formContext.getAttribute("name").getValue();
            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                console.log(success);
            }, function (error) {
                console.log(error);
            });
        }
        Account.DisplayCarrierCPTNewForm = DisplayCarrierCPTNewForm;
        function DisplayFacilityCPTNewForm(primaryControl) {
            var formid = "3d6E272CCE-A0FD-4F48-B0E7-72B28769E290";
            var formContext = primaryControl;
            var parameters = { formid: formid };
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_facilitycpt";
            var recordId = formContext.data.entity.getId();
            Xrm.Navigation.openForm(entityFormOptions, parameters);
        }
        Account.DisplayFacilityCPTNewForm = DisplayFacilityCPTNewForm;
        function SaveAndClose() {
            Xrm.Page.data.entity.save("saveandclose");
        }
        Account.SaveAndClose = SaveAndClose;
    })(Account = intake.Account || (intake.Account = {}));
})(intake || (intake = {}));
