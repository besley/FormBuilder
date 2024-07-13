/*
* Slickflow 工作流引擎企业版快速开发框架遵循LGPL协议，也可联系作者获取商业授权
* 和技术支持服务；除此之外的使用，则视为不正当使用，请您务必避免由此带来的
* 商业版权纠纷。
*
The Slickflow Product.
Copyright (C) 2017  .NET Authorization Framework Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, you can access the official
web page about lgpl: https://www.gnu.org/licenses/lgpl.html
*/

var fieldManager = (function () {
    function fieldManager() {
    }

    var mInputControlType = {
        "textbox": 1,
        "passwordbox": 2,
        "combobox": 3,
        "checkboxgroup": 4,
        "radiogroup": 5,
        "selectmultiplelist": 6,
        "date": 7,
        "text": 8,
        "label": 9,
        "datatable": 15,
        "btn": 16,
        "image": 17
    };

    fieldManager.saveAttribute = function (e, obj) {
        //validate control code
        var title = $("#eavForm").find("input[name=title]").val();
        if (title === null || title === "") {
            kmsgbox.warn(kresource.getItem('updateattributewarningmsg'));
            return false;
        }

        var formValues = {};
        var val = null;
        //get field description value
        $("#eavForm").find("input, textarea, select").each(function (i, o) {
            if (o.type === "checkbox") {
                val = o.checked;
            }
            else {
                val = o.value;
            }

            //save value
            if (o.name !== "") formValues[o.name] = val;
        });

        commonSave(formValues);
        function commonSave(values) {
            var divCtrl = $("#" + values.forCtrl);
            divCtrl.find(".control-label").text(values.title);
            var listHiddenMandatory = divCtrl.find(".hiddenMandatory");

            if (listHiddenMandatory != null && listHiddenMandatory.length > 0) {
                var ctrlMandatory = listHiddenMandatory[0];
                ctrlMandatory.value = values.mandatory;
            }

            //save field info to database
            var attributeEntity = divCtrl.find(".hiddenAttributeEntity").val() != "" ?
                $.parseJSON(divCtrl.find(".hiddenAttributeEntity").val()) : {};

            attributeEntity.EntityDefID = kmaster.mcurrentEntityDefID;
            attributeEntity.DivCtrlKey = values.forCtrl;
            attributeEntity.AttrName = values.title.replace(/\s/g, "");     //remove spaces in the input text title
            attributeEntity.AttrDataType = values.dataType;

            var inputStorageType = getFieldInputStorageType(values.type);
            attributeEntity.FieldInputType = inputStorageType.fieldInputType;
            attributeEntity.StorageType = inputStorageType.storageType;
            attributeEntity.Format = values.dateFormat === undefined ? "" : values.dateFormat;
            attributeEntity.IsMandatory = values.mandatory === true ? 1 : 0;
            attributeEntity.ConditionKey = values.conditionKey;
            attributeEntity.VariableName = values.variableName;
            attributeEntity.CommandText = values.commandText;
            attributeEntity.DataSourceType = values.dataSourceType;
            attributeEntity.DataEntityName = values.dataEntityName;
            attributeEntity.DataValueField = values.dataValueField;
            attributeEntity.DataTextField = values.dataTextField;
            attributeEntity.CascadeControlID = values.cascadeControlID;
            attributeEntity.CascadeFieldName = values.cascadeFieldName;
            attributeEntity.Url = values.hyperlink;

            //post
            jshelper.ajaxPost('api/eavdata/SaveAttribute',
                JSON.stringify(attributeEntity),
                function (result) {
                    if (result.Status === 1) {
                        var updAttrEntity = result.Entity;
                        //binding
                        divCtrl.find(".hiddenAttributeEntity").attr("value", JSON.stringify(updAttrEntity));
                        //更新控件
                        values.code = updAttrEntity.AttrCode;
                        var specificSaveMethod = fieldChangeSave[values.type];
                        if (typeof (specificSaveMethod) !== "undefined") {
                            specificSaveMethod(values);
                            setFieldMandatory(values);
                        }

                        //保存模板
                        var templateContent = $("#selected-content")[0].outerHTML;
                        var entityDef = {
                            "ID": kmaster.mcurrentEntityDefID,
                            "TemplateContent": templateContent
                        };
                        jshelper.ajaxPost('api/eavdata/SaveTemplateContent',
                            JSON.stringify(entityDef), function (result) {
                                if (result.Status === 1) {
                                    //draggable the current drop field
                                    kmaster.makeDropFieldDraggable();
                                } else {
                                    kmsgbox.error(kresource.getItem('fieldManager.saveAttribute.saveTemplate.errormsg', result.Message));
                                }
                            });
                    } else {
                        kmsgbox.error(result.Message);
                    }
                });
        }

        function setFieldMandatory(values) {
            var divCtrl = $("#" + values.forCtrl);
            var label = $(divCtrl).find("label")[0];

            if (values.mandatory === true) {
                $(divCtrl).addClass("form-group required");
                $(label).addClass("mandatory-field");
            } else {
                $(divCtrl).removeClass("form-group required");
                $(label).removeClass("mandatory-field");
            }
        }

        function getFieldInputStorageType(type) {
            var inputStorageType = {};
            inputStorageType.storageType = 1;

            //get control type
            inputStorageType.fieldInputType = mInputControlType[type];
            if (inputStorageType.fieldInputType !== null) {
                if (type === "datatable"
                    || type === "btn"
                    || type === "image"
                    || type === "label") {
                    inputStorageType.storageType = 0;
                }
            }
            else {
                //show error message
                kmsgbox.error(kresource.getItem("fieldManagerunknowncontroltypeerrormsg"));
            }
            return inputStorageType;
        }
    };

    fieldManager.deleteField = function (e, obj) {
        var ctrlId = $("#eavForm").find("input[name=forCtrl]").val();
        var divCtrl = $("#" + ctrlId);

        kmsgbox.confirm(kresource.getItem('fieldcontroldeleteconfirmmsg'), function (result) {
            if (result === "Yes") {
                //remove database record
                var attrEntity = divCtrl.find(".hiddenAttributeEntity").val() != "" ?
                    $.parseJSON(divCtrl.find(".hiddenAttributeEntity").val()) : null;

                //remove ui element
                divCtrl.remove();

                //not saved in database, only appreare in the UI.
                if (attrEntity === null) return false;

                //save template content
                var templateContent = $("#selected-content")[0].outerHTML;
                var entityDef = {
                    "ID": kmaster.mcurrentEntityDefID,
                    "TemplateContent": templateContent,
                    "HTMLContent": ""
                };

                var attrEntityList = new Array();
                attrEntityList[0] = attrEntity;

                var entity = {};
                entity.EntityDef = entityDef;
                entity.EntityAttributeList = attrEntityList;

                //post
                jshelper.ajaxPost('api/eavdata/DeleteAttributeWithTemplate',
                    JSON.stringify(entity),
                    function (result) {
                        if (result.Status === 1) {
                            kmsgbox.info(kresource.getItem('fieldcontroldeletedokmsg'));
                            //$("[name=customization_modal]").modal("hide");
                        } else {
                            kmsgbox.error(result.Message);
                        }
                    });
            }
        });
    };
    return fieldManager;
})();

