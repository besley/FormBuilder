using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 实体属性流程节点绑定信息
    /// </summary>
    [Table("FbFormFieldActivityEdit")]
    public class FormFieldActivityEditEntity
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string Version { get; set; }
        public int ProcessID { get; set; }
        public string ProcessGUID { get; set; }
        public string ProcessVersion { get; set; }
        public string ActivityGUID { get; set; }
        public string ActivityName { get; set; }
        public int FieldID { get; set; }
        public string FieldGUID { get; set; }
        public string FieldName { get; set; }
        public bool IsNotVisible { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
