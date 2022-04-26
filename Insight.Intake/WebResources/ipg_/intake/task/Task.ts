/**
 * @namespace Intake.Task
 * 
 */
namespace Intake.Task {
  const taskSubjectOptions = {
    portalDocument: 427880000,
    anyDocument: 427880001,
    portalField: 427880002,
    anyField: 427880003
  };
  enum taskGeneratedBy {
    user = 427880001,
    both = 427880002
  }

  enum caseStatus{
    Open = 923720000,
    Closed = 923720001
  }

  var tempTaskTypeFilterFetch;

  async function setDataFromTaskType(form: Xrm.FormContext) {
    if (form.ui.getFormType() === XrmEnum.FormType.Create) {
      const taskTypeField = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_tasktypeid");
      const taskTypeRef = taskTypeField.getValue() && taskTypeField.getValue().length > 0 && taskTypeField.getValue()[0];
      if (taskTypeRef) {
        const taskType = await Xrm.WebApi.retrieveRecord(taskTypeRef.entityType, taskTypeRef.id
          , "?$select=_ipg_documenttypeid_value,ipg_startdate,ipg_priority,ipg_subcategory,_ipg_assigntouserid_value,ipg_isportal,_ipg_taskcategoryid_value,_ipg_assigntoteam_value,ipg_name,ipg_description,ipg_duedate");

        await setDates(form, taskType);

        const assignToTeamAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>('ipg_assignedtoteamid');
        const ownerAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>('ownerid');

        if(taskType._ipg_assigntouserid_value)
        {
          ownerAttr?.setValue([
            {
              entityType: 'systemuser',
              name: taskType['_ipg_assigntouserid_value@OData.Community.Display.V1.FormattedValue'],
              id: taskType._ipg_assigntouserid_value
            }
          ])
        }
        
        if (taskType._ipg_assigntoteam_value) {
          let assignToTeamVal = [
            {
              entityType: 'team',
              name: taskType['_ipg_assigntoteam_value@OData.Community.Display.V1.FormattedValue'],
              id: taskType._ipg_assigntoteam_value
            }
          ];
          
          assignToTeamAttr?.setValue(assignToTeamVal);

          if(!taskType._ipg_assigntouserid_value)
          {
            ownerAttr?.setValue(assignToTeamVal);
          }

          assignToTeamAttr.fireOnChange();
        
        
        }
        
        //set owner the sam as on the Case/Referral if assignToTeam the same.
        let assignToTeamAttrVal = assignToTeamAttr?.getValue();
        const regardingAttrVal= form.getAttribute<Xrm.Attributes.LookupAttribute>("regardingobjectid")?.getValue();

        if(assignToTeamAttrVal?.length > 0 && regardingAttrVal?.length > 0 && (regardingAttrVal[0].entityType == "ipg_referral" || regardingAttrVal[0].entityType == "incident"))
        {
          let regarding = (await Xrm.WebApi.retrieveRecord(regardingAttrVal[0].entityType, regardingAttrVal[0].id, "?$select=_ipg_assignedtoteamid_value,_ownerid_value"));

          if(regarding._ipg_assignedtoteamid_value 
            && regarding._ipg_assignedtoteamid_value.replace("{","").replace("}","").toLowerCase() === assignToTeamAttrVal[0].id.replace("{","").replace("}","").toLowerCase() 
            && regarding["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"] === "systemuser")
          {           
            ownerAttr.setValue([{entityType: "systemuser", id:regarding._ownerid_value, name:regarding["_ownerid_value@OData.Community.Display.V1.FormattedValue"]}]);
          }
        }

        const taskCategoryAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>('ipg_taskcategoryid');
        if (taskType._ipg_taskcategoryid_value && taskCategoryAttr) {
          if (!taskCategoryAttr.getValue() || taskCategoryAttr.getValue()[0].id != taskType._ipg_taskcategoryid_value) {
            taskCategoryAttr.setValue([{
              entityType: 'ipg_taskcategory',
              name: taskType['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue']
              , id: taskType._ipg_taskcategoryid_value
            }]);
          }
        }

        const visibleOnPortalAttr = form.getAttribute<Xrm.Attributes.BooleanAttribute>('ipg_isvisibleonportal');
        visibleOnPortalAttr && visibleOnPortalAttr.setValue(taskType.ipg_isportal ?? false);

        const priorityAttr = form.getAttribute<Xrm.Attributes.OptionSetAttribute>('ipg_priority');
        if (priorityAttr && taskType.ipg_priority) {
          priorityAttr.setValue(taskType.ipg_priority);
        }

        const descriptionAttr = form.getAttribute<Xrm.Attributes.StringAttribute>('description');
        if (descriptionAttr && taskType.ipg_description) {
          descriptionAttr.setValue(taskType.ipg_description);
        }

        // Sub-Category
        const subCategoryAttr = form.getAttribute<Xrm.Attributes.StringAttribute>('subcategory');
        if (subCategoryAttr && taskType.ipg_subcategory) {
          subCategoryAttr.setValue(taskType.ipg_subcategory);
        }
      }
    }
  }

