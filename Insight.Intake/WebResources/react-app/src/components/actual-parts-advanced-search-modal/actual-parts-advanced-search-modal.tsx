import React from "react";

import { Button } from '@material-ui/core';

import './actual-parts-advanced-search-modal.css';

import { AdvancedSearchForm } from "./advanced-search-form/advanced-search-form";
import { AdvancedSearchResults } from "./advanced-search-results/advanced-search-results";

import { IPart } from "../../models/IPart";
import { ISearchValues } from "../../models/ISearchValues";
import { PartService } from "../../services/parts-service";
import { IActualPart } from "../../models/IActualPart";
import {INotification} from "../../models/INotification";
import { ICase } from "../../models/ICase";
import _ from "lodash";

interface State {
    parts: IPart[];
    rowsPerPage: number;
    totalAmount: number;
    page: number;
    isLoading: boolean;

    selectedParts: {
        [key: string]: IPart;
    };

    searchValues: ISearchValues;
    categories: Xrm.Metadata.OptionMetadata[];
    keywords: Xrm.Metadata.OptionMetadata[];
}

interface Props {
    case: ICase;
    onClose: Function;
    addTemporaryParts: Function;
    setNotification:(value:INotification) => void;
    IsPartAlreadyExist(productid:string):Promise<boolean>;
}

export class ActualPartsAdvancedSearchModal extends React.Component<Props, State> {
    private readonly _partsService: PartService;

    private readonly _textFieldHeight: number = 32;

    state: State = {
        parts: [],
        page: 0,
        rowsPerPage: window.innerWidth < 1600 ? 5 : 10,
        totalAmount: 0,
        isLoading: false,
        selectedParts: {},
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

        this._partsService = new PartService();
    }

    private onSearchSubmit = async (searchValue: ISearchValues) => {
        this.setState({
            isLoading: true,
            page: 0
        });

        const partsCollection = await this._partsService.advancedSearch(searchValue, this.state.rowsPerPage, this.state.page + 1);

        this.setState({
            searchValues: searchValue,
            parts: partsCollection.value,
            isLoading: false,
            totalAmount: partsCollection["@odata.count"]
        });
    }

    private handleChangePage = async (newPage: number) => {
        this.setState({
            isLoading: true
        });

        const partsCollection = await this._partsService.advancedSearch(this.state.searchValues, this.state.rowsPerPage, newPage + 1);

        this.setState({
            parts: partsCollection.value,
            page: newPage,
            isLoading: false
        });
    }

    private addPartsButtonClick = async () => {
        this.setState({
            isLoading: true
        });

        try {
            let newParts: IActualPart[] = [];
            const products = Object.entries(this.state.selectedParts).map(([key, product])=> product);
            const dos = this.props.case.ipg_actualdos || this.props.case.ipg_surgerydate;


            for(let i = 0; i < products.length; i++)
            {
                if(await this.props.IsPartAlreadyExist(products[i].productid))
                {
                    throw new Error(`Part ${products[i].ipg_manufacturerpartnumber}, ${products[i].name} already exists.`);
                }
            }

            this._partsService.CheckIsPartsWithinEffictiveDate(dos, products);
  
          await this._partsService.CheckIsPartsBillable(dos, products, this.props.case._ipg_carrierid_value, this.props.case._ipg_homeplancarrierid_value);
           
            for (const part in this.state.selectedParts) 
            {
                const p = this.state.selectedParts[part];

                if (!await this.props.IsPartAlreadyExist(p.productid))
                {
                    newParts.push({
                        Part: p
                    } as IActualPart);
                }
                else
                {
                    throw new Error(`Part ${p.ipg_manufacturerpartnumber}, ${p.name} already exists.`);
                }
            }

            this.props.addTemporaryParts(newParts);
            this.props.onClose();
        }
        catch (exc) {
            this.props.setNotification({message:exc.message, sevirity:"error"});
        }

        this.setState({
            isLoading: false
        });
    }

    private closeButtonClick = () => {
        this.props.onClose();
    }

    private childRowCheckboxClicked = (part: IPart) => {
        let parts = this.state.selectedParts;

        if (parts[part.productid]) {
            this.setState({
                selectedParts: _.omit(parts, part.productid)
            });
        }
        else {
            parts[part.productid] = part;

            this.setState({
                selectedParts: parts
            });
        }
    }

    render() {
        return (
            <div className="actual-parts-advanced-search-modal">
                <div className="actual-parts-advanced-search-modal-header">
                    <h2>Advanced Search - Parts</h2>
                </div>

                <AdvancedSearchForm
                    onSubmitSearch={this.onSearchSubmit} 
                    isDisabled={this.state.isLoading}/>

                <AdvancedSearchResults
                    parts={this.state.parts}
                    totalAmount={this.state.totalAmount}
                    rowsPerPage={this.state.rowsPerPage}
                    page={this.state.page}
                    selectedPartIds={this.state.selectedParts}

                    onChangePage={this.handleChangePage}
                    checkboxClicked={this.childRowCheckboxClicked} />

                <div className="actual-parts-advanced-search-buttons">
                    <Button
                        id="modal-add-button"
                        className="button modal-add-button"
                        variant="outlined"
                        disabled={this.state.isLoading}
                        onClick={this.addPartsButtonClick}>
                        Add selected parts
                    </Button>
                    <Button
                        id="modal-close-button"
                        className="button modal-close-button"
                        variant="outlined"
                        disabled={this.state.isLoading}
                        onClick={this.closeButtonClick}>
                        Close
                    </Button>
                </div>
            </div >
        );
    }
}
