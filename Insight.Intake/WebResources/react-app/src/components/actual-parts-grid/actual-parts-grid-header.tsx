import React from "react";

import "./actual-parts-grid-header.css";

import { ActualPartsGridFilter} from "./actual-parts-grid-filter";

interface IActualPartsGridHeaderProps {
    onFilterChange: Function
}

export class ActualPartsGridHeader extends React.Component<IActualPartsGridHeaderProps> {
    render(){
        return (
            <div className="actual-parts-grid-header">
                <ActualPartsGridFilter onFilterChange={this.props.onFilterChange} />
            </div>
        );
    }
}