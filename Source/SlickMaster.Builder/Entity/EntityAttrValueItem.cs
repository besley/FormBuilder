using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性取值对象
    /// </summary>
    public class EntityAttrValueItem
    {
        public int ID { get; set; }
        public int EntityInfoID { get; set; }
        public int EntityDefID { get; set; }
        public int AttrID { get; set; }
        public string AttrName { get; set; }
        public int AttrDataType { get; set; }
        public string Value { get; set; }
    }
}



