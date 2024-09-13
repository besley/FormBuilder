import jquery from 'jquery';

var $ = require("jquery");
window.$ = $;
/*import $ from '/app/script/jquery-vendor.js'*/

import bootstrap from 'bootstrap'
import BootstrapDialog from 'bootstrap5-dialog'
window.BootstrapDialog = BootstrapDialog;

import agGrid from 'ag-grid-community'
window.agGrid = require('ag-grid-community');

import ztree from 'ztree'
//window.ztree = require('ztree');

import {
  debounce
} from 'min-dash';

import '../styles/app.less';

//import kmain js file
import kmain from '/app/viewjs/kmain.js'
window.kmain = kmain;

kmain.init();

import kprocessbind from '/app/viewjs/kprocessbind.js'
window.kprocessbind = kprocessbind;

import nextActivityTree from '/app/viewjs/nextactivitytree.js'
window.nextActivityTree = nextActivityTree;

//#region render functions and button event
// bootstrap diagram functions
$(function () {
    $('#js-create-form').click(function (e) {
        e.stopPropagation();
        e.preventDefault();

        kmain.createNewForm();
    });

    $('#js-open-form-list').click(function (e) {
        e.stopPropagation();
        e.preventDefault();

        kmain.openForm();
    });
});

$('#btnCreateForm').click(function (e) {
    e.stopPropagation();
    e.preventDefault();

    kmain.createNewForm();
});

$('#btnOpenForm').click(function (e) {
    e.stopPropagation();
    e.preventDefault();

    kmain.openForm();
});

$('#btnSaveForm').click(function (e) {
    e.stopPropagation();
    e.preventDefault();

    kmain.saveFormFile();
});

$('#btnHelp').click(function (e) {
    e.stopPropagation();
    e.preventDefault();

    kmain.openHelpPage();
});
//#endregion