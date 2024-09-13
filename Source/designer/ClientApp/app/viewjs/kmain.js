import jquery from 'jquery';

import { debounce } from 'min-dash';

import { Form, FormEditor, FormViewer, FormPlayground, getSchemaVariables } from '@bpmn-io/form-js';

import jshelper from '../script/jshelper.js'
window.jshelper = jshelper;

import kresource from '../viewjs/kresource.js'
window.kresource = kresource;

import kmsgbox from '../script/kmsgbox.js'
window.kmsgbox = kmsgbox;

import formlist from '../viewjs/formlist.js'
window.formlist = formlist;

import formapi from '../viewjs/formapi.js'
import kprocessrun from './kprocessrun.js';
window.formapi = formapi;

const kmain = (function () {
    function kmain() {

    }

    kmain.mxFormD = 0;
    kmain.mxFormVersion = '';
    kmain.mxFormEditor = null;
    kmain.mxSelectedFormEntity = null;
    kmain.mxImageUrl = {};

    kmain.init = function () {
        //waiting...
        showProgressBar();

        kmain.mxSelectedFormEntity = null;

        const formEditor = new FormEditor({
            container: document.querySelector('#form-editor'),
        });
        kmain.mxFormEditor = formEditor;

        //register process opened event
        if ("undefined" !== typeof formlist) {
            formlist.afterOpened.subscribe(afterFormOpened);
        }
    }

    function setSessionStorage(formEntity) {
        //var 
    }

    //#region preparation
    function showProgressBar() {
        $(".progress-bar").animate({
            width: "70%",
        }, 200);

        setTimeout(function () {
            $(".progress").hide();
        }, 1000);
    }
	//#endregion
    kmain.createNewForm = function () {
        kmain.mxSelectedFormEntity = null;
        const schema = {
            "schemaVersion": 1,
            "exporter": {
                "name": "form-js",
                "version": "0.1.0"
            },
            "components": [
                {
                    "key": "Name",
                    "type": "textfield",
                    "label": "Name"
                }
            ],
            "type": "default"
        }

        kmain.mxFormEditor.importSchema(schema);
        openedFormHTMLCanvas();
    }

    kmain.openFormFile = function (xml) {
        openFormHTML(xml);
    }

    async function openedFormHTMLCanvas() {
        try {
            var container = $('#js-drop-zone');

            container
                .removeClass('with-error')
                .addClass('with-diagram');
        } catch (err) {

            container
                .removeClass('with-diagram')
                .addClass('with-error');

            container.find('.error pre').text(err.message);

            console.error(err);
        }
    }
    kmain.openForm = function () {
        var BootstrapDialog = require('bootstrap5-dialog');
        BootstrapDialog.show({
            message: $('<div id="popupContent"></div>'),
            title: kresource.getItem("formlist"),
            onshown: function () {
                $("#popupContent").load('pages/form/list.html')
            },
            draggable: true
        });
    }

    kmain.saveFormFile = function () {
        const schema = kmain.mxFormEditor.saveSchema();
        formapi.saveFormSchema(schema);
    }

    function afterFormOpened(e, data) {
        var formEntity = data.FormEntity;
        kmain.mxSelectedFormEntity = formEntity
        if (formEntity.TemplateContent !== null
            && formEntity.TemplateContent !== "") {
            var schema = JSON.parse(formEntity.TemplateContent)
            kmain.mxFormEditor.importSchema(schema)
            //render canvas
            openedFormHTMLCanvas();
        } else {
            kmsgbox.warn(kresource.getItem('formtemplatenullwarnningmsg'));
        }
    }

    kmain.previewForm = function () {
        const schema = kmain.mxFormEditor.saveSchema();
        var label = schema.id
        sessionStorage.setItem(label, JSON.stringify(schema))

        window.open("pages/form/preview.html?FormSchemaID=" + label);
    };
    kmain.bindProcess = function () {
        window.open("pages/process/bind.html?FormID=" + kmain.mcurrentFormID);
    }

    kmain.loadFormPlayground = function (container, schema, data) {
        const playground = new FormPlayground({
            container: container,
            schema: schema,
            data: data,
        });
        return playground;
    }

    kmain.loadFormData = function (container, schema, data) {
        //clear form container content firstly
        $('#form').empty();

        const form = new Form({ container: container });

        form.importSchema(schema, data);

        // add event listeners
        form.on('submit', (event) => {
            console.log('Form <submit>', event);
            var error = event.errors;
            kprocessrun.saveFormData(event.data);
        });

        // provide a priority to event listeners
        form.on('changed', 500, (event) => {
            //console.log('Form <changed>', event);
            //var formData = event.data;
        });

        return form;
    }

    kmain.runProcess = function () {
        const schema = kmain.mxFormEditor.saveSchema();
        var label = schema.id
        sessionStorage.setItem(label, JSON.stringify(schema))

        window.open("pages/process/run.html?FormID=" + kmain.mcurrentFormID + "&FormSchemaID=" + label);
    }

    kmain.changeLang = function (lang) {
        kresource.setLang(lang);
        window.location.reload();
    }

    kmain.openHelpPage = function () {
        var win = null;
        var lang = kresource.getLang();
        if (lang === 'zh') {
            win = window.open("http://doc.slickflow.com/", '_blank');
        } else {
            win = window.open("http://doc.slickflow.net/", '_blank');
        }
        win.focus();
    }

    return kmain;
})()

export default kmain;


