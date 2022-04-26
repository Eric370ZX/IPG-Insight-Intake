var Intake;
(function (Intake) {
    var FacilityManufacturerRelationship;
    (function (FacilityManufacturerRelationship) {
        function RepresentativeContactInfo(executionContext) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
            var formContext = executionContext.getFormContext();
            var rep = formContext.getAttribute("ipg_manufacturerrep").getValue();
            var repPhone = formContext.getAttribute("ipg_manufacturerrepphone").getValue();
            var repEmail = formContext.getAttribute("ipg_manufacturerrepemailmask").getValue();
            (_a = formContext.getAttribute("ipg_manufacturerrepemailmask")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("none");
            (_b = formContext.getAttribute("ipg_manufacturerrepphone")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("none");
            (_c = formContext.getAttribute("ipg_manufacturerrep")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("none");
            if (rep === null && repPhone === null && repEmail === null) {
                (_d = formContext.getAttribute("ipg_manufacturerrepemailmask")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("required");
                (_e = formContext.getAttribute("ipg_manufacturerrepphone")) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("required");
                (_f = formContext.getAttribute("ipg_manufacturerrep")) === null || _f === void 0 ? void 0 : _f.setRequiredLevel("required");
            }
            if (rep != null) {
                (_g = formContext.getAttribute("ipg_manufacturerrepphone")) === null || _g === void 0 ? void 0 : _g.setRequiredLevel("none");
                (_h = formContext.getAttribute("ipg_manufacturerrepemailmask")) === null || _h === void 0 ? void 0 : _h.setRequiredLevel("none");
                (_j = formContext.getAttribute("ipg_manufacturerrep")) === null || _j === void 0 ? void 0 : _j.setRequiredLevel("required");
            }
            if (repPhone != null && repEmail != null) {
                (_k = formContext.getAttribute("ipg_manufacturerrep")) === null || _k === void 0 ? void 0 : _k.setRequiredLevel("none");
            }
        }
        FacilityManufacturerRelationship.RepresentativeContactInfo = RepresentativeContactInfo;
    })(FacilityManufacturerRelationship = Intake.FacilityManufacturerRelationship || (Intake.FacilityManufacturerRelationship = {}));
})(Intake || (Intake = {}));
