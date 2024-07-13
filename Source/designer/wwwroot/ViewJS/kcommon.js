var kcommon = (function () {
    function kcommon() {
    }

    kcommon.dataTypeDict = {};
    kcommon.inputTypeDict = {};

    kcommon.dataTypeDict["VARCHAR"] = 1;
    kcommon.dataTypeDict["INT"] = 2;
    kcommon.dataTypeDict["DECIMAL"] = 3;
    kcommon.dataTypeDict["DATE"] = 4;
    kcommon.dataTypeDict["TEXT"] = 5;

    kcommon.inputTypeDict["TEXTBOX"] = 1;
    kcommon.inputTypeDict["PASSWORD"] = 2;
    kcommon.inputTypeDict["COMBOBOX"] = 3;
    kcommon.inputTypeDict["CHECKBOXGROUP"] = 4;
    kcommon.inputTypeDict["RADIOGROUP"] = 5;
    kcommon.inputTypeDict["MULTIPLELIST"] = 6;
    kcommon.inputTypeDict["DATE"] = 7;
    kcommon.inputTypeDict["TEXT"] = 8;
    kcommon.inputTypeDict["LABEL"] = 9;
    kcommon.inputTypeDict["HYPERLINK"] = 12;
    kcommon.inputTypeDict["DATATABLE"] = 15;
    kcommon.inputTypeDict["BUTTON"] = 16;
    kcommon.inputTypeDict["IMAGE"] = 17;

    kcommon.getDataTypeByID = function (id) {
        var text = "";
        $.each(kcommon.dataTypeDict, function (key, value) {
            if (value === id) {
                text = key;
                return false;
            }
        })
        return text;
    }

    kcommon.getInputTypeByID = function (id) {
        var text = "";
        $.each(kcommon.inputTypeDict, function (key, value) {
            if (value === id) {
                text = key;
                return false;
            }
        })
        return text;
    }

    //initialize form controls include button, datetime control
    kcommon.initializeControls = function (entityDefID, formContainer, isSubPage) {
        //datetime picker
        $(".form_datetime").datepicker({
            format: 'yyyy-mm-dd',
            language: 'zh',
            autoclose: true,
            todayBtn: true,
            useCurrent: false,
            pickerPosition: "bottom-left"
        }).datepicker({
            "setDate": new Date(),
            });

        //attach button click event 
        $("button[data-onclick]").each(function (i) {
            var func = $(this).attr("data-onclick");
            if (func) {
                var f = eval("(" + func + ")");
                $(this).click(function () {
                    try {
                        f();
                    } catch (error) {
                        //window.console.log(error);
                    }
                })
            }
        });

        //dropdown list control binding data source
        $.each($(".ctrl-combobox"), function (index, item) {
            var parent = $(item).parent();          
            var attrEntity = fieldPopup.getHiddenAttribute(parent);
            //loading data source
            if (attrEntity.DataSourceType >= 2
                && (!attrEntity.CascadeControlID || attrEntity.CascadeControlID === "")) {
                $(item).empty();
                $(item).append("<option value=''>--default--</option>");

                $(item).removeAttr("onchange");
                $(item).attr("onchange", "kcommon.reloadAllCascadeDataList(" + attrEntity.EntityDefID
                    + "," + attrEntity.ID
                    + ",'" + attrEntity.AttrCode 
                    + "')");
                loadControlDataSource(attrEntity, isSubPage, $(item), renderFormControlOnPage);
            } else {
                ;
            }
        });

        //radio group control binding data source
        $.each($(".ctrl-radiogroup"), function (index, item) {
            var parent = $(item).parent();
            var attrEntity = fieldPopup.getHiddenAttribute(parent);
            //loading data source
            if (attrEntity.DataSourceType >= 2
                && (!attrEntity.CascadeControlID || attrEntity.CascadeControlID === "")) {
                $(item).empty();
                loadControlDataSource(attrEntity, isSubPage, $(item), renderFormControlOnPage);
            } else {
                ;
            }
        });

        //checkbox group control binding data source
        $.each($(".ctrl-checkboxgroup"), function (index, item) {
            var parent = $(item).parent();
            var attrEntity = fieldPopup.getHiddenAttribute(parent);
            //loading data source
            if (attrEntity.DataSourceType >= 2
                && (!attrEntity.CascadeControlID || attrEntity.CascadeControlID === "")) {
                $(item).empty();
                loadControlDataSource(attrEntity, isSubPage, $(item), renderFormControlOnPage);
            } else {
                ;
            }
        });

        $.each($(".ctrl-selectmultiplelist"), function (index, item) {
            var parent = $(item).parent();
            var attrEntity = fieldPopup.getHiddenAttribute(parent);
            //loading data source
            if (attrEntity.DataSourceType >= 2
                && (!attrEntity.CascadeControlID || attrEntity.CascadeControlID === "")) {
                $(item).empty();
                loadControlDataSource(attrEntity, isSubPage, $(item), renderFormControlOnPage);
            } else {
                ;
            }
        });

        //binding textbox event
        var eventQuery = {};
        eventQuery.EntityDefID = entityDefID;

        var url = 'api/fbmaster/GetEntityAttributeEventListByForm';
        if (isSubPage === true) {
            url = '../../api/fbmaster/GetEntityAttributeEventListByForm';
        }

        jshelper.ajaxPost(url,
            JSON.stringify(eventQuery),
            function (result) {
                if (result.Status === 1) {
                    var eventList = result.Entity;
                    $.each(eventList, function (index, item) {
                        var attrID = item.AttrID;
                        var divCtrlKey = item.DivCtrlKey;
                        var divCtrl = $("#" + divCtrlKey);

                        //binding event on textbox control
                        var textBoxList = divCtrl.find(".ctrl-textbox");
                        $.each(textBoxList, function (index, textbox) {
                            $(textbox).removeAttr(item.EventName);
                            $(textbox).attr(item.EventName, item.CommandText);
                        });
                    });
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }

    function renderFormControlOnPage(attrEntity, control, value, text) {
        var inputTypeID = attrEntity.FieldInputType;
        var inputControlType = kcommon.getInputTypeByID(inputTypeID);

        if (inputControlType === "COMBOBOX") {
            $(control).append("<option value='" + value + "'>" + text + "</option>");
        } else if (inputControlType === "CHECKBOXGROUP") {
            $(control).append('<span style="display:block;"><input type="checkbox" name=' + attrEntity.AttrCode + ' value = ' + value + ' /> ' + text + '</span > ');
        } else if (inputControlType === "RADIOGROUP") {
            $(control).append('<span style="display:block;"><input type="radio" name=' + attrEntity.AttrCode + ' value = ' + value + ' /> ' + text + '</span > ');
        } else if (inputControlType === "MULTIPLELIST") {
            $(control).append("<option value='" + value + "'>" + text + "</option>");
        } else {
            kmsgbox.error(kresource.getItem('fieldManagerunknowncontroltypeerrormsg'), inputControlType);
        }
    }

    function getControlTypeByDivControl(attrEntity, divControl) {
        var inputTypeID = attrEntity.FieldInputType;
        var inputControlType = kcommon.getInputTypeByID(inputTypeID);

        var control = null;
        if (inputControlType === "COMBOBOX") {
            control = $(divControl).find(".ctrl-combobox");
            $(control).find("option").remove();
            $(control).append("<option value=''>--default--</option>");
        } else if (inputControlType === "CHECKBOXGROUP") {
            control = $(divControl).find(".ctrl-checkboxgroup"); 
            $(control).find("span").remove();
        } else if (inputControlType === "RADIOGROUP") {
            control = $(divControl).find(".ctrl-radiogroup");
            $(control).find("span").remove();
        } else if (inputControlType === "MULTIPLELIST") {
            control = $(divControl).find(".ctrl-selectmultiplelist");
            $(control).find("option").remove();
        } else {
            kmsgbox.error(kresource.getItem('fieldManagerunknowncontroltypeerrormsg'), inputControlType);
        }
        return control;
    }

    function loadControlDataSource(attrEntity, isSubPage, dynControl, callback) {
        var url = 'api/eavdata/LoadControlDataSource';
        if (isSubPage === true) {
            url = '../../api/eavdata/LoadControlDataSource';
        }

        jshelper.ajaxPost(url,
            JSON.stringify(attrEntity),
            function (result) {
                if (result.Status === 1) {
                    var dataSourceEntityList = result.Entity;
                    if (dataSourceEntityList !== null) {
                        for (var i = 0; i < dataSourceEntityList.length; i++) {
                            var item = dataSourceEntityList[i];
                            var value = item[attrEntity.DataValueField];
                            var text = item[attrEntity.DataTextField];
                            //build options
                            callback(attrEntity, dynControl, value, text);
                        }
                    } else {
                        window.console.log("data source entity list is null...");
                    }
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }

    kcommon.reloadAllCascadeDataList = function (entityDefID, attrID, attrCode) {
        var parentControl = $("select[name=" + attrCode + "]");
        var parentControlValue = $(parentControl).val();

        var query = {};
        query.EntityDefID = entityDefID;
        query.AttributeID = attrID;

        var url = '../../api/eavdata/QueryCascadeChildControlList';
        jshelper.ajaxPost(url,
            JSON.stringify(query),
            function (result) {
                if (result.Status === 1) {
                    var attrList = result.Entity;
                    $.each(attrList, function (index, item) {
                        loadEachCascadeControlData(entityDefID, parentControlValue, item);
                    });
                }
            });
    }

    function loadEachCascadeControlData(entityDefID, parentControlValue, attribute) {
        var divControlID = attribute.DivCtrlKey;
        var divControl = $("#" + divControlID);
        var attrObj = fieldPopup.getHiddenAttribute(divControl);

        var query = {};
        query.EntityDefID = entityDefID;
        query.DataEntityName = attribute.DataEntityName;
        query.ParentControlValue = parentControlValue;
        query.CascadeFieldName = attribute.CascadeFieldName;

        var url = '../../api/eavdata/LoadCascadeControlDataSource';
        jshelper.ajaxPost(url,
            JSON.stringify(query),
            function (result) {
                if (result.Status === 1) {
                    var optionList = result.Entity;
                    $.each(optionList, function (index, item) {
                        var control = getControlTypeByDivControl(attribute, divControl);
                        renderFormControlOnPage(attribute, control, item[attrObj.DataValueField], item[attrObj.DataTextField]);
                    });
                }
            });
    }

    //file upload
    kcommon.upload = function () {
        BootstrapDialog.show({
            title: 'File Upload...',
            message: $('<div></div>').load(jshelper.getAppName() + '/file/upload'),
            draggable: true
        });
    }

    //upload file
    kcommon.initFileImport = function () {
        var appName = jshelper.getAppName();
        var restrictedUploader = new qq.FineUploader({
            element: document.getElementById("fine-uploader-validation"),
            template: 'qq-template-validation',
            request: {
                endpoint: appName + '/api/FineUpload/Import',       //fineupload webapi
                params: {
                    extraParam1: "1",
                    extraParam2: "2"
                }
            },
            thumbnails: {
                placeholders: {
                    waitingPath: appName + '/Content/fineuploader/waiting-generic.png',
                    notAvailablePath: appName + '/Content/fineuploader/not_available-generic.png'
                }
            },
            validation: {
                allowedExtensions: ['xml', 'txt', 'doc', 'docx', 'pdf', 'xls', 'xlsx', 'png', 'jpg', 'gif'],
                itemLimit: 1,
                sizeLimit: 51200 // 50 kB = 50 * 1024 bytes
            },
            callbacks: {
                onComplete: function (id, fileName, result) {
                    if (result.success === true) {
                        kmsgbox.info(result.Message);
                    }
                    else {
                        kmsgbox.error(result.ExceptionMessage);
                    }
                }
            }
        });
    }
    return kcommon;
})();