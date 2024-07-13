using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 表单定义视图
    /// </summary>
    public class EntityDefProcessView
    {
        public int ID { get; set; }     //EntityDefID
        public string EntityTitle { get; set; }
        public string EntityName { get; set; }
        public string EntityCode { get; set; }
        public string TemplateContent { get; set; }
        public string HTMLContent { get; set; }
        public string MobileTemplateContent { get; set; }
        public int ProcessID { get; set; }
        public string ProcessGUID { get; set; }
        public string Version { get; set; }
        public string ProcessName { get; set; }
        public string ProcessCode { get; set; }
    }
}
