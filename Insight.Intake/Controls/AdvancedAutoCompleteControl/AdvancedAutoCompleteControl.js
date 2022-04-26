var __extends = this && this.__extends || function (d, b) {
    for (var p in b)
        if (b.hasOwnProperty(p)) d[p] = b[p];

    function __() {
        this.constructor = d
    }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __)
}, Ipg;

(function (Ipg) {
    var CustomControls;
    (function (CustomControls) {
        var Dictionary = function () {
            function Dictionary() {
                this._keys = [];
                this._values = [];
            }
            Dictionary.prototype.add = function (key, value) {
                var indexOfKey = this._keys.indexOf(key);
                if (indexOfKey >= 0) {
                    this._keys.splice(indexOfKey, 1);
                    this._values.splice(indexOfKey, 1);
                }
                this._keys.push(key);
                this._values.push(value);
            };
            Dictionary.prototype.has = function (key) {
                var indexOfKey = this._keys.indexOf(key);
                return indexOfKey >= 0;
            };
            Dictionary.prototype.get = function (key) {
                var indexOfKey = this._keys.indexOf(key);
                if (indexOfKey >= 0) {
                    return this._values[indexOfKey];
                }
                return null;
            };
            Dictionary.prototype.remove = function (key) {
                var indexOfKey = this._keys.indexOf(key);
                if (indexOfKey >= 0) {
                    this._keys.splice(indexOfKey, 1);
                    this._values.splice(indexOfKey, 1);
                } 
            };
            return Dictionary;
        }();
        var CCManager = function () {
            function CCManager() {
                this._dicitionary = new Dictionary();
            }
            CCManager.prototype.getControl = function (key) {
                if (this._dicitionary.has(key)) {
                    return this._dicitionary.get(key);
                }
                return null;
            };
            CCManager.prototype.addControl = function (key, control) {
                this._dicitionary.add(key,  control);   
            };
            CCManager.prototype.removeControl = function (key) {
                this._dicitionary.remove(key);  
            };
            return CCManager;
        }();
        CustomControls.CCManager = new CCManager();
    })(CustomControls = Ipg.CustomControls || (Ipg.CustomControls = {}));
})(Ipg || (Ipg = {}));

