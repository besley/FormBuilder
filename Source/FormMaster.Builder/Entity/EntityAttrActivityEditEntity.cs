using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性流程节点绑定信息
    /// </summary>
    [Table("EavEntityAttrActivityEdit")]
    public class EntityAttrActivityEditEntity
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public int ProcessID { get; set; }
        public string ProcessGUID { get; set; }
        public string Version { get; set; }
        public string ActivityGUID { get; set; }
        public int AttrID { get; set; }
        public string AttrName { get; set; }
        public byte IsNotVisible { get; set; }
        public byte IsReadOnly { get; set; }
    }
}
