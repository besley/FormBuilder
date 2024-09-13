
const kprocessbind = (function () {
    function kprocessbind() {
    }

    kprocessbind.mcurrentFormID = 0;
    kprocessbind.mcurrentProcessID = 0;

    kprocessbind.loadProcess = function (formID) {
        $('#loading-indicator').show();

        //判断当前表单ID是否有效
        kprocessbind.mcurrentFormID = formID;
        if (kprocessbind.mcurrentFormID === 0) {
            kmsgbox.warn(kresource.getItem('Process.onFormBindingProcess.null.warn'));
            return false;
        }

        jshelper.ajaxGet(kconfig.webApiUrl + "api/FormProcess/GetFormProcessView/" + kprocessbind.mcurrentFormID, null,
            function (result) {
                if (result.Status === 1) {
                    var processList = result.Entity.ProcessList;
                    var formProcessList = result.Entity.FormProcessList;

                    $.each(processList, function (i, process) {
                        $('#ddlProcesses').append($('<option>', {
                            value: process.ID,
                            text: process.ProcessName
                        }));
                    });

                    if (formProcessList) {
                        displayBindedProcessInfo(formProcessList);
                    }

                    $('#loading-indicator').hide();
                } else {
                    kmsgbox.error(kresource.getItem('formbindingprocessreaderrormsg'), result.Message);
                }
            })
    };

    function displayBindedProcessInfo(formProcessList) {
        var divProcessGrid = document.querySelector('#myProcessGrid');
        $(divProcessGrid).empty();

        var gridOptions = {
            columnDefs: [
                { headerName: 'ID', field: 'ID', width: 80 },
                { headerName: kresource.getItem('processguid'), field: 'ProcessGUID', width: 120 },
                { headerName: kresource.getItem('processname'), field: 'ProcessName', width: 200 },
                { headerName: kresource.getItem('version'), field: 'Version', width: 80 },
            ],
            rowSelection: 'single',
            onSelectionChanged: onSelectionChanged
        };

        new agGrid.Grid(divProcessGrid, gridOptions);
        gridOptions.api.setRowData(formProcessList);

        function onSelectionChanged() {
            var selectedRows = gridOptions.api.getSelectedRows();
            selectedRows.forEach(function (selectedRow, index) {
                kprocessbind.pselectedProcessEntity = selectedRow;
            });
        }
    }

    kprocessbind.bindProcess = function () {
        //判断当前表单ID是否有效
        if (kprocessbind.mcurrentFormID === 0) {
            kmsgbox.warn(kresource.getItem('Process.onFormBindingProcess.null.warn'));
            return false;
        }

        //先判断表单有没有绑定流程，如果绑定则给出提示确认，然后重新绑定
        var selProcessID = parseInt($("#ddlProcesses").val());
        if (selProcessID !== 0
            && selProcessID !== kprocessbind.mcurrentProcessID) {

            kmsgbox.confirm(kresource.getItem('Process.onFormBindingProcess.confirm'), function () {
                var entityProcessView = {};
                entityProcessView.FormID = kprocessbind.mcurrentFormID;
                entityProcessView.ProcessID = selProcessID;

                jshelper.ajaxPost(kconfig.webApiUrl + "api/formprocess/BindFormProcess", JSON.stringify(entityProcessView),
                    function (result) {
                        if (result.Status === 1) {
                            kprocessbind.loadProcess(kprocessbind.mcurrentFormID);
                            //kmsgbox.info(kresource.getItem('Process.onFormBindingProcess.ok'));
                            kprocessbind.mcurrentProcessID = selProcessID;
                        } else {
                            kmsgbox.error(kresource.getItem('Process.onFormBindingProcess.error'), result.Message);
                        }
                    });
            });
        } else {
            kmsgbox.warn(kresource.getItem('Process.onFormBindingProcess.sameorinvalid.warn'));
        }
    }

    kprocessbind.unbindProcess = function () {
        //判断当前表单ID是否有效
        if (kprocessbind.mcurrentFormID === 0
            || kprocessbind.pselectedProcessEntity === null) {
            kmsgbox.warn(kresource.getItem('Process.onFormUnbindingProcess.null.warn'));
            return false;
        }

        kmsgbox.confirm(kresource.getItem('Process.onFormUnbindingProcess.confirm'), function () {
            var entityProcessView = {};
            entityProcessView.FormID = kprocessbind.mcurrentFormID;
            entityProcessView.ProcessID = kprocessbind.pselectedProcessEntity.ProcessID;

            jshelper.ajaxPost(kconfig.webApiUrl + "api/formprocess/UnbindFormProcess", JSON.stringify(entityProcessView),
                function (result) {
                    if (result.Status === 1) {
                        kprocessbind.loadProcess(kprocessbind.mcurrentFormID);
                        //kmsgbox.info(kresource.getItem('Process.onFormUnbindingProcess.ok'));
                    } else {
                        kmsgbox.error(kresource.getItem('Process.onFormUnbindingProcess.error'), result.Message);
                    }
                });
        });
    }

    return kprocessbind;
})()

export default kprocessbind;
