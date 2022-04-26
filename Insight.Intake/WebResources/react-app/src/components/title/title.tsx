import React from "react";

import { ITitleProps } from "../../models/properties/ITitleProps";

export class Title extends React.Component<ITitleProps> {
    render() {
        return (
            <h2 className="actual-parts-header">
                {this.props.CaseNumber} - {this.props.PatientName}
            </h2>
        );
    }
}