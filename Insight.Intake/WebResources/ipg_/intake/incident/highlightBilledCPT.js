var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
       * Called on load form
       * @function Intake.Case.HighlightBilledCPT
       * @returns {void}
      */
        function HighlightBilledCPT(executionContext) {
            var formContext = executionContext.getFormContext();
            //let cptCodeId1Control = formContext.getControl("ipg_cptcodeid1");
            //cptCodeId1Control.setLabel("Billed CPT");
            var incidentId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
            Xrm.WebApi.retrieveRecord("incident", incidentId, "?$select=_ipg_cptcodeid1_value,_ipg_cptcodeid2_value,_ipg_cptcodeid3_value,_ipg_cptcodeid4_value,_ipg_cptcodeid5_value,_ipg_cptcodeid6_value,_ipg_billedcptid_value").then(function (incident) {
                console.log(incident);
                if (incident._ipg_billedcptid_value) {
                    if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid1_value) {
                        var control = formContext.getControl("ipg_cptcodeid1");
                        control.setLabel("Billed CPT");
                    }
                    else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid2_value) {
                        var control = formContext.getControl("ipg_cptcodeid2");
                        control.setLabel("Billed CPT");
                    }
                    else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid3_value) {
                        var control = formContext.getControl("ipg_cptcodeid3");
                        control.setLabel("Billed CPT");
                    }
                    else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid4_value) {
                        var control = formContext.getControl("ipg_cptcodeid4");
                        control.setLabel("Billed CPT");
                    }
                    else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid5_value) {
                        var control = formContext.getControl("ipg_cptcodeid5");
                        control.setLabel("Billed CPT");
                    }
                    else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid6_value) {
                        var control = formContext.getControl("ipg_cptcodeid6");
                        control.setLabel("Billed CPT");
                    }
                }
            }, function (error) {
            });
        }
        Case.HighlightBilledCPT = HighlightBilledCPT;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