(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete) {
        var DataSetDataAdapter = function () {
            function DataSetDataAdapter(createPerformanceStopwatch) {
                this.createPerformanceStopwatch = createPerformanceStopwatch
            }

            DataSetDataAdapter.prototype.initialize = function (inputBag, htmlEncode) {
                var executeFetch = function (fetchXml, entityName) {
                    var encodedFetchXml = encodeURI(fetchXml);
                    var queryPath = "/api/data/v9.0/" + entityName + "s?fetchXml=" + encodedFetchXml;
                    var requestPath = Xrm.Page.context.getClientUrl() + queryPath;

                    var req = new XMLHttpRequest();
                    req.open("GET", requestPath, false);
                    req.setRequestHeader("Accept", "application/json");
                    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");

                    req.send();
                    return JSON.parse(req.responseText);
                };

                var _this = this;
                this.inputBag = inputBag;
                this.refreshedItemsCallbackTriggeredFlag = false;
                this.validateInputData();
                var distinct = inputBag.distinct && inputBag.distinct.raw && inputBag.distinct.raw.toLowerCase() == "yes";

                var entityName = inputBag.dynamicSource.getTargetEntityType();
                var displayValue = inputBag.dynamicSource.filtering.aliasMap.displayValue;
                if (inputBag.fetch && inputBag.fetch.raw) {
                    var response = executeFetch(inputBag.fetch.raw, entityName);
                    this.itemsCollection = response.value.map(function (x) {
                        return {
                            Id: x[entityName + "id"],
                            DisplayValue: x[displayValue],
                            OriginalValue: x[displayValue]
                        }
                    });
                }
                else {
                    this.itemsCollection = (new AdvancedAutoComplete.ItemsCollectionBuilder(htmlEncode)).build(this.inputBag.dynamicSource.sortedRecordIds, this.inputBag.dynamicSource.records, distinct);
                }

                if (this.refreshedItemsCallback != null) {
                    this.refreshedItemsCallbackTriggeredFlag = true;
                    this.refreshedItemsCallback(this.itemsCollection);
                    this.refreshedItemsCallback = null;
                    !MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.stopwatch) && this.stopwatch.stop()
                }
            };
            Object.defineProperty(DataSetDataAdapter.prototype, "refreshItemsCallbackTriggered", {
                "get": function () {
                    return this.refreshedItemsCallbackTriggeredFlag
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(DataSetDataAdapter.prototype, "selectedValuePath", {
                "get": function () {
                    return "Id"
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(DataSetDataAdapter.prototype, "displayMemberPath", {
                "get": function () {
                    return "DisplayValue"
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(DataSetDataAdapter.prototype, "defaultSelectedItem", {
                "get": function () {
                    var _this = this;
                    if (this.refreshDataSetTriggeredFlag) {
                        this.refreshDataSetTriggeredFlag = false;
                        return null
                    }
                    if (!MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.value.raw)) {
                        var selectedItem = null;
                        (new MscrmCommon.ArrayQuery(this.itemsCollection)).each(function (item) {
                            if (_this.inputBag.value.raw.localeCompare(item.OriginalValue) === 0) selectedItem = item
                        })
                    }
                    return selectedItem
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(DataSetDataAdapter.prototype, "itemsSource", {
                "get": function () {
                    return this.itemsCollection
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(DataSetDataAdapter.prototype, "itemsSourceFunction", {
                "get": function () {
                    var _this = this;
                    return function (query, max, callback) {
                        var filterManager = new AdvancedAutoComplete.DataSetFilterManager(_this.inputBag.dynamicSource.filtering);
                        filterManager.setFilter(query);
                        _this.refreshedItemsCallback = callback;
                        if (!MscrmCommon.ControlUtils.Object.isNullOrUndefined(_this.createPerformanceStopwatch)) _this.stopwatch = _this.createPerformanceStopwatch("CustomControl.Autocomplete.RetrieveDataSet");
                        !MscrmCommon.ControlUtils.Object.isNullOrUndefined(_this.stopwatch) && _this.stopwatch.start();
                        _this.inputBag.dynamicSource.refresh()
                    }
                },
                enumerable: true,
                configurable: true
            });
            DataSetDataAdapter.prototype.getCorrespondingOutputBag = function (item) {
                var selectedItem = item;
                return {
                    value: MscrmCommon.ControlUtils.Object.isNullOrUndefined(selectedItem) ? null : selectedItem.OriginalValue
                }
            };
            DataSetDataAdapter.prototype.refreshData = function () {
                var filterManager = new AdvancedAutoComplete.DataSetFilterManager(this.inputBag.dynamicSource.filtering);
                filterManager.clearFilter();
                this.inputBag.dynamicSource.refresh();
                this.refreshDataSetTriggeredFlag = true
            };
            DataSetDataAdapter.prototype.dispose = function () {
                if (!MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.itemsCollection)) {
                    this.itemsCollection.splice(0);
                    this.itemsCollection = null
                }
                this.createPerformanceStopwatch = null;
                this.inputBag = null;
                this.refreshedItemsCallback = null;
                this.stopwatch = null
            };
            DataSetDataAdapter.prototype.validateInputData = function () {
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.dynamicSource)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "DynamicSource");
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.dynamicSource.filtering)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "DynamicSource.filtering");
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.dynamicSource.records)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "DynamicSource.records");
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.dynamicSource.sortedRecordIds)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "DynamicSource.sortedRecordIds")
            };
            return DataSetDataAdapter
        }();
        AdvancedAutoComplete.DataSetDataAdapter = DataSetDataAdapter
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}));

var Ipg;
(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete) {
        var DataSetFilterManager = function () {
            function DataSetFilterManager(dataSetFiltering) {
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(dataSetFiltering)) throw new Error("Missing argument : dataSetFiltering");
                this.dataSetFiltering = dataSetFiltering
            }
            DataSetFilterManager.prototype.setFilter = function (expression) {
                var expressionCondition = {
                    attributeName: AdvancedAutoComplete.DynamicSourceDataSetPropertyNames.displayValueName,
                    conditionOperator: 6,
                    value: MscrmCommon.ControlUtils.String.Format("%{0}%", expression)
                },
                    filterExpression = {
                        conditions: [expressionCondition],
                        filterOperator: 1
                    };
                this.dataSetFiltering.clearFilter();
                this.dataSetFiltering.setFilter(filterExpression)
            };
            DataSetFilterManager.prototype.clearFilter = function () {
                this.dataSetFiltering.clearFilter()
            };
            return DataSetFilterManager
        }();
        AdvancedAutoComplete.DataSetFilterManager = DataSetFilterManager
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}));

