import {IInputs, IOutputs} from "./generated/ManifestTypes";

'use strict';

const DefaultImageFileName: string = "default.png";
const UploadedFileImageFileName: string = "uploadedFile.png";
// Show Error css classname
const ShowErrorClassName = "ShowError";
// No Image css classname
const NoImageClassName = "NoImage";
// 'RemoveButton' css class name
const RemoveButtonClassName = "RemoveButton";

export class DocumentUploadComponent implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    // Value of the field is stored and used inside the control 
    private _value: string | null;

    private _fileName: string;
    private _documentName: string;
    private _mimeType: string;
    private _accept: string;
    // PowerApps component framework framework context, "Input Properties" containing the parameters, control metadata and interface functions.
    private _context: ComponentFramework.Context<IInputs>;
    // PowerApps component framework framework delegate which will be assigned to this object which would be called whenever an update happens. 
    private _notifyOutputChanged: () => void;
    // Control's container
    private controlContainer: HTMLDivElement;
    // button element created as part of this control
    private uploadButton: HTMLButtonElement;
    // button element created as part of this control
    private removeButton: HTMLButtonElement;
    // label element created as part of this control
    private imgElement: HTMLImageElement;
    // label element created as part of this control
    private errorLabelElement: HTMLLabelElement;
	/**
	 * Empty constructor.
	 */
	constructor()
	{

	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='starndard', it will receive an empty div element within which it can render its content.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container:HTMLDivElement)
	{
        this._context = context;
        this._notifyOutputChanged = notifyOutputChanged;
        this.controlContainer = document.createElement("div");
        //Create an upload button to upload the image
        this.uploadButton = document.createElement("button");
        // Get the localized string from localized string 
        this.uploadButton.innerHTML = context.resources.getString("PCF_ImageUploadControl_Upload_ButtonLabel");
        this.uploadButton.addEventListener("click", this.onUploadButtonClick.bind(this));
        // Creating the label for the control and setting the relevant values.
        this.imgElement = document.createElement("img");
        //Create a remove button to reset the image
        this.removeButton = document.createElement("button");
        this.removeButton.classList.add(RemoveButtonClassName);
        // Get the localized string from localized string 
        this.removeButton.innerHTML = context.resources.getString("PCF_ImageUploadControl_Remove_ButtonLabel");
        this.removeButton.addEventListener("click", this.onRemoveButtonClick.bind(this));
        // Create an error label element
        this.errorLabelElement = document.createElement("label");
        // If there is a raw value bound means there already have an image
        if (this._context.parameters.value.raw) {
            this.imgElement.src = context.parameters.value.raw;
        }
        else {
            this.setDefaultImage();
        }
        if (this._context.parameters.accept && this._context.parameters.accept.raw) {
          this._accept = this._context.parameters.accept.raw;
        }
        // Adding the label and button created to the container DIV.
        this.controlContainer.appendChild(this.uploadButton);
        this.controlContainer.appendChild(this.imgElement);
        this.controlContainer.appendChild(this.removeButton);
        this.controlContainer.appendChild(this.errorLabelElement);
        container.appendChild(this.controlContainer);
	}


	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void
	{
        // Always need to update the _context obj
        this._context = context;
    }

    /**
* Button Event handler for the button created as part of this control
* @param event
*/
    private onUploadButtonClick(event: Event): void {
        // context.device.pickFile(successCallback, errorCallback) is used to initiate the File Explorer
        // successCallback will be triggered if there successfully pick a file
        // errorCallback will be triggered if there is an error
      this._context.device.pickFile(
        { accept: this._accept, allowMultipleFiles: false, maximumAllowedFileSize: 0 })
        .then(this.processFile.bind(this), this.showError.bind(this));
    }
    /**
     * Button Event handler for the button created as part of this control
     * @param event
     */
    private onRemoveButtonClick(event: Event): void {
        this.setDefaultImage();
    }
    /**
     * 
     * @param files 
     */
    private processFile(files: ComponentFramework.FileObject[]): void {
        if (files.length > 0) {
            let file: ComponentFramework.FileObject = files[0];
            try {
                let fileExtension: string | undefined;
                let documentName: string | undefined;
                if (file && file.fileName) {
                    fileExtension = file.fileName.split('.').pop();
                    documentName = file.fileName.split('.').shift();
                }
              if (fileExtension && documentName) {
                if (this.validateFileFormat(fileExtension, file.mimeType)) {
                  this.setFile(true, fileExtension, file.fileContent, file.fileName, documentName, file.mimeType);
                  this.setUploadedFileImage();
                  this.controlContainer.classList.remove(NoImageClassName);
                }
                else {
                  this.setDefaultImage();
                  this.showError(this._context.resources.getString("PCF_ImageUploadControl_Wrong_File_Format"));
                }  
              }
              else {
                  this.showError();
              }
            }
            catch (err) {
                this.showError();
            }
        }
    }
    /**
     * Set Default Image
     */
    private setDefaultImage(): void {
        this._context.resources.getResource(DefaultImageFileName, this.setImage.bind(this, false, "png"), this.showError.bind(this));
        this.controlContainer.classList.add(NoImageClassName);
        // If it already has value, we need to update the output
        if (this._context.parameters.value.raw) {
            this._value = null;
            this._fileName = "";
            this._documentName = "";
            this._mimeType = "";
            this._notifyOutputChanged();
        }
    }
    /**
     * Set Uploaded File Image
     */
    private setUploadedFileImage(): void {
        this._context.resources.getResource(UploadedFileImageFileName, this.setImage.bind(this, false, "png"), this.showError.bind(this));       
    }
    /**
     * Set the Image content
     * @param shouldUpdateOutput indicate if needs to inform the infra of the change
     * @param fileType file extension name like "png", "gif", "jpg"
     * @param fileContent file content, base64 format
     */
    private setImage(shouldUpdateOutput: boolean, fileType: string, fileContent: string): void {
        let imageUrl: string = this.generateImageSrcUrl(fileType, fileContent);
        this.imgElement.src = imageUrl;
        if (shouldUpdateOutput) {
            this.controlContainer.classList.remove(ShowErrorClassName);
            this._value = imageUrl;
            this._notifyOutputChanged();
        }
    }

    /**
     * Set the Image content
     * @param shouldUpdateOutput indicate if needs to inform the infra of the change
     * @param fileType file extension name like "png", "gif", "jpg"
     * @param fileContent file content, base64 format
     */
    private setFile(shouldUpdateOutput: boolean, fileType: string, fileContent: string, fileName: string, documentName: string, mimeType: string): void {
        //let fileUrl: string = this.generateFileSrcUrl(fileType, fileContent);
        //this.imgElement.src = fileUrl;
        if (shouldUpdateOutput) {
            this.controlContainer.classList.remove(ShowErrorClassName);
            this._value = fileContent;
            this._fileName = fileName;
            this._documentName = documentName;
            this._mimeType = mimeType;
            this._notifyOutputChanged();
        }
    }
    /**
     * Generate Image Element src url
     * @param fileType file extension
     * @param fileContent file content, base 64 format
     */
    private generateImageSrcUrl(fileType: string, fileContent: string): string {
        return "data:image/" + fileType + ";base64, " + fileContent;
    }

    /**
     * Generate Image Element src url
     * @param fileType file extension
     * @param fileContent file content, base 64 format
     */
    private generateFileSrcUrl(fileType: string, fileContent: string): string {
        return "data:application/" + fileType + ";base64, " + fileContent;
    }
    /** 
     *  Show Error Message
     */
    private showError(errorMessage: string|undefined = undefined): void {
      this.errorLabelElement.innerText = errorMessage || this._context.resources.getString("PCF_ImageUploadControl_Can_Not_Find_File");
        this.controlContainer.classList.add(ShowErrorClassName);
  }

  private validateFileFormat(fileExtension: string, fileMimeType: string): boolean {
    if (this._accept) {
      let acceptFormats: string[] = this._accept.split(',');
      for (let acceptFormat of acceptFormats) {
        if (acceptFormat.trim().startsWith('.')) {
          let acceptExtension: string = acceptFormat.trim().substring(1);
          if ((fileExtension || '').toLowerCase() === acceptExtension.toLowerCase()) {
            return true;
          }
        }
        else {
          if ((fileMimeType || '').toLowerCase() === acceptFormat.trim().toLowerCase()) {
            return true;
          }
        }
      }

      return false;
    }
    else {
      return true;
    }
  }

	/** 
	 * It is called by the framework prior to a control receiving new data. 
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs
	{
        // return outputs
        let result: IOutputs =
        {
            value: this._value!,
            fileName: this._fileName,
            documentName: this._documentName,
            mimeType: this._mimeType
        };
        return result;
	}

	/** 
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void
	{
		// Add code to cleanup control if necessary
	}
}