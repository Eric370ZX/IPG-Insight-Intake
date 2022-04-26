/**
 * @namespace Intake.PortalComment
 */
namespace Intake.PortalComment {
  const _entityName: string = "adx_portalcomment";

  export async function isRegardingReferralClosedAndHasAssociatedCase(selectedControlIds) {
    var results = [];
    for (const portalCommentId of selectedControlIds) {
      var isReferralClosedAndHasAssociatedCase = await CheckIfRegardingReferralClosedAndHasAssociatedCase(portalCommentId)["response"];
      results.push(!isReferralClosedAndHasAssociatedCase);
    }
    return results.indexOf(false) <= 0;
  }

  async function CheckIfRegardingReferralClosedAndHasAssociatedCase(portalCommentId): Promise<object> {
    const fetch = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
                     <entity name="adx_portalcomment">
                       <attribute name="activityid" />
                       <attribute name="subject" />
                       <attribute name="createdon" />
                       <order attribute="subject" descending="false" />
                       <filter type="and">
                         <condition attribute="activityid" operator="eq" uiname="reply" uitype="adx_portalcomment" value="${portalCommentId}" />
                       </filter>
                       <link-entity name="ipg_referral" from="ipg_referralid" to="regardingobjectid" link-type="inner" alias="ac">
                         <filter type="and">
                           <condition attribute="ipg_casestatus" operator="eq" value="923720001" />
                           <condition attribute="ipg_associatedcaseid" operator="not-null" />
                         </filter>
                       </link-entity>
                     </entity>
                   </fetch>`;
    var response = await Xrm.WebApi.retrieveMultipleRecords('adx_portalcomment', `?fetchXml=${fetch}`);
    return {
      response: response && response.entities.length > 0 ? true : false
    };
  }

  /**
  * Open new PortalComment form from existing PortalComment
  * @function Intake.PortalComment.ReplyToPortalComment
  * @returns {void}
  */
  export function ReplyToPortalComment(primaryControl) {
      var formContext = primaryControl;
      var entityFormOptions = {};
      entityFormOptions["entityName"] = "adx_portalcomment";
      entityFormOptions["useQuickCreateForm"] = false;
      var formParameters = {};
      if (formContext !== null) {
        formParameters["regardingobjectid"] = formContext.getAttribute("regardingobjectid").getValue();
        formParameters["to"] = formContext.getAttribute("from").getValue().filter(x => x.entityType === "contact");
        formParameters["description"] = "Regarding your note: \r\n" + formContext.getAttribute("description").getValue() + "\r\n";
        //From will default to current user
        //formParameters["from"] = formContext.getAttribute("to").getValue();
      }
      Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
        //formContext.data.refresh(true);
        console.log(success);
      }, function (error) {
        console.log(error);
      });
    }

    export async function ReplyToFromHomeGrid(selectedControl, selectedRecordId: string) {
      if (selectedRecordId) {
        selectedRecordId = selectedRecordId.replace("}", "").replace("{", "");

        const $select = `$select=_regardingobjectid_value,description`;
        const $expand = `$expand=adx_portalcomment_activity_parties($filter=participationtypemask eq 1)`;
        const params = `?${$select}&${$expand}`;

        const response = await fetch(`/api/data/v9.1/${_entityName}s(${selectedRecordId})${params}`, {
          method: "GET",
          headers: {
            "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
          }
        });

        const portalComment = await response.json();

        let regardingObject = await GetIncidentOrReferral(portalComment._regardingobjectid_value);
        regardingObject["name"] = portalComment["_regardingobjectid_value@OData.Community.Display.V1.FormattedValue"];

        const to = {
          entityType: "contact",
          id: portalComment.adx_portalcomment_activity_parties[0]["_partyid_value"],
          name: portalComment.adx_portalcomment_activity_parties[0]["_partyid_value@OData.Community.Display.V1.FormattedValue"]
        };

        const description = `Regarding your note: \r\n${portalComment.description}\r\n`;

        OpenPrefilledFromForReply([regardingObject], [to], description);
      }
    }
    async function RetriveSaleOrdersByIds(selectedRecordIds: string[]) {
      let idsQueryString: string = "&$filter=";
      selectedRecordIds.forEach((id) => {
        idsQueryString += `activityid eq ${id}`

        if (selectedRecordIds.indexOf(id) != selectedRecordIds.length - 1)
          idsQueryString += " or ";
      });
      return Xrm.WebApi.retrieveMultipleRecords(_entityName, `?$select=statuscode${idsQueryString}`);
    }
    export async function MarkAsUnreadEnableRule(selectedRecordIds: string[]) {
      const retrivedRecords = await RetriveSaleOrdersByIds(selectedRecordIds);
      if (retrivedRecords.entities.length > 0) {
        return !retrivedRecords.entities.some((comment) => comment["statuscode"] != 427880000 /* Viewed */);
      }
    }
    export async function MarkAsReadEnableRule(selectedRecordIds: string[]) {
      const retrivedRecords = await RetriveSaleOrdersByIds(selectedRecordIds);
      if (retrivedRecords.entities.length > 0) {
        return !retrivedRecords.entities.some((comment) => comment["statuscode"] == 427880000 /* Viewed */);
      }
    }

    export async function MarkAsViewed(selectedControl, selectedRecordIds: string[]): Promise<void> {
      Xrm.Utility.showProgressIndicator("Updating the records");

      try {
        for (let recordId of selectedRecordIds) {
          await Xrm.WebApi.updateRecord(_entityName, recordId, {
            statuscode: 427880000, //Viewed
            statecode: 1, //Completed
            ipg_markedread: true
          });
        }
      }
      finally {
        Xrm.Utility.closeProgressIndicator();
        (selectedControl.data && selectedControl.data.refresh()) || selectedControl.refresh();
      }
    }

    export async function MarkAsUnread(selectedControl, selectedRecordIds: string[]): Promise<void> {
      Xrm.Utility.showProgressIndicator("Updating the records");

      try {
        for (let recordId of selectedRecordIds) {
          await Xrm.WebApi.updateRecord(_entityName, recordId, {
            statuscode: 923720000, //Unread
            statecode: 0, //Open
            ipg_markedread: false
          });
        }
      }
      finally {
        Xrm.Utility.closeProgressIndicator();
        (selectedControl.data && selectedControl.data.refresh()) || selectedControl.refresh();
      }
    }

    function OpenPrefilledFromForReply(regardingObject, to, description) {
      const entityFormOptions = {
        entityName: "adx_portalcomment",
        useQuickCreateForm: false
      };

      let formParameters: Xrm.Utility.OpenParameters = {};

      formParameters["regardingobjectid"] = regardingObject;
      formParameters["to"] = to;
      formParameters["description"] = description;

      Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
        console.log(success);
      }, function (error) {
        console.log(error);
      });
    }

    async function GetIncidentOrReferral(recordId: string): Promise<object> {
      try {
        await Xrm.WebApi.retrieveRecord("incident", recordId, "?$select=title");
        return {
          id: recordId,
          entityType: "incident"
        };
      }
      catch (exc) {
        await Xrm.WebApi.retrieveRecord("ipg_referral", recordId, "?$select=ipg_name");
        return {
          id: recordId,
          entityType: "ipg_referral"
        };
      }
    }

    export function enableViewedButtonOnForm(primaryControl: Xrm.FormContext) {
      let formContext = primaryControl;
      let statusAttr = formContext.getAttribute("statuscode");
      if (statusAttr && statusAttr.getValue() && statusAttr.getValue() !== 427880000) {
        return true;
      }
      return false;
    }

    export function enableNewCommentButtonOnRibbon(primaryControl: any) {
      if (primaryControl) {
        if (primaryControl == "ipg_referral") {
          return false;
        }
      }
      return true;
    }
  }
