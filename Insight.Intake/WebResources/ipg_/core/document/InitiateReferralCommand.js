/**
 * @namespace Intake.Core.Document
 */
var Intake;
(function (Intake) {
    var Core;
    (function (Core) {
        var Document;
        (function (Document) {
            var xrm = window.Xrm || parent.Xrm;
            var pif1formId = '5650e49e-0ada-45b8-8aa7-2e40a5617c65';
            var pif2formId = '7266869e-f61a-45e5-84f9-c7c5446dbfc5';
            /**
             * Called when the user clicks 'Initiate Referral' button on Documents grid or form. Redirects the user to Referral form with these parameters:
             * -ipg_sourcedocumentid
             * -ipg_facilityid
             * -ipg_facility
             *
             * @function Intake.Core.Document.InitiateReferral
             * @returns {void}
             */
            function InitiateReferral(selectedDocumentsIds) {
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
                    }
                };
                var alertStrings = { confirmButtonLabel: "Ok", text: "", title: "Error" };
                var alertOptions = { height: 120, width: 260 };
                if (!selectedDocumentsIds || selectedDocumentsIds.length === 0) {
                    alertStrings.text = "Select at least one document.";
                    xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                    return;
                }
                var parameters = {};
                var fetchXML = generateFetchXml(selectedDocumentsIds);
                xrm.WebApi.retrieveMultipleRecords("ipg_document", fetchXML).then(function success(docs) {
                    var pifDoc = null;
                    if (!docs || docs.entities.length === 0) {
                        alertStrings.text = "Select at least one document.";
                        xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                        return;
                    }
                    var pifCount = 0;
                    docs.entities.forEach(function (doc) {
                        if (doc["ipg_documenttypeid.ipg_documenttypeabbreviation"] === 'PIF') {
                            pifCount++;
                            if (pifCount > 1) {
                                alertStrings.text = "Referrals can be initiated only from one PIF document.";
                                xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                                return;
                            }
                            pifDoc = doc;
                            var lookupValue = new Array();
                            lookupValue[0] = new Object();
                            lookupValue[0].id = pifDoc.ipg_documentid;
                            lookupValue[0].name = pifDoc.ipg_name;
                            lookupValue[0].entityType = "ipg_document";
                            parameters["ipg_sourcedocumentid"] = lookupValue;
                        }
                    });
                    if (!pifDoc || !pifDoc.ipg_documentid) {
                        alertStrings.text = "Referrals can be initiated from PIF documents only.";
                        xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                        return;
                    }
                    var initiatedOn = new Date();
                    try {
                        var newPifData = {
                            "ipg_InitiatedReferralById@odata.bind": "/systemusers(" + xrm.Page.context.getUserId().replace((/[{|}]/g), '') + ")",
                            "ipg_initiatedreferralon": initiatedOn
                        };
                        var requests = [new Sdk.UpdateRequest("ipg_document", pifDoc.ipg_documentid, newPifData)];
                        if (docs.entities.length > 1) {
                            var newRelatedDocData = {
                                "ipg_maindocument@odata.bind": "/ipg_documents(" + pifDoc.ipg_documentid + ")",
                                "ipg_InitiatedReferralById@odata.bind": "/systemusers(" + xrm.Page.context.getUserId().replace((/[{|}]/g), '') + ")",
                                "ipg_initiatedreferralon": initiatedOn
                            };
                            docs.entities.forEach(function (doc) {
                                if (doc.ipg_documentid != pifDoc.ipg_documentid) {
                                    requests.push(new Sdk.UpdateRequest("ipg_document", doc.ipg_documentid, newRelatedDocData));
                                }
                            });
                        }
                        xrm.WebApi.online.executeMultiple(requests).then(function (success) {
                            openNewReferralForm(pifDoc);
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                    catch (_a) {
                        alertStrings.text = "Referral was already initiated from provided PIF document. Please select another one.";
                        xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                    }
                }, function (error) {
                    alertStrings.text = "Could not retrieve ipg_document: " + error.message;
                    xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                });
                function openNewReferralForm(doc) {
                    if (doc["ipg_facilityid.accountid"] && doc["ipg_facilityid.name"]) {
                        parameters["ipg_facilityid"] = doc["ipg_facilityid.accountid"];
                        parameters["ipg_facility"] = doc["ipg_facilityid.name"];
                    }
                    localStorage.facilityid = doc["ipg_facilityid.accountid"];
                    localStorage.facilityname = doc["ipg_facilityid.name"];
                    parameters["ipg_isinitiatereferral"] = "1";
                    parameters["ipg_faxreceivedon"] = doc.ipg_documentreceiveddate;
                    var entityFormOptions = {
                        entityName: "ipg_referral",
                        formId: pif1formId,
                        navbar: "on",
                        cmdbar: true,
                        openInNewWindow: false
                    };
                    xrm.Navigation.openForm(entityFormOptions, parameters);
                }
                function generateFetchXml(documentIds) {
                    var filterValues = "";
                    documentIds.forEach(function (id) {
                        filterValues += "\n<value>" + id + "</value>";
                    });
                    var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\n                    <entity name=\"ipg_document\">\n                      <attribute name=\"ipg_documentid\" />\n                      <attribute name=\"ipg_name\" />\n                      <attribute name=\"ipg_initiatedreferralon\" />\n                      <attribute name=\"ipg_documentreceiveddate\" />\n                      <attribute name=\"ipg_referralid\" />\n                      <filter type=\"and\">\n                        <condition attribute=\"ipg_documentid\" operator=\"in\">" + filterValues +
                        "\n                        </condition>\n                      </filter>\n                      <link-entity name=\"ipg_documenttype\" from=\"ipg_documenttypeid\" to=\"ipg_documenttypeid\" visible=\"false\" link-type=\"outer\" alias=\"ipg_documenttypeid\">\n                        <attribute name=\"ipg_documenttypeid\" />\n                        <attribute name=\"ipg_documenttypeabbreviation\" />\n                      </link-entity>\n                      <link-entity name=\"account\" from=\"accountid\" to=\"ipg_facilityid\" visible=\"false\" link-type=\"outer\" alias=\"ipg_facilityid\">\n                        <attribute name=\"accountid\" />\n                        <attribute name=\"name\" />\n                      </link-entity>\n                    </entity>\n                  </fetch>";
                    fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
                    return fetchXml;
                }
            }
            Document.InitiateReferral = InitiateReferral;
        })(Document = Core.Document || (Core.Document = {}));
    })(Core = Intake.Core || (Intake.Core = {}));
})(Intake || (Intake = {}));
