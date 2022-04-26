import React from "react";

import { IActualPartsCardsGridProps } from "../../models/properties/IActualPartsCardsGridProps";
import { ActualPartsCardsGridItem } from "../actual-parts-grid-card/actual-parts-cards-grid-item";

export class ActualPartsCardsGrid extends React.Component<IActualPartsCardsGridProps> {
    render() {
        return (
            <div>
                {(this.props.actualParts as []).map((part:any) => {
                    const id = part[this.props.tempService._service._keyField]
                    return (<ActualPartsCardsGridItem
                        key={id || part.Part?.productid}
                        actualPart={part}
                        dirtyAttributes={(this.props.dirtyParts.has(id) && this.props.dirtyParts.get(id)) || []}
                        refreshGrid={this.props.refreshGrid}
                        case={this.props.case}
                        caseId={this.props.case.incidentid}
                        tempService={this.props.tempService}
                        setNotification={this.props.setNotification}
                        disabled={this.props.disabled}
                        ></ActualPartsCardsGridItem>)
                })}
            </div>
        );
    }
}