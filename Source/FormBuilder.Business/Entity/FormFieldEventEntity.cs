using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 实体属性事件绑定代码
    /// </summary>
    [Table("FbFormFieldEvent")]
    public class FormFieldEventEntity
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public int FieldID { get; set; }
        public string EventName { get; set; }
        public string EventArguments { get; set; }
        public int IsDisabled { get; set; }
        public string CommandText { get; set; }
    }
}
