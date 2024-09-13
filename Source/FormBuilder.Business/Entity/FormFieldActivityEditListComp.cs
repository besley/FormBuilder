using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 字段权限列表实体
    /// </summary>
    public class FormFieldActivityEditListComp
    {
        public int FormID { get; set; }
        public int ProcessID { get; set; }
        public string ActivityGUID { get; set; }

        public string ActivityName { get; set; }
        public List<FormFieldActivityEditEntity> FieldEditList { get; set; }
    }
}
