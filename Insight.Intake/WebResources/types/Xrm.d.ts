declare namespace Xrm {
  interface XrmStatic {
    Page: Page;
  }
  interface ExecuteResponse {
    json(): any;
    json<T>(): T;
  }
  namespace Controls {
    interface Grid {
      addOnRecordSelect(callback: (...args: any[]) => void): void;
    }
    interface QuickFormControl {
      ui: {
        controls: Collection.ItemCollection<Control>
      };
    }
    interface Control {
        refreshRibbon() : void;
        getGrid(): any;
      getViewSelector(): any;     
        removePreSearch(filterFunction: () => void): any;
        addOption(item: any): any;
        clearOptions(): void;
        removeOption(item: number): any;
        getOptions(): any;
        removeOption(item: number): any;
      addCustomFilter(filterXml: string, entityName: string): any;      
      addPreSearch(arg0: () => void): any;
      addPreSearch(arg0: (eventContext: Xrm.Events.EventContext) => void): any;
      setVisible(isVisible: boolean): void;
      setDisabled(isDisable: boolean): void;
      refresh(): void;
      clearNotification(uniqueId: string): void;
      addNotification(notification: object): void;
      setNotification(notification: string, uniqueId: string): void;
    }
    interface ViewSelector {
      setCurrentView(viewSelectorItem: any): void;
    }
  }
  interface Ui {
    refreshRibbon(refreshAll: boolean): void;
  }
  interface JQuery {
    datepicker(): any;
  }
  namespace Page {
    interface GridControl {
      setFilterXml(filterXml: string): void;
      getGrid(): Xrm.Controls.Grid
    }
  }

  namespace Attributes {
    interface Attribute {
      getOptions(): any;
    }
  }

  namespace Collection {
    interface ItemCollection<T> {
      getByIndex(index: number): T;
    }
  }

  interface Page {
    OnPercentChange: any;
    OnAmountChange: any;
  }
}

declare var Microsoft: any;
