import React from 'react';
import {Link} from '@fluentui/react';
import { ILookup } from "../../models/ILookup";

const Xrm = window.parent.Xrm;
const RecordRef = (props:{LookUp?:ILookup}) =>
{
    if(props.LookUp != null)
    {
        return <Link
        onClick={() => Xrm.Navigation.navigateTo({pageType: "entityrecord", entityName:props.LookUp!.entityName, entityId: props.LookUp!.id  })}>{props.LookUp.name}</Link>
    }
    else
    {
        return <span></span>;
    }
}

export default RecordRef;