import React from "react";

import { Button, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';

import './advanced-search-form.css';
import { ISearchValues } from "../../../models/ISearchValues";
import { MetadataService } from "../../../services/metadata-service";

interface State {
    isControlsDisabled: boolean;
    searchValues: ISearchValues;
    categories: Xrm.Metadata.OptionMetadata[];
    keywords: Xrm.Metadata.OptionMetadata[]
}

interface Props {
    onSubmitSearch: Function;
    isDisabled: boolean;
}

export class AdvancedSearchForm extends React.Component<Props, State> {
    private readonly _textFieldHeight: number = 32;
    private readonly _textFieldInputProps = { style: { height: this._textFieldHeight, padding: '0 14px' } };
    private readonly _textFieldStyle = { height: this._textFieldHeight };
    private readonly _menuProps = { PaperProps: { style: { maxHeight: "300px" } } };

    state: State = {
        isControlsDisabled: false,
        searchValues: {
            category: null,
            hcpcs: "",
            keyword: null,
            manufacturerName: "",
            partDescription: "",
            partNumber: ""
        },
        categories: [],
        keywords: []
    };

    public constructor(props: Props) {
        super(props);
    }

    async componentDidMount() {
        const metadataService = new MetadataService();

        let categories = await metadataService.getOptionSetValues("product", "producttypecode");
        let keywords = await metadataService.getOptionSetValues("product", "ipg_ipgpartnumber");

        this.setState({
            categories,
            keywords
        });
    }

    private _onSearchSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        event.stopPropagation();

        this.setState({
            isControlsDisabled: true
        });

        await this.props.onSubmitSearch(this.state.searchValues);

        this.setState({
            isControlsDisabled: false
        });
    }

    private _handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement | any>) => {
        let searchValues = this.state.searchValues;
        searchValues[event.target.name] = event.target.value;

        this.setState({
            searchValues
        });
    }

    render() {
        return (
            <div className="advanced-search-form">
                <form onSubmit={this._onSearchSubmit}>
                    <div className="advanced-search-inputs">
                        <div className="search-input-col-container">
                            <div className="search-input-container">
                                <InputLabel className="search-input-label col1-label" htmlFor="partNumber">Part #</InputLabel>
                                <TextField
                                    size="small" variant="outlined"
                                    style={this._textFieldStyle} inputProps={this._textFieldInputProps}
                                    className="search-input"
                                    name="partNumber"
                                    onChange={this._handleInputChange}
                                    disabled={this.state.isControlsDisabled || this.props.isDisabled} />
                            </div>
                            <div className="search-input-container">
                                <InputLabel className="search-input-label col1-label" htmlFor="hcpcs">HCPCS</InputLabel>
                                <TextField
                                    size="small" variant="outlined"
                                    style={this._textFieldStyle} inputProps={this._textFieldInputProps}
                                    className="search-input"
                                    name="hcpcs"
                                    onChange={this._handleInputChange}
                                    disabled={this.state.isControlsDisabled || this.props.isDisabled} />
                            </div>
                        </div>
                        <div className="search-input-col-container">
                            <div className="search-input-container">
                                <InputLabel className="search-input-label col2-label" htmlFor="partDescription">Part Description</InputLabel>
                                <TextField
                                    size="small" variant="outlined"
                                    style={this._textFieldStyle} inputProps={this._textFieldInputProps}
                                    className="search-input"
                                    name="partDescription"
                                    onChange={this._handleInputChange}
                                    disabled={this.state.isControlsDisabled || this.props.isDisabled} />
                            </div>
                            <div className="search-input-container">
                                <InputLabel className="search-input-label col2-label" htmlFor="category">Category</InputLabel>
                                <Select
                                    style={this._textFieldStyle} inputProps={this._textFieldInputProps}
                                    name="category"
                                    className="search-input"
                                    onChange={this._handleInputChange}
                                    MenuProps={this._menuProps}
                                    disabled={this.state.isControlsDisabled || this.props.isDisabled}
                                >
                                    <MenuItem value=""><em>None</em></MenuItem>
                                    {this.state.categories.length > 0 && this.state.categories.map(cat => (
                                        <MenuItem key={cat.Value} value={cat.Value}>{cat.Label?.UserLocalizedLabel?.Label}</MenuItem>
                                    ))}
                                </Select>
                            </div>
                        </div>
                        <div className="search-input-col-container">
                            <div className="search-input-container">
                                <InputLabel className="search-input-label col3-label" htmlFor="manufacturerName">Manufacturer Name</InputLabel>
                                <TextField
                                    size="small" variant="outlined"
                                    style={this._textFieldStyle} inputProps={this._textFieldInputProps}
                                    className="search-input"
                                    name="manufacturerName"
                                    onChange={this._handleInputChange}
                                    disabled={this.state.isControlsDisabled || this.props.isDisabled} />
                            </div>
                            <div className="search-input-container">
                                <InputLabel className="search-input-label col3-label" htmlFor="keyword">Keyword</InputLabel>
                                <Select
                                    name="keyword"
                                    className="search-input"
                                    onChange={this._handleInputChange}
                                    MenuProps={this._menuProps}
                                    disabled={this.state.isControlsDisabled || this.props.isDisabled}
                                >
                                    <MenuItem value=""><em>None</em></MenuItem>
                                    {this.state.keywords.length > 0 && this.state.keywords.map(keyword => (
                                        <MenuItem key={keyword.Value} value={keyword.Value}>{keyword.Label?.UserLocalizedLabel?.Label}</MenuItem>
                                    ))}
                                </Select>
                            </div>
                        </div>
                    </div>
                    <Button
                        className="search-submit" variant="contained"
                        color="primary" type="submit"
                        disabled={this.state.isControlsDisabled || this.props.isDisabled}
                    >
                        Search</Button>
                </form>
            </div >
        );
    }
}