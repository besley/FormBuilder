var kcommon = (function () {
    function kcommon() {
    }

    kcommon.dataTypeDict = {};
    kcommon.inputTypeDict = {};

    kcommon.dataTypeDict["VARCHAR"] = 1;
    kcommon.dataTypeDict["INT"] = 2;
    kcommon.dataTypeDict["DECIMAL"] = 3;
    kcommon.dataTypeDict["DATE"] = 4;
    kcommon.dataTypeDict["TEXT"] = 5;

    kcommon.inputTypeDict["TEXTBOX"] = 1;
    kcommon.inputTypeDict["PASSWORD"] = 2;
    kcommon.inputTypeDict["COMBOBOX"] = 3;
    kcommon.inputTypeDict["CHECKBOXGROUP"] = 4;
    kcommon.inputTypeDict["RADIOGROUP"] = 5;
    kcommon.inputTypeDict["MULTIPLELIST"] = 6;
    kcommon.inputTypeDict["DATE"] = 7;
    kcommon.inputTypeDict["TEXT"] = 8;
    kcommon.inputTypeDict["BUTTON"] = 16;
    kcommon.inputTypeDict["IMAGE"] = 17;

    return kcommon;
})();