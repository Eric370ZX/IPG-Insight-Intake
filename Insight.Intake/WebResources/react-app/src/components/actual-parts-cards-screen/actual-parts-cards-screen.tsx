import React from "react";

import { LinearProgress } from "@material-ui/core";

import './actual-parts-cards-screen.css';

import { ActualPartsCardsGrid } from "../actual-parts-cards-grid/actual-parts-cards-grid";
import { Header } from "../header/header";

import { BaseService } from "../../services/base-service";
import { CaseService } from "../../services/case-service";
import { TaskService } from "../../services/task-service";
import { CasePartDetailService } from "../../services/case-part-detail-service";
import { CaseEstimatedPartDetailService } from "../../services/case-estimated-part-service";

import { TemporaryService } from "../../services/temp-service";
import { WebresourcePropertiesService } from "../../services/webresource-properties-service";
import { ManufacturerService } from "../../services/manufacturer-service";
import { GridView } from "../../enums/grid-views";
import { ActualPartsCardsGridViews } from "../actual-parts-cards-grid-views/actual-parts-cards-grid-views";
import { ActualPartsCardsGridHeader } from "../actual-parts-cards-grid-header/actual-parts-cards-grid-header";
import { ICase } from "../../models/ICase";
import { POType } from "../../enums/po-types";
import { IGenericPart } from "../../models/IGenericPart";
import {INotification} from "../../models/INotification"
import Alert from '@material-ui/lab/Alert';
import { Snackbar } from '@material-ui/core';
import { PartService } from "../../services/parts-service";
import { IPart } from "../../models/IPart";

interface State {
    actualParts: IGenericPart[],
    dirtyParts: Map<string, string[]>,
    isSubgridReady: boolean,
    isProgressBarEnabled: boolean,
    caseId: string,
    case: any,
    isEstimatedParts?: boolean,
    currentView: GridView,
    unsaved?:boolean,
    notification?: INotification,
}

export class ActualPartsCardsScreen extends React.Component<{}, State> {
    private readonly _tempService: TemporaryService;
    private readonly _casePartService: BaseService;
    private readonly _productService: PartService;
    private readonly _taskService: TaskService;
    private readonly _manufacturerService: ManufacturerService;

    state: State = {
        actualParts: [],
        dirtyParts: new Map<string, string[]>(),
        isSubgridReady: false,
        isProgressBarEnabled: false,
        caseId: "",
        case: null,
        currentView: GridView.ActiveParts
    };

    public constructor(props: any) {
        super(props);

        const webresourceParams = new WebresourcePropertiesService().getParameters();

        this.state.caseId = webresourceParams.CaseId;
        this.state.isEstimatedParts = webresourceParams.isEstimatedParts;

        this._casePartService = this.state.isEstimatedParts ? new CaseEstimatedPartDetailService() : new CasePartDetailService();
        this._taskService = new TaskService();
        this._tempService = new TemporaryService(this.state.caseId,this._casePartService, this.setUnsavedParts);
        this._manufacturerService = new ManufacturerService();
        this._productService = new PartService();
        this.state.unsaved = this._tempService.getAll().length > 0;
    }


    setUnsavedParts = () =>
    {
        this.setState({unsaved:true});
    }
    onChangeView = async (view: GridView) => {
        this.setState({
            currentView: view
        });

        await this.refreshData(view);
    }

    refreshData = async (view: GridView | null) => { 
        const targetView = view || this.state.currentView;

        this.setState({
            actualParts: [],
            isSubgridReady: false
        });
        let parts = await (await this._casePartService.getPartsByView(targetView, this.state.caseId));
        
        const result = this._tempService.combineTempPartsAndActualParts(this._tempService.getAll(), parts);

        for (let p of result.actualParts)
        {
            if(!(p.ipg_casepartdetailid || p.ipg_estimatedcasepartdetailid) && p.Part?.productid)
            {
                p.Part = (await this._productService.retrieveById(p.Part?.productid)) as IPart;
                p.Part.Manufacturer = await this._manufacturerService.getById(p.Part?._ipg_manufacturerid_value);
            }
        }

        this.setState({
            actualParts: result.actualParts,
            dirtyParts: result.dirtyParts,
            isSubgridReady: true,
            unsaved: this._tempService.getAll().length > 0
        });
    }

