var setting = (function () {
    function setting() {
    }
    setting.mcurrentProcessGUID = "";

    setting.loadProcess = function () {
        //判断当前表单ID是否有效
        if (kmaster.mcurrentEntityDefID == "") {
            $.msgBox({
                title: "Master / Setting",
                content: "请先选定表单，然后再绑定流程！",
                type: "alert"
            });
            $("#modalSettingForm").modal("hide");
            return false;
        }

        jshelper.ajaxGet("api/eavprocess/GetEntityProcessView/" + kmaster.mcurrentEntityDefID, null,
            function (result) {
                if (result.Status == 1) {
                    var processList = result.Entity.ProcessList;
                    var entityProcess = result.Entity.EntityProcess;
                    $.each(processList, function (i, process) {
                        $('#ddlProcesses').append($('<option>', {
                            value: process.ProcessGUID,
                            text: process.ProcessName
                        }));
                    });

                    if (entityProcess) {
                        $('#ddlProcesses').val(entityProcess.ProcessGUID);
                        setting.mcurrentProcessGUID = entityProcess.ProcessGUID;
                    }
                } else {
                    $.msgBox({
                        title: "Master / Setting",
                        content: "读取表单绑定流程记录失败！错误信息：" + result.Message,
                        type: "error"
                    });
                }
        })
    };

    setting.sure = function () {
        //判断当前表单ID是否有效
        if (kmaster.mcurrentEntityDefID == "") {
            $.msgBox({
                title: "Master / Setting",
                content: "请先选定表单，再绑定流程！",
                type: "alert"
            });
            $("#modalSettingForm").modal("hide");
            return false;
        }

        var selProcessGUID = $.trim($("#ddlProcesses").val());
        if (selProcessGUID != ""
            && selProcessGUID != setting.mcurrentProcessGUID) {
            var entityProcess = {};
            entityProcess.EntityDefID = kmaster.mcurrentEntityDefID;
            entityProcess.ProcessGUID = selProcessGUID;
            entityProcess.ProcessName = $("#ddlProcesses option:selected").text();

            jshelper.ajaxPost("api/eavprocess/SaveEntityProcess", JSON.stringify(entityProcess),
              function (result) {
                  if (result.Status == 1) {
                      $.msgBox({
                          title: "Master / Setting",
                          content: "流程绑定信息已经保存！",
                          type: "info"
                      });
                      setting.mcurrentProcessGUID = selProcessGUID;
                      $("#modalSettingForm").modal("hide");
                  } else {
                      $.msgBox({
                          title: "Master / Setting",
                          content: "保存表单绑定流程记录失败！错误信息：" + result.Message,
                          type: "error"
                      });
                  }
              });
        } else {
            $("#modalSettingForm").modal("hide");
        }
    }

    return setting;
})();