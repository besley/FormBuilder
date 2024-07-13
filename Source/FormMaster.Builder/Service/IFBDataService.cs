using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Manager;

namespace FormMaster.Builder.Service
{
    /// <summary>
    /// 表单数据接口
    /// </summary>
    public interface IFBDataService
    {
        //表单实例数据
        EntityInfoEntity GetEntityInfo(int id);
        EntityInfoEntity SaveEntityInfo(EntityInfoEntity entity);

        List<EntityInfoView> GetEntityInfoTop10();
        List<EntityInfoView> GetEntityInfoSimpleList(EntityInfoQuery query);
        List<EntityAttrValueItem> QueryEntityAttrValue(int entityInfoID);

        void DeleteEntityInfo(int id);

        //save entity info with attribute value
        List<dynamic> GetEntityInfoWithAttrValueListPaged(EntityInfoAttrQuery query, out int count);
        List<dynamic> QueryEntityInfoWithAttrValueListDynamic(EntityInfoAttrQuery query);
        List<dynamic> GetEntityInfoWithAttrValue(int entityInfoID);
        List<dynamic> GetEntityInfoWithAttrValueList(EntityInfoQuery query);

        int InsertRow(EntityInfoWithAttrValueListItem item);
        void UpdateRow(EntityInfoWithAttrValueListItem item);
        void DeleteRow(int entityInfoID);

        //control data source
        List<dynamic> LoadControlDataSource(EntityAttributeEntity entity);
        List<dynamic> LoadCascadeControlDataSource(CascadeControlInfo entity);
        List<EntityAttributeEntity> QueryCascadeChildControlList(CascadeParentControlInfo entity);
    }
}
