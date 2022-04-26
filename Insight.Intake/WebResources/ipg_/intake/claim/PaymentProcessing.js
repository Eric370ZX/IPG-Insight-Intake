/**
 * @namespace Intake
 */
var Intake;
(function (Intake) {
    var Claim;
    (function (Claim) {
        if (typeof ($) === 'undefined') {
            $ = window.parent.$;
        }
        var data = window.location.search.split('=');
        var params = decodeURIComponent(data[1]);
        var keyValue = params.split('=');
        var parameters = JSON.parse(keyValue[1]);
        /**
        * Fill Payment
        * @function Intake.Claim.FillPayment
        * @returns {void}
        */
        function FillPayment() {
            if (parameters.claimResponseHeaderId)
                FillAutomaticPayment(parameters.claimResponseHeaderId);
            else if (parameters.incidentId)
                FillManualPayment(parameters.incidentId);
            SetRestrictions();
        }
        Claim.FillPayment = FillPayment;
        /**
        * Set resctrictions for form values;
        * @function Intake.Claim.SetRestrictions
        * @returns {void}
        */
        function SetRestrictions() {
            setInputFilter(document.getElementById("memberpaid"), function (value) {
                return /^-?\d*[.,]?\d*$/.test(value);
            });
            $(".ip").each(function () {
                setInputFilter($(this)[0], function (value) {
                    return /^-?\d*[.,]?\d*$/.test(value);
                });
            });
            $(".allowed").each(function () {
                setInputFilter($(this)[0], function (value) {
                    return /^-?\d*[.,]?\d*$/.test(value);
                });
            });
        }
        /**
        * Set input filter for form values;
        * @function Intake.Claim.setInputFilter
        * @returns { void}
        */
        function setInputFilter(textbox, inputFilter) {
            ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
                textbox.addEventListener(event, function () {
                    if (inputFilter(this.value)) {
                        this.oldValue = this.value;
                        this.oldSelectionStart = this.selectionStart;
                        this.oldSelectionEnd = this.selectionEnd;
                    }
                    else if (this.hasOwnProperty("oldValue")) {
                        this.value = this.oldValue;
                        this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
                    }
                });
            });
        }
        /**
        * Fill Automatic Payment
        * @function Intake.Claim.FillAutomaticPayment
        * @returns {void}
        */
        function FillAutomaticPayment(claimResponseHeaderId) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var claimResponseHeaderRecord = [];
            var url = baseUrl
                + "ipg_claimresponseheaderSet?"
                + "$select=ipg_CaseNumber,ipg_CorrectedCaseNumber,ipg_ClaimId,ipg_CaseId"
                + "&$filter=ipg_claimresponseheaderId eq (guid'" + claimResponseHeaderId + "')";
            GetRecords(url, claimResponseHeaderRecord);
            if (claimResponseHeaderRecord.length > 0) {
                var caseNumber = (claimResponseHeaderRecord[0].ipg_CorrectedCaseNumber) ? claimResponseHeaderRecord[0].ipg_CorrectedCaseNumber : claimResponseHeaderRecord[0].ipg_CaseNumber;
                var caseRecord = [];
                url = baseUrl
                    + "IncidentSet?"
                    + "$select=IncidentId,ipg_CarrierId,ipg_ActualCarrierResponsibility,ipg_ActualMemberResponsibility,ipg_PatientId,ipg_PatientFullName"
                    + "&$filter=Title eq ('" + caseNumber + "')";
                GetRecords(url, caseRecord);
                if (caseRecord.length > 0) {
                    caseid = caseRecord[0].IncidentId;
                    var globalContext = parent.Xrm.Utility.getGlobalContext();
                    var orgURL = globalContext.getClientUrl();
                    var caseURL = orgURL + "/main.aspx?appid=cddc249a-a191-e811-a95f-000d3a37043b&etn=incident&id=" + caseid + "&pagetype=entityrecord";
                    var patientURL = orgURL + "/main.aspx?appid=cddc249a-a191-e811-a95f-000d3a37043b&etn=contact&id=" + caseRecord[0].ipg_PatientId.Id + "&pagetype=entityrecord";
                    $('#case').html('<a href=' + caseURL + '>' + caseNumber + '</a>');
                    $('#patient').html('<a href=' + patientURL + '>' + caseRecord[0].ipg_PatientFullName + '</a>');
                    window["ipg_ActualCarrierResponsibility"] = (caseRecord[0].ipg_ActualCarrierResponsibility.Value == null) ? '0.00' : caseRecord[0].ipg_ActualCarrierResponsibility.Value;
                    window["ipg_ActualMemberResponsibility"] = (caseRecord[0].ipg_ActualMemberResponsibility.Value == null) ? '0.00' : caseRecord[0].ipg_ActualMemberResponsibility.Value;
                    if (caseRecord[0].ipg_CarrierId.Id) {
                        var carrierRecord = [];
                        url = baseUrl
                            + "AccountSet?"
                            + "$select=ipg_contract"
                            + "&$filter=AccountId eq (guid'" + caseRecord[0].ipg_CarrierId.Id + "')";
                        GetRecords(url, carrierRecord);
                        var priceBy = '';
                        if (carrierRecord.length > 0)
                            window["priceBy"] = carrierRecord[0].ipg_contract ? 'CONTRACT' : 'NO CONTRACT';
                    }
                    else
                        window["priceBy"] = '';
                    window["denStr"] = GetDenialString(baseUrl);
                }
                var claimRecord = [];
                url = baseUrl
                    + "InvoiceSet?"
                    + "$select=Name,InvoiceId"
                    + "&$filter=ipg_caseid/Id eq (guid'" + claimResponseHeaderRecord[0].ipg_CaseId.Id + "') and (ipg_isedisubmitted eq true or ipg_ismanuallysubmitted eq true)";
                GetRecords(url, claimRecord);
                var claims_1 = '';
                claimRecord.forEach(function (claim) {
                    claims_1 += "<option label='" + claim.Name + "' value='" + claim.InvoiceId + "'>" + claim.Name + "</option> ";
                });
                claims_1 += "<option label='' value = ''></option>";
                $('#claim_id').html(claims_1);
                BuildLineItems(claimResponseHeaderRecord[0].ipg_ClaimId.Id);
            }
        }
        /**
        * Fill Manual Payment
        * @function Intake.Claim.FillManualPayment
        * @returns {void}
        */
        function FillManualPayment(incidentId) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var caseRecord = [];
            caseid = incidentId;
            var url = baseUrl
                + "IncidentSet?"
                + "$select=ipg_CarrierId,ipg_ActualCarrierResponsibility,ipg_ActualMemberResponsibility,ipg_PatientId,ipg_PatientFullName,Title"
                + "&$filter=IncidentId eq (guid'" + incidentId + "')";
            GetRecords(url, caseRecord);
            if (caseRecord.length > 0) {
                var globalContext = parent.Xrm.Utility.getGlobalContext();
                var orgURL = globalContext.getClientUrl();
                var caseURL = orgURL + "/main.aspx?appid=cddc249a-a191-e811-a95f-000d3a37043b&etn=incident&id=" + incidentId + "&pagetype=entityrecord";
                var patientURL = orgURL + "/main.aspx?appid=cddc249a-a191-e811-a95f-000d3a37043b&etn=contact&id=" + caseRecord[0].ipg_PatientId.Id + "&pagetype=entityrecord";
                $('#case').html('<a href=' + caseURL + '>' + caseRecord[0].Title + '</a>');
                $('#patient').html('<a href=' + patientURL + '>' + caseRecord[0].ipg_PatientFullName + '</a>');
                //$("#paymentdate").datepicker().datepicker("setDate", new Date());
                window["ipg_ActualCarrierResponsibility"] = (caseRecord[0].ipg_ActualCarrierResponsibility.Value == null) ? '0.00' : caseRecord[0].ipg_ActualCarrierResponsibility.Value;
                window["ipg_ActualMemberResponsibility"] = (caseRecord[0].ipg_ActualMemberResponsibility.Value == null) ? '0.00' : caseRecord[0].ipg_ActualMemberResponsibility.Value;
                if (caseRecord[0].ipg_CarrierId.Id) {
                    var carrierRecord = [];
                    url = baseUrl
                        + "AccountSet?"
                        + "$select=ipg_contract"
                        + "&$filter=AccountId eq (guid'" + caseRecord[0].ipg_CarrierId.Id + "')";
                    GetRecords(url, carrierRecord);
                    var priceBy = '';
                    if (carrierRecord.length > 0)
                        window["priceBy"] = carrierRecord[0].ipg_contract ? 'CONTRACT' : 'NO CONTRACT';
                }
                else
                    window["priceBy"] = '';
                window["denStr"] = GetDenialString(baseUrl);
            }
            var claimRecord = [];
            url = baseUrl
                + "InvoiceSet?"
                + "$select=Name,InvoiceId"
                + "&$filter=ipg_caseid/Id eq (guid'" + incidentId + "') and (ipg_isedisubmitted eq true or ipg_ismanuallysubmitted eq true)";
            GetRecords(url, claimRecord);
            var claims = '';
            var firstClaimId = null;
            claimRecord.forEach(function (claim) {
                if (!firstClaimId)
                    firstClaimId = claim.InvoiceId;
                claims += "<option label='" + claim.Name + "' value='" + claim.InvoiceId + "'>" + claim.Name + "</option> ";
            });
            claims += "<option label='' value = ''></option>";
            $('#claim_id').html(claims);
            BuildLineItems(firstClaimId);
        }
        /**
        * Fill line items
        * @function Intake.Claim.BuildLineItems
        * @returns {void}
        */
        function BuildLineItems(claimId) {
            $("#claimlineitemts").empty();
            if (claimId) {
                var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
                var claimResponseLineRecord = [];
                var url = baseUrl
                    + "ipg_claimlineitemSet?"
                    + "$filter=ipg_claimid/Id eq (guid'" + claimId + "') and ipg_quantity gt 0";
                GetRecords(url, claimResponseLineRecord);
                var i_1 = 1;
                claimResponseLineRecord.forEach(function (claimResponseLine) {
                    //let markup = "<tr><td width='15%'>" + HCPCS + "</td><td width='10%' style='text-align:center'>" + parseFloat(part.Quantity) + "</td><td width='45%'>" + part.ProductId.Name + "</td><td width='15%'>" + partNumber + "</td><td width='15%'  style='text-align:right'>$" + parseFloat(part.ExtendedAmount.Value).toLocaleString('en-us', { minimumFractionDigits: 2 }) + "</td></tr>";
                    var markup = "<tr><td>" + claimResponseLine.ipg_name + "</td>"
                        //+ "<td>" + Math.round(claimResponseLine.ipg_billedchg.Value / claimResponseLine.ipg_quantity * 100) / 100 + "</td>"
                        + "<td>" + Number(claimResponseLine.ipg_quantity) + "</td>"
                        + "<td>" + claimResponseLine.ipg_billedchg.Value + "</td>"
                        + "<td>" + Math.round((claimResponseLine.ipg_billedchg.Value - claimResponseLine.ipg_allowed.Value) * 100) / 100 + "</td>"
                        //+ "<td>" + window["priceBy"] + "</td>"
                        + "<td>" + claimResponseLine.ipg_unitprice.Value + "</td>"
                        + "<td>" + Math.round(claimResponseLine.ipg_unitprice.Value * claimResponseLine.ipg_quantity * 100) / 100 + "</td>"
                        + "<td>" + window["ipg_ActualCarrierResponsibility"] + "</td>"
                        + "<td>" + window["ipg_ActualMemberResponsibility"] + "</td>"
                        + "<td>&nbsp;<input type='text' class='allowed' name='allowed" + i_1 + "' id='allowed" + i_1 + "' value='" + ((claimResponseLine.ipg_allowed.Value == null) ? '0.00' : claimResponseLine.ipg_allowed.Value) + "' width='20' style='text-align: right;'></td>"
                        + "<td>&nbsp;<input type='text' class='ip' name = 'ip" + i_1 + "' id = 'ip" + i_1 + "' value = '0.00' width = '20' style='text-align: right;'></td>"
                        + "<td>&nbsp;<select class='denials' id='denials" + i_1 + "[]' name='denials" + i_1 + "[]' multiple='multiple'>" + window["denStr"] + "</select></td>"
                        + "</tr>";
                    $("#claimlineitemts").append(markup);
                    i_1++;
                });
            }
        }
        Claim.BuildLineItems = BuildLineItems;
        /**
        * type cast OData DateTime type to CRM DateTime
        * @function Intake.Incident.ToDateTime
        * @returns {Date}
        */
        function ToDateTime(dt) {
            dt = dt.replace("/Date(", "");
            dt = dt.replace(") /", "");
            return new Date(parseInt(dt, 10));
        }
        /**
        * type cast DateTime to string
        * @function Intake.Incident.ToDateTime
        * @returns {string}
        */
        function formatDate(date) {
            var d = new Date(date), month = '' + (d.getMonth() + 1), day = '' + d.getDate(), year = d.getFullYear();
            if (month.length < 2)
                month = '0' + month;
            if (day.length < 2)
                day = '0' + day;
            return [year, month, day].join('-');
        }
        Claim.formatDate = formatDate;
        /**
        * Gets records using oData
        * @function Intake.Incident.GetRecords
        * @returns {array}
        */
        function GetRecords(url, entities) {
            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: url,
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    if (data && data.d != null && data.d.results != null) {
                        AddRecordsToArray(data.d.results, entities);
                        FetchRecordsCallBack(data.d, entities);
                    }
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    alert("Error :  has occurred during retrieval of the records ");
                    console.log(XmlHttpRequest.responseText);
                }
            });
        }
        function AddRecordsToArray(records, entities) {
            for (var i = 0; i < records.length; i++) {
                entities.push(records[i]);
            }
        }
        function FetchRecordsCallBack(records, entities) {
            if (records.__next != null) {
                var url = records.__next;
                GetRecords(url, entities);
            }
        }
        function GetDenialString(baseUrl) {
            var denialCodeRecord = [];
            var url = baseUrl
                + "ipg_claimstatuscodeSet?"
                + "$select=ipg_claimstatuscodeId,ipg_name,"
                + "&$filter=ipg_isdenial eq true";
            GetRecords(url, denialCodeRecord);
            var denStr = '';
            denialCodeRecord.forEach(function (denialCode) {
                denStr += "<option value='" + denialCode.ipg_claimstatuscodeId.Id + "'>" + denialCode.ipg_name + "</option> ";
            });
            return denStr;
        }
        /**
        * Save Payment
        * @function Intake.Claim.SavePayment
        * @returns {void}
        */
        function SavePayment() {
            var ipg_totalinsurancepaid = 0;
            $(".ip").each(function () {
                var value = $(this).val();
                //ipg_totalinsurancepaid += parseFloat(value);
            });
            var data = {
                "ipg_CaseId@odata.bind": "/incidents(" + caseid.slice(1, -1) + ")",
                //"ipg_paymentdate": $("#paymentdate").datepicker("getDate").toISOString(),
                //"ipg_name": "test",
                "ipg_memberpaid_new": $("#memberpaid").val(),
                "ipg_totalinsurancepaid": ipg_totalinsurancepaid
            };
            Xrm.WebApi.createRecord("ipg_payment", data).then(function success(result) {
                //var paymentId = result.id;
            }, function (error) {
                console.log(error.message);
            });
        }
        Claim.SavePayment = SavePayment;
    })(Claim = Intake.Claim || (Intake.Claim = {}));
})(Intake || (Intake = {}));
