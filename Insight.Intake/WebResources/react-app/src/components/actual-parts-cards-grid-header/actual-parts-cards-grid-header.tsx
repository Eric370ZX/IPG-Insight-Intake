import React from "react";

import "./actual-parts-cards-grid-header.css";

import { ActualPartsCardsGridSearch } from "../actual-parts-cards-grid-search/actual-parts-cards-grid-search";
import { ActualPartsCardsGridButtons } from "../actual-parts-cards-grid-buttons/actual-parts-cards-grid-buttons";

import { ActualPartsAdvancedSearchModal } from "../actual-parts-advanced-search-modal/actual-parts-advanced-search-modal";

import { IActualPartsCardsGridHeaderProps } from "../../models/properties/IActualPartsCardsGridHeaderProps";
import { Button, Modal } from "@material-ui/core";


interface IState {
    isAdvancedSearchOpened: boolean;
    isSuccessSnackEnabled: boolean;
}

export class ActualPartsCardsGridHeader extends React.Component<IActualPartsCardsGridHeaderProps, IState> {
    state: IState = {
        isAdvancedSearchOpened: false,
        isSuccessSnackEnabled: false,

    };

    private _advancedSearchClick = () => {
        this.setState({
            isAdvancedSearchOpened: true
        });
    }

    private _advancedSearchClosed = () => {
        this.setState({
            isAdvancedSearchOpened: false
        });
    }

    private _isPartExists = async (productid:string):Promise<boolean> => 
    {
        return this.props.tempService.getPartByProductId(productid) != null
        || (await this.props.searchparts.getByProductAndCase(productid,this.props.case.incidentid,1))?.length > 0;
    }

    render() {
        return (
            <div className="actual-parts-cards-grid-header">
                <div className="col actual-parts-search">
                    <ActualPartsCardsGridSearch
                        setNotification={this.props.setNotification}
                        case={this.props.case}
                        refreshGrid={this.props.refreshGrid}
                        toggleProgressBar={this.props.toggleProgressBar}
                        addTemporaryParts={this.props.addTemporaryParts}
                        IsPartAlreadyExist={this._isPartExists}
                        disabled={this.props.disabled}></ActualPartsCardsGridSearch>
                </div>
                <div className="col actual-parts-advanced-search">
                    <Button
                        id="actual-parts-advanced-search-button"
                        variant="outlined"
                        disabled={this.props.disabled}
                        onClick={this._advancedSearchClick}>
                        Advanced Search
                    </Button>
                </div>

                <div className="col actual-parts-grid-buttons">
                    <ActualPartsCardsGridButtons
                        taskService={this.props.taskService}
                        setNotification={this.props.setNotification}
                        tempService={this.props.tempService}
                        case={this.props.case}
                        parts={this.props.actualParts}
                        toggleProgressBar={this.props.toggleProgressBar}
                        refreshGrid={this.props.refreshGrid}></ActualPartsCardsGridButtons>
                </div>

                <Modal
                    open={this.state.isAdvancedSearchOpened}
                    onClose={this._advancedSearchClosed}
                    disableBackdropClick={true}>
                    <ActualPartsAdvancedSearchModal
                        setNotification={this.props.setNotification}
                        case={this.props.case}
                        onClose={this._advancedSearchClosed}
                        addTemporaryParts={this.props.addTemporaryParts}
                        IsPartAlreadyExist={this._isPartExists}></ActualPartsAdvancedSearchModal>
                </Modal>
            </div>
        );
    }
}