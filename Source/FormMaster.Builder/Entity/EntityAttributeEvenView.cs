using System;
using System.Collections.Generic;
using System.Text;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性事件绑定代码
    /// </summary>
    [Table("vw_EavEntityAttributeEvent")]
    public class EntityAttributeEventView
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public int AttrID { get; set; }
        public string DivCtrlKey { get; set; }
        public string AttrName { get; set; }
        public string AttrCode { get; set; }
        public string EventName { get; set; }
        public int IsDisabled { get; set; }
        public string CommandText { get; set; }
    }
}