var fieldPopup = (function () {
    function fieldPopup() {
    }

    //tab configuration
    var myPageTabs = {};
    myPageTabs["textbox"] = ["tabControlVar", "tabBindingEvent"];
    myPageTabs["text"] = ["tabControlVar", "tabBindingEvent"];
    myPageTabs["combobox"] = ["tabControlVar", "tabDataSource", "tabBindingEvent"];
    myPageTabs["radiogroup"] = ["tabControlVar", "tabDataSource", "tabBindingEvent"];
    myPageTabs["checkboxgroup"] = ["tabControlVar", "tabDataSource", "tabBindingEvent"];
    myPageTabs["selectmultiplelist"] = ["tabControlVar", "tabDataSource", "tabBindingEvent"];
    myPageTabs["date"] = ["tabControlVar", "tabBindingEvent"];
    myPageTabs["btn"] = ["tabCommandText"];
    myPageTabs["hyperlink"] = ["tabHyperLink"];
    myPageTabs["datatable"] = ["tabDataTable"];

    var myDataSourceTabs = {};
    myDataSourceTabs["customized"] = ["tabDsCustomized"];
    myDataSourceTabs["others"] = ["tabEntityInfo", "tabCascadeField"];

    function showTabsByControlType(controlType) {
        $(".hideme").hide();

        var tabs = myPageTabs[controlType];
        if (tabs) {
            for (var i = tabs.length; i > 0; i--) {
                var tab = tabs[i - 1];
                $("#" + tab).parent().show();
                $("#" + tab).show();
                $("#" + tab).tab('show');
            }
        }
    }

    fieldPopup.popupControl = function (ctrlType, ctrlId) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var listHiddenMandatory = divCtrl.find(".hiddenMandatory");

        if (listHiddenMandatory !== null && listHiddenMandatory.length > 0) {
            var ctrlMandatory = listHiddenMandatory[0];
            form.find("[name=mandatory]").attr("checked", ctrlMandatory.value === "true");
            form.find("#pMandatory").show;
        }
        else {
            form.find("#pMandatory").hide();
        }

        form.find("[name=title]").val(divCtrl.find(".control-label").text());

        var attrEntity = fieldPopup.getHiddenAttribute(divCtrl);
        if (attrEntity) {
            form.find("[name=conditionKey]").val(attrEntity.ConditionKey);
            form.find("[name=variableName]").val(attrEntity.VariableName);
        }

        //show mytab
        showTabsByControlType(ctrlType);

        //datasource type render
        $("#ddlDataSourceType").on('change', function (e) {
            var dataSourceType = this.value;
            setDataSourceControlVisible(parseInt(dataSourceType));
        });

        var specificLoadMethod = fieldPopup[ctrlType];
        if (typeof (specificLoadMethod) != "undefined") {
            specificLoadMethod(ctrlType, ctrlId, attrEntity);
        }

        if (attrEntity) {
            //load bingding event list
            attributelist.pselectedAttributeEntity = attrEntity;
            attributelist.loadEvent(attrEntity.EntityDefID, attrEntity.ID);
        }
    };

    fieldPopup.getHiddenAttribute = function (divCtrl) {
        var attrJSONObj = null;
        var hiddenValue = divCtrl.find(".hiddenAttributeEntity").val();
        if (hiddenValue && hiddenValue !== "") {
            attrJSONObj = $.parseJSON(hiddenValue);
        }
        return attrJSONObj;
    }

    fieldPopup.textbox = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrlText = divCtrl.find("input[type=text]")[0];

        form.find("[name=placeholder]").val(ctrlText.placeholder);
        if (attrEntity != null) form.find("[name=dataType]").val(attrEntity.AttrDataType);
    };

    fieldPopup.passwordbox = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrlText = divCtrl.find("input[type=password]")[0];

        form.find("[name=placeholder]").val(ctrlText.placeholder);
    };

    fieldPopup.combobox = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrl = divCtrl.find("select")[0];

        var dsType = attrEntity !== null ? parseInt(attrEntity.DataSourceType) : 0;
        form.find("[name=dataSourceType]").val(dsType);
        setDataSourceControlVisible(dsType);

        //loading cascading control list
        loadEntityAttributeList(function () {
            //set customized options for combobox
            if (dsType === 1) {
                var options = '';
                $(ctrl).find('option').each(function (i, o) { options += o.text + '\n'; });
                form.find("[name=options]").val($.trim(options));
            } else if (dsType === 2
                || dsType === 3
                || dsType === 4
                || dsType === 5) {
                fillDataSourceItems(form, attrEntity);
            }
        });
    };

    //if there is combobox control in the form
    function loadEntityAttributeList(callback) {
        jshelper.ajaxGet("api/eavdata/GetEntityAttributeList/" + kmaster.mcurrentEntityDefID, null, function (result) {
            if (result.Status === 1) {
                $("#ddlDataSourceCascadeControl").empty();
                var attributelist = result.Entity;
                $("#ddlDataSourceCascadeControl").append("<option value=''>--default--</option>");
                $.each(attributelist, function (index, item) {
                    var value = item.ID;
                    var text = item.AttrName + '--' + item.AttrCode;
                    $("#ddlDataSourceCascadeControl").append("<option value='" + value + "'>" + text + "</option>");
                });

                callback();
            }
        });
    }

    fieldPopup.radiogroup = function (ctrlType, ctrlId, attrEntity) {
        $("#pDataSourceType").show();

        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);

        var dsType = attrEntity !== null ? parseInt(attrEntity.DataSourceType) : 0;
        form.find("[name=dataSourceType]").val(dsType);
        setDataSourceControlVisible(dsType);

        //loading cascading control list
        loadEntityAttributeList(function () {
            if (dsType === 1) {
                //set custmozied options
                var options = '';
                var ctrls = divCtrl.find("div").find("span");
                var radios = divCtrl.find("div").find("input");

                ctrls.each(function (i, o) { options += $(o).text() + '\n'; });
                form.find("[name=options]").val($.trim(options));
            } else if (dsType === 2
                || dsType === 3
                || dsType === 4
                || dsType === 5) {
                fillDataSourceItems(form, attrEntity);
            }
        });
    };

    fieldPopup.checkboxgroup = function (ctrlType, ctrlId, attrEntity) {
        $("#pDataSourceType").show();

        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);

        var dsType = attrEntity !== null ? parseInt(attrEntity.DataSourceType) : 0;
        form.find("[name=dataSourceType]").val(dsType);
        setDataSourceControlVisible(dsType);

        //loading cascading control list
        loadEntityAttributeList(function () {
            //set customized options for radiogroup
            if (dsType === 1) {
                var options = '';
                var ctrls = divCtrl.find("div").find("span");
                var checkbox = divCtrl.find("div").find("input");

                ctrls.each(function (i, o) { options += $(o).text() + '\n'; });
                form.find("[name=options]").val($.trim(options));
            } else if (dsType === 2
                || dsType === 3
                || dsType === 4
                || dsType === 5) {
                fillDataSourceItems(form, attrEntity);
            }
        });
    };

    fieldPopup.selectmultiplelist = function (ctrlType, ctrlId, attrEntity) {
        $("#pDataSourceType").show();

        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrl = divCtrl.find("select")[0];

        var dsType = attrEntity !== null ? parseInt(attrEntity.DataSourceType) : 0;
        form.find("[name=dataSourceType]").val(dsType);
        setDataSourceControlVisible(dsType);

        //loading cascading control list
        loadEntityAttributeList(function () {
            //set customized options for combobox
            if (dsType === 1) {
                var options = '';
                $(ctrl).find('option').each(function (i, o) { options += o.text + '\n'; });
                form.find("[name=options]").val($.trim(options));
            } else if (dsType === 2
                || dsType === 3
                || dsType === 4
                || dsType === 5) {
                fillDataSourceItems(form, attrEntity);
            }
        });
    };

    function fillDataSourceItems(form, attrEntity) {
        form.find("[name=dataEntityName]").val(attrEntity.DataEntityName);
        form.find("[name=dataValueField]").val(attrEntity.DataValueField);
        form.find("[name=dataTextField]").val(attrEntity.DataTextField);
        form.find("[name=cascadeControlID]").val(attrEntity.CascadeControlID);       
        form.find("[name=cascadeFieldName]").val(attrEntity.CascadeFieldName);
    }

    fieldPopup.btn = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrl = divCtrl.find("button")[0];

        form.find("[name=title]").val($(ctrl).text().trim());

        if (attrEntity !== null) {
            form.find("[name=commandText]").val(attrEntity.CommandText);
        }
        $("#tLabel").hide();
    };

    fieldPopup.text = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrlText = divCtrl.find(".ctrl-text")[0];
    };

    fieldPopup.hyperlink = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrlHyperLink = divCtrl.find(".ctrl-hyperlink")[0];

        form.find("[name=title]").val($(ctrlHyperLink).text().trim());

        if (attrEntity !== null) {
            form.find("[name=hyperlink]").val(attrEntity.Url);
        }
    };

    fieldPopup.date = function (ctrlType, ctrlId, attrEntity) {
        var form = $("#eavForm");
        var divCtrl = $("#" + ctrlId);
        var ctrlText = divCtrl.find(".ctrl-date")[0];
        form.find("[name=dataType]").val(kcommon.dataTypeDict["DATE"]);

        if (attrEntity != null) form.find("select[name=dateFormat]").val(attrEntity.Format);
    };

    fieldPopup.datatable = function (ctrlType, ctrlId, attrEntity) {
        entitydeflist.loadEntityDefList();
        $("#tLabel").hide();
    }

    function setDataSourceControlVisible(dataSourceType) {
        //1-cutomized;2-localtable;3-sql;4-storeprocedure;5-webapi-http
        hideTabs(myDataSourceTabs["customized"]);
        hideTabs(myDataSourceTabs["others"]);

        if (dataSourceType === 1) {
            showTabs(myDataSourceTabs["customized"]);
        } else {
            showTabs(myDataSourceTabs["others"]);
        }
    }

    function showTabs(tabs) {
        for (var i = tabs.length; i > 0; i--) {
            var tab = tabs[i - 1];
            $("#" + tab).parent().show();
            $("#" + tab).show();
            $("#" + tab).tab('show');
        }
    }

    function hideTabs(tabs) {
        for (var i = tabs.length; i > 0; i--) {
            var tab = tabs[i - 1];
            $("#" + tab).parent().hide();
            $("#" + tab).hide();
        }
    }

    return fieldPopup;
})();

