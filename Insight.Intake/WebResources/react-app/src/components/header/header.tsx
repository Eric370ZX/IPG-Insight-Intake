import React from "react";
import { Title } from "../title/title";
import { IHeaderProps } from "../../models/properties/IHeaderProps";

export class Header extends React.Component<IHeaderProps> {
    render(){
        return (
            <div>
                <Title CaseNumber={this.props.Case.title} PatientName={this.props.Case["_ipg_patientid_value@OData.Community.Display.V1.FormattedValue"]}></Title>
            </div>
        );
    }
}