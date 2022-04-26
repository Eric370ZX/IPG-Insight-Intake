/**
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        var patientGridKey = 'Patients';
        var patientLastNameKey = 'ipg_patientlastname';
        var patientMiddleNameKey = 'ipg_patientmiddlename';
        var patientFirstNameKey = 'ipg_patientfirstname';
        var patientDateOfBirthKey = 'ipg_patientdateofbirth';
        var patientLookupKey = 'ipg_patientid';
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Referral.FilterPatientGrid
         * @returns {void}
         */
        function FilterPatientGrid() {
            if ('undefined' === typeof Intake.Utility) {
                throw new Error('Utility does not exist. Add Utility first.');
            }
            var patientGridControl = Xrm.Page.getControl(patientGridKey);
            var patientLastNameControl = Xrm.Page.getControl(patientLastNameKey);
            var patientMiddleNameControl = Xrm.Page.getControl(patientMiddleNameKey);
            var patientFirstNameControl = Xrm.Page.getControl(patientFirstNameKey);
            var patientDateOfBirthControl = Xrm.Page.getControl(patientDateOfBirthKey);
            var patientLastNameAttribute = patientLastNameControl.getAttribute();
            var patientMiddleNameAttribute = patientMiddleNameControl.getAttribute();
            var patientFirstNameAttribute = patientFirstNameControl.getAttribute();
            var patientDateOfBirthAttribute = patientDateOfBirthControl.getAttribute();
            var patientLookupAttribute = Xrm.Page.getAttribute(patientLookupKey);
            // Filter patients
            function filter() {
                var patientLastNameValue = patientLastNameAttribute.getValue();
                var patientMiddleNameValue = patientMiddleNameAttribute.getValue();
                var patientFirstNameValue = patientFirstNameAttribute.getValue();
                var patientDateOfBirthValue = patientDateOfBirthAttribute.getValue();
                var patientLookupValue = patientLookupAttribute.getValue();
                var promise = Promise.resolve();
                if (patientLookupValue && patientLookupValue.length) {
                    var patientId_1 = Intake.Utility.removeCurlyBraces(patientLookupValue[0].id);
                    promise = promise.then(function () {
                        return Xrm.WebApi
                            .retrieveRecord('contact', patientId_1)
                            .then(function (response) {
                            if (response) {
                                var dateString = void 0;
                                if (patientDateOfBirthValue) {
                                    dateString = (patientDateOfBirthValue.getFullYear() + '-' + ('0' + (patientDateOfBirthValue.getMonth() + 1)).slice(-2) + '-' + ('0' + patientDateOfBirthValue.getDate()).slice(-2));
                                }
                                var patientLastName = patientLastNameValue ? patientLastNameValue.toLowerCase() : '';
                                var patientFirstName = patientFirstNameValue ? patientFirstNameValue.toLowerCase() : '';
                                var patientMI = patientMiddleNameValue ? patientMiddleNameValue.toLowerCase() : '';
                                var patientLastNameInResponse = response.lastname ? response.lastname.toLowerCase() : '';
                                var patientFirstNameInResponse = response.firstname ? response.firstname.toLowerCase() : '';
                                var patientMIInResponse = response.middlename ? response.middlename.toLowerCase() : '';
                                if (response.birthdate !== dateString || patientLastNameInResponse !== patientLastName ||
                                    patientMIInResponse !== patientMI || patientFirstNameInResponse !== patientFirstName) {
                                    patientLookupAttribute.setValue(null);
                                }
                            }
                        });
                    });
                }
                promise = promise.then(function () {
                    var filterXml = '<fetch><entity name="contact"><filter type="and"><condition attribute="contactid" operator="null" /></filter></entity></fetch>';
                    // If at least one field contains data.
                    if (patientLastNameValue || patientMiddleNameValue || patientFirstNameValue || patientDateOfBirthValue) {
                        var filters = [
                            '<condition attribute="customertypecode" operator="eq" value="923720000" />',
                        ];
                        if (patientLastNameValue)
                            filters.push("<condition attribute=\"lastname\" operator=\"eq\" value=\"" + patientLastNameValue + "\" />");
                        if (patientMiddleNameValue)
                            filters.push("<condition attribute=\"middlename\" operator=\"eq\" value=\"" + patientMiddleNameValue + "\" />");
                        if (patientFirstNameValue)
                            filters.push("<condition attribute=\"firstname\" operator=\"eq\" value=\"" + patientFirstNameValue + "\" />");
                        if (patientDateOfBirthValue) {
                            var dateString = (patientDateOfBirthValue.getFullYear() + '-' + ('0' + (patientDateOfBirthValue.getMonth() + 1)).slice(-2) + '-' + ('0' + patientDateOfBirthValue.getDate()).slice(-2));
                            filters.push("<condition attribute=\"birthdate\" operator=\"eq\" value=\"" + dateString + "\" />");
                        }
                        filterXml = "<fetch><entity name=\"contact\"><filter type=\"and\">" + filters.join('') + "</filter></fetch>";
                    }
                    patientGridControl.setFilterXml(filterXml);
                    patientGridControl.refresh();
                });
            }
            // Populate value to patient fields.
            function setPatientFields() {
                var patientLookupValue = patientLookupAttribute.getValue();
                if (patientLookupValue && patientLookupValue.length) {
                    var patientId = Intake.Utility.removeCurlyBraces(patientLookupValue[0].id);
                    Xrm.WebApi
                        .retrieveRecord('contact', patientId)
                        .then(function (response) {
                        if (response) {
                            var dateOfBirth = void 0;
                            if (response.birthdate) {
                                dateOfBirth = new Date(response.birthdate);
                            }
                            patientDateOfBirthAttribute.setValue(dateOfBirth);
                            patientLastNameAttribute.setValue(response.lastname);
                            patientMiddleNameAttribute.setValue(response.middlename);
                            patientFirstNameAttribute.setValue(response.firstname);
                        }
                    });
                }
            }
            // Create MutationObserver and listeners to listen for checking / unchecking the record in the grid.
            var mutationObserver;
            function onGridLoaded() {
                setTimeout(function () {
                    // Disconnect old MutationObserver.
                    if (mutationObserver) {
                        mutationObserver.disconnect();
                    }
                    function subscirber(mutations) {
                        var selectedRows = patientGridControl.getGrid().getSelectedRows();
                        if (selectedRows.getLength() === 0) {
                            patientLookupAttribute.fireOnChange();
                        }
                        else if (selectedRows.getLength() === 1) {
                            var selectedPatient = selectedRows.get(0);
                            patientLookupAttribute.setValue([
                                {
                                    id: selectedPatient.data.entity.getId(),
                                    entityType: 'contact',
                                    name: 'Automatically Selected Patient'
                                }
                            ]);
                            patientLookupAttribute.fireOnChange();
                        }
                        else {
                            Xrm.Navigation
                                .openErrorDialog({ message: 'Please, select only one patient.' })
                                .then(function () { return patientGridControl.refresh(); });
                        }
                    }
                    mutationObserver = new MutationObserver(subscirber);
                    // Observe all nodes with data-selectable class.
                    var nodes = window.parent.document.querySelectorAll('[aria-selected]');
                    for (var index = 0, length_1 = nodes.length; index < length_1; index++) {
                        mutationObserver.observe(nodes[index], { attributes: true, attributeFilter: ['aria-selected'] });
                    }
                }, 100);
            }
            // Register listeners
            patientGridControl.addOnLoad(onGridLoaded);
            patientLastNameAttribute.addOnChange(filter);
            patientMiddleNameAttribute.addOnChange(filter);
            patientFirstNameAttribute.addOnChange(filter);
            patientDateOfBirthAttribute.addOnChange(filter);
            patientLookupAttribute.addOnChange(setPatientFields);
            filter();
        }
        Referral.FilterPatientGrid = FilterPatientGrid;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
