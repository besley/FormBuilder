using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 表单定义视图
    /// </summary>
    public class FormProcessView
    {
        public int FormID { get; set; }     //FormID
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public string Version { get; set; }
        public string TemplateContent { get; set; }
        public string HTMLContent { get; set; }
        public int ProcessID { get; set; }
        public string ProcessGUID { get; set; }
        public string ProcessVersion { get; set; }
        public string ProcessName { get; set; }
        public string ProcessCode { get; set; }
    }
}
