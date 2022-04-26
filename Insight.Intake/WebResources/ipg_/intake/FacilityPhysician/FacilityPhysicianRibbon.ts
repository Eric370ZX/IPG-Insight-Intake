/**
 * @namespace Intake.FacilityPhysician.Ribbon
 */
 namespace Intake.FacilityPhysician.Ribbon {
      
    // just fast format for ids to not use Utility.js in ribbon file
    const format: Function = (id: string): string=> {
      return id.replace("{", "").replace("}", "").toLowerCase()
    }
  
     /**
     * Show buttons only on related Tab
     * @function Intake.FacilityPhysician.Ribbon.ShowOnlyOnRelatedTab
     * @returns {bool}
     */
    export async function ShowOnlyOnRelatedTab(selectedControl: Xrm.Controls.GridControl, tabName: string){    
      return selectedControl["_controlName"] === tabName;
    }
    
     /**
     * Hide buttons only on related Tab
     * @function Intake.FacilityPhysician.Ribbon.HideOnlyOnRelatedTab
     * @returns {bool}
     */
      export async function HideOnlyOnRelatedTab(selectedControl: Xrm.Controls.GridControl, tabName: string){    
        return selectedControl["_controlName"] != tabName;
      }
  
    /**
     * Is All Selected Records Are Active
     * @function Intake.FacilityPhysician.Ribbon.IsAllSelectedRecordsAreActive
     * @returns {bool}
     */
    export async function IsAllSelectedRecordsAreActive(selectedIds: string[]){
        if (selectedIds && selectedIds.length > 0){
            var fetchXml = generateFetchXmlToRetrieveFacilityPhysiciansByIds(selectedIds);
            var records = await Xrm.WebApi.retrieveMultipleRecords("ipg_facilityphysician", fetchXml);
            if (records && records.entities.length > 0){
                return records.entities.every(function (r) {
                    return r.ipg_status;
                    });
            }
        }
        return false;
    }

    /**
     * Is All Selected Records Are Inactive
     * @function Intake.FacilityPhysician.Ribbon.IsAllSelectedRecordsAreInactive
     * @returns {bool}
     */
     export async function IsAllSelectedRecordsAreInactive(selectedIds: string[]){
        if (selectedIds && selectedIds.length > 0){
            var fetchXml = generateFetchXmlToRetrieveFacilityPhysiciansByIds(selectedIds);
            var records = await Xrm.WebApi.retrieveMultipleRecords("ipg_facilityphysician", fetchXml);
            if (records && records.entities.length > 0){
                return records.entities.every(function (r) {
                    return !r.ipg_status;
                    });
            }
        }
        return false;
    }

    function generateFetchXmlToRetrieveFacilityPhysiciansByIds(recordIds) {
        var filterValues = "";
        recordIds.forEach((id) => {
            filterValues += "\n<value>" + id + "</value>";
        });
        var fetchXml =
            `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
                        <entity name="ipg_facilityphysician">
                        <attribute name="ipg_facilityphysicianid" />
                        <attribute name="ipg_status" />
                        <filter type="and">
                            <condition attribute="ipg_facilityphysicianid" operator="in">` +
            filterValues +
            `
                            </condition>
                        </filter>
                        </entity>
                    </fetch>`;
        fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
        return fetchXml;
    }

    /**
     * Set Active Flag
     * @function Intake.FacilityPhysician.Ribbon.setActiveFlag
     * @returns {void}
     */
    export function setActiveFlag(primaryControl, selectedIds: string[], isActive: boolean){
        var Sdk = {
            UpdateRequest: function (entityTypeName, id, payload) {
              this.etn = entityTypeName;
              this.id = id;
              this.payload = payload;
              this.getMetadata = function () {
                return {
                  boundParameter: null,
                  parameterTypes: {},
                  operationType: 2,
                  operationName: "Update",
                };
              };
            },
          };

        var requests = [];

        let newActiveFlag = {
            "ipg_status" : isActive
        };

        selectedIds.forEach(recordId => {
            requests.push(
                new Sdk.UpdateRequest(
                  "ipg_facilityphysician",
                  recordId,
                  newActiveFlag
                )
              );
        });

        Xrm.WebApi.online.executeMultiple(requests).then(
            function (success) {
                var alertStrings = {
                    confirmButtonLabel: "Ok",
                    text: "The records are updated successfully",
                    title: "Success",
                  };
                  var alertOptions = { height: 120, width: 260 };
                  Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function (success) {
                      primaryControl.refresh(true);
                    }
                  );
            },
            function (error) {
              console.log(error.message);
            }
          );
      }

  }