  async function setTitle(form: Xrm.FormContext) {
    if (form.ui.getFormType() === XrmEnum.FormType.Create) {
      const regarding = form.getAttribute<Xrm.Attributes.LookupAttribute>("regardingobjectid")?.getValue();
      const incident = regarding.length > 0 && regarding[0].entityType == 'incident' ? (await Xrm.WebApi.retrieveRecord('incident', regarding[0].id, "?$select=ipg_statecode")) : null;
      const subj = form.getAttribute<Xrm.Attributes.StringAttribute>("subject");
      const taskTypeId = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_tasktypeid")?.getValue()[0]?.id;

      if (incident  && subj && taskTypeId) {
        const taskType = await Xrm.WebApi.retrieveRecord("ipg_tasktype", taskTypeId, "?$select=ipg_name,ipg_tasktitle");
        subj.setValue(`${taskType.ipg_tasktitle ?? taskType.ipg_name ?? ''}`);
      }
    }
  }

  function ipg_tasktypeidOnChange(form: Xrm.FormContext) {
    setTitle(form);
    setDataFromTaskType(form);
  }

  function getRegardingCaseId(form: Xrm.FormContext) {
    const regObj = form.getAttribute<Xrm.Attributes.LookupAttribute>("regardingobjectid");
    if (!regObj || !regObj.getValue() || regObj.getValue()[0].entityType != "incident") {
      return;
    }
    return regObj.getValue()[0].name;
  }
  function getDescriptionForNotEnyFieldSubject(subject: any, caseId: any) {
    return `Please provide missing value of ${subject} for Case ${caseId}`
  }
  function changeFieldPropertyByTaskSubject(form: Xrm.FormContext, taskSubjectValue: any, taskSubjectOptionName: any) {
    if (taskSubjectValue == taskSubjectOptions.anyField) {
      form.getAttribute("ipg_fieldname")?.setRequiredLevel("required");
      form.getControl("ipg_fieldname")?.setVisible(true);

      var fieldName = form.getAttribute("ipg_fieldname")?.getValue();

    } else {
      form.getAttribute("ipg_fieldname")?.setRequiredLevel("none");
      form.getControl("ipg_fieldname")?.setVisible(false);

      let caseId = getRegardingCaseId(form);
      let description = getDescriptionForNotEnyFieldSubject(taskSubjectOptionName, caseId);
    }
  }
  function fieldNameOnChange(form: Xrm.FormContext) {
    var fieldName = form.getAttribute("ipg_fieldname")?.getValue();
  }
  function taskSubjectOnChange(form: Xrm.FormContext) {
    let taskSubjectValue = form.getAttribute("ipg_tasksubject")?.getValue();
    let taskSubjectOptionName = form.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_tasksubject")?.getSelectedOption().text;

    changeFieldPropertyByTaskSubject(form, taskSubjectValue, taskSubjectOptionName);
  }