var Ipg;
(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete) {
        var OptionSetDataAdapter = function () {
            function OptionSetDataAdapter() { }
            OptionSetDataAdapter.prototype.initialize = function (inputBag, htmlEncode) {
                this.inputBag = inputBag;
                this.htmlEncode = htmlEncode;
                this.validateInputData();
                this.prepareItemCollection()
            };
            Object.defineProperty(OptionSetDataAdapter.prototype, "refreshItemsCallbackTriggered", {
                "get": function () {
                    return false
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(OptionSetDataAdapter.prototype, "selectedValuePath", {
                "get": function () {
                    return "Value"
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(OptionSetDataAdapter.prototype, "displayMemberPath", {
                "get": function () {
                    return "EncodedLabel"
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(OptionSetDataAdapter.prototype, "defaultSelectedItem", {
                "get": function () {
                    var _this = this;
                    if (!MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.value.raw)) {
                        var selectedItem = (new MscrmCommon.ArrayQuery(this.itemsSource)).firstOrDefault(function (item) {
                            return _this.inputBag.value.raw.localeCompare(item.Label) === 0
                        });
                        if (!MscrmCommon.ControlUtils.Object.isNullOrUndefined(selectedItem)) return selectedItem
                    }
                    var defaultValue = this.inputBag.source.attributes.DefaultValue;
                    return this.getDefaultSelectedItem(this.itemsSource, defaultValue)
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(OptionSetDataAdapter.prototype, "itemsSource", {
                "get": function () {
                    return this.inputBag.source.attributes.Options
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(OptionSetDataAdapter.prototype, "itemsSourceFunction", {
                "get": function () {
                    return null
                },
                enumerable: true,
                configurable: true
            });
            OptionSetDataAdapter.prototype.getCorrespondingOutputBag = function (item) {
                ger;
                var option = item;
                return {
                    value: MscrmCommon.ControlUtils.Object.isNullOrUndefined(option) ? null : option.Label
                }
            };
            OptionSetDataAdapter.prototype.refreshData = function () { };
            OptionSetDataAdapter.prototype.dispose = function () {
                this.htmlEncode = null;
                this.inputBag = null
            };
            OptionSetDataAdapter.prototype.validateInputData = function () {
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.source)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "source");
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.source.attributes)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "source.attributes");
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.inputBag.source.attributes.Options)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "source.attributes.Options")
            };
            OptionSetDataAdapter.prototype.getDefaultSelectedItem = function (optionSetArray, defaultValue) {
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(defaultValue)) throw MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "source.attributes.defaultValue");
                return this.getOption(optionSetArray, defaultValue)
            };
            OptionSetDataAdapter.prototype.getOption = function (optionSet, value) {
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(value)) return null;
                return (new MscrmCommon.ArrayQuery(optionSet)).firstOrDefault(function (item) {
                    return item.Value === value
                })
            };
            OptionSetDataAdapter.prototype.prepareItemCollection = function () {
                var _this = this,
                    collection = this.inputBag.source.attributes.Options;
                (new MscrmCommon.ArrayQuery(collection)).each(function (item) {
                    item.Label = item.Label;
                    item.EncodedLabel = _this.htmlEncode(item.Label)
                })
            };
            OptionSetDataAdapter.invalidSelectedItemIndex = -1;
            return OptionSetDataAdapter
        }();
        AdvancedAutoComplete.OptionSetDataAdapter = OptionSetDataAdapter
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}));

var Ipg;
(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete) {
        var DataAdapterFactory = function () {
            function DataAdapterFactory() { }
            DataAdapterFactory.prototype.create = function (inputBag, createPerformanceStopwatch) {
                MscrmCommon.ControlUtils.Object.isNullOrUndefined(inputBag.dataSourceSwitch) && MscrmCommon.ErrorHandling.ExceptionHandler.throwException(MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidDataBagKeyFormat, "inputBag.dataSourceSwitch"));
                switch (inputBag.dataSourceSwitch) {
                    case DataAdapterFactory.DataSetGroup:
                        MscrmCommon.ControlUtils.Object.isNullOrUndefined(inputBag.dynamicSource) && MscrmCommon.ErrorHandling.ExceptionHandler.throwException(MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidDataBagKeyFormat, "inputBag.DynamicSource"));
                        return new AdvancedAutoComplete.DataSetDataAdapter(createPerformanceStopwatch);
                    case DataAdapterFactory.OptionSetGroup:
                        MscrmCommon.ControlUtils.Object.isNullOrUndefined(inputBag.source) && MscrmCommon.ErrorHandling.ExceptionHandler.throwException(MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidDataBagKeyFormat, "inputBag.source"));
                        return new AdvancedAutoComplete.OptionSetDataAdapter;
                    default:
                        MscrmCommon.ErrorHandling.ExceptionHandler.throwException(DataAdapterFactory.ErrorMessageInvalidData);
                        return null
                }
            };
            DataAdapterFactory.ErrorMessageInvalidData = "No data adapter could be provided based on the input data you provided.";
            DataAdapterFactory.DataSetGroup = "DataSetGroup";
            DataAdapterFactory.OptionSetGroup = "OptionSetGroup";
            return DataAdapterFactory
        }();
        AdvancedAutoComplete.DataAdapterFactory = DataAdapterFactory
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}));

