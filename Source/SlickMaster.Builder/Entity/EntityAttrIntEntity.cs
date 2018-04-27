using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 属性整数取值
    /// </summary>
    [Table("EavEntityAttrInt")]
    public class EntityAttrIntEntity
    {
        public int ID { get; set; }
        public int EntityInfoID { get; set; }
        public int EntityDefID { get; set; }
        public int AttrID { get; set; }
        public int Value { get; set; }
    }
}
