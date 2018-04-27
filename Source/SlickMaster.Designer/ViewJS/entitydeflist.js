var entitydeflist = (function () {
    entitydeflist.mselectedEntityDefID = 0;
    entitydeflist.mselectedEntityDefRow = null;

    function entitydeflist() {
    }

    entitydeflist.load = function () {
    	$('#loading-indicator').show();

        jshelper.ajaxGet("api/eavdata/GetEntityDefList2", null, function (result) {
            if (result.Status == 1) {
                var columnEntityDef = [
                    { id: "ID", name: "ID", field: "ID", width: 40, cssClass: "bg-gray" },
                    { id: "EntityTitle", name: "标题", field: "EntityTitle", width: 120, cssClass: "bg-gray" },
                    { id: "EntityName", name: "表单名称", field: "EntityName", width: 120, cssClass: "bg-gray" },
                    { id: "EntityCode", name: "表单编码", field: "EntityCode", width: 160, cssClass: "bg-gray" },
                    { id: "Description", name: "描述", field: "Description", width: 120, cssClass: "bg-gray" },
                    { id: "CreatedDate", name: "创建时间", field: "CreatedDate", width: 200, cssClass: "bg-gray", formatter: datetimeFormatter }
                ];

                var optionsEntityDef = {
                    editable: true,
                    enableCellNavigation: true,
                    enableColumnReorder: true,
                    asyncEditorLoading: true,
                    forceFitColumns: false,
                    topPanelHeight: 25
                };

                var dsEntityDef = result.Entity;
                var dvEntityDef = new Slick.Data.DataView({ inlineFilters: true });
                var gridEntityDef = new Slick.Grid("#myEntityDefGrid", dvEntityDef, columnEntityDef, optionsEntityDef);

                dvEntityDef.onRowsChanged.subscribe(function (e, args) {
                    gridEntityDef.invalidateRows(args.rows);
                    gridEntityDef.render();

                });

                dvEntityDef.onRowCountChanged.subscribe(function (e, args) {
                    gridEntityDef.updateRowCount();
                    gridEntityDef.render();
                });

                dvEntityDef.beginUpdate();
                dvEntityDef.setItems(dsEntityDef, "ID");
                gridEntityDef.setSelectionModel(new Slick.RowSelectionModel());
                dvEntityDef.endUpdate();
                
                gridEntityDef.onSelectedRowsChanged.subscribe(function (e, args) {
                    var selectionRowIndex = args.rows[0];
                    var row = dvEntityDef.getItemByIdx(selectionRowIndex);

                    if (row) {
                        entitydeflist.mselectedEntityDefID = row.ID;
                        entitydeflist.mselectedEntityDefRow = row;
                    }
                });

                $('#loading-indicator').hide();
            } else {
                $.msgBox({
                    title: "Master / Entity",
                    content: "读取表单定义记录失败！错误信息：" + result.Message,
                    type: "error"
                });
            }
        });

        function datetimeFormatter(row, cell, value, columnDef, dataContext) {
            if (value != null && value != "") {
                return value.substring(0, 10);
            }
        }
    }

    entitydeflist.sure = function () {
        kmaster.mcurrentEntityDefID = entitydeflist.mselectedEntityDefID;
        if (kmaster.mcurrentEntityDefID == 0) return false;

        $("#modelEntityDefListForm").modal("hide");
        //load entitydef and attribute list
        jshelper.ajaxGet("api/eavdata/GetEntityDefByID/" + kmaster.mcurrentEntityDefID, null,
            function (result) {
                if (result.Status == 1) {
                    var entityDef = result.Entity;

                    if (entityDef.TemplateContent != null
                            && entityDef.TemplateContent != "") {
                        $("#selected-content").replaceWith(entityDef.TemplateContent);
                    } else {
                        $.msgBox({
                            title: "Master / Entity",
                            content: "表单模板内容为空，请每次设计后并保存模板内容！",
                            type: "alert"
                        });
                    }
                    kmaster.docReady();

                    $("#txtFormTitle").attr("value", entitydeflist.mselectedEntityDefRow.EntityTitle);
                } else {
                    $.msgBox({
                        title: "Ooops",
                        content: result.Message,
                        type: "error",
                        buttons: [{ value: "Ok" }],
                    });
                }
            });
    }

    entitydeflist.delete = function () {
        if (entitydeflist.mselectedEntityDefID == 0) {
            return;
        }
        $.msgBox({
            title: "Are You Sure",
            content: "确定要删除表单定义记录吗? 这将会删除表单实例数据！",
            type: "confirm",
            buttons: [{ value: "Yes" }, { value: "Cancel" }],
            success: function (result) {
                if (result == "Yes") {
                    jshelper.ajaxGet("api/eavdata/DeleteEntityDef/" + entitydeflist.mselectedEntityDefID,
                        null, function (result) {
                        if (result.Status == 1) {
                            $.msgBox({
                                title: "Master / Entity",
                                content: "表单定义记录已经删除！",
                                type: "info"
                            });
                            entitydeflist.mselectedEntityDefID = 0;

                            //refresh
                            entitydeflist.load();
                        } else {
                            $.msgBox({
                                title: "Master / Entity",
                                content: "表单定义记录删除失败！错误信息：" + result.Message,
                                type: "error"
                            });
                        }
                    });
                }
            }
        });
    }

    entitydeflist.loadEntityDefList = function () {
        //判断当前表单ID是否有效
        if (kmaster.mcurrentAttributeID == "") {
            $.msgBox({
                title: "Designer / Field",
                content: "请先选定数据控件，然后再绑定数据源！",
                type: "alert"
            });
            $("#modalEntityBindingForm").modal("hide");
            return false;
        }

        var ctrlId = kmaster.mcurrentControlID;
        var divCtrl = $("#" + ctrlId);
        var attrEntity = fieldPopup.getHiddenAttribute(divCtrl);

        if (attrEntity) {
            kmaster.mcurrentAttributeID = attrEntity.ID;
        } else {
            kmaster.mcurrentAttributeID = 0;
        }

        jshelper.ajaxGet("api/eavdata/GetAttributeEntityView/" + kmaster.mcurrentAttributeID, null,
            function (result) {
                if (result.Status == 1) {
                    var entityDefList = result.Entity.EntityDefList;
                    attrEntity = result.Entity.AttributeEntity;

                    $.each(entityDefList, function (i, def) {
                        $('#ddlEntityDef').append($('<option>', {
                            value: def.ID,
                            text: def.EntityName
                        }));
                    });

                    if (attrEntity) {
                        $('#ddlEntityDef').val(attrEntity.RefEntityDefID);
                    }
                } else {
                    $.msgBox({
                        title: "Designer / Field",
                        content: "数据控件的数据源失败！错误信息：" + result.Message,
                        type: "error"
                    });
                }
            })
    }

    entitydeflist.saveEntityBinding = function () {
        var selEntityDefID = $.trim($("#ddlEntityDef").val());

        if (selEntityDefID !== ""
            && selEntityDefID !== entitydeflist.mcurrentRefEntityDefID) {
            jshelper.ajaxGet("api/eavdata/GetAttributeEntity/" + kmaster.mcurrentAttributeID, null,
                function (r1) {
                    if (r1.Status == 1) {
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
                                    $.msgBox({
                                        title: "Designer / Attribute",
                                        content: "数据控件绑定信息已经保存！",
                                        type: "info"
                                    });
                                } else {
                                    $.msgBox({
                                        title: "Designer / Attribute",
                                        content: "数据控件绑定失败！错误信息：" + result.Message,
                                        type: "error"
                                    });
                                }
                            });
                    } else {
                        $.msgBox({
                            title: "Designer / Field",
                            content: "字段属性读取失败！错误信息：" + r1.Message,
                            type: "error"
                        });
                    }
                })
        } else {
            $.msgBox({
                title: "Designer / Field",
                content: "请选择实体列表!",
                type: "error"
            });
        }
    }
    return entitydeflist;
})();