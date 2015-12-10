using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 表单定义视图
    /// </summary>
    public class EntityDefView
    {
        public int ID { get; set; }
        public string EntityTitle { get; set; }
        public string EntityName { get; set; }
        public string EntityCode { get; set; }
        public string ProcessGUID { get; set; }
    }
}
