var kmaster = (function () {
    function kmaster() {
    }
    kmaster.mcurrentEntityDefID = 0;
    kmaster.mcurrentEntityDef = null;
    kmaster.mctrlIndex = 1001;
    kmaster.moutputHtmlPageContent = "";

    kmaster.docReady = function () {
        showProgressBar();

        //#region compile template
        compileTemplates();
        //#endregion

        //#region dropped fields
        stylizeDroppedFields();
        makeDraggable();
        //#endregion

        //#region custom control window
        initControlBehaviour();
        //#endregion

        //#region empty modal dialog
        emptyModalDialog();
        //#endregon
    };

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

    function compileTemplates() {
        window.templates = {};
        window.templates.common = Handlebars.compile($("#control-customize-template").html());
        window.templates.textbox = Handlebars.compile($("#textbox-template").html());
        window.templates.passwordbox = Handlebars.compile($("#textbox-template").html());
        window.templates.combobox = Handlebars.compile($("#combobox-template").html());
        window.templates.selectmultiplelist = Handlebars.compile($("#combobox-template").html());
        window.templates.radiogroup = Handlebars.compile($("#combobox-template").html());
        window.templates.checkboxgroup = Handlebars.compile($("#combobox-template").html());
        window.templates.text = Handlebars.compile($("#text-template").html());
        window.templates.date = Handlebars.compile($("#date-template").html());
        window.templates.form = Handlebars.compile($("#form-template").html());
    }

    function stylizeDroppedFields() {
        $(".droppedFields").droppable({
            activeClass: "activeDroppable",
            hoverClass: "hoverDroppable",
            accept: ":not(.ui-sortable-helper)",
            drop: function (event, ui) {
                if (kmaster.mcurrentEntityDefID === 0) {
                    kmsgbox.info(kresource.getItem('formrecordselectedwarningmsg'));
                    return;
                }
                var draggable = ui.draggable;
                var modelCtrl = $(draggable).find(".modele");

                if (modelCtrl.length === 0) {
                    //drag only this control
                    ;
                } else {
                    //drag from left bar
                    draggable = $(ui.draggable).find(".modele").clone();
                    draggable.removeClass("modele");
                    draggable.removeClass("selectorField");
                    draggable.addClass("droppedField");
                    draggable.addClass("label-div-hidden");

                    draggable[0].id = "CTRL-DIV-" + getNextCtrlIndex();
                    draggable.appendTo(this);

                    draggable.unbind('click').bind('click', function (e) {
                        var me = $(this);
                        var ctrl = me.find("[class*=ctrl]")[0];
                        var ctrlType = $.trim(ctrl.className.match("ctrl-.*")[0].split(" ")[0].split("-")[1]);
                        displayControlForm(ctrlType, this.id);
                    });

                    //stop default event behaviours
                    var ctrlUnclick = draggable.find(".unclick");
                    if (ctrlUnclick !== undefined) {
                        $(ctrlUnclick).unbind('click').bind('click', function (e) {
                            e.preventDefault();
                        });
                    }
                }
            }
        });

        $(".droppedFields").mouseenter(function () {
            kmaster.tableToDelete = this;
            $("#divDeleteTable").show();
            $("#divDeleteTable").position({
                my: "right top",
                at: "right top",
                of: $(this).parent().children().last()
            });
        });
        $(".droppedFields").mouseleave(function () {
            $("#divDeleteTable").hide();
        });
        $(".droppedField").unbind('click').bind('click', function (e) {
            var me = $(this);
            var ctrl = me.find("[class*=ctrl]")[0];
            var ctrlType = $.trim(ctrl.className.match("ctrl-.*")[0].split(" ")[0].split("-")[1]);

            displayControlForm(ctrlType, this.id);
        });

        //stop default event behaviours
        $(".unclick").unbind('click').bind('click', function (e) {
            e.preventDefault();
        });
    }

    function getNextCtrlIndex() {
        var lastId = 1000;
        $(".droppedField").each(function (i, o) {
            var strId = $(o).attr("id").split("-")[2];
            if (parseInt(strId) > parseInt(lastId)) lastId = strId;
        })

        kmaster.mctrlIndex = parseInt(lastId) + 1;
        return kmaster.mctrlIndex;
    }

    function makeDraggable() {
        $(".selectorField").draggable({
            helper: "clone",
            stack: "div",
            cursor: "move",
            cancel: null
        });

        $(".droppedField").draggable({
            helper: "original",
            stack: "div",
            cursor: "move",
            cancel: null
        });
    }

    kmaster.makeDropFieldDraggable = function () {
        $(".droppedField").draggable({
            helper: "original",
            stack: "div",
            cursor: "move",
            cancel: null
        });
    }

    function displayControlForm(ctrlType, ctrlId) {
        kmaster.mcurrentControlID = ctrlId;

        //normal control type
        var ctrlParams = {};
        var specificTemplate = window.templates[ctrlType];
        if (typeof (specificTemplate) === "undefined") {
            specificTemplate = function () {
                return '';
            };
        }
        var modalHeader = $("#" + ctrlId).find('.control-label').text();

        var templateParams = {
            header: modalHeader,
            content: specificTemplate(ctrlParams),
            type: ctrlType,
            forCtrl: ctrlId,
            display: "block"
            //displayStyle: ctrlType == "text" || ctrlType == "date" ? "none" : "block"
        };
        var dialogContent = window.templates.common(templateParams) + "";
        BootstrapDialog.show({
            title: kresource.getItem('fieldproperty'),
            message: $('<div id="customization_modal" name="customization_modal"/>').append(dialogContent),
            draggable: true,
            resizeable: true
        });

        setTimeout(function () {
            fieldPopup.popupControl(ctrlType, ctrlId);
        }, 300);
    }

    function initControlBehaviour() {
        //#region remove table image show/hide
        $("#divDeleteTable").hide();
        $("#divDeleteTable").mouseenter(function () {
            $("#divDeleteTable").show();
        });
        //#endregion

        //#region allow sortable
        $("#selected-content").sortable({
            cancel: null,
            start: function (event, ui) {
                $("#divDeleteTable").hide();
            }
        }).disableSelection();
        //#endregion

        //empty modal dialog
        emptyModalDialog();
    }

    function emptyModalDialog() {
        $('.eavModalDialog', ' body').on('hidden', function () {
            $(this).removeData('modal');
            $('.modal-body', this).empty();
        })
    }

    kmaster.createEntityDef = function () {
        kmaster.mcurrentEntityDefID = 0;

        var ctrlParams = {};
        ctrlParams.label = "";
        ctrlParams.name = "";
        ctrlParams.version = "1";
        ctrlParams.desc = "";
        var specificTemplate = window.templates["form"];
        if (typeof (specificTemplate) === "undefined") {
            specificTemplate = function () {
                return '';
            };
        }
        var templateParams = {
            header: kresource.getItem('formdefinition'),
            content: specificTemplate(ctrlParams),
            display: "block"
        };

        $("[name=custom_form_modal]").remove();
        BootstrapDialog.show({
            title: kresource.getItem('formproperty'),
            message: $('<div id="custom_form_modal" name="custom_form_modal"/>')
                .append(templateParams.content),
            draggable: true
        });
    }

    kmaster.openEntityDef = function () {
        BootstrapDialog.show({
            title: kresource.getItem('formlist'),
            message: $('<div></div>').load('entitydef/list'),
            draggable: true
        });
    }

    kmaster.showAttributeList = function () {
        BootstrapDialog.show({
            title: kresource.getItem('fieldlist'),
            message: $('<div></div>').load('attribute/list'),
            draggable: true
        });
    }

    kmaster.popupEntityDef = function () {
        if (kmaster.mcurrentEntityDefID === 0) {
            return false;
        }

        if (kmaster.mcurrentEntityDef === null) {
            jshelper.ajaxGet("api/eavdata/GetEntityDefByID/" + kmaster.mcurrentEntityDefID, null, function (result) {
                if (result.Status === 1) {
                    var entity = result.Entity;
                    kmaster.mcurrentEntityDef = entity;
                    showEntityDefDialog(entity)
                } else {
                    kmsgbox.error(result.Message);
                }
            });
        } else {
            showEntityDefDialog(kmaster.mcurrentEntityDef);
        }
    };

    function showEntityDefDialog(entity) {
        var ctrlParams = {};
        ctrlParams.label = entity.EntityTitle;
        ctrlParams.name = entity.EntityName;
        ctrlParams.version = entity.Version;
        ctrlParams.desc = entity.Description;

        var specificTemplate = window.templates["form"];
        if (typeof (specificTemplate) === "undefined") {
            specificTemplate = function () {
                return '';
            };
        }
        var templateParams = {
            header: kresource.getItem('formdefinition'),
            content: specificTemplate(ctrlParams),
            display: "block"
        };

        $("[name=custom_form_modal]").remove();
        BootstrapDialog.show({
            title: kresource.getItem('formproperty'),
            message: $('<div id="customer_form_modal" name="custom_form_modal"/>')
                .append(templateParams.content),
            draggable: true
        });
    }

    kmaster.deleteTable = function () {
        if (kmaster.tableToDelete) {
            kmsgbox.confirm(kresource.getItem('deletetableconfirmmsg'), function (result) {
                if (result === "Yes") {
                    var attrCtrlList = $(kmaster.tableToDelete).parent().find("input[class=hiddenAttributeEntity]");
                    var attrArray = new Array();
                    for (var i = 0; i < attrCtrlList.length; i++) {
                        if (attrCtrlList[i].val != undefined) {
                            attrArray[i] = $.parseJSON(attrCtrlList[i].value);
                        }
                    }

                    //remove attribute and update template content
                    $("body").append($("#divDeleteTable"));
                    $(kmaster.tableToDelete).parent().remove();
                    kmaster.tableToDelete = null;
                    $("#divDeleteTable").hide();

                    //save template content
                    var templateContent = $("#selected-content")[0].outerHTML;
                    var entityDef = {
                        "ID": kmaster.mcurrentEntityDefID,
                        "TemplateContent": templateContent,
                        "HTMLContent": ""
                    };

                    var entity = {};
                    entity.EntityDef = entityDef;
                    entity.EntityAttributeList = attrArray;

                    //post
                    jshelper.ajaxPost('api/eavdata/DeleteAttributeWithTemplate',
                        JSON.stringify(entity),
                        function (result) {
                            if (result.Status === 1) {
                                kmsgbox.info(kresource.getItem('deletetableokmsg'));
                            } else {
                                kmsgbox.error(result.Message);
                            }
                        });

                    return;
                }
            });
        }
    };

    kmaster.generateHTMLContent = function () {
        var selectedContent = $('#selected-content').clone();
        selectedContent.find("div").each(function (i, o) {
            var obj = $(o);
            obj.removeClass("draggableField ui-draggable well ui-droppable ui-sortable");

        });

        var txtLegend = $("#txtFormTitle")[0].value;
        if (txtLegend === undefined || txtLegend === "") {
            txtLegend = kresource.getItem('customformexample');
        }
        selectedContent.find("#form-title-div").remove();
        var htmlSelectedContent = selectedContent.html();

        return htmlSelectedContent;
    }
    kmaster.previewForm = function () {
        ////var win = window.open("about:blank");
        ////win.document.write(kmaster.generateHTMLContent());
        var isValidated = kmaster.validateInput();
        if (isValidated === true) {
            window.open("preview/index/" + kmaster.mcurrentEntityDefID);
        }
    };

    kmaster.bindProcess = function () {
        var isValidated = kmaster.validateInput();
        if (isValidated === true) {
            window.open("setting/index/" + kmaster.mcurrentEntityDefID);
        }
    }

    kmaster.runProcess = function () {
        window.open("flow/index/" + kmaster.mcurrentEntityDefID);
    }

    kmaster.showAdjustDialog = function () {
        BootstrapDialog.show({
            title: kresource.getItem('tableproperty'),
            message: $('<div></div>').load("setting/table"),
            draggable: true
        });
    };
    kmaster.validateInput = function () {
        //check fields input info
        var isValidated = true;
        var attrList = $("#selected-content").find("[class=hiddenAttributeEntity]");

        attrList.each(function (i, o) {
            if (o.value && o.value !== "") {
                var attrObj = $.parseJSON(o.value);
                if (attrObj.AttrName === null || attrObj.AttrName === "") {
                    isValidated = false;
                    kmsgbox.warn(kresource.getItem('fieldinforunfillwarningmsg'), attrObj.type);
                    return false;
                }
            } else {
                isValidated = false;
                var title = $(o).parent().children(":first").text();
                kmsgbox.warn(kresource.getItem('fieldinforunfilltitlewarningmsg'), title);
                return false;
            }
        });
        return isValidated;
    }
    kmaster.validatedSaveTemplate = function () {
        var isValidated = kmaster.validateInput();

        if (isValidated === true) {
            //save html template
            eavManager.SaveTemplateWithHTMLContent();
        }
    }

    //#region resource 
    kmaster.changeLang = function (lang) {
        kresource.setLang(lang);
        location.reload();
    }
    //#endregion
    return kmaster;
})();