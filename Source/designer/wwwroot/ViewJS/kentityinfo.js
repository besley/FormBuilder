var kentityinfo = (function () {
    function kentityinfo() {
    }

    kentityinfo.initEntityInfoAttrValueTable = function (gridCtrl, entityDefID) {
        jshelper.ajaxGet("../../api/FBMaster/GetEntityAttributeListOnlyInfoValue/" + entityDefID, null, function (result) {
            if (result.Status === 1) {
                var attrList = result.Entity;
                var query = { "EntityDefID": entityDefID };
                
                jshelper.ajaxPost("../../api/FBData/GetEntityInfoWithAttrValueList", JSON.stringify(query), function (r) {
                    if (r.Status === 1) {
                        var valueList = r.Entity;

                        pivotEntityInfoToDataGrid(gridCtrl, attrList, valueList);
                    } else {
                        kmsgbox.error(r.Message);
                    }
                });
            } else {
                kmsgbox.error(result.Message);
            }
        });
    }

    function pivotEntityInfoToDataGrid(gridCtrl, attrList, valueList) {
        $(gridCtrl).empty();

        var columnsDef = [];
        columnsDef.push({ headerName: "ID", field: "ID", width: 50 });
        columnsDef.push({ headerName: kresource.getItem('formtitle'), field: "EntityTitle", width: 200 });
        
        //填充属性标题列（通过行转置而来）
        $.each(attrList, function (i, o) {
            var column = { headerName: o.AttrName, field: o.AttrCode, width: 200 };
            columnsDef.push(column);
        });

        var gridOptions = {
            columnDefs: columnsDef,
            rowSelection: 'single',
            onSelectionChanged: onSelectionChanged
        };

        new agGrid.Grid(gridCtrl, gridOptions);
        gridOptions.api.setRowData(valueList);

        $('#loading-indicator').hide();

        function onSelectionChanged() {
            var selectedRows = gridOptions.api.getSelectedRows();
            selectedRows.forEach(function (selectedRow, index) {
                kflowmain.mcurrentEntityInfoID = selectedRow.ID;
                //fill data into form
                fillDataIntoDynamicForm(selectedRow.ID);
                //get task list
                processlist.getTaskList();
                processlist.getDoneList();
            });
        }
    }

    function getAttrSchema(key, attrList) {
        var attr = null;
        $.each(attrList, function (i, o) {
            if (key === o.AttrCode) {
                attr = o;
                return;
            }
        });
        return attr;
    }

    var fillDataIntoDynamicForm = function (entityInfoID) {
        //打开表单
        var query1 = { "ID": kflowmain.mcurrentEntityDefID };
        jshelper.ajaxPost("../../api/FBMaster/QueryEntityDef",
            JSON.stringify(query1), function (result) {
                if (result.Status === 1) {
                    //填充表单数据
                    kflowmain.mcurrentEntityInfoID = entityInfoID;
                    var query2 = { "ID": kflowmain.mcurrentEntityInfoID };
                    jshelper.ajaxPost("../../api/FBData/QueryEntityAttrValue",
                        JSON.stringify(query2), function (result) {
                            if (result.Status === 1) {
                                var attrValueList = result.Entity;
                                //填充字段值
                                fillAttrValueData(attrValueList);
                            } else {
                                kmsgbox.error(kresource.getItem('formfieldreaderrormsg'), result.Message);
                            }
                        });
                } else {
                    kmsgbox.error(kresource.getItem('formdefinitionreaderrormsg'), result.Message);
                }
            });
    }

    kentityinfo.checkAttrInActivityFormVisibleReadOnly = function (taskID) {
        var entityInfoID = kflowmain.mcurrentEntityInfoID;
        var runner = {
            "ProcessGUID": kflowmain.mcurrentProcessGUID,
            "Version": kflowmain.mcurrentProcessVersion,
            "AppInstanceID": entityInfoID.toString(),
            "TaskID": taskID
        };

        jshelper.ajaxPost("../../api/FBData/QueryAttrActivityPermission",
            JSON.stringify(runner), function (result) {
                if (result.Status === 1) {
                    var editList = result.Entity;
                    bindingFieldOfAttributePermission(editList);
                } else {
                    kmsgbox.error(kresource.getItem('formfieldpermissionreaderrormsg'), result.Message);
                }
            });
    }

    function bindingFieldOfAttributePermission(editList) {
        $.each(editList, function (i, attr) {
            var attrID = attr.AttrID;
            $(".hiddenAttributeEntity").each(function (i, ele) {
                var attrObj = JSON.parse(ele.value);
                if (attrObj.ID === attrID) {
                    if (attr.IsReadOnly === 1) {
                        $(ele).parent().children().attr('readonly', true);
                    }
                    if (attr.IsNotVisible === 1) {
                        $(ele).parent().hide();         //hide div element
                    }
                    return;
                }
            });
        })
    }

    function fillAttrValueData(attrValueList) {
        var controlList = $("#dynamicFormContainer").find(".hiddenAttributeEntity");
        $.each(controlList, function (i, o) {
            dfieldManagerRender.setControlValue(o, attrValueList);
        });
    }

    kentityinfo.deleteEntityInfo = function (query, callback) {
        jshelper.ajaxPost('../../api/FBData/DeleteEntityInfo',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });   
    }

    return kentityinfo;
})()