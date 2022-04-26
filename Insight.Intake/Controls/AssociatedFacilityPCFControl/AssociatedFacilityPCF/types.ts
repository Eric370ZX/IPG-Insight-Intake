import {IComboBoxOption, IComboBox} from "office-ui-fabric-react";

export interface IMultiselectOptionSetProps
{
    options: IComboBoxOption[],
    selectedKeys: string[],
    onSelectedChanged: (event: React.FormEvent<IComboBox>, option?: IComboBoxOption, index?: number, value?: string) => void;


}