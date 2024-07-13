var entitydeflist = (function () {
    entitydeflist.mselectedEntityDefID = 0;
    entitydeflist.mselectedEntityDefRow = null;

    function entitydeflist() {
    }

    entitydeflist.load = function () {
    	$('#loading-indicator').show();

        jshelper.ajaxGet("api/eavdata/GetEntityDefList2", null, function (result) {
            if (result.Status === 1) {
                var divEntityDefGrid = document.querySelector("#myEntityDefGrid");
                $(divEntityDefGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'ID', width: 40 },
                        { headerName: kresource.getItem('formtitle'), field: 'EntityTitle', width: 120 },
                        { headerName: kresource.getItem('formname'), field: 'EntityName', width: 120 },
                        { headerName: kresource.getItem('formcode'), field: 'EntityCode', width: 160 },
                        { headerName: kresource.getItem('version'), field: 'Version', width: 80 },
                        { headerName: kresource.getItem('createddate'), field: 'CreatedDate', width: 160 },
                        { headerName: kresource.getItem('description'), field: 'Description', width: 160 }
                    ],
                    rowSelection: 'single',
                    onSelectionChanged: onSelectionChanged,
                };

                new agGrid.Grid(divEntityDefGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                function onSelectionChanged() {
                    var selectedRows = gridOptions.api.getSelectedRows();
                    selectedRows.forEach(function (selectedRow, index) {
                        entitydeflist.mselectedEntityDefID = selectedRow.ID;
                        entitydeflist.mselectedEntityDefRow = selectedRow;
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

    entitydeflist.sure = function () {
        kmaster.mcurrentEntityDefID = entitydeflist.mselectedEntityDefID;
        if (kmaster.mcurrentEntityDefID === 0) return false;

        $("#modelEntityDefListForm").modal("hide");
        //load entitydef and attribute list
        jshelper.ajaxGet("api/eavdata/GetEntityDefByID/" + kmaster.mcurrentEntityDefID, null,
            function (result) {
                if (result.Status === 1) {
                    var entityDef = result.Entity;
                    //$("#selected-content").empty();

                    if (entityDef.TemplateContent !== null
                            && entityDef.TemplateContent !== "") {
                        $("#selected-content").replaceWith(entityDef.TemplateContent);
                    } else {
                        kmsgbox.warn(kresource.getItem('formtemplatenullwarnningmsg'));
                    }
                    kmaster.docReady();
                    var formTitle = entitydeflist.mselectedEntityDefRow.EntityTitle;
                    $("#txtFormTitle").val(formTitle);
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }

    entitydeflist.delete = function () {
        if (entitydeflist.mselectedEntityDefID === 0) {
            return;
        }

        kmsgbox.confirm(kresource.getItem('entitydefdeleteconfirmmsg'), function (result) {
            if (result === "Yes") {
                jshelper.ajaxGet("api/eavdata/DeleteEntityDef/" + entitydeflist.mselectedEntityDefID,
                    null, function (result) {
                        if (result.Status === 1) {
                            kmsgbox.info(kresource.getItem('entitydefdeleteokmsg'));
                            entitydeflist.mselectedEntityDefID = 0;

                            //refresh
                            entitydeflist.load();
                        } else {
                            kmsgbox.error(kresource.getItem("entitydefdeleteerrormsg"), result.Message);
                        }
                    });
            }
        });
    }

    entitydeflist.loadEntityDefList = function () {
        //判断当前表单ID是否有效
        if (kmaster.mcurrentAttributeID === "") {
            kmsgbox.warn(kresource.getItem('bindingdatacontrolwarningmsg'));

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
                if (result.Status === 1) {
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
                    kmsgbox.error(kresource.getItem("bindingdatacontrolerrormsg"), result.Message);
                }
            })
    }

    entitydeflist.upgrade = function () {
        if (entitydeflist.mselectedEntityDefID === 0) {
            return;
        }

        kmsgbox.confirm(kresource.getItem('entitydefupgradeconfirmmsg'), function (result) {
            if (result === "Yes") {
                jshelper.ajaxGet("api/eavdata/UpgradeEntityDef/" + entitydeflist.mselectedEntityDefID,
                    null, function (result) {
                        if (result.Status === 1) {
                            kmsgbox.info(kresource.getItem('entitydefupgradeokmsg'));
                            entitydeflist.mselectedEntityDefID = 0;

                            //refresh
                            entitydeflist.load();
                        } else {
                            kmsgbox.error(kresource.getItem("entitydefupgradeerrormsg"), result.Message);
                        }
                    });
            }
        });
    }

    entitydeflist.refresh = function () {
        entitydeflist.load();
    }
    return entitydeflist;
})();