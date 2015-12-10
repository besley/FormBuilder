using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlickMaster.Builder.Entity;
using SlickMaster.Builder.Manager;

namespace SlickMaster.Builder.Service
{
    /// <summary>
    /// 表单定制接口
    /// </summary>
    public interface IFBMasterService
    {
        //表单
        EntityDefEntity SaveEntityDef(EntityDefEntity entity);
        void DeleteEntityDef(int id);
        void SaveTemplateContent(EntityDefEntity entity);
        void SaveTemplateWithHTMLContent(EntityDefEntity entity);
        EntityDefEntity GetEntityDef(int id);
        List<EntityDefEntity> GetEntityDefList2();
        List<EntityDefView> GetEntityDefViewList();

        //字段
        List<EntityAttributeEntity> GetEntityAttributeList(int entityDefID);
        EntityAttributeEntity SaveAttribute(EntityAttributeEntity entity);
        EntityAttributeEntity SaveAttributeWithTemplate(EntityAttributeView view);
        bool DeleteAttributeWithTemplate(EntityAttributeListView view);

        //表单组合字段
        EntityAttributeListView GetEntityAttributeComp(int entityDefID);
    }
}
