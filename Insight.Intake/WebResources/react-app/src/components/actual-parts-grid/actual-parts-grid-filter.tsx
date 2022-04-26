import React from "react";
import TextField from '@material-ui/core/TextField';

interface IActualPartsGridFilterProps {
    onFilterChange: Function
}

export class ActualPartsGridFilter extends React.Component<IActualPartsGridFilterProps> {
    onChangeTextField = (event: React.ChangeEvent<{ value: unknown }>) => {
        this.props.onFilterChange(event.target.value as string);
    }

    render(){
        return (
            <div className="actual-parts-grid-filter">
                 <TextField size="small" label="Filter" onChange={this.onChangeTextField} />
            </div>
        );
    }
}