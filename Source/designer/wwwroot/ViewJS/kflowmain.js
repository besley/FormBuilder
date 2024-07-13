var kflowmain = (function () {
    kflowmain.WfFlag = 0;      //0-新建表单,1-打开现有表单
    kflowmain.mcurrentEntityDefID = 0;
    kflowmain.mcurrentEntityInfoID = 0;
    kflowmain.mcurrentProcessInstanceID = 0;
    kflowmain.mcurrentProcessGUID = "";
    kflowmain.mcurrentAppName = "";
    kflowmain.WfAppRunner = {};

    function kflowmain() {
    }

    kflowmain.ready = function (entityDefID) {
        if (entityDefID === 0) {
            kmsgbox.warn(kresource.getItem('formdatanotselectedwarningmsg'));
        } else {
            showProgressBar();
            loadEntityInfo(entityDefID);
        }
    }

    function showProgressBar() {
        $('.progress .bar').progressbar({
            transition_delay: 200
        });
        var $modal = $('.js-loading-bar'),
            $bar = $modal.find('.bar');

        $modal.modal('show');

        setTimeout(function () {
            $modal.modal('hide');
        }, 500);
    }

    function loadEntityInfo(entityDefID) {
        //create a new dynamic form
        kflowmain.mcurrentEntityDefID = entityDefID;

        $(document).ajaxStop(function () {
            $(this).unbind("ajaxStop"); //prevent running again when other calls finish
            //list entity info data
            var gridCtrl = document.querySelector("#myEntityInfoAttrValueGrid");
            kentityinfo.initEntityInfoAttrValueTable(gridCtrl, kflowmain.mcurrentEntityDefID);
        });

        //loading entity definition with process data
        var query = { "ID": entityDefID };
        jshelper.ajaxPost("../../api/FBMaster/QueryEntityDefProcessView", JSON.stringify(query),
            function (result) {
                if (result.Status === 1) {
                    $("#dynamicFormTitle").empty();
                    $("#dynamicFormContainer").empty();

                    if (result.Entity !== null) {
                        //get form and process info
                        kflowmain.mcurrentProcessGUID = result.Entity.ProcessGUID;
                        kflowmain.mcurrentProcessVersion = result.Entity.Version;
                        kflowmain.mcurrentAppName = result.Entity.EntityName;


                        //append form template and data
                        $("#dynamicFormTitle").append(result.Entity.EntityTitle);
                        var formContainer = $("#dynamicFormContainer");
                        formContainer.append(result.Entity.HTMLContent);

                        //initialize form control, such as loading data source...
                        kcommon.initializeControls(kflowmain.mcurrentEntityDefID, formContainer, true);
                    } else {
                        kmsgbox.warn(kresource.getItem('formdataloadentityinfowarningmsg'));
                    }
                } else {
                    kmsgbox.warn(kresource.getItem('formdataloadentityinfoerrormsg', result.Message));
                }
            });
    }

    //保存表单数据
    kflowmain.saveFormData = function () {
        if (kflowmain.WfFlag === 0 && kflowmain.mcurrentEntityInfoID === 0) {
            kmsgbox.warn(kresource.getItem('formdatasavewarningmsg'));
            return false;
        }

        //提交数据保存
        var chkResult = getEntityAttrBindingItem();
        if (chkResult.Status === 1) {
            var entityAttrBindingItem = chkResult.Row;
            jshelper.ajaxPost("../../api/FBData/UpdateEntityRow", JSON.stringify(entityAttrBindingItem),
                function (result) {
                    if (result.Status === 1) {
                        kmsgbox.info(kresource.getItem('formdatasaveokmsg'));
                    } else {
                        kmsgbox.error(kresource.getItem('formdatasaveerrormsg'), result.Message);
                    }
                });
        }
    }

    //获取控件录入数据实体
    function getEntityAttrBindingItem() {
        //此处需要加入数据验证功能：检验输入项是否合法，数据类型是否匹配
        var chkResult = {};
        chkResult.Status = 1;

        var ctrlList = $("#dynamicFormContainer").find("[class*=ctrl]");
        var entityInfo = {};
        entityInfo.EntityDefID = kflowmain.mcurrentEntityDefID;
        entityInfo.ID = kflowmain.mcurrentEntityInfoID;

        var entityAttrBindingItem = {};
        var entityAttrValueList = [];
        entityAttrBindingItem.EntityInfo = entityInfo;
        entityAttrBindingItem.EntityAttrValueList = entityAttrValueList;

        ctrlList.each(function (i, o) {
            var attrCode = $(o).attr("name");
            var ctrlType = $.trim(o.className.match("ctrl-.*")[0].split(" ")[0].split("-")[1]);

            var hiddenAttrEntity = $(o).parent().find(".hiddenAttributeEntity")[0];
            var attrEntity = $.parseJSON(hiddenAttrEntity.value);

            //获取控件输入数值
            var inputValue = dfieldManagerGet.getControlValue(ctrlType, attrEntity, $(o));
            var divCtrl = $(o).parent();
            var lblCtrl = $(divCtrl).find(".mandatory-field")[0];
            if (lblCtrl !== undefined) {
                if (inputValue === "") {
                    //必填字段没有数值，弹出警告消息
                    chkResult.Status = -1;
                    kmsgbox.warn(kresource.getItem('formvaluesavingwarningmsg'));
                    return false;
                }
            } else {
                window.console.log("undefined...");
            }
            
            attrEntity.Value = inputValue;
            attrEntity.EntityInfoID = kflowmain.mcurrentEntityInfoID;
            entityAttrValueList.push(attrEntity);
        });
        chkResult.Row = entityAttrBindingItem;

        return chkResult;
    }

    //创建新表单，启动流程
    kflowmain.startProcess = function () {
        //判断流程是否已经启动
        if (kflowmain.mcurrentProcessInstanceID > 0) {
            kmsgbox.warn(kresource.getItem('formprocessstartwarningmsg'));
            return false;
        }

        //执行流程启动操作
        var chkResult = getEntityAttrBindingItem();
        if (chkResult.Status === 1) {
            var entityAttrBindingItem = chkResult.Row;
            var request = {};
            request.WfAppRunner = kflowmain.WfAppRunner;
            request.WfAppRunner.ProcessGUID = kflowmain.mcurrentProcessGUID;
            request.WfAppRunner.Version = kflowmain.mcurrentProcessVersion;
            request.WfAppRunner.AppName = kflowmain.mcurrentAppName;
            request.WfAppRunner.UserID = "01";
            request.WfAppRunner.UserName = "Zero";

            request.EntityInfoWithAttrValueListItem = entityAttrBindingItem;

            jshelper.ajaxPost("../../api/FBData/InsertEntityRowFlow", JSON.stringify(request),
                function (result) {
                    if (result.Status === 1) {
                        var entityInfo = result.Entity;
                        kflowmain.mcurrentEntityInfoID = entityInfo.ID;

                        //load new entity info
                        loadEntityInfo(kflowmain.mcurrentEntityDefID);

                        //refresh task list
                        processlist.getTaskList();
                        kmsgbox.info(kresource.getItem('formflowstartokmsg'));
                    } else {
                        kmsgbox.error(kresource.getItem('formflowstarterrormsg'), result.Message);
                    }
                });
        }
    }

    kflowmain.runProcess = function () {
        var runner = {};
        runner.ProcessGUID = kflowmain.mcurrentProcessGUID;
        runner.Version = kflowmain.mcurrentProcessVersion;
        runner.AppInstanceID = kflowmain.mcurrentEntityInfoID.toString();

        var taskEntity = processlist.pselectedTaskEntity ;
        if (taskEntity !== null) {
            runner["UserID"] = taskEntity.AssignedToUserID;
            runner["UserName"] = taskEntity.AssignedToUserName;
            runner["TaskID"] = taskEntity.TaskID;
            runner["Conditions"] = fillConditionKey();

            $('#modelNextStepForm').modal('show');
            nextActivityTree.getNextActivityTree(runner);
        } else {
            kmsgbox.warn(kresource.getItem('formflowrunselectedtaskwarningmsg'));
        }
    }

    function fillConditionKey() {
        var Conditions = {};
        var attrEntity = null;
        var inputValue = "";
        var ctrlList = $("#dynamicFormContainer").find("[class*=ctrl]");

        ctrlList.each(function (i, o) {
            var ctrlType = $.trim(o.className.match("ctrl-.*")[0].split(" ")[0].split("-")[1]);
            var hiddenAttrEntity = $(o).parent().find(".hiddenAttributeEntity")[0];

            attrEntity = $.parseJSON(hiddenAttrEntity.value);
            if (attrEntity.ConditionKey && $.trim(attrEntity.ConditionKey) !== "") {
                //获取控件输入数值
                inputValue = dfieldManagerGet.getControlValue(ctrlType, hiddenAttrEntity, $(o));
                Conditions[attrEntity.ConditionKey] = inputValue;      //fixed value to condition key variable
            }
        });
        return Conditions;
    }

    kflowmain.showKGraph = function () {
        if (kflowmain.mcurrentProcessGUID === "") {
            kmsgbox.warn(kresource.getItem('formflowshowdiagramwarningmsg'));
            return false;
        } else {
            window.open('/sfd/Diagram?AppInstanceID=' + kflowmain.mcurrentEntityInfoID
                + '&ProcessGUID=' + kflowmain.mcurrentProcessGUID + '&Mode=' + 'READONLY');
        }
    }

    kflowmain.deleteEntityInfo = function () {
        var query = {};
        query.EntityInfoID = kflowmain.mcurrentEntityInfoID;

        if (query.EntityInfoID === 0) {
            kmsgbox.warn(kresource.getItem('formflowdeleteinstancewarningmsg'));
            return false;
        }

        kmsgbox.confirm(kresource.getItem('formflowdeleteinstanceconfirmmsg'), function (result) {
            if (result === "Yes") {
                kentityinfo.deleteEntityInfo(query, function (result) {
                    if (result.Status === 1) {
                        kmsgbox.info(kresource.getItem('formflowdeleteinstancesaveokmsg'));
                        //refresh entity info
                        loadEntityInfo(kflowmain.mcurrentEntityDefID);
                    } else {
                        kmsgbox.error(kresource.getItem('formflowdeleteinstanceerrormsg'), result.Message);
                    }
                });
            }
        });
    }

    kflowmain.refreshEntityInfo = function () {
        var entityDefID = kflowmain.mcurrentEntityDefID;
        kflowmain.ready(entityDefID);
    }

    return kflowmain;
})()