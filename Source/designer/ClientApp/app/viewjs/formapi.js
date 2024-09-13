const formapi = (function () {
    function formapi() {
    }
    formapi.saveFormSchema = function (schema) {
        var entity = {};
        var currentFormEntity = kmain.mxSelectedFormEntity;

        if (currentFormEntity) {
            entity.ID = currentFormEntity.ID;
        }
        else {
            entity.FormCode = jshelper.getRandomString(6);
            entity.Version = "1";
        }
        entity.FormName = schema.id;
        entity.TemplateContent = JSON.stringify(schema);
        jshelper.ajaxPost(kconfig.webApiUrl + 'api/Form/SaveTemplate',
            JSON.stringify(entity),
            function (result) {
                if (result.Status === 1) {
                    kmsgbox.info(kresource.getItem('Form.Template.Save.ok'));
                } else {
                    kmsgbox.error(result.Message);
                }
            });
    }
    return formapi;
})()

export default formapi;
