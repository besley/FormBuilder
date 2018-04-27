using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SlickMaster.Builder.Common;
using SlickMaster.Builder.Entity;
using SlickMaster.Builder.Service;

namespace SlickMaster.Designer.Models
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