/**
 * @namespace Intake.PortalComment
 */
var Intake;
(function (Intake) {
    var PortalComment;
    (function (PortalComment) {
        function initialize() {
            var incident = Xrm.Utility.getPageContext().input["createFromEntity"];
            Xrm.WebApi.retrieveRecord(incident.entityType, incident.id).then(function (value) {
                Xrm.Page.getAttribute("ipg_portalcreatedby").setValue(Xrm.Page.context.getUserName());
                Xrm.Page.getAttribute("ipg_referralnumber").setValue(value["title"]);
                Xrm.Page.getAttribute("ipg_facility").setValue(value["_ipg_facilityid_value@OData.Community.Display.V1.FormattedValue"]);
                Xrm.Page.getAttribute("ipg_patientname").setValue(value["ipg_patientfirstname"] + " " + value["ipg_patientlastname"]);
            });
            if (Xrm.Page.ui.getFormType() == 1) {
                //Set the default value for direction code as Outgoing for Web Client.
                if (Xrm.Page.getAttribute("adx_portalcommentdirectioncode").getValue() != null) {
                    Xrm.Page.getAttribute("adx_portalcommentdirectioncode").setValue(2);
                }
                //Set From lookup field with the value based on current user.
                Xrm.Page.getAttribute("from").setValue([{ id: "de597215-4dcc-22ae-6db3-48f8e2790988", name: "IPG", entityType: "account" }]);
                //Set To Lookup field value based on the Query String parameters passed from Case form.
                var parameters = Xrm.Page.context.getQueryStringParameters();
                if (parameters["partyid"] != null && parameters["partyname"] != null && parameters["partytype"] != null) {
                    var entityTypeName = null;
                    if (Xrm.Page.context.client.getClient() == 'Web') {
                        var entityTypeCode = parseInt(parameters["partytype"], 10);
                        entityTypeName = Xrm["Internal"].getEntityName(entityTypeCode);
                    }
                    else {
                        entityTypeName = parameters["partytype"];
                    }
                    Xrm.Page.getAttribute("to").setValue([{ id: parameters["partyid"], name: parameters["partyname"], entityType: entityTypeName }]);
                }
            }
        }
        PortalComment.initialize = initialize;
        function changeActiveStatus() {
            if (Xrm.Page.getAttribute("ipg_markedread").getValue() == false) {
                Xrm.Page.getAttribute("statecode").setValue(0);
                Xrm.Page.getAttribute("statuscode").setValue(923720000);
            }
            if (Xrm.Page.getAttribute("ipg_markedread").getValue() == true) {
                Xrm.Page.getAttribute("statecode").setValue(1);
                Xrm.Page.getAttribute("statuscode").setValue(427880000);
            }
            Xrm.Page.data.entity.save();
        }
    })(PortalComment = Intake.PortalComment || (Intake.PortalComment = {}));
})(Intake || (Intake = {}));
