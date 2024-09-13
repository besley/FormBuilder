using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slickflow.Engine.Common;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 表单数据流程用户对象
    /// </summary>
    public class FormDataProcessRunner
    {
        public string ProcessGUID { get; set; }
        public string Version { get; set; }
        public int FormDataID { get; set; }
        public string FormID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
