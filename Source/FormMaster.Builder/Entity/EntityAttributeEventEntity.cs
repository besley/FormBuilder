using System;
using System.Collections.Generic;
using System.Text;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性事件绑定代码
    /// </summary>
    [Table("EavEntityAttributeEvent")]
    public class EntityAttributeEventEntity
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public int AttrID { get; set; }
        public string EventName { get; set; }
        public int IsDisabled { get; set; }
        public string CommandText { get; set; }
    }
}
