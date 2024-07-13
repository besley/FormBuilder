using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 表单字段组合视图对象
    /// </summary>
    public class EntityAttributeListView
    {
        public EntityDefEntity EntityDef { get; set; }
        public List<EntityAttributeEntity> EntityAttributeList { get; set; }
    }
}