var Ipg;
(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete) {
        var ItemsCollectionBuilder = function () {
            function ItemsCollectionBuilder(htmlEncode) {
                this.htmlEncode = htmlEncode
            }
            ItemsCollectionBuilder.prototype.build = function (sortedRecordIds, records, distinct) {
                var _this = this;
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(sortedRecordIds)) throw MscrmCommon.ControlUtils.String.Format(ItemsCollectionBuilder.ErrorInvalidParameter, "sortedRecordIds");
                if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(records)) throw MscrmCommon.ControlUtils.String.Format(ItemsCollectionBuilder.ErrorInvalidParameter, "records");
                var result = [];
                (new MscrmCommon.ArrayQuery(sortedRecordIds)).each(function (recId) {
                    if (MscrmCommon.ControlUtils.Object.isNullOrUndefined(records[recId])) throw ItemsCollectionBuilder.ErrorInvalidRecordId;
                    var recordObject = {
                        Id: recId,
                        OriginalValue: records[recId].displayValue,
                        DisplayValue: _this.htmlEncode(records[recId].displayValue)
                    };

                    //new code
                    if (distinct) {
                        var sameValue = result.filter(function (x) {
                            if (x.Id == recordObject.Id || x.OriginalValue == recordObject.OriginalValue ||
                                x.DisplayValue == recordObject.DisplayValue)
                                return x;
                        });

                        if (!sameValue || sameValue && sameValue.length < 1)
                            result.push(recordObject);
                    }
                    else
                    //end
                        result.push(recordObject);
                });
                return result
            };
            ItemsCollectionBuilder.ErrorInvalidParameter = "Invalid input parameter building the itemsSource collection: {0}";
            ItemsCollectionBuilder.ErrorInvalidRecordId = "Invalid record ID was found while building the itemsSource collection.";
            return ItemsCollectionBuilder
        }();
        AdvancedAutoComplete.ItemsCollectionBuilder = ItemsCollectionBuilder
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}));

var Ipg;
(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete) {
        var DynamicSourceDataSetPropertyNames = function () {
            function DynamicSourceDataSetPropertyNames() { }
            Object.defineProperty(DynamicSourceDataSetPropertyNames, "displayValueName", {
                "get": function () {
                    return "displayValue"
                },
                enumerable: true,
                configurable: true
            });
            return DynamicSourceDataSetPropertyNames
        }();
        AdvancedAutoComplete.DynamicSourceDataSetPropertyNames = DynamicSourceDataSetPropertyNames
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}));

