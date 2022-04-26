import React from "react";

import "./actual-parts-cards-grid-buttons.css";

import { Button} from '@material-ui/core';
import { ICase } from "../../models/ICase";
import { IGenericPart } from "../../models/IGenericPart";
import { TemporaryService } from "../../services/temp-service";
import { TaskService } from "../../services/task-service";
import {INotification} from "../../models/INotification";

interface Props {
    case: ICase,
    parts: IGenericPart[],
    refreshGrid: Function,
    toggleProgressBar: Function,
    tempService: TemporaryService,
    taskService: TaskService,
    setNotification:(value:INotification) => void

}

interface State {
    isSaveDisabled: boolean,
    isSaveAndCloseDisabled: boolean,
    isCloseDisabled: boolean
}

export class ActualPartsCardsGridButtons extends React.Component<Props, State> {
    state: State = {
        isCloseDisabled: false,
        isSaveAndCloseDisabled: false,
        isSaveDisabled: false
    };

    saveButtonClick = async () => {
        await this.saveParts();
    }

    saveAndCloseButtonClick = async () => {
        await this.saveParts();

        window.close();
    }

    closeButtonClick() {
        window.close();
    }

    refresh = async() =>
    {
        this.props.refreshGrid();
    }

    private saveParts = async () => {
        this.setState({ isSaveDisabled: true, isCloseDisabled: true, isSaveAndCloseDisabled: true });
        this.props.toggleProgressBar(true);
        try
        {
            await this.props.tempService.createTempParts();
            await this.props.tempService.updateTempParts();
            await this.props.tempService.deleteRemovedParts();
            
            this.refreshSubGridsOnCase();

            if(this.props.tempService._service._entityName == "ipg_casepartdetail")
            {
                if(await this.props.tempService._service.isNoFacilityManufacturerRelationship(this.props.case.incidentid, this.props.case.ipg_FacilityId?.accountid))
                {
                    this.props.setNotification({message:`Manufacturer/Facility relationship not defined for ${this.props.case.ipg_FacilityId?.name}`, sevirity:"warning"});
                }    
            }
                    
            this.props.refreshGrid();
        }
        catch(e)
        {
            this.props.setNotification({message:`${(e as any)?.message}`, sevirity:"error"});
        }

        this.setState({ isSaveDisabled: false, isCloseDisabled: false, isSaveAndCloseDisabled: false });
        this.props.toggleProgressBar(false);
    }

    private refreshSubGridsOnCase = () =>
    {
        const parentForm = window.parent?.opener?.Xrm.Page;  

        if(parentForm)
        {
            (parentForm.getControl("EstimatedParts") as Xrm.Controls.GridControl)?.refresh();
            (parentForm.getControl("ActualParts") as Xrm.Controls.GridControl)?.refresh();

            let customGrid = parentForm.getControl("WebResource_CaseActualPartsGrid") as any;

            if(customGrid)
            {
            var src = customGrid.getSrc();

            var aboutBlank = "about:blank";
            customGrid.setSrc(aboutBlank);
            
            setTimeout(function(){ 
                customGrid.setSrc(src);
            }, 1000);
            }  
        }
    }

    render() {
        return (
            <div className="grid-buttons">
                <Button
                    id="grid-header-refresh-button"
                    className="grid-header-button"
                    variant="outlined"
                    disabled={this.state.isSaveDisabled}
                    onClick={this.saveButtonClick}>
                    Save
                </Button>
                <Button
                    id="grid-header-save-and-close-button"
                    className="grid-header-button"
                    variant="outlined"
                    disabled={this.state.isSaveAndCloseDisabled}
                    onClick={this.saveAndCloseButtonClick}>
                    Save and Close
                </Button>
                <Button
                    id="grid-header-close-button"
                    className="grid-header-button"
                    variant="outlined"
                    disabled={this.state.isCloseDisabled}
                    onClick={this.closeButtonClick}>
                    Close
                </Button>
            </div>
        );
    }
}
