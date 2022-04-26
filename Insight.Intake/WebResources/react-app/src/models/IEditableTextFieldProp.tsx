export interface IEditableTextFieldProp {
    LogicalName: string;
    Label: string;
    Type?: EditableTextFieldPropType;
    isReadOnly?: Function;
    isNotModifiable? : boolean;
    filteredOptions?:Function;
}

export enum EditableTextFieldPropType {
    Text = 0,
    Number = 1,
    OptionSet = 2,
    Сurrency = 3
}