import React from "react";

import { LinearProgress } from "@material-ui/core";

import './actual-parts-grid-screen.css';

import { ActualPartsGrid } from "../actual-parts-grid/actual-parts-grid";

import { CasePartDetailService } from "../../services/case-part-detail-service";
import { CaseService } from "../../services/case-service";
import { WebresourcePropertiesService } from "../../services/webresource-properties-service";
import { IActualPart } from "../../models/IActualPart";
import { GridView } from "../../enums/grid-views";
import { ActualPartsCardsGridViews } from "../actual-parts-cards-grid-views/actual-parts-cards-grid-views";
import { ICase } from "../../models/ICase";

interface State {
    actualParts: IActualPart[],
    isSubgridReady: boolean,
    caseId: string,
    case?: ICase | null
}

export default class ActualPartsGridScreen extends React.Component<{}, State> {
    state: State = {
        actualParts: [],
        isSubgridReady: false,
        caseId: "",
        case: null
    };

    onChangeView = async (view: GridView) => {
        const casePartDetailsService = new CasePartDetailService();

        this.setState({
            actualParts: [],
            isSubgridReady: false
        });

        const parts = await casePartDetailsService.getPartsByView(view, this.state.caseId);
        this.setState({
            actualParts: parts,
            isSubgridReady: true
        });
    }

    async componentDidMount() {
        const caseService = new CaseService();
        const casePartDetailsService = new CasePartDetailService();
        const webresourceProps = new WebresourcePropertiesService().getParameters();

        const caseObj = await caseService.getCaseData(webresourceProps.CaseId);
        this.setState({ case: caseObj, caseId: webresourceProps.CaseId });

        const casePartDetails = await casePartDetailsService.getActiveCaseParts(webresourceProps.CaseId);
        this.setState({ actualParts: casePartDetails, isSubgridReady: true });
    }

    render() {
        return (
            <div className="actual-parts-grid-container">
                {<ActualPartsCardsGridViews onChangeView={this.onChangeView}></ActualPartsCardsGridViews>}
                {
                    this.state.isSubgridReady ?
                        <ActualPartsGrid actualParts={this.state.actualParts}></ActualPartsGrid> :
                        <LinearProgress />
                }
            </div>
        );
    }
}