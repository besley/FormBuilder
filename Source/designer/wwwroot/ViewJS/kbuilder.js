var kbuilder = (function () {
    function kbuilder() {
    }

    kbuilder.initEntityInfoAttrValueTable = function (gridCtrl, entityDefID) {
        jshelper.ajaxGet("api/FBMaster/GetEntityAttributeList/" + entityDefID, null, function (result) {
            if (result.Status === 1) {
                var attrList = result.Entity;
                var query = { "EntityDefID": entityDefID };

                jshelper.ajaxPost("api/FBData/GetEntityInfoWithAttrValueList", JSON.stringify(query), function (r) {
                    if (r.Status === 1) {
                        fillDataIntoTable(gridCtrl, attrList);
                    } else {
                        kmsgbox.error(r.Message);
                    }
                });
            } else {
                kmsgbox.error(result.Message);
            }
        });

    }

    function fillDataIntoTable(gridCtrl, attrList) {
        $(gridCtrl).empty();

        var columnsDef = [];
        columnsDef.push({ headerName: "ID", field: "ID", width: 50 });
        //columnsDef.push({ headerName: "", field: "ID", width: 50 });

        $.each(attrList, function (i, o) {
            var column = { headerName: o.AttrName, field: o.AttrCode, width: 100 };
            columnsDef.push(column);
        });

        var gridOptions = {
            columnDefs: columnsDef,
            rowSelection: 'single',
            //onSelectionChanged: onSelectionChanged
        };
        new agGrid.Grid(gridCtrl, gridOptions);
    }

    return kbuilder;
})()