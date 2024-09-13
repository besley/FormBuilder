using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 表单和字段视图
    /// </summary>
    public class FormFieldView
    {
        public FormEntity Form { get; set; }
        public List<FormFieldEntity> FormFieldList { get; set; }
    }
}
