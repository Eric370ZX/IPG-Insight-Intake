import * as React from "react";
import {IMultiselectOptionSetProps} from "./types";
import {VirtualizedComboBox} from "@fluentui/react";
import {PanelType} from "@fluentui/react/lib/Panel";
import {DirectionalHint} from "office-ui-fabric-react/lib-commonjs/common/DirectionalHint";

export default class MultiSelectOptionSetControl extends React.Component<IMultiselectOptionSetProps,{}>
{
    render()
    {
        return (
        <VirtualizedComboBox
        options={this.props.options}
        onChange = {this.props.onSelectedChanged}
        selectedKey={this.props.selectedKeys}
        allowFreeform
        autoComplete="on"
        multiSelect
        persistMenu
        calloutProps = {{directionalHint: DirectionalHint.topAutoEdge, calloutMaxHeight: 700
        }}
        ></VirtualizedComboBox>);  
    }
}