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
    /// 表单数据接口
    /// </summary>
    public interface IFBDataService
    {
        //表单数据
        List<EntityInfoView> GetEntityInfoTop10();
        List<EntityAttrValueItem> QueryEntityAttrValue(int entityInfoID);
        void DeleteEntityInfo(int id);
        //save entity info with attribute value
        List<dynamic> GetEntityInfoWithAttrValueListPaged(EntityInfoAttrQuery query, out int count);
        List<dynamic> GetEntityInfoWithAttrValue(int entityInfoID);
        List<dynamic> GetEntityInfoWithAttrValueList(EntityInfoQuery query);
        int InsertRow(EntityInfoWithAttrValueListItem item);
        void UpdateRow(EntityInfoWithAttrValueListItem item);
        void DeleteRow(int entityInfoID);
        void InsertRowFlow(EntityAttrValueFlowItem item);
        
    }
}
