using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormMaster.Builder.Common;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Service;

namespace FormMaster.Designer.Models
{
    /// <summary>
    /// EAV模型
    /// </summary>
    public class EavModel
    {
        /// <summary>
        /// 获取表单渲染HTML文本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EntityDefEntity GetEntityDef(int id)
        {
            var masterService = new FBMasterService();
            return masterService.GetEntityDef(id);
        }
    }
}