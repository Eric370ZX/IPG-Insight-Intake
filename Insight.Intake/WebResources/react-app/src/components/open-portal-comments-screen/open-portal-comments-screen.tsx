import React from "react";
import { Text, 
    DetailsList, 
    IColumn, 
    initializeIcons, 
    IGroup, 
    CommandBar, 
    Selection, 
    CheckboxVisibility, 
    IRenderFunction, 
    IDetailsRowProps,
    ScrollablePane,
    ScrollbarVisibility,
    Sticky,
    DetailsListLayoutMode,
    StickyPositionType
} from '@fluentui/react';
import RecordREf from '../common/RecordRef';
import {IObjectWithKey} from '@fluentui/utilities/src/selection/Selection.types';
import { Spinner, SpinnerSize } from '@fluentui/react/lib/Spinner';

import { CaseStatus } from "../../enums/case-statuses";
import { PCMessageStatus } from "../../enums/portal-comment-status-reason";
import { PortalCommentService } from "../../services/portal-comment-service";
import { IPortalComment } from "../../models/IPortalComment";
import { INotification } from "../../models/INotification";
import { relative } from "path/posix";

interface State {
    portalcomments: IPortalComment[];
    groups:IGroup[];
    isProgressBarEnabled: boolean,
    notification?: INotification,
    selection:Selection
}

export class OpenPortalCommentScreen extends React.Component<{}, State> {
    private readonly _portalCommentService: PortalCommentService;
    private readonly Xrm = window.parent.Xrm;
    private _columns = 
    [
        { key: 'IDcolumn', name: 'Referral or Case ID', minWidth: 100, maxWidth: 200, isResizable: true,
        onRender: (item: IPortalComment, index?: number, column?: IColumn) => <RecordREf LookUp={item.RegardingId} />},
        { key: 'Facilitycolumn', name: 'Facility Name', fieldName: 'FacillityIdName', minWidth: 100, maxWidth: 200, isResizable: true,
        onRender: (item: IPortalComment, index?: number, column?: IColumn) => <RecordREf LookUp={item.Facillity} /> },
        { key: 'Datecolumn', name: 'Date Created', fieldName: 'createdon', minWidth: 100, maxWidth: 200, isResizable: true,
        onRender: (item: IPortalComment, index?: number, column?: IColumn) =>  item.createdon?.toLocaleDateString() ?? "" },
        { key: 'Messagecolumn', name: 'Message Description', fieldName: 'description', minWidth: 100, maxWidth: 200, isResizable: true,
        onRender: (item: IPortalComment, index?: number, column?: IColumn) => item.description ?? "" },
        { key: 'Statuscolumn', name: 'Message Status', fieldName: 'messagestatus', minWidth: 100, maxWidth: 200, isResizable: true,
        onRender: (item: IPortalComment, index?: number, column?: IColumn) =>  item.statuscode ? PCMessageStatus[item.statuscode] : "" },
        { key: 'CaseStatuscolumn', name: 'Case Status', fieldName: 'casestatus', minWidth: 100, maxWidth: 200, isResizable: true,
        onRender: (item: IPortalComment, index?: number, column?: IColumn) =>  item.casestatus ? CaseStatus[item.casestatus] : "" },
    ];
    private _displayedButtons = [
        { key: 'markUsRead', text: 'Mark as Read', onClick: () => {this.onMarkAsread()}, iconProps: { iconName: 'read' }},
        { key: 'refresh', text: '', onClick: () => {this.refreshData()}, iconProps: { iconName: 'refresh' }}];

    state: State = {
        selection: new Selection({onSelectionChanged: () => this.setState({selection:this.state.selection})}),
        portalcomments:[],
        groups:[],
        isProgressBarEnabled: false
    };

    public constructor(props: any) {
        super(props);
        this._portalCommentService = new PortalCommentService();
    }
    

