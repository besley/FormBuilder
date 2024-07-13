/*
* Slickflow 工作流引擎遵循LGPL协议，也可联系作者商业授权并获取技术支持；
* 除此之外的使用则视为不正当使用，请您务必避免由此带来的商业版权纠纷。

The Slickflow Designer project.
Copyright (C) 2014  .NET Workflow Engine Library

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, you can access the official
web page about lgpl: https://www.gnu.org/licenses/lgpl.html
*/

var processlist = (function () {
    function processlist() {
    }

    processlist.pselectedProcessEntity = null;
    processlist.pselectedTaskEntity = null;

    //#region Process DataGrid
    processlist.getProcessList = function () {
        $('#loading-indicator').show();

        jshelper.ajaxPost('api/Wf2Xml/GetProcessListSimple', null, function (result) {
            if (result.Status === 1) {
                var divProcessGrid = document.querySelector('#myProcessGrid');
                $(divProcessGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'ID', width: 50 },
                        { headerName: kresource.getItem('processguid'), field: 'ProcessGUID', width: 120 },
                        { headerName: kresource.getItem('processname'), field: 'ProcessName', width: 200 },
                        { headerName: kresource.getItem('version'), field: 'Version', width: 40 },
                        { headerName: kresource.getItem('status'), field: 'IsUsing', width: 40, cellRenderer: onIsUsingCellRenderer },
                    ],
                    rowSelection: 'single',
                    onSelectionChanged: onSelectionChanged,
                    onRowDoubleClicked: onRowDoubleClicked
                };

                function onIsUsingCellRenderer(params) {
                    return params.value === 1 ? kresource.getItem('active') : kresource.getItem('unactive');
                }

                function onStartTypeCellRenderer(params) {
                    var startType = '';
                    if (params.value === 1)
                        startType = kresource.getItem('timer');
                    else if (params.value === 2)
                        startType = kresource.getItem('email');
                    return startType;
                }

                new agGrid.Grid(divProcessGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                $('#loading-indicator').hide();

                function onSelectionChanged() {
                    var selectedRows = gridOptions.api.getSelectedRows();
                    var selectedProcessID = 0;
                    selectedRows.forEach(function (selectedRow, index) {
                        processlist.pselectedProcessEntity = selectedRow;
                        processlist.getTaskList();
                        processlist.getDoneList();
                    });
                }

                function onRowDoubleClicked(e, args) {
                    processlist.editProcess();
                }
            }
            else {
                kmsgbox.error(kresource.getItem("processlistloaderrormsg"), result.Message);
            }
        });
    }

    processlist.getTaskList = function () {
        $('#loading-indicator').show();

        var taskQuery = {};
        taskQuery.ProcessGUID = kflowmain.mcurrentProcessGUID;
        taskQuery.Version = kflowmain.mcurrentProcessVersion;
        taskQuery.AppInstanceID = kflowmain.mcurrentEntityInfoID.toString();

        jshelper.ajaxPost('../../api/Wf2Xml/GetReadyTaskList', JSON.stringify(taskQuery), function (result) {
            if (result.Status === 1) {
                var divTaskGrid = document.querySelector('#myToDoTaskGrid');
                $(divTaskGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'TaskID', width: 50 },
                        { headerName: kresource.getItem('activityname'), field: 'ActivityName', width: 160 },
                        { headerName: kresource.getItem('assignedtouserid'), field: 'AssignedToUserID', width: 100 },
                        { headerName: kresource.getItem('assignedtousername'), field: 'AssignedToUserName', width: 100 },
                        { headerName: kresource.getItem('createddatetime'), field: 'CreatedDateTime', width: 200 },
                    ],
                    rowSelection: 'single',
                    onSelectionChanged: onSelectionChanged,
                };

                new agGrid.Grid(divTaskGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                $('#loading-indicator').hide();

                function onSelectionChanged() {
                    var selectedRows = gridOptions.api.getSelectedRows();
                    selectedRows.forEach(function (selectedRow, index) {
                        processlist.pselectedTaskEntity = selectedRow;
                        //检验字段权限数据
                        kentityinfo.checkAttrInActivityFormVisibleReadOnly(processlist.pselectedTaskEntity.TaskID);
                    });
                }
            } else {
                kmsgbox.error(kresource.getItem('tasktodolistreaderrormsg'), result.Message);
            }
        });
    };

    processlist.getDoneList = function () {
        $('#loading-indicator').show();

        var taskQuery = {};
        taskQuery.ProcessGUID = kflowmain.mcurrentProcessGUID;
        taskQuery.Version = kflowmain.mcurrentProcessVersion;
        taskQuery.AppInstanceID = kflowmain.mcurrentEntityInfoID.toString();

        jshelper.ajaxPost('../../api/Wf2Xml/GetDoneList', JSON.stringify(taskQuery), function (result) {
            if (result.Status === 1) {
                var divTaskGrid = document.querySelector('#myDoneTaskGrid');
                $(divTaskGrid).empty();

                var gridOptions = {
                    columnDefs: [
                        { headerName: 'ID', field: 'TaskID', width: 50 },
                        { headerName: kresource.getItem('activityname'), field: 'ActivityName', width: 160 },
                        { headerName: kresource.getItem('endedbyuserid'), field: 'EndedByUserID', width: 100 },
                        { headerName: kresource.getItem('endedbyusername'), field: 'EndedByUserName', width: 100 },
                        { headerName: kresource.getItem('endeddatetime'), field: 'EndedDateTime', width: 200 },
                    ],
                    rowSelection: 'single'
                };

                new agGrid.Grid(divTaskGrid, gridOptions);
                gridOptions.api.setRowData(result.Entity);

                $('#loading-indicator').hide();
            } else {
                kmsgbox.error(kresource.getItem('taskdonelistreaderrormsg'), result.Message);
            }
        });
    }

    processlist.start = function (appName, appInstanceID, appInstanceCode, processGUID, version) {
        sfconfig.initRunner(appName, appInstanceID, appInstanceCode);
        sfconfig.Runner["ProcessGUID"] = processGUID;
        sfconfig.Runner["Version"] = version;

        processapi.start(sfconfig.Runner, function (result) {
            if (result.Status === 1) {
                kmsgbox.info(kresource.getItem('processstartedokmsg'));
                processlist.getTaskList();
                processlist.getDoneList();
            } else {
                kmsgbox.error(kresource.getItem('formflowstartwarningmsg'), result.Message);
            }
        })
    }

    processlist.run = function(appName, appInstanceID, appInstanceCode, processGUID, version) {
        sfconfig.initRunner(appName, appInstanceID, appInstanceCode);
        sfconfig.Runner["ProcessGUID"] = processGUID;
        sfconfig.Runner["Version"] = version;

        var taskEntity = processlist.pselectedTaskEntity;
        if (taskEntity !== null) {
            sfconfig.Runner["UserID"] = taskEntity.AssignedToUserID;
            sfconfig.Runner["UserName"] = taskEntity.AssignedToUserName;
            sfconfig.Runner["TaskID"] = taskEntity.TaskID;

            $('#modelNextStepForm').modal('show');
            nextactivitytree.showNextActivityTree(sfconfig.Runner, sfconfig.Command.RUN);
        } else {
            kmsgbox.warn(kresource.getItem('formflowrunwarningmsg'));
        }
    }
    return processlist;
})()

//process api
var processapi = (function () {
    function processapi() {
    }

    processapi.queryProcessFile = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/QueryProcessFile',
            JSON.stringify(query),
            function (result) {
                callback(result);
            }
        );
    }

    processapi.start = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/StartProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    //processapi.next = function (query, callback) {
    //    jshelper.ajaxPost('api/Wf2Xml/GextNextActivityUserTree',
    //        JSON.stringify(query),
    //        function (result) {
    //            callback(result);
    //        });
    //}

    processapi.next = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/GetNextStepInfo',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.run = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/RunProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.revise = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/ReviseProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    //processapi.prev = function (query, callback) {
    //    jshelper.ajaxPost('api/Wf2Xml/GetPrevActivityUserTree',
    //        JSON.stringify(query),
    //        function (result) {
    //            callback(result);
    //        });
    //}

    processapi.prev = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/GetPreviousStepInfo',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.sendback = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/SendBackProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.resend = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/ResendProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.reject = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/RejectProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.close = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/CloseProcess',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });
    }

    processapi.deleteInstance = function (query, callback) {
        jshelper.ajaxPost('api/Wf2Xml/DeleteInstance',
            JSON.stringify(query),
            function (result) {
                callback(result);
            });   
    }

    return processapi;
})()