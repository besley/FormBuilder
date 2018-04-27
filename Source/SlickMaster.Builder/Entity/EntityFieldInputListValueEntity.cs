using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 输入字段列表取值
    /// </summary>
    public class EntityFieldInputListValueEntity
    {
        public int ID { get; set; }
        public int EntityID { get; set; }
        public int AttrID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
