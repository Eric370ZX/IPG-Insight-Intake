function callJiraButton() {
    if (!window.top.IssueDialog) {
        var IssueDialog = function (options) {
            var $body = $("body", window.top.document);
            var that = this;
            var css = ".atlwdg-blanket{margin:auto;background:#000;height:100%;left:0;opacity:.5;position:fixed;top:0;width:100%;z-index:1000000;top:0;right:0;left:0;bottom:0}.atlwdg-popup{background:#fff;border:1px solid #ccc;position:fixed;margin:auto;z-index:10000011;border-radius:5px;background-clip:padding-box;overflow:hidden;width:810px;height:calc(100% - 100px);top:0;bottom:0;right:0;left:0;}#atlwdg-frame{height:100%;width:100%}.atlwdg-hidden{visibility:hidden}.atlwdg-loading{position:fixed;top:0;right:0;bottom:0;left:0;margin:auto;height:40px;width:40px;z-index:1000001;border-radius:5px;background-color:#fff;background-image:url(data:image/gif;base64,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);background-repeat:no-repeat;background-position:center center}";
            options.baseUrl = options.baseUrl || "https://eti-ipg.atlassian.net";
            this.options = options;
            this.frameUrl = options.baseUrl + "/rest/collectors/1.0/template/form/" + this.options.collectorId + "?os_authType=none";
            $("head", window.top.document).append("<style type='text/css'>" + css + "</style>");
            this.ShowDialog = () => $("#atlwdg-frame", window.top.document).length == 0 && that.show();
        };
        IssueDialog.prototype = {
            hideDialog: void 0,
            show: function () {
                if ($("#atlwdg-container", window.top.document).length == 0)
                    $("body", window.top.document).append('<div id="atlwdg-container" class="atlwdg-popup"></div>');
                var i = this,
                    t = $("#atlwdg-container", window.top.document),
                    o = $("body", window.top.document),
                    e = $('<iframe id="atlwdg-frame" frameborder="0" scrolling="no" src="' + this.frameUrl + '"></iframe>', window.top.document),
                    r = "Issue collector is loading",
                    A = $('<div id="atlwdg-loading" class="atlwdg-loading" title="' + r + '" tabindex="0"></div>', window.top.document);
                hideDialog = function (t) {
                    27 === t.keyCode && i.hide()
                }, A.appendTo(o), A.show(), A.focus(), o.css("overflow", "hidden").keydown(hideDialog), window.top.scroll(0, 0);

                var n = "";
                if (this.options.collectFeedback) {
                    var g = this.options.collectFeedback();
                    n = [];
                    for (var s in g) g.hasOwnProperty(s) && g[s] && "string" == typeof g[s] && n.push("*" + s + "*: " + g[s]);
                    n = n.join("\n")
                }
                var a = {};
                this.options.fieldValues && !$.isEmptyObject(this.options.fieldValues) && $.extend(a, this.options.fieldValues), e.on("load", function () {
                    var t = {
                        messageType: "collector.iframe.loaded",
                        feedbackString: n,
                        fieldValues: a
                    };
                    e[0].contentWindow.postMessage(JSON.stringify(t), i.options.baseUrl)
                }), t.show(), this.options.customHeight && setDialogHeight(this.options.customHeight), $(window.top).bind("message", function (o) {
                    if (o.originalEvent.source === e[0].contentWindow && o.originalEvent.data)
                        if ("cancelFeedbackDialog" === o.originalEvent.data) i.hide();
                        else if ("collectorLoaded" === o.originalEvent.data) A.remove(), t.removeClass("atlwdg-hidden");
                    else if (!i.options.customHeight) try {
                        var r = JSON.parse(o.originalEvent.data);
                        setDialogHeight(r.frameHeight)
                    } catch (i) {}
                });
                var d = window.top.document.createElement("a");
                d.href = "https://eti-ipg.atlassian.net";
                $("#atlwdg-blanket", window.top.document).show();
                t.append(e);
            },
            hide: function () {
                $("body", window.top.document).css("overflow", "auto").unbind("keydown", hideDialog), $("#atlwdg-container", window.top.document).empty(), $("#atlwdg-container", window.top.document).addClass("atlwdg-hidden").hide(), $("#atlwdg-blanket", window.top.document).hide()
            }
        };

        const createDialog = function (settings, collectorId) {
            const userData = {
                Location: window.top.location.href,
                Referrer: window.top.document.referrer,
                "User-Agent": navigator.userAgent,
                "Screen Resolution": screen.width + " x " + screen.height
            };

            const dialogoptions = {
                collectorId: collectorId,
                fieldValues: {},
                collectFeedback: function () {
                    return userData
                },
                triggerText: settings.triggerText,
                triggerPosition: settings.triggerPosition,
                triggerFunction: settings.triggerFunction,
                baseUrl: settings.baseUrl,
                customHeight: !1
            };

            window.top.IssueDialog = new IssueDialog(dialogoptions);
        }

        $.ajax({
            url: "https://eti-ipg.atlassian.net/rest/collectors/1.0/configuration/trigger/c0ba685a?os_authType=none",
            dataType: "jsonp",
            crossDomain: !0,
            jsonpCallback: "trigger_c0ba685a",
            cache: !0,
            success: function (settings) {
                createDialog(settings, "c0ba685a");
                window.top.IssueDialog.ShowDialog();
            }
        })
    } else {
        window.top.IssueDialog.ShowDialog();
    }
}
