var attributelist = (function () {
    attributelist.mselectedAttributeID = 0;

    function attributelist() {
    }

    attributelist.load = function () {
        if (kmaster.mcurrentEntityDefID == 0) {
            return;
        }

        jshelper.ajaxGet("api/eavdata/GetEntityAttributeList/" + kmaster.mcurrentEntityDefID, null, function (result) {
            if (result.Status == 1) {
                var columnAttr = [
                    { id: "ID", name: "ID", field: "ID", width: 40, cssClass: "bg-gray" },
                    { id: "AttrName", name: "字段名称", field: "AttrName", width: 120, cssClass: "bg-gray" },
                    { id: "AttrCode", name: "字段编码", field: "AttrCode", width: 160, cssClass: "bg-gray" },
                    { id: "AttrDataType", name: "数据类型", field: "AttrDataType", width: 160, cssClass: "bg-gray" },
                    { id: "FieldInputType", name: "控件类型", field: "FieldInputType", width: 120, cssClass: "bg-gray" },
                    { id: "Description", name: "描述", field: "Description", width: 120, cssClass: "bg-gray" },
                ];

                var optionsAttr = {
                    editable: true,
                    enableCellNavigation: true,
                    enableColumnReorder: true,
                    asyncEditorLoading: true,
                    forceFitColumns: false,
                    topPanelHeight: 25
                };

                var dsAttr = result.Entity;
                var dvAttr = new Slick.Data.DataView({ inlineFilters: true });
                var gridAttr = new Slick.Grid("#myAttributeGrid", dvAttr, columnAttr, optionsAttr);

                dvAttr.onRowsChanged.subscribe(function (e, args) {
                    gridAttr.invalidateRows(args.rows);
                    gridAttr.render();

                });

                dvAttr.onRowCountChanged.subscribe(function (e, args) {
                    gridAttr.updateRowCount();
                    gridAttr.render();
                });

                dvAttr.beginUpdate();
                dvAttr.setItems(dsAttr, "ID");
                gridAttr.setSelectionModel(new Slick.RowSelectionModel());
                dvAttr.endUpdate();

                gridAttr.onSelectedRowsChanged.subscribe(function (e, args) {
                    var selectionRowIndex = args.rows[0];
                    var row = dvAttr.getItemByIdx(selectionRowIndex);
                    attributelist.mselectedAttributeID = row.ID;
                });
            };
        });
    }
    return attributelist;
})();