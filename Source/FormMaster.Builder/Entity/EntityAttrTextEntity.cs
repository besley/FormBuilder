using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 属性大文本取值
    /// </summary>
    [Table("EavEntityAttrText")]
    public class EntityAttrTextEntity
    {
        public int ID { get; set; }
        public int EntityInfoID { get; set; }
        public int EntityDefID { get; set; }
        public int AttrID { get; set; }
        public int Value { get; set; }
    }
}
