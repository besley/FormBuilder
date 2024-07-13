var attributelist = (function () {
    attributelist.mselectedAttributeID = 0;
    attributelist.mselectedAttributeRow = null;
    attributelist.pselectedAttributeEntity = null;
    attributelist.mselectedAttributeEventID = 0;

    function attributelist() {
    }

    attributelist.load = function () {
        if (kmaster.mcurrentEntityDefID === 0) {
            return;
        }

        $('#loading-indicator').show();
        jshelper.ajaxGet("api/eavdata/GetEntityAttributeList/" + kmaster.mcurrentEntityDefID, null, function (result) {
            if (result.Status === 1) {
                var divAttrListGrid = document.querySelector("#myAttributeGrid");
                $(divAttrListGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'ID', width: 40 },
                        { headerName: kresource.getItem('fieldname'), field: 'AttrName', width: 120 },
                        { headerName: kresource.getItem('fieldcode'), field: 'AttrCode', width: 160 },
                        { headerName: kresource.getItem('datatype'), field: 'AttrDataType', width: 120, cellRenderer: onDataTypeCellRenderer },
                        { headerName: kresource.getItem('controltype'), field: 'FieldInputType', width: 120, cellRenderer: onFieldInputCellRenderer },
                        { headerName: kresource.getItem('description'), field: 'Description', width: 120 }
                    ],
                    rowSelection: 'single',
                    onSelectionChanged: onSelectionChanged,
                };

                new agGrid.Grid(divAttrListGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                function onSelectionChanged() {
                    var selectedRows = gridOptions.api.getSelectedRows();
                    selectedRows.forEach(function (selectedRow, index) {
                        attributelist.mselectedAttributeID = selectedRow.ID;
                        attributelist.mselectedAttributeRow = selectedRow;
                    });
                }

                function onDataTypeCellRenderer(params) {
                    var text = kcommon.getDataTypeByID(params.value);
                    return kresource.getItem(text);
                }

                function onFieldInputCellRenderer(params) {
                    var text = kcommon.getInputTypeByID(params.value);
                    return kresource.getItem(text);
                }

                $('#loading-indicator').hide();
            } else {
                kmsgbox.error(kresource.getItem('fieldlistreaderrormsg'), result.Message);
            }
        });
    }

    attributelist.loadEvent = function (entityDefID, attrID) {
        var query = {};
        query.EntityDefID = entityDefID;
        query.AttrID = attrID;

        $('#loading-indicator').show();
        jshelper.ajaxPost("api/FBMaster/GetEntityAttributeEventList", JSON.stringify(query), function (result) {
            if (result.Status === 1) {
                var divBindingEventGrid = document.querySelector("#myBindingEventGrid");
                $(divBindingEventGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'ID', width: 40 },
                        { headerName: kresource.getItem('eventname'), field: 'EventName', width: 160 },
                        { headerName: kresource.getItem('isdisabled'), field: 'IsDisabled', width: 120 },
                    ],
                    rowSelection: 'single',
                    onSelectionChanged: onSelectionChanged,
                };

                new agGrid.Grid(divBindingEventGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                function onSelectionChanged() {
                    var selectedRows = gridOptions.api.getSelectedRows();
                    selectedRows.forEach(function (selectedRow, index) {
                        attributelist.mselectedAttributeEventID = selectedRow.ID;
                        attributelist.mselectedAttributeEventRow = selectedRow;
                    });
                    displayEvent(attributelist.mselectedAttributeEventRow);
                }

                $('#loading-indicator').hide();
            } else {
                kmsgbox.error(kresource.getItem('fieldlistreaderrormsg'), result.Message);
            }
        });
    }

    function displayEvent(eventRow) {
        $("#ddlHtmlEventName").val(eventRow.EventName);
        $("#txtHtmlCommandText").val(eventRow.CommandText);
        $("#chkIsEventDisabled").attr('checked', eventRow.IsDisabled === 1);
    }

    attributelist.saveEvent = function () {
        var eventName = $("#ddlHtmlEventName").val();
        var commandText = $("#txtHtmlCommandText").val();
        var isDisabled = $("#chkIsEventDisabled").is(":checked") === true ? 1 : 0;

        var entity = {};
        var attrEntity = attributelist.pselectedAttributeEntity;
        if (attrEntity !== null) {
            entity.EntityDefID = attrEntity.EntityDefID;
            entity.AttrID = attrEntity.ID;
            entity.EventName = eventName;
            entity.IsDisabled = isDisabled;
            entity.CommandText = commandText;

            if (attributelist.mselectedAttributeEventRow !== undefined) {
                entity.ID = attributelist.mselectedAttributeEventRow.ID;
            }

            if (entity.EventName !== "default") {
                jshelper.ajaxPost("api/FBMaster/SaveAttributeEvent", JSON.stringify(entity), function (result) {
                    if (result.Status === 1) {
                        attributelist.loadEvent(attrEntity.EntityDefID, attrEntity.ID);
                    } else {
                        kmsgbox.error(kresource.getItem('attributelist.addEvent.error'), result.Message);
                    }
                });
            } else {
                kmsgbox.warn(kresource.getItem('attributelist.addEvent.invalideventname.warning'));
            }
        } else {
            kmsgbox.warn(kresource.getItem('attributelist.addEvent.none.warning'));
        }
    }

    attributelist.delEvent = function () {
        var eventID = attributelist.mselectedAttributeEventID;
        if (eventID !== 0) {
            kmsgbox.confirm(kresource.getItem('attributelist.delEvent.confirm'), function (result) {
                if (result === "Yes") {
                    jshelper.ajaxGet("api/FBMaster/DeleteAttributeEvent/" + eventID, null, function (result) {
                        if (result.Status === 1) {
                            var entity = attributelist.pselectedAttributeEntity;
                            attributelist.loadEvent(entity.EntityDefID, entity.ID);
                        } else {
                            kmsgbox.error(kresource.getItem('attributelist.addEvent.error'), result.Message)
                        }
                    });
                }
            });
        }
    }
    return attributelist;
})();