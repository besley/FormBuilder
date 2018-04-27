using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slickflow.Engine.Common;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 表单流程实体
    /// </summary>
    public class EntityAttrValueFlowItem
    {
        /// <summary>
        /// 实体基本信息
        /// </summary>
        public EntityInfoEntity EntityInfo { get; set; }

        /// <summary>
        /// 实体扩展属性数值列表
        /// </summary>
        public List<EntityAttrValueItem> EntityAttrValueList { get; set; }

        /// <summary>
        /// 流程执行对象
        /// </summary>
        public WfAppRunner AppRunner { get; set; }
    }
}