var Ipg;
(function (Ipg) {
    var AdvancedAutoComplete;
    (function (AdvancedAutoComplete_1) {
        "use strict";
        var AdvancedAutoComplete = wijmo.input.AutoComplete,
            AdvancedAutoCompleteControl = function (_super) {
                __extends(AdvancedAutoCompleteControl, _super);

                function AdvancedAutoCompleteControl() {
                    var _this = this;
                    _super.call(this);
                    this.isUpdating = false;
                    this.hasFocus = false;
                    this.isDropdown = false;
                    this.checkValueOnFocusLost = function () {
                        if (!_this.advancedAutoCompleteControl.text || _this.advancedAutoCompleteControl.text == "") {
                            _this.advancedAutoCompleteControl.selectedItem = null;
                            _this.lastSelectedItem = null
                        } else {
                            if (_this.advancedAutoCompleteControl.selectedItem != _this.lastSelectedItem) _this.advancedAutoCompleteControl.selectedItem = _this.lastSelectedItem;
                            var text = _this.advancedAutoCompleteControl.getDisplayText(_this.advancedAutoCompleteControl.selectedIndex) || "";
                            if (text != _this.advancedAutoCompleteControl.text) {
                                //new code
                                text = _this.advancedAutoCompleteControl.text;
                                _this.advancedAutoCompleteControl.selectedItem = null;
                                _this.advancedAutoCompleteControl.selectedIndex = -1;
                                //end

                                _this.advancedAutoCompleteControl.text = text;
                            }
                        }
                        var updating = _this.isUpdating;
                        _this.isUpdating = false;
                        _this.notifyEnabledControlOutputChanged();
                        _this.isUpdating = updating
                    };
                    this.preventEditModePanoramaEvents = true
                }
                Object.defineProperty(AdvancedAutoCompleteControl.prototype, "controlDataAdapter", {
                    "get": function () {
                        return this.dataAdapter
                    },
                    enumerable: true,
                    configurable: true
                });
                AdvancedAutoCompleteControl.prototype.initCore = function (context) {
                    var _this = this;
                    this.dataAdapter = (new AdvancedAutoComplete_1.DataAdapterFactory).create(context.parameters, context.utils.createPerformanceStopwatch.bind(context.utils));
                    this.validateBoundTextField(context.parameters);
                    this.maxLength = context.parameters.value.attributes.MaxLength;
                    this.maxLengthExceededMessageFormat = context.resources.getString(AdvancedAutoCompleteControl.MaxLengthExceededId);
                    this.scrollToView = context.utils.scrollToView.bind(context.utils);
                    this.advancedAutoCompleteControl = this.createControlOnDOM(context);
                    this.advancedAutoCompleteControl.gotFocus.addHandler(function () {
                        _this.setEditMode(context.theming);
                        _this.hasFocus = true
                    });
                    this.advancedAutoCompleteControl.lostFocus.addHandler(function () {
                        _this.setReadMode(context.theming);
                        _this.hasFocus = false
                    });
                    this.advancedAutoCompleteControl.lostFocus.addHandler(this.checkValueOnFocusLost);
                    this.advancedAutoCompleteControl.selectedIndexChanged.addHandler(function () {
                        return _this.lastSelectedItem = _this.advancedAutoCompleteControl.selectedItem || _this.lastSelectedItem
                    });
                    this.advancedAutoCompleteControl.isDroppedDownChanged.addHandler(function () {
                        return _this.performControlRenderingAdjustments.apply(_this)
                    });
                    this.applyStyling(context.theming);
                    this.setReadMode(context.theming);
                    this.advancedAutoCompleteControl.refresh();
                    this.advancedAutoCompleteControl.fetchXml = function (fetchXml, entityName) {
                        var displayValue = context.parameters.dynamicSource.filtering.aliasMap.displayValue;
                        var encodedFetchXml = encodeURI(fetchXml);
                        var queryPath = "/api/data/v9.0/" + entityName + "s?fetchXml=" + encodedFetchXml;
                        var requestPath = Xrm.Page.context.getClientUrl() + queryPath;

                        var req = new XMLHttpRequest();
                        req.open("GET", requestPath, false);
                        req.setRequestHeader("Accept", "application/json");
                        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");

                        req.send();
                        var response = JSON.parse(req.responseText);
                        
                        var resultArray = response.value.map(function (x) {
                            return {
                                Id: x[entityName + "id"],
                                DisplayValue: x[displayValue],
                                OriginalValue: x[displayValue]
                            }
                        });

                        this.itemsSource.sourceCollection = resultArray;
                        this.itemsSource.currentPosition = -1;
                    };
                    var logicalName = context.parameters;
                    logicalName = logicalName && logicalName.value;
                    logicalName = logicalName && logicalName.attributes;
                    logicalName = logicalName && logicalName.LogicalName;
                    if (logicalName) {
                        Ipg.CustomControls.CCManager.addControl(
                            logicalName,
                            this.advancedAutoCompleteControl
                        );
                    }
                };
                AdvancedAutoCompleteControl.prototype.setEditMode = function (theming) {
                    this.advancedAutoCompleteControl.placeholder = "";
                    $(this.container).toggleClass(AdvancedAutoCompleteControl.ReadModeClassName, false);
                    $(this.advancedAutoCompleteControl.inputElement).css({
                        fontWeight: theming.textbox.fontweight,
                        color: theming.textbox.contentcolor
                    })
                };
                AdvancedAutoCompleteControl.prototype.setReadMode = function (theming) {
                    this.advancedAutoCompleteControl.placeholder = "---";
                    $(this.container).toggleClass(AdvancedAutoCompleteControl.ReadModeClassName, true);
                    $(this.advancedAutoCompleteControl.inputElement).css({
                        fontWeight: theming.textbox.contentfontweight,
                        color: theming.textbox.contentcolor
                    })
                };
                AdvancedAutoCompleteControl.prototype.updateCore = function (context) {
                    var _this = this;
                    if (context.parameters.dataSourceSwitch === AdvancedAutoComplete_1.DataAdapterFactory.DataSetGroup && !MscrmCommon.ControlUtils.Object.isNullOrUndefined(context.parameters.dynamicSource) && !MscrmCommon.ControlUtils.Object.isNullOrUndefined(context.parameters.dynamicSource.working) && context.parameters.dynamicSource.working) return;
                    this.isUpdating = true;
                    this.dataAdapter.initialize(context.parameters, this.htmlEncode);

                    this.validateBoundTextField(context.parameters);
                    this.advancedAutoCompleteControl.isDisabled = context.mode.isControlDisabled || !context.parameters.value.security.editable;
                    if (!this.hasFocus && (context.updatedProperties.length == 0 || context.updatedProperties.indexOf("dataset") != -1)) {
                        var value = this.advancedAutoCompleteControl.text;

                        if (!this.advancedAutoCompleteControl.itemsSource) this.advancedAutoCompleteControl.itemsSource = this.dataAdapter.itemsSource;
                        if (!this.advancedAutoCompleteControl.itemsSourceFunction) this.advancedAutoCompleteControl.itemsSourceFunction = this.dataAdapter.itemsSourceFunction;
                        this.advancedAutoCompleteControl.selectedValuePath = this.dataAdapter.selectedValuePath;
                        this.advancedAutoCompleteControl.displayMemberPath = this.dataAdapter.displayMemberPath;
                        this.advancedAutoCompleteControl.selectedItem = this.dataAdapter.defaultSelectedItem;
                        if (!this.advancedAutoCompleteControl.selectedItem && this.advancedAutoCompleteControl.text != value)
                            this.advancedAutoCompleteControl.text = value
                        else if (!this.advancedAutoCompleteControl.selectedItem && context.parameters &&
                            context.parameters.value && context.parameters.value.raw) {
                            this.advancedAutoCompleteControl.text = context.parameters.value.raw;
                        }
                    }
                    else if (!this.hasFocus && context.updatedProperties.length > 0 && context.updatedProperties.indexOf("value") > -1) {
                        if (context.parameters && context.parameters.value && context.parameters.value.raw && 
                            this.advancedAutoCompleteControl.text != context.parameters.value.raw) {
                            this.advancedAutoCompleteControl.text = context.parameters.value.raw;
                        }
                    }

                    this.isUpdating = false
                };
                AdvancedAutoCompleteControl.prototype.renderReadMode = function (context) { };
                AdvancedAutoCompleteControl.prototype.renderEditMode = function (context) { };
                AdvancedAutoCompleteControl.prototype.getOutputsCore = function () {
                    var outputBag = this.dataAdapter.getCorrespondingOutputBag(this.advancedAutoCompleteControl.selectedItem);
                    if (!outputBag)
                        outputBag = {};
                    if (outputBag && !outputBag.value) 
                        outputBag.value = this.advancedAutoCompleteControl.text;

                    this.validateOutputBag(outputBag);
                    return outputBag
                };
                AdvancedAutoCompleteControl.prototype.destroyCore = function () {
                    if (this.isControlInitialized) {
                        this.advancedAutoCompleteControl.isDroppedDownChanged.removeAllHandlers();
                        this.advancedAutoCompleteControl.selectedIndexChanged.removeAllHandlers();
                        this.advancedAutoCompleteControl.itemsSource = null;
                        this.advancedAutoCompleteControl.dispose();
                        this.dataAdapter.dispose();
                        $(this.container).removeAttr("style");
                        $(this.container).removeAttr("class");
                        $(this.container).empty()
                    }
                    this.advancedAutoCompleteControl = null;
                    this.scrollToView = null;
                    this.dataAdapter = null
                };
                AdvancedAutoCompleteControl.prototype.notifyEnabledControlOutputChanged = function () {
                    //!this.advancedAutoCompleteControl.isDisabled && !this.isUpdating && !this.advancedAutoCompleteControl.isDroppedDown && this.advancedAutoCompleteControl.selectedIndex !== -1 && this.notifyOutputChanged()
                    !this.advancedAutoCompleteControl.isDisabled && !this.isUpdating && !this.advancedAutoCompleteControl.isDroppedDown && this.notifyOutputChanged()
                };
                AdvancedAutoCompleteControl.prototype.validateBoundTextField = function (inputBag) {
                    MscrmCommon.ControlUtils.Object.isNullOrUndefined(inputBag.value) && MscrmCommon.ErrorHandling.ExceptionHandler.throwException(MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "value"));
                    MscrmCommon.ControlUtils.Object.isNullOrUndefined(inputBag.value.attributes) && MscrmCommon.ErrorHandling.ExceptionHandler.throwException(MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "value.attributes"));
                    MscrmCommon.ControlUtils.Object.isNullOrUndefined(inputBag.value.attributes.MaxLength) && MscrmCommon.ErrorHandling.ExceptionHandler.throwException(MscrmCommon.ControlUtils.String.Format(MscrmCommon.CommonControl.InvalidInputParam, "value.attributes.maxLength"))
                };
                AdvancedAutoCompleteControl.prototype.createControlOnDOM = function (context) {
                    $(this.container).css("position", "relative");
                    var control = new AdvancedAutoComplete(_super.prototype.createWrapperContainer.call(this));
                    control.inputElement.setAttribute("aria-label", context.mode.label ? context.mode.label : context.parameters.value.attributes.DisplayName);
                    control.inputElement.setAttribute("aria-autocomplete", "list");
                    control.inputElement.setAttribute("aria-haspopup", "true");
                    control.inputElement.setAttribute("aria-multiline", "false");
                    control.inputElement.setAttribute("role", "combobox");
                    return control
                };
                AdvancedAutoCompleteControl.prototype.validateOutputBag = function (outputBag) {
                    this.notificationHandler.clear(MscrmCommon.ErrorHandling.ErrorCode.MaxLengthExceededId);
                    if (!MscrmCommon.ControlUtils.String.isNullOrEmpty(outputBag.value) && outputBag.value.length > this.maxLength) {
                        var notification = MscrmCommon.ControlUtils.String.Format(this.maxLengthExceededMessageFormat, this.maxLength);
                        this.notificationHandler.notify(notification, MscrmCommon.ErrorHandling.ErrorCode.MaxLengthExceededId)
                    }
                };
                AdvancedAutoCompleteControl.prototype.performControlRenderingAdjustments = function () {
                    if (this.advancedAutoCompleteControl.isDroppedDown) {
                        this.isDropdown != this.advancedAutoCompleteControl.isDroppedDown && this.advancedAutoCompleteControl.refresh();
                        this.autoScrollControlIntoView()
                    }
                    this.isDropdown = this.advancedAutoCompleteControl.isDroppedDown
                };
                AdvancedAutoCompleteControl.prototype.autoScrollControlIntoView = function () {
                    var itemsExistFromManualUpdate = !MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.advancedAutoCompleteControl.itemsSource) && this.advancedAutoCompleteControl.itemsSource.length > 0,
                        itemsExistFromCallbackUpdate = !MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.advancedAutoCompleteControl.itemsSource.items) && this.advancedAutoCompleteControl.itemsSource.items.length > 0;
                    if (itemsExistFromManualUpdate || itemsExistFromCallbackUpdate) {
                        var controlContainer = $(this.container);
                        !MscrmCommon.ControlUtils.Object.isNullOrUndefined(this.scrollToView) && this.scrollToView(controlContainer);
                        var rootScrollElement = !MscrmCommon.ControlUtils.Object.isNullOrUndefined(document.body) ? document.body : document.documentElement;
                        rootScrollElement.scrollTop = controlContainer.offsetParent().offset().top
                    }
                };
                AdvancedAutoCompleteControl.prototype.applyStyling = function (theming) {
                    var themeFormat = ".crm-wijmo-autocomplete.wj-dropdown-panel :not(.wj-state-selected):not(.wj-separator).wj-listbox-item:active {                                                                                background-color: {0};                                                                               font-family: {2};                                                                      }                .crm-wijmo-autocomplete.wj-dropdown-panel .wj-listbox-item.wj-state-selected {                        background-color: {1}                        font-family: {2};                }                .wj-content.crm-wijmo-autocomplete {                     border: {3} {4} {5};                }                .crm-wijmo-autocomplete .wj-btn-default {                    border: {3} {4} transparent;                }                .crm-wijmo-autocomplete .wj-btn-default:hover, .crm-wijmo-autocomplete .wj-btn-default:focus{                     background-color: inherit;                }                .crm-wijmo-autocomplete .wj-input {                    background-color: {6};                }",
                        theme = MscrmCommon.ControlUtils.String.Format(themeFormat, MscrmCommon.ThemingHelper.getHoverLinkColor(theming), MscrmCommon.ThemingHelper.getGlobalLinkColor(theming), MscrmCommon.ThemingHelper.getFontFamily(theming), theming.solidborderstyle, theming.textbox.linethickness, theming.textbox.hoverboxcolor, theming.textbox.backgroundcolor);
                    MscrmCommon.ThemingHelper.injectStyle(this.container, theme);
                    this.advancedAutoCompleteControl.hostElement.classList.add(AdvancedAutoCompleteControl.CustomWrapperClassName);
                    this.advancedAutoCompleteControl.dropDown.classList.add(AdvancedAutoCompleteControl.CustomWrapperClassName);
                    this.advancedAutoCompleteControl.maxDropDownHeight = AdvancedAutoCompleteControl.MaxDropDownHeightInPixels;
                    var arrowStyle = {
                        width: "0.6em",
                        height: "0.6em",
                        marginTop: "-0.4em",
                        borderColor: "#1E83CD",
                        borderWidth: "0em",
                        borderRightWidth: "0.1em",
                        borderBottomWidth: "0.1em",
                        borderStyle: "solid",
                        transform: "rotate(45deg)",
                        content: "",
                        display: "flex"
                    },
                        icon = $("<div>").css(arrowStyle);
                    $(this.advancedAutoCompleteControl._btn).find("button").empty().append(icon)
                };
                AdvancedAutoCompleteControl.MaxDropDownHeightInPixels = 160;
                AdvancedAutoCompleteControl.CustomWrapperClassName = "crm-wijmo-autocomplete";
                AdvancedAutoCompleteControl.ReadModeClassName = "crm-wijmo-readmode";
                AdvancedAutoCompleteControl.MaxLengthExceededId = "AutoCompleteControl_MaxLengthExceeded_Error";
                return AdvancedAutoCompleteControl
            }(MscrmCommon.FieldControlBase);
        AdvancedAutoComplete_1.AdvancedAutoCompleteControl = AdvancedAutoCompleteControl
    })(AdvancedAutoComplete = Ipg.AdvancedAutoComplete || (Ipg.AdvancedAutoComplete = {}))
})(Ipg || (Ipg = {}))