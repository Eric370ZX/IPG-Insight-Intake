import React from "react";

import "./actual-parts-cards-grid-search.css";

import { TextField, CircularProgress } from "@material-ui/core";
import { Autocomplete, AutocompleteInputChangeReason } from '@material-ui/lab';

import { PartService } from '../../services/parts-service';
import { IPart } from "../../models/IPart";
import { IActualPart } from "../../models/IActualPart";
import { debounce } from 'lodash';
import { ICase } from "../../models/ICase";
import {INotification} from "../../models/INotification"

interface State {
    isOpen: boolean,
    isLoading: boolean,
    isDisabled: boolean,
    options: IPart[],
    inputValue: string;
    autocompleteKey: string;
}

interface Props {
    refreshGrid: Function;
    toggleProgressBar: Function;
    addTemporaryParts: Function;
    case: ICase;
    setNotification:(value:INotification) => void;
    IsPartAlreadyExist(productid:string):Promise<boolean>;
    disabled:boolean;
}

export class ActualPartsCardsGridSearch extends React.Component<Props, State> {
    state: State = {
        isOpen: false,
        isDisabled: false,
        options: [],
        isLoading: this.state && this.state.isOpen && this.state.options.length === 0,
        inputValue: "",
        autocompleteKey: new Date().toISOString(),
    };

    partService: PartService;

    constructor(props: any) {
        super(props);
        this.partService = new PartService();
    }

    onSearchOpened() {
        this.setState({ isOpen: true });
        this.setState({
            isLoading: true,
        });

        this.delayedInputChange(this.state.inputValue ?? "");
    }

    updatePartsSource(value: string) {
        (async () => {
            var dos = this.props.case.ipg_actualdos || this.props.case.ipg_surgerydate;

            const parts = value ? await this.partService.search(value) : await this.partService.getActive();

            this.setState({
                options: parts,
                isLoading: false
            });
        })();
    }

    onInputChange(event: React.ChangeEvent<{}> | null, inputValue: string, reason: AutocompleteInputChangeReason | null) {
        //inputValue = inputValue.replace(/[^a-zA-Z0-9 -]/g, '');
        this.setState({ inputValue: inputValue });

        if ((reason === "input" || reason === null) && inputValue !== this.state.inputValue) {
            this.setState({
                isLoading: true,
            });

            this.delayedInputChange(inputValue);
        }
    }

    async onChange(event: React.ChangeEvent<{}>, newValue: IPart | null) {
        this.setState({ isDisabled: true });
        this.props.toggleProgressBar(true);

        if (newValue && newValue.productid) {
            var dos = this.props.case.ipg_actualdos || this.props.case.ipg_surgerydate;
          var carrierid = this.props.case._ipg_carrierid_value;
          var homeplanid = this.props.case._ipg_homeplancarrierid_value;

            try
            {
                if(await this.props.IsPartAlreadyExist(newValue.productid))
                {
                    this.props.setNotification({message:`Part ${newValue.ipg_manufacturerpartnumber}, ${newValue.name} already exists.`, sevirity:"error"});
                }
                else if(!this.partService.isWithinEffectiveDate(dos, newValue))
                {
                    this.props.setNotification({message:`Part ${newValue.ipg_manufacturerpartnumber}, ${newValue.name} cannot be added to the Case.  Case DOS outside of the Part effective Date Range`, sevirity:"error"});
                }
                else if (!await this.partService.IsPartBillable(dos, newValue.productid, carrierid, homeplanid))
                {
                    this.props.setNotification({message:`Part ${newValue.ipg_manufacturerpartnumber}, ${newValue.name} is not billable!`, sevirity:"error"});
                }
                else
                {
                    this.props.addTemporaryParts([{
                        Part: newValue
                    } as IActualPart]);
                }
            }
            catch(e)
            {
                this.props.setNotification({message:"There is an Error. Please try Later. If reproduce contact System Administrator!", sevirity:"error"});
            }

            this.setState({ isDisabled: false, autocompleteKey: new Date().toISOString() });
            this.props.toggleProgressBar(false);
        }
    }

    delayedInputChange = debounce(value => this.updatePartsSource(value), 1000);

    onSearchClosed() {
        this.setState({ isOpen: false });
    }

    render() {
        return (
            <div>
                <Autocomplete
                    id="parts-search"
                    key={this.state.autocompleteKey}
                    className="grid-search"
                    size="small"
                    noOptionsText="No parts have been found"
                    open={this.state.isOpen}
                    onOpen={() => this.onSearchOpened()}
                    onClose={() => this.onSearchClosed()}
                    onChange={(event, newValue) => this.onChange(event, newValue)}
                    onInputChange={(event, inputValue, reason) => this.onInputChange(event, inputValue, reason)}
                    inputValue={this.state.inputValue}
                    getOptionSelected={(option, value) => option.productid === value.productid}
                    getOptionLabel={(option) => option.CompositeName}
                    options={this.state.options}
                    loading={this.state.isLoading}
                    disabled={this.state.isDisabled || this.props.disabled}
                    clearOnBlur={false}
                    clearOnEscape={false}
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            label="Add a new part"
                            variant="outlined"
                            InputProps={{
                                ...params.InputProps,
                                endAdornment: (
                                    <React.Fragment>
                                        {this.state.isLoading ? <CircularProgress color="inherit" size={14} /> : null}
                                        {params.InputProps.endAdornment}
                                    </React.Fragment>
                                ),
                            }}
                        />
                    )}
                />
            </div>
        )
    }
}
