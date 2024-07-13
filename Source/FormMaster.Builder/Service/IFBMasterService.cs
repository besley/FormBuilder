using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Manager;

namespace FormMaster.Builder.Service
{
    /// <summary>
    /// 表单定制接口
    /// </summary>
    public interface IFBMasterService
    {
        //表单
        EntityDefEntity SaveEntityDef(EntityDefEntity entity);
        void DeleteEntityDef(int entityDefID);
        void SaveTemplateContent(EntityDefEntity entity);
        void SaveTemplateWithHTMLContent(EntityDefEntity entity);
        EntityDefEntity GetEntityDef(int entityDefID);
        EntityDefEntity GetEntityDef(IDbConnection conn, int entityDefID, IDbTransaction trans);
        List<EntityDefEntity> GetEntityDefList2();
        List<EntityDefProcessView> GetEntityDefViewList();
        void UpgradeEntityDef(int entityDefID);

        //表单绑定流程
        EntityProcessEntity GetEntityProcess(int entityDefID);
        EntityDefProcessView GetEntityDefProcessView(int entityDefID);
        void BindEntityProcess(EntityDefProcessView view);
        void UnbindEntityProcess(EntityDefProcessView view);
        void RemoveEntityProcess(int entityDefID);
        Boolean IsEntityDefProcessBinding(int processID);

        //字段
        List<EntityAttributeEntity> GetEntityAttributeList(int entityDefID);
        List<EntityAttributeEntity> GetEntityAttributeList(IDbConnection conn, int entityDefID, IDbTransaction trans);
        List<EntityAttributeEntity> GetEntityAttributeListOnlyInfoValue(int entityDefID);
        EntityAttributeEntity GetEntityAttribute(int attrID);
        EntityAttributeEntity SaveAttribute(EntityAttributeEntity entity);
        EntityAttributeEntity SaveAttributeWithTemplate(EntityAttributeView view);
        Boolean DeleteAttributeWithTemplate(EntityAttributeListView view);
        EntityAttributeEventEntity SaveAttributeEvent(EntityAttributeEventEntity entity);
        List<EntityAttributeEventView> GetEntityAttributeEventList(int entityDefID, int attrID);
        List<EntityAttributeEventView> GetEntityAttributeEventListByForm(int entityDefID);
        void DeleteAttributeEvent(int eventID);
        void DeleteAttributeEvent(int entityDefID, int attrID);

        //表单组合字段
        EntityAttributeListView GetEntityAttributeComp(int entityDefID);

        //字段权限
        List<EntityAttrActivityEditEntity> GetEntityAttrActivityEditList(int entityDefID, int processID, string activityGUID);
        List<EntityAttrActivityEditEntity> GetEntityAttrActivityEditList(int entityDefID, string processGUID, string version, string activityGUID);
        void SaveEntityAttrActivityEditList(int processID, List<EntityAttrActivityEditEntity> editList);
        bool DeleteEntityAttrActivityEdit(int entityDefID, int processID, string activityID);
        bool ClearEntityAttrActivityEdit(int entityDefID);
    }
}