    addTempoparyParts = async (actualParts: any[]) => {
        let currentActualParts = this.state.actualParts;
        
        if(this.state.isEstimatedParts)
        {
            actualParts.map(r => r.ipg_potypecode = POType.TPO);
        }
       
        for (const actualPart of actualParts) {
            //set default values on add new part
            actualPart.ipg_quantity = actualPart.ipg_quantity ?? 1;
            actualPart.ipg_potypecode = actualPart.ipg_potypecode ?? this._tempService.getDefaultPOType(actualPart, this.state.case);

            if (this._tempService.addNewPart(actualPart)) {
                currentActualParts.splice(0, 0, actualPart);
            }
        }

        this.setState({
            actualParts: currentActualParts,
            unsaved: true
        });
    }

    toggleProgressBar = (toggle: boolean) => {
        this.setState({
            isProgressBarEnabled: toggle
        });
    }

    async componentDidMount() {
        const caseService = new CaseService();

        const caseObj = await caseService.getCaseData(this.state.caseId);
        this.setState({ case: caseObj });

        this.refreshData(null);
    }

    handleNotificationClose = (event?: React.SyntheticEvent, reason?: string) => {
        if (reason === 'clickaway') {
            return;
        }

        this.setState({
            notification: undefined
        })
    };

    showNotification = (notification: INotification) =>
    {
        this.setState({notification:{...notification}});
    }

    render() {

        const today = new Date();
        const lastebvcheckDate = (this.state.case as ICase)?.ipg_lastebvcheckdatetime && new Date((this.state.case as ICase)?.ipg_lastebvcheckdatetime || '');
        const ebvAlert = this.state.case && (!lastebvcheckDate || new Date(lastebvcheckDate.getFullYear(), lastebvcheckDate.getMonth(), lastebvcheckDate.getDate() + 3) < new Date(today.getFullYear(), today.getMonth(), today.getDate()));
        
        return (
            <div className="actual-parts-container">
                <header>
                {this._tempService?.getAll().length > 0 ? <Alert variant="standard" severity="warning">You have unsaved parts on this page!</Alert> : ""}
                {ebvAlert ? <Alert variant="standard" severity="warning">Last EBV is &gt; 72 hours</Alert> : ""}
                { this.state.case && <Header Case={this.state.case}></Header>}

                <ActualPartsCardsGridViews
                 disabled={!this.state.isSubgridReady || this.state.isProgressBarEnabled || this.state.isEstimatedParts}
                 onChangeView={this.onChangeView} ></ActualPartsCardsGridViews>
                {
                    this.state.isSubgridReady &&
                    <ActualPartsCardsGridHeader
                        taskService = {this._taskService}
                        setNotification={this.showNotification}
                        case={this.state.case}
                        actualParts={this.state.actualParts}
                        refreshGrid={this.refreshData}
                        toggleProgressBar={this.toggleProgressBar}
                        addTemporaryParts={this.addTempoparyParts}
                        tempService={this._tempService}
                        disabled={this.state.isProgressBarEnabled}
                        searchparts= { this.state.isEstimatedParts ? new CaseEstimatedPartDetailService() : new CasePartDetailService()}
                        ></ActualPartsCardsGridHeader>
                }

                {
                    !this.state.isSubgridReady || this.state.isProgressBarEnabled ?
                        <LinearProgress className="grid-progress-bar" /> :
                        <br />
                }
                </header>

                {
                    this.state.isSubgridReady &&
                    <div className="actual-parts-rows">
                    <ActualPartsCardsGrid
                        setNotification={this.showNotification}
                        case={this.state.case}
                        actualParts={this.state.actualParts}
                        dirtyParts={this.state.dirtyParts}
                        toggleProgressBar={this.toggleProgressBar}
                        refreshGrid={this.refreshData} 
                        tempService={this._tempService}
                        disabled={this.state.isProgressBarEnabled}/>
                    </div>
                }

            {this.state.notification && <Snackbar  
            anchorOrigin={{vertical:"top", horizontal:"center"} } 
            key = {this.state.notification.message + this.state.notification.sevirity}
            open={this.state.notification != null} 
            autoHideDuration={this.state.notification?.autoHideDuration || 5000} 
            onClose={this.handleNotificationClose}>
                <Alert severity={this.state.notification?.sevirity} onClose={this.handleNotificationClose}>
                    {this.state.notification?.message}
                </Alert>
            </Snackbar>}
            </div>

        );
    }
}
