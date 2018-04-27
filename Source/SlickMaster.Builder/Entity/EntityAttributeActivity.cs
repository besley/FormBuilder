using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性流程节点绑定信息
    /// </summary>
    public class EntityAttributeActivity
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public string ProcessGUID { get; set; }
        public string ActivityGUID { get; set; }
        public byte IsEdited { get; set; }
        public byte IsInvisible { get; set; }
    }
}