  function setAssignedToTeam(form: Xrm.FormContext, teamName: string) {
    Xrm.WebApi.retrieveMultipleRecords('team', `?$select=name, teamid&$filter=contains(name,'${teamName}')`).then(
      function success(result) {
        let assignedToTeamAttr = form.getAttribute('ipg_assignedtoteamid');
        if (assignedToTeamAttr) {
          assignedToTeamAttr.setValue([{
            entityType: 'team',
            name: result.entities[0].name,
            id: result.entities[0].teamid
          }]);
        }
      }
    )
  }

  function setEndDate(endDateAttr: Xrm.Attributes.DateAttribute, startDate?: Date, ipg_duedate?: number) {
    if (endDateAttr && startDate) {
      if (ipg_duedate > 0) {
        const parametersConfirm = {
          "StartDate": startDate.toISOString(),
          "BusinessDaysToAdd": ipg_duedate
        }
        callAction("ipg_IPGIntakeActionsAddBusinessDays", parametersConfirm, true, (resultsConfirm) => {
          endDateAttr.setValue(new Date(resultsConfirm.ResultDate));
          Xrm.Utility.closeProgressIndicator();
        });
      }
      else {
        endDateAttr.setValue(startDate);
        Xrm.Utility.closeProgressIndicator();
      }
    }
  }

  async function setDates(form: Xrm.FormContext, taskType?: any) {
    if (form.ui.getFormType() === XrmEnum.FormType.Create
      && form.getAttribute<Xrm.Attributes.DateAttribute>("scheduledstart")
      && form.getAttribute<Xrm.Attributes.DateAttribute>("scheduledend")
      && taskType) {
      var startDateAttr = form.getAttribute<Xrm.Attributes.DateAttribute>("scheduledstart");
      var endDateAttr = Xrm.Page.getAttribute<Xrm.Attributes.DateAttribute>("scheduledend");
      if (taskType["ipg_startdate"] > 0) {
        var parametersConfirm = {
          "StartDate": new Date().toISOString(),
          "BusinessDaysToAdd": taskType["ipg_startdate"]
        };

        Xrm.Utility.showProgressIndicator("Processing...");
        callAction("ipg_IPGIntakeActionsAddBusinessDays", parametersConfirm, true, (resultsConfirm) => {
          startDateAttr.setValue(new Date(resultsConfirm.ResultDate));
          setEndDate(endDateAttr, startDateAttr.getValue(), taskType["ipg_duedate"] ?? 0);
        });
      }
      else {
        startDateAttr.setValue(new Date());
        setEndDate(endDateAttr, startDateAttr.getValue(), taskType["ipg_duedate"] ?? 0);
      }
    }
  }

