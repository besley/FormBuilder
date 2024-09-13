using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Manager;

namespace FormBuilder.Business.Service
{
    /// <summary>
    /// 表单定制接口
    /// </summary>
    public interface IFormService
    {
        //表单
        FormEntity SaveForm(FormEntity entity);
        void DeleteForm(int formID);
        void SaveTemplateContent(FormEntity entity);
        void SaveTemplateWithHTMLContent(FormEntity entity);
        FormEntity GetForm(int formID);
        FormEntity GetForm(IDbConnection conn, int formID, IDbTransaction trans);
        List<FormEntity> GetFormList2();
        List<FormProcessView> GetFormViewList();
        void UpgradeForm(int formID);

        //表单绑定流程
        List<FormProcessEntity> GetFormProcess(int formID);
        void BindFormProcess(FormProcessView view);
        void UnbindFormProcess(FormProcessView view);
        void RemoveFormProcess(int formID);
        Boolean IsFormProcessBinding(int processID);

        //字段
        FormFieldEntity GetFormField(int attrID);
        FormFieldEntity GetFormFieldByGUID(FormFieldQuery query);
        List<FormFieldEntity> GetFormFieldList(int formID);
        List<FormFieldEntity> GetFormFieldList(IDbConnection conn, int formID, IDbTransaction trans);
        List<FormFieldEntity> GetFormFieldListOnlyInfoValue(int formID);

        FormFieldEntity SaveFormField(FormFieldEntity entity);
        void SaveTemplateWithFieldList(FormFieldListView view);
        Boolean DeleteFieldWithTemplate(FormFieldListView view);
        FormFieldEventEntity SaveFieldEvent(FormFieldEventEntity entity);
        List<FormFieldEventEntity> GetFormFieldEventList(int formID, int fieldID);
        List<FormFieldEventEntity> GetFormFieldEventListByForm(int formID);
        void DeleteFieldEvent(int eventID);
        void DeleteFieldEvent(int formID, int fieldID);
        Boolean DeleteComponent(FormFieldQuery query);

        //表单组合字段
        FormFieldListView GetFormFieldComp(int formID);

        //字段权限
        List<FormFieldActivityEditEntity> GetFormFieldActivityEditList(int formID, int processID, string activityGUID);
        List<FormFieldActivityEditEntity> GetFormFieldActivityEditList(int formID, string processGUID, string version, string activityGUID);
        List<FormFieldActivityEditEntity> GetFormFieldActivityEditList(int taskID);
        void SaveFormFieldActivityEditList(FormFieldActivityEditListComp comp);
        bool DeleteFormFieldActivityEdit(int formID, int processID, string activityID);
        bool ClearFormFieldActivityEdit(int formID, int processID);
    }
}
