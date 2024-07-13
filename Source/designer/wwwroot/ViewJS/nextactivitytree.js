var nextActivityTree = (function () {
    function nextActivityTree() {
    }
    nextActivityTree.mzTree = null;
    function getZTreeSetting() {
        var setting = {
            check: {
                enable: true
            },
            view: {
                //addHoverDom: addHoverDom,
                //removeHoverDom: removeHoverDom,
                dblClickExpand: false,
                showLine: true,
                selectedMulti: false
            },
            data: {
                simpleData: {
                    enable: true,
                    idKey: "id",
                    pIdKey: "pId",
                    rootPId: ""
                }
            },
            callback: {
                beforeClick: function (treeId, treeNode) {
                    var zTree = $.fn.zTree.getZTreeObj("nextStepTree");
                    if (treeNode.isParent) {
                        zTree.expandNode(treeNode);
                        return false;
                    } else {
                        return true;
                    }
                }
            }
        };
        return setting;
    }

    //流程继续流转
    nextActivityTree.getNextActivityTree = function (runner) {
        //get next step information
        jshelper.ajaxPost('../../api/wf2xml/GetNextStepInfo',
            JSON.stringify(runner),
            function (result) {
                if (result.Status === 1) {
                    //弹窗步骤人员办理弹窗
                    var nextStep = result.Entity[0];        //单步演示
                    if (nextStep !== undefined) {
                        var zNodes = [
                            { id: 0, pId: -1, name: kresource.getItem("nextstepinfo"), type: "root", open: true },
                            {
                                id: 1,
                                pId: 0,
                                name: nextStep.ActivityName,
                                activityGUID: nextStep.ActivityGUID,
                                activityName: nextStep.ActivityName,
                                type: "activity",
                                open: false
                            }
                        ];

                        if (nextStep.Users != null) {
                            var id = 2;
                            var userNode = null;
                            $.each(nextStep.Users, function (i, o) {
                                userNode = {
                                    id: id,
                                    pId: 1,
                                    name: o.UserName,
                                    uid: o.UserID,
                                    type: "user"
                                };
                                zNodes.push(userNode);
                                id = id + 1;
                            });
                        }
                        var t = $("#nextStepTree");
                        nextActivityTree.mzTree = $.fn.zTree.init(t, getZTreeSetting(), zNodes);
                    } else {
                        kmsgbox.warn(kresource.getItem('nextactivitytreenonenextstepwarnmsg'));
                    }  
                } else {
                    kmsgbox.error(result.Message);
                }
            }
        );
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
            if (attrEntity.ConditionKey && $.trim(attrEntity.ConditionKey) != "") {
                //获取控件输入数值
                inputValue = dfieldManagerGet.getControlValue(ctrlType, attrEntity, $(o));
                Conditions[attrEntity.ConditionKey] = inputValue;      //fixed value to condition key variable
            }
        });
        return Conditions;
    }

    nextActivityTree.sure = function () {
        //取得下一步节点信息
        var selectedNodes = nextActivityTree.mzTree.getCheckedNodes();
        if (selectedNodes.length <= 0) {
            kmsgbox.warn(kresource.getItem('formflowforwardnextstepwarningmsg'));
            return false;
        }

        //单步可选示例
        var nextStep = {};
        nextStep.Users = [];
        var activityGUID = "", activityName="";
        var user = null;
        var userlist = [];
        $.each(selectedNodes, function (i, o) {
            
            if (o.type === "activity") {
                activityGUID = o.activityGUID;
                activityName = o.activityName;
            } else if (o.type === "user") {
                user = { UserID: o.uid, UserName: o.name };
                userlist.push(user);
            }
        });

        var wfAppRunner = {};
        wfAppRunner.ProcessGUID = kflowmain.mcurrentProcessGUID;
        wfAppRunner.AppName = kflowmain.mcurrentAppName;
        wfAppRunner.AppInstanceID = kflowmain.mcurrentEntityInfoID.toString();

        var taskEntity = processlist.pselectedTaskEntity;
        wfAppRunner.UserID = taskEntity.AssignedToUserID;
        wfAppRunner.UserName = taskEntity.AssignedToUserName;
        wfAppRunner.TaskID = taskEntity.TaskID;

        wfAppRunner.NextActivityPerformers = {};
        wfAppRunner.NextActivityPerformers[activityGUID] = userlist;
        wfAppRunner.Conditions = fillConditionKey();

        kmsgbox.confirm(kresource.getItem('formflowforwardnextstepconfirmmsg') + activityName, function (result) {
            if (result === "Yes") {
                jshelper.ajaxPost("../../api/wf2xml/RunProcess",
                    JSON.stringify(wfAppRunner), function (result) {
                        if (result.Status === 1) {
                            kmsgbox.info(kresource.getItem('formflowforwardnextstepokmsg'));
                            //refresh task list
                            processlist.getTaskList();
                            processlist.getDoneList();

                            $("#modelNextStepForm").modal("hide");
                        } else {
                            kmsgbox.error(result.Message);
                        }
                    });
            }
        });
    }
    return nextActivityTree;
})();
