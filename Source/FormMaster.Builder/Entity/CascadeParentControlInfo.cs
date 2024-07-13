using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public class CascadeParentControlInfo
    {
        public int EntityDefID { get; set; }
        public int AttributeID { get; set; }
        public string AttributeControlValue { get; set; }
    }
}