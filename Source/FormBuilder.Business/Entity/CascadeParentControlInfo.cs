using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 级联实体属性
    /// </summary>
    public class CascadeParentControlInfo
    {
        public int FormID { get; set; }
        public int FieldID { get; set; }
        public string FieldCode { get; set; }
        public string FieldControlValue { get; set; }
    }
}