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
    public class FormFieldQuery
    {
        public int FormID { get; set; }
        public string FieldGUID { get; set; }
    }
}