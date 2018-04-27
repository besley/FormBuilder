using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 属性字符类型取值
    /// </summary>
    [Table("EavEntityAttrVarchar")]
    public class EntityAttrVarcharEntity
    {
        public int ID { get; set; }
        public int EntityInfoID { get; set; }
        public int EntityDefID { get; set; }
        public int AttrID { get; set; }
        public int Value { get; set; }
    }
}
