import { IWebresourceProps } from "../models/properties/IWebresourceProps";

export class WebresourcePropertiesService {
    getParameters(): IWebresourceProps {      
        const params = new URLSearchParams(window.location.search);
        const recordId = params.get("id");
        const isEstimatedParts = params.get("isEstimatedParts");

        var args = JSON.parse(params.get("data") || "{}");

        if (process.env.NODE_ENV === "development") {
            return {
                CaseId: recordId || "69154160-18ad-e811-a968-000d3a370e23",
                isEstimatedParts: isEstimatedParts || false
            } as IWebresourceProps;
        }   
        else {
            return {
                CaseId: recordId || args.caseId,
                isEstimatedParts: isEstimatedParts || args.isEstimatedParts || false
            } as IWebresourceProps;
        }
    }
}