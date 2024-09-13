using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 实体属性取值对象
    /// </summary>
    public class FormFieldValueItem
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public int FormInfoID { get; set; }
        public int FieldID { get; set; }
        public string FieldName { get; set; }
        public int FieldDataType { get; set; }
        public string DataValueField { get; set; }
        public string DataTextField { get; set; }
        public string Value { get; set; }
    }
}



