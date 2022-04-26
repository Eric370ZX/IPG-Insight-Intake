/**
 * @namespace Intake
 */
namespace Intake {
  const xmlPath:string = "../WebResources/ipg_/intake/ViewSettings.xml";
  let ViewSettingsXml:string = "";

  /**
  * Changes column title in a view
  * @function Intake.ChangeColumnTitle
  * @returns {void}
  */

  declare let $: typeof import("jquery");
  if (typeof ($) === 'undefined') {
    $ = (<any>window.parent).$;
  }

  $.ajax({
    type: "GET",
    url: xmlPath,
    dataType: "xml",
    success: function (xml) {
        ViewSettingsXml = xml;
    }
});

  export function ChangeColumnTitle(rowData, userLCID) {
    var divs = parent.document.querySelectorAll("div.wj-cells");
    divs.forEach(function (div) {
      var viewName = div.getAttribute("aria-label");
      if (viewName)
      {
        ReadXML(viewName);
      }
    });
  }

  function parseXML(xml, viewName) {
    $(xml).find("View").each(function () {
      if ($(this).attr("Name") == viewName) {
        var entity = $(this).attr("Entity");
        $(this).find("Field").each(function () {
          var fieldName = $(this).attr("Name");
          var title = $(this).attr("Title");
          var divs = parent.document.querySelectorAll('[id$="' + fieldName + '"]');
          divs.forEach(function (div) {
            var data_lp_id: string = div.getAttribute("data-lp-id");
            var rightEntity = true;
            if (data_lp_id) {
              if (data_lp_id.indexOf(entity) == -1)
                rightEntity = false;
            }
            if (rightEntity) {
              div.setAttribute("title", title);
              var divElements = div.getElementsByTagName("*");
              for (var i = 0; i < divElements.length; i++) {
                if ((divElements[i].className == "headerCaption") ||
                  (divElements[i].className == "grid-header-text") ||
                  divElements[i].className.includes("headerText-")) {
                  divElements[i].innerHTML = title;
                  break;
                }
              }
            }
          })
        })
      }
    });

  }

  function ReadXML(viewName) {
    try {
      console.log("Change columns in" + viewName);
      if(ViewSettingsXml)
      {
        parseXML(ViewSettingsXml, viewName);
      }
      else
      {
        $.ajax({
          type: "GET",
          url: xmlPath,
          dataType: "xml",
          success: function (xml) {
            ViewSettingsXml = xml;
            parseXML(ViewSettingsXml, viewName);
          }
        });   
      }
    } catch (e) {
      alert("Error while reading XML; Description – " + e.description);
    }
  }

  async function GetFirstFieldName(viewName:string):Promise<string | null | undefined>
  {
    if(ViewSettingsXml)
    {
      return $($(ViewSettingsXml).find("View[Name='" + viewName + "']").children()[0]).attr("Name")
    }
    else
    {
      var result = await fetch("../WebResources/ipg_/intake/ViewSettings.xml");
      ViewSettingsXml = await result.text();

      return $($(ViewSettingsXml).find("View[Name='" + viewName + "']").children()[0]).attr("Name")
    }
  }
 
  export async function ChangeColumnTittleOnLoadGrid(executionContext: Xrm.Events.EventContext, gridName:string)
  {
    const formContext = executionContext.getFormContext();
    const gridControl = formContext.getControl(gridName) as Xrm.Controls.GridControl;
    const entityName = gridControl.getEntityName();
    const controlName = gridControl.getName();
    let trycount = 0

    if(gridControl)
    {
      const checkIfGridRendered = async ()=> 
      {
        const currentView = gridControl.getViewSelector().getCurrentView();
        const firstfieldName = await GetFirstFieldName(currentView.name);
        
        if(parent.document.querySelectorAll("[id$='" + firstfieldName + "'][data-lp-id*='" + entityName + "']")?.length > 0
        || parent.document.querySelectorAll("[id$='" + firstfieldName + "'][data-lp-id*='" + controlName + "']")?.length > 0)
        {
          ReadXML(currentView.name);
          trycount = 0;
        }
        else if(trycount < 10)
        {
          trycount++;
          setTimeout(checkIfGridRendered,100)
        }
        else
        {
          trycount = 0;
        }
      };
      
      gridControl.addOnLoad(() => setTimeout(checkIfGridRendered,100));
  }
  }
}
