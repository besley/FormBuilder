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
    public class CascadeControlInfo
    {
        public int EntityDefID { get; set; }
        public string CascadeControlID { get; set; }
        public string DataEntityName { get; set; }
        public string ParentControlValue { get; set; }
        public string CascadeFieldName { get; set; }
        public string ThisControlName { get; set; }
    }
}