var fieldChangeSave = (function () {
    function fieldChangeSave() {
    }

    fieldChangeSave.textbox = function (values) {
        var divCtrl = $("#" + values.forCtrl);      
        var ctrlText = divCtrl.find("input[type=text]")[0];
        ctrlText.placeholder = values.placeholder;
        ctrlText.name = values.code;
    };

    fieldChangeSave.passwordbox = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrlText = divCtrl.find("input[type=password]")[0];
        ctrlText.placeholder = values.placeholder;
        ctrlText.name = values.code;
    };

    fieldChangeSave.combobox = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrl = divCtrl.find("select")[0];
        ctrl.name = values.code;
        $(ctrl).empty();
        $(values.options.split('\n')).each(function (i, o) {
            $(ctrl).append("<option>" + $.trim(o) + "</option>");
        });

        //set cascade control change event
        var cascadeControlID = values.cascadeControlID;
        if (cascadeControlID !== "") {
            var controlName = values.code;
            $(ctrl).removeAttr("onfocus");
        }
    };

    fieldChangeSave.radiogroup = function (values) {
        $("#pDataSourceType").show();

        var divCtrl = $("#" + values.forCtrl);
        var labelTemplate = $(".selectorField .ctrl-radiogroup span")[0];
        var radioTemplate = $(".selectorField .ctrl-radiogroup input")[0];
        var ctrl = divCtrl.find(".ctrl-radiogroup");
        ctrl.empty();

        $(values.options.split('\n')).each(function (i, o) {
            var label = $(labelTemplate).clone().text($.trim(o));
            var radio = $(radioTemplate).clone();
            radio[0].name = values.code;
            radio[0].value = $.trim(o);
            label.prepend(radio);
            $(ctrl).append(label);
        });
    };

    fieldChangeSave.checkboxgroup = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var labelTemplate = $(".selectorField .ctrl-checkboxgroup span")[0];
        var checkboxTemplate = $(".selectorField .ctrl-checkboxgroup input")[0];
        var ctrl = divCtrl.find(".ctrl-checkboxgroup");
        ctrl.empty();

        $(values.options.split('\n')).each(function (i, o) {
            var label = $(labelTemplate).clone().text($.trim(o));
            var checkbox = $(checkboxTemplate).clone();
            checkbox[0].name = values.code;
            checkbox[0].value = $.trim(o);
            label.prepend(checkbox);
            $(ctrl).append(label);
        });
    };

    fieldChangeSave.selectmultiplelist = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrl = divCtrl.find("select")[0];
        ctrl.name = values.code;
        $(ctrl).empty();
        $(values.options.split('\n')).each(function (i, o) {
            $(ctrl).append("<option>" + $.trim(o) + "</option>");
        });
    };

    fieldChangeSave.btn = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrl = divCtrl.find("button")[0];
        $(ctrl).html($(ctrl).html().replace($(ctrl).text(), " " + $.trim(values.title)));
        ctrl.name = values.code;

        if (values.commandText !== '') {
            var func = "function func" + ctrl.name + "(){" + values.commandText + "}";
            $(ctrl).attr("data-onclick", func);
        }
    };

    fieldChangeSave.text = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrlText = divCtrl.find(".ctrl-text");
        ctrlText.text(values.text);
        ctrlText.attr("name", values.code);
    };

    fieldChangeSave.date = function (values) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrlText = divCtrl.find(".ctrl-date");
        ctrlText.attr("name", values.code);
    };

    fieldChangeSave.simpletext = function (values, ctrl, value) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrlText = divCtrl.find(ctrl);
        ctrlText.text(value);
    };

    fieldChangeSave.hyperlink = function (values, ctrl, value) {
        var divCtrl = $("#" + values.forCtrl);
        var ctrlHyperLink = divCtrl.find(".ctrl-hyperlink");
        ctrlHyperLink.text(values.title);
        ctrlHyperLink.attr("href", values.hyperlink);
    }

    fieldChangeSave.datatable = function (values, ctrl, value) {
        var selEntityDefID = $.trim($("#ddlEntityDef").val());

        if (selEntityDefID !== ""
            && selEntityDefID !== entitydeflist.mcurrentRefEntityDefID) {
            jshelper.ajaxGet("api/eavdata/GetAttributeEntity/" + kmaster.mcurrentAttributeID, null,
                function (r1) {
                    if (r1.Status === 1) {
                        var attrEntity = r1.Entity;
                        if (attrEntity === undefined || attrEntity === null) {
                            attrEntity = {};
                            attrEntity.EntityDefID = kmaster.mcurrentEntityDefID;
                            attrEntity.DivCtrlKey = kmaster.mcurrentControlID;
                            attrEntity.AttrName = "DataTable";

                            attrEntity.AttrDataType = 0;    //none data type
                            attrEntity.FieldInputType = 9; //data table
                            attrEntity.StorageType = 0;   //page control
                        }
                        attrEntity.RefEntityDefID = selEntityDefID;     //binding entity
                        jshelper.ajaxPost("api/eavdata/SaveAttribute", JSON.stringify(attrEntity),
                            function (result) {
                                if (result.Status === 1) {
                                    kmaster.mcurrentAttributeID = result.Entity.ID;
                                    kmaster.mcurrentRefEntityDefID = selEntityDefID;
                                    var divCtrl = $("#" + kmaster.mcurrentControlID);

                                    divCtrl.find(".hiddenAttributeEntity").attr("value", JSON.stringify(result.Entity));
                                    //refresh data table header columns
                                    var gridCtrl = divCtrl.find(".ctrl-datatable")[0];
                                    kbuilder.initEntityInfoAttrValueTable(gridCtrl, selEntityDefID);
                                } else {
                                    kmsgbox.error(kresource.getItem('bindingdatacontrolerrormsg'), result.Message);
                                }
                            });
                    } else {
                        kmsgbox.error(kresource.getItem('readingattrpropertyerrormsg'), r1.Message);
                    }
                })
        } else {
            kmsgbox.error(kresource.getItem('selectedentitydefwarningmsg'));
        }
    }
    return fieldChangeSave;
})();