    refreshData = async () => { 
        this.enableLoader();
        
        let result = (await this._portalCommentService.getOpenPortalComments())?.value as IPortalComment[] ?? [];
        
        result =  result.filter(r => (r.RegardingId?.name ?? "") != "" && !isNaN(Number(r.RegardingId?.name)));
        result.sort((a, b) => 
        {
            const anumb = Number(a.RegardingId?.name);
            const bnumb = Number(b.RegardingId?.name);

            return anumb < bnumb ? -1 : anumb > bnumb ? 1 : 0;
        });

        this.setState({
            portalcomments:result,
            isProgressBarEnabled: false,
            groups:this.groupsGenerator(result)
        });

    }

    async componentDidMount() {
        this.refreshData();
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

    groupsGenerator = (itemsList:IPortalComment[]):IGroup[] => {
        const groupObjArr:IGroup[] = [];
    
        const groupNames = new Set(itemsList.map(item => item.RegardingId?.name ?? "")) ;

        groupNames.forEach(gn => {
            const groupLength = itemsList.filter(item => (item.RegardingId?.name ?? "") === gn).length
            const groupIndex = itemsList.map(item => item.RegardingId?.name ?? "").indexOf(gn);
            groupObjArr.push({
                key: gn, name: gn, level: 0, count: groupLength, startIndex: groupIndex, isCollapsed: true
            })
        });
    
        return groupObjArr;
    }
    enableLoader = () =>  this.setState((prevState, props) => {return {isProgressBarEnabled: true, portalcomments:[], groups: []}});
    onMarkAsread =  async() =>
    {
        const ids:string[] = this.state.selection.getSelection()?.map((r:IObjectWithKey) => (r as IPortalComment).id);

        if(ids.length > 0)
        {
            this.setState({portalcomments:[], isProgressBarEnabled: true});
            await this._portalCommentService.markCommentAsRead(ids);
            this.refreshData();
        }
    }

    onRenderRow:IRenderFunction<IDetailsRowProps> = (props?: IDetailsRowProps
        , defaultRender?: (props?: IDetailsRowProps) => JSX.Element | null): JSX.Element | null => {
        const id = props?.item?.id;
        return id ? <div 
                    onDoubleClick={()=>this.Xrm.Navigation.navigateTo({pageType: "entityrecord", entityName:"adx_portalcomment", entityId: id})}>
                    {defaultRender && defaultRender(props)}
                    </div> 
                    : defaultRender && defaultRender(props) || null;}
    render() {
        initializeIcons();
        return (
                <div>
                    <ScrollablePane scrollbarVisibility={ScrollbarVisibility.auto}>
                    <Sticky>
                    <CommandBar
                    items={[{key:'name', commandBarButtonAs: ()=> 
                    <Text variant={"xLarge"}>Open Portal Comments</Text>}]} 
                    farItems={this.state.selection.getSelectedCount() > 0 ? this._displayedButtons : this._displayedButtons.filter(b=>b.key != 'markUsRead')}/>
                    </Sticky> 
                    <DetailsList
                    onRenderDetailsHeader={(headerProps, defaultRender) => {
                        return (
                          <Sticky
                            stickyPosition={StickyPositionType.Header}
                            isScrollSynced={true}
                            stickyBackgroundColor="transparent"
                          >
                            <div>{defaultRender && defaultRender(headerProps)}</div>
                          </Sticky>
                        );
                      }}
                    layoutMode={DetailsListLayoutMode.fixedColumns}
                    checkboxVisibility ={CheckboxVisibility.always}
                    items={this.state.portalcomments}
                    groups={this.state.groups}
                    selection={this.state.selection}
                    columns = {this._columns}
                    onRenderRow = {this.onRenderRow}
                    ></DetailsList>
                    {this.state.isProgressBarEnabled && <Spinner size={SpinnerSize.large}  />}
                    </ScrollablePane>
                    </div>
        );
    }
}
