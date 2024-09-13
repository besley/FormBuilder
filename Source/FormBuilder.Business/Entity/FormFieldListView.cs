using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 表单字段组合视图对象
    /// </summary>
    public class FormFieldListView
    {
        public FormEntity Form { get; set; }
        public List<FormFieldEntity> FormFieldList { get; set; }
    }
}
