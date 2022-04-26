var Intake;
(function (Intake) {
    var Configs;
    (function (Configs) {
        var DuplicateButton;
        (function (DuplicateButton) {
            var _enabledEntities = [
                "ipg_unsupportedprocedure",
                "ipg_staterule",
                "ipg_carrierstaterelationship",
                "ipg_carrierpart",
                "product",
                "entitlement",
                "ipg_carriernetwork",
                "ipg_carrierfeeschedule",
                "ipg_carrierprecertcpt",
                "ipg_carrierprecerthcpcs",
                "ipg_carrierpricinginformation",
                "ipg_chargecenter",
                "ipg_claimstatuscode",
                "ipg_cptcode",
                "ipg_documenttype",
                "ipg_dxcode",
                "account",
                "ipg_facilitycarriercptrule",
                "ipg_facilitycpt",
                "ipg_facilitymanufacturerrelationship",
                "ipg_feeschedule",
                "ipg_homeplancarriermap",
                "ipg_masterhcpcs",
                "ipg_casepartdetail",
                "contact",
                "ipg_procedurename"
            ];
            function GetEnabledEntities() {
                return _enabledEntities;
            }
            DuplicateButton.GetEnabledEntities = GetEnabledEntities;
        })(DuplicateButton = Configs.DuplicateButton || (Configs.DuplicateButton = {}));
    })(Configs = Intake.Configs || (Intake.Configs = {}));
})(Intake || (Intake = {}));
