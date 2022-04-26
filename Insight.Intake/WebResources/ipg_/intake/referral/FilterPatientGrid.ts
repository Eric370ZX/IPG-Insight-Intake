/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {
  const patientGridKey = 'Patients';
  const patientLastNameKey = 'ipg_patientlastname';
  const patientMiddleNameKey = 'ipg_patientmiddlename';
  const patientFirstNameKey = 'ipg_patientfirstname';
  const patientDateOfBirthKey = 'ipg_patientdateofbirth';
  const patientLookupKey = 'ipg_patientid';
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Referral.FilterPatientGrid
   * @returns {void}
   */
  export function FilterPatientGrid(): void {
    if ('undefined' === typeof Intake.Utility) {
      throw new Error('Utility does not exist. Add Utility first.');
    }
    const patientGridControl: Xrm.Page.GridControl = Xrm.Page.getControl(patientGridKey);
    const patientLastNameControl: Xrm.Controls.StringControl = Xrm.Page.getControl(patientLastNameKey);
    const patientMiddleNameControl: Xrm.Controls.StringControl = Xrm.Page.getControl(patientMiddleNameKey);
    const patientFirstNameControl: Xrm.Controls.StringControl = Xrm.Page.getControl(patientFirstNameKey);
    const patientDateOfBirthControl: Xrm.Controls.DateControl = Xrm.Page.getControl(patientDateOfBirthKey);
    const patientLastNameAttribute: Xrm.Attributes.StringAttribute = patientLastNameControl.getAttribute();
    const patientMiddleNameAttribute: Xrm.Attributes.StringAttribute = patientMiddleNameControl.getAttribute();
    const patientFirstNameAttribute: Xrm.Attributes.StringAttribute = patientFirstNameControl.getAttribute();
    const patientDateOfBirthAttribute: Xrm.Attributes.DateAttribute = patientDateOfBirthControl.getAttribute();
    const patientLookupAttribute: Xrm.Attributes.LookupAttribute = Xrm.Page.getAttribute(patientLookupKey);
    // Filter patients
    function filter(): void {
      const patientLastNameValue = patientLastNameAttribute.getValue();
      const patientMiddleNameValue = patientMiddleNameAttribute.getValue();
      const patientFirstNameValue = patientFirstNameAttribute.getValue();
      const patientDateOfBirthValue = patientDateOfBirthAttribute.getValue();
      const patientLookupValue = patientLookupAttribute.getValue();
      let promise = Promise.resolve();
      if (patientLookupValue && patientLookupValue.length) {
        const patientId = Intake.Utility.removeCurlyBraces(patientLookupValue[0].id);
        promise = promise.then(() =>
          Xrm.WebApi
            .retrieveRecord('contact', patientId)
            .then((response): void => {
              if (response) {
                let dateString;
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
            })
        );
      }
      promise = promise.then(() => {
        let filterXml = '<fetch><entity name="contact"><filter type="and"><condition attribute="contactid" operator="null" /></filter></entity></fetch>';
        // If at least one field contains data.
        if (patientLastNameValue || patientMiddleNameValue || patientFirstNameValue || patientDateOfBirthValue) {
          const filters: Array<string> = [
            '<condition attribute="customertypecode" operator="eq" value="923720000" />',
          ];
          if (patientLastNameValue) filters.push(`<condition attribute="lastname" operator="eq" value="${patientLastNameValue}" />`);
          if (patientMiddleNameValue) filters.push(`<condition attribute="middlename" operator="eq" value="${patientMiddleNameValue}" />`);
          if (patientFirstNameValue) filters.push(`<condition attribute="firstname" operator="eq" value="${patientFirstNameValue}" />`);
          if (patientDateOfBirthValue) {
            const dateString = (patientDateOfBirthValue.getFullYear() + '-' + ('0' + (patientDateOfBirthValue.getMonth() + 1)).slice(-2) + '-' + ('0' + patientDateOfBirthValue.getDate()).slice(-2));
            filters.push(`<condition attribute="birthdate" operator="eq" value="${dateString}" />`);
          }
          filterXml = `<fetch><entity name="contact"><filter type="and">${filters.join('')}</filter></fetch>`;
        }
        patientGridControl.setFilterXml(filterXml);
        patientGridControl.refresh();
      });
    }
    // Populate value to patient fields.
    function setPatientFields(): void {
      const patientLookupValue = patientLookupAttribute.getValue();
      if (patientLookupValue && patientLookupValue.length) {
        let patientId = Intake.Utility.removeCurlyBraces(patientLookupValue[0].id);
        Xrm.WebApi
          .retrieveRecord('contact', patientId)
          .then((response) => {
            if (response) {
              let dateOfBirth;
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
    let mutationObserver: MutationObserver;
    function onGridLoaded(): void {
      setTimeout(() => {
        // Disconnect old MutationObserver.
        if (mutationObserver) {
          mutationObserver.disconnect();
        }
        function subscirber(mutations: Array<MutationRecord>): void {
          const selectedRows = patientGridControl.getGrid().getSelectedRows();
          if (selectedRows.getLength() === 0) {
            patientLookupAttribute.fireOnChange();
          }
          else if (selectedRows.getLength() === 1) {
            const selectedPatient = selectedRows.get(0);
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
              .then(() => patientGridControl.refresh());
          }
        }
        mutationObserver = new MutationObserver(subscirber);
        // Observe all nodes with data-selectable class.
        const nodes = window.parent.document.querySelectorAll('[aria-selected]');
        for (let index = 0, length = nodes.length; index < length; index++) {
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
}
