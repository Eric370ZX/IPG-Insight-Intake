import React from "react";

import { debounce } from 'lodash';

import { MenuItem } from '@material-ui/core';
import { Select } from '@material-ui/core';
import { TextField } from '@material-ui/core';

import './custom-update-text-field.css';
import { EditableTextFieldPropType } from "../../models/IEditableTextFieldProp";
import { POType } from '../../enums/po-types';
import NumberFormat from 'react-number-format';

interface IProps {
    defaultValue?: any;
    attributeLogicalName?: string;
    disabled?: boolean;
    isDirty?: boolean;
    isNotModifiable?: boolean;
    type?: EditableTextFieldPropType;
    OnValueChange: CallableFunction;
}

interface IState {
    value:any
}

export class CustomUpdateTextField extends React.Component<IProps, IState> {
    private readonly _textFieldHeight: number = 32;

    public constructor(props: IProps) {
        super(props);
        this.state = {
            value: this.props.defaultValue || ''
        };
    }

    componentDidUpdate(prevProps:IProps) {
        if (prevProps !== this.props)
        {
            (this.props.defaultValue || '')  != this.state.value && this.setState({value:this.props.defaultValue});
        }
    }

    private _onChange = (value:any) => {
        if (this.props.attributeLogicalName) {
        if (this.props.type === EditableTextFieldPropType.Number 
            || this.props.type === EditableTextFieldPropType.Сurrency)
        {
            value = +value;
        }

        this.setState({
            value: value
        });
        
        if(this.props.attributeLogicalName)
        {
            this.props.OnValueChange(this.props.attributeLogicalName, value);
        }
    }};

    private _delayedOnChange = debounce(value=>this._onChange(value), 800);

    render() {
        const textFieldInputProps = { style: { height: this._textFieldHeight, padding: '0 14px' }, pattern: EditableTextFieldPropType.Number == this.props.type ? "[0-9]*" : undefined };
        const textFieldStyle = { height: this._textFieldHeight };

        return (
            this.props.type == EditableTextFieldPropType.OptionSet ? 
            <Select
            className={this.props.isDirty && !this.props.isNotModifiable ? "dirty" : ""}
            value={this.state.value}
            onChange={event => this._onChange( event.target.value)}
            disabled={this.props.disabled}
            inputProps={{ 'aria-label': 'Without label' }}>
            {Object.keys(POType).filter(k => parseInt(k) >= 0).map(povalue => <MenuItem value={povalue}>{POType[Number(povalue) as POType]}</MenuItem>)}
          </Select>
            :  this.props.type == EditableTextFieldPropType.Сurrency ? 
                <NumberFormat
                value = {this.state.value}
                thousandSeparator={true} 
                prefix={'$'} 
                customInput={TextField}
                style={textFieldStyle}
                decimalScale={2}
                fixedDecimalScale={true}
                className={this.props.isDirty && !this.props.isNotModifiable  ? "dirty" : ""}
                displayType={this.props.isNotModifiable ? "text" : "input"}
                disabled={this.props.disabled}
                onValueChange={(values) => this._delayedOnChange(values.value)}
                />
                :<TextField
                className={this.props.isDirty && !this.props.isNotModifiable  ? "dirty" : ""}
                disabled={this.props.disabled}
                style={textFieldStyle} 
                inputProps={textFieldInputProps}
                size="small" variant="standard"
                type={EditableTextFieldPropType[EditableTextFieldPropType.Text].toLowerCase()}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => {if(e.target.validity.valid || e.target.value === "")
                        {
                            this._onChange(e.target.value)}                   
                        }
                    }
                value={this.state.value}></TextField>
        );
    }
}