  function ipg_taskblocksgatingOnChange(form: Xrm.FormContext) {
    const taskBlockGateAttr = form.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_taskblocksgating");
    const blockedgateAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_blockedgateid");
    if (taskBlockGateAttr && blockedgateAttr) {
      const blockGateVisible = taskBlockGateAttr ? taskBlockGateAttr.getValue() : false;
      const blockedgateCtrl = form.getControl<Xrm.Controls.LookupControl>("ipg_blockedgateid");
      blockedgateCtrl.setVisible(blockGateVisible);
      blockedgateAttr.setRequiredLevel(blockGateVisible ? "required" : "none");
      if (!blockGateVisible) {
        blockedgateAttr.setValue(null);
        blockedgateAttr.setSubmitMode("always");
      }
    }
  }
  /**
   * Called on load a Task form
   * @function Intake.Task.OnLoadForm
   * @returns {void}
   */
  export function OnLoadForm(context: Xrm.Events.EventContext) {
    const form = context.getFormContext();
    makeSystemTaskReadOnly(form);
    const taskType = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_tasktypeid");
    const taskSubject = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_tasksubject");
    taskType.addOnChange(() => ipg_tasktypeidOnChange(form));
    taskSubject.addOnChange(() => taskSubjectOnChange(form));


    const taskBlockGateAttr = form.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_taskblocksgating");
    if (taskBlockGateAttr) {
      taskBlockGateAttr.addOnChange(() => ipg_taskblocksgatingOnChange(form));
      taskBlockGateAttr.fireOnChange();
    }

    form.getAttribute("ipg_fieldname")?.addOnChange(() => { fieldNameOnChange(form); })

    var ownerAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>("ownerid");
    var assignToTeamAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_assignedtoteamid");
    ownerAttr?.addOnChange(OnOwnerChangeOrAssignedToteam);
    assignToTeamAttr?.addOnChange(OnOwnerChangeOrAssignedToteam);
    ownerAttr?.fireOnChange();

    if (form.ui.getFormType() === XrmEnum.FormType.Create) {
      form.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_generatedbycode")?.setValue(taskGeneratedBy.user);//set default as usertask
      form.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_generatedbycode")?.setSubmitMode("always");//set default as user

      var editablefields = ["subject", "ipg_taskcategoryid", "ipg_isvisibleonportal", "ipg_tasksubcategoryid", "description", "ipg_assignedtoteamid"];
      editablefields.forEach(f => form.getControl(f)?.setDisabled(false));

      const sStart = form.getAttribute<Xrm.Attributes.DateAttribute>("scheduledstart");
      if (sStart) {
        sStart.setValue(new Date());
      }


      var taskAttr = form.getAttribute("ipg_taskcategoryid");
      taskAttr.addOnChange(OnTaskCategoryChange);
      taskAttr.fireOnChange();
    }

    AddCustomFilterForTaskCategory(form);
    AddCustomFilterForPortalUser(form);

    ConfigureEnableFields(form);
    OnTaskCategoryChange(context);

    form.getControl("statuscode")?.removeOption(4);//Waiting on someone else
    form.getControl("statuscode")?.removeOption(7);//Deferred
    filterOwnerLookupForPoolTask(form);
  }

  /** Called on form save */
  // export function OnSave(context: Xrm.Events.SaveEventContext) {
  //   const formContext: Xrm.FormContext = context.getFormContext();

  //   const exceptionApprovedAttribute = formContext.getAttribute("ipg_is_exception_approved");
  //   if (exceptionApprovedAttribute?.getIsDirty() && exceptionApprovedAttribute?.getValue())
  //     Xrm.Navigation.navigateTo({
  //       pageType: "entityrecord",
  //       formId:"56EDCFE7-B4F3-4378-BAB5-FC8F40DB3AF3",
  //       entityName: "annotation",
  //       data: {
  //         "objectid": formContext.data.entity.getEntityReference()
  //       }
  //     }, {
  //       target: 2
  //     })
  //       .then(val => {}, error => console.error(error));
  // }

  async function filterOwnerLookupForPoolTask(formContext: Xrm.FormContext) {
    var taskType = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_tasktypeid")?.getValue();
    var assignedTeam = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_assignedtoteamid")?.getValue();
    if (taskType && taskType[0].name === "Request to Complete Case Mgmt. Work (Pool)") {
      if (assignedTeam && assignedTeam[0].name === "Pool") {
        setCustomOwnerLookupViewForPoolTask(formContext, assignedTeam[0].id);
      }
      else {
        var poolTeam = await Xrm.WebApi.retrieveMultipleRecords("team", "?$select=name,teamid&$filter=name eq 'Pool'");
        if (poolTeam.entities.length > 0) {
          setCustomOwnerLookupViewForPoolTask(formContext, poolTeam.entities[0].id);
        }
      }
    }
  }

