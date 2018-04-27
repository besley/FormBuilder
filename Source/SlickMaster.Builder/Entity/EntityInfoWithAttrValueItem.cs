using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 实体对象封装
    /// </summary>
    public class EntityInfoWithAttrValueListItem
    {
        /// <summary>
        /// 实体基本信息
        /// </summary>
        public EntityInfoEntity EntityInfo { get; set; }

        /// <summary>
        /// 实体扩展属性数值列表
        /// </summary>
        public List<EntityAttrValueItem> EntityAttrValueList { get; set; }
    }
}
