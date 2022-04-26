import {IInputs, IOutputs} from "./generated/ManifestTypes";
import {ISelectableOption} from "@fluentui/react/lib/SelectableOption";
import {IComboBox, IComboBoxOption} from "@fluentui/react/lib/ComboBox";
import {IMultiselectOptionSetProps} from "./types"
import MultiSelectOptionSetControl from "./MultiselectOptionSetControl";
import * as React from 'react';
import * as ReactDOM from 'react-dom';

export class AssociatedFacilityPCF implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	private facilities:ISelectableOption[];
	private selectedFacilities: string[];
	private container:HTMLDivElement;
	private notifyOutputChanged: () => void;

	constructor()
	{

	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public async init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container:HTMLDivElement)
	{
		this.facilities = await (await this.GetFacilities(context)).entities.map(r=>({key: r['accountid'], text:r["name"] }));;
		this.container = container;
		this.renderControl(context);
		this.notifyOutputChanged = notifyOutputChanged;
	}

	private async renderControl(context:ComponentFramework.Context<IInputs>)
	{
		const currentValue = context.parameters.primarycontrol.raw;
		this.selectedFacilities = currentValue?.split(",").map(id => id.trim()) || [];

		let multiSelectOptionSetProperties:IMultiselectOptionSetProps  = 
		{
			options: this.facilities,
			selectedKeys: this.selectedFacilities,
			onSelectedChanged: (event: React.FormEvent<IComboBox>, option?: IComboBoxOption, index?: number, value?: string) =>
			{ if(option)
				{
					this.selectedFacilities = option.selected ? [...this.selectedFacilities, option.key as string] : this.selectedFacilities.filter(key => key !== option.key);
				}
		
				this.notifyOutputChanged();
			}
		}

		ReactDOM.render(React.createElement(MultiSelectOptionSetControl, multiSelectOptionSetProperties),this.container);
	}

	private GetFacilities(context:ComponentFramework.Context<IInputs>):Promise<ComponentFramework.WebApi.RetrieveMultipleResponse>
	{ 
		return context.webAPI.retrieveMultipleRecords('account', `?fetchXml=<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false" >
			<entity name="account" >
			  <attribute name="name" />
			  <attribute name="accountid" />
			  <order attribute="name" descending="false" />
			  <filter type="and" >
				<condition attribute="customertypecode" operator="eq" value="923720000" />
				<condition attribute="statecode" operator="eq" value="0" />
				<condition attribute="ipg_isactive" operator="eq" value="1" />
				<condition attribute="name" operator="not-null" />
			  </filter>
			</entity>
		  </fetch>`);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void
	{
		this.renderControl(context);
	}

	/** 
	 * It is called by the framework prior to a control receiving new data. 
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs
	{
		return {
			primarycontrol: this.selectedFacilities == null ? undefined : this.selectedFacilities.join(",")
		};
	}

	/** 
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void
	{
		ReactDOM.unmountComponentAtNode(this.container);
	}
}