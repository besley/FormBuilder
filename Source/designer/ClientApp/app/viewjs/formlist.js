import kconfig from '../config/kconfig.js';

import slick from '../script/slick.event.js'
window.slick = slick;

const formlist = (function () {
    formlist.mselectedFormID = 0;
    formlist.mselectedFormRow = null;
    formlist.afterOpened = new slick.Event();

    function formlist() {
    }

    formlist.getFormList = function () {
    	$('#loading-indicator').show();

        jshelper.ajaxGet(kconfig.webApiUrl + "api/form/GetFormList2", null, function (result) {
            if (result.Status === 1) {
                var divFromGrid = document.querySelector("#myFormGrid");
                $(divFromGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'ID', width: 40 },
                        { headerName: kresource.getItem('formname'), field: 'FormName', width: 120 },
                        { headerName: kresource.getItem('formcode'), field: 'FormCode', width: 160 },
                        { headerName: kresource.getItem('version'), field: 'Version', width: 80 },
                        { headerName: kresource.getItem('createddate'), field: 'CreatedDate', width: 160 },
                        { headerName: kresource.getItem('description'), field: 'Description', width: 160 }
                    ],
                    rowSelection: 'single',
                    onSelectionChanged: onSelectionChanged,
                };

                new agGrid.Grid(divFromGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                function onSelectionChanged() {
                    var selectedRows = gridOptions.api.getSelectedRows();
                    selectedRows.forEach(function (selectedRow, index) {
                        formlist.mselectedFormID = selectedRow.ID;
                        formlist.mselectedFormRow = selectedRow;
                    });
                }

                $('#loading-indicator').hide();
            } else {
                kmsgbox.error(kresource.getItem('readentitydeferrormsg'), result.Message);
            }
        });

        function datetimeFormatter(row, cell, value, columnDef, dataContext) {
            if (value != null && value != "") {
                return value.substring(0, 10);
            }
        }
    }

    formlist.sure = function () {
        kmain.mcurrentFormID = formlist.mselectedFormID;
        if (kmain.mcurrentFormID === 0) return false;

        $("#modelFormListForm").modal("hide");
        //load formdefine and attribute list
        jshelper.ajaxGet(kconfig.webApiUrl + "api/form/GetForm/" + kmain.mcurrentFormID, null,
            function (result) {
                if (result.Status === 1) {
                    if (formlist.afterOpened) {
                        slick.trigger(formlist.afterOpened, {
                            "FormEntity": result.Entity
                        });
                    }
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }

    formlist.deleteForm = function () {
        if (formlist.mselectedFormID === 0) {
            return;
        }

        kmsgbox.confirm(kresource.getItem('Form.onFormDelete.confirm'), function () {
            jshelper.ajaxGet(kconfig.webApiUrl + "api/Form/DeleteForm/" + formlist.mselectedFormID,
                null, function (result) {
                    if (result.Status === 1) {
                        formlist.mselectedFormID = 0;

                        //refresh
                        formlist.getFormList();
                    } else {
                        kmsgbox.error(kresource.getItem("Form.onFormDelete.error"), result.Message);
                    }
                });
        });
    }

    formlist.upgrade = function () {
        if (formlist.mselectedEntityDefID === 0) {
            return;
        }

        kmsgbox.confirm(kresource.getItem('entitydefupgradeconfirmmsg'), function (result) {
            if (result === "Yes") {
                jshelper.ajaxGet("api/form/UpgradeE/" + formlist.mselectedEntityDefID,
                    null, function (result) {
                        if (result.Status === 1) {
                            kmsgbox.info(kresource.getItem('entitydefupgradeokmsg'));
                            formlist.mselectedEntityDefID = 0;

                            //refresh
                            formlist.load();
                        } else {
                            kmsgbox.error(kresource.getItem("entitydefupgradeerrormsg"), result.Message);
                        }
                    });
            }
        });
    }

    formlist.refreshForm = function () {
        formlist.getFormList();
    }
    return formlist;
})()

export default formlist;