import React from "react";

import "./actual-parts-cards-grid-view.css";

import Select from '@material-ui/core/Select';
import { MenuItem } from "@material-ui/core";

import { GridView } from "../../enums/grid-views";
import { IActualPartsCardsGridViewsProps } from "../../models/properties/IActualPartsCardsGridViewsProps";

interface State {
    currentView: GridView
};

export class ActualPartsCardsGridViews extends React.Component<IActualPartsCardsGridViewsProps, State> {
    state: State = {
        currentView: GridView.ActiveParts
    }

    onChangeView = (event: React.ChangeEvent<{ value: unknown }>) => {
        this.setState({
            currentView: event.target.value as GridView
        });

        this.props.onChangeView(event.target.value as GridView);
    }

    render() {
        return (
            <div className="grid-view-selector-container">
                <h3 className="grid-view-selector-label">Views:</h3>
                <Select
                    disabled={this.props.disabled}
                    className="grid-view-selector"
                    value={this.state.currentView}
                    onChange={this.onChangeView}>
                    <MenuItem value={GridView.AllParts}>All Parts</MenuItem>
                    <MenuItem value={GridView.ActiveParts}>Active Parts</MenuItem>
                    <MenuItem value={GridView.DebitedParts}>Debited Parts</MenuItem>
                </Select>
            </div>
        );
    }
}