import React from "react";

import { Checkbox, FormControlLabel } from '@material-ui/core';

import { TemporaryService } from "../../services/temp-service";
import { IActualPart } from "../../models/IActualPart";

interface IProps {
    tempService: TemporaryService;
    label: string;
    caseId: string;
    actualPart: IActualPart;
    defaultValue?: boolean;
    attributeLogicalName?: string;
    disabled?: boolean;
    isDirty?: boolean;
    OnValueChange: CallableFunction;
}

interface IState {
    isDirty?: boolean;
    type: string;
    isChecked: boolean;
}

export class CustomUpdateCheckbox extends React.Component<IProps, IState> {
    state: IState = {
        isDirty: this.props.isDirty,
        isChecked: this.props.defaultValue || false,
        type: ""
    }

    private _onChangeValue = (event: React.ChangeEvent<{}>, checked: boolean) => {
        if (this.props.attributeLogicalName) {
            this.props.tempService.addFieldForUpdate(this.props.actualPart, this.props.attributeLogicalName, checked);

            this.setState({
                isChecked: checked
            });

        this.props.OnValueChange(this.props.attributeLogicalName, checked);
            
        }
    }

    render() {
        return (
            <FormControlLabel
                control={<Checkbox name={this.props.attributeLogicalName} />}
                label={this.props.label}
                disabled={this.props.disabled}
                checked={this.state.isChecked}
                onChange={this._onChangeValue} />
        );
    }
}