  function setCustomOwnerLookupViewForPoolTask(formContext: Xrm.FormContext, poolTeamId: string) {
    var ownerControl = <Xrm.Controls.LookupControl>formContext.getControl("ownerid");
    if (ownerControl) {
      var viewId = "00000000-0000-0000-00AA-000013001121";
      var fetchXml = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
        <entity name="systemuser">
          <attribute name="fullname" />
          <attribute name="systemuserid" />
          <order attribute="fullname" descending="false" />
          <link-entity name="teammembership" from="systemuserid" to="systemuserid" visible="false" intersect="true">
            <link-entity name="team" from="teamid" to="teamid" alias="aa">
              <filter type="and">
                <condition attribute="teamid" operator="eq" value="${poolTeamId}" />
              </filter>
            </link-entity>
          </link-entity>
        </entity>
      </fetch>`;
      var viewDisplayName = "Team Members";
      var layoutXml = `<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>
      <row name='result' id='systemuserid'>
      <cell name='fullname' width='300' />
      </row>
      </grid>`;
      ownerControl.addCustomView(viewId, 'systemuser', viewDisplayName, fetchXml, layoutXml, true);
    }
  }

  async function OnOwnerChangeOrAssignedToteam(event: Xrm.Events.EventContext) 
  {
    const form = event.getFormContext();
    const ownerAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>("ownerid")
    const assignToTeamAttr = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_assignedtoteamid");

    let ownerAttrVal = ownerAttr?.getValue();
    let assignToTeamAttrVal = assignToTeamAttr?.getValue();

    form.getControl<Xrm.Controls.LookupControl>("ipg_portaluser").setDisabled(!(ownerAttrVal?.length > 0 && ownerAttrVal[0].name?.toLowerCase().indexOf("portal") > -1
      || assignToTeamAttrVal?.length > 0 && assignToTeamAttrVal[0].name?.toLowerCase().indexOf("portal") > -1));
  }

  export function ConfigureEnableFields(form: Xrm.FormContext) {
    Xrm.Utility.getGlobalContext().getCurrentAppName().then(appname => {
      if (appname === "Collections") {
        const subjVal = form.getAttribute<Xrm.Attributes.StringAttribute>("subject") && form.getAttribute<Xrm.Attributes.StringAttribute>("subject").getValue();

        if (subjVal && subjVal.indexOf("Scheduled Payment Plan/Follow up Date") > -1) {
          const startAttr = form.getAttribute<Xrm.Attributes.DateAttribute>("scheduledstart");
          const endAttr = form.getAttribute<Xrm.Attributes.DateAttribute>("scheduledend");

          startAttr.controls.forEach(control => {
            control.setDisabled(false);
          });

          endAttr.controls.forEach(control => {
            control.setDisabled(false);
          });
        }
      }
    });
  }
  /**
   * Called on change a Task Category field
   * @function Intake.Task.OnChangeTaskCategory
   * @returns {void}
   */
  export function OnChangeTaskCategory(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    makeSystemTaskReadOnly(formContext);
  }

  /**
   * the function makes the form of system task read-only
   * @function Intake.Task.makeSystemTaskReadOnly
   * @returns {void}
   */
  function makeSystemTaskReadOnly(formContext: Xrm.FormContext) {
    let controls = formContext.getControl();
    if (!formContext.getAttribute("ipg_taskcategorycode"))
      return;
    let taskCategoryValue = formContext.getAttribute("ipg_taskcategorycode").getValue();
    if (controls) { //system task
      controls.forEach((control: Xrm.Controls.Control): void => {
        if (taskCategoryValue === 427880001 && control.getVisible()) {
          control.setDisabled(true);
        }
      });
    }
  }

  /**
   * call Custom action
   * @function Intake.Incident.Ribbon.callAction
   * @returns {void}
  */
  function callAction(actionName, parameters, async, successCallback) {
    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + actionName, async);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
      if (this.readyState === 4) {
        req.onreadystatechange = null;
        if (this.status === 200) {
          successCallback(JSON.parse(this.response));
        } else {
          Xrm.Utility.closeProgressIndicator();
          alert(JSON.parse(this.response).error.message);
        }
      }
    };
    req.send(JSON.stringify(parameters));
  }


  function AddCustomFilterForPortalUser(form: Xrm.FormContext) {
    var portalUserCtr = form.getControl("ipg_portaluser");
    if (portalUserCtr) {
      portalUserCtr.addPreSearch(FilterPortalUser);
    }
  }

  async function AddCustomFilterForTaskCategory(form: Xrm.FormContext) {
    const taskCategoryCtr = form.getControl("ipg_taskcategoryid");
    if (taskCategoryCtr) {
      taskCategoryCtr.addPreSearch(await filterTaskCategory(form));
    }
  }

  function AddCustomFilterForTaskType(form: Xrm.FormContext) {
    const taskTypeCtr = form.getControl("ipg_tasktypeid");
    if (taskTypeCtr) {
      filterTaskTypes(form);
    }
  }

  function FilterPortalUser(executionContext: Xrm.Events.EventContext) {
    const viewId = '00000000-0000-0000-0000-27f3954693a9';
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let taskTypeatr = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_tasktypecode");
    let taskTypeText = taskTypeatr && taskTypeatr.getText();
    let regardingatr = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("regardingobjectid");
    let regardingatrVal = regardingatr && regardingatr.getValue() && regardingatr.getValue().length > 0 && regardingatr.getValue()[0];

    if (regardingatrVal && regardingatrVal.entityType == 'incident'
      && taskTypeText && taskTypeText.toLowerCase().indexOf('missing information') > -1) {
      let ctr: Xrm.Controls.LookupControl = formContext.getControl<Xrm.Controls.LookupControl>("ipg_portaluser");
      const layoutXml = "<grid name='contacts' object='2' jump='fullname' select='1' icon='1' preview='1'><row name='contact' id='contactid'><cell name='fullname'/></row></grid>";

      ctr.addCustomView(viewId, "contact", "Facility Contacts", BuildFetchForContactsByCaseFacility(regardingatrVal), layoutXml, true);
    }
  }

  function BuildFetchForContactsByCaseFacility(caseRef: Xrm.LookupValue): string {
    let fetchXml = [
      "<fetch>",
      "  <entity name='contact'>",
      "    <attribute name='fullname' />",
      "    <link-entity name='ipg_contactsaccounts' from='ipg_contactid' to='contactid' link-type='inner'>",
      "      <link-entity name='incident' from='ipg_facilityid' to='ipg_accountid' link-type='inner'>",
      "        <filter type='and'>",
      "          <condition attribute='incidentid' operator='eq' value='", caseRef.id, "'/>",
      "        </filter>",
      "      </link-entity>",
      "    </link-entity>",
      "  </entity>",
      "</fetch>",
    ].join("");

    return fetchXml;
  }

  async function filterTaskCategory(form: Xrm.FormContext) {
    var regarding = form.getAttribute<Xrm.Attributes.LookupAttribute>("regardingobjectid")?.getValue();
    var entityType = regarding.length > 0 ? regarding[0].entityType : null;
    var regardingEntity = entityType && entityType == 'incident' || entityType == 'ipg_referral'
      ? await Xrm.WebApi.retrieveRecord(entityType, regarding[0].id, "?$select=ipg_statecode") : null;

    const validTaskCategories = regardingEntity
      ? (await Xrm.WebApi.retrieveMultipleRecords(
        "ipg_tasktype", `?$select=_ipg_taskcategoryid_value&$filter=(ipg_generatedbycode eq ${taskGeneratedBy.user} or ipg_generatedbycode eq ${taskGeneratedBy.both}) and Microsoft.Dynamics.CRM.ContainValues(PropertyName=@p1,PropertyValues=@p2)&@p1='ipg_casestatecodes'&@p2=['${regardingEntity.ipg_statecode}']`)) : null;

    return (executionContext: Xrm.Events.EventContext) => {
      let formContext = executionContext.getFormContext();
      let taskCategoryCtr = formContext.getControl<Xrm.Controls.LookupControl>("ipg_taskcategoryid");

      if (regardingEntity && validTaskCategories.entities.length > 0) {
        const fetchTaskCategoryValues = validTaskCategories.entities
          .map(x => x["_ipg_taskcategoryid_value"] ? "<value>" + x["_ipg_taskcategoryid_value"] + "</value>" : "").join("");

        const filterXml = [
          "    <filter type='and'>",
          "      <condition attribute='ipg_taskcategoryid' operator='in' >",
          fetchTaskCategoryValues,
          "      </condition>",
          "    </filter>"].join("");

        taskCategoryCtr.addCustomFilter(filterXml);
      }
      else {
        taskCategoryCtr.setDisabled(true);
        taskCategoryCtr.setNotification("Task for this case can not be created.", "taskCantBeCreated");
      }
    }
  }

  function filterTaskTypes(form: Xrm.FormContext) {
    let taskTypeCtr = form.getControl<Xrm.Controls.LookupControl>("ipg_tasktypeid");
    let regarding = form.getAttribute<Xrm.Attributes.LookupAttribute>("regardingobjectid")?.getValue();
    if (regarding) {
      if (regarding.length > 0 && regarding[0].entityType == 'incident') {
        Xrm.WebApi.retrieveRecord('incident', regarding[0].id, "?$select=ipg_statecode,ipg_casestatus").then(
          function success(incident) {
            let taskCategory = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_taskcategoryid");
            addTaskTypeCustomFilter(taskCategory, taskTypeCtr, incident.ipg_statecode, incident.ipg_casestatus)
          }
        );
      }
      else if (regarding.length > 0 && regarding[0].entityType == 'ipg_referral') {
        Xrm.WebApi.retrieveRecord('ipg_referral', regarding[0].id, "?$select=ipg_statecode,ipg_casestatus").then(
          function success(ipg_referral) {
            let taskCategory = form.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_taskcategoryid");
            addTaskTypeCustomFilter(taskCategory, taskTypeCtr, ipg_referral.ipg_statecode, ipg_referral.ipg_casestatus)
          }
        );
      }
    }
  }

  function addTaskTypeCustomFilter(taskCategory, taskTypeCtr, statecode, casestatus){
    if (taskCategory?.getValue()) {

      tempTaskTypeFilterFetch = [
        "    <filter type='and'>",
        "      <condition attribute='ipg_isactive' operator='eq' value='1'/>",
        "      <condition attribute='statecode' operator='eq' value='0'/>",
        "      <condition attribute='ipg_casestatecodes' operator='contain-values'>",
        "       <value>", statecode, "</value>",
        "      </condition>",
        "      <condition attribute='ipg_taskcategoryid' operator = 'eq' value='", taskCategory.getValue()[0].id, "'/>",
        "    <filter type='", casestatus != caseStatus.Closed ? 'or' : 'and', "'>",
        "      <condition attribute='ipg_generatedbycode' operator = 'eq' value='", taskGeneratedBy.user, "'/>",
        "      <condition attribute='ipg_generatedbycode' operator = 'eq' value='", taskGeneratedBy.both, "'/>",
        "    </filter>",
        "    </filter>"].join("");

      let addFilter = (executionContext: Xrm.Events.EventContext) => taskTypeCtr.addCustomFilter(tempTaskTypeFilterFetch);

      taskTypeCtr.addPreSearch(addFilter);
    }
  }
  /**
 * add custom views to document type field
 * @function Intake.Task.addFiltersToDocumentType
 * @returns {void}
*/

  export function OnTaskCategoryChange(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    var entityLabel;
    let taskCategory = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_taskcategoryid");

    AddCustomFilterForTaskType(formContext);
    let tasktypeattr = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_tasktypeid");

    if (formContext.ui.getFormType() === 1) {
      if (taskCategory.getValue() != null) {
        entityLabel = taskCategory.getValue()[0].name;

        tasktypeattr.controls.forEach(c => c.setDisabled(false));
      }
      else {
        tasktypeattr.setValue(null);
        tasktypeattr.controls.forEach(c => c.setDisabled(true));
      }
    }
  }
}