var tableManager = (function () {
    function tableManager() {
    }

    tableManager.appendTable = function () {
        var isValid = true;
        if (isValid) {
            var numCols = $("#sliderNumberColumn").slider("value");
            var contentToAdd = "<div class=\"row\">";
            var largerSpan = 12 / numCols;
            for (var i = 0; i < numCols; i++) {
                contentToAdd += "<div class=\"col-md-" + largerSpan + " well droppedFields\" style=\"z-index:0;\"></div>\n";
            }
            contentToAdd += "</div>";
            $("#dialog-form-number-column").modal("hide");
            $("#selected-content").append(contentToAdd);
            kmaster.docReady();
        }
    };
    return tableManager;
})();

var eavManager = (function () {
    function eavManager() {
    }

    eavManager.EntityDef = {};
    eavManager.Attributes = [];

    eavManager.saveEntityDef = function () {
        var formValues = {};
        $("#defForm").find("input").each(function (i, o) {
            formValues[o.name] = o.value;
        });

        //check the input validation
        if (formValues["title"] === ""
            || formValues["name"] === "") {
            kmsgbox.info(kresource.getItem('formsavewarningmsg'));
            return;
        }

        var entity = {};
        entity.ID = kmaster.mcurrentEntityDefID;
        entity.EntityTitle = formValues["label"];
        entity.EntityName = formValues["name"];
        entity.Version = formValues["version"];
        entity.Description = formValues["desc"];

        if (entity.ID === 0) {
            //创建新的表单
            var defaultTemplateContent = '<div class="col-md-12 ui-sortable" id="selected-content">'
                + '<div class="row divFormTitle" id="form-title-div">'
                + '<input value=' + entity.EntityTitle + ' class="input-large col-md-12 form-control" placeholder="please input form title..." id="txtFormTitle" '
                + 'onclick="kmaster.popupEntityDef();" type="text"></div>'
                + '<div class="row"><div class="col-md-6 well droppedFields ui-droppable" style="z-index:0;"></div>'
                + '<div class="col-md-6 well droppedFields ui-droppable" style="z-index:0;"></div></div><div class="row">'
                + '<div class="col-md-12 well action-bar droppedFields ui-droppable" style="min-height:80px;z-index:0;"></div></div>'
                + '</div>';
            var defaultHTMLContent = '<div class="row"><div class="col-md-6 droppedFields" style="z-index:0;"></div>'
                + '<div class="col-md-6 droppedFields" style="z-index:0;"></div></div>'
                + '<div class="row"><div class="col-md-12 action-bar droppedFields" style="min-height:80px;z-index:0;"></div></div>';

            entity.TemplateContent = defaultTemplateContent;
            entity.HTMLContent = defaultHTMLContent;
        }


        jshelper.ajaxPost('api/eavdata/SaveEntityDef',
            JSON.stringify(entity),
            function (result) {
                if (result.Status === 1) {
                    kmsgbox.info(kresource.getItem('formsaveokmsg'));

                    var returnEntity = result.Entity;
                    kmaster.mcurrentEntityDefID = returnEntity.ID;
                    kmaster.mcurrentEntityDef = returnEntity;
                    //refresh
                    $("#selected-content").replaceWith(returnEntity.TemplateContent);
                    $("#txtFormTitle").val(entity.EntityTitle);
                    //ready
                    kmaster.docReady();
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }

    eavManager.SaveTemplateWithHTMLContent = function () {
        var templateContent = $("#selected-content")[0].outerHTML;
        var htmlContent = kmaster.generateHTMLContent();
        var entity = {
            "ID": kmaster.mcurrentEntityDefID,
            "TemplateContent": templateContent,
            "HTMLContent": htmlContent
        };
        jshelper.ajaxPost('api/eavdata/SaveTemplateWithHTMLContent',
            JSON.stringify(entity),
            function (result) {
                if (result.Status === 1) {
                    kmsgbox.info(kresource.getItem('formtemplatesaveokmsg'));
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }
    return eavManager;
})();

//获取控件输入数值, 用以保存到属性表中
//getting control value and saving into attribute table
var dfieldManagerGet = (function () {
    function dfieldManagerGet() {
    }

    //get method
    dfieldManagerGet.getControlValue = function (ctrlType, attrEntity, ctrl) {
        return dfieldManagerGet[ctrlType](ctrl, attrEntity);
    }

    dfieldManagerGet.textbox = function (ctrl, attrEntity) {
        return ctrl.val();
    }

    dfieldManagerGet.combobox = function (ctrl, attrEntity) {
        var text = $(ctrl).find("option:selected").text();
        var value = ctrl.val();

        //saving json obj into attribute info field
        var jsonObj = {};
        jsonObj[attrEntity.DataValueField] = value;
        jsonObj[attrEntity.DataTextField] = text;

        return JSON.stringify(jsonObj);
    }

    dfieldManagerGet.radiogroup = function (ctrl, attrEntity) {
        var text = $(ctrl).find("input:radio:checked").parent('span').text();
        var value = $(ctrl).find("input:radio:checked").val();

        //saving json obj into attribute info field
        var jsonObj = {};
        jsonObj[attrEntity.DataValueField] = value;
        jsonObj[attrEntity.DataTextField] = text;

        return JSON.stringify(jsonObj);
    }

    dfieldManagerGet.checkboxgroup = function (ctrl, attrEntity) {
        var jsonObjList = [];
        var ctrlList = $(ctrl).find("input:checkbox:checked");
        $.each(ctrlList, function(index, item){
            var jsonObj = {};
            jsonObj[attrEntity.DataValueField] = $(item).val();
            jsonObj[attrEntity.DataTextField] = $(item).parent('span').text();
            jsonObjList.push(jsonObj);
        });
        return JSON.stringify(jsonObjList);
    }

    dfieldManagerGet.selectmultiplelist = function (ctrl, attrEntity) {
        var jsonObjList = [];
        var ctrlList = $(ctrl).find("option:selected");
        $.each(ctrlList, function (index, item) {
            var jsonObj = {};
            jsonObj[attrEntity.DataValueField] = $(item).val();
            jsonObj[attrEntity.DataTextField] = $(item).text();
            jsonObjList.push(jsonObj);
        });
        return JSON.stringify(jsonObjList);
    }

    dfieldManagerGet.date = function (ctrl, attrEntity) {
        return ctrl.val();
    }

    dfieldManagerGet.text = function (ctrl, attrEntity) {
        return ctrl.val();
    }

    dfieldManagerGet.hyperlink = function (ctrl, attrEntity) {
        return ctrl.val();
    }

    dfieldManagerGet.datatable = function (ctrl, attrEntity) {
        console.log('get datatable data...')
        return;
    }

    dfieldManagerGet.btn = function (ctrl, attrEntity) {
        return;
    }

    dfieldManagerGet.label = function (ctrl, attrEntity) {
        return;
    }

    return dfieldManagerGet;
})();


//属性字段取值显示在界面控件上
//Set attribute value into controls on Form
var dfieldManagerRender = (function () {
    function dfieldManagerRender() {

    }

    dfieldManagerRender.setControlValue = function (hdn, attrValueList) {
        var ctrlJSONObj = $.parseJSON(hdn.value);
        var ctrlType = getFieldInputType(ctrlJSONObj.FieldInputType);
        var ctrlValue = "";

        $.each(attrValueList, function (index, obj) {
            if (obj.AttrID === ctrlJSONObj.ID) {
                ctrlValue = obj.Value;
                return false;
            }
        });

        //begin to rendering control
        if (ctrlType !== '') {
            var ctrlInput = $(hdn).parent().children(".ctrl-" + ctrlType)[0];
            dfieldManagerRender[ctrlType](ctrlInput, ctrlJSONObj, ctrlValue);
        }
    }

    dfieldManagerRender.textbox = function (ctrl, ctrlJSONObj, value) {
        $(ctrl).val(value);
    }

    dfieldManagerRender.combobox = function (ctrl, ctrlJSONObj, value) {
        if (value !== "") {
            var jsonControlValue = $.parseJSON(value);
            var optionValue = jsonControlValue[ctrlJSONObj.DataValueField];
            var optionText = jsonControlValue[ctrlJSONObj.DataTextField];

            if (ctrlJSONObj.CascadeControlID !== '') {
                $(ctrl).empty();
                $(ctrl).append("<option value='" + optionValue + "'>" + optionText + "</option>");
            } else {
                $(ctrl).val(optionValue);
            }
        }
    }

    dfieldManagerRender.radiogroup = function (ctrl, ctrlJSONObj, value) {
        if (value !== "") {
            var jsonControlValue = $.parseJSON(value);
            var optionValue = jsonControlValue[ctrlJSONObj.DataValueField];
            var optionText = jsonControlValue[ctrlJSONObj.DataTextField];

            if (ctrlJSONObj.CascadeControlID !== '') {
                $(ctrl).empty();
                $(ctrl).append('<span style="display:block;"><input type="radio" name=' + ctrlJSONObj.AttrCode + ' value = ' + optionValue + ' /> ' + optionText + '</span > ');
            }
            var radio = $(ctrl).find("input[value='" + optionValue + "']")[0];
            if (radio) radio.checked = true;
        }
    }

    dfieldManagerRender.checkboxgroup = function (ctrl, ctrlJSONObj, value) {
        if (value !== "") {
            var jsonControlValueList = $.parseJSON(value);
            $.each(jsonControlValueList, function (index, item) {
                var jsonControlValue = item;
                var optionValue = jsonControlValue[ctrlJSONObj.DataValueField];
                var optionText = jsonControlValue[ctrlJSONObj.DataTextField];

                if (ctrlJSONObj.CascadeControlID !== '') {
                    $(ctrl).empty();
                    $(ctrl).append('<span style="display:block;"><input type="checkbox" name=' + ctrlJSONObj.AttrCode + ' value = ' + optionValue + ' /> ' + optionText + '</span > ');
                } 
                var checkbox = $(ctrl).find("input[value='" + optionValue + "']")[0];
                if (checkbox) checkbox.checked = true;
            });
        }
    }

    dfieldManagerRender.selectmultiplelist = function (ctrl, ctrlJSONObj, value) {
        if (value !== "") {
            var jsonControlValueList = $.parseJSON(value);
            $.each(jsonControlValueList, function (index, item) {
                var jsonControlValue = item;
                var optionValue = jsonControlValue[ctrlJSONObj.DataValueField];
                var optionText = jsonControlValue[ctrlJSONObj.DataTextField];

                if (ctrlJSONObj.CascadeControlID !== '') {
                    $(ctrl).empty();
                    $(ctrl).append("<option value='" + optionValue + "'>" + optionText + "</option>");
                } 
                var checkedItem = $(ctrl).find("option[value='" + optionValue + "']")[0];
                if (checkedItem) $(checkedItem).prop('selected', true);
            });
        }
    }

    dfieldManagerRender.date = function (ctrl, ctrlJSONObj, value) {
        $(ctrl).val(value);
        $(ctrl).datepicker('update', new Date(value));
    }

    dfieldManagerRender.text = function (ctrl, ctrlJSONObj, value) {
        $(ctrl).val(value);
    }

    dfieldManagerRender.hyperlink = function (ctrl, ctrlJSONObj, value) {
        ;
    }

    dfieldManagerRender.button = function (ctrl, ctrlJSONObj, value) {
        ;
    }

    dfieldManagerRender.label = function (ctrl, ctrlJSONObj, value) {
        ;
    }

    dfieldManagerRender.datatable = function (ctrl, ctrlJSONObj, value) {
        var refEntityDefID = ctrlJSONObj.RefEntityDefID;
        kentityinfo.initEntityInfoAttrValueTable(ctrl, refEntityDefID);
    }

    function getFieldInputType(inputType) {
        var ctrlType = "";
        if (inputType === 1)
            ctrlType = "textbox";
        else if (inputType === 2)
            ctrlType = "password";
        else if (inputType === 3)
            ctrlType = "combobox";
        else if (inputType === 4)
            ctrlType = "checkboxgroup";
        else if (inputType === 5)
            ctrlType = "radiogroup";
        else if (inputType === 6)
            ctrlType = "selectmultiplelist";
        else if (inputType === 7)
            ctrlType = "date";
        else if (inputType === 8)
            ctrlType = "text";
        else if (inputType === 9)
            ctrlType = "label";
        else if (inputType === 12)
            ctrlType = "hyperlink";
        else if (inputType === 15)
            ctrlType = "datatable";
        else if (inputType === 16)
            ctrlType = "button";
        else if (inputType === 17)
            ctrlType = "image";
        else
            kmsgbox.warn(kresource.getItem("fieldManagerunknowncontroltypeerrormsg"));
        return ctrlType;
    }
    return dfieldManagerRender;
})();