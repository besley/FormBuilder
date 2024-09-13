using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public class CascadeControlInfo
    {
        public int FormID { get; set; }
        public string CascadeControlCode { get; set; }
        public string DataEntityName { get; set; }
        public string ParentControlValue { get; set; }
        public string CascadeFieldName { get; set; }
        public string ThisControlName { get; set; }
    }
}