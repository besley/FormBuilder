var setting = (function () {
    function setting() {
    }

    setting.mcurrentEntityDefID = 0;
    setting.mcurrentProcessID = 0;
    setting.mcurrentActivityGUID = "";
    setting.mcurrentSelectedRow = null;
    setting.mchangedSelectedRows = [];
    setting.mcurrentSelectedNotVisible = false;
    setting.mcurrentSelectedReadOnly = false;
    setting.mcurrentSelectedFieldType = "";

    setting.loadProcess = function (entityDefID) {
        $('#loading-indicator').show();

        //判断当前表单ID是否有效
        setting.mcurrentEntityDefID = entityDefID;
        if (setting.mcurrentEntityDefID === 0) {
            kmsgbox.warn(kresource.getItem('formbindingprocessnoselectedwarningmsg'));
            $("#modalSettingForm").modal("hide");
            return false;
        }

        jshelper.ajaxGet("../../api/eavprocess/GetEntityProcessView/" + setting.mcurrentEntityDefID, null,
            function (result) {
                if (result.Status === 1) {
                    var processList = result.Entity.ProcessList;
                    var entityProcess = result.Entity.EntityProcess;

                    $.each(processList, function (i, process) {
                        $('#ddlProcesses').append($('<option>', {
                            value: process.ID,
                            text: process.ProcessName
                        }));
                    });

                    if (entityProcess) {
                        $('#ddlProcesses').val(entityProcess.ProcessID);
                        setting.mcurrentProcessID = entityProcess.ProcessID;
                        loadActivityList(entityProcess.ProcessID);
                    } else {
                        setting.mcurrentProcessID = 0;
                        fillActivityGrid();
                    }

                    $('#loading-indicator').hide();
                } else {
                    kmsgbox.error(kresource.getItem('formbindingprocessreaderrormsg'), result.Message);
                }
        })
    };

    setting.bindProcess = function () {
        //判断当前表单ID是否有效
        if (setting.mcurrentEntityDefID === 0) {
            kmsgbox.warn(kresource.getItem('formbindingprocessselectedformwarningmsg'));
            $("#modalSettingForm").modal("hide");
            return false;
        }

        //先判断表单有没有绑定流程，如果绑定则给出提示确认，然后重新绑定
        var selProcessID = parseInt($("#ddlProcesses").val());
        if (selProcessID !== 0
            && selProcessID !== setting.mcurrentProcessID) {

            kmsgbox.confirm(kresource.getItem('formbindingprocessconfirmmsg'), function (result) {
                if (result === "Yes") {
                    var entityProcessView = {};
                    entityProcessView.ID = setting.mcurrentEntityDefID;
                    entityProcessView.ProcessID = selProcessID;

                    jshelper.ajaxPost("../../api/eavprocess/BindEntityProcess", JSON.stringify(entityProcessView),
                        function (result) {
                            if (result.Status === 1) {
                                kmsgbox.info(kresource.getItem('formbindingprocessokmsg'));
                                setting.mcurrentProcessID = selProcessID;
                                loadActivityList(selProcessID);
                            } else {
                                kmsgbox.error(kresource.getItem('formbindingprocesserrormsg'), result.Message);
                            }
                        });
                }
            });
        } else {
            kmsgbox.warn(kresource.getItem('formbindingprocessinvalidwarningmsg'));
        }
    }

    setting.unbindProcess = function () {
        //判断当前表单ID是否有效
        if (setting.mcurrentEntityDefID === 0) {
            kmsgbox.warn(kresource.getItem('formbindingprocessselectedformwarningmsg'));
            $("#modalSettingForm").modal("hide");
            return false;
        }

        kmsgbox.confirm(kresource.getItem('formunbindingprocessconfirmmsg'), function (result) {
            if (result === "Yes") {
                var entityProcessView = {};
                entityProcessView.EntityDefID = setting.mcurrentEntityDefID;

                jshelper.ajaxPost("../../api/eavprocess/UnbindEntityProcess", JSON.stringify(entityProcessView),
                    function (result) {
                        if (result.Status === 1) {
                            kmsgbox.info(kresource.getItem('formbindingprocessokmsg'));
                            unloadActivityList();
                        } else {
                            kmsgbox.error(kresource.getItem('formbindingprocesserrormsg'), result.Message);
                        }
                    });
            }
        });
    }

    var loadActivityList = function (processID) {
        jshelper.ajaxGet("../../api/eavprocess/GetActivityList/" + processID, null,
            function (result) {
                if (result.Status === 1) {
                    fillActivityGrid(result.Entity, processID);
                } else {
                    kmsgbox.error(kresource.getItem('activitylistreaderrormsg'), result.Message);
                }
            })
    }

    var unloadActivityList = function () {
        $("#ddlProcesses").val(0);
        fillActivityGrid(null, 0);
    }

    var fillActivityGrid = function (entity, processID) {
        var divTaskActivityGrid = document.querySelector('#myTaskActivityGrid');
        $(divTaskActivityGrid).empty();

        var gridOptions = {
            columnDefs: [
                { headerName: kresource.getItem('activityguid'), field: 'ActivityGUID', width: 120 },
                { headerName: kresource.getItem('activityname'), field: 'ActivityName', width: 180 },
                { headerName: kresource.getItem('activitycode'), field: 'ActivityCode', width: 100 },
            ],
            rowSelection: 'single',
            onRowClicked: onRowClicked,
        };

        new agGrid.Grid(divTaskActivityGrid, gridOptions);
        gridOptions.api.setRowData(entity);

        function onRowClicked(e, args) {
            var activityGUID = setting.mcurrentActivityGUID = e.data.ActivityGUID;
            queryEntityAttrActivityEditList(setting.mcurrentEntityDefID, processID, activityGUID);
        }
    }

    setting.onNotVisibleClick = function (e) {
        if (e.checked === true)
            setting.mcurrentSelectedNotVisible = true;
        else
            setting.mcurrentSelectedNotVisible = false;
        setting.mcurrentSelectedFieldType = "IsNotVisible";
    }

    setting.onReadOnlyClick = function (e) {
        if (e.checked === true)
            setting.mcurrentSelectedReadOnly = true;
        else
            setting.mcurrentSelectedReadOnly = false;

        setting.mcurrentSelectedFieldType = "IsReadOnly";
    }

    var queryEntityAttrActivityEditList = function (entityDefID, processID, activityGUID) {
        setting.mchangedSelectedRows = [];

        var query = {};
        query.EntityDefID = entityDefID;
        query.ProcessID = processID;
        query.ActivityGUID = activityGUID;

        jshelper.ajaxPost("../../api/FBMaster/QueryEntityAttrActivityEditList", JSON.stringify(query),
            function (result) {
                if (result.Status === 1) {
                    var divAttrActivityEditGrid = document.querySelector('#myAttrActivityEditGrid');
                    $(divAttrActivityEditGrid).empty();

                    var gridOptions = {
                        columnDefs: [
                            { headerName: kresource.getItem('fieldid'), field: 'AttrID', width: 60 },
                            { headerName: kresource.getItem('fieldname'), field: 'AttrName', width: 160 },
                            {
                                headerName: kresource.getItem('fieldhidden'), field: 'IsNotVisible', width: 80, cellRenderer: function (params) {
                                    if (params.value === 1)
                                        return "<input type='checkbox' onclick='setting.onNotVisibleClick(this);' checked='checked' />"; 
                                    else
                                        return "<input type='checkbox' onclick='setting.onNotVisibleClick(this);'/>";
                                }
                            },
                            {
                                headerName: kresource.getItem('fieldreadonly'), field: 'IsReadOnly', width: 80, cellRenderer: function (params) {
                                    if (params.value === 1)
                                        return "<input type='checkbox'  onclick='setting.onReadOnlyClick(this);' checked='checked' />";
                                    else
                                        return "<input type='checkbox' onclick='setting.onReadOnlyClick(this);'/>";
                                },
                            },
                        ],
                        rowSelection: 'single',
                        onRowClicked: onRowClicked,
                    };

                    new agGrid.Grid(divAttrActivityEditGrid, gridOptions);
                    gridOptions.api.setRowData(result.Entity);

                    function onRowClicked(e, args) {
                        var selectedRow = setting.mcurrentSelectedRow = e.data;

                        var index = setting.mchangedSelectedRows.findIndex(o => o.AttrID === selectedRow.AttrID);
                        if (index === -1) {
                            setting.mchangedSelectedRows.push(selectedRow);
                        }

                        var index2 = setting.mchangedSelectedRows.findIndex(o => o.AttrID === selectedRow.AttrID);
                        if (index2 >= 0) {
                            if (setting.mcurrentSelectedFieldType === "IsNotVisible") {
                                setting.mchangedSelectedRows[index2]["IsNotVisible"] = setting.mcurrentSelectedNotVisible ? 1 : 0;
                            } else if (setting.mcurrentSelectedFieldType === "IsReadOnly") {
                                setting.mchangedSelectedRows[index2]["IsReadOnly"] = setting.mcurrentSelectedReadOnly ? 1 : 0;
                            }
                        }
                    }
                } else {
                    kmsgbox.error(kresource.getItem('activityfieldpermissionlistreaderrormsg'), result.Message);
                }
            })
    }

    setting.saveAttribute = function () {
        if (setting.mchangedSelectedRows.length === 0)
            return false;

        var comp = {};
        comp.EntityDefID = setting.mcurrentEntityDefID;
        comp.ProcessID = setting.mcurrentProcessID;
        comp.ActivityGUID = setting.mcurrentActivityGUID;
        comp.AttrEditList = setting.mchangedSelectedRows;

        jshelper.ajaxPost("../../api/FBMaster/SaveEntityAttrActivityEditList", JSON.stringify(comp),
            function (result) {
                if (result.Status === 1) {
                    kmsgbox.info(kresource.getItem('activityfieldpermissionsaveokmsg'));
                } else {
                    kmsgbox.error(kresource.getItem('activityfieldpermissionsaveerrormsg'), result.Message);
                }
            });
    }
    return setting;
})();