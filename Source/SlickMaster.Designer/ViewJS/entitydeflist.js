var entitydeflist = (function () {
    entitydeflist.mselectedEntityDefID = 0;
    entitydeflist.mselectedEntityDefRow = null;

    function entitydeflist() {
    }

    entitydeflist.load = function () {
        jshelper.ajaxGet("/smd/api/eavdata/GetEntityDefList2", null, function (result) {
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
        jshelper.ajaxGet("/smd/api/eavdata/GetEntityDefByID/" + kmaster.mcurrentEntityDefID, null,
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
                    jshelper.ajaxGet("/smd/api/eavdata/DeleteEntityDef/" + entitydeflist.mselectedEntityDefID,
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

    return entitydeflist;